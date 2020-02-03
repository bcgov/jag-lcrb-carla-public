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
using Grpc.Core;
using Gov.Lclb.Cllb.Services.FileManager;
using System.IO;
using Google.Protobuf;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;

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
        private readonly FileManagerClient _fileManagerClient;


        public FileController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, FileManagerClient fileClient)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(FileController));

            _fileManagerClient = fileClient;
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
            var hasAccess = await CanAccessEntity(entityName, entityId);
            if (!hasAccess)
            {
                return new NotFoundResult();
            }

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

            MemoryStream ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            byte[] data = ms.ToArray();

            // call the web service
            var uploadRequest = new UploadFileRequest() { 
                 ContentType = file.ContentType,
                 Data = ByteString.CopyFrom(data),
                 EntityName = entityName,
                 FileName = fileName,
                 FolderName = folderName
            };

            var uploadResult = _fileManagerClient.UploadFile(uploadRequest);

            if (uploadResult.ResultStatus == ResultStatus.Success)
            {
                // Update modifiedon to current time
                UpdateEntityModifiedOnDate(entityName, entityId, true);
                _logger.LogInformation($"SUCCESS in uploading file {fileName} to folder {folderName}");
            }
            else
            {
                _logger.LogError($"ERROR in uploading file {fileName} to folder {folderName}");
                throw new Exception($"ERROR in uploading file {fileName} to folder {folderName}");
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
                    var account = await _dynamicsClient.GetAccountByIdAsync(id);
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
            bool hasFile = false;
            
            var fileExistsRequest = new FileExistsRequest()
            {
                DocumentType = documentType,
                EntityId = entityId,
                EntityName = entityName,
                FolderName = await GetFolderName(entityName, entityId, _dynamicsClient),
                ServerRelativeUrl = serverRelativeUrl
            };
            
            var hasFileResult = _fileManagerClient.FileExists( fileExistsRequest );

            if (hasFileResult.ResultStatus == FileExistStatus.Exist)
            {
                // Update modifiedon to current time
                hasFile = true;
            }
            else
            {
                _logger.LogError($"Unexpected error - Unable to validate file - {serverRelativeUrl}");                
            }
            
            return result && hasFile;
        }


        public static async Task<string> GetFolderName(string entityName, string entityId, IDynamicsClient _dynamicsClient)
        {
            var folderName = "";
            switch (entityName.ToLower())
            {
                case "account":
                    var account = await _dynamicsClient.GetAccountByIdAsync(Guid.Parse(entityId));
                    folderName = account.GetDocumentFolderName();
                    break;
                case "application":
                    var application = await _dynamicsClient.GetApplicationById(Guid.Parse(entityId));
                    folderName = application.GetDocumentFolderName();
                    break;
                case "contact":
                    var contact = await _dynamicsClient.GetContactById(Guid.Parse(entityId));
                    folderName = contact.GetDocumentFolderName();
                    break;
                case "worker":
                    var worker = await _dynamicsClient.GetWorkerById(Guid.Parse(entityId));
                    folderName = worker.GetDocumentFolderName();
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
                        throw;
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
                        throw;
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
                return BadRequest();
            }

            // call the web service
            var downloadRequest = new DownloadFileRequest()
            {
                 ServerRelativeUrl = serverRelativeUrl
            };

            var downloadResult = _fileManagerClient.DownloadFile(downloadRequest);

            if (downloadResult.ResultStatus == ResultStatus.Success)
            {
                // Update modifiedon to current time
                UpdateEntityModifiedOnDate(entityName, entityId, true);
                _logger.LogInformation($"SUCCESS in getting file {serverRelativeUrl}");
                byte[] fileContents = downloadResult.Data.ToByteArray();
                return new FileContentResult(fileContents, "application/octet-stream")
                {
                };
            }
            else
            {
                _logger.LogError($"ERROR in getting file {serverRelativeUrl} - {downloadResult.ErrorDetail}");
                throw new Exception($"ERROR in getting file {serverRelativeUrl} - {downloadResult.ErrorDetail}");
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
            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType))
            {
                return BadRequest();
            }

            ValidateSession();

            List<ViewModels.FileSystemItem> fileSystemItemVMList = await GetListFilesInFolder(entityId, entityName, documentType);

            var hasAccess = await CanAccessEntity(entityName, entityId);
            if (!hasAccess)
            {
                return new NotFoundResult();
            }

            return new JsonResult(fileSystemItemVMList);
        }

        [HttpGet("{entityName}/{entityId}/documentStatus/{formId}")]
        public async Task<IActionResult> GetDocumentTypeStatus (string entityName, string entityId, string formId)
        {
            List<ViewModels.DocumentTypeStatus> result = null;

            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(formId))
            {
                return BadRequest();
            }

            // lookup the entity
            string folderName = null;
            switch (entityName.ToLower())
            {
                case "account":
                    MicrosoftDynamicsCRMaccount account = _dynamicsClient.GetAccountById(entityId);
                    folderName = account.GetDocumentFolderName();
                    break;

                case "application":
                    var application = await _dynamicsClient.GetApplicationById(entityId);
                    folderName = application.GetDocumentFolderName();
                    break;

                default:
                    break;
            }
            
            if (folderName != null)
            {
                var folderContents = _fileManagerClient.GetFileDetailsListInFolder(_logger, entityName, entityId, folderName);

                // get any file form fields that are related to the form
                var formFileFields = _dynamicsClient.Formelementuploadfields.GetDocumentFieldsByForm(formId);
                
                result = new List<ViewModels.DocumentTypeStatus>();

                foreach (var formFileField in formFileFields)
                {
                    string documentTypePrefix = formFileField.AdoxioFileprefix;
                    string documentTypeName = formFileField.AdoxioName;
                    string routerLink = formFileField.AdoxioRouterlink;

                    bool valid = false;

                    // determine if there are any files with this prefix.

                    var firstMatch = folderContents.FirstOrDefault(f => f.documenttype == documentTypePrefix);

                    if (firstMatch != null)
                    {
                        valid = true;
                    }

                    result.Add(new ViewModels.DocumentTypeStatus() { DocumentType = documentTypePrefix, Name = documentTypeName, Valid = valid, RouterLink=routerLink });
                }

            }


            // ensure that the entity has files.
            if (result == null)
            {
                return BadRequest();
            }
            else
            {
                return new JsonResult(result);
            }
            
        }

        private async Task<List<ViewModels.FileSystemItem>> GetListFilesInFolder(string entityId, string entityName, string documentType)
        {
            List<ViewModels.FileSystemItem> fileSystemItemVMList = new List<ViewModels.FileSystemItem>();

            ValidateSession();


            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType))
            {
                return fileSystemItemVMList;
            }

            try
            {
                // call the web service
                var request = new FolderFilesRequest()
                {
                    DocumentType = documentType,
                    EntityId = entityId,
                    EntityName = entityName,
                    FolderName = await GetFolderName(entityName, entityId, _dynamicsClient) 
            };

                var result = _fileManagerClient.FolderFiles(request);

                if (result.ResultStatus == ResultStatus.Success)
                {
                    // convert the results to the view model.
                    foreach (var fileDetails in result.Files)
                    {
                        ViewModels.FileSystemItem fileSystemItemVM = new ViewModels.FileSystemItem()
                        {
                            // remove the document type text from file name
                            name = fileDetails.Name.Substring(fileDetails.Name.IndexOf("__") + 2),
                            // convert size from bytes (original) to KB
                            size = fileDetails.Size,
                            serverrelativeurl = fileDetails.ServerRelativeUrl,
                            //timelastmodified = fileDetails.TimeLastModified.ToDateTime(),
                            documenttype = fileDetails.DocumentType
                        };

                        fileSystemItemVMList.Add(fileSystemItemVM);
                    }

                }
                else
                {
                    _logger.LogError($"ERROR in getting folder files for entity {entityName}, entityId {entityId}, docuemnt type {documentType} ");                    
                }


                
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting SharePoint File List");
            }

            return fileSystemItemVMList;
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

            // call the web service
            var deleteRequest = new DeleteFileRequest()
            {
                ServerRelativeUrl = serverRelativeUrl
            };

            var deleteResult = _fileManagerClient.DeleteFile(deleteRequest);

            if (deleteResult.ResultStatus == ResultStatus.Success)
            {
                // Update modifiedon to current time
                UpdateEntityModifiedOnDate(entityName, entityId, true);
                _logger.LogInformation($"SUCCESS in deleting file {serverRelativeUrl}");
                return new OkResult();
            }
            else
            {
                _logger.LogError($"ERROR in deleting file {serverRelativeUrl} - {deleteResult.ErrorDetail}");
                throw new Exception($"ERROR in deleting file {serverRelativeUrl} - {deleteResult.ErrorDetail}");
            }
            
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


    }
}
