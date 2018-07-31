using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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
    public class AdoxioApplicationController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SharePointFileManager _sharePointFileManager;
        private readonly ILogger _logger;
        private readonly IDynamicsClient _dynamicsClient;

        public AdoxioApplicationController(SharePointFileManager sharePointFileManager, IConfiguration configuration, IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            Configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _sharePointFileManager = sharePointFileManager;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(AdoxioLegalEntityController));
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
                dynamicsApplicationList = _dynamicsClient.Applications.Get(filter: "_adoxio_applicant_value eq " + applicantId).Value;
            }

            if (dynamicsApplicationList != null)
            {
                foreach (MicrosoftDynamicsCRMadoxioApplication dynamicsApplication in dynamicsApplicationList)
                {
                    result.Add(await dynamicsApplication.ToViewModel(_dynamicsClient));
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

        [HttpPost("{id}/attachments")]
        public async Task<IActionResult> UploadFile([FromRoute] string id, [FromForm]IFormFile file, [FromForm] string documentType)
        {
            ViewModels.FileSystemItem result = null;
            // get the LegalEntity.
            // Adoxio_legalentity legalEntity = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();

            if (id != null)
            {
                var applicationId = Guid.Parse(id);
                var application = await _dynamicsClient.GetApplicationById(applicationId);

                if (application == null)
                {
                    return new NotFoundResult();
                }

                if (!CurrentUserHasAccessToApplicationOwnedBy(application._adoxioApplicantValue))
                {
                    return new NotFoundResult();
                }

                // Update modifiedon to current time
                var patchApplication = new MicrosoftDynamicsCRMadoxioApplication();
                try
                {
                    _dynamicsClient.Applications.Update(id, patchApplication);
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

                string fileName = FileSystemItemExtensions.CombineNameDocumentType(file.FileName, documentType);
                var applicationIdCleaned = application.AdoxioApplicationid.ToString().ToUpper().Replace("-", "");
                // Dynamics code for the name is {Code(Licence Type (Licence Type))} - {Business Type(Application)} - {Job Number(Application)} 
                //string folderName = $"{application.AdoxioLicenceType.AdoxioCode} - {application.AdoxioApplicant.AdoxioBusinesstype}_{applicationIdCleaned}";
                string folderName = GetApplicationFolderName(application);
                try
                {
                    await _sharePointFileManager.AddFile(ApplicationDocumentListTitle, folderName, fileName, file.OpenReadStream(), file.ContentType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.StackTrace);
                    return new NotFoundResult();
                }
            }
            return Json(result);
        }

        [HttpGet("download-file/{serverRelativeUrl}")]
        public async Task<IActionResult> DownloadFile(string serverRelativeUrl)
        {
            // get the file.
            if (string.IsNullOrEmpty(serverRelativeUrl))
            {
                return BadRequest();
            }
            else
            {

                byte[] fileContents = await _sharePointFileManager.DownloadFile(serverRelativeUrl);
                return new FileContentResult(fileContents, "application/octet-stream")
                {
                };
            }
        }

        /// <summary>
        /// Get the file details list in folder associated to the application folder and document type
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        [HttpGet("{applicationId}/attachments/{documentType}")]
        public async Task<IActionResult> GetFileDetailsListInFolder([FromRoute] string applicationId, [FromRoute] string documentType)
        {
            List<ViewModels.FileSystemItem> fileSystemItemVMList = new List<ViewModels.FileSystemItem>();

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();

            // validate that the user account id matches the applicant for this application
            var applicationGUID = new Guid(applicationId);
            var application = await _dynamicsClient.GetApplicationById(applicationGUID);

            if (!DynamicsExtensions.CurrentUserHasAccessToAccount(new Guid(application._adoxioApplicantValue), _httpContextAccessor, _dynamicsClient))
            {
                return NotFound();
            }

            if (application != null)
            {
                string folderName = GetApplicationFolderName( application );
                // Get the file details list in folder
                List<FileDetailsList> fileDetailsList = null;
                try
                {
                    fileDetailsList = await _sharePointFileManager.GetFileDetailsListInFolder(SharePointFileManager.ApplicationDocumentListTitle, folderName, documentType);
                }
                catch (SharePointRestException spre)
                {
                    _logger.LogError("Error getting SharePoint File List");
                    _logger.LogError("Request URI:");
                    _logger.LogError(spre.Request.RequestUri.ToString());
                    _logger.LogError("Response:");
                    _logger.LogError(spre.Response.Content);
                    throw new Exception("Unable to get Sharepoint File List.");
                }

                if (fileDetailsList != null)
                {
                    foreach (FileDetailsList fileDetails in fileDetailsList)
                    {
                        ViewModels.FileSystemItem fileSystemItemVM = new ViewModels.FileSystemItem();
                        // remove the document type text from file name
                        fileSystemItemVM.name = fileDetails.Name.Substring(fileDetails.Name.IndexOf("__") + 2);
                        // convert size from bytes (original) to KB
                        fileSystemItemVM.size = int.Parse(fileDetails.Length);
                        fileSystemItemVM.serverrelativeurl = fileDetails.ServerRelativeUrl;
                        fileSystemItemVM.timelastmodified = DateTime.Parse(fileDetails.TimeLastModified);
                        fileSystemItemVM.documenttype = fileDetails.DocumentType;
                        fileSystemItemVMList.Add(fileSystemItemVM);
                    }
                }
            }

            else
            {
                _logger.LogError("Application not found.");
                return new NotFoundResult();
            }

            return Json(fileSystemItemVMList);
        }

        /// <summary>
        /// Delete a file.
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <param name="serverRelativeUrl">The ServerRelativeUrl to delete</param>
        /// <returns></returns>
        [HttpDelete("{applicationId}/attachments")]
        public async Task<IActionResult> DeleteFile([FromQuery] string serverRelativeUrl, [FromRoute] string applicationId)
        {
            // get the file.
            if (applicationId == null || serverRelativeUrl == null)
            {
                return BadRequest();
            }
            else
            {
                // get the current user.
                string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
                UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
                // check that the session is setup correctly.
                userSettings.Validate();

                // validate that the user account id matches the applicant for this application
                var applicationGUID = new Guid(applicationId);
                var application = await _dynamicsClient.GetApplicationById(applicationGUID);

                if (!CurrentUserHasAccessToApplicationOwnedBy(application._adoxioApplicantValue))
                {
                    return new NotFoundResult();
                }

                // Update modifiedon to current time
               var patchApplication = new MicrosoftDynamicsCRMadoxioApplication();
                try
                {
                    _dynamicsClient.Applications.Update(applicationId, patchApplication);
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

                var result = await _sharePointFileManager.DeleteFile(serverRelativeUrl);
                if (result)
                {
                    return new OkResult();
                }
            }
            return new NotFoundResult();
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
