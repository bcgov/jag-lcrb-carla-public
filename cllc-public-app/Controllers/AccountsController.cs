using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
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
using System.Reflection;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class AccountsController : ControllerBase
    {
        private readonly BCeIDBusinessQuery _bceid;
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IOrgBookClient _orgBookclient;        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public AccountsController(IConfiguration configuration,            
            IHttpContextAccessor httpContextAccessor,
            IOrgBookClient orgBookClient,
            BCeIDBusinessQuery bceid,
            ILoggerFactory loggerFactory,
            IDynamicsClient dynamicsClient)
        {
            _configuration = configuration;
            _bceid = bceid;
            _dynamicsClient = dynamicsClient;
            _orgBookclient = orgBookClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(AccountsController));
        }
        
        /// GET account in Dynamics for the current user
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAccount()
        {
            _logger.LogDebug(LoggingEvents.HttpGet, "Begin method " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            ViewModels.Account result = null;

            // get the current user.
            string sessionSettings = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(sessionSettings);
            _logger.LogDebug(LoggingEvents.HttpGet, "UserSettings: " + JsonConvert.SerializeObject(userSettings));

            // query the Dynamics system to get the account record.
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                var accountId = GuidUtility.SanitizeGuidString(userSettings.AccountId);
                MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountById(new Guid(accountId));
                _logger.LogDebug(LoggingEvents.HttpGet, "Dynamics Account: " + JsonConvert.SerializeObject(account));

                if (account == null)
                {
                    // Sometimes we receive the siteminderbusienssguid instead of the account id. 
                    account = await _dynamicsClient.GetAccountBySiteminderBusinessGuid(accountId);
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

        /// GET account in Dynamics for the current user
        [HttpGet("bceid")]
        public async Task<IActionResult> GetCurrentBCeIDBusiness()
        {
            _logger.LogDebug(LoggingEvents.HttpGet, "Begin method " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            _logger.LogDebug(LoggingEvents.HttpGet, "UserSettings: " + JsonConvert.SerializeObject(userSettings));

            // query the BCeID API to get the business record.
            var business = await _bceid.ProcessBusinessQuery(userSettings.SiteMinderGuid);

            _logger.LogDebug(LoggingEvents.Get, $"business Info from bceid: {Newtonsoft.Json.JsonConvert.SerializeObject(business)}");

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
            return new JsonResult (business);
        }

        /// <summary>
        /// Get Autocomplete data for a given name using startswith
        /// </summary>
        /// <param name="name">The name to filter by using startswith</param>
        /// <returns>Dictionary of key value pairs with accountid and name as the pairs</returns>
        [HttpGet("autocomplete")]
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
                    filter = $"contains(name,'{name}')";
                }
                var expand = new List<string> { "primarycontactid" };
                var accounts = _dynamicsClient.Accounts.Get(filter: filter, expand: expand).Value;
                foreach (var account in accounts)
                {
                    var transferAccount = new TransferAccount()
                    {
                        AccountId = account.Accountid,
                        AccountName = account.Name,
                        BusinessType = (AdoxioApplicantTypeCodes?)account.AdoxioBusinesstype

                    };
                    if(account.Primarycontactid != null)
                    {

                        transferAccount.ContactName = $"{account.Primarycontactid.Firstname} {account.Primarycontactid.Lastname}";
                    }
                    results.Add(transferAccount);
                }
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError(odee, "Error while getting autocomplete data.");                
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
        public async Task<IActionResult> GetAccount(string id)
        {
            _logger.LogDebug(LoggingEvents.HttpGet, "Begin method " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
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
                catch (OdataerrorException odee)
                {
                    _logger.LogError(odee, "Error while checking if current user has access to account.");
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

                MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountById(accountId);                
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
        public IActionResult GetBusinessProfile(string accountId)
        {
            _logger.LogDebug(LoggingEvents.HttpGet, "Begin method " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
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
                            var entity = new ViewModels.BusinessProfileLegalEntity
                            {
                                AdoxioLegalEntity = legalEntity,
                                Account = le.AdoxioShareholderAccountID == null ? account : le.AdoxioShareholderAccountID.ToViewModel(),
                            };
                            entity.corporateDetailsFilesExists = FileUploadExists(entity.Account.id, entity.Account.name, "Corporate Information").Result;
                            entity.organizationStructureFilesExists = FileUploadExists(entity.Account.id, entity.Account.name, "Organization Structure").Result;
                            entity.keyPersonnelFilesExists = FileUploadExists(entity.Account.id, entity.Account.name, "Key Personnel").Result;
                            entity.financialInformationFilesExists = FileUploadExists(entity.Account.id, entity.Account.name, "Financial Information").Result;
                            entity.shareholderFilesExists = FileUploadExists(entity.Account.id, entity.Account.name, "Central Securities Register").Result;
                            var tiedHouse = _dynamicsClient.Tiedhouseconnections
                                .Get(filter: $"_adoxio_accountid_value eq {entity.Account.id}")
                                .Value.FirstOrDefault();
                            if (tiedHouse != null)
                            {
                                entity.TiedHouse = tiedHouse.ToViewModel();
                            }
                            entity.ChildEntities = GetLegalEntityChildren(entity.AdoxioLegalEntity.id);
                            return entity;
                        })
                        .ToList();
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError(odee, "Error getting legal entities for the account {accountId}. ");
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

        private List<ViewModels.BusinessProfileLegalEntity> GetLegalEntityChildren(string parentLegalEntityId)
        {
            _logger.LogDebug(LoggingEvents.Get, "Begin method " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            _logger.LogDebug(LoggingEvents.Get, "parentLegalEntityId: {accouparentLegalEntityIdntId}");

            List<ViewModels.BusinessProfileLegalEntity> children = null;
            var childEntitiesFilter = $"_adoxio_legalentityowned_value eq {parentLegalEntityId}";
            var expandList = new List<string> { "adoxio_ShareholderAccountID", "adoxio_Account" };

            try
            {
                children = _dynamicsClient.Legalentities
                        .Get(filter: childEntitiesFilter, expand: expandList).Value
                        .Select(le =>
                        {
                            var legalEntity = le.ToViewModel();
                            var entity = new ViewModels.BusinessProfileLegalEntity
                            {
                                AdoxioLegalEntity = legalEntity,
                                Account = le.AdoxioShareholderAccountID == null ? le.AdoxioAccount.ToViewModel() : le.AdoxioShareholderAccountID.ToViewModel()
                            };
                            var tiedHouse = _dynamicsClient.Tiedhouseconnections
                                .Get(filter: $"_adoxio_accountid_value eq {entity.Account.id}")
                                .Value.FirstOrDefault();
                            if (tiedHouse != null)
                            {
                                entity.TiedHouse = tiedHouse.ToViewModel();
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
            catch (OdataerrorException odee)
            {
                _logger.LogError(odee, $"Error getting legal entity children for parentLegalEntityId {parentLegalEntityId}. ");
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
            _logger.LogDebug(LoggingEvents.Get, "Begin method " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            _logger.LogDebug(LoggingEvents.Get, "accountId: {accountId}, accountName: {accountName}, documentType: {documentType}");

            var exists = false;
            var accountIdCleaned = accountId.ToUpper().Replace("-", "");
            var folderName = $"{accountName}_{accountIdCleaned}";
            SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);
            var fileDetailsList = await _sharePointFileManager.GetFileDetailsListInFolder(SharePointFileManager.DefaultDocumentListTitle, folderName, documentType);
            if (fileDetailsList != null)
            {
                exists = fileDetailsList.Count() > 0;
            }

            _logger.LogDebug(LoggingEvents.Get, "FileUploadExists: " + exists);
            return exists;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateDynamicsAccount([FromBody] ViewModels.Account item)
        {
            _logger.LogDebug(LoggingEvents.HttpPost, "Begin method " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
            _logger.LogDebug(LoggingEvents.HttpPost, "Account parameters: " + JsonConvert.SerializeObject(item));

            ViewModels.Account result = null;
            bool updateIfNull = true;
            Guid tryParseOutGuid;

            bool createContact = true;

            // get UserSettings from the session
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
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
            _logger.LogDebug(LoggingEvents.Get, $"business Info from bceid: {Newtonsoft.Json.JsonConvert.SerializeObject(bceidBusiness)}");
            

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
                userContact = _dynamicsClient.GetContactByExternalId(contactSiteminderGuid);
                if (userContact != null)
                {
                    createContact = false;
                }

            }
            catch (OdataerrorException odee)
            {
                _logger.LogError(odee, $"Error getting contact by Siteminder Guid. ");
                throw new Exception("Error getting contact by Siteminder Guid");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting contact by Siteminder Guid.");
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
                    userContact.Firstname = userSettings.UserDisplayName.GetFirstName();
                    userContact.Lastname = userSettings.UserDisplayName.GetLastName();
                }
                userContact.Statuscode = 1;
            }
            // this may be an existing account, as this service is used during the account confirmation process.
            MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountBySiteminderBusinessGuid(accountSiteminderGuid);
            _logger.LogDebug(LoggingEvents.HttpGet, "Account by siteminder business guid: " + JsonConvert.SerializeObject(account));

            if (account == null) // do a deep create.  create 3 objects at once.
            {
                _logger.LogDebug(LoggingEvents.HttpGet, "Account is null. Do a deep create of 3 objects at once.");
                // create a new account
                account = new MicrosoftDynamicsCRMaccount();
                account.CopyValues(item, updateIfNull);
                // business type must be set only during creation, not in update (removed from copyValues() )
                account.AdoxioBusinesstype = (int)Enum.Parse(typeof(ViewModels.AdoxioApplicantTypeCodes), item.businessType, true);
                // ensure that we create an account for the current user.

                // by convention we strip out any dashes present in the guid, and force it to uppercase.
                string sanitizedAccountSiteminderId = GuidUtility.SanitizeGuidString(accountSiteminderGuid);

                account.AdoxioExternalid = sanitizedAccountSiteminderId;
                account.Primarycontactid = userContact;
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

                var legalEntity = new MicrosoftDynamicsCRMadoxioLegalentity()
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
                catch (OdataerrorException odee)
                {
                    string legalEntityId = _dynamicsClient.GetCreatedRecord(odee, null);
                    if (!string.IsNullOrEmpty(legalEntityId) && Guid.TryParse(legalEntityId, out Guid legalEntityGuid))
                    {
                        legalEntity = await _dynamicsClient.GetLegalEntityById(legalEntityGuid);
                    }
                    else
                    {
                        _logger.LogError(odee, $"Error creating legal entity. ");
                        throw new OdataerrorException("Error creating legal entitiy");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error creating legal entity.");
                    throw new Exception("Error creating legal entity.");
                }

                account.Accountid = legalEntity._adoxioAccountValue;

                // fetch the account and get the created contact.
                if (legalEntity.AdoxioAccount == null)
                {
                    legalEntity.AdoxioAccount = await _dynamicsClient.GetAccountById(Guid.Parse(account.Accountid));
                }

                if (legalEntity.AdoxioAccount.Primarycontactid == null)
                {
                    legalEntity.AdoxioAccount.Primarycontactid = await _dynamicsClient.GetContactById(Guid.Parse(legalEntity.AdoxioAccount._primarycontactidValue));
                }

                userContact.Contactid = legalEntity.AdoxioAccount._primarycontactidValue;

                legalEntityString = JsonConvert.SerializeObject(legalEntity);
                _logger.LogDebug("Legal Entity after creation in dynamics --> " + legalEntityString);

                var tiedHouse = new MicrosoftDynamicsCRMadoxioTiedhouseconnection() { };
                tiedHouse.AccountODataBind = _dynamicsClient.GetEntityURI("accounts", account.Accountid);


                try
                {
                    tiedHouse = await _dynamicsClient.Tiedhouseconnections.CreateAsync(tiedHouse);
                }
                catch (OdataerrorException odee)
                {
                    string tiedHouseId = _dynamicsClient.GetCreatedRecord(odee, null);
                    if (string.IsNullOrEmpty(tiedHouseId))
                    {
                        _logger.LogError(odee, "Error creating Tied house connection. ");
                        throw new OdataerrorException("Error creating Tied house connection.");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error creating Tied house connection.");
                }
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
                    catch (OdataerrorException odee)
                    {
                        string contactId = _dynamicsClient.GetCreatedRecord(odee, null);
                        if (!string.IsNullOrEmpty(contactId) && Guid.TryParse(contactId, out Guid contactGuid))
                        {
                            userContact = await _dynamicsClient.GetContactById(contactGuid);
                        }
                        else
                        {
                            _logger.LogError(odee, "Error creating contact. ");                            
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

            // always patch the userContact so it relates to the account.
            _logger.LogDebug(LoggingEvents.Save, "Patching the userContact so it relates to the account.");
            // parent customer id relationship will be created using the method here:
            //https://msdn.microsoft.com/en-us/library/mt607875.aspx
            MicrosoftDynamicsCRMcontact patchUserContact = new MicrosoftDynamicsCRMcontact();
            patchUserContact.ParentCustomerIdAccountODataBind = _dynamicsClient.GetEntityURI("accounts", account.Accountid);
            try
            {
                await _dynamicsClient.Contacts.UpdateAsync(userContact.Contactid, patchUserContact);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError(odee, $"Error binding contact to account. ");
                throw new Exception("Error binding contact to account");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error binding contact to account");
                throw new Exception("Error binding contact to account");
            }

            // if we have not yet authenticated, then this is the new record for the user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.AccountId = account.Accountid.ToString();
                userSettings.ContactId = userContact.Contactid.ToString();

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

        private async Task<string>  GetAccountDataFromOrgBook(){
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
        public async Task<IActionResult> UpdateDynamicsAccount([FromBody] ViewModels.Account item, string id)
        {
            _logger.LogDebug(LoggingEvents.HttpPut, "Begin method " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);
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
                _logger.LogWarning(LoggingEvents.NotFound, "Current user has NO access to the account.");
                return NotFound();
            }

            MicrosoftDynamicsCRMaccount adoxioAccount = await _dynamicsClient.GetAccountById(accountId);
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
            catch (OdataerrorException odee)
            {
                _logger.LogError(odee, "Error updating the account. ");
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
        public async Task<IActionResult> DeleteDynamicsAccount(string id)
        {
            _logger.LogDebug(LoggingEvents.HttpPost, "Begin method " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);

            // verify the currently logged in user has access to this account
            Guid accountId = new Guid(id);
            if (!DynamicsExtensions.CurrentUserHasAccessToAccount(accountId, _httpContextAccessor, _dynamicsClient))
            {
                _logger.LogWarning(LoggingEvents.NotFound, "Current user has NO access to the account.");
                return new NotFoundResult();
            }

            // get the account
            MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountById(accountId);
            if (account == null)
            {
                _logger.LogWarning(LoggingEvents.NotFound, "Account NOT found.");
                return new NotFoundResult();
            }

            // delete the associated LegalEntity
            string accountFilter = "_adoxio_account_value eq " + id.ToString();
            var legalEntities = _dynamicsClient.Legalentities.Get(filter: accountFilter).Value.ToList();
            legalEntities.ForEach(le =>
            {
                try
                {
                    _dynamicsClient.Legalentities.Delete(le.AdoxioLegalentityid);
                    _logger.LogDebug(LoggingEvents.HttpDelete, "Legal Entity deleted: " + le.AdoxioLegalentityid);
                }
                catch (OdataerrorException odee)
                {
                    _logger.LogError(odee, "Error deleting the Legal Entity: ");
                    throw new Exception("Error deleting the Legal Entity");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error deleting the Legal Entity");
                    throw new Exception("Error deleting the Legal Entity");
                }
            });

            try
            {
                await _dynamicsClient.Accounts.DeleteAsync(accountId.ToString());
                _logger.LogDebug(LoggingEvents.HttpDelete, "Account deleted: " + accountId.ToString());
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError(odee, "Error deleting the account: " );
                throw new Exception("Error deleting the account" );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting the account");
                throw new Exception("Error deleting the account");
            }

            _logger.LogDebug(LoggingEvents.HttpDelete, "No content returned.");
            return NoContent(); // 204 
        }


        [HttpPost("delete/current")]
        public async Task<IActionResult> DeleteCurrentAccount()
        {
            _logger.LogDebug(LoggingEvents.HttpGet, "Begin method " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().ReflectedType.Name);

            // get the current user.
            string sessionSettings = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(sessionSettings);
            _logger.LogDebug(LoggingEvents.HttpGet, "UserSettings: " + JsonConvert.SerializeObject(userSettings));

            // query the Dynamics system to get the account record.
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return await DeleteDynamicsAccount(userSettings.AccountId);                                
            }
            else
            {
                return NotFound();
            }
        }

        /**************
         * TIED HOUSE *
         **************/

        /// <summary>
        /// Get TiedHouseConnection by accountId
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("{accountId}/tiedhouseconnection")]
        public JsonResult GetTiedHouseConnection(string accountId)
        {
            var result = new List<ViewModels.TiedHouseConnection>();
            IEnumerable<MicrosoftDynamicsCRMadoxioTiedhouseconnection> tiedHouseConnections = null;
            String accountfilter = null;

            // set account filter
            accountfilter = "_adoxio_accountid_value eq " + accountId;
            _logger.LogDebug("Account filter = " + accountfilter);

            tiedHouseConnections = _dynamicsClient.Tiedhouseconnections.Get(filter: accountfilter).Value;

            foreach (var tiedHouse in tiedHouseConnections)
            {
                result.Add(tiedHouse.ToViewModel());
            }

            return new JsonResult(result.FirstOrDefault());
        }

        /// <summary>
        /// Add Tied House connection
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("{accountId}/tiedhouseconnection")]
        public ActionResult AddTiedHouseConnection([FromBody] ViewModels.TiedHouseConnection item, string accountId)
        {
            if (item == null)
            {
                return BadRequest();
            }

            var tiedHouse = new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

            // copy values over from the data provided
            tiedHouse.CopyValues(item);
            tiedHouse.AccountODataBind = _dynamicsClient.GetEntityURI("accounts", accountId);
            try
            {
                tiedHouse = _dynamicsClient.Tiedhouseconnections.Create(tiedHouse);
            }
            catch (OdataerrorException odee)
            {
                tiedHouse.AdoxioTiedhouseconnectionid = _dynamicsClient.GetCreatedRecord(odee, null);
                if (string.IsNullOrEmpty(tiedHouse.AdoxioTiedhouseconnectionid))
                {
                    _logger.LogError(odee, "Error creating tiedhouse connection ");
                    throw new Exception("Error creating tiedhouse connection");
                }            
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating tiedhouse connection");
                throw new Exception("Error creating tiedhouse connection");
            }

            return new JsonResult(tiedHouse.ToViewModel());
        }

    }
}
