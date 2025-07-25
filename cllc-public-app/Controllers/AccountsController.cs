using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
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
using Microsoft.Rest;
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

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly BCeIDBusinessQuery _bceid;
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
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
            IDynamicsClient dynamicsClient,
            FileManagerClient fileManagerClient,
            IWebHostEnvironment env,
            TiedHouseConnectionsRepository tiedHouseConnectionsRepository
        )
        {
            _configuration = configuration;
            _bceid = bceid;
            _dynamicsClient = dynamicsClient;
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
                MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountByIdAsync(new Guid(accountId));
                _logger.LogDebug(LoggingEvents.HttpGet, "Dynamics Account: " + JsonConvert.SerializeObject(account));

                if (account == null)
                {
                    // Sometimes we receive the siteminderbusienssguid instead of the account id. 
                    account = await _dynamicsClient.GetActiveAccountBySiteminderBusinessGuid(accountId);
                    if (account == null)
                    {
                        _logger.LogWarning(LoggingEvents.NotFound, "No Account Found.");
                        return new NotFoundResult();
                    }
                }
                result = account.ToViewModel();
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
                List<MicrosoftDynamicsCRMcontact> contacts = _dynamicsClient.GetActiveContactsByAccountId(userSettings.AccountId);
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
        public IActionResult GetAutocomplete(string name)
        {
            var results = new List<TransferAccount>();
            try
            {
                string filter = null;
                // escape any apostophes.
                if (name != null)
                {
                    name = name.Replace("'", "''");
                    // select active accounts that match the given name
                    filter = $"statecode eq 0 and contains(name,'{name}')";
                }
                var expand = new List<string> { "primarycontactid" };
                var accounts = _dynamicsClient.Accounts.Get(filter: filter, expand: expand, top: 10).Value;
                foreach (var account in accounts)
                {
                    var transferAccount = new TransferAccount
                    {
                        AccountId = account.Accountid,
                        AccountName = account.Name,
                        BusinessType = (AdoxioApplicantTypeCodes?)account.AdoxioBusinesstype

                    };
                    if (account.Primarycontactid != null)
                    {

                        transferAccount.ContactName = $"{account.Primarycontactid.Firstname} {account.Primarycontactid.Lastname}";
                    }
                    results.Add(transferAccount);
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error while getting autocomplete data.");
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
                    userAccessToAccount = DynamicsExtensions.CurrentUserHasAccessToAccount(accountId, _httpContextAccessor, _dynamicsClient);
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.LogError(httpOperationException, "Error while checking if current user has access to account.");
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

                MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountByIdAsync(accountId);
                if (account == null)
                {
                    _logger.LogWarning(LoggingEvents.NotFound, "Account NOT found.");
                    return new NotFoundResult();
                }
                result = account.ToViewModel();
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
        public IActionResult GetBusinessProfile(string accountId)
        {
            _logger.LogDebug(LoggingEvents.HttpGet, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            _logger.LogDebug(LoggingEvents.HttpGet, "accountId: {accountId}");

            List<BusinessProfileLegalEntity> legalEntities;

            var expand = new List<string> { "primarycontactid" };
            var account = (_dynamicsClient.Accounts.Get(filter: "", expand: expand).Value.FirstOrDefault()).ToViewModel();
            _logger.LogDebug(LoggingEvents.HttpGet, "Account details: " + JsonConvert.SerializeObject(account));

            // get legal entities
            var entityFilter = $"_adoxio_account_value eq {accountId}";
            var expandList = new List<string> { "adoxio_ShareholderAccountID" };
            try
            {
                legalEntities = _dynamicsClient.Legalentities.Get(filter: entityFilter, expand: expandList).Value
                        .Select(le =>
                        {
                            var legalEntity = le.ToViewModel();
                            var entity = new BusinessProfileLegalEntity
                            {
                                AdoxioLegalEntity = legalEntity,
                                Account = le.AdoxioShareholderAccountID == null ? account : le.AdoxioShareholderAccountID.ToViewModel(),
                            };
                            entity.corporateDetailsFilesExists = FileUploadExists(entity.Account.id, entity.Account.name, "Corporate Information").Result;
                            entity.organizationStructureFilesExists = FileUploadExists(entity.Account.id, entity.Account.name, "Organization Structure").Result;
                            entity.keyPersonnelFilesExists = FileUploadExists(entity.Account.id, entity.Account.name, "Key Personnel").Result;
                            entity.financialInformationFilesExists = FileUploadExists(entity.Account.id, entity.Account.name, "Financial Information").Result;
                            entity.shareholderFilesExists = FileUploadExists(entity.Account.id, entity.Account.name, "Central Securities Register").Result;
                            var cannabisTiedHouseConnection = _tiedHouseConnectionsRepository.GetCannabisTiedHouseConnectionForUser(entity.Account.id);
                            if (cannabisTiedHouseConnection != null)
                            {
                                entity.TiedHouse = cannabisTiedHouseConnection;
                            }
                            entity.ChildEntities = GetLegalEntityChildren(entity.AdoxioLegalEntity.id);
                            return entity;
                        })
                        .ToList();
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting legal entities for the account {accountId}. ");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting legal entities for the account");
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

        private List<BusinessProfileLegalEntity> GetLegalEntityChildren(string parentLegalEntityId)
        {
            _logger.LogDebug(LoggingEvents.Get, "Begin method " + GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            _logger.LogDebug(LoggingEvents.Get, "parentLegalEntityId: {accouparentLegalEntityIdntId}");

            List<BusinessProfileLegalEntity> children = null;
            var childEntitiesFilter = $"_adoxio_legalentityowned_value eq {parentLegalEntityId}";
            var expandList = new List<string> { "adoxio_ShareholderAccountID", "adoxio_Account" };

            try
            {
                children = _dynamicsClient.Legalentities
                        .Get(filter: childEntitiesFilter, expand: expandList).Value
                        .Select(le =>
                        {
                            var legalEntity = le.ToViewModel();
                            var entity = new BusinessProfileLegalEntity
                            {
                                AdoxioLegalEntity = legalEntity,
                                Account = le.AdoxioShareholderAccountID == null ? le.AdoxioAccount.ToViewModel() : le.AdoxioShareholderAccountID.ToViewModel()
                            };
                            var cannabisTiedHouseConnection = _tiedHouseConnectionsRepository.GetCannabisTiedHouseConnectionForUser(entity.Account.id);
                            if (cannabisTiedHouseConnection != null)
                            {
                                entity.TiedHouse = cannabisTiedHouseConnection;
                            }
                            if (entity.AdoxioLegalEntity.isShareholder == true && entity.AdoxioLegalEntity.isindividual == false)
                            {
                                entity.ChildEntities = GetLegalEntityChildren(entity.AdoxioLegalEntity.id);
                            }
                            return entity;
                        })
                        .ToList();
                _logger.LogDebug(LoggingEvents.Get, "LegalEntityChildren: " +
                JsonConvert.SerializeObject(children, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, $"Error getting legal entity children for parentLegalEntityId {parentLegalEntityId}. ");
                children = null;
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

            var fileDetailsList = _fileManagerClient.GetFileDetailsListInFolder(_logger, AccountDocumentUrlTitle, accountId, folderName);
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
            bool updateIfNull = true;
            Guid tryParseOutGuid;

            bool createContact = true;
            bool mustCreateContactToAccountLink = false;

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
            MicrosoftDynamicsCRMcontact userContact = null;

            // see if the contact exists.
            try
            {
                userContact = _dynamicsClient.GetActiveContactByExternalId(contactSiteminderGuid);
                if (userContact != null)
                {
                    createContact = false;
                }

            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting contact by Siteminder Guid. ");
                throw new Exception("Error getting contact by Siteminder Guid");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting contact by Siteminder Guid.");
                throw new Exception("Error getting contact by Siteminder Guid");
            }

            if (userContact == null)
            {
                // create the user contact record.
                userContact = new MicrosoftDynamicsCRMcontact();
                // Adoxio_externalid is where we will store the guid from siteminder.
                string sanitizedContactSiteminderId = GuidUtility.SanitizeGuidString(contactSiteminderGuid);
                userContact.AdoxioExternalid = sanitizedContactSiteminderId;
                userContact.Fullname = userSettings.UserDisplayName;
                userContact.Nickname = userSettings.UserDisplayName;
                if (Guid.TryParse(userSettings.UserId, out tryParseOutGuid)) // BCeid id goes here
                {
                    userContact.Employeeid = userSettings.UserId;
                }
                else // Store the BC service card id here
                {
                    userContact.Externaluseridentifier = userSettings.UserId;
                }

                if (bceidBusiness != null)
                {
                    // set contact according to item
                    userContact.Firstname = bceidBusiness.individualFirstname;
                    userContact.Middlename = bceidBusiness.individualMiddlename;
                    userContact.Lastname = bceidBusiness.individualSurname;
                    userContact.Emailaddress1 = bceidBusiness.contactEmail;
                    userContact.Telephone1 = bceidBusiness.contactPhone;
                }
                else
                {
                    //LCSD-6488: Change to BCEID Web Query
                    Gov.Lclb.Cllb.Interfaces.BCeIDBasic bceidBasic = await _bceid.ProcessBasicQuery(userSettings.SiteMinderGuid);
                    _logger.LogDebug(LoggingEvents.Get, $"basic Info from bceid: {JsonConvert.SerializeObject(bceidBasic)}");
                    if(bceidBasic != null)
                    {
                        userContact.Firstname = bceidBasic.individualFirstname;
                        userContact.Lastname = bceidBasic.individualSurname;
                    }
                }
                userContact.Statuscode = 1;
            }
            // this may be an existing account, as this service is used during the account confirmation process.
            MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetActiveAccountBySiteminderBusinessGuid(accountSiteminderGuid);
            _logger.LogDebug(LoggingEvents.HttpGet, "Account by siteminder business guid: " + JsonConvert.SerializeObject(account));

            if (account == null) // do a deep create.  create 2 objects at once.
            {
                _logger.LogDebug(LoggingEvents.HttpGet, "Account is null. Do a deep create of 3 objects at once.");
                // create a new account
                account = new MicrosoftDynamicsCRMaccount();
                account.CopyValues(item, updateIfNull);
                // business type must be set only during creation, not in update (removed from copyValues() )
                account.AdoxioBusinesstype = (int)Enum.Parse(typeof(AdoxioApplicantTypeCodes), item.businessType, true);
                // ensure that we create an account for the current user.

                // by convention we strip out any dashes present in the guid, and force it to uppercase.
                string sanitizedAccountSiteminderId = GuidUtility.SanitizeGuidString(accountSiteminderGuid);

                account.AdoxioExternalid = sanitizedAccountSiteminderId;
                // 12/8/2020 - GW - Remove the primary contact element as that will cause the create to fail.

                mustCreateContactToAccountLink = true;

                account.AdoxioAccounttype = (int)AdoxioAccountTypeCodes.Applicant;

                if (bceidBusiness != null)
                {
                    account.Emailaddress1 = bceidBusiness.contactEmail;
                    account.Telephone1 = bceidBusiness.contactPhone;
                    account.Address1City = bceidBusiness.addressCity;
                    account.Address1Postalcode = bceidBusiness.addressPostal;
                    account.Address1Line1 = bceidBusiness.addressLine1;
                    account.Address1Line2 = bceidBusiness.addressLine2;
                    account.Address1Postalcode = bceidBusiness.addressPostal;

                    // 7-29-19 - Enable BN9 collection.
                    account.Accountnumber = bceidBusiness.businessNumber;
                    // 7-29-19 - We are not currently collecting the incorporation number
                    account.AdoxioBcincorporationnumber = bceidBusiness.incorporationNumber;
                }

                // sets Business type with numerical value found in Adoxio_applicanttypecodes
                // using account.businessType which is set in bceid-confirmation.component.ts
                account.AdoxioBusinesstype = (int)Enum.Parse(typeof(AdoxioApplicantTypeCodes), item.businessType, true);

                var legalEntity = new MicrosoftDynamicsCRMadoxioLegalentity
                {
                    AdoxioAccount = account,
                    AdoxioName = item.name,
                    AdoxioIsindividual = 0,
                    AdoxioIsapplicant = true,
                    AdoxioLegalentitytype = account.AdoxioBusinesstype
                };

                string legalEntityString = JsonConvert.SerializeObject(legalEntity);
                _logger.LogDebug("Legal Entity before creation in dynamics --> " + legalEntityString);

                try
                {
                    legalEntity = await _dynamicsClient.Legalentities.CreateAsync(legalEntity);
                }
                catch (HttpOperationException httpOperationException)
                {
                    string legalEntityId = _dynamicsClient.GetCreatedRecord(httpOperationException, null);
                    if (!string.IsNullOrEmpty(legalEntityId) && Guid.TryParse(legalEntityId, out Guid legalEntityGuid))
                    {
                        legalEntity = await _dynamicsClient.GetLegalEntityById(legalEntityGuid);
                    }
                    else
                    {
                        _logger.LogError(httpOperationException, "Error creating legal entity. ");
                        throw new HttpOperationException("Error creating legal entitiy");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error creating legal entity.");
                    throw new Exception("Error creating legal entity.");
                }

                account.Accountid = legalEntity._adoxioAccountValue;

                if (string.IsNullOrEmpty(userContact.Contactid))
                {
                    // create the contact.
                    try
                    {
                        var createdContact = _dynamicsClient.Contacts.Create(userContact);
                        userContact.Contactid = createdContact.Contactid;
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error creating contact for account");
                        throw new HttpOperationException("Error creating contact for account");
                    }

                }

                // create the account primary contact relationship.
                if (mustCreateContactToAccountLink)
                {
                    var patchContact = new MicrosoftDynamicsCRMcontact()
                    {
                        ParentCustomerIdAccountODataBind = _dynamicsClient.GetEntityURI("accounts", account.Accountid),
                        
                };
                    try
                    {
                        _dynamicsClient.Contacts.Update(userContact.Contactid, patchContact);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error setting primary contact link for contact");
                            throw new HttpOperationException("Error setting primary contact link for contact");
                        
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error binding account to contact");
                        throw new Exception("Error binding account to contact");
                    }

                    Odataid contactId = new Odataid()
                    { 
                        OdataidProperty = _dynamicsClient.GetEntityURI("contacts", userContact.Contactid)
                    };
                    try
                    {
                        _dynamicsClient.Accounts.AddReferenceWithHttpMessagesAsync(account.Accountid, "contact_customer_accounts", contactId);
                        _dynamicsClient.Accounts.AddReferenceWithHttpMessagesAsync(account.Accountid, "primarycontactid", contactId);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error setting primary contact link for account");
                        throw new HttpOperationException("Error setting primary contact link for account");

                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error binding account to contact");
                        throw new Exception("Error binding account to contact");
                    }

                }

                // create the sharepoint document location for the account

                var accountFolderName = await _dynamicsClient.GetFolderName("account", account.Accountid).ConfigureAwait(true);

                // create the folder for the account

                _dynamicsClient.CreateEntitySharePointDocumentLocation("account", account.Accountid, accountFolderName, accountFolderName);

                // fetch the account and get the created contact.
                if (legalEntity.AdoxioAccount == null)
                {
                    legalEntity.AdoxioAccount = await _dynamicsClient.GetAccountByIdAsync(Guid.Parse(account.Accountid));
                }

 

                legalEntityString = JsonConvert.SerializeObject(legalEntity);
                _logger.LogDebug("Legal Entity after creation in dynamics --> " + legalEntityString);

                try
                {
                    // Create the singletone cannabis tied house connection record for the user account
                    await _tiedHouseConnectionsRepository.CreateCannabisTiedHouseConnection(account.Accountid, null);
                }
                catch (HttpOperationException httpOperationException)
                {
                    string tiedHouseId = _dynamicsClient.GetCreatedRecord(httpOperationException, null);
                    if (string.IsNullOrEmpty(tiedHouseId))
                    {
                        _logger.LogError(httpOperationException, $"Error creating Cannabis Tied house connection for account {account.Accountid}.");
                        throw new HttpOperationException("Error creating Cannabis Tied house connection.");
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Error creating Cannabis Tied house connection for account {account.Accountid}.");
                    throw new Exception("Error creating Cannabis Tied house connection.");
                }

                // call the web service
                var createFolderRequest = new CreateFolderRequest
                {
                    EntityName = "account",
                    FolderName = accountFolderName
                };

                _fileManagerClient.CreateFolder(createFolderRequest);

            }
            else // it is a new user only.
            {
                if (createContact)
                {
                    _logger.LogDebug(LoggingEvents.HttpGet, "Account is NOT null. Only a new user.");
                    try
                    {
                        userContact = await _dynamicsClient.Contacts.CreateAsync(userContact);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        string contactId = _dynamicsClient.GetCreatedRecord(httpOperationException, null);
                        if (!string.IsNullOrEmpty(contactId) && Guid.TryParse(contactId, out Guid contactGuid))
                        {
                            userContact = await _dynamicsClient.GetContactById(contactGuid);
                        }
                        else
                        {
                            _logger.LogError(httpOperationException, "Error creating contact. ");
                            throw new Exception("Error creating contact");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error creating contact.");
                        throw new Exception("Error creating contact");
                    }
                }
            }

            
            _logger.LogDebug(LoggingEvents.Save, "Patching the userContact so it relates to the account.");
            // parent customer id relationship will be created using the method here:
            //https://msdn.microsoft.com/en-us/library/mt607875.aspx
            MicrosoftDynamicsCRMcontact patchUserContact = new MicrosoftDynamicsCRMcontact();
            patchUserContact.ParentCustomerIdAccountODataBind = _dynamicsClient.GetEntityURI("accounts", account.Accountid);
            try
            {
                await _dynamicsClient.Contacts.UpdateAsync(userContact.Contactid, patchUserContact);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error binding contact to account. ");
                throw new Exception("Error binding contact to account");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error binding contact to account");
                throw new Exception("Error binding contact to account");
            }

            // create the bridge entity for login.

            if (!string.IsNullOrEmpty(_configuration["FEATURE_BRIDGE_LOGIN"]))
            {
                _dynamicsClient.UpdateContactBridgeLogin(userContact.Contactid, contactSiteminderGuid, account.Accountid, accountSiteminderGuid);
            }

            // if we have not yet authenticated, then this is the new record for the user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.AccountId = account.Accountid;
                userSettings.ContactId = userContact.Contactid;

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

                //
                var newUserClaim = identity.FindFirst(Permission.NewUserRegistration);
                if (newUserClaim != null)
                {
                    identity.RemoveClaim(newUserClaim); // User has complete registration, remove new user permission
                }

                //Add existing user claim
                identity.AddClaim(new Claim("permission_claim", Permission.ExistingUser));
                //Add the updated identity to the HttpContext
                HttpContext.User.AddIdentity(identity);

                string userSettingsString = JsonConvert.SerializeObject(userSettings);
                _logger.LogDebug("userSettingsString --> " + userSettingsString);

                // add the user to the session.
                _httpContextAccessor.HttpContext.Session.SetString("UserSettings", userSettingsString);
                _logger.LogDebug("user added to session. ");
            }
            else
            {
                _logger.LogDebug(LoggingEvents.Error, "Invalid user registration.");
                throw new Exception("Invalid user registration.");
            }

            //account.accountId = id;
            result = account.ToViewModel();

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

            // get the legal entity.
            Guid accountId = new Guid(id);

            if (!DynamicsExtensions.CurrentUserHasAccessToAccount(accountId, _httpContextAccessor, _dynamicsClient))
            {
                _logger.LogError(LoggingEvents.BadRequest, "Current user has NO access to the account.");
                return NotFound();
            }

            MicrosoftDynamicsCRMaccount adoxioAccount = await _dynamicsClient.GetAccountByIdAsync(accountId);
            if (adoxioAccount == null)
            {
                _logger.LogWarning(LoggingEvents.NotFound, "Account NOT found.");
                return new NotFoundResult();
            }

            // we are doing a patch, so wipe out the record.
            adoxioAccount = new MicrosoftDynamicsCRMaccount();

            // copy values over from the data provided
            adoxioAccount.CopyValues(item);

            try
            {
                await _dynamicsClient.Accounts.UpdateAsync(accountId.ToString(), adoxioAccount);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating the account. ");
                throw new Exception("Error updating the account.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating the account.");
                throw new Exception("Error updating the account.");
            }

            var updatedAccount = adoxioAccount.ToViewModel();
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
            if (!DynamicsExtensions.CurrentUserHasAccessToAccount(accountId, _httpContextAccessor, _dynamicsClient))
            {
                _logger.LogWarning(LoggingEvents.NotFound, "Current user has NO access to the account.");
                return new NotFoundResult();
            }

            // get the account
            MicrosoftDynamicsCRMaccount account = _dynamicsClient.GetAccountByIdWithChildren(id);
            if (account == null)
            {
                _logger.LogWarning(LoggingEvents.NotFound, "Account NOT found.");
                return new NotFoundResult();
            }

            if (account.AdoxioAccountAdoxioLegalentityAccount != null)
            {
                foreach (var le in account.AdoxioAccountAdoxioLegalentityAccount)
                {
                    try
                    {
                        _dynamicsClient.Legalentities.Delete(le.AdoxioLegalentityid);
                        _logger.LogDebug(LoggingEvents.HttpDelete, "Legal Entity deleted: " + le.AdoxioLegalentityid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error deleting the Legal Entity: ");
                        throw new Exception("Error deleting the Legal Entity");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error deleting the Legal Entity");
                        throw new Exception("Error deleting the Legal Entity");
                    }
                }
            }

            if (account.AdoxioAccountAdoxioEstablishmentLicencee != null)
            {
                // adoxio_account_adoxio_establishment_Licencee
                foreach (var establishment in account.AdoxioAccountAdoxioEstablishmentLicencee)
                {
                    try
                    {
                        _dynamicsClient.Establishments.Delete(establishment.AdoxioEstablishmentid);
                        _logger.LogDebug(LoggingEvents.HttpDelete, "Establishment deleted: " + establishment.AdoxioEstablishmentid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error deleting the Establishment: ");
                        throw new Exception("Error deleting the Establishment");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error deleting the Establishment");
                        throw new Exception("Error deleting the Establishment");
                    }
                }

            }

            if (account.AdoxioAccountAdoxioApplicationApplicant != null)
            {
                // adoxio_account_adoxio_establishment_Licencee
                foreach (var application in account.AdoxioAccountAdoxioApplicationApplicant)
                {
                    try
                    {
                        _dynamicsClient.Applications.Delete(application.AdoxioApplicationid);
                        _logger.LogDebug(LoggingEvents.HttpDelete, "Application deleted: " + application.AdoxioApplicationid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error deleting the Application: ");
                        throw new Exception("Error deleting the Application");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error deleting the Application");
                        throw new Exception("Error deleting the Application");
                    }
                }
            }

            if (account.AdoxioLicenseechangelogParentBusinessAccount != null)
            {
                // change logs
                foreach (var changelog in account.AdoxioLicenseechangelogParentBusinessAccount)
                {
                    try
                    {
                        _dynamicsClient.Licenseechangelogs.Delete(changelog.AdoxioLicenseechangelogid);
                        _logger.LogDebug(LoggingEvents.HttpDelete, "Application deleted: " + changelog.AdoxioLicenseechangelogid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error deleting the Changelog: ");
                        throw new Exception("Error deleting the Changelog");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error deleting the Changelog");
                        throw new Exception("Error deleting the Changelog");
                    }
                }
            }

            if (account.AdoxioLicenseechangelogBusinessAccount != null)
            {
                // change logs
                foreach (var changelog in account.AdoxioLicenseechangelogBusinessAccount)
                {
                    try
                    {
                        _dynamicsClient.Licenseechangelogs.Delete(changelog.AdoxioLicenseechangelogid);
                        _logger.LogDebug(LoggingEvents.HttpDelete, "Application deleted: " + changelog.AdoxioLicenseechangelogid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error deleting the Application: ");
                        throw new Exception("Error deleting the Changelog");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error deleting the Changelog");
                        throw new Exception("Error deleting the Changelog");
                    }
                }
            }

            if (account.AdoxioLicenseechangelogShareholderBusinessAccount != null)
            {
                // change logs
                foreach (var changelog in account.AdoxioLicenseechangelogShareholderBusinessAccount)
                {
                    try
                    {
                        _dynamicsClient.Licenseechangelogs.Delete(changelog.AdoxioLicenseechangelogid);
                        _logger.LogDebug(LoggingEvents.HttpDelete, "Application deleted: " + changelog.AdoxioLicenseechangelogid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error deleting the Application: ");
                        throw new Exception("Error deleting the Application");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error deleting the Changelog");
                        throw new Exception("Error deleting the Changelog");
                    }
                }
            }


            if (account.AdoxioAccountAdoxioLicencesLicencee != null)
            {
                // delete related licences
                foreach (var license in account.AdoxioAccountAdoxioLicencesLicencee)
                {
                    try
                    {
                        _dynamicsClient.Licenseechangelogs.Delete(license.AdoxioLicencesid);
                        _logger.LogDebug(LoggingEvents.HttpDelete, "License deleted: " + license.AdoxioLicencesid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error deleting the License: ");
                        throw new Exception("Error deleting the License");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error deleting the License");
                        throw new Exception("Error deleting the License");
                    }
                }
            }

            if (account.ContactCustomerAccounts != null)
            {
                foreach (var contact in account.ContactCustomerAccounts)
                {
                    try
                    {
                        _dynamicsClient.Contacts.Delete(contact.Contactid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error deleting the Contact: ");
                        throw new Exception("Error deleting the Contact");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error deleting the Contact");
                        throw new Exception("Error deleting the Contact");
                    }
                }
            }

            // applications
            if (account.AdoxioAccountAdoxioApplicationApplicant != null)
            {
                foreach (var application in account.AdoxioAccountAdoxioApplicationApplicant)
                {
                    try
                    {
                        MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await _dynamicsClient.GetApplicationByIdWithChildren(application.AdoxioApplicationid);

                        if (adoxioApplication?.AdoxioInvoice != null)
                        {
                            _dynamicsClient.Invoices.Delete(adoxioApplication.AdoxioInvoice.Invoiceid);
                        }

                        // TODO - add any other entities that might block a delete.

                        _dynamicsClient.Applications.Delete(application.AdoxioApplicationid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error deleting the application");
                        throw new Exception("Error deleting the application");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error deleting the application");
                        throw new Exception("Error deleting the application");
                    }
                }
            }


            if (account.AdoxioAccountTiedhouseconnections != null)
            {
                foreach (var tiedHouseConnection in account.AdoxioAccountTiedhouseconnections)
                {
                    try
                    {
                        _tiedHouseConnectionsRepository.DeleteTiedHouseConnectionById(
                            tiedHouseConnection.AdoxioTiedhouseconnectionid
                        );
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error deleting the Tied house connection");
                        throw new Exception("Error deleting the Tied house connection");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error deleting the Tied house connection");
                        throw new Exception("Error deleting the Tied house connection");
                    }
                }
            }


            // delete SharePoint document locations

            if (account.AccountSharepointDocumentLocation != null)
            {
                foreach (var sharePointDocumentLocations in account.AccountSharepointDocumentLocation)
                {
                    try
                    {
                        _dynamicsClient.Sharepointdocumentlocations.Delete(sharePointDocumentLocations.Sharepointdocumentlocationid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error deleting the SharePoint Document Locations");
                        throw new Exception("Error deleting the SharePoint Document Locations");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error deleting the SharePoint Document Locations");
                        throw new Exception("Error deleting the SharePoint Document Locations");
                    }
                }
            }


            // delete the account
            try
            {
                await _dynamicsClient.Accounts.DeleteAsync(accountId.ToString());
                _logger.LogDebug(LoggingEvents.HttpDelete, "Account deleted: " + accountId);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error deleting the account: ");
                throw new Exception("Error deleting the account");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting the account");
                throw new Exception("Error deleting the account");
            }

            return Ok("OK"); // OK 
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

                // call the bpf.
                try
                {
                    // this needs to be the guid for the published workflow.
                    await _dynamicsClient.Workflows.ExecuteWorkflowWithHttpMessagesAsync("df4e4623-a2f5-4e9f-a305-d8a578d1c49f", userSettings.AccountId);
                    return Ok("OK");
                }
                catch (Exception e)
                {
                    return StatusCode(500, $"ERROR executing workflow. {e.Message}");
                    _logger.LogError(e, "Error executing delete account workflow.");
                }

            }
            
            return Ok("OK");
            
        }
    }
}
