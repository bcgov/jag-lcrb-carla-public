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
using Gov.Lclb.Cllb.Public.Utility;

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
        private readonly string _encryptionKey;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="dynamicsClient"></param>
        /// <param name="fileClient"></param>

        public FileController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, FileManagerClient fileClient)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
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
        private async Task<bool> CanAccessEntity(string entityName, string entityId, bool isDelete = false)
        {
            var result = false;
            var id = Guid.Parse(entityId);
            switch (entityName.ToLower())
            {
                case "account":
                    var account = await _dynamicsClient.GetAccountByIdAsync(id).ConfigureAwait(true);
                    result = account != null && CurrentUserHasAccessToAccount(account.Accountid);
                    break;
                case "application":
                    var application = await _dynamicsClient.GetApplicationById(id).ConfigureAwait(true);
                    result = application != null && CurrentUserHasAccessToAccount(application._adoxioApplicantValue);
                    bool allowLGAccess = await CurrentUserIsLGForApplication(application);
                    result = result || (allowLGAccess && !isDelete);
                    break;
                case "contact":
                    var contact = await _dynamicsClient.GetContactById(id).ConfigureAwait(true);
                    result = contact != null && CurrentUserHasAccessToContactOwnedBy(contact.Contactid);
                    break;
                case "worker":
                    var worker = await _dynamicsClient.GetWorkerById(id).ConfigureAwait(true);
                    result = worker != null && CurrentUserHasAccessToContactOwnedBy(worker._adoxioContactidValue);
                    break;
                case "event":
                    var eventEntity = _dynamicsClient.GetEventById(id);
                    result = eventEntity != null && CurrentUserHasAccessToAccount(eventEntity._adoxioAccountValue);
                    break;
                default:
                    break;
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
            string logUrl = WordSanitizer.Sanitize(serverRelativeUrl);

            var result = await CanAccessEntity(entityName, entityId, isDelete).ConfigureAwait(true);
            //get list of files for entity
            bool hasFile = false;

            var fileExistsRequest = new FileExistsRequest()
            {
                DocumentType = documentType,
                EntityId = entityId,
                EntityName = entityName,
                FolderName = await GetFolderName(entityName, entityId, _dynamicsClient).ConfigureAwait(true),
                ServerRelativeUrl = serverRelativeUrl
            };

            var hasFileResult = _fileManagerClient.FileExists(fileExistsRequest);

            if (hasFileResult.ResultStatus == FileExistStatus.Exist)
            {
                // Update modifiedon to current time
                hasFile = true;
            }
            else
            {
                _logger.LogError($"Unexpected error - Unable to validate file - {logUrl}");
            }

            return result && hasFile;
        }


        private async Task<bool> CurrentUserIsLGForApplication(MicrosoftDynamicsCRMadoxioApplication application)
        {
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // get user account
            var accountId = Utils.GuidUtility.SanitizeGuidString(userSettings.AccountId);
            MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountByIdAsync(new Guid(accountId));

            // make sure the application and account have matching local government values
            bool isLGForApplication = (application != null && application._adoxioLocalgovindigenousnationidValue == account._adoxioLginlinkidValue);
            return isLGForApplication;
        }


        /// <summary>
        /// Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
        private bool CurrentUserHasAccessToAccount(string accountId)
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
        public async Task<IActionResult> DownloadAttachment(string entityId, string entityName, [FromQuery]string serverRelativeUrl, [FromQuery]string documentType)
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
        private async Task<IActionResult> DownloadAttachmentInternal(string entityId, string entityName, [FromQuery]string serverRelativeUrl, [FromQuery]string documentType, bool checkUser)
        {
            // get the file.
            if (string.IsNullOrEmpty(serverRelativeUrl) || string.IsNullOrEmpty(documentType) || string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName))
            {
                return BadRequest();
            }        

            bool hasAccess = true;
            if (checkUser)
            {
                ValidateSession();
                hasAccess = await CanAccessEntityFile(entityName, entityId, documentType, serverRelativeUrl).ConfigureAwait(true);
            }

            if (!hasAccess)
            {
                return BadRequest();
            }

            string logUrl = WordSanitizer.Sanitize(serverRelativeUrl);

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
                _logger.LogInformation($"SUCCESS in getting file {logUrl}");
                byte[] fileContents = downloadResult.Data.ToByteArray();
                return new FileContentResult(fileContents, "application/octet-stream")
                {
                };
            }
            else
            {
                _logger.LogError($"ERROR in getting file {logUrl} - {downloadResult.ErrorDetail}");
                throw new Exception($"ERROR in getting file {logUrl} - {downloadResult.ErrorDetail}");
            }

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

        /// <summary>
        /// Returns the SharePoint document Location for a given entity record
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        private static async Task<string> GetEntitySharePointDocumentLocation(string entityName, string entityId, IDynamicsClient _dynamicsClient)
        {
            string result = null;
            var id = Guid.Parse(entityId);
            try
            {
                switch (entityName.ToLower())
                {
                    case "account":
                        var account = _dynamicsClient.GetAccountById(entityId);
                        var accountLocation = account.AccountSharepointDocumentLocation.FirstOrDefault();
                        if (accountLocation != null && !string.IsNullOrEmpty(accountLocation.Relativeurl))
                        {
                            result = accountLocation.Relativeurl;
                        }
                        break;
                    case "application":
                        var application = await _dynamicsClient.GetApplicationByIdWithChildren(entityId).ConfigureAwait(true);
                        var applicationLocation = application.AdoxioApplicationSharePointDocumentLocations.FirstOrDefault();
                        if (applicationLocation != null && !string.IsNullOrEmpty(applicationLocation.Relativeurl))
                        {
                            result = applicationLocation.Relativeurl;
                        }
                        break;
                    case "contact":
                        var contact = await _dynamicsClient.GetContactById(entityId).ConfigureAwait(true);
                        var contactLocation = contact.ContactSharePointDocumentLocations.FirstOrDefault();
                        if (contactLocation != null && !string.IsNullOrEmpty(contactLocation.Relativeurl))
                        {
                            result = contactLocation.Relativeurl;
                        }
                        break;
                    case "worker":
                        var worker = await _dynamicsClient.GetWorkerByIdWithChildren(entityId).ConfigureAwait(true);
                        var workerLocation = worker.AdoxioWorkerSharePointDocumentLocations.FirstOrDefault();
                        if (workerLocation != null && !string.IsNullOrEmpty(workerLocation.Relativeurl))
                        {
                            result = workerLocation.Relativeurl;
                        }
                        break;
                    case "event":
                        var eventEntity = _dynamicsClient.GetEventByIdWithChildren(entityId);
                        MicrosoftDynamicsCRMsharepointdocumentlocation? eventLocation = eventEntity.AdoxioEventSharePointDocumentLocations.FirstOrDefault();
                        if (eventLocation != null && !string.IsNullOrEmpty(eventLocation.Relativeurl))
                        {
                            result = eventLocation.Relativeurl;
                        }
                        break;


                    default:
                        break;
                }
            }
            catch (System.ArgumentNullException)
            {
                return null;
            }
            return result;
        }

        async Task CreateEntitySharePointDocumentLocation(string entityName, string entityId, string folderName, string name)
        {
            
            var id = Guid.Parse(entityId);
            switch (entityName.ToLower())
            {
                case "account":
                    var account = _dynamicsClient.GetAccountById(entityId);
                    await CreateAccountDocumentLocation(account, folderName, name).ConfigureAwait(true);
                    break;
                case "application":
                    var application = await _dynamicsClient.GetApplicationByIdWithChildren(entityId).ConfigureAwait(true);
                    await CreateApplicationDocumentLocation(application, folderName, name).ConfigureAwait(true);
                    break;
                case "contact":
                    var contact = await _dynamicsClient.GetContactById(entityId).ConfigureAwait(true);
                    await CreateContactDocumentLocation(contact, folderName, name);
                    break;
                case "worker":
                    var worker = await _dynamicsClient.GetWorkerByIdWithChildren(entityId).ConfigureAwait(true);
                    await CreateWorkerDocumentLocation(worker, folderName, name).ConfigureAwait(true);
                    break;
                case "event":
                    var eventEntity = _dynamicsClient.GetEventByIdWithChildren(entityId);
                    await CreateEventDocumentLocation(eventEntity, folderName, name).ConfigureAwait(true);
                    break;

                default:
                    break;
            }
        }

        private async Task CreateAccountDocumentLocation(MicrosoftDynamicsCRMaccount account, string folderName, string name )
        {           

            // now create a document location to link them.

            // Create the SharePointDocumentLocation entity
            MicrosoftDynamicsCRMsharepointdocumentlocation mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation()
            {
                Relativeurl = folderName,
                Description = "Account Files",
                Name = name
            };


            try
            {
                mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, "Error creating SharepointDocumentLocation");
                mdcsdl = null;
            }
            if (mdcsdl != null)
            {

                // set the parent document library.
                string parentDocumentLibraryReference = GetDocumentLocationReferenceByRelativeURL("account");

                string accountUri = _dynamicsClient.GetEntityURI("accounts", account.Accountid);
                // add a regardingobjectid.
                var patchSharePointDocumentLocationIncident = new MicrosoftDynamicsCRMsharepointdocumentlocation()
                {
                    RegardingobjectIdAccountODataBind = accountUri,
                    ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference),
                    Relativeurl = folderName,
                    Description = "Account Files",
                };

                try
                {
                    _dynamicsClient.Sharepointdocumentlocations.Update(mdcsdl.Sharepointdocumentlocationid, patchSharePointDocumentLocationIncident);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference SharepointDocumentLocation to account");
                }

                string sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", mdcsdl.Sharepointdocumentlocationid);

                Odataid oDataId = new Odataid()
                {
                     OdataidProperty = sharePointLocationData
                };
                try
                {
                    _dynamicsClient.Accounts.AddReference(account.Accountid, "Account_SharepointDocumentLocation", oDataId);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference to SharepointDocumentLocation");
                }
            }
        }

        private async Task CreateApplicationDocumentLocation(MicrosoftDynamicsCRMadoxioApplication application, string folderName, string name)
        {

            // now create a document location to link them.

            // Create the SharePointDocumentLocation entity
            MicrosoftDynamicsCRMsharepointdocumentlocation mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation()
            {
                Relativeurl = folderName,
                Description = "Application Files",
                Name = name
            };


            try
            {
                mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, "Error creating SharepointDocumentLocation");
                mdcsdl = null;
            }
            if (mdcsdl != null)
            {

                // set the parent document library.
                string parentDocumentLibraryReference = GetDocumentLocationReferenceByRelativeURL("adoxio_application");

                string accountUri = _dynamicsClient.GetEntityURI("adoxio_applications", application.AdoxioApplicationid);
                // add a regardingobjectid.
                var patchSharePointDocumentLocationIncident = new MicrosoftDynamicsCRMsharepointdocumentlocation()
                {
                    RegardingobjectIdAccountODataBind = accountUri,
                    ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference),
                    Relativeurl = folderName,
                    Description = "Application Files",
                };

                try
                {
                    _dynamicsClient.Sharepointdocumentlocations.Update(mdcsdl.Sharepointdocumentlocationid, patchSharePointDocumentLocationIncident);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference SharepointDocumentLocation to application");
                }

                string sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", mdcsdl.Sharepointdocumentlocationid);

                Odataid oDataId = new Odataid()
                {
                    OdataidProperty = sharePointLocationData
                };
                try
                {
                    _dynamicsClient.Applications.AddReference(application.AdoxioApplicationid, "adoxio_application_SharePointDocumentLocations", oDataId);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference to SharepointDocumentLocation");
                }
            }
        }

        private async Task CreateContactDocumentLocation(MicrosoftDynamicsCRMcontact contact, string folderName, string name)
        {

            // now create a document location to link them.

            // Create the SharePointDocumentLocation entity
            MicrosoftDynamicsCRMsharepointdocumentlocation mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation()
            {
                Relativeurl = folderName,
                Description = "Contact Files",
                Name = name
            };


            try
            {
                mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, "Error creating SharepointDocumentLocation");
                mdcsdl = null;
            }
            if (mdcsdl != null)
            {

                // set the parent document library.
                string parentDocumentLibraryReference = GetDocumentLocationReferenceByRelativeURL("contact");

                string contactUri = _dynamicsClient.GetEntityURI("contacts", contact.Contactid);
                // add a regardingobjectid.
                var patchSharePointDocumentLocationContact = new MicrosoftDynamicsCRMsharepointdocumentlocation()
                {
                    RegardingobjectIdContactODataBind = contactUri,
                    ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference),
                    Relativeurl = folderName,
                    Description = "Contact Files",
                };

                try
                {
                    _dynamicsClient.Sharepointdocumentlocations.Update(mdcsdl.Sharepointdocumentlocationid, patchSharePointDocumentLocationContact);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference SharepointDocumentLocation to contact");
                }

                string sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", mdcsdl.Sharepointdocumentlocationid);

                Odataid oDataId = new Odataid()
                {
                    OdataidProperty = sharePointLocationData
                };
                try
                {
                    _dynamicsClient.Contacts.AddReference(contact.Contactid, "adoxio_application_SharePointDocumentLocations", oDataId);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference to SharepointDocumentLocation");
                }
            }
        }

        private async Task CreateWorkerDocumentLocation(MicrosoftDynamicsCRMadoxioWorker worker, string folderName, string name)
        {

            // now create a document location to link them.

            // Create the SharePointDocumentLocation entity
            MicrosoftDynamicsCRMsharepointdocumentlocation mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation()
            {
                Relativeurl = folderName,
                Description = "Worker Files",
                Name = name
            };


            try
            {
                mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, "Error creating SharepointDocumentLocation");
                mdcsdl = null;
            }
            if (mdcsdl != null)
            {

                // set the parent document library.
                string parentDocumentLibraryReference = GetDocumentLocationReferenceByRelativeURL("adoxio_worker");

                string accountUri = _dynamicsClient.GetEntityURI("adoxio_worker", worker.AdoxioWorkerid);
                // add a regardingobjectid.
                var patchSharePointDocumentLocationIncident = new MicrosoftDynamicsCRMsharepointdocumentlocation()
                {
                    RegardingobjectIdAccountODataBind = accountUri,
                    ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference),
                    Relativeurl = folderName,
                    Description = "Application Files",
                };

                try
                {
                    _dynamicsClient.Sharepointdocumentlocations.Update(mdcsdl.Sharepointdocumentlocationid, patchSharePointDocumentLocationIncident);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference SharepointDocumentLocation to worker");
                }

                string sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", mdcsdl.Sharepointdocumentlocationid);

                Odataid oDataId = new Odataid()
                {
                    OdataidProperty = sharePointLocationData
                };
                try
                {
                    _dynamicsClient.Workers.AddReference(worker.AdoxioWorkerid, "adoxio_worker_SharePointDocumentLocations", oDataId);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference to SharepointDocumentLocation");
                }
            }
        }

        private async Task CreateEventDocumentLocation(MicrosoftDynamicsCRMadoxioEvent eventEntity, string folderName, string name)
        {

            // now create a document location to link them.

            // Create the SharePointDocumentLocation entity
            MicrosoftDynamicsCRMsharepointdocumentlocation mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation()
            {
                Relativeurl = folderName,
                Description = "Event Files",
                Name = name
            };


            try
            {
                mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, "Error creating SharepointDocumentLocation");
                mdcsdl = null;
            }
            if (mdcsdl != null)
            {

                // set the parent document library.
                string parentDocumentLibraryReference = GetDocumentLocationReferenceByRelativeURL("adoxio_event");

                string eventUri = _dynamicsClient.GetEntityURI("adoxio_events", eventEntity.AdoxioEventid);
                // add a regardingobjectid.
                var patchSharePointDocumentLocationEvent = new MicrosoftDynamicsCRMsharepointdocumentlocation()
                {
                    RegardingobjectIdEventODataBind = eventUri,
                    ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference),
                    Relativeurl = folderName,
                    Description = "Event Files",
                };

                try
                {
                    _dynamicsClient.Sharepointdocumentlocations.Update(mdcsdl.Sharepointdocumentlocationid, patchSharePointDocumentLocationEvent);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference SharepointDocumentLocation to event");
                }

                string sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", mdcsdl.Sharepointdocumentlocationid);

                Odataid oDataId = new Odataid()
                {
                    OdataidProperty = sharePointLocationData
                };
                try
                {
                    _dynamicsClient.Events.AddReference(eventEntity.AdoxioEventid, "adoxio_event_SharePointDocumentLocations", oDataId);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, "Error adding reference to SharepointDocumentLocation");
                }
            }
        }

        /// <summary>
        /// Returns the folder name for a given entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="_dynamicsClient"></param>
        /// <returns></returns>
        private static async Task<string> GetFolderName(string entityName, string entityId, IDynamicsClient _dynamicsClient)
        {

            string folderName = null;

            folderName = await GetEntitySharePointDocumentLocation(entityName, entityId, _dynamicsClient).ConfigureAwait(true);

            if (folderName == null)
            {
                switch (entityName.ToLower())
                {
                    case "account":
                        var account = await _dynamicsClient.GetAccountByIdAsync(Guid.Parse(entityId)).ConfigureAwait(true);
                        folderName = account.GetDocumentFolderName();
                        break;
                    case "application":
                        var application = await _dynamicsClient.GetApplicationById(Guid.Parse(entityId)).ConfigureAwait(true);
                        folderName = application.GetDocumentFolderName();
                        break;
                    case "contact":
                        var contact = await _dynamicsClient.GetContactById(Guid.Parse(entityId)).ConfigureAwait(true);
                        folderName = contact.GetDocumentFolderName();
                        break;
                    case "worker":
                        var worker = await _dynamicsClient.GetWorkerById(Guid.Parse(entityId)).ConfigureAwait(true);
                        folderName = worker.GetDocumentFolderName();
                        break;
                    case "event":
                        var eventEntity = _dynamicsClient.GetEventById(Guid.Parse(entityId));
                        folderName = eventEntity.GetDocumentFolderName();
                        break;
                    default:
                        break;
                }
            }
            
            return folderName;
        }

        /// <summary>
        ///  helper function used by the public file upload features to verify that the user has access.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsPublicUserAuthorized(string entityName, string entityId)
        {
            // currently this service only supports contacts
            bool authorized = true;
            if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(entityId) || entityName != "contact")
            {
                authorized = false;
            }
            else
            {
                // lookup the contact
                var contact = await _dynamicsClient.GetContactById(entityId);
                if (contact == null)
                {
                    authorized = false;
                }
                else
                {
                    // treat empty value as incomplete.
                    if (contact.AdoxioPhscomplete == null && contact.AdoxioPhscomplete == 845280001)
                    {
                        authorized = false;
                    }
                }

            }
            return authorized;
        }

        [HttpDelete("{token}/public-attachments/{entityName}")]
        [AllowAnonymous]
        public async Task<IActionResult> PublicDeleteFile([FromRoute] string token, [FromQuery] string serverRelativeUrl, [FromQuery] string documentType, [FromRoute] string entityName)
        {
            // decode the entityID
            string entityId = EncryptionUtility.DecryptStringHex(token, _encryptionKey);

            bool authorized = await IsPublicUserAuthorized(entityName, entityId).ConfigureAwait(true);

            if (authorized)
            {
                return await DeleteFileInternal(serverRelativeUrl, documentType, entityId, entityName, false).ConfigureAwait(true);
            }
            else
            {
                return Unauthorized();
            }
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
        public async Task<IActionResult> PublicDownloadAttachment(string token, string entityName, [FromQuery]string serverRelativeUrl, [FromQuery]string documentType)
        {
            // decode the entityID
            string entityId = EncryptionUtility.DecryptStringHex(token, _encryptionKey);

            bool authorized = await IsPublicUserAuthorized(entityName, entityId).ConfigureAwait(true);

            if (authorized)
            {
                return await DownloadAttachmentInternal(entityId, entityName, serverRelativeUrl, documentType, false).ConfigureAwait(true);
            }
            else
            {
                return Unauthorized();
            }
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
            string entityId = EncryptionUtility.DecryptStringHex(token, _encryptionKey);

            bool authorized = await IsPublicUserAuthorized(entityName, entityId).ConfigureAwait(true);

            if (authorized)
            {
                return await GetAttachmentsInternal(entityId, entityName, documentType, false);
            }
            else
            {
                return Unauthorized();
            }

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
          [FromForm]IFormFile file, [FromForm] string documentType)
        {
            // decode the entityID
            string entityId = EncryptionUtility.DecryptStringHex(token, _encryptionKey);

            bool authorized = await IsPublicUserAuthorized(entityName, entityId).ConfigureAwait(true);

            if (authorized)
            {
                return await UploadAttachmentInternal(entityId, entityName, file, documentType, false).ConfigureAwait(true);
            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpPost("{id}/public-covid-application")]
        [DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<IActionResult> PublicCovidApplication([FromRoute] string id,
          [FromForm]IFormFile file, [FromForm] string documentType)
        {
            string entityName = "application";
            // decode the entityID
            var application = _dynamicsClient.GetApplicationById(id);
            if (application == null) 
            { 
                return BadRequest();  
            }
            else
            {
                return await UploadAttachmentInternal(id, entityName, file, documentType, false).ConfigureAwait(true);
            }
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
            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType))
            {
                return BadRequest();
            }

            bool hasAccess = true;

            if (checkUser)
            {
                ValidateSession();
                hasAccess = await CanAccessEntity(entityName, entityId);
            }

            if (!hasAccess)
            {
                return new NotFoundResult();
            }
            else
            {
                List<ViewModels.FileSystemItem> fileSystemItemVMList = await FileController.GetListFilesInFolder(entityId, entityName, documentType, _dynamicsClient, _fileManagerClient, _logger);
                return new JsonResult(fileSystemItemVMList);
            }
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

                case "contact":
                    var contact = await _dynamicsClient.GetContactById(entityId);
                    folderName = contact.GetDocumentFolderName();
                    break;

                case "worker":
                    var worker = await _dynamicsClient.GetWorkerById(entityId);
                    folderName = worker.GetDocumentFolderName();
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

                    result.Add(new ViewModels.DocumentTypeStatus() { DocumentType = documentTypePrefix, Name = documentTypeName, Valid = valid, RouterLink = routerLink });
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

        /// <summary>
        /// Return the list of files in a given folder.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityName"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        public static async Task<List<ViewModels.FileSystemItem>> GetListFilesInFolder(string entityId, string entityName, string documentType, IDynamicsClient _dynamicsClient, FileManagerClient _fileManagerClient, ILogger _logger)
        {
            List<ViewModels.FileSystemItem> fileSystemItemVMList = new List<ViewModels.FileSystemItem>();

            // 4-9-2020 - GW removed session check to resolve issue with PHS links not working.  Session checks occur further up the call stack.

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
            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(serverRelativeUrl) || string.IsNullOrEmpty(documentType))
            {
                return BadRequest();
            }

            bool hasAccess = true;
            if (checkUser)
            {
                ValidateSession();
                hasAccess = await CanAccessEntityFile(entityName, entityId, documentType, serverRelativeUrl, true);
            }

            if (!hasAccess)
            {
                return new NotFoundResult();
            }

            string logUrl = WordSanitizer.Sanitize(serverRelativeUrl);

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
                _logger.LogError($"ERROR in deleting file {logUrl} - {deleteResult.ErrorDetail}");
                throw new Exception($"ERROR in deleting file {logUrl} - {deleteResult.ErrorDetail}");
            }

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
            [FromForm]IFormFile file, [FromForm] string documentType)
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
            ViewModels.FileSystemItem result = null;            

            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(documentType))
            {
                return BadRequest();
            }

            bool hasAccess = true;
            if (checkUser)
            {
                ValidateSession();
                hasAccess = await CanAccessEntity(entityName, entityId).ConfigureAwait(true);
            }

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
            
            string folderName = await GetFolderName(entityName, entityId, _dynamicsClient).ConfigureAwait(true);

            await CreateEntitySharePointDocumentLocation(entityName, entityId, folderName, folderName);

            MemoryStream ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            byte[] data = ms.ToArray();

            // call the web service
            var uploadRequest = new UploadFileRequest()
            {
                ContentType = file.ContentType,
                Data = ByteString.CopyFrom(data),
                EntityName = entityName,
                FileName = fileName,
                FolderName = folderName
            };

            var uploadResult = _fileManagerClient.UploadFile(uploadRequest);

            string logFolderName = WordSanitizer.Sanitize(folderName);
            string logFileName = WordSanitizer.Sanitize(fileName);

            if (uploadResult.ResultStatus == ResultStatus.Success)
            {
                // Update modifiedon to current time
                UpdateEntityModifiedOnDate(entityName, entityId, true);
                _logger.LogInformation($"SUCCESS in uploading file {logFileName} to folder {logFolderName}");
            }
            else
            {
                _logger.LogError($"ERROR in uploading file {logFileName} to folder {logFolderName}");
                throw new Exception($"ERROR in uploading file {logFileName} to folder {logFolderName}");
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Utility function used when we want to update the last modified date on a given record; this is used to ensure staff are aware files have been added.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="setUploadedFromPortal"></param>

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

        /// <summary>
        /// Utility function used to verify that the current user session is valid.  Only used for authenticated endpoints
        /// </summary>
        private void ValidateSession()
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();
        }
    }
}
