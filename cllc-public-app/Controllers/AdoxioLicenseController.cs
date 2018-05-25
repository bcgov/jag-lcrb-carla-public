using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OData.Client;
using Gov.Lclb.Cllb.Public.ViewModels;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Http;
using Gov.Lclb.Cllb.Public.Authentication;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AdoxioLicenseController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Contexts.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdoxioLicenseController(Contexts.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = distributedCache;
            this._httpContextAccessor = httpContextAccessor;
        }

        private async Task<List<AdoxioLicense>> GetLicensesByLicencee(string licenceeId)
        {
            List<AdoxioLicense> adoxioLiceseVMList = new List<AdoxioLicense>();
            IEnumerable<Adoxio_licences> dynamicsLicenseList = null;
            if (string.IsNullOrEmpty(licenceeId))
            {
                dynamicsLicenseList = await _system.Adoxio_licenceses.ExecuteAsync();
            }
            else
            {
                // get all licenses in Dynamics filtered by the GUID of the licencee
                var filter = "_adoxio_licencee_value eq " + licenceeId;
                dynamicsLicenseList = await _system.Adoxio_licenceses
                        .AddQueryOption("$filter", filter).ExecuteAsync();
            }

            if (dynamicsLicenseList != null)
            {
                foreach (Adoxio_licences dynamicsLicense in dynamicsLicenseList)
                {
                    adoxioLiceseVMList.Add(await dynamicsLicense.ToViewModel(_system));
                }
            }
            return adoxioLiceseVMList;
        }

        /// GET all licenses in Dynamics
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsLicenses()
        {
            // get all licenses in Dynamics
            List<AdoxioLicense> adoxioLicenses = await GetLicensesByLicencee(null);

            return Json(adoxioLicenses);
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
