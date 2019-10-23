using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.ClassMaps;
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

        /// GET all monthly reports in Dynamics by Licence filtered by the current user's licencee
        [HttpGet("licence/{licenceId}")]
        public async Task<IActionResult> GetMonthlyReportsByLicence(string licenceId)
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
            catch (HttpOperationException)
            {
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
        public async Task<IActionResult> GetCurrentUserMonthlyReports()
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
    }
}
