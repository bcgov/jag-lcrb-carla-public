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
    //[Authorize(Policy = "Business-User")]
    public class FileController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SharePointFileManager _sharePointFileManager;
        private readonly ILogger _logger;
        private readonly IDynamicsClient _dynamicsClient;

        public FileController(SharePointFileManager sharePointFileManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            Configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _sharePointFileManager = sharePointFileManager;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(AdoxioLegalEntityController));
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

        [HttpPost("{entityId}/attachments/{entityName}")]
        // allow large uploads
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile([FromRoute] string entityId, [FromRoute] string entityName,
            [FromForm]IFormFile file, [FromForm] string documentType)
        {
            ViewModels.FileSystemItem result = null;
            ValidateSession();

            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType))
            {
                return BadRequest();
            }

            var hasAccess = await CanAccessEntity(entityName, entityId);
            if (!hasAccess)
            {
                return new NotFoundResult();
            }

            // Update modifiedon to current time
            UpdateEntityModifiedOnDate(entityName, entityId);

            string fileName = FileSystemItemExtensions.CombineNameDocumentType(file.FileName, documentType);
            string folderName = await GetFolderName(entityName, entityId, documentType);
            try
            {
                await _sharePointFileManager.AddFile(GetDocumentListTitle(entityName), folderName, fileName, file.OpenReadStream(), file.ContentType);
            }
            catch (SharePointRestException ex)
            {
                _logger.LogError("Error uploading file to SharePoint");
                _logger.LogError(ex.Response.Content);
                _logger.LogError(ex.Message);
                return new NotFoundResult();
            }
            return Json(result);
        }

        private async Task<bool> CanAccessEntity(string entityName, string entityId)
        {
            var result = false;
            switch(entityName.ToLower())
            {
                case "application":
                    var applicationId = Guid.Parse(entityId);
                    var application = await _dynamicsClient.GetApplicationById(applicationId);
                    result = application != null && CurrentUserHasAccessToApplicationOwnedBy(application._adoxioApplicantValue);
                    break;
                default:
                    break;
            }
            return result;
        }

        private async Task<string> GetFolderName(string entityName, string entityId, string documentType)
        {
            var folderName = "";
            switch (entityName.ToLower())
            {
                case "application":
                    var application = await _dynamicsClient.GetApplicationById(Guid.Parse(entityId));
                    var applicationIdCleaned = application.AdoxioApplicationid.ToString().ToUpper().Replace("-", "");
                    folderName = GetApplicationFolderName(application);
                    break;
                default:
                    break;
            }

            return "";
        }

        private void UpdateEntityModifiedOnDate(string entityName, string entityId)
        {
            switch (entityName.ToLower())
            {
                case "application":
                    var patchApplication = new MicrosoftDynamicsCRMadoxioApplication();
                    try
                    {
                        _dynamicsClient.Applications.Update(entityId, patchApplication);
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
                    break;
                default:
                    break;
            }
        }

        private string GetDocumentListTitle(string entityName)
        {
            switch (entityName.ToLower())
            {
                case "application":
                    return "adoxio_application";
                default:
                    return null;
            }
        }

        private void ValidateSession()
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();
        }

        [HttpGet("download-file/{applicationId}")]
        public async Task<IActionResult> DownloadFile(string applicationId, [FromQuery]string serverRelativeUrl)
        {
            // get the file.
            if (string.IsNullOrEmpty(serverRelativeUrl) || string.IsNullOrEmpty(applicationId))
            {
                return BadRequest();
            }
            else
            {
                var application = await _dynamicsClient.GetApplicationById(Guid.Parse(applicationId));

                if (application == null)
                {
                    return new NotFoundResult();
                }

                if (!CurrentUserHasAccessToApplicationOwnedBy(application._adoxioApplicantValue))
                {
                    return new NotFoundResult();
                }

                byte[] fileContents = await _sharePointFileManager.DownloadFile(serverRelativeUrl);
                return new FileContentResult(fileContents, "application/octet-stream")
                {
                };
            }
        }

        /// <summary>
        /// Get the file details list in folder associated to the application folder and document type
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        [HttpGet("{entityId}/attachments/{entityName}/{documentType}")]
        public async Task<IActionResult> GetFileDetailsListInFolder([FromRoute] string entityId, [FromRoute] string entityName, [FromRoute] string documentType)
        {
            List<ViewModels.FileSystemItem> fileSystemItemVMList = new List<ViewModels.FileSystemItem>();

            ValidateSession();

            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType))
            {
                return BadRequest();
            }

            var hasAccess = await CanAccessEntity(entityName, entityId);
            if (!hasAccess)
            {
                return new NotFoundResult();
            }

            string folderName = await GetFolderName(entityName, entityId, documentType); ;
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
        [HttpDelete("{applicationId}/attachments/{entityName}")]
        public async Task<IActionResult> DeleteFile([FromQuery] string serverRelativeUrl, [FromRoute] string applicationId, [FromRoute] string entityName)
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
