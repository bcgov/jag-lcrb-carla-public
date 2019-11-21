using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class MonthlyReportsController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public MonthlyReportsController(IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration, ILoggerFactory loggerFactory, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger(typeof(MonthlyReportsController));
        }

        private List<MonthlyReport> GetMonthlyReportsByUser(string licenceeId)
        {
            List<MonthlyReport> monthlyReportsList = new List<MonthlyReport>();
            IEnumerable<MicrosoftDynamicsCRMadoxioCannabismonthlyreport> monthlyReports;
            if (string.IsNullOrEmpty(licenceeId))
            {
                monthlyReports = _dynamicsClient.Cannabismonthlyreports.Get().Value;
            }
            else
            {
                try
                {
                    var filter = $"_adoxio_licenseeid_value eq {licenceeId}";
                    monthlyReports = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter, orderby: new List<string> { "modifiedon desc" }).Value;
                }
                catch (HttpOperationException)
                {
                    monthlyReports = null;
                }
            }

            if (monthlyReports != null)
            {
                foreach (var monthlyReport in monthlyReports)
                {
                    monthlyReportsList.Add(monthlyReport.ToViewModel(_dynamicsClient));
                }
            }

            return monthlyReportsList;
        }

        /// <summary>
        /// Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
        private bool CurrentUserHasAccessToMonthlyReportOwnedBy(string accountId)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // For now, check if the account id matches the user's account.
            // TODO there may be some account relationships in the future
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return userSettings.AccountId == accountId;
            }

            // if current user doesn't have an account they are probably not logged in
            return false;
        }

        /// GET all monthly reports in Dynamics by Licence filtered by the current user's licencee
        [HttpGet("licence/{licenceId}")]
        public IActionResult GetMonthlyReportsByLicence(string licenceId)
        {
            if (_configuration["FEATURE_FEDERAL_REPORTING"] == null)
            {
                return new NotFoundResult();
            }
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            List<MonthlyReport> monthlyReportsList = new List<MonthlyReport>();
            IEnumerable<MicrosoftDynamicsCRMadoxioCannabismonthlyreport> monthlyReports;
            try
            {
                var filter = $"_adoxio_licenceid_value eq {licenceId} and _adoxio_licenseeid_value eq {userSettings.AccountId}";
                monthlyReports = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter, orderby: new List<string> { "modifiedon desc" }).Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying monthly reports");
                monthlyReports = null;
            }

            if (monthlyReports != null)
            {
                foreach (var monthlyReport in monthlyReports)
                {
                    monthlyReportsList.Add(monthlyReport.ToViewModel(_dynamicsClient));
                }
            }

            return new JsonResult(monthlyReportsList);
        }


        /// GET all monthly reports in Dynamics by Licencee using the account Id assigned to the user logged in
        [HttpGet("current")]
        public IActionResult GetCurrentUserMonthlyReports()
        {
            if (_configuration["FEATURE_FEDERAL_REPORTING"] == null)
            {
                return new NotFoundResult();
            }
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // get all licenses in Dynamics by Licencee using the account Id assigned to the user logged in
            List<MonthlyReport> monthlyReports = GetMonthlyReportsByUser(userSettings.AccountId);

            return new JsonResult(monthlyReports);
        }

        /// GET monthly report by id in Dynamics by if owned by user
        [HttpGet("{reportId}")]
        public IActionResult GetMonthlyReport(string reportId)
        {
            if (_configuration["FEATURE_FEDERAL_REPORTING"] == null)
            {
                return new NotFoundResult();
            }
            try
            {
                var filter = $"adoxio_cannabismonthlyreportid eq {reportId}";
                MicrosoftDynamicsCRMadoxioCannabismonthlyreport monthlyReport = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter).Value.FirstOrDefault();
                if (monthlyReport != null && CurrentUserHasAccessToMonthlyReportOwnedBy(monthlyReport._adoxioLicenseeidValue))
                {
                    return new JsonResult(monthlyReport.ToViewModel(_dynamicsClient));
                }
            }
            catch (HttpOperationException ex)
            {
                _logger.LogError(ex, "Error getting cannabis monthly report");
            }
            return new NotFoundResult();
        }

        /// PUT update monthly report by id
        [HttpPut("{id}")]
        public IActionResult UpdateMonthlyReport([FromBody] ViewModels.MonthlyReport item, string id)
        {
            if (item != null && id != item.monthlyReportId)
            {
                return BadRequest();
            }

            // for association with current user
            string userJson = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(userJson);

            Guid monthlyReportId = new Guid(id);
            string filter = $"adoxio_cannabismonthlyreportid eq {id}";
            var monthlyReportResp = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter);
            if (monthlyReportResp.Value.Count < 1 || !CurrentUserHasAccessToMonthlyReportOwnedBy(monthlyReportResp.Value[0]._adoxioLicenseeidValue))
            {
                return new NotFoundResult();
            }

            try
            {
                // Update monthly report
                MicrosoftDynamicsCRMadoxioCannabismonthlyreport monthlyReport = new MicrosoftDynamicsCRMadoxioCannabismonthlyreport()
                {
                    AdoxioEmployeesmanagement = item.employeesManagement,
                    AdoxioEmployeesadministrative = item.employeesAdministrative,
                    AdoxioEmployeessales = item.employeesSales,
                    AdoxioEmployeesproduction = item.employeesProduction,
                    AdoxioEmployeesother = item.employeesOther,
                    Statuscode = item.statusCode
                };
                _dynamicsClient.Cannabismonthlyreports.Update(item.monthlyReportId, monthlyReport);

                // Update inventory reports
                if (item.inventorySalesReports != null && item.inventorySalesReports.Count > 0) {
                  foreach (InventorySalesReport invReport in item.inventorySalesReports)
                  {
                      MicrosoftDynamicsCRMadoxioCannabisinventoryreport updateReport = new MicrosoftDynamicsCRMadoxioCannabisinventoryreport()
                      {
                          AdoxioOpeninginventory = invReport.openingInventory,
                          AdoxioQtyreceiveddomestic = invReport.domesticAdditions,
                          AdoxioQtyreceivedreturns = invReport.returnsAdditions,
                          AdoxioQtyreceivedother = invReport.otherAdditions,
                          AdoxioQtyshippeddomestic = invReport.domesticReductions,
                          AdoxioQtyshippedreturned = invReport.returnsReductions,
                          AdoxioQtydestroyed = invReport.destroyedReductions,
                          AdoxioQtyloststolen = invReport.lostReductions,
                          AdoxioOtherreductions = invReport.otherReductions,
                          AdoxioClosinginventory = invReport.closingNumber,
                          AdoxioValueofclosinginventory = invReport.closingValue,
                          AdoxioPackagedunitsnumber = invReport.totalSalesToConsumerQty,
                          AdoxioTotalvalue = invReport.totalSalesToConsumerValue,
                          AdoxioPackagedunitsnumberretailer = invReport.totalSalesToRetailerQty,
                          AdoxioTotalvalueretailer = invReport.totalSalesToRetailerValue
                      };
                      if (invReport.product == "Seeds")
                      {
                          updateReport.AdoxioTotalnumberseeds = invReport.totalSeeds;
                      }
                      else if (invReport.product != "Vegetative Cannabis")
                      {
                          updateReport.AdoxioWeightofclosinginventory = invReport.closingWeight;
                      }
                      _dynamicsClient.Cannabisinventoryreports.Update(invReport.inventoryReportId, updateReport);
                  }
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating monthly report");
                // fail if we can't update.
                throw (httpOperationException);
            }

            return GetMonthlyReport(id);
        }
    }
}
