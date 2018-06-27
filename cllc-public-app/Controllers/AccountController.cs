using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Client;
using Newtonsoft.Json;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Logging;
using Gov.Lclb.Cllb.Interfaces.Models;
using System.Net;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Interfaces.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BCeIDBusinessQuery _bceid;
		private readonly ILogger _logger;        

		public AccountController(Interfaces.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor, BCeIDBusinessQuery bceid, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = null; //distributedCache;                        
            this._httpContextAccessor = httpContextAccessor;
			this._bceid = bceid;
            this._dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(AccountController));                    
        }

		/// GET account in Dynamics for the current user
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAccount()
        {
            ViewModels.Account result = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // query the Dynamics system to get the account record.
			if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                var accountId = Guid.Parse(userSettings.AccountId);
                MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountById(accountId);
                if (account == null)
                {
                    return new NotFoundResult();
                }
                				
                result = account.ToViewModel();
            }
			else
			{
				return new NotFoundResult();
			}

            return Json(result);
        }

        /// GET account in Dynamics for the current user
        [HttpGet("bceid")]
        public async Task<IActionResult> GetCurrentBCeIDBusiness()
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // query the BCeID API to get the business record.
			var business = await _bceid.ProcessBusinessQuery("44437132CF6B4E919FE6FBFC5594FC44");
			//var business = await _bceid.ProcessBusinessQuery(userSettings.SiteMinderGuid);

            if (business == null)
            {
                return new NotFoundResult();
            }

            return Json(business);
        }

        /// <summary>
        /// Get a specific legal entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
		public async Task<IActionResult> GetAccount(string id)
		{
			ViewModels.Account result = null;

			// query the Dynamics system to get the account record.
			if (id != null)
			{
				// verify the currently logged in user has access to this account
                Guid accountId = new Guid(id);
                if (!CurrentUserHasAccessToAccount(accountId))
                {
                    return new NotFoundResult();
                }

                MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountById(accountId);
                if (account == null)
                {
                    return new NotFoundResult();
                }
                result = account.ToViewModel();
			}
			else
			{
				return BadRequest();
			}

            return Json(result);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateDynamicsAccount([FromBody] ViewModels.Account item)
        {    

            ViewModels.Account result = null;
            Boolean updateIfNull = true;

            // get UserSettings from the session
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            
			// get account Siteminder GUID
			string accountSiteminderGuid = userSettings.SiteMinderBusinessGuid;
			if (accountSiteminderGuid == null || accountSiteminderGuid.Length == 0)
				throw new Exception("Oops no accountSiteminderGuid exernal id");

			// first check to see that a contact exists.
			string contactSiteminderGuid = userSettings.SiteMinderGuid;
			if (contactSiteminderGuid == null || contactSiteminderGuid.Length == 0)
				throw new Exception("Oops no ContactSiteminderGuid exernal id");

            // get BCeID record for the current user
			var bceidBusiness = await _bceid.ProcessBusinessQuery("44437132CF6B4E919FE6FBFC5594FC44");
			//var bceidBusiness = await _bceid.ProcessBusinessQuery(userSettings.SiteMinderGuid);

            // get the contact record.
            MicrosoftDynamicsCRMcontact userContact = null;

            // see if the contact exists.
            userContact = await _dynamicsClient.GetContactBySiteminderGuid(contactSiteminderGuid);            
            
            if (userContact == null)
            {
                // create the user contact record.
                userContact = new MicrosoftDynamicsCRMcontact();
                // Adoxio_externalid is where we will store the guid from siteminder.
                userContact.AdoxioExternalid = contactSiteminderGuid;
                userContact.Fullname = userSettings.UserDisplayName;
                userContact.Nickname = userSettings.UserDisplayName;
                userContact.Employeeid = userSettings.UserId;

				if (bceidBusiness != null)
				{
					// set contact according to item
                    userContact.Firstname = item.primarycontact.firstname;
                    userContact.Lastname = item.primarycontact.lastname;
                    userContact.Emailaddress1 = item.primarycontact.emailaddress1;
                    userContact.Telephone1 = item.primarycontact.telephone1;
                    userContact.Address1City = item.primarycontact.address1_city;
                    userContact.Address1Postalcode = item.primarycontact.address1_postalcode;
                    userContact.Address1Addressid = item.primarycontact.address1_line1;
                    userContact.Address1Postalcode = item.primarycontact.address1_postalcode;
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
            
            if (account == null) // do a deep create.  create 3 objects at once.
            {
                // create a new account
                account = new MicrosoftDynamicsCRMaccount();
                account.CopyValues(item, updateIfNull);
                // ensure that we create an account for the current user.				
				account.AdoxioExternalid = accountSiteminderGuid;

                account.Primarycontactid = userContact;
                account.AdoxioAccounttype = (int)Adoxio_accounttypecodes.Applicant;

                if (bceidBusiness != null)
				{
                    // sets Business type with numerical value found in Adoxio_applicanttypecodes
                    // using account.businessType which is set in bceid-confirmation.component.ts
                    account.AdoxioBusinesstype = (int)Enum.Parse(typeof(Adoxio_applicanttypecodes), item.businessType, true);
				}
				else
				{
                    account.AdoxioBusinesstype = (int)Adoxio_applicanttypecodes.PublicCorporation;
                }

                var legalEntity = new MicrosoftDynamicsCRMadoxioLegalentity()
                {
                    AdoxioAccount = account,
                    AdoxioName = item.name,
                    AdoxioIsindividual = 0,
                    AdoxioIsapplicant = true
                };

                string legalEntityString = JsonConvert.SerializeObject(legalEntity);
                _logger.LogError("Legal Entity Before --> " + legalEntityString);
                
                legalEntity = await _dynamicsClient.Adoxiolegalentities.CreateAsync(legalEntity);

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
                _logger.LogError("Legal Entity After --> " + legalEntityString);

            }
            else // it is a new user only.
            {                
                userContact = await _dynamicsClient.Contacts.CreateAsync(userContact);
            }

            // always patch the userContact so it relates to the account.
    
            // parent customer id relationship will be created using the method here:
            //https://msdn.microsoft.com/en-us/library/mt607875.aspx
            MicrosoftDynamicsCRMcontact patchUserContact = new MicrosoftDynamicsCRMcontact();
            patchUserContact.ParentCustomerIdAccountODataBind = _dynamicsClient.GetEntityURI("accounts", account.Accountid);
            await _dynamicsClient.Contacts.UpdateAsync(userContact.Contactid, patchUserContact);

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
                    user.SmUserId = userSettings.UserId;
                    userSettings.AuthenticatedUser = user;
                }

                userSettings.IsNewUserRegistration = false;

                string userSettingsString = JsonConvert.SerializeObject(userSettings);
				_logger.LogError("AccountController --> " + userSettingsString);

				// add the user to the session.
                _httpContextAccessor.HttpContext.Session.SetString("UserSettings", userSettingsString);
            }
			else
			{
				throw new Exception("Oops not a new user registration");
			}

            //account.accountId = id;
            result = account.ToViewModel();


            return Json(result);
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
            if (id != item.id)
            {
                return BadRequest();
            }

            // get the legal entity.
            Guid accountId = new Guid(id);

            MicrosoftDynamicsCRMaccount adoxioAccount = await _dynamicsClient.GetAccountById(accountId);
            if (adoxioAccount == null)
            {
                return new NotFoundResult();
            }

            // we are doing a patch, so wipe out the record.
            adoxioAccount = new MicrosoftDynamicsCRMaccount();

            // copy values over from the data provided
            adoxioAccount.CopyValues(item);

            await _dynamicsClient.Accounts.UpdateAsync(accountId.ToString(), adoxioAccount);
            return Json(adoxioAccount.ToViewModel());
        }

        /// <summary>
        /// Delete a legal entity.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteDynamicsAccount(string id)
        {
			// verify the currently logged in user has access to this account
            Guid accountId = new Guid(id);
            if (!CurrentUserHasAccessToAccount(accountId))
            {
                return new NotFoundResult();
            }

            // get the account
            MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountById(accountId);
            if (account == null)
            {
                return new NotFoundResult();
            }

			// delete the associated LegalEntity
			MicrosoftDynamicsCRMadoxioLegalentity legalentity = await _dynamicsClient.GetAdoxioLegalentityByAccountId(accountId);
			if (legalentity != null) 
			{
				_dynamicsClient.Adoxiolegalentities.Delete(legalentity.AdoxioLegalentityid);
			}

			await _dynamicsClient.Accounts.DeleteAsync(accountId.ToString());

            return NoContent(); // 204 
        }

		/// <summary>
        /// Verify whether currently logged in user has access to this account
        /// </summary>
        /// <returns>boolean</returns>
		private bool CurrentUserHasAccessToAccount(ViewModels.Account account)
		{
			return CurrentUserHasAccessToAccount(Guid.Parse(account.id));
		}

		/// <summary>
        /// Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
		private bool CurrentUserHasAccessToAccount(Guid id)
		{
			// get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // For now, check if the account id matches the user's account.
            // TODO there may be some account relationships in the future
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return Guid.Parse(userSettings.AccountId) == id;
            }

            // if current user doesn't have an account they are probably not logged in
            return false;
		}
    }
}
