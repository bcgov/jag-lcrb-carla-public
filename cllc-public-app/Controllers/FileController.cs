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
            _logger = loggerFactory.CreateLogger(typeof(FileController));
        }



        private string GetApplicationFolderName(MicrosoftDynamicsCRMadoxioApplication application)
        {
            string applicationIdCleaned = application.AdoxioApplicationid.ToString().ToUpper().Replace("-", "");
            string folderName = $"{application.AdoxioJobnumber}_{applicationIdCleaned}";
            return folderName;
        }

        private string GetContactFolderName(MicrosoftDynamicsCRMcontact contact)
        {
            string applicationIdCleaned = contact.Contactid.ToString().ToUpper().Replace("-", "");
            string folderName = $"{contact.Fullname}_{applicationIdCleaned}";
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

            CreateDocumentLibraryIfMissing(GetDocumentListTitle(entityName));

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
            var id = Guid.Parse(entityId);
            switch (entityName.ToLower())
            {
                case "application":
                    var application = await _dynamicsClient.GetApplicationById(id);
                    result = application != null && CurrentUserHasAccessToApplicationOwnedBy(application._adoxioApplicantValue);
                    break;
                case "contact":
                    var contact = await _dynamicsClient.GetContactById(id);
                    result = contact != null && CurrentUserHasAccessToContactOwnedBy(contact.Contactid);
                    break;
                default:
                    break;
            }
            return result;
        }
        private async Task<bool> CanAccessEntityFile(string entityName, string entityId, string documentType, string serverRelativeUrl)
        {
            var result = await CanAccessEntity(entityName, entityId);
            //get list of files for entity
            var files = await getFileDetailsListInFolder(entityId, entityName, documentType);
            //confirm the serverRelativeUrl is in one of the files
            var hasFile = files.Any(f => f.serverrelativeurl == serverRelativeUrl);
            return result && hasFile;
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
                case "contact":
                    var contact = await _dynamicsClient.GetContactById(Guid.Parse(entityId));
                    var cleanId = contact.Contactid.ToString().ToUpper().Replace("-", "");
                    folderName = GetContactFolderName(contact);
                    break;
                default:
                    break;
            }
            return folderName;
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
                case "contact":
                    var patchContact = new MicrosoftDynamicsCRMcontact();
                    try
                    {
                        _dynamicsClient.Contacts.Update(entityId, patchContact);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating Contact");
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


        private void ValidateSession()
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();
        }

        [HttpGet("{entityId}/download-file/{entityName}/{fileName}")]
        public async Task<IActionResult> DownloadFile(string entityId, string entityName, [FromQuery]string serverRelativeUrl, [FromQuery]string documentType)
        {
            // get the file.
            if (string.IsNullOrEmpty(serverRelativeUrl) || string.IsNullOrEmpty(documentType) || string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName))
            {
                return BadRequest();
            }

            ValidateSession();

            var hasAccess = await CanAccessEntityFile(entityName, entityId, documentType, serverRelativeUrl);
            if (!hasAccess)
            {
                return new NotFoundResult();
            }

            // Update modifiedon to current time
            UpdateEntityModifiedOnDate(entityName, entityId);

            byte[] fileContents = await _sharePointFileManager.DownloadFile(serverRelativeUrl);
            return new FileContentResult(fileContents, "application/octet-stream")
            {
            };

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
            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType))
            {
                return BadRequest();
            }

            ValidateSession();

            List<ViewModels.FileSystemItem> fileSystemItemVMList = await getFileDetailsListInFolder(entityId, entityName, documentType);

            var hasAccess = await CanAccessEntity(entityName, entityId);
            if (!hasAccess)
            {
                return new NotFoundResult();
            }

            return Json(fileSystemItemVMList);
        }

        private async Task<List<ViewModels.FileSystemItem>> getFileDetailsListInFolder(string entityId, string entityName, string documentType)
        {
            List<ViewModels.FileSystemItem> fileSystemItemVMList = new List<ViewModels.FileSystemItem>();

            ValidateSession();

            CreateDocumentLibraryIfMissing(GetDocumentListTitle(entityName));

            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType))
            {
                return fileSystemItemVMList;
            }

            string folderName = await GetFolderName(entityName, entityId, documentType); ;
            // Get the file details list in folder
            List<FileDetailsList> fileDetailsList = null;
            try
            {
                fileDetailsList = await _sharePointFileManager.GetFileDetailsListInFolder(GetDocumentListTitle(entityName), folderName, documentType);
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

            return fileSystemItemVMList;
        }


        private string GetDocumentListTitle(string entityName)
        {
            var listTitle = "";
            switch (entityName.ToLower())
            {
                case "application":
                    listTitle = SharePointFileManager.ApplicationDocumentListTitle;
                    break;
                case "contact":
                    listTitle = SharePointFileManager.ContactDocumentListTitle;
                    break;
                default:
                    break;
            }
            return listTitle;
        }

        /// <summary>
        /// Delete a file.
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <param name="serverRelativeUrl">The ServerRelativeUrl to delete</param>
        /// <returns></returns>
        [HttpDelete("{entityId}/attachments/{entityName}")]
        public async Task<IActionResult> DeleteFile([FromQuery] string serverRelativeUrl, [FromQuery] string documentType, [FromRoute] string entityId, [FromRoute] string entityName)
        {
            // get the file.
            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(serverRelativeUrl) || string.IsNullOrEmpty(documentType))
            {
                return BadRequest();
            }

            ValidateSession();

            var hasAccess = await CanAccessEntityFile(entityName, entityId, documentType, serverRelativeUrl);
            if (!hasAccess)
            {
                return new NotFoundResult();
            }

            // Update modifiedon to current time
            UpdateEntityModifiedOnDate(entityName, entityId);

            var result = await _sharePointFileManager.DeleteFile(serverRelativeUrl);
            if (result)
            {
                return new OkResult();
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

        /// <summary>
        /// Verify whether currently logged in user has access to this contact id
        /// </summary>
        /// <returns>boolean</returns>
        private bool CurrentUserHasAccessToContactOwnedBy(string contactId)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // For now, check if the account id matches the user's account.
            // TODO there may be some account relationships in the future
            if (userSettings.ContactId != null && userSettings.ContactId.Length > 0)
            {
                return userSettings.ContactId == contactId;
            }

            // if current user doesn't have an account they are probably not logged in
            return false;
        }


        private async void CreateDocumentLibraryIfMissing(string listTitle)
        {
            var exists = await _sharePointFileManager.DocumentLibraryExists(listTitle);
            if (!exists)
            {
                await _sharePointFileManager.CreateDocumentLibrary(listTitle);
            }
        }
    }
}
