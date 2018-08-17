using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using static Gov.Lclb.Cllb.Interfaces.SharePointFileManager;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class AdoxioLegalEntityController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly string _encryptionKey;
        private readonly SharePointFileManager _sharePointFileManager;

        public AdoxioLegalEntityController(IConfiguration configuration, SharePointFileManager sharePointFileManager, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            Configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _encryptionKey = Configuration["ENCRYPTION_KEY"];
            _sharePointFileManager = sharePointFileManager;
            _logger = loggerFactory.CreateLogger(typeof(AdoxioLegalEntityController));
        }

        /// <summary>
        /// Get all Dynamics Legal Entities
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet()]
        public JsonResult GetDynamicsLegalEntities()
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            IEnumerable<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = null;
            String accountfilter = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();

            // set account filter
            accountfilter = "_adoxio_account_value eq " + userSettings.AccountId;
            _logger.LogError("Account filter = " + accountfilter);

            legalEntities = _dynamicsClient.Adoxiolegalentities.Get(filter: accountfilter).Value;

            foreach (var legalEntity in legalEntities)
            {
                result.Add(legalEntity.ToViewModel());
            }

            return Json(result);
        }

        /// <summary>
        /// Get all Dynamics Legal Entities for the current Business Profile Summary
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("business-profile-summary")]
        public JsonResult GetBusinessProfileSummary()
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();

            List<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = GetAccountLegalEntities(userSettings.AccountId);

            foreach (var legalEntity in legalEntities)
            {
                result.Add(legalEntity.ToViewModel());
            }

            return Json(result);
        }

        private List<MicrosoftDynamicsCRMadoxioLegalentity> GetAccountLegalEntities(string accountId)
        {
            List<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = null;
            var filter = "_adoxio_account_value eq " + accountId;
            filter += " and adoxio_isindividual eq 0";

            var response = _dynamicsClient.Adoxiolegalentities.Get(filter: filter);

            if (response != null && response.Value != null)
            {
                legalEntities = response.Value.ToList();
                var children = new List<MicrosoftDynamicsCRMadoxioLegalentity>();
                foreach (var legalEntity in legalEntities)
                {
                    if (!String.IsNullOrEmpty(legalEntity._adoxioShareholderaccountidValue))
                    {
                        children.AddRange(GetAccountLegalEntities(legalEntity._adoxioShareholderaccountidValue));
                    }
                }
                legalEntities.AddRange(children);
            }
            return legalEntities.Distinct().ToList();

        }

        /// <summary>
        /// Get all Legal Entities where the position matches the parameter received
        /// By default, the account linked to the current user is used
        /// </summary>
        /// <param name="positionType"></param>
        /// <param name="parentLegalEntityId"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("position/{parentLegalEntityId}/{positionType}")]
        public IActionResult GetDynamicsLegalEntitiesByPosition(string parentLegalEntityId, string positionType)
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            IEnumerable<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = null;
            String filter = null;

            // Stops injections
            try
            {
                new Guid(parentLegalEntityId);
            }
            catch
            {
                return NotFound();
            }

            filter = "_adoxio_legalentityowned_value eq " + parentLegalEntityId;
            switch (positionType)
            {
                case "shareholders":
                    filter += " and adoxio_isshareholder eq true";
                    break;
                case "partners":
                    filter += " and adoxio_ispartner eq true";
                    break;
                case "directors-officers-management":
                    filter += " and adoxio_isshareholder ne true and adoxio_ispartner ne true";
                    break;
                case "director-officer-shareholder":
                    filter += " and adoxio_isindividual eq 1";
                    break;
                default:
                    break;
            }

            try
            {
                _logger.LogError("Account filter = " + filter);
                legalEntities = _dynamicsClient.Adoxiolegalentities.Get(filter: filter).Value;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }

            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            if (legalEntities != null)
            {
                foreach (var legalEntity in legalEntities)
                {
                    // Users can't access other users legal entities.
                    if (!DynamicsExtensions.CurrentUserHasAccessToAccount(new Guid(legalEntity._adoxioAccountValue), _httpContextAccessor, _dynamicsClient))
                    {
                        return NotFound();
                    }
                    result.Add(legalEntity.ToViewModel());
                }
            }

            return Json(result);
        }

        /// <summary>
        /// Get the special applicant legal entity for the current user
        /// </summary>
        /// <returns></returns>
        [HttpGet("applicant")]
        public async Task<IActionResult> GetApplicantDynamicsLegalEntity()
        {
            ViewModels.AdoxioLegalEntity result = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();

            // query the Dynamics system to get the legal entity record.
            MicrosoftDynamicsCRMadoxioLegalentity legalEntity = null;
            _logger.LogError("Find legal entity for applicant = " + userSettings.AccountId.ToString());

            legalEntity = _dynamicsClient.GetAdoxioLegalentityByAccountId(Guid.Parse(userSettings.AccountId));

            if (legalEntity == null)
            {
                return new NotFoundResult();
            }
            // fix the account.

            result = legalEntity.ToViewModel();

            if (result.account == null)
            {
                MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountById(Guid.Parse(userSettings.AccountId));
                result.account = account.ToViewModel();
            }

            return Json(result);
        }

        /// <summary>
        /// Get a specific legal entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDynamicsLegalEntity(string id)
        {
            ViewModels.AdoxioLegalEntity result = null;
            // query the Dynamics system to get the legal entity record.
            if (string.IsNullOrEmpty(id))
            {
                return new NotFoundResult();
            }
            else
            {
                // get the current user.
                string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
                UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

                Guid adoxio_legalentityid = new Guid(id);
                MicrosoftDynamicsCRMadoxioLegalentity adoxioLegalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);
                //prevent getting legal entity data if the user is not associated with the account
                if (adoxioLegalEntity == null || !DynamicsExtensions.CurrentUserHasAccessToAccount(new Guid(adoxioLegalEntity._adoxioAccountValue), _httpContextAccessor, _dynamicsClient))
                {
                    return new NotFoundResult();
                }
                result = adoxioLegalEntity.ToViewModel();
            }

            return Json(result);
        }

        /// <summary>
        /// Get the file details list in folder associated to the account folder and document type
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        [HttpGet("{accountId}/attachments/{documentType}")]
        public async Task<IActionResult> GetFileDetailsListInFolder([FromRoute] string accountId, [FromRoute] string documentType)
        {
            List<ViewModels.FileSystemItem> fileSystemItemVMList = new List<ViewModels.FileSystemItem>();

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();
            var accountGUID = new Guid(accountId);

            // User should not be able to access accounts that they do not directly own
            if (!DynamicsExtensions.CurrentUserHasAccessToAccount(accountGUID, _httpContextAccessor, _dynamicsClient))
            {
                return NotFound();
            }

            var account = await _dynamicsClient.GetAccountById(accountGUID);
            if (account != null)
            {
                var accountIdCleaned = account.Accountid.ToString().ToUpper().Replace("-", "");
                string folderName = $"{account.Name}_{accountIdCleaned}";
                // Get the file details list in folder
                List<FileDetailsList> fileDetailsList = null;
                try
                {
                    fileDetailsList = await _sharePointFileManager.GetFileDetailsListInFolder(SharePointFileManager.DefaultDocumentListTitle, folderName, documentType);
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
                _logger.LogError("Account not found.");
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
        [HttpDelete("{id}/attachments")]
        public async Task<IActionResult> DeleteFile([FromQuery] string serverRelativeUrl, [FromRoute] string id)
        {
            // get the file.
            if (id == null || serverRelativeUrl == null)
            {
                return BadRequest();
            }
            else
            {
                // get the current user.
                string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
                UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

                Guid adoxio_legalentityid = new Guid(id);
                MicrosoftDynamicsCRMadoxioLegalentity adoxioLegalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);
                //prevent getting legal entity data if the user is not associated with the account
                if (adoxioLegalEntity == null || !DynamicsExtensions.CurrentUserHasAccessToAccount(new Guid(adoxioLegalEntity._adoxioAccountValue), _httpContextAccessor, _dynamicsClient))
                {
                    return new NotFoundResult();
                }

                var result = await _sharePointFileManager.DeleteFile(serverRelativeUrl);
                if (result)
                {
                    return new OkResult();
                }
            }
            return new NotFoundResult();
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

            Guid adoxio_legalentityid = new Guid(id);
            MicrosoftDynamicsCRMadoxioLegalentity adoxioLegalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);
            //prevent getting legal entity data if the user is not associated with the account
            if (adoxioLegalEntity == null || !DynamicsExtensions.CurrentUserHasAccessToAccount(new Guid(adoxioLegalEntity._adoxioAccountValue), _httpContextAccessor, _dynamicsClient))
            {
                return new NotFoundResult();
            }

            if (id != null)
            {
                var effectiveAccountId = adoxioLegalEntity._adoxioShareholderaccountidValue ?? adoxioLegalEntity._adoxioAccountValue;
                var accountIdGUID = Guid.Parse(effectiveAccountId);
                var account = await _dynamicsClient.GetAccountById(accountIdGUID);

                if (account == null)
                {
                    return new NotFoundResult();
                }

                string fileName = FileSystemItemExtensions.CombineNameDocumentType(file.FileName, documentType);
                var accountIdCleaned = account.Accountid.ToString().ToUpper().Replace("-", "");
                string folderName = $"{account.Name}_{accountIdCleaned}";

                try
                {
                    await _sharePointFileManager.AddFile(folderName, fileName, file.OpenReadStream(), file.ContentType);
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


        /// <summary>
        /// Create a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateDynamicsLegalEntity([FromBody] ViewModels.AdoxioLegalEntity item)
        {

            // create a new legal entity.
            MicrosoftDynamicsCRMadoxioLegalentity adoxioLegalEntity = new MicrosoftDynamicsCRMadoxioLegalentity();

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();
            // copy received values to Dynamics LegalEntity
            adoxioLegalEntity.CopyValues(item);
            try
            {
                adoxioLegalEntity = await _dynamicsClient.Adoxiolegalentities.CreateAsync(adoxioLegalEntity);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error creating legal entity");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                throw new Exception("Unable to create legal entity");
            }

            // setup navigation properties.
            MicrosoftDynamicsCRMadoxioLegalentity patchEntity = new MicrosoftDynamicsCRMadoxioLegalentity();
            Guid accountId = Guid.Parse(userSettings.AccountId);
            var userAccount = await _dynamicsClient.GetAccountById(accountId);
            patchEntity.AdoxioAccountValueODataBind = _dynamicsClient.GetEntityURI("accounts", accountId.ToString());

            // patch the record.
            try
            {
                await _dynamicsClient.Adoxiolegalentities.UpdateAsync(adoxioLegalEntity.AdoxioLegalentityid, patchEntity);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error patching legal entity");
                _logger.LogError(odee.Request.RequestUri.ToString());
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
            }

            // TODO take the default for now from the parent account's legal entity record
            // TODO likely will have to re-visit for shareholders that are corporations/organizations
            MicrosoftDynamicsCRMadoxioLegalentity tempLegalEntity = _dynamicsClient.GetAdoxioLegalentityByAccountId(Guid.Parse(userSettings.AccountId));
            if (tempLegalEntity != null)
            {
                Guid tempLegalEntityId = Guid.Parse(tempLegalEntity.AdoxioLegalentityid);

                // see https://msdn.microsoft.com/en-us/library/mt607875.aspx
                patchEntity = new MicrosoftDynamicsCRMadoxioLegalentity();
                patchEntity.AdoxioLegalEntityOwnedODataBind = _dynamicsClient.GetEntityURI("adoxio_legalentities", tempLegalEntityId.ToString());

                // patch the record.
                try
                {
                    await _dynamicsClient.Adoxiolegalentities.UpdateAsync(adoxioLegalEntity.AdoxioLegalentityid, patchEntity);
                }
                catch (OdataerrorException odee)
                {
                    _logger.LogError("Error adding LegalEntityOwned reference to legal entity");
                    _logger.LogError(odee.Request.RequestUri.ToString());
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                }
            }

            return Json(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Create a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("child-legal-entity")]
        public async Task<IActionResult> CreateDynamicsShareholderLegalEntity([FromBody] ViewModels.AdoxioLegalEntity item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            var adoxioLegalEntity = new MicrosoftDynamicsCRMadoxioLegalentity();
            adoxioLegalEntity.CopyValues(item);

            if (item.isindividual != true)
            {
                var account = new MicrosoftDynamicsCRMaccount();
                account.Name = item.name;
                if (item.isShareholder == true)
                {
                    account.AdoxioAccounttype = (int)AdoxioAccountTypeCodes.Shareholder;
                }
                else if (item.isPartner == true)
                {
                    account.AdoxioAccounttype = (int)AdoxioAccountTypeCodes.Partner;
                }
                account.AdoxioBusinesstype = (int)Enum.ToObject(typeof(Gov.Lclb.Cllb.Public.ViewModels.AdoxioApplicantTypeCodes), item.legalentitytype);
                account = await _dynamicsClient.Accounts.CreateAsync(account);

                //create tied house under account
                var tiedHouse = new MicrosoftDynamicsCRMadoxioTiedhouseconnection()
                {
                };
                tiedHouse.AccountODataBind = _dynamicsClient.GetEntityURI("accounts", account.Accountid);
                adoxioLegalEntity.AdoxioShareholderAccountODataBind = _dynamicsClient.GetEntityURI("accounts", account.Accountid);

                var res = await _dynamicsClient.AdoxioTiedhouseconnections.CreateAsync(tiedHouse);
            }
            adoxioLegalEntity.AdoxioAccountValueODataBind = _dynamicsClient.GetEntityURI("accounts", item.account.id);


            adoxioLegalEntity.AdoxioLegalEntityOwnedODataBind = _dynamicsClient.GetEntityURI("adoxio_legalentities", item.parentLegalEntityId);

            adoxioLegalEntity = await _dynamicsClient.Adoxiolegalentities.CreateAsync(adoxioLegalEntity);

            return Json(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Update a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDynamicsLegalEntity([FromBody] ViewModels.AdoxioLegalEntity item, string id)
        {
            if (id != item.id)
            {
                return BadRequest();
            }

            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);

            MicrosoftDynamicsCRMadoxioLegalentity adoxioLegalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);
            if (adoxioLegalEntity == null)
            {
                return new NotFoundResult();
            }

            // we are doing a patch, so wipe out the record.
            adoxioLegalEntity = new MicrosoftDynamicsCRMadoxioLegalentity();

            // copy values over from the data provided
            adoxioLegalEntity.CopyValues(item);

            await _dynamicsClient.Adoxiolegalentities.UpdateAsync(adoxio_legalentityid.ToString(), adoxioLegalEntity);

            adoxioLegalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);

            return Json(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Delete a legal entity.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteDynamicsLegalEntity(string id)
        {
            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            MicrosoftDynamicsCRMadoxioLegalentity legalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);
            if (legalEntity == null)
            {
                return new NotFoundResult();
            }

            try
            {
                await _dynamicsClient.Adoxiolegalentities.DeleteAsync(adoxio_legalentityid.ToString());
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error deleting legal entity");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                throw new Exception("Unable to delete legal entity");
            }

            return NoContent(); // 204
        }
        /// <summary>
        /// Generate a link to be sent to an email address.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="individualId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private string GetConsentLink(string email, string individualId, string parentId)
        {
            string result = Configuration["BASE_URI"] + Configuration["BASE_PATH"];

            result += "/bcservice?path=/security-consent/" + parentId + "/" + individualId + "?code=";

            // create a newsletter confirmation object.

            ViewModels.SecurityConsentConfirmation securityConsentConfirmation = new ViewModels.SecurityConsentConfirmation()
            {
                email = email,
                parentid = parentId,
                individualid = individualId
            };

            // convert it to a json string.
            string json = JsonConvert.SerializeObject(securityConsentConfirmation);

            // encrypt that using two way encryption.

            result += System.Net.WebUtility.UrlEncode(EncryptionUtility.EncryptString(json, _encryptionKey));

            return result;
        }

        [HttpGet("{id}/verifyconsentcode/{individualid}")]
        public JsonResult VerifyConsentCode(string id, string individualid, string code)
        {
            string result = "Error";
            // validate the code.

            string decrypted = EncryptionUtility.DecryptString(code, _encryptionKey);
            if (decrypted != null)
            {
                // convert the json back to an object.
                ViewModels.SecurityConsentConfirmation consentConfirmation = JsonConvert.DeserializeObject<ViewModels.SecurityConsentConfirmation>(decrypted);
                // check that the keys match.
                if (id.Equals(consentConfirmation.parentid) && individualid.Equals(consentConfirmation.individualid))
                {
                    // update the appropriate dynamics record here.
                    result = "Success";
                }
            }
            return Json(result);
        }


        /// <summary>
        /// send consent requests to the supplied list of legal entities.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost("{id}/sendconsentrequests")]
        public async Task<IActionResult> SendConsentRequests(string id, [FromBody] List<string> recipientIds)
        {
            // start by getting the record for the current legal entity.
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();

            var userAccount = await _dynamicsClient.GetAccountById(Guid.Parse(userSettings.AccountId));
            MicrosoftDynamicsCRMadoxioLegalentity userLegalentity = _dynamicsClient.GetAdoxioLegalentityByAccountId(Guid.Parse(userSettings.AccountId));

            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);

            // TODO verify that this is the current user's legal entity
            var adoxioLegalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);

            // now get each of the supplied ids and send an email to them.

            foreach (string recipientId in recipientIds)
            {
                Guid recipientIdGuid = new Guid(recipientId);
                // TODO verify that each recipient is part of the current user's user set
                // TODO switch over to new AuthAPI framework
                var recipientEntity = await _dynamicsClient.GetLegalEntityById(recipientIdGuid);
                string email = recipientEntity.AdoxioEmail;
                string firstname = recipientEntity.AdoxioFirstname;
                string lastname = recipientEntity.AdoxioLastname;

                string confirmationEmailLink = GetConsentLink(email, recipientId, id);
                string bclogo = Configuration["BASE_URI"] + Configuration["BASE_PATH"] + "/assets/bc-logo.svg";
                /* send the user an email confirmation. */
                string body =
                        "<img src='" + bclogo + "'/><br><h2>Security Screening and Financial Integrity Checks</h2>"
                    + "<p>"
                    + "Dear " + firstname + " " + lastname + ","
                    + "</p>"
                    + "<p>"
                    + "An application from " + "[TBD Company Name]"
                    + " has been submitted for a non-medical retail cannabis licence in British Columbia. "
                    + "As a " + "[TBD Position]" + " of " + "[TBD Company Name]"
                    + " you are required to authorize a security screening — including criminal and police record checks—"
                    + "and financial integrity checks as part of the application process. "
                    + "</p>"
                    + "<p>"
                    + "Where you reside will determine how you are able to authorize the security screening."
                    + "</p>"
                    + "<p><strong>B.C. Residents</strong></p>"
                    + "<p>"
                    + "Residents of B.C. require a Photo B.C. Services Card to login to the application.  A Services Card "
                    + "verifies your identity, and has enhanced levels of security making the card more secure and helps protect your privacy."
                    + "</p>"
                    + "<p>"
                    + "If you don’t have a B.C. Services Card, or haven’t activated it for online login, visit the B.C. Services Card website to find how to get a card."
                    + "</p>"
                    + "<p>"
                    + "After you receive your verified Photo B.C. Services Card, login through this unique link:"
                    + "</p>"
                    + "<p><a href='" + confirmationEmailLink + "'>" + confirmationEmailLink + "</a></p>"
                    + "<p><strong>Out of Province Residents</strong></p>"
                    + "<p>TBD</p>"
                    + "<p><strong>Residents Outside of Canada</strong></p>"
                    + "<p>TBD</p>"
                    + "<p>If you have any questions about the security authorization, contact helpdesk@lclbc.ca</p>"
                    + "<p>Do not reply to this email address</p>";

                // send the email.
                SmtpClient client = new SmtpClient(Configuration["SMTP_HOST"]);

                // Specify the message content.
                MailMessage message = new MailMessage("no-reply@gov.bc.ca", email);
                message.Subject = "BC LCLB Cannabis Licensing Security Consent";
                message.Body = body;
                message.IsBodyHtml = true;
                client.Send(message);


                // save the consent link and the fact that the email has been sent
                MicrosoftDynamicsCRMadoxioLegalentity patchEntity = new MicrosoftDynamicsCRMadoxioLegalentity();
                patchEntity.AdoxioDateemailsent = DateTime.Now;

                // patch the record.
                try
                {
                    await _dynamicsClient.Adoxiolegalentities.UpdateAsync(adoxioLegalEntity.AdoxioLegalentityid, patchEntity);
                }
                catch (OdataerrorException odee)
                {
                    _logger.LogError("Error updating date email sent.");
                    _logger.LogError(odee.Request.RequestUri.ToString());
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                }
            }

            return NoContent(); // 204
        }
    }
}
