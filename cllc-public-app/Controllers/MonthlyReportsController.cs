using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class MonthlyReportsController : ControllerBase
    {
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly int monthlyReportsMaxMonths;

        public MonthlyReportsController(IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = Log.Logger;
            _configuration = configuration;

            if (!string.IsNullOrEmpty(_configuration["MONTHLY_REPORTS_MAX_MONTHS"]))
            {
                if (!int.TryParse(_configuration["MONTHLY_REPORTS_MAX_MONTHS"], out monthlyReportsMaxMonths))
                {
                    monthlyReportsMaxMonths = 12;
                }
            }
            else
            {
                monthlyReportsMaxMonths = 12;
            }
        }

        private List<MonthlyReport> GetMonthlyReportsByUser(string licenceeId, bool expandInventoryReports)
        {
            List<MonthlyReport> monthlyReportsList = new List<MonthlyReport>();
            IEnumerable<MicrosoftDynamicsCRMadoxioCannabismonthlyreport> monthlyReports;
            if (string.IsNullOrEmpty(licenceeId))
            {
                return monthlyReportsList;
            }

            try
            {
                var select = new List<string>() { "_adoxio_licenceid_value", "adoxio_licencenumber", "adoxio_reportingperiodmonth", "adoxio_reportingperiodyear", "statuscode", "adoxio_employeesmanagement", "adoxio_employeesadministrative", "adoxio_employeessales", "adoxio_employeesproduction", "adoxio_employeesother", "adoxio_cannabismonthlyreportid" };
                var expand = new List<string>() { "adoxio_EstablishmentId($select=adoxio_establishmentid, adoxio_name, adoxio_addresscity, adoxio_addresspostalcode)" };
                // filter the data to only show data for the given licensee and only since the start date.
                var filter = $"_adoxio_licenseeid_value eq {licenceeId} and createdon ge {GetStartDateForMonthlyReports()}";

                // Sort monthly reports so that they display in the proper order (Jan -> Feb, Feb -> Mar etc)
                monthlyReports = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter, select: select, expand: expand, @orderby: new List<string> { "adoxio_reportingperiodyear asc", "adoxio_reportingperiodmonth asc" }).Value;
            }
            catch (HttpOperationException)
            {
                monthlyReports = null;
            }

            if (monthlyReports != null)
            {
                foreach (var monthlyReport in monthlyReports)
                {
                    monthlyReportsList.Add(monthlyReport.ToViewModel(_dynamicsClient, expandInventoryReports));
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
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

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
            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            List<MonthlyReport> monthlyReportsList = new List<MonthlyReport>();
            IEnumerable<MicrosoftDynamicsCRMadoxioCannabismonthlyreport> monthlyReports;
            try
            {
                var filter = $"_adoxio_licenceid_value eq {licenceId} and _adoxio_licenseeid_value eq {userSettings.AccountId} and createdon ge {GetStartDateForMonthlyReports()}";
                var expand = new List<string>() { "adoxio_EstablishmentId($select=adoxio_establishmentid, adoxio_name, adoxio_addresscity, adoxio_addresspostalcode)" };
                monthlyReports = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter, expand: expand, orderby: new List<string> { "modifiedon desc" }).Value;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error querying monthly reports");
                monthlyReports = null;
            }

            if (monthlyReports != null)
            {
                foreach (var monthlyReport in monthlyReports)
                {
                    monthlyReportsList.Add(monthlyReport.ToViewModel(_dynamicsClient, true));
                }
            }

            return new JsonResult(monthlyReportsList);
        }


        /// GET all monthly reports in Dynamics by Licence filtered by the current user's licencee
        [HttpGet("licenceYearMonth")]
        public IActionResult GetMonthlyReportByLicenceYearMonth([FromQuery] string licenceId, [FromQuery] string year="", [FromQuery] string month="")
        {
            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            MonthlyReport monthlyReport = new MonthlyReport();
            List<MonthlyReport> monthlyReportsList = new List<MonthlyReport>();
            IEnumerable<MicrosoftDynamicsCRMadoxioCannabismonthlyreport> monthlyReports;
            try
            {
                var select = new List<string>() { "_adoxio_licenceid_value", "adoxio_licencenumber", "adoxio_reportingperiodmonth", "adoxio_reportingperiodyear", "statuscode", "adoxio_employeesmanagement", "adoxio_employeesadministrative", "adoxio_employeessales", "adoxio_employeesproduction", "adoxio_employeesother", "adoxio_cannabismonthlyreportid" };
                var expand = new List<string>() { "adoxio_EstablishmentId($select=adoxio_establishmentid, adoxio_name, adoxio_addresscity, adoxio_addresspostalcode)" };
                var filter = $"_adoxio_licenceid_value eq {licenceId} and adoxio_reportingperiodyear eq '{year}' and adoxio_reportingperiodmonth eq '{month}'";
                monthlyReports = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter, select: select, expand: expand, @orderby: new List<string> { "adoxio_reportingperiodyear asc", "adoxio_reportingperiodmonth asc" }).Value;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error querying monthly reports");
                monthlyReports = null;
            }

            if (monthlyReports != null && monthlyReports.Count() > 0)
            {               
                return new JsonResult(monthlyReports.ToArray()[0].ToViewModel(_dynamicsClient, true));  
            }
            throw new HttpOperationException("Error: no monthly report exist for this "+year+" and "+month);    
        }

        /// GET all monthly reports in Dynamics by Licencee using the account Id assigned to the user logged in
        [HttpGet("current")]
        public IActionResult GetCurrentUserMonthlyReports([FromQuery] bool expandInventoryReports)
        {
            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            if (string.IsNullOrEmpty(userSettings.AccountId))
            {
                return new BadRequestResult();
            }
            // get all licenses in Dynamics by Licencee using the account Id assigned to the user logged in
            List<MonthlyReport> monthlyReports = GetMonthlyReportsByUser(userSettings.AccountId, expandInventoryReports);

            return new JsonResult(monthlyReports);
        }

        private string GetStartDateForMonthlyReports()
        {
            var startDate = DateTimeOffset.Now.AddMonths(-1 * monthlyReportsMaxMonths);
            // return as YYYY/MM/DD
            return startDate.ToString("yyyy-MM-dd");
        }

        /// GET monthly report by id in Dynamics by if owned by user
        [HttpGet("{reportId}")]
        public IActionResult GetMonthlyReport(string reportId)
        {
            try
            {
                var filter = $"adoxio_cannabismonthlyreportid eq {reportId}";
                var expand = new List<string>() { "adoxio_EstablishmentId($select=adoxio_establishmentid, adoxio_name, adoxio_addresscity, adoxio_addresspostalcode)" };
                MicrosoftDynamicsCRMadoxioCannabismonthlyreport monthlyReport = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter, expand: expand).Value.FirstOrDefault();
                if (monthlyReport != null && CurrentUserHasAccessToMonthlyReportOwnedBy(monthlyReport._adoxioLicenseeidValue))
                {
                    return new JsonResult(monthlyReport.ToViewModel(_dynamicsClient, true));
                }
            }
            catch (HttpOperationException ex)
            {
                _logger.Error(ex, "Error getting cannabis monthly report");
            }
            return new NotFoundResult();
        }

        /// PUT update monthly report by id
        [HttpPut("{id}")]
        public IActionResult UpdateMonthlyReport([FromBody] MonthlyReport item, string id)
        {
            if (item != null && id != item.monthlyReportId)
            {
                return BadRequest();
            }

            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            Guid monthlyReportId = new Guid(id);
            string filter = $"adoxio_cannabismonthlyreportid eq {id}";
            var monthlyReportResp = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter);
            if (monthlyReportResp.Value.Count < 1 || !CurrentUserHasAccessToMonthlyReportOwnedBy(monthlyReportResp.Value[0]._adoxioLicenseeidValue))
            {
                return new NotFoundResult();
            }

            // Update inventory reports
            if (item.inventorySalesReports != null && item.inventorySalesReports.Count > 0)
            {
                foreach (InventorySalesReport invReport in item.inventorySalesReports)
                {
                    MicrosoftDynamicsCRMadoxioCannabisinventoryreport updateReport = new MicrosoftDynamicsCRMadoxioCannabisinventoryreport
                    {
                        AdoxioOpeninginventory = invReport.openingInventory == null ? 0 : invReport.openingInventory,
                        AdoxioQtyreceiveddomestic = invReport.domesticAdditions == null ? 0 : invReport.domesticAdditions,
                        AdoxioQtyreceivedreturns = invReport.returnsAdditions == null ? 0 : invReport.returnsAdditions,
                        AdoxioQtyreceivedother = invReport.otherAdditions == null ? 0 : invReport.otherAdditions,
                        AdoxioQtyshippeddomestic = invReport.domesticReductions == null ? 0 : invReport.domesticReductions,
                        AdoxioQtyshippedreturned = invReport.returnsReductions == null ? 0 : invReport.returnsReductions,
                        AdoxioQtydestroyed = invReport.destroyedReductions == null ? 0 : invReport.destroyedReductions,
                        AdoxioQtyloststolen = invReport.lostReductions == null ? 0 : invReport.lostReductions,
                        AdoxioOtherreductions = invReport.otherReductions == null ? 0 : invReport.otherReductions,
                        AdoxioClosinginventory = invReport.closingNumber == null ? 0 : invReport.closingNumber,
                        AdoxioValueofclosinginventory = invReport.closingValue == null ? 0 : invReport.closingValue,
                        AdoxioPackagedunitsnumber = invReport.totalSalesToConsumerQty == null ? 0 : invReport.totalSalesToConsumerQty,
                        AdoxioTotalvalue = invReport.totalSalesToConsumerValue == null ? 0 : invReport.totalSalesToConsumerValue,
                        AdoxioPackagedunitsnumberretailer = invReport.totalSalesToRetailerQty == null ? 0 : invReport.totalSalesToRetailerQty,
                        AdoxioTotalvalueretailer = invReport.totalSalesToRetailerValue == null ? 0 : invReport.totalSalesToRetailerValue
                    };
                    if (invReport.product == "Seeds")
                    {
                        updateReport.AdoxioTotalnumberseeds = invReport.totalSeeds == null ? 0 : invReport.totalSeeds;
                    }
                    else if (invReport.product == "Extracts - Other" || invReport.product == "Other")
                    {
                        updateReport.AdoxioOtherdescription = invReport.otherDescription;
                    }

                    if (invReport.product != "Vegetative Cannabis")
                    {
                        updateReport.AdoxioWeightofclosinginventory = invReport.closingWeight == null ? 0 : invReport.closingWeight;
                    }
                    try
                    {
                        _dynamicsClient.Cannabisinventoryreports.Update(invReport.inventoryReportId, updateReport);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating inventory report");
                        // fail if we can't update.
                        throw (httpOperationException);
                    }
                }
            }

            try
            {
                // Update monthly report.  Changed 4/7/2021 to occur after the inventory reports are done.
                MicrosoftDynamicsCRMadoxioCannabismonthlyreport monthlyReport =
                    new MicrosoftDynamicsCRMadoxioCannabismonthlyreport
                    {
                        AdoxioEmployeesmanagement = item.employeesManagement,
                        AdoxioEmployeesadministrative = item.employeesAdministrative,
                        AdoxioEmployeessales = item.employeesSales,
                        AdoxioEmployeesproduction = item.employeesProduction,
                        AdoxioEmployeesother = item.employeesOther,
                        Statuscode = item.statusCode
                    };
                _dynamicsClient.Cannabismonthlyreports.Update(item.monthlyReportId, monthlyReport);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.Error(httpOperationException, "Error updating monthly report");
                // fail if we can't update.
                throw (httpOperationException);
            }


            return GetMonthlyReport(id);
        }
    }
}
