using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    // No authorize policy as this can be used by workers to upload files.
    public class FileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IDynamicsClient _dynamicsClient;

        public FileController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(FileController));
        }



        private static string GetAccountFolderName(MicrosoftDynamicsCRMaccount account)
        {
            string accountIdCleaned = account.Accountid.ToString().ToUpper().Replace("-", "");
            string folderName = $"{account.Accountid}_{accountIdCleaned}";
            return folderName;
        }

        private static string GetApplicationFolderName(MicrosoftDynamicsCRMadoxioApplication application)
        {
            string applicationIdCleaned = application.AdoxioApplicationid.ToString().ToUpper().Replace("-", "");
            string folderName = $"{application.AdoxioJobnumber}_{applicationIdCleaned}";
            return folderName;
        }

        private static string GetContactFolderName(MicrosoftDynamicsCRMcontact contact)
        {
            string applicationIdCleaned = contact.Contactid.ToString().ToUpper().Replace("-", "");
            string folderName = $"contact_{applicationIdCleaned}";
            return folderName;
        }

        private static string GetWorkerFolderName(MicrosoftDynamicsCRMadoxioWorker worker)
        {
            string applicationIdCleaned = worker.AdoxioWorkerid.ToString().ToUpper().Replace("-", "");
            
            string folderName = $"{worker.AdoxioName}_{applicationIdCleaned}";
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
            var locations = _dynamicsClient.Sharepointdocumentlocations.Get(filter: "relativeurl eq '" + sanitized + "'");

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
                    location = _dynamicsClient.Sharepointdocumentlocations.Create(newRecord);
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.LogError(httpOperationException, "Error creating document location");
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

            await CreateDocumentLibraryIfMissing(GetDocumentListTitle(entityName), GetDocumentTemplateUrlPart(entityName));

            var hasAccess = await CanAccessEntity(entityName, entityId);
            if (!hasAccess)
            {
                return new NotFoundResult();
            }

            // Update modifiedon to current time
            UpdateEntityModifiedOnDate(entityName, entityId, true);

            // Sanitize file name
            Regex illegalInFileName = new Regex(@"[#%*<>?{}~¿""]");
            string fileName = illegalInFileName.Replace(file.FileName, "");
            illegalInFileName = new Regex(@"[&:/\\|]");
            fileName = illegalInFileName.Replace(fileName, "-");

            fileName = FileSystemItemExtensions.CombineNameDocumentType(fileName, documentType);
            string folderName = null;

            folderName = await GetEntitySharePointDocumentationLocation(entityName, entityId);

            if (folderName == null)
            {
                folderName = await GetFolderName(entityName, entityId, _dynamicsClient);
            }
            SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);
            string headers = LoggingEvents.GetHeaders(Request);
            try
            {
                fileName = await _sharePointFileManager.AddFile(GetDocumentTemplateUrlPart(entityName), folderName, fileName, file.OpenReadStream(), file.ContentType);
                result = new ViewModels.FileSystemItem()
                {
                    name = fileName
                };

                _logger.LogInformation($"SUCCESS in uploading file {fileName} to folder {folderName} Headers: {headers} ");
            }
            catch (SharePointRestException ex)
            {
                _logger.LogError($"ERROR in uploading file {fileName} to folder {folderName} Headers: {headers} - SharePointRestException - {ex.Message} {ex.Response.Content}");
                return new NotFoundResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR in uploading file {fileName} to folder {folderName} Headers: {headers} Unexpected Exception {e.ToString()} {e.Message} {e.StackTrace.ToString()}");
            }
            return new JsonResult(result);
        }

        private async Task<string> GetEntitySharePointDocumentationLocation(string entityName, string entityId)
        {
            string result = null;
            var id = Guid.Parse(entityId);
            switch (entityName.ToLower())
            {
                case "worker":
                    var worker = await _dynamicsClient.GetWorkerByIdWithChildren(entityId);
                    var location = worker.AdoxioWorkerSharePointDocumentLocations.FirstOrDefault();
                    if (location != null && ! string.IsNullOrEmpty(location.Relativeurl))
                    {
                        result = location.Relativeurl;
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        private async Task<bool> CanAccessEntity(string entityName, string entityId)
        {
            var result = false;
            var id = Guid.Parse(entityId);
            switch (entityName.ToLower())
            {
                case "account":
                    var account = await _dynamicsClient.GetAccountById(id);
                    result = account != null && CurrentUserHasAccessToApplicationOwnedBy(account.Accountid);
                    break;
                case "application":
                    var application = await _dynamicsClient.GetApplicationById(id);
                    result = application != null && CurrentUserHasAccessToApplicationOwnedBy(application._adoxioApplicantValue);
                    break;
                case "contact":
                    var contact = await _dynamicsClient.GetContactById(id);
                    result = contact != null && CurrentUserHasAccessToContactOwnedBy(contact.Contactid);
                    break;
                case "worker":
                    var worker = await _dynamicsClient.GetWorkerById(id);
                    result = worker != null && CurrentUserHasAccessToContactOwnedBy(worker._adoxioContactidValue);
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


        public static async Task<string> GetFolderName(string entityName, string entityId, IDynamicsClient _dynamicsClient)
        {
            var folderName = "";
            switch (entityName.ToLower())
            {
                case "account":
                    var account = await _dynamicsClient.GetAccountById(Guid.Parse(entityId));
                    folderName = GetAccountFolderName(account);
                    break;
                case "application":
                    var application = await _dynamicsClient.GetApplicationById(Guid.Parse(entityId));
                    folderName = GetApplicationFolderName(application);
                    break;
                case "contact":
                    var contact = await _dynamicsClient.GetContactById(Guid.Parse(entityId));
                    folderName = GetContactFolderName(contact);
                    break;
                case "worker":
                    var worker = await _dynamicsClient.GetWorkerById(Guid.Parse(entityId));
                    folderName = GetWorkerFolderName(worker);
                    break;
                default:
                    break;
            }
            return folderName;
        }

        private void UpdateEntityModifiedOnDate(string entityName, string entityId, bool setUploadedFromPortal = false)
        {
            switch (entityName.ToLower())
            {
                case "application":
                    var patchApplication = new MicrosoftDynamicsCRMadoxioApplication();
                    if (setUploadedFromPortal)
                    {
                        patchApplication.AdoxioFileuploadedfromportal = (int?)ViewModels.GeneralYesNo.Yes;
                    }

                    try
                    {
                        _dynamicsClient.Applications.Update(entityId, patchApplication);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error updating application");
                        // fail if we can't create.
                        throw (httpOperationException);
                    }
                    break;
                case "contact":
                    var patchContact = new MicrosoftDynamicsCRMcontact();
                    try
                    {
                        _dynamicsClient.Contacts.Update(entityId, patchContact);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error updating Contact");
                        // fail if we can't create.
                        throw (httpOperationException);
                    }
                    break;
                case "worker":
                    var patchWorker = new MicrosoftDynamicsCRMadoxioWorker();
                    try
                    {
                        _dynamicsClient.Workers.Update(entityId, patchWorker);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error updating Contact");
                        // fail if we can't create.
                        throw (httpOperationException);
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
            SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);
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

            return new JsonResult(fileSystemItemVMList);
        }

        private async Task<List<ViewModels.FileSystemItem>> getFileDetailsListInFolder(string entityId, string entityName, string documentType)
        {
            List<ViewModels.FileSystemItem> fileSystemItemVMList = new List<ViewModels.FileSystemItem>();

            ValidateSession();


            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType))
            {
                return fileSystemItemVMList;
            }

            try
            {
                await CreateDocumentLibraryIfMissing(GetDocumentListTitle(entityName), GetDocumentTemplateUrlPart(entityName));

                string folderName = await GetFolderName(entityName, entityId, _dynamicsClient); ;
                // Get the file details list in folder
                List<Interfaces.SharePointFileManager.FileDetailsList> fileDetailsList = null;
                SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);
                try
                {
                    fileDetailsList = await _sharePointFileManager.GetFileDetailsListInFolder(GetDocumentTemplateUrlPart(entityName), folderName, documentType);
                }
                catch (SharePointRestException spre)
                {
                    _logger.LogError(spre, "Error getting SharePoint File List");
                    throw new Exception("Unable to get Sharepoint File List.");
                }

                if (fileDetailsList != null)
                {
                    foreach (Interfaces.SharePointFileManager.FileDetailsList fileDetails in fileDetailsList)
                    {
                        ViewModels.FileSystemItem fileSystemItemVM = new ViewModels.FileSystemItem()
                        {
                            // remove the document type text from file name
                            name = fileDetails.Name.Substring(fileDetails.Name.IndexOf("__") + 2),
                            // convert size from bytes (original) to KB
                            size = int.Parse(fileDetails.Length),
                            serverrelativeurl = fileDetails.ServerRelativeUrl,
                            timelastmodified = DateTime.Parse(fileDetails.TimeLastModified),
                            documenttype = fileDetails.DocumentType
                        };

                        fileSystemItemVMList.Add(fileSystemItemVM);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting SharePoint File List");

                _logger.LogError(e.Message);
            }



            return fileSystemItemVMList;
        }


        public static string GetDocumentListTitle(string entityName)
        {
            var listTitle = "";
            switch (entityName.ToLower())
            {
                case "account":
                    listTitle = SharePointFileManager.DefaultDocumentListTitle;
                    break;
                case "application":
                    listTitle = SharePointFileManager.ApplicationDocumentListTitle;
                    break;
                case "contact":
                    listTitle = SharePointFileManager.ContactDocumentListTitle;
                    break;
                case "worker":
                    listTitle = SharePointFileManager.WorkertDocumentListTitle;
                    break;
                default:
                    break;
            }
            return listTitle;
        }

        public static string GetDocumentTemplateUrlPart(string entityName)
        {
            var listTitle = "";
            switch (entityName.ToLower())
            {
                case "account":
                    listTitle = SharePointFileManager.DefaultDocumentListTitle;
                    break;
                case "application":
                    listTitle = "adoxio_application";
                    break;
                case "contact":
                    listTitle = SharePointFileManager.ContactDocumentListTitle;
                    break;
                case "worker":
                    listTitle = "adoxio_worker";
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
            SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);
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


        private async Task CreateDocumentLibraryIfMissing(string listTitle, string documentTemplateUrl = null)
        {
            SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);
            var exists = await _sharePointFileManager.DocumentLibraryExists(listTitle);
            if (!exists)
            {
                await _sharePointFileManager.CreateDocumentLibrary(listTitle, documentTemplateUrl);
            }
        }
    }
}
