using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ClassMaps;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FederalTrackingController : ControllerBase
    {
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public FederalTrackingController(IDynamicsClient dynamicsClient, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _dynamicsClient = dynamicsClient;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger(typeof(FederalTrackingController));
        }


        /// <summary>
        /// Get a csv with the federal tracking report for a given reporting period
        /// </summary>
        /// <returns></returns>
        [HttpGet("{month}/{year}")]
        public IActionResult GetFederalTrackingReport(int month, int year)
        {
            if(month < 1 || month > 12 || year < 2018)
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
                        RetailerDistributor = report.AdoxioRetailerdistributor?.ToString(),
                        CompanyName = report.AdoxioLicenseeId?.ToString(),
                        SiteID = "BC"+report.AdoxioLicencenumber,
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
                        if(product.AdoxioName == "Dried Cannabis")
                        {
                            export.DriedCannabisPackagedOpeningInventory = inventoryReport.AdoxioOpeninginventory ?? 0;
                            export.DriedCannabisPackagedAdditionsReceivedDomestic = inventoryReport.AdoxioQtyreceiveddomestic ?? 0;
                            export.DriedCannabisPackagedAdditionsReceivedReturned = inventoryReport.AdoxioQtyreceivedreturns ?? 0;
                            export.DriedCannabisPackagedAdditionsOther = inventoryReport.AdoxioQtyreceivedother ?? 0;
                            export.DriedCannabisPackagedReductionsShippedDomestic = inventoryReport.AdoxioQtyshippeddomestic ?? 0;
                            export.DriedCannabisPackagedReductionsShippedReturned = inventoryReport.AdoxioQtyshippedreturned ?? 0;
                            export.DriedCannabisPackagedReductionsDestroyed = inventoryReport.AdoxioQtydestroyed ?? 0;
                            export.DriedCannabisPackagedReductionsLostStolen = inventoryReport.AdoxioQtyloststolen ?? 0;
                            export.DriedCannabisPackagedReductionsOther = inventoryReport.AdoxioOtherreductions ?? 0;
                            export.DriedCannabisPackagedClosingInventory = inventoryReport.AdoxioClosinginventory ?? 0;
                            export.DriedCannabisPackagedClosingInventoryValue = (double)inventoryReport.AdoxioValueofclosinginventory;
                            export.DriedCannabisPackagedClosingInventoryWeight = (double)inventoryReport.AdoxioWeightofclosinginventory;
                            export.DriedCannabisPackagedUnitsSold = inventoryReport.AdoxioNumberpackagedunits ?? 0;
                            export.DriedCannabisTotalValueSold = (double)inventoryReport.AdoxioTotalvalue;
                        }
                    }
                    monthlyReports.Add(export);
                }
                using (var writer = new StreamWriter("/tmp/test.csv"))
                using (var csv = new CsvWriter(writer))
                {
                    csv.Configuration.RegisterClassMap<FederalTrackingMonthlyExportMap>();
                    csv.WriteRecords(monthlyReports);
                }

                return new JsonResult(new Dictionary<string, int>{
                    { "found", resp.Value.Count },
                    { "month", month },
                    { "year", year }
                });
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error querying federal tracking reports");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }
    }
}
