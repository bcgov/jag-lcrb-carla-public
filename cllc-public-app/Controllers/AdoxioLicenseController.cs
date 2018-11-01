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
using System;
using System.Linq;
using System.Net;
using Gov.Lclb.Cllb.Public.Utils;

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
        private readonly PdfClient _pdfClient;

        public AdoxioLicenseController(IDynamicsClient dynamicsClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, PdfClient pdfClient)
        {
            Configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _pdfClient = pdfClient;
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

        /// <summary>
        /// Utility function to convert the unix timestamp to the time string.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ConvertOpenHoursToString(int? data)
        {
            string result = "";
            if (data != null)
            {
                result = new DateTime((long)data).ToShortTimeString();
            }
            return result;
        }

        /// GET a licence as PDF.
        [HttpGet("{licenceId}/pdf")]
        public async Task<IActionResult> GetLicencePDF(string licenceId)
        {
            
            var expand = new List<string> {
                "adoxio_Licencee",
                "adoxio_adoxio_licences_adoxio_applicationtermsconditionslimitation_Licence",
                "adoxio_adoxio_licences_adoxio_application_AssignedLicence",
                "adoxio_establishment"
            };

            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.Licenses.GetByKey(licenceId, expand: expand);
            if (adoxioLicense == null)
            {
                throw new Exception("Error getting license.");
            }



            var effectiveDateParam = "";
            if (adoxioLicense.AdoxioEffectivedate.HasValue)
            {
                DateTime effectiveDate = adoxioLicense.AdoxioEffectivedate.Value.DateTime;
                effectiveDateParam = effectiveDate.ToString("dd/MM/yyyy");
            }

            var expiraryDateParam = "";
            if (adoxioLicense.AdoxioExpirydate.HasValue)
            {
                DateTime expiryDate = adoxioLicense.AdoxioExpirydate.Value.DateTime;
                expiraryDateParam = expiryDate.ToString("dd/MM/yyyy");
            }

            var termsAndConditions = "";
            foreach (var item in adoxioLicense.AdoxioAdoxioLicencesAdoxioApplicationtermsconditionslimitationLicence)
            {
                termsAndConditions += $"<li>{item.AdoxioTermsandconditions}</li>";
            }

            var application = adoxioLicense?.AdoxioAdoxioLicencesAdoxioApplicationAssignedLicence?.FirstOrDefault();
            var storeHours = $@"
                <tr>
                    <td>Open</td>
                    <td>9:00 am</td>
                    <td>9:00 am</td>
                    <td>9:00 am</td>
                    <td>9:00 am</td>
                    <td>9:00 am</td>
                    <td>9:00 am</td>
                    <td>9:00 am</td>
                </tr>                
                <tr>
                    <td>Close</td>
                    <td>11:00 pm</td>
                    <td>11:00 pm</td>
                    <td>11:00 pm</td>
                    <td>11:00 pm</td>
                    <td>11:00 pm</td>
                    <td>11:00 pm</td>
                    <td>11:00 pm</td>
                </tr>";
            if (application.AdoxioServicehoursstandardhours != true)
            {
                



                storeHours = $@"
                <tr>
                    <td>Open</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehoursmondayopen)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehourstuesdayopen)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehourswednesdayopen)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehoursthursdayopen)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehoursfridayopen)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehourssaturdayopen)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehourssundayopen)}</td>
                </tr>                
                <tr>
                    <td>Close</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehoursmondayclose)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehourstuesdayclose)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehourswednesdayclose)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehoursthursdayclose)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehoursfridayclose)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehourssaturdayclose)}</td>
                    <td>{ConvertOpenHoursToString(application?.AdoxioServicehourssundayclose)}</td>
                </tr>";
            }

            var parameters = new Dictionary<string, string>
            {
                { "title", "Canabis_License" },
                { "licenceNumber", adoxioLicense.AdoxioLicencenumber },
                { "establishmentName", adoxioLicense.AdoxioEstablishment.AdoxioName },
                { "establishmentStreet", adoxioLicense.AdoxioEstablishment.AdoxioAddressstreet },
                { "establishmentCity", adoxioLicense.AdoxioEstablishment.AdoxioAddresscity + ", B.C." },
                { "establishmentPostalCode", adoxioLicense.AdoxioEstablishment.AdoxioAddresspostalcode },
                { "licencee", adoxioLicense.AdoxioLicencee.Name },
                { "effectiveDate", effectiveDateParam },
                { "expiryDate", expiraryDateParam },
                { "restrictionsText", termsAndConditions },
                { "storeHours", storeHours }
            };

            try
            {
                byte[] data = await _pdfClient.GetPdf(parameters, "cannabis_licence");
                return File(data, "application/pdf");
            }
            catch
            {
                string basePath = string.IsNullOrEmpty(Configuration["BASE_PATH"]) ? "" : Configuration["BASE_PATH"];
                basePath += "/dashboard-lite";
                return Redirect(basePath);
            }
        }
        
        /// <summary>
        /// Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
        private bool CurrentUserHasAccessToLicenseOwnedBy(string accountId)
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
    }
}
