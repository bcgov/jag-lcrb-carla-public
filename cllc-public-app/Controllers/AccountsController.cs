extern alias DV;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using System.Security.Claims;
using Gov.Lclb.Cllb.Public.Extensions;
using Gov.Lclb.Cllb.Services.FileManager;
using Contact = Gov.Lclb.Cllb.Public.ViewModels.Contact;
using Gov.Lclb.Cllb.Public.Repositories;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DvAccount = DV::Gov.Lclb.Cllb.Interfaces.Account;
using DvContact = DV::Gov.Lclb.Cllb.Interfaces.Contact;
using DvLegalEntity = DV::Gov.Lclb.Cllb.Interfaces.adoxio_legalentity;
using DvAdoxioApplicantTypeCodes = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicanttypecodes;
using DvAdoxioAccountType = DV::Gov.Lclb.Cllb.Interfaces.adoxio_accounttype;
using DvAdoxioGeneralYesNo = DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly BCeIDBusinessQuery _bceid;
        private readonly IConfiguration _configuration;
        private readonly IDataverseClient _dataverse;
        private readonly IOrgBookClient _orgBookclient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly FileManagerClient _fileManagerClient;
        private readonly IWebHostEnvironment _env;
        private readonly TiedHouseConnectionsRepository _tiedHouseConnectionsRepository;

        public AccountsController(IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IOrgBookClient orgBookClient,
            BCeIDBusinessQuery bceid,
            ILoggerFactory loggerFactory,
            IDataverseClient dataverse,
            FileManagerClient fileManagerClient,
            IWebHostEnvironment env,
            TiedHouseConnectionsRepository tiedHouseConnectionsRepository
        )
        {
            _configuration = configuration;
            _bceid = bceid;
            _dataverse = dataverse;
            _env = env;
            _tiedHouseConnectionsRepository = tiedHouseConnectionsRepository;
            _orgBookclient = orgBookClient;
            _httpContextAccessor = httpContextAccessor;
            _fileManagerClient = fileManagerClient;
            _logger = loggerFactory.CreateLogger(typeof(AccountsController));
        }

        /// GET account in Dynamics for the current user
        [HttpGet("current")]
        [Authorize(Policy = "Business-User")]
        public async Task<IActionResult> GetCurrentAccount()
        {
            _logger.LogDebug(LoggingEvents.HttpGet, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            ViewModels.Account result = null;

            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            // query the Dynamics system to get the account record.
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                var accountId = GuidUtility.SanitizeGuidString(userSettings.AccountId);
                DvAccount account = await _dataverse.GetAccountByIdAsync(accountId);
                _logger.LogDebug(LoggingEvents.HttpGet, "Dynamics Account: " + JsonConvert.SerializeObject(account));

                if (account == null)
                {
                    // Sometimes we receive the siteminderbusienssguid instead of the account id.
                    account = await _dataverse.GetAccountByExternalIdAsync(accountId);
                    if (account == null)
                    {
                        _logger.LogWarning(LoggingEvents.NotFound, "No Account Found.");
                        return new NotFoundResult();
                    }
                }
                result = account.ToViewModel();
                if (account.PrimaryContactId != null)
                {
                    var primaryContact = await _dataverse.GetContactByIdAsync(account.PrimaryContactId.Id.ToString());
                    if (primaryContact != null)
                        result.primarycontact = primaryContact.ToViewModel();
                }
            }
            else
            {
                _logger.LogWarning(LoggingEvents.NotFound, "No Account Found.");
                return new NotFoundResult();
            }

            _logger.LogDebug(LoggingEvents.HttpGet, "Current Account Result: " +
               JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            return new JsonResult(result);
        }

        /// GET the contacts for the current account.
        [HttpGet("current/contacts")]
        [Authorize(Policy = "Business-User")]
        public async Task<IActionResult> GetCurrentAccountContacts()
        {
            List<ViewModels.Contact> result = new List<Contact>();

            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            // query the Dynamics system to get the account record.
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                var contacts = await _dataverse.GetContactsByAccountIdAsync(userSettings.AccountId);
                if (contacts != null)
                {
                    foreach (var contact in contacts)
                    {
                        result.Add(contact.ToViewModel());
                    }
                }
            }
            else
            {
                _logger.LogWarning(LoggingEvents.NotFound, "GetCurrentAccountContacts - No Current Account Found.");
            }

            return new JsonResult(result);
        }

        /// GET account in Dynamics for the current user
        [HttpGet("bceid")]
        [Authorize(Policy = "Business-User")]
        public async Task<IActionResult> GetCurrentBCeIDBusiness()
        {
            _logger.LogDebug(LoggingEvents.HttpGet, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);

            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            _logger.LogDebug(LoggingEvents.HttpGet, "UserSettings: " + JsonConvert.SerializeObject(userSettings));

            // query the BCeID API to get the business record.
            var business = await _bceid.ProcessBusinessQuery(userSettings.SiteMinderGuid);

            _logger.LogDebug(LoggingEvents.Get, $"business Info from bceid: {JsonConvert.SerializeObject(business)}");

            var cleanNumber = BusinessNumberSanitizer.SanitizeNumber(business?.businessNumber);
            if (cleanNumber != null)
            {
                business.businessNumber = cleanNumber;
            }

            if (business == null)
            {
                _logger.LogWarning(LoggingEvents.NotFound, "No Business Found.");
                return new NotFoundResult();
            }

            _logger.LogDebug(LoggingEvents.HttpGet, "BCeID business record: " +
                JsonConvert.SerializeObject(business, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            return new JsonResult(business);
        }

        /// <summary>
        /// Get Autocomplete data for a given name using startswith
        /// </summary>
        /// <param name="name">The name to filter by using startswith</param>
        /// <returns>Dictionary of key value pairs with accountid and name as the pairs</returns>
        [HttpGet("autocomplete")]
        [Authorize(Policy = "Business-User")]
        public async Task<IActionResult> GetAutocomplete(string name)
        {
            var results = new List<TransferAccount>();
            try
            {
                string filter = null;
                if (name != null)
                {
                    name = name.Replace("'", "''");
                    filter = $"%{name}%";
                }
                var accounts = (await _dataverse.GetAccountsAsync(filter, activeOnly: true)).Take(10);
                foreach (var account in accounts)
                {
                    var transferAccount = new TransferAccount
                    {
                        AccountId = account.AccountId?.ToString() ?? account.Id.ToString(),
                        AccountName = account.Name,
                        BusinessType = (AdoxioApplicantTypeCodes?)((int?)account.adoxio_BusinessType)
                    };
                    if (account.PrimaryContactId != null)
                    {
                        var contact = await _dataverse.GetContactByIdAsync(account.PrimaryContactId.Id.ToString());
                        if (contact != null)
                            transferAccount.ContactName = $"{contact.FirstName} {contact.LastName}";
                    }
                    results.Add(transferAccount);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting autocomplete data.");
            }

            return new JsonResult(results);
        }

        /// <summary>
        /// Get a specific legal entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "Business-User")]
        public async Task<IActionResult> GetAccount(string id)
        {
            _logger.LogDebug(LoggingEvents.HttpGet, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            _logger.LogDebug(LoggingEvents.HttpGet, "id: " + id);

            Boolean userAccessToAccount = false;
            ViewModels.Account result = null;

            // query the Dynamics system to get the account record.
            if (id != null)
            {
                // verify the currently logged in user has access to this account
                Guid accountId = new Guid(id);

                try
                {
                    userAccessToAccount = await DynamicsExtensions.CurrentUserHasAccessToAccountAsync(accountId, _httpContextAccessor, _dataverse);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while checking if current user has access to account.");
                }

                if (!userAccessToAccount)
                {
                    _logger.LogWarning(LoggingEvents.NotFound, "Current user has NO access to account.");
                    return new NotFoundResult();
                }

                DvAccount account = await _dataverse.GetAccountByIdAsync(id);
                if (account == null)
                {
                    _logger.LogWarning(LoggingEvents.NotFound, "Account NOT found.");
                    return new NotFoundResult();
                }
                result = account.ToViewModel();
                if (account.PrimaryContactId != null)
                {
                    var primaryContact = await _dataverse.GetContactByIdAsync(account.PrimaryContactId.Id.ToString());
                    if (primaryContact != null)
                        result.primarycontact = primaryContact.ToViewModel();
                }
            }
            else
            {
                _logger.LogWarning(LoggingEvents.BadRequest, "Bad Request.");
                return BadRequest();
            }

            _logger.LogDebug(LoggingEvents.HttpGet, "Account result: " +
                JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            return new JsonResult(result);
        }

        [HttpGet("business-profile/{accountId}")]
        [Authorize(Policy = "Business-User")]
        public async Task<IActionResult> GetBusinessProfile(string accountId)
        {
            _logger.LogDebug(LoggingEvents.HttpGet, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            _logger.LogDebug(LoggingEvents.HttpGet, "accountId: {accountId}");

            List<BusinessProfileLegalEntity> legalEntities;

            var accountEntity = await _dataverse.GetAccountByIdAsync(accountId);
            var account = accountEntity?.ToViewModel();
            _logger.LogDebug(LoggingEvents.HttpGet, "Account details: " + JsonConvert.SerializeObject(account));

            try
            {
                var legalEntityList = await _dataverse.GetLegalEntitiesByAccountIdAsync(accountId);
                var leTasks = legalEntityList.Select(async le =>
                {
                    var legalEntity = le.ToViewModel();
                    ViewModels.Account leAccount;
                    if (le.adoxio_ShareholderAccountID != null)
                    {
                        var shareholderAccount = await _dataverse.GetAccountByIdAsync(le.adoxio_ShareholderAccountID.Id.ToString());
                        leAccount = shareholderAccount?.ToViewModel() ?? account;
                    }
                    else
                    {
                        leAccount = account;
                    }
                    var entity = new BusinessProfileLegalEntity
                    {
                        AdoxioLegalEntity = legalEntity,
                        Account = leAccount
                    };
                    entity.corporateDetailsFilesExists = await FileUploadExists(entity.Account.id, entity.Account.name, "Corporate Information");
                    entity.organizationStructureFilesExists = await FileUploadExists(entity.Account.id, entity.Account.name, "Organization Structure");
                    entity.keyPersonnelFilesExists = await FileUploadExists(entity.Account.id, entity.Account.name, "Key Personnel");
                    entity.financialInformationFilesExists = await FileUploadExists(entity.Account.id, entity.Account.name, "Financial Information");
                    entity.shareholderFilesExists = await FileUploadExists(entity.Account.id, entity.Account.name, "Central Securities Register");
                    var cannabisTiedHouseConnection = await _tiedHouseConnectionsRepository.GetCannabisTiedHouseConnectionForUser(entity.Account.id);
                    if (cannabisTiedHouseConnection != null)
                    {
                        entity.TiedHouse = cannabisTiedHouseConnection;
                    }
                    entity.ChildEntities = await GetLegalEntityChildrenAsync(entity.AdoxioLegalEntity.id);
                    return entity;
                });
                legalEntities = (await Task.WhenAll(leTasks)).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting legal entities for the account {accountId}. ");
                return null;
            }

            var profile = new BusinessProfile
            {
                Account = account,
                LegalEntities = legalEntities
            };

            var isComplete = legalEntities.Select(le =>
            {
                var valid = new ProfileValidation
                {
                    LegalEntityId = le.AdoxioLegalEntity.id,
                    IsComplete = (le.IsComplete())
                };
                return valid;
            }).ToList();

            _logger.LogDebug(LoggingEvents.HttpGet, "BusinessProfile.isComplete: " +
                JsonConvert.SerializeObject(isComplete, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            return new JsonResult(isComplete);
        }

        private async Task<List<BusinessProfileLegalEntity>> GetLegalEntityChildrenAsync(string parentLegalEntityId)
        {
            _logger.LogDebug(LoggingEvents.Get, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            _logger.LogDebug(LoggingEvents.Get, "parentLegalEntityId: {parentLegalEntityId}");

            List<BusinessProfileLegalEntity> children = null;

            try
            {
                var childEntities = await _dataverse.GetLegalEntitiesByParentEntityIdAsync(parentLegalEntityId);
                var leTasks = childEntities.Select(async le =>
                {
                    var legalEntity = le.ToViewModel();
                    ViewModels.Account leAccount;
                    if (le.adoxio_ShareholderAccountID != null)
                    {
                        var shareholderAccount = await _dataverse.GetAccountByIdAsync(le.adoxio_ShareholderAccountID.Id.ToString());
                        leAccount = shareholderAccount?.ToViewModel();
                    }
                    else if (le.adoxio_Account != null)
                    {
                        var parentAccount = await _dataverse.GetAccountByIdAsync(le.adoxio_Account.Id.ToString());
                        leAccount = parentAccount?.ToViewModel();
                    }
                    else
                    {
                        leAccount = null;
                    }
                    var entity = new BusinessProfileLegalEntity
                    {
                        AdoxioLegalEntity = legalEntity,
                        Account = leAccount
                    };
                    var cannabisTiedHouseConnection = await _tiedHouseConnectionsRepository.GetCannabisTiedHouseConnectionForUser(entity.Account?.id);
                    if (cannabisTiedHouseConnection != null)
                    {
                        entity.TiedHouse = cannabisTiedHouseConnection;
                    }
                    if (entity.AdoxioLegalEntity.isShareholder == true && entity.AdoxioLegalEntity.isindividual == false)
                    {
                        entity.ChildEntities = await GetLegalEntityChildrenAsync(entity.AdoxioLegalEntity.id);
                    }
                    return entity;
                });
                children = (await Task.WhenAll(leTasks)).ToList();
                _logger.LogDebug(LoggingEvents.Get, "LegalEntityChildren: " +
                    JsonConvert.SerializeObject(children, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting legal entity children for parentLegalEntityId");
                return null;
            }

            return children;
        }

        private async Task<bool> FileUploadExists(string accountId, string accountName, string documentType)
        {
            _logger.LogDebug(LoggingEvents.Get, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            _logger.LogDebug(LoggingEvents.Get, "accountId: {accountId}, accountName: {accountName}, documentType: {documentType}");

            var exists = false;
            var accountIdCleaned = accountId.ToUpper().Replace("-", "");
            var folderName = $"{accountName}_{accountIdCleaned}";

            var fileDetailsList = _fileManagerClient.GetFileDetailsListInFolder(_logger, SharePointConstants.AccountFolderInternalName, accountId, folderName);
            if (fileDetailsList != null)
            {
                exists = fileDetailsList.Count() > 0;
            }

            _logger.LogDebug(LoggingEvents.Get, "FileUploadExists: " + exists);
            return exists;
        }

        [HttpPost]
        [Authorize(Policy = "Can-Create-Account")]
        public async Task<IActionResult> CreateAccount([FromBody] ViewModels.Account item)
        {
            _logger.LogDebug(LoggingEvents.HttpPost, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            _logger.LogDebug(LoggingEvents.HttpPost, "Account parameters: " + JsonConvert.SerializeObject(item));

            ViewModels.Account result = null;
            Guid tryParseOutGuid;

            bool createContact = true;

            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            _logger.LogDebug(LoggingEvents.HttpPost, "UserSettings: " + JsonConvert.SerializeObject(userSettings));

            // get account Siteminder GUID
            string accountSiteminderGuid = userSettings.SiteMinderBusinessGuid;
            if (accountSiteminderGuid == null || accountSiteminderGuid.Length == 0)
            {
                _logger.LogDebug(LoggingEvents.Error, "No account Siteminder Guid exernal id");
                throw new Exception("Error. No accountSiteminderGuid exernal id");
            }

            // first check to see that a contact exists.
            string contactSiteminderGuid = userSettings.SiteMinderGuid;
            if (contactSiteminderGuid == null || contactSiteminderGuid.Length == 0)
            {
                _logger.LogDebug(LoggingEvents.Error, "No Contact Siteminder Guid exernal id");
                throw new Exception("Error. No ContactSiteminderGuid exernal id");
            }

            // get BCeID record for the current user
            Gov.Lclb.Cllb.Interfaces.BCeIDBusiness bceidBusiness = await _bceid.ProcessBusinessQuery(userSettings.SiteMinderGuid);
            _logger.LogDebug(LoggingEvents.Get, $"business Info from bceid: {JsonConvert.SerializeObject(bceidBusiness)}");

            var cleanNumber = BusinessNumberSanitizer.SanitizeNumber(bceidBusiness?.businessNumber);
            if (cleanNumber != null)
            {
                bceidBusiness.businessNumber = cleanNumber;
            }

            _logger.LogDebug(LoggingEvents.HttpGet, "BCeId business: " + JsonConvert.SerializeObject(bceidBusiness));

            // get the contact record.
            DvContact userContact = null;

            // see if the contact exists.
            try
            {
                userContact = await _dataverse.GetContactByExternalIdAsync(contactSiteminderGuid);
                if (userContact != null)
                {
                    createContact = false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting contact by Siteminder Guid.");
                throw new Exception("Error getting contact by Siteminder Guid");
            }

            if (userContact == null)
            {
                // create the user contact record.
                userContact = new DvContact();
                string sanitizedContactSiteminderId = GuidUtility.SanitizeGuidString(contactSiteminderGuid);
                userContact.adoxio_ExternalID = sanitizedContactSiteminderId;
                userContact.NickName = userSettings.UserDisplayName;
                if (Guid.TryParse(userSettings.UserId, out tryParseOutGuid)) // BCeid id goes here
                {
                    userContact.EmployeeId = userSettings.UserId;
                }
                else // Store the BC service card id here
                {
                    userContact.ExternalUserIdentifier = userSettings.UserId;
                }

                if (bceidBusiness != null)
                {
                    userContact.FirstName = bceidBusiness.individualFirstname;
                    userContact.MiddleName = bceidBusiness.individualMiddlename;
                    userContact.LastName = bceidBusiness.individualSurname;
                    userContact.EMailAddress1 = bceidBusiness.contactEmail;
                    userContact.Telephone1 = bceidBusiness.contactPhone;
                }
                else
                {
                    Gov.Lclb.Cllb.Interfaces.BCeIDBasic bceidBasic = await _bceid.ProcessBasicQuery(userSettings.SiteMinderGuid);
                    _logger.LogDebug(LoggingEvents.Get, $"basic Info from bceid: {JsonConvert.SerializeObject(bceidBasic)}");
                    if (bceidBasic != null)
                    {
                        userContact.FirstName = bceidBasic.individualFirstname;
                        userContact.LastName = bceidBasic.individualSurname;
                    }
                }
                userContact.StatusCode = DV::Gov.Lclb.Cllb.Interfaces.contact_statuscode.Active;
            }

            // this may be an existing account, as this service is used during the account confirmation process.
            DvAccount account = await _dataverse.GetAccountByExternalIdAsync(accountSiteminderGuid);
            _logger.LogDebug(LoggingEvents.HttpGet, "Account by siteminder business guid: " + JsonConvert.SerializeObject(account));

            string accountId;
            string contactId;

            if (account == null) // create new account, legal entity, and contact
            {
                _logger.LogDebug(LoggingEvents.HttpGet, "Account is null. Creating account, legal entity, and contact.");

                // create the account
                account = new DvAccount();
                account.CopyValues(item, copyIfNull: true);
                account.adoxio_BusinessType = (DvAdoxioApplicantTypeCodes)Enum.Parse(typeof(AdoxioApplicantTypeCodes), item.businessType, true);
                string sanitizedAccountSiteminderId = GuidUtility.SanitizeGuidString(accountSiteminderGuid);
                account.adoxio_ExternalID = sanitizedAccountSiteminderId;
                account.adoxio_AccountType = DvAdoxioAccountType.Applicant;

                if (bceidBusiness != null)
                {
                    account.EMailAddress1 = bceidBusiness.contactEmail;
                    account.Telephone1 = bceidBusiness.contactPhone;
                    account.Address1_City = bceidBusiness.addressCity;
                    account.Address1_PostalCode = bceidBusiness.addressPostal;
                    account.Address1_Line1 = bceidBusiness.addressLine1;
                    account.Address1_Line2 = bceidBusiness.addressLine2;
                    account.AccountNumber = bceidBusiness.businessNumber;
                    account.adoxio_BCIncorporationNumber = bceidBusiness.incorporationNumber;
                }

                try
                {
                    var newAccountId = await _dataverse.CreateAccountAsync(account);
                    accountId = newAccountId.ToString();
                    account.Id = newAccountId;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error creating account.");
                    throw new Exception("Error creating account.");
                }

                // create legal entity linked to the new account
                var legalEntity = new DvLegalEntity
                {
                    adoxio_Account = new Microsoft.Xrm.Sdk.EntityReference(DvAccount.EntityLogicalName, account.Id),
                    adoxio_name = item.name,
                    adoxio_IsIndividual = DvAdoxioGeneralYesNo.No,
                    adoxio_IsApplicant = true,
                    adoxio_LegalEntityType = (DvAdoxioApplicantTypeCodes)Enum.Parse(typeof(AdoxioApplicantTypeCodes), item.businessType, true)
                };

                try
                {
                    await _dataverse.CreateLegalEntityAsync(legalEntity);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error creating legal entity.");
                    throw new Exception("Error creating legal entity.");
                }

                // create or get the contact
                if (userContact.Id == Guid.Empty)
                {
                    try
                    {
                        var newContactId = await _dataverse.CreateContactAsync(userContact);
                        contactId = newContactId.ToString();
                        userContact.Id = newContactId;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error creating contact for account.");
                        throw new Exception("Error creating contact for account");
                    }
                }
                else
                {
                    contactId = userContact.Id.ToString();
                }

                // link contact to account
                try
                {
                    await _dataverse.SetContactParentAccountAsync(contactId, accountId);
                    await _dataverse.SetAccountPrimaryContactAsync(accountId, contactId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error binding contact to account.");
                    throw new Exception("Error binding contact to account");
                }

                // create the SharePoint document location for the account
                var accountIdCleaned = accountId.ToUpper().Replace("-", "");
                var accountFolderName = $"{account.Name}_{accountIdCleaned}";

                await _dataverse.CreateAccountSharePointDocLocAsync(accountId, accountFolderName, accountFolderName);

                // create the folder in SharePoint
                _fileManagerClient.CreateFolder(new CreateFolderRequest
                {
                    EntityName = "account",
                    FolderName = accountFolderName
                });

                // create the singleton cannabis tied house connection record for the user account
                try
                {
                    await _tiedHouseConnectionsRepository.UpsertCannabisTiedHouseConnection(accountId, null);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Error creating Cannabis Tied house connection for account {accountId}.");
                    throw new Exception("Error creating Cannabis Tied house connection.");
                }
            }
            else // existing account, new user only
            {
                accountId = account.Id.ToString();

                if (createContact)
                {
                    _logger.LogDebug(LoggingEvents.HttpGet, "Account is NOT null. Only a new user.");
                    try
                    {
                        var newContactId = await _dataverse.CreateContactAsync(userContact);
                        contactId = newContactId.ToString();
                        userContact.Id = newContactId;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error creating contact.");
                        throw new Exception("Error creating contact");
                    }
                }
                else
                {
                    contactId = userContact.Id.ToString();
                }
            }

            _logger.LogDebug(LoggingEvents.Save, "Patching the userContact so it relates to the account.");
            try
            {
                await _dataverse.SetContactParentAccountAsync(contactId, accountId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error binding contact to account.");
                throw new Exception("Error binding contact to account");
            }

            // create the bridge entity for login.
            if (!string.IsNullOrEmpty(_configuration["FEATURE_BRIDGE_LOGIN"]))
            {
                await _dataverse.UpdateContactBridgeLoginAsync(contactId, contactSiteminderGuid, accountId, accountSiteminderGuid);
            }

            // if we have not yet authenticated, then this is the new record for the user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.AccountId = accountId;
                userSettings.ContactId = contactId;

                // we can now authenticate.
                if (userSettings.AuthenticatedUser == null)
                {
                    Models.User user = new Models.User();
                    user.Active = true;
                    user.AccountId = Guid.Parse(userSettings.AccountId);
                    user.ContactId = Guid.Parse(userSettings.ContactId);
                    user.UserType = userSettings.UserType;
                    user.SmUserId = userSettings.UserId;
                    userSettings.AuthenticatedUser = user;
                }

                userSettings.IsNewUserRegistration = false;

                // Delete the newUserClaim and add the ExistingUser claim to allow logged in user access to authorized services
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                var newUserClaim = identity.FindFirst(Permission.NewUserRegistration);
                if (newUserClaim != null)
                {
                    identity.RemoveClaim(newUserClaim);
                }

                identity.AddClaim(new Claim("permission_claim", Permission.ExistingUser));
                HttpContext.User.AddIdentity(identity);

                string userSettingsString = JsonConvert.SerializeObject(userSettings);
                _logger.LogDebug("userSettingsString --> " + userSettingsString);

                _httpContextAccessor.HttpContext.Session.SetString("UserSettings", userSettingsString);
                _logger.LogDebug("user added to session. ");
            }
            else
            {
                _logger.LogDebug(LoggingEvents.Error, "Invalid user registration.");
                throw new Exception("Invalid user registration.");
            }

            result = account.ToViewModel();
            if (account.PrimaryContactId != null)
            {
                var primaryContact = await _dataverse.GetContactByIdAsync(account.PrimaryContactId.Id.ToString());
                if (primaryContact != null)
                    result.primarycontact = primaryContact.ToViewModel();
            }

            _logger.LogDebug(LoggingEvents.HttpPost, "result: " +
                JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            return new JsonResult(result);
        }

        private async Task<string> GetAccountDataFromOrgBook()
        {
            Response12 results = await _orgBookclient.V2SearchCredentialTopicGetAsync(null, null, null, "BC1165060", Inactive3.False, Latest3.True, Revoked3.False, "registration", null, null, null);
            CredentialTopicSearch credentialTopic = results.Results.FirstOrDefault();
            // Get business name
            var businessName = credentialTopic.Topic.Names.FirstOrDefault()?.Text;
            // Get business type
            var businessType = credentialTopic.Topic.Attributes.Where(a => a.Type == "entity_type").FirstOrDefault()?.Value;
            // Get incorporation date
            var incorporationDate = credentialTopic.Topic.Attributes.Where(a => a.Type == "entity_status_effective").FirstOrDefault()?.Value;
            return businessType;
        }

        /// <summary>
        /// Update a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "Business-User")]
        public async Task<IActionResult> UpdateAccount([FromBody] ViewModels.Account item, string id)
        {
            _logger.LogDebug(LoggingEvents.HttpPut, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            _logger.LogDebug(LoggingEvents.HttpPut, "Account parameter: " + JsonConvert.SerializeObject(item));
            _logger.LogDebug(LoggingEvents.HttpPut, "id parameter: " + id);

            if (id != item.id)
            {
                _logger.LogWarning(LoggingEvents.BadRequest, "Bad Request. Id doesn't match the account id.");
                return BadRequest();
            }

            Guid accountId = new Guid(id);

            if (!await DynamicsExtensions.CurrentUserHasAccessToAccountAsync(accountId, _httpContextAccessor, _dataverse))
            {
                _logger.LogError(LoggingEvents.BadRequest, "Current user has NO access to the account.");
                return NotFound();
            }

            DvAccount adoxioAccount = await _dataverse.GetAccountByIdAsync(id);
            if (adoxioAccount == null)
            {
                _logger.LogWarning(LoggingEvents.NotFound, "Account NOT found.");
                return new NotFoundResult();
            }

            // Preserve T&C acceptance from existing record — the profile form does not include
            // these fields, so they arrive as null and would otherwise overwrite the accepted state.
            if (item.TermsOfUseAccepted == null)
            {
                item.TermsOfUseAccepted = adoxioAccount.adoxio_TermsofUseAccepted;
                item.TermsOfUseAcceptedDate = adoxioAccount.adoxio_TermsofUseAcceptedDate.HasValue
                    ? (DateTimeOffset?)adoxioAccount.adoxio_TermsofUseAcceptedDate.Value
                    : null;
            }

            // Preserve name and BCeID external-id link from existing record — the profile form
            // does not include these fields either, so they'd otherwise be silently nulled out
            // on every save via CopyValues' copyIfNull:true, breaking the account's login linkage.
            if (item.name == null) item.name = adoxioAccount.Name;
            if (item.externalId == null) item.externalId = adoxioAccount.adoxio_ExternalID;

            // patch - create a new entity with only the changed values
            adoxioAccount = new DvAccount();
            adoxioAccount.CopyValues(item, copyIfNull: true);
            adoxioAccount.Id = accountId;

            try
            {
                await _dataverse.UpdateAccountAsync(adoxioAccount);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating the account.");
                throw new Exception("Error updating the account.");
            }

            var updatedAccount = adoxioAccount.ToViewModel();
            updatedAccount.primarycontact = item.primarycontact;
            _logger.LogDebug(LoggingEvents.HttpPut, "updatedAccount: " +
                JsonConvert.SerializeObject(updatedAccount, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

            return new JsonResult(updatedAccount);
        }

        /// <summary>
        /// Delete an account.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        [Authorize(Policy = "Business-User")]
        public async Task<IActionResult> DeleteDynamicsAccount(string id)
        {
            _logger.LogDebug(LoggingEvents.HttpPost, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);

            // verify the currently logged in user has access to this account
            Guid accountId = new Guid(id);
            if (!await DynamicsExtensions.CurrentUserHasAccessToAccountAsync(accountId, _httpContextAccessor, _dataverse))
            {
                _logger.LogWarning(LoggingEvents.NotFound, "Current user has NO access to the account.");
                return new NotFoundResult();
            }

            // verify the account exists
            DvAccount account = await _dataverse.GetAccountByIdAsync(id);
            if (account == null)
            {
                _logger.LogWarning(LoggingEvents.NotFound, "Account NOT found.");
                return new NotFoundResult();
            }

            // delete legal entities
            var legalEntities = await _dataverse.GetLegalEntitiesByAccountIdAsync(id);
            foreach (var le in legalEntities)
            {
                try
                {
                    await _dataverse.DeleteLegalEntityAsync(le.Id.ToString());
                    _logger.LogDebug(LoggingEvents.HttpDelete, "Legal Entity deleted: " + le.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error deleting the Legal Entity");
                    throw new Exception("Error deleting the Legal Entity");
                }
            }

            // delete establishments
            var establishments = await _dataverse.GetEstablishmentsByAccountIdAsync(id);
            foreach (var establishment in establishments)
            {
                try
                {
                    await _dataverse.DeleteEstablishmentAsync(establishment.Id.ToString());
                    _logger.LogDebug(LoggingEvents.HttpDelete, "Establishment deleted: " + establishment.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error deleting the Establishment");
                    throw new Exception("Error deleting the Establishment");
                }
            }

            // delete changelogs
            var changelogIds = await _dataverse.GetLicenseeChangelogIdsByAccountIdAsync(id);
            foreach (var changelogId in changelogIds)
            {
                try
                {
                    await _dataverse.DeleteByLogicalNameAsync("adoxio_licenseechangelog", changelogId);
                    _logger.LogDebug(LoggingEvents.HttpDelete, "Changelog deleted: " + changelogId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error deleting the Changelog");
                    throw new Exception("Error deleting the Changelog");
                }
            }

            // delete licences
            var licences = await _dataverse.GetLicencesByAccountIdAsync(id);
            foreach (var licence in licences)
            {
                try
                {
                    await _dataverse.DeleteLicenceAsync(licence.Id.ToString());
                    _logger.LogDebug(LoggingEvents.HttpDelete, "Licence deleted: " + licence.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error deleting the Licence");
                    throw new Exception("Error deleting the Licence");
                }
            }

            // delete contacts
            var contacts = await _dataverse.GetContactsByAccountIdAsync(id);
            foreach (var contact in contacts)
            {
                try
                {
                    await _dataverse.DeleteContactAsync(contact.Id.ToString());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error deleting the Contact");
                    throw new Exception("Error deleting the Contact");
                }
            }

            // delete applications and their invoices
            var applications = await _dataverse.GetApplicationsByAccountIdAsync(id);
            foreach (var application in applications)
            {
                try
                {
                    var fullApplication = await _dataverse.GetApplicationByIdWithChildrenAsync(application.Id.ToString());
                    if (fullApplication?.adoxio_Invoice != null)
                    {
                        await _dataverse.DeleteInvoiceAsync(fullApplication.adoxio_Invoice.Id.ToString());
                    }
                    await _dataverse.DeleteApplicationAsync(application.Id.ToString());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error deleting the application");
                    throw new Exception("Error deleting the application");
                }
            }

            // delete tied house connections
            var tiedHouseConnections = await _dataverse.GetTiedHouseConnectionsByAccountIdAsync(id);
            foreach (var connection in tiedHouseConnections)
            {
                try
                {
                    await _tiedHouseConnectionsRepository.DeleteTiedHouseConnectionById(connection.Id.ToString());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error deleting the Tied house connection");
                    throw new Exception("Error deleting the Tied house connection");
                }
            }

            // delete SharePoint document locations
            var docLocations = await _dataverse.GetSharePointDocLocsByObjectIdAsync(id);
            foreach (var docLoc in docLocations)
            {
                try
                {
                    await _dataverse.DeleteSharePointDocLocAsync(docLoc.Id.ToString());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error deleting the SharePoint Document Locations");
                    throw new Exception("Error deleting the SharePoint Document Locations");
                }
            }

            // delete the account
            try
            {
                await _dataverse.DeleteAccountAsync(id);
                _logger.LogDebug(LoggingEvents.HttpDelete, "Account deleted: " + accountId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting the account");
                throw new Exception("Error deleting the account");
            }

            return Ok("OK");
        }


        [HttpGet("delete/current")]
        [Authorize(Policy = "Business-User")]
        public async Task<IActionResult> DeleteCurrentAccount()
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");

            _logger.LogDebug(LoggingEvents.HttpGet, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);

            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            _logger.LogDebug(LoggingEvents.HttpGet, "UserSettings: " + JsonConvert.SerializeObject(userSettings));

            // query the Dynamics system to get the account record.
            if (userSettings.AccountId != null)
            {
                try
                {
                    await _dataverse.ExecuteWorkflowAsync("df4e4623-a2f5-4e9f-a305-d8a578d1c49f", userSettings.AccountId);
                    return Ok("OK");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error executing delete account workflow.");
                    return StatusCode(500, $"ERROR executing workflow. {e.Message}");
                }
            }

            return Ok("OK");
        }

        /// <summary>
        /// Returns a summary of the current user's account. This contains basic high level information about the
        /// account and all licences they have. This is useful for conditional logic that depends on a user having
        /// or not having certain licences.
        /// <remarks>
        /// This controller could be expanded to include other high level information, as needed.
        /// </remarks>
        /// </summary>
        /// <returns></returns>
        [HttpGet("current/summary")]
        [Authorize(Policy = "Business-User")]
        public async Task<ActionResult<AccountSummary>> AccountSummary()
        {
            _logger.LogDebug("getAccountSummary");

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            if (userSettings.AccountId == null)
            {
                _logger.LogError(LoggingEvents.NotFound, "No account found for the user.");
                return NotFound("No account found for the user.");
            }

            var allLicences = await _dataverse.GetLicencesByAccountIdAsync(userSettings.AccountId);
            var activeLicences = allLicences.Where(l => l.statecode == DV::Gov.Lclb.Cllb.Interfaces.adoxio_licences_statecode.Active).ToList();

            // fetch unique licence types
            var licenceTypeIds = activeLicences
                .Where(l => l.adoxio_LicenceType != null)
                .Select(l => l.adoxio_LicenceType.Id.ToString())
                .Distinct()
                .ToList();

            var licenceTypeCache = new Dictionary<string, DV::Gov.Lclb.Cllb.Interfaces.adoxio_licencetype>();
            foreach (var typeId in licenceTypeIds)
            {
                var lt = await _dataverse.GetLicenceTypeByIdAsync(typeId);
                if (lt != null)
                    licenceTypeCache[typeId] = lt;
            }

            var accountSummary = new AccountSummary
            {
                accountId = userSettings.AccountId,
                licences = activeLicences.Select(item =>
                {
                    DV::Gov.Lclb.Cllb.Interfaces.adoxio_licencetype licenceType = null;
                    if (item.adoxio_LicenceType != null)
                        licenceTypeCache.TryGetValue(item.adoxio_LicenceType.Id.ToString(), out licenceType);
                    return new AccountSummaryLicence
                    {
                        licenceId = item.adoxio_licencesId?.ToString(),
                        licenceType = licenceType?.adoxio_name,
                        licenceTypeCategory = licenceType?.adoxio_Category != null
                            ? (LicenceTypeCategory)(int)licenceType.adoxio_Category
                            : (LicenceTypeCategory?)null,
                        expiryDate = item.adoxio_ExpiryDate.HasValue ? (DateTimeOffset?)item.adoxio_ExpiryDate.Value : null,
                        statusCode = (int?)item.statuscode
                    };
                }).ToList(),
                applications = new List<AccountSummaryApplications>()
            };

            return Ok(accountSummary);
        }
    }
}
