using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = "Business-User")]
    public class AdoxioLicenseController : Controller
    {
        private readonly IConfiguration Configuration;        
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdoxioLicenseController(IDynamicsClient dynamicsClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<List<AdoxioLicense>> GetLicensesByLicencee(string licenceeId)
        {
            List<AdoxioLicense> adoxioLicenseVMList = new List<AdoxioLicense>();
            IEnumerable<MicrosoftDynamicsCRMadoxioLicences> dynamicsLicenseList = null;
            if (string.IsNullOrEmpty(licenceeId))
            {
                var response = await _dynamicsClient.Licenses.GetAsync();
                dynamicsLicenseList = response.Value;
            }
            else
            {
                // get all licenses in Dynamics filtered by the GUID of the licencee
                var filter = "_adoxio_licencee_value eq " + licenceeId;
                var response = await _dynamicsClient.Licenses.GetAsync(filter: filter);
                dynamicsLicenseList = response.Value;
            }

            if (dynamicsLicenseList != null)
            {
                foreach (var dynamicsLicense in dynamicsLicenseList)
                {
                    adoxioLicenseVMList.Add(dynamicsLicense.ToViewModel(_dynamicsClient));
                }
            }
            return adoxioLicenseVMList;
        }

        /// GET all licenses in Dynamics by Licencee using the account Id assigned to the user logged in
        [HttpGet("current")]
        public async Task<JsonResult> GetCurrentUserDyanamicsApplications()
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // get all licenses in Dynamics by Licencee using the account Id assigned to the user logged in
            List<AdoxioLicense> adoxioLicenses = await GetLicensesByLicencee(userSettings.AccountId);

            // For Demo Only, hardcode the account id !!!
            //string accountId = "f3310e39-e352-e811-8140-480fcfeac941";
            //List<AdoxioLicense> adoxioLicenses = await GetLicensesByLicencee(accountId);

            return Json(adoxioLicenses);
        }

        /// GET all licenses in Dynamics
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsLicenses()
        {
            // get all licenses in Dynamics
            List<AdoxioLicense> adoxioLicenses = await GetLicensesByLicencee(null);

            return Json(adoxioLicenses);
        }

        /// GET all licenses in Dynamics filtered by the GUID of the licencee
        [HttpGet("{licenceeId}")]
        public async Task<JsonResult> GetDynamicsLicenses(string licenceeId)
        {
            // get all licenses in Dynamics by Licencee Id
            List<AdoxioLicense> adoxioLicenses = await GetLicensesByLicencee(licenceeId);

            return Json(adoxioLicenses);
        }


    }
}
