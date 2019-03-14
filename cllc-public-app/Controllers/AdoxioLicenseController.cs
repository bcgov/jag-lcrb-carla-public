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
using Microsoft.Extensions.Logging;

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
        private readonly ILogger _logger;

        public AdoxioLicenseController(IDynamicsClient dynamicsClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, PdfClient pdfClient, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _pdfClient = pdfClient;
            _logger = loggerFactory.CreateLogger(typeof(AdoxioApplicationController));
        }

        private async Task<List<AdoxioLicense>> GetLicensesByLicencee(string licenceeId)
        {
            List<AdoxioLicense> adoxioLicenseVMList = new List<AdoxioLicense>();
            IEnumerable<MicrosoftDynamicsCRMadoxioLicences> dynamicsLicenseList = null;
            if (string.IsNullOrEmpty(licenceeId))
            {
                var response = await _dynamicsClient.Licenceses.GetAsync();
                dynamicsLicenseList = response.Value;
            }
            else
            {
                // get all licenses in Dynamics filtered by the GUID of the licencee
                var filter = "_adoxio_licencee_value eq " + licenceeId;
                var response = await _dynamicsClient.Licenceses.GetAsync(filter: filter);
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
        /// Utility function to convert the option constant to the time string.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ConvertOpenHoursToString(int? data)
        {
            string result = "";
            switch (data)
            {
                case 845280000:
                    result = "00:00";
                    break;
                case 845280024:
                    result = "00:15";
                    break;
                case 845280025:
                    result = "00:30";
                    break;
                case 845280026:
                    result = "00:45";
                    break;
                case 845280001:
                    result = "01:00";
                    break;
                case 845280027:
                    result = "01:15";
                    break;
                case 845280028:
                    result = "01:30";
                    break;
                case 845280029:
                    result = "01:45";
                    break;
                case 845280002:
                    result = "02:00";
                    break;
                case 845280030:
                    result = "02:15";
                    break;
                case 845280031:
                    result = "02:30";
                    break;
                case 845280032:
                    result = "02:45";
                    break;
                case 845280003:
                    result = "03:00";
                    break;
                case 845280033:
                    result = "03:15";
                    break;
                case 845280034:
                    result = "03:30";
                    break;
                case 845280035:
                    result = "03:45";
                    break;
                case 845280004:
                    result = "04:00";
                    break;
                case 845280036:
                    result = "04:15";
                    break;
                case 845280037:
                    result = "04:30";
                    break;
                case 845280038:
                    result = "04:45";
                    break;
                case 845280005:
                    result = "05:00";
                    break;
                case 845280039:
                    result = "05:15";
                    break;
                case 845280040:
                    result = "05:30";
                    break;
                case 845280041:
                    result = "05:45";
                    break;
                case 845280006:
                    result = "06:00";
                    break;
                case 845280042:
                    result = "06:15";
                    break;
                case 845280043:
                    result = "06:30";
                    break;
                case 845280044:
                    result = "06:45";
                    break;
                case 845280007:
                    result = "07:00";
                    break;
                case 845280045:
                    result = "07:15";
                    break;
                case 845280046:
                    result = "07:30";
                    break;
                case 845280047:
                    result = "07:45";
                    break;
                case 845280008:
                    result = "08:00";
                    break;
                case 845280048:
                    result = "08:15";
                    break;
                case 845280049:
                    result = "08:30";
                    break;
                case 845280050:
                    result = "08:45";
                    break;
                case 845280009:
                    result = "09:00";
                    break;
                case 845280051:
                    result = "09:15";
                    break;
                case 845280052:
                    result = "09:30";
                    break;
                case 845280053:
                    result = "09:45";
                    break;
                case 845280010:
                    result = "10:00";
                    break;
                case 845280054:
                    result = "10:15";
                    break;
                case 845280055:
                    result = "10:30";
                    break;
                case 845280056:
                    result = "10:45";
                    break;
                case 845280011:
                    result = "11:00";
                    break;
                case 845280057:
                    result = "11:15";
                    break;
                case 845280058:
                    result = "11:30";
                    break;
                case 845280059:
                    result = "11:45";
                    break;
                case 845280012:
                    result = "12:00";
                    break;
                case 845280060:
                    result = "12:15";
                    break;
                case 845280061:
                    result = "12:30";
                    break;
                case 845280062:
                    result = "12:45";
                    break;
                case 845280013:
                    result = "13:00";
                    break;
                case 845280063:
                    result = "13:15";
                    break;
                case 845280064:
                    result = "13:30";
                    break;
                case 845280065:
                    result = "13:45";
                    break;
                case 845280014:
                    result = "14:00";
                    break;
                case 845280066:
                    result = "14:15";
                    break;
                case 845280067:
                    result = "14:30";
                    break;
                case 845280068:
                    result = "14:45";
                    break;
                case 845280015:
                    result = "15:00";
                    break;
                case 845280069:
                    result = "15:15";
                    break;
                case 845280070:
                    result = "15:30";
                    break;
                case 845280071:
                    result = "15:45";
                    break;
                case 845280016:
                    result = "16:00";
                    break;
                case 845280072:
                    result = "16:15";
                    break;
                case 845280073:
                    result = "16:30";
                    break;
                case 845280074:
                    result = "16:45";
                    break;
                case 845280017:
                    result = "17:00";
                    break;
                case 845280075:
                    result = "17:15";
                    break;
                case 845280076:
                    result = "17:30";
                    break;
                case 845280077:
                    result = "17:45";
                    break;
                case 845280018:
                    result = "18:00";
                    break;
                case 845280078:
                    result = "18:15";
                    break;
                case 845280079:
                    result = "18:30";
                    break;
                case 845280080:
                    result = "18:45";
                    break;
                case 845280019:
                    result = "19:00";
                    break;
                case 845280081:
                    result = "19:15";
                    break;
                case 845280082:
                    result = "19:30";
                    break;
                case 845280083:
                    result = "19:45";
                    break;
                case 845280020:
                    result = "20:00";
                    break;
                case 845280084:
                    result = "20:15";
                    break;
                case 845280085:
                    result = "20:30";
                    break;
                case 845280086:
                    result = "20:45";
                    break;
                case 845280021:
                    result = "21:00";
                    break;
                case 845280087:
                    result = "21:15";
                    break;
                case 845280088:
                    result = "21:30";
                    break;
                case 845280089:
                    result = "21:45";
                    break;
                case 845280022:
                    result = "22:00";
                    break;
                case 845280090:
                    result = "22:15";
                    break;
                case 845280091:
                    result = "22:30";
                    break;
                case 845280092:
                    result = "22:45";
                    break;
                case 845280023:
                    result = "23:00";
                    break;
                case 845280093:
                    result = "23:15";
                    break;
                case 845280094:
                    result = "23:30";
                    break;
                case 845280095:
                    result = "23:45";
                    break;
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

            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.Licenceses.GetByKey(licenceId, expand: expand);
            if (adoxioLicense == null)
            {
                throw new Exception("Error getting license.");
            }

            if (CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.AdoxioLicencee.Accountid))
            {
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
                    return File(data, "application/pdf", $"{adoxioLicense.AdoxioLicencenumber}.pdf");
                }
                catch (Exception e)
                {
                    _logger.LogError("Error returning PDF response");
                    _logger.LogError(e.Message);

                    return new NotFoundResult();
                }
            }
            else
            {
                return new NotFoundResult();
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
