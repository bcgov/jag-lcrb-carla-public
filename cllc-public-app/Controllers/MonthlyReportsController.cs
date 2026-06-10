extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class MonthlyReportsController : ControllerBase
    {
        private readonly IDataverseClient _dataverse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly int monthlyReportsMaxMonths;

        public MonthlyReportsController(IDataverseClient dataverse, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataverse = dataverse;
            _logger = Log.Logger;
            _configuration = configuration;

            if (!string.IsNullOrEmpty(_configuration["MONTHLY_REPORTS_MAX_MONTHS"]))
            {
                if (!int.TryParse(_configuration["MONTHLY_REPORTS_MAX_MONTHS"], out monthlyReportsMaxMonths))
                    monthlyReportsMaxMonths = 12;
            }
            else
            {
                monthlyReportsMaxMonths = 12;
            }
        }

        private async Task<List<MonthlyReport>> GetMonthlyReportsByUserAsync(string licenceeId, bool expandInventoryReports)
        {
            var result = new List<MonthlyReport>();
            if (string.IsNullOrEmpty(licenceeId)) return result;

            var reports = await _dataverse.GetCannabisMonthlyReportsByLicenceeAsync(licenceeId, GetStartDateForMonthlyReports());
            foreach (var report in reports)
            {
                IList<adoxio_cannabisinventoryreport>? invReports = null;
                if (expandInventoryReports && report.adoxio_cannabismonthlyreportId.HasValue)
                    invReports = await _dataverse.GetInventoryReportsByMonthlyReportIdAsync(report.adoxio_cannabismonthlyreportId.Value.ToString());
                result.Add(report.ToViewModel(invReports));
            }
            return result;
        }

        private bool CurrentUserHasAccessToMonthlyReportOwnedBy(string accountId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
                return userSettings.AccountId == accountId;
            return false;
        }

        [HttpGet("licence/{licenceId}")]
        public async Task<IActionResult> GetMonthlyReportsByLicence(string licenceId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            var monthlyReportsList = new List<MonthlyReport>();
            try
            {
                var reports = await _dataverse.GetCannabisMonthlyReportsByLicenceAndLicenceeAsync(licenceId, userSettings.AccountId, GetStartDateForMonthlyReports());
                foreach (var report in reports)
                {
                    IList<adoxio_cannabisinventoryreport>? invReports = null;
                    if (report.adoxio_cannabismonthlyreportId.HasValue)
                        invReports = await _dataverse.GetInventoryReportsByMonthlyReportIdAsync(report.adoxio_cannabismonthlyreportId.Value.ToString());
                    monthlyReportsList.Add(report.ToViewModel(invReports));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error querying monthly reports");
            }

            return new JsonResult(monthlyReportsList);
        }

        [HttpGet("licenceYearMonth")]
        public async Task<IActionResult> GetMonthlyReportByLicenceYearMonth([FromQuery] string licenceId, [FromQuery] string year = "", [FromQuery] string month = "")
        {
            try
            {
                var report = await _dataverse.GetCannabisMonthlyReportByLicenceYearMonthAsync(licenceId, year, month);
                if (report == null) return new NotFoundResult();

                IList<adoxio_cannabisinventoryreport>? invReports = null;
                if (report.adoxio_cannabismonthlyreportId.HasValue)
                    invReports = await _dataverse.GetInventoryReportsByMonthlyReportIdAsync(report.adoxio_cannabismonthlyreportId.Value.ToString());
                return new JsonResult(report.ToViewModel(invReports));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error querying monthly reports by year/month");
                return new NotFoundResult();
            }
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUserMonthlyReports([FromQuery] bool expandInventoryReports)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            if (string.IsNullOrEmpty(userSettings.AccountId)) return new BadRequestResult();

            var monthlyReports = await GetMonthlyReportsByUserAsync(userSettings.AccountId, expandInventoryReports);
            return new JsonResult(monthlyReports);
        }

        private string GetStartDateForMonthlyReports()
        {
            var startDate = DateTimeOffset.Now.AddMonths(-1 * monthlyReportsMaxMonths);
            return startDate.ToString("yyyy-MM-dd");
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetMonthlyReport(string reportId)
        {
            try
            {
                var report = await _dataverse.GetCannabisMonthlyReportByIdAsync(reportId);
                if (report != null && CurrentUserHasAccessToMonthlyReportOwnedBy(report.adoxio_LicenseeId?.Id.ToString()))
                {
                    IList<adoxio_cannabisinventoryreport>? invReports = null;
                    if (report.adoxio_cannabismonthlyreportId.HasValue)
                        invReports = await _dataverse.GetInventoryReportsByMonthlyReportIdAsync(reportId);
                    return new JsonResult(report.ToViewModel(invReports));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting cannabis monthly report");
            }
            return new NotFoundResult();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMonthlyReport([FromBody] MonthlyReport item, string id)
        {
            if (item != null && id != item.monthlyReportId) return BadRequest();

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            var existing = await _dataverse.GetCannabisMonthlyReportByIdAsync(id);
            if (existing == null || !CurrentUserHasAccessToMonthlyReportOwnedBy(existing.adoxio_LicenseeId?.Id.ToString()))
                return new NotFoundResult();

            if (item.inventorySalesReports != null && item.inventorySalesReports.Count > 0)
            {
                foreach (InventorySalesReport invReport in item.inventorySalesReports)
                {
                    var updateReport = new adoxio_cannabisinventoryreport { Id = Guid.Parse(invReport.inventoryReportId) };
                    updateReport.adoxio_OpeningInventory = invReport.openingInventory ?? 0;
                    updateReport.adoxio_QtyReceivedDomestic = invReport.domesticAdditions ?? 0;
                    updateReport.adoxio_QtyReceivedReturns = invReport.returnsAdditions ?? 0;
                    updateReport.adoxio_QtyReceivedOther = invReport.otherAdditions ?? 0;
                    updateReport.adoxio_QtyShippedDomestic = invReport.domesticReductions ?? 0;
                    updateReport.adoxio_QtyShippedReturned = invReport.returnsReductions ?? 0;
                    updateReport.adoxio_QtyDestroyed = invReport.destroyedReductions ?? 0;
                    updateReport.adoxio_QtyLostStolen = invReport.lostReductions ?? 0;
                    updateReport.adoxio_OtherReductions = invReport.otherReductions ?? 0;
                    updateReport.adoxio_ClosingInventory = invReport.closingNumber ?? 0;
                    updateReport.adoxio_ValueofClosingInventory = invReport.closingValue.HasValue ? new Microsoft.Xrm.Sdk.Money(invReport.closingValue.Value) : null;
                    updateReport.adoxio_PackagedUnitsNumber = invReport.totalSalesToConsumerQty.HasValue ? (decimal?)invReport.totalSalesToConsumerQty.Value : null;
                    updateReport.adoxio_TotalValue = invReport.totalSalesToConsumerValue.HasValue ? new Microsoft.Xrm.Sdk.Money(invReport.totalSalesToConsumerValue.Value) : null;
                    updateReport.adoxio_PackagedUnitsNumberRetailer = invReport.totalSalesToRetailerQty.HasValue ? (decimal?)invReport.totalSalesToRetailerQty.Value : null;
                    updateReport.adoxio_TotalValueRetailer = invReport.totalSalesToRetailerValue.HasValue ? new Microsoft.Xrm.Sdk.Money(invReport.totalSalesToRetailerValue.Value) : null;
                    if (invReport.product == "Seeds")
                        updateReport.adoxio_TotalNumberSeeds = invReport.totalSeeds ?? 0;
                    else if (invReport.product == "Extracts - Other" || invReport.product == "Other")
                        updateReport.adoxio_OtherDescription = invReport.otherDescription;
                    if (invReport.product != "Vegetative Cannabis")
                        updateReport.adoxio_WeightofClosingInventory = invReport.closingWeight;
                    try { await _dataverse.UpdateCannabisInventoryReportAsync(updateReport); }
                    catch (Exception e)
                    {
                        _logger.Error(e, "Error updating inventory report");
                        throw;
                    }
                }
            }

            var patchReport = new adoxio_cannabismonthlyreport { Id = Guid.Parse(id) };
            patchReport.adoxio_EmployeesManagement = item.employeesManagement;
            patchReport.adoxio_EmployeesAdministrative = item.employeesAdministrative;
            patchReport.adoxio_EmployeesSales = item.employeesSales;
            patchReport.adoxio_EmployeesProduction = item.employeesProduction;
            patchReport.adoxio_EmployeesOther = item.employeesOther;
            patchReport.statuscode = item.statusCode.HasValue ? (adoxio_cannabismonthlyreport_statuscode?)item.statusCode.Value : null;
            try { await _dataverse.UpdateCannabisMonthlyReportAsync(patchReport); }
            catch (Exception e)
            {
                _logger.Error(e, "Error updating monthly report");
                throw;
            }

            return await GetMonthlyReport(id);
        }
    }
}
