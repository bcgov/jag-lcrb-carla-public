extern alias DV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Protobuf;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Extensions;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Gov.Lclb.Cllb.Services.FileManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Winista.Mime;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DvApplication = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application;
using DvWorker = DV::Gov.Lclb.Cllb.Interfaces.adoxio_worker;
using DvContact = DV::Gov.Lclb.Cllb.Interfaces.Contact;
using DvAccount = DV::Gov.Lclb.Cllb.Interfaces.Account;
using DvEvent = DV::Gov.Lclb.Cllb.Interfaces.adoxio_event;
using adoxio_generalyesno = DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno;
using FileSystemItem = Gov.Lclb.Cllb.Public.ViewModels.FileSystemItem;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    // No authorize policy as this can be used by workers to upload files.
    public class FileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDataverseClient _dataverse;
        private readonly string _encryptionKey;
        private readonly FileManagerClient _fileManagerClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public FileController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDataverseClient dataverse, FileManagerClient fileClient)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dataverse = dataverse;
            _encryptionKey = _configuration["ENCRYPTION_KEY"];
            _logger = loggerFactory.CreateLogger(typeof(FileController));
            _fileManagerClient = fileClient;
        }

        /// <summary>
        /// Returns true if the current user can access the entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="isDelete">Some access rules are different for deletes</param>
        /// <returns></returns>
        private async Task<bool> CanAccessEntity(string entityName, string entityId, string relativeUrl, bool isDelete = false)
        {
            var result = false;
            string folderName = null;
            switch (entityName.ToLower())
            {
                case "account":
                    var account = await _dataverse.GetAccountByIdAsync(entityId).ConfigureAwait(true);
                    if (account != null)
                    {
                        result = CurrentUserHasAccessToAccount(account.Id.ToString());
                        folderName = await _dataverse.GetFolderNameAsync(entityName, entityId).ConfigureAwait(true);
                    }
                    break;
                case "application":
                    var application = await _dataverse.GetApplicationByIdAsync(entityId).ConfigureAwait(true);
                    if (application != null)
                    {
                        var applicantId = application.adoxio_Applicant?.Id.ToString();
                        result = CurrentUserHasAccessToAccount(applicantId);
                        Console.WriteLine($"User has access to account {applicantId}: {result}");
                        var allowLGAccess = await CurrentUserIsLGForApplication(application);
                        result = result || allowLGAccess && !isDelete;
                        folderName = await _dataverse.GetFolderNameAsync(entityName, entityId).ConfigureAwait(true);
                    }
                    break;
                case "contact":
                    var contact = await _dataverse.GetContactByIdAsync(entityId).ConfigureAwait(true);
                    if (contact != null)
                    {
                        result = CurrentUserHasAccessToContactOwnedBy(contact.Id.ToString());
                        folderName = await _dataverse.GetFolderNameAsync(entityName, entityId).ConfigureAwait(true);
                    }
                    break;
                case "worker":
                    var worker = await _dataverse.GetWorkerByIdAsync(entityId).ConfigureAwait(true);
                    if (worker != null)
                    {
                        result = CurrentUserHasAccessToContactOwnedBy(worker.adoxio_ContactId?.Id.ToString());
                        folderName = await _dataverse.GetFolderNameAsync(entityName, entityId).ConfigureAwait(true);
                    }
                    break;
                case "event":
                    var eventEntity = await _dataverse.GetEventByIdAsync(entityId).ConfigureAwait(true);
                    if (eventEntity != null)
                    {
                        result = CurrentUserHasAccessToAccount(eventEntity.adoxio_Account?.Id.ToString());
                        folderName = await _dataverse.GetFolderNameAsync(entityName, entityId).ConfigureAwait(true);
                    }
                    break;
            }

            if (folderName != null && result && relativeUrl != null)
            {
                int slashPos = relativeUrl.IndexOf("/");
                if (slashPos != -1 && slashPos < relativeUrl.Length)
                {
                    slashPos = relativeUrl.IndexOf("/", slashPos + 1);
                }
                if (entityName.ToLower() != "account")
                {
                    result = relativeUrl.ToUpper().Substring(slashPos + 1).StartsWith(folderName.ToUpper());
                    if (!result)
                    {
                        var segments = relativeUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                        var appIndex = Array.FindIndex(
                            segments,
                            s => string.Equals(s, SharePointConstants.ApplicationFolderInternalName, StringComparison.OrdinalIgnoreCase));
                        result = appIndex >= 0
                            && appIndex + 1 < segments.Length
                            && string.Equals(segments[appIndex + 1], folderName, StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// True if the user can access a given file
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="documentType"></param>
        /// <param name="serverRelativeUrl"></param>
        /// <returns></returns>
        private async Task<bool> CanAccessEntityFile(string entityName, string entityId, string documentType, string serverRelativeUrl, bool isDelete = false)
        {
            var logUrl = WordSanitizer.Sanitize(serverRelativeUrl);
            Console.WriteLine($"CanAccessEntityFile called with entityName {entityName}, entityId {entityId}, documentType {documentType}, serverRelativeUrl {serverRelativeUrl}, isDelete {isDelete}");
            var result = await CanAccessEntity(entityName, entityId, serverRelativeUrl, isDelete).ConfigureAwait(true);

            Console.WriteLine($"CanAccessEntity result: {result}");
            //get list of files for entity
            var hasFile = false;

            var fileExistsRequest = new FileExistsRequest
            {
                DocumentType = documentType,
                EntityId = entityId,
                EntityName = entityName,
                FolderName = await _dataverse.GetFolderNameAsync(entityName, entityId).ConfigureAwait(true),
                ServerRelativeUrl = serverRelativeUrl
            };
            Console.WriteLine($"Checking if file exists with serverRelativeUrl {logUrl}");
            var hasFileResult = _fileManagerClient.FileExists(fileExistsRequest);

            if (hasFileResult.ResultStatus == FileExistStatus.Exist)
                // Update modifiedon to current time
                hasFile = true;
            else
                _logger.LogError($"Unexpected error - Unable to validate file - {logUrl}");



            return result && hasFile;
        }


        private async Task<bool> CurrentUserIsLGForApplication(DvApplication application)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var accountId = GuidUtility.SanitizeGuidString(userSettings.AccountId);
            var account = await _dataverse.GetAccountByIdAsync(accountId);
            var isLGForApplication = application != null
                && application.adoxio_localgovindigenousnationid?.Id == account?.adoxio_LGINLinkId?.Id;
            return isLGForApplication;
        }


        /// <summary>
        /// Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
        private bool CurrentUserHasAccessToAccount(string accountId)
        {


            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // For now, check if the account id matches the user's account.
            // TODO there may be some account relationships in the future
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0) return userSettings.AccountId == accountId;

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
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            // For now, check if the account id matches the user's account.
            // TODO there may be some account relationships in the future
            if (userSettings.ContactId != null && userSettings.ContactId.Length > 0) return userSettings.ContactId == contactId;

            // if current user doesn't have an account they are probably not logged in
            return false;
        }

        /// <summary>
        /// Download a file attachment
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityName"></param>
        /// <param name="serverRelativeUrl"></param>
        /// <param name="documentType"></param>
        /// <returns>The file as binary data, or bad request if the request is invalid</returns>
        ///
        [HttpGet("{entityId}/download-file/{entityName}/{fileName}")]
        public async Task<IActionResult> DownloadAttachment(string entityId, string entityName, [FromQuery] string serverRelativeUrl, [FromQuery] string documentType)
        {
            return await DownloadAttachmentInternal(entityId, entityName, serverRelativeUrl, documentType, true).ConfigureAwait(true);
        }

        /// <summary>
        /// Internal implementation of download attachment
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityName"></param>
        /// <param name="serverRelativeUrl"></param>
        /// <param name="documentType"></param>
        /// <param name="checkUser"></param>
        /// <returns></returns>
        private async Task<IActionResult> DownloadAttachmentInternal(string entityId, string entityName, [FromQuery] string serverRelativeUrl, [FromQuery] string documentType, bool checkUser)
        {
            Console.WriteLine("******************************************");
            Console.WriteLine($"DownloadAttachmentInternal called with entityId {entityId}, entityName {entityName}, serverRelativeUrl {serverRelativeUrl}, documentType {documentType}, checkUser {checkUser}");
            // get the file.
            if (string.IsNullOrEmpty(serverRelativeUrl) || string.IsNullOrEmpty(documentType) || string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName)) return BadRequest();

            var hasAccess = true;

            if (checkUser)
            {
                ValidateSession();
                hasAccess = await CanAccessEntityFile(entityName, entityId, documentType, serverRelativeUrl).ConfigureAwait(true);
            }

            _logger.LogInformation($"Attempting to download file {serverRelativeUrl} for entity {entityName}, entityId {entityId}, documentType {documentType}");
            Console.WriteLine($"User has access: {hasAccess}");
            if (!hasAccess) return BadRequest();
            _logger.LogInformation($"Attempting to download file {serverRelativeUrl} for entity {entityName}, entityId {entityId}, documentType {documentType}");


            var logUrl = WordSanitizer.Sanitize(serverRelativeUrl);

            // call the web service
            var downloadRequest = new DownloadFileRequest
            {
                ServerRelativeUrl = serverRelativeUrl
            };

            var downloadResult = _fileManagerClient.DownloadFile(downloadRequest);

            if (downloadResult.ResultStatus == ResultStatus.Success)
            {
                // Update modifiedon to current time
                await UpdateEntityModifiedOnDate(entityName, entityId, true);
                _logger.LogInformation($"SUCCESS in getting file {logUrl}");
                var fileContents = downloadResult.Data.ToByteArray();
                return new FileContentResult(fileContents, "application/octet-stream");
            }

            _logger.LogError($"ERROR in getting file {logUrl} - {downloadResult.ErrorDetail}");
            throw new Exception($"ERROR in getting file {logUrl} - {downloadResult.ErrorDetail}");

        }





        /// <summary>
        ///  helper function used by the public file upload features to verify that the user has access.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsPublicUserAuthorized(string entityName, string entityId, string relativeUrl)
        {
            // currently this service only supports contacts
            var authorized = true;
            string folderName = null;
            if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(entityId) || entityName != "contact")
            {
                authorized = false;
            }
            else
            {
                var contact = await _dataverse.GetContactByIdAsync(entityId);
                if (contact == null)
                {
                    authorized = false;
                }
                else
                {
                    folderName = await _dataverse.GetFolderNameAsync("contact", entityId);
                }
            }

            if (folderName != null && authorized && relativeUrl != null)
            {
                int slashPos = relativeUrl.IndexOf("/");
                if (slashPos != -1 && slashPos < relativeUrl.Length)
                {
                    slashPos = relativeUrl.IndexOf("/", slashPos + 1);
                }
                authorized = relativeUrl.ToUpper().Substring(slashPos + 1).StartsWith(folderName.ToUpper());
            }

            return authorized;
        }

        [HttpDelete("{token}/public-attachments/{entityName}")]
        [AllowAnonymous]
        public async Task<IActionResult> PublicDeleteFile([FromRoute] string token, [FromQuery] string serverRelativeUrl, [FromQuery] string documentType, [FromRoute] string entityName)
        {
            // decode the entityID
            var entityId = EncryptionUtility.DecryptStringHex(token, _encryptionKey);

            var authorized = await IsPublicUserAuthorized(entityName, entityId, serverRelativeUrl).ConfigureAwait(true);

            if (authorized)
                return await DeleteFileInternal(serverRelativeUrl, documentType, entityId, entityName, false).ConfigureAwait(true);
            return Unauthorized();
        }

        /// <summary>
        /// Public version of the file download.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityName"></param>
        /// <param name="serverRelativeUrl"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>

        [HttpGet("{token}/public-download-file/{entityName}/{fileName}")]
        [AllowAnonymous]
        public async Task<IActionResult> PublicDownloadAttachment(string token, string entityName, [FromQuery] string serverRelativeUrl, [FromQuery] string documentType)
        {
            // decode the entityID
            var entityId = EncryptionUtility.DecryptStringHex(token, _encryptionKey);

            var authorized = await IsPublicUserAuthorized(entityName, entityId, serverRelativeUrl).ConfigureAwait(true);

            if (authorized)
                return await DownloadAttachmentInternal(entityId, entityName, serverRelativeUrl, documentType, false).ConfigureAwait(true);
            return Unauthorized();
        }

        /// <summary>
        /// Get the file details list in folder associated to the application folder and document type
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        [HttpGet("{token}/public-attachments/{entityName}/{documentType}")]
        [AllowAnonymous]
        public async Task<IActionResult> PublicGetAttachments([FromRoute] string token, [FromRoute] string entityName, [FromRoute] string documentType)
        {
            // decode the entityID
            var entityId = EncryptionUtility.DecryptStringHex(token, _encryptionKey);

            var authorized = await IsPublicUserAuthorized(entityName, entityId, null).ConfigureAwait(true);

            if (authorized)
                return await GetAttachmentsInternal(entityId, entityName, documentType, false);
            return Unauthorized();
        }

        /// <summary>
        /// Public version of the file upload.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityName"></param>
        /// <param name="file"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>

        [HttpPost("{token}/public-attachments/{entityName}")]
        [DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<IActionResult> PublicUploadAttachment([FromRoute] string token, [FromRoute] string entityName,
          [FromForm] IFormFile file, [FromForm] string documentType)
        {
            // decode the entityID
            var entityId = EncryptionUtility.DecryptStringHex(token, _encryptionKey);

            var authorized = await IsPublicUserAuthorized(entityName, entityId, null).ConfigureAwait(true);

            if (authorized)
                return await UploadAttachmentInternal(entityId, entityName, file, documentType, false).ConfigureAwait(true);
            return Unauthorized();
        }


        [HttpPost("{id}/public-covid-application")]
        [DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<IActionResult> PublicCovidApplication([FromRoute] string id,
          [FromForm] IFormFile file, [FromForm] string documentType)
        {
            var application = await _dataverse.GetApplicationByIdAsync(id);
            if (application == null)
                return BadRequest();
            return await UploadAttachmentInternal(id, "application", file, documentType, false).ConfigureAwait(true);
        }


        /// <summary>
        /// Get the file details list in folder associated to the application folder and document type
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        [HttpGet("{entityId}/attachments/{entityName}/{documentType}")]
        public async Task<IActionResult> GetAttachments([FromRoute] string entityId, [FromRoute] string entityName, [FromRoute] string documentType)
        {
            return await GetAttachmentsInternal(entityId, entityName, documentType, true);
        }

        private async Task<IActionResult> GetAttachmentsInternal(string entityId, string entityName, string documentType, bool checkUser)
        {
            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType)) return BadRequest();

            var hasAccess = true;

            if (checkUser)
            {
                ValidateSession();
                hasAccess = await CanAccessEntity(entityName, entityId, null);
            }

            if (!hasAccess) return new NotFoundResult();

            var fileSystemItemVMList = await GetListFilesInFolder(entityId, entityName, documentType, _dataverse, _fileManagerClient, _logger);
            return new JsonResult(fileSystemItemVMList);
        }

        /// <summary>
        /// Get the status of the various document types for a given entity record
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpGet("{entityName}/{entityId}/documentStatus/{formId}")]
        public async Task<IActionResult> GetDocumentTypeStatus(string entityName, string entityId, string formId)
        {
            return await GetDocumentTypeStatusInternal(entityName, entityId, formId, true);
        }

        /// <summary>
        /// Internal implementation of method for getting the status of document types for a given entity record
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="formId"></param>
        /// <param name="checkUser"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetDocumentTypeStatusInternal(string entityName, string entityId, string formId, bool checkUser)
        {
            List<DocumentTypeStatus> result = null;

            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(formId)) return BadRequest();

            string folderName = await _dataverse.GetFolderNameAsync(entityName, entityId);

            if (folderName != null)
            {
                var folderContents = _fileManagerClient.GetFileDetailsListInFolder(_logger, entityName, entityId, folderName);

                var formFileFields = await _dataverse.GetFormDocumentFieldsAsync(formId);

                result = new List<DocumentTypeStatus>();

                foreach (var formFileField in formFileFields)
                {
                    var documentTypePrefix = formFileField.FilePrefix;
                    var documentTypeName = formFileField.Name;
                    var routerLink = formFileField.RouterLink;

                    var valid = false;

                    // determine if there are any files with this prefix.

                    var firstMatch = folderContents.FirstOrDefault(f => f.documenttype == documentTypePrefix);

                    if (firstMatch != null) valid = true;

                    result.Add(new DocumentTypeStatus { DocumentType = documentTypePrefix, Name = documentTypeName, Valid = valid, RouterLink = routerLink });
                }

            }


            // ensure that the entity has files.
            if (result == null)
                return BadRequest();
            return new JsonResult(result);
        }


        /// <summary>
        /// Return the list of files in a given folder.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityName"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        public static async Task<List<FileSystemItem>> GetListFilesInFolder(string entityId, string entityName, string documentType, IDataverseClient _dataverse, FileManagerClient _fileManagerClient, ILogger _logger)
        {
            var fileSystemItemVMList = new List<FileSystemItem>();

            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType)) return fileSystemItemVMList;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var request = new FolderFilesRequest
                    {
                        DocumentType = documentType,
                        EntityId = entityId,
                        EntityName = entityName,
                        FolderName = await _dataverse.GetFolderNameAsync(entityName, entityId)
                    };

                    var result = _fileManagerClient.FolderFiles(request);

                    if (result.ResultStatus == ResultStatus.Success)
                    {

                        // convert the results to the view model.
                        foreach (var fileDetails in result.Files)
                        {
                            var fileSystemItemVM = new FileSystemItem
                            {
                                // remove the document type text from file name
                                name = fileDetails.Name.Substring(fileDetails.Name.IndexOf("__") + 2),
                                // convert size from bytes (original) to KB
                                size = fileDetails.Size,
                                serverrelativeurl = fileDetails.ServerRelativeUrl,
                                timecreated = fileDetails.TimeCreated.ToDateTime(),
                                timelastmodified = fileDetails.TimeLastModified.ToDateTime(),
                                documenttype = fileDetails.DocumentType
                            };

                            fileSystemItemVMList.Add(fileSystemItemVM);
                        }

                        break;

                    }

                    _logger.LogError($"ERROR in getting folder files for entity {entityName}, entityId {entityId}, docuemnt type {documentType} ");

                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting SharePoint File List");
                }
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
            return await DeleteFileInternal(serverRelativeUrl, documentType, entityId, entityName, true);
        }

        /// <summary>
        /// Internal implementation of delete file
        /// </summary>
        /// <param name="serverRelativeUrl"></param>
        /// <param name="documentType"></param>
        /// <param name="entityId"></param>
        /// <param name="entityName"></param>
        /// <param name="checkUser">If true, check that the current user has access</param>
        /// <returns></returns>
        private async Task<IActionResult> DeleteFileInternal(string serverRelativeUrl, string documentType, string entityId, string entityName, bool checkUser)
        {
            // get the file.
            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(serverRelativeUrl) || string.IsNullOrEmpty(documentType)) return BadRequest();

            var hasAccess = true;
            if (checkUser)
            {
                ValidateSession();
                hasAccess = await CanAccessEntityFile(entityName, entityId, documentType, serverRelativeUrl, true);
            }

            if (!hasAccess) return new NotFoundResult();

            var logUrl = WordSanitizer.Sanitize(serverRelativeUrl);

            // call the web service
            var deleteRequest = new DeleteFileRequest
            {
                ServerRelativeUrl = serverRelativeUrl
            };

            var deleteResult = _fileManagerClient.DeleteFile(deleteRequest);

            if (deleteResult.ResultStatus == ResultStatus.Success)
            {
                // Update modifiedon to current time
                await UpdateEntityModifiedOnDate(entityName, entityId, true);
                _logger.LogInformation($"SUCCESS in deleting file {serverRelativeUrl}");
                return new OkResult();
            }

            _logger.LogError($"ERROR in deleting file {logUrl} - {deleteResult.ErrorDetail}");
            throw new Exception($"ERROR in deleting file {logUrl} - {deleteResult.ErrorDetail}");

        }


        /// <summary>
        /// Process a file upload from an authenticated user
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityName"></param>
        /// <param name="file"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>

        [HttpPost("{entityId}/attachments/{entityName}")]
        // allow large uploads
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAttachment([FromRoute] string entityId, [FromRoute] string entityName,
            [FromForm] IFormFile file, [FromForm] string documentType)
        {
            return await UploadAttachmentInternal(entityId, entityName, file, documentType, true).ConfigureAwait(true);
        }

        /// <summary>
        /// Internal implementation of upload attachment
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityName"></param>
        /// <param name="file"></param>
        /// <param name="documentType"></param>
        /// <param name="checkUser"></param>
        /// <returns></returns>
        private async Task<IActionResult> UploadAttachmentInternal(string entityId, string entityName,
            IFormFile file, string documentType, bool checkUser)
        {
            FileSystemItem result = null;

            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType)) return BadRequest();

            var hasAccess = true;
            if (checkUser)
            {
                ValidateSession();
                hasAccess = await CanAccessEntity(entityName, entityId, null).ConfigureAwait(true);
            }

            if (!hasAccess) return new NotFoundResult();

            var ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            var data = ms.ToArray();

            // Check for a bad file type.

            var mimeTypes = new MimeTypes();

            var mimeType = mimeTypes.GetMimeType(data);

            // Add additional allowed mime types here
            if (mimeType == null || !(mimeType.Name.Equals("image/png") || mimeType.Name.Equals("image/jpeg") ||
                                     mimeType.Name.Equals("application/pdf")))
            {
                _logger.LogError($"ERROR in uploading file due to invalid mime type {mimeType?.Name}");
                return new NotFoundResult();
            }
            else
            {
                // Sanitize file name
                var illegalInFileName = new Regex(@"[#%*<>?{}~¿""]");
                var fileName = illegalInFileName.Replace(file.FileName, "");
                illegalInFileName = new Regex(@"[&:/\\|]");
                fileName = illegalInFileName.Replace(fileName, "-");

                fileName = FileSystemItemExtensions.CombineNameDocumentType(fileName, documentType);

                var folderName = await _dataverse.GetFolderNameAsync(entityName, entityId).ConfigureAwait(true);

                await _dataverse.CreateEntitySharePointDocumentLocationAsync(entityName, entityId, folderName, folderName);

                // call the web service
                var uploadRequest = new UploadFileRequest
                {
                    ContentType = file.ContentType,
                    Data = ByteString.CopyFrom(data),
                    EntityName = entityName,
                    FileName = fileName,
                    FolderName = folderName
                };

                var uploadResult = _fileManagerClient.UploadFile(uploadRequest);

                var logFolderName = WordSanitizer.Sanitize(folderName);
                var logFileName = WordSanitizer.Sanitize(fileName);

                if (uploadResult.ResultStatus == ResultStatus.Success)
                {
                    // Update modifiedon to current time
                    await UpdateEntityModifiedOnDate(entityName, entityId, true);
                    _logger.LogInformation($"SUCCESS in uploading file {logFileName} to folder {logFolderName}");
                }
                else
                {
                    _logger.LogError($"ERROR in uploading file {logFileName} to folder {logFolderName}");
                    throw new Exception($"ERROR in uploading file {logFileName} to folder {logFolderName}");
                }
            }



            return new JsonResult(result);
        }

        /// <summary>
        /// Utility function used when we want to update the last modified date on a given record; this is used to ensure staff are aware files have been added.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="setUploadedFromPortal"></param>

        private async Task UpdateEntityModifiedOnDate(string entityName, string entityId, bool setUploadedFromPortal = false)
        {
            if (!Guid.TryParse(entityId, out var guid)) return;
            switch (entityName.ToLower())
            {
                case "application":
                    var patchApplication = new DvApplication { Id = guid };
                    if (setUploadedFromPortal) patchApplication.adoxio_FileUploadedFromPortal = adoxio_generalyesno.Yes;
                    try { await _dataverse.UpdateApplicationAsync(patchApplication); }
                    catch (Exception e) { _logger.LogError(e, "Error updating application"); throw; }
                    break;
                case "contact":
                    try { await _dataverse.UpdateContactAsync(new DvContact { Id = guid }); }
                    catch (Exception e) { _logger.LogError(e, "Error updating Contact"); throw; }
                    break;
                case "worker":
                    try { await _dataverse.UpdateWorkerAsync(new DvWorker { Id = guid }); }
                    catch (Exception e) { _logger.LogError(e, "Error updating Worker"); throw; }
                    break;
            }
        }

        /// <summary>
        /// Utility function used to verify that the current user session is valid.  Only used for authenticated endpoints
        /// </summary>
        private void ValidateSession()
        {
            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // check that the session is setup correctly.
            userSettings.Validate();
        }
    }
}
