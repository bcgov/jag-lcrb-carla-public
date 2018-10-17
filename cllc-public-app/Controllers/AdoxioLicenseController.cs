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
            string filter = $"adoxio_licencesid eq {licenceId}";

            var expand = new List<string> { "adoxio_Licencee", "adoxio_adoxio_licences_adoxio_applicationtermsconditionslimitation_Licence" };

            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.Licenses.Get(filter: filter, expand: expand).Value.FirstOrDefault();
            AdoxioLicense license = new AdoxioLicense();

            try
            {
                license = adoxioLicense.ToViewModel(_dynamicsClient);
            }
            catch (Exception)
            {
                throw new Exception("Error getting license by id.");
            }

            var parameters = new Dictionary<string, string>();
            parameters.Add("title", "Canabis_License");
            parameters.Add("licenceNumber", license.licenseNumber);
            parameters.Add("establishmentName", license.establishmentName);
            parameters.Add("establishmentAddress", license.establishmentAddress);
            parameters.Add("licencee", adoxioLicense.AdoxioLicencee.Name);
            try
            {
                DateTime effectiveDate = adoxioLicense.AdoxioEffectivedate.HasValue ? adoxioLicense.AdoxioEffectivedate.Value.DateTime : DateTime.MaxValue;
                DateTime expiryDate = adoxioLicense.AdoxioExpirydate.HasValue ? adoxioLicense.AdoxioExpirydate.Value.DateTime : DateTime.MaxValue;
                parameters.Add("effectiveDate", effectiveDate.ToString("dd/MM/yyyy"));
                parameters.Add("expiryDate", expiryDate.ToString("dd/MM/yyyy"));
            }
            catch
            {
                parameters.Add("effectiveDate", "");
                parameters.Add("expiryDate", "");
            }

            var termsAndConditions = "";
            foreach (var item in adoxioLicense.AdoxioAdoxioLicencesAdoxioApplicationtermsconditionslimitationLicence)
            {
                termsAndConditions += $"<li>{item.AdoxioTermsandconditions}</li>";
            }
            parameters.Add("restrictionsText", termsAndConditions);

            byte[] data = await _pdfClient.GetPdf(parameters, "cannabis_licence");
            return File(data, "application/pdf");
        }
    }
}
