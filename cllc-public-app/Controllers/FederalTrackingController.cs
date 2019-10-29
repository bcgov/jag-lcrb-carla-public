using CsvHelper;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ClassMaps;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System.Collections.Generic;
using System.IO;
using System;
using Gov.Lclb.Cllb.Public.Models;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FederalTrackingController : ControllerBase
    {
        private readonly string DOCUMENT_LIBRARY = "Federal Reporting";
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IConfiguration _configuration;
        private readonly SharePointFileManager _sharepoint;
        private readonly ILogger _logger;

        public FederalTrackingController(IDynamicsClient dynamicsClient, IConfiguration configuration, ILoggerFactory loggerFactory, SharePointFileManager sharepoint)
        {
            _dynamicsClient = dynamicsClient;
            _configuration = configuration;
            _sharepoint = sharepoint;
            _logger = loggerFactory.CreateLogger(typeof(FederalTrackingController));
        }


        /// <summary>
        /// Generate a csv with the federal tracking report for a given reporting period
        /// </summary>
        /// <returns></returns>
        [HttpGet("generate")]
        public IActionResult GenerateFederalTrackingReport()
        {
            if (_configuration["FEATURE_FEDERAL_CSV"] == null)
            {
                return new NotFoundResult();
            }

            try
            {
                CannabismonthlyreportsGetResponseModel previousReport = _dynamicsClient.Cannabismonthlyreports.Get(top: 1, orderby: new List<string> {"adoxio_csvexportid desc"});
                int currentExportId = (previousReport.Value.Count > 0) ? previousReport.Value[0].AdoxioCsvexportid + 1 : 1;

                // Submitted reports
                string filter = $"statuscode eq {(int)MonthlyReportStatus.Submitted}";
                CannabismonthlyreportsGetResponseModel dynamicsMonthlyReports = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter);
                List<FederalTrackingMonthlyExport> monthlyReports = new List<FederalTrackingMonthlyExport>();
                foreach (MicrosoftDynamicsCRMadoxioCannabismonthlyreport report in dynamicsMonthlyReports.Value)
                {
                    FederalTrackingMonthlyExport export = new FederalTrackingMonthlyExport()
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
                    CannabisinventoryreportsGetResponseModel invResp = _dynamicsClient.Cannabisinventoryreports.Get(filter: filter);
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
                if (monthlyReports.Count > 0)
                {
                    string filePath = "";
                    using (var mem = new MemoryStream())
                    using (var writer = new StreamWriter(mem))
                    using (var csv = new CsvWriter(writer))
                    {
                        csv.Configuration.RegisterClassMap<FederalTrackingMonthlyExportMap>();
                        csv.WriteRecords(monthlyReports);

                        writer.Flush();
                        mem.Position = 0;
                        string filename = $"{currentExportId.ToString("0000")}_{DateTime.Now.ToString("yyy-MM-dd")}-CannabisTrackingReport.csv";
                        bool result = _sharepoint.UploadFile(filename, DOCUMENT_LIBRARY, "", mem, "text/csv").GetAwaiter().GetResult();
                        string url = _sharepoint.GetServerRelativeURL(DOCUMENT_LIBRARY, "");
                        filePath = _configuration["SHAREPOINT_NATIVE_BASE_URI"] + url + filename;
                    }

                    return new JsonResult(new Dictionary<string, string>{
                        { "file", filePath },
                        { "csvexportid", currentExportId.ToString("0000") },
                        { "count", dynamicsMonthlyReports.Value.Count.ToString() }
                    });
                }
                return new JsonResult(new Dictionary<string, string>{
                    { "count", dynamicsMonthlyReports.Value.Count.ToString() }
                });
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating federal tracking CSV");
                return new BadRequestResult();
            }
            catch (SharePointRestException e)
            {
                _logger.LogError(e, "Error saving csv to sharepoint");
                return new BadRequestResult();
            }
        }
    }
}
