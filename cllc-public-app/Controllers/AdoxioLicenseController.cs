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


        /// GET a licence as PDF.
        [HttpGet("{licenceId}/pdf")]
        [AllowAnonymous]
        public async Task<FileContentResult> GetLicencePDF(string licenceId)
        {
            var parameters = new Dictionary<string, string>();

            string filter = $"adoxio_licencesid eq {licenceId}";

            var expand = new List<string> {
                "adoxio_Licencee",
                "adoxio_adoxio_licences_adoxio_applicationtermsconditionslimitation_Licence",
                "adoxio_adoxio_licences_adoxio_application_AssignedLicence"
            };

            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.Licenses.GetByKey(licenceId, expand: expand);
            if (adoxioLicense == null)
            {
                throw new Exception("Error getting license.");
            }

            AdoxioLicense license = new AdoxioLicense();
            license = adoxioLicense.ToViewModel(_dynamicsClient);
            
            parameters.Add("title", "Canabis_License");
            parameters.Add("licenceNumber", license.licenseNumber);
            parameters.Add("establishmentName", license.establishmentName);
            parameters.Add("establishmentAddress", license.establishmentAddress);
            parameters.Add("licencee", adoxioLicense.AdoxioLicencee.Name);

            if (adoxioLicense.AdoxioEffectivedate.HasValue)
            {
                DateTime effectiveDate = adoxioLicense.AdoxioEffectivedate.Value.DateTime;
                parameters.Add("effectiveDate", effectiveDate.ToString("dd/MM/yyyy"));
            }
            else
            {
                parameters.Add("effectiveDate", "");
            }

            if (adoxioLicense.AdoxioExpirydate.HasValue)
            {
                DateTime expiryDate = adoxioLicense.AdoxioExpirydate.Value.DateTime;
                parameters.Add("expiryDate", expiryDate.ToString("dd/MM/yyyy"));
            }
            else
            {
                parameters.Add("expiryDate", "");
            }

            var termsAndConditions = "";
            foreach (var item in adoxioLicense.AdoxioAdoxioLicencesAdoxioApplicationtermsconditionslimitationLicence)
            {
                termsAndConditions += $"<li>{item.AdoxioTermsandconditions}</li>";
            }
            parameters.Add("restrictionsText", termsAndConditions);



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
                    <td>{application?.AdoxioServicehoursmondayopen}</td>
                    <td>{application?.AdoxioServicehourstuesdayopen}</td>
                    <td>{application?.AdoxioServicehourswednesdayopen}</td>
                    <td>{application?.AdoxioServicehoursthursdayopen}</td>
                    <td>{application?.AdoxioServicehoursfridayopen}</td>
                    <td>{application?.AdoxioServicehourssaturdayopen}</td>
                    <td>{application?.AdoxioServicehourssundayopen}</td>
                </tr>                
                <tr>
                    <td>Close</td>
                    <td>{application?.AdoxioServicehoursmondayclose}</td>
                    <td>{application?.AdoxioServicehourstuesdayclose}</td>
                    <td>{application?.AdoxioServicehourswednesdayclose}</td>
                    <td>{application?.AdoxioServicehoursthursdayclose}</td>
                    <td>{application?.AdoxioServicehoursfridayclose}</td>
                    <td>{application?.AdoxioServicehourssaturdayclose}</td>
                    <td>{application?.AdoxioServicehourssundayclose}</td>
                </tr>";
            }
            parameters.Add("storeHours", storeHours);    

            byte[] data = await _pdfClient.GetPdf(parameters, "cannabis_licence");
            return File(data, "application/pdf");
        }
    }
}
