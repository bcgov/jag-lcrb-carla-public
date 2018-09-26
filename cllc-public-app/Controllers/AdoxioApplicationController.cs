using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Gov.Lclb.Cllb.Interfaces.SharePointFileManager;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "Business-User")]
    public class AdoxioApplicationController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SharePointFileManager _sharePointFileManager;
        private readonly ILogger _logger;
        private readonly IDynamicsClient _dynamicsClient;

        public AdoxioApplicationController(SharePointFileManager sharePointFileManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            Configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _sharePointFileManager = sharePointFileManager;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(AdoxioApplicationController));
        }

        /// <summary>
        /// Get a license application by applicant id
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        private async Task<List<ViewModels.AdoxioApplication>> GetApplicationsByApplicant(string applicantId)
        {
            List<ViewModels.AdoxioApplication> result = new List<ViewModels.AdoxioApplication>();
            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> dynamicsApplicationList = null;
            if (string.IsNullOrEmpty(applicantId))
            {
                dynamicsApplicationList = _dynamicsClient.Applications.Get().Value;
            }
            else
            {
                var filter = $"_adoxio_applicant_value eq {applicantId} and statuscode ne {(int)AdoxioApplicationStatusCodes.Terminated}";
                var expand = new List<string> { "adoxio_LicenceFeeInvoice", "adoxio_AssignedLicence" };
                try
                {
                    dynamicsApplicationList = _dynamicsClient.Applications.Get(filter: filter, expand: expand).Value;
                }
                catch (OdataerrorException)
                {
                    dynamicsApplicationList = null;
                }
            }

            if (dynamicsApplicationList != null)
            {
                foreach (MicrosoftDynamicsCRMadoxioApplication dynamicsApplication in dynamicsApplicationList)
                {
                    // hide terminated applications from view.
                    if (dynamicsApplication.Statuscode == null || dynamicsApplication.Statuscode != (int)AdoxioApplicationStatusCodes.Terminated)
                    {
                        result.Add(await dynamicsApplication.ToViewModel(_dynamicsClient));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the number of applications that are submitted
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        private int GetSubmittedCountByApplicant(string applicantId)
        {
            var result = 0;
            if (!string.IsNullOrEmpty(applicantId))
            {
                var filter = $"_adoxio_applicant_value eq {applicantId} and adoxio_paymentrecieved eq true and statuscode ne {(int)AdoxioApplicationStatusCodes.Terminated}";
                try
                {
                    result = _dynamicsClient.Applications.Get(filter: filter).Value.Count;
                }
                catch (OdataerrorException)
                {
                    result = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// GET all applications in Dynamics. Optional parameter for applicant ID. Or all applications if the applicantId is null
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsApplications(string applicantId)
        {
            List<ViewModels.AdoxioApplication> adoxioApplications = await GetApplicationsByApplicant(applicantId);
            return Json(adoxioApplications);
        }

        /// GET all applications in Dynamics for the current user
        [HttpGet("current")]
        public async Task<JsonResult> GetCurrentUserDyanamicsApplications()
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // GET all applications in Dynamics by applicant using the account Id assigned to the user logged in
            List<ViewModels.AdoxioApplication> adoxioApplications = await GetApplicationsByApplicant(userSettings.AccountId);
            return Json(adoxioApplications);
        }

        /// GET submitted applications in Dynamics for the current user
        [HttpGet("current/submitted-count")]
        public JsonResult GetCountForCurrentUserSubmittedApplications()
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // GET all applications in Dynamics by applicant using the account Id assigned to the user logged in
            var count = GetSubmittedCountByApplicant(userSettings.AccountId);
            return Json(count);
        }

        /// <summary>
        /// GET an Application by ID
        /// </summary>
        /// <param name="id">GUID of the Application to get</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDynamicsApplication(string id)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            _logger.LogError("Application id = " + id);
            _logger.LogError("User id = " + userSettings.AccountId);

            ViewModels.AdoxioApplication result = null;
            var dynamicsApplication = await _dynamicsClient.GetApplicationById(Guid.Parse(id));
            if (dynamicsApplication == null)
            {
                return NotFound();
            }
            else
            {
                if (!CurrentUserHasAccessToApplicationOwnedBy(dynamicsApplication._adoxioApplicantValue))
                {
                    return new NotFoundResult();
                }
                result = await dynamicsApplication.ToViewModel(_dynamicsClient);
            }

            return Json(result);
        }

        private string GetApplicationFolderName(MicrosoftDynamicsCRMadoxioApplication application)
        {
            string applicationIdCleaned = application.AdoxioApplicationid.ToString().ToUpper().Replace("-", "");
            string folderName = $"{application.AdoxioJobnumber}_{applicationIdCleaned}";
            return folderName;
        }

        /// <summary>
        /// Get a document location by reference
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        private string GetDocumentLocationReferenceByRelativeURL(string relativeUrl)
        {
            string result = null;
            string sanitized = relativeUrl.Replace("'", "''");
            // first see if one exists.
            var locations = _dynamicsClient.SharepointDocumentLocations.Get(filter: "relativeurl eq '" + sanitized + "'");

            var location = locations.Value.FirstOrDefault();

            if (location == null)
            {
                MicrosoftDynamicsCRMsharepointdocumentlocation newRecord = new MicrosoftDynamicsCRMsharepointdocumentlocation()
                {
                    Relativeurl = relativeUrl
                };
                // create a new document location.
                try
                {
                    location = _dynamicsClient.SharepointDocumentLocations.Create(newRecord);
                }
                catch (OdataerrorException odee)
                {
                    _logger.LogError("Error creating document location");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                }
            }

            if (location != null)
            {
                result = location.Sharepointdocumentlocationid;
            }

            return result;
        }


        /// <summary>
        /// Create an Application in Dynamics (POST)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateApplication([FromBody] ViewModels.AdoxioApplication item)
        {

            // for association with current user
            string userJson = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(userJson);
            int count = GetSubmittedCountByApplicant(userSettings.AccountId);
            if (count >= 8)
            {
                return BadRequest("8 applications have already been submitted. Can not create more");
            }
            MicrosoftDynamicsCRMadoxioApplication adoxioApplication = new MicrosoftDynamicsCRMadoxioApplication();
            // copy received values to Dynamics Application
            adoxioApplication.CopyValues(item);
            adoxioApplication.AdoxioApplicanttype = (int?)item.applicantType;
            try
            {
                adoxioApplication = _dynamicsClient.Applications.Create(adoxioApplication);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error creating application");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail if we can't create.
                throw (odee);
            }


            MicrosoftDynamicsCRMadoxioApplication patchAdoxioApplication = new MicrosoftDynamicsCRMadoxioApplication();

            // set license type relationship 

            try
            {
                var adoxioLicencetype = _dynamicsClient.GetAdoxioLicencetypeByName(item.licenseType);
                patchAdoxioApplication.AdoxioLicenceTypeODataBind = _dynamicsClient.GetEntityURI("adoxio_licencetypes", adoxioLicencetype.AdoxioLicencetypeid); ;
                patchAdoxioApplication.AdoxioApplicantODataBind = _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId);
                _dynamicsClient.Applications.Update(adoxioApplication.AdoxioApplicationid, patchAdoxioApplication);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating application");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail if we can't create.
                throw (odee);
            }

            // in case the job number is not there, try getting the record from the server.
            if (adoxioApplication.AdoxioJobnumber == null)
            {
                _logger.LogError("AdoxioJobnumber is null, fetching record again.");
                Guid id = Guid.Parse(adoxioApplication.AdoxioApplicationid);
                adoxioApplication = await _dynamicsClient.GetApplicationById(id);
            }

            if (adoxioApplication.AdoxioJobnumber == null)
            {
                _logger.LogError("Unable to get the Job Number for the Application.");
                throw new Exception("Error creating Licence Application.");
            }

            // create a SharePointDocumentLocation link
            string folderName = GetApplicationFolderName(adoxioApplication);
            string name = adoxioApplication.AdoxioJobnumber + " Files";

            // Create the folder
            bool folderExists = await _sharePointFileManager.FolderExists(ApplicationDocumentListTitle, folderName);
            if (!folderExists)
            {
                var folder = await _sharePointFileManager.CreateFolder(ApplicationDocumentListTitle, folderName);
            }

            // Create the SharePointDocumentLocation entity
            MicrosoftDynamicsCRMsharepointdocumentlocation mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation()
            {
                Relativeurl = folderName,
                Description = "Application Files",
                Name = name
            };


            try
            {
                mdcsdl = _dynamicsClient.SharepointDocumentLocations.Create(mdcsdl);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error creating SharepointDocumentLocation");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                mdcsdl = null;
            }
            if (mdcsdl != null)
            {
                // add a regardingobjectid.
                string applicationReference = _dynamicsClient.GetEntityURI("adoxio_applications", adoxioApplication.AdoxioApplicationid);
                var patchSharePointDocumentLocation = new MicrosoftDynamicsCRMsharepointdocumentlocation();
                patchSharePointDocumentLocation.RegardingobjectidAdoxioApplicationODataBind = applicationReference;
                // set the parent document library.
                string parentDocumentLibraryReference = GetDocumentLocationReferenceByRelativeURL("adoxio_application");
                patchSharePointDocumentLocation.ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference);

                try
                {
                    _dynamicsClient.SharepointDocumentLocations.Update(mdcsdl.Sharepointdocumentlocationid, patchSharePointDocumentLocation);
                }
                catch (OdataerrorException odee)
                {
                    _logger.LogError("Error adding reference SharepointDocumentLocation to application");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                }

                string sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", mdcsdl.Sharepointdocumentlocationid);
                // update the sharePointLocationData.
                Odataid oDataId = new Odataid()
                {
                    OdataidProperty = sharePointLocationData
                };
                try
                {
                    _dynamicsClient.Applications.AddReference(adoxioApplication.AdoxioApplicationid, "adoxio_application_SharePointDocumentLocations", oDataId);
                }
                catch (OdataerrorException odee)
                {
                    _logger.LogError("Error adding reference to SharepointDocumentLocation");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                }
            }

            return Json(await adoxioApplication.ToViewModel(_dynamicsClient));

        }

        /// <summary>
        /// Update a Dynamics Application (PUT)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplication([FromBody] ViewModels.AdoxioApplication item, string id)
        {
            if (id != item.id)
            {
                return BadRequest();
            }

            // for association with current user
            string userJson = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(userJson);


            //Prepare application for update
            Guid adoxio_applicationId = new Guid(id);
            MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await _dynamicsClient.GetApplicationById(adoxio_applicationId);

            if (!CurrentUserHasAccessToApplicationOwnedBy(adoxioApplication._adoxioApplicantValue))
            {
                return new NotFoundResult();
            }

            adoxioApplication = new MicrosoftDynamicsCRMadoxioApplication();

            adoxioApplication.CopyValues(item);

            try
            {
                _dynamicsClient.Applications.Update(id, adoxioApplication);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating application");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail if we can't create.
                throw (odee);
            }

            adoxioApplication = await _dynamicsClient.GetApplicationById(adoxio_applicationId);

            return Json(await adoxioApplication.ToViewModel(_dynamicsClient));
        }

        /// <summary>
        /// Delete an Application.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelApplication(string id)
        {
            // get the application.
            Guid adoxio_applicationid = new Guid(id);

            MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await _dynamicsClient.GetApplicationById(adoxio_applicationid);
            if (adoxioApplication == null)
            {
                return new NotFoundResult();
            }

            if (!CurrentUserHasAccessToApplicationOwnedBy(adoxioApplication._adoxioApplicantValue))
            {
                return new NotFoundResult();
            }

            // set the status to Terminated.
            MicrosoftDynamicsCRMadoxioApplication patchRecord = new MicrosoftDynamicsCRMadoxioApplication()
            {
                //StatusCodeODataBind = ((int)AdoxioApplicationStatusCodes.Terminated).ToString()
                Statuscode = (int)AdoxioApplicationStatusCodes.Terminated
            };

            try
            {
                _dynamicsClient.Applications.Update(id, patchRecord);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error cancelling application");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail if we can't create.
                throw (odee);
            }


            return NoContent(); // 204
        }

        /// <summary>
        /// Delete an Application.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteApplication(string id)
        {
            // get the application.
            Guid adoxio_applicationid = new Guid(id);

            MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await _dynamicsClient.GetApplicationById(adoxio_applicationid);
            if (adoxioApplication == null)
            {
                return new NotFoundResult();
            }

            if (!CurrentUserHasAccessToApplicationOwnedBy(adoxioApplication._adoxioApplicantValue))
            {
                return new NotFoundResult();
            }


            await _dynamicsClient.Applications.DeleteAsync(adoxio_applicationid.ToString());

            return NoContent(); // 204
        }

        /// <summary>
        /// Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
        private bool CurrentUserHasAccessToApplicationOwnedBy(string accountId)
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
