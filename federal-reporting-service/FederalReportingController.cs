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

        public async Task ExportFederalReports(PerformContext hangfireContext)
        {
            // Get any new exports that have been created
            string filter = "adoxio_exportcompleted eq null";
            MicrosoftDynamicsCRMadoxioFederalreportexportCollection exports = _dynamicsClient.Federalreportexports.Get(filter: filter);
            if (exports.Value.Count > 0)
            {
                string exportId = exports.Value.FirstOrDefault().AdoxioFederalreportexportid;
                MicrosoftDynamicsCRMadoxioFederalreportexport export = new MicrosoftDynamicsCRMadoxioFederalreportexport();
                export.AdoxioExporttriggered = DateTime.UtcNow;
                _dynamicsClient.Federalreportexports.Update(exportId, export);
                try
                {
                    // Gather submitted reports
                    filter = $"statuscode eq {(int)MonthlyReportStatus.Submitted}";
                    var dynamicsMonthlyReports = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter);
                    List<FederalReportingMonthlyExport> monthlyReports = new List<FederalReportingMonthlyExport>();
                    foreach (MicrosoftDynamicsCRMadoxioCannabismonthlyreport report in dynamicsMonthlyReports.Value)
                    {
                        FederalReportingMonthlyExport exportVM = new FederalReportingMonthlyExport()
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

                        // Get inventory reports for those submitted reports
                        filter = $"_adoxio_monthlyreportid_value eq {report.AdoxioCannabismonthlyreportid}";
                        var invResp = _dynamicsClient.Cannabisinventoryreports.Get(filter: filter);
                        foreach (MicrosoftDynamicsCRMadoxioCannabisinventoryreport inventoryReport in invResp.Value)
                        {
                            MicrosoftDynamicsCRMadoxioCannabisproductadmin product = _dynamicsClient.Cannabisproductadmins.GetByKey(inventoryReport._adoxioProductidValue);
                            exportVM.PopulateProduct(inventoryReport, product);
                        }
                        monthlyReports.Add(exportVM);

                        MicrosoftDynamicsCRMadoxioCannabismonthlyreport patchRecord = new MicrosoftDynamicsCRMadoxioCannabismonthlyreport()
                        {
                            // AdoxioCsvexportdate = DateTime.UtcNow,
                            // AdoxioCsvexportid = currentExportId,
                            Statuscode = (int)MonthlyReportStatus.Closed
                        };
                        _dynamicsClient.Cannabismonthlyreports.Update(report.AdoxioCannabismonthlyreportid, patchRecord);

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
                                string filename = $"{export.AdoxioExportnumber}_{DateTime.Now.ToString("yyy-MM-dd")}-CannabisTrackingReport.csv";
                                string sharepointFilename = await _sharepoint.UploadFile(filename, DOCUMENT_LIBRARY, "", mem, "text/csv");
                                string url = _sharepoint.GetServerRelativeURL(DOCUMENT_LIBRARY, "");
                            }
                            hangfireContext.WriteLine($"Successfully exported Federal Reporting CSV {export.AdoxioExportnumber}.");
                            _logger.LogInformation($"Successfully exported Federal Reporting CSV {export.AdoxioExportnumber}.");
                        }
                    }
                    export.AdoxioExportcompleted = DateTime.UtcNow;
                    _dynamicsClient.Federalreportexports.Update(exportId, export);
                }
                catch (HttpOperationException httpOperationException)
                {
                    hangfireContext.WriteLine("Error creating federal tracking CSV");
                    _logger.LogError(httpOperationException, "Error creating federal tracking CSV");
                }
            }
        }
    }
}
