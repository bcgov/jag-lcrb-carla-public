using CsvHelper;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using System.Linq;

namespace Gov.Lclb.Cllb.FederalReportingService
{
    public class FederalReportingController
    {
        private readonly string DOCUMENT_LIBRARY = "Federal Reporting";
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IConfiguration _configuration;
        private readonly SharePointFileManager _sharepoint;
        private readonly ILogger _logger;

        public FederalReportingController(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            if (_configuration["DYNAMICS_ODATA_URI"] != null)
            {
                _dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            }
            if (_configuration["SHAREPOINT_ODATA_URI"] != null)
            {
                _sharepoint = new SharePointFileManager(_configuration);
            }
            _logger = loggerFactory.CreateLogger(typeof(FederalReportingController));
        }


        /// <summary>
        /// Generate a csv with the federal tracking report for a given reporting period
        /// </summary>
        /// <returns></returns>
        public async Task GenerateFederalTrackingReport(PerformContext hangfireContext)
        {
            try
            {
                MicrosoftDynamicsCRMadoxioCannabismonthlyreport previousReport = _dynamicsClient.Cannabismonthlyreports.Get(top: 1, orderby: new List<string> {"adoxio_csvexportid desc"}).Value.FirstOrDefault();
                int currentExportId = (previousReport != null && previousReport.AdoxioCsvexportid != null) ? (int)previousReport.AdoxioCsvexportid + 1 : 1;

                // Submitted reports
                string filter = $"statuscode eq {(int)MonthlyReportStatus.Submitted}";
                var dynamicsMonthlyReports = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter);
                List<FederalReportingMonthlyExport> monthlyReports = new List<FederalReportingMonthlyExport>();
                foreach (MicrosoftDynamicsCRMadoxioCannabismonthlyreport report in dynamicsMonthlyReports.Value)
                {
                    FederalReportingMonthlyExport export = new FederalReportingMonthlyExport()
                    {
                        ReportingPeriodMonth = report.AdoxioReportingperiodmonth,
                        ReportingPeriodYear = report.AdoxioReportingperiodyear,
                        RetailerDistributor = report.AdoxioRetailerdistributor?.ToString() ?? "1",
                        CompanyName = report.AdoxioEstablishmentnametext,
                        SiteID = report.AdoxioSiteidnumber,
                        City = report.AdoxioCity,
                        PostalCode = report.AdoxioPostalcode,
                        ManagementEmployees = report.AdoxioEmployeesmanagement ?? 0,
                        AdministrativeEmployees = report.AdoxioEmployeesadministrative ?? 0,
                        SalesEmployees = report.AdoxioEmployeessales ?? 0,
                        ProductionEmployees = report.AdoxioEmployeesproduction ?? 0,
                        OtherEmployees = report.AdoxioEmployeesother ?? 0
                    };

                    filter = $"_adoxio_monthlyreportid_value eq {report.AdoxioCannabismonthlyreportid}";
                    var invResp = _dynamicsClient.Cannabisinventoryreports.Get(filter: filter);
                    foreach (MicrosoftDynamicsCRMadoxioCannabisinventoryreport inventoryReport in invResp.Value)
                    {
                        MicrosoftDynamicsCRMadoxioCannabisproductadmin product = _dynamicsClient.Cannabisproductadmins.GetByKey(inventoryReport._adoxioProductidValue);
                        export.PopulateProduct(inventoryReport, product);
                    }
                    monthlyReports.Add(export);

                    MicrosoftDynamicsCRMadoxioCannabismonthlyreport patchRecord = new MicrosoftDynamicsCRMadoxioCannabismonthlyreport()
                    {
                        AdoxioCsvexportdate = DateTime.UtcNow,
                        AdoxioCsvexportid = currentExportId,
                        Statuscode = (int)MonthlyReportStatus.Closed
                    };
                    _dynamicsClient.Cannabismonthlyreports.Update(report.AdoxioCannabismonthlyreportid, patchRecord);
                    
                }
                hangfireContext.WriteLine($"Found {monthlyReports.Count} monthly reports to export.");
                _logger.LogInformation($"Found {monthlyReports.Count} monthly reports to export.");
                if (monthlyReports.Count > 0)
                {
                    string filePath = "";
                    using (var mem = new MemoryStream())
                    using (var writer = new StreamWriter(mem))
                    using (var csv = new CsvWriter(writer))
                    {
                        csv.Configuration.RegisterClassMap<FederalReportingMonthlyExportMap>();
                        csv.WriteRecords(monthlyReports);

                        writer.Flush();
                        mem.Position = 0;
                        string filename = $"{currentExportId.ToString("0000")}_{DateTime.Now.ToString("yyy-MM-dd")}-CannabisTrackingReport.csv";
                        bool result = _sharepoint.UploadFile(filename, DOCUMENT_LIBRARY, "", mem, "text/csv").GetAwaiter().GetResult();
                        string url = _sharepoint.GetServerRelativeURL(DOCUMENT_LIBRARY, "");
                    }
                    hangfireContext.WriteLine($"Successfully exported Federal Reporting CSV {currentExportId}.");
                    _logger.LogInformation($"Successfully exported Federal Reporting CSV {currentExportId}.");
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                hangfireContext.WriteLine("Error creating federal tracking CSV");
                _logger.LogError(httpOperationException, "Error creating federal tracking CSV");
            }
            catch (SharePointRestException e)
            {
                hangfireContext.WriteLine("Error saving csv to sharepoint");
                _logger.LogError(e, "Error saving csv to sharepoint");
            }
        }
    }
}
