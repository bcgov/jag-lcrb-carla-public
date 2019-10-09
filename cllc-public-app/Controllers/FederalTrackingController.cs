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

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpGet("{month}/{year}")]
        public IActionResult GenerateFederalTrackingReport(int month, int year)
        {
            if (month < 1 || month > 12 || year < 2018)
            {
                return new BadRequestResult();
            }

            string monthStr = month.ToString("00");
            string yearStr = year.ToString();

            string filter = $"adoxio_reportingperiodmonth eq '{monthStr}' and adoxio_reportingperiodyear eq '{yearStr}'";
            try
            {
                CannabismonthlyreportsGetResponseModel resp = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter);
                List<FederalTrackingMonthlyExport> monthlyReports = new List<FederalTrackingMonthlyExport>();
                foreach (MicrosoftDynamicsCRMadoxioCannabismonthlyreport report in resp.Value)
                {
                    FederalTrackingMonthlyExport export = new FederalTrackingMonthlyExport()
                    {
                        ReportingPeriodMonth = monthStr,
                        ReportingPeriodYear = yearStr,
                        RetailerDistributor = report.AdoxioRetailerdistributor?.ToString() ?? "1",
                        //TBR
                        CompanyName = report.AdoxioLicenseeId?.ToString(),
                        //TBR
                        SiteID = "BC" + report.AdoxioLicencenumber,
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
                }

                string filePath = "";
                using (var mem = new MemoryStream())
                using (var writer = new StreamWriter(mem))
                using (var csv = new CsvWriter(writer))
                {
                    csv.Configuration.RegisterClassMap<FederalTrackingMonthlyExportMap>();
                    csv.WriteRecords(monthlyReports);

                    writer.Flush();
                    mem.Position = 0;
                    string filename = $"{yearStr}-{monthStr}-CannabisTrackingReport.csv";
                    bool result = _sharepoint.UploadFile(filename, DOCUMENT_LIBRARY, "", mem, "text/csv").GetAwaiter().GetResult();
                    string url = _sharepoint.GetServerRelativeURL(DOCUMENT_LIBRARY, "");
                    filePath = _configuration["SHAREPOINT_NATIVE_BASE_URI"] + url + filename;
                }

                return new JsonResult(new Dictionary<string, string>{
                    { "file", filePath },
                    { "count", resp.Value.Count.ToString() },
                    { "month", month.ToString() },
                    { "year", year.ToString() }
                });
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error querying federal tracking reports");
                return new BadRequestResult();
            }
            catch (SharePointRestException e)
            {
                _logger.LogError(e, "Error saving csv to sharepoint");
                return new BadRequestResult();
            }
        }

        /// <summary>
        /// Get monthly report
        /// </summary>
        /// <returns></returns>
        // [HttpGet("{month}/{year}")]
        // public IActionResult GetMonthlyReport(int month, int year)
        // {
        //     if(month < 1 || month > 12 || year < 2018)
        //     {
        //         return new BadRequestResult();
        //     }

        //     string monthStr = month.ToString("00");
        //     string yearStr = year.ToString();
            
        //     string filter = $"adoxio_reportingperiodmonth eq '{monthStr}' and adoxio_reportingperiodyear eq '{yearStr}'";
            
        //     try
        //     {
        //         CannabismonthlyreportsGetResponseModel resp = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter);
        //     }
        // }
    }
}
