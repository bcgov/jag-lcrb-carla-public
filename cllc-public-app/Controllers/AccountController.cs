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
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
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
				Interfaces.Microsoft.Dynamics.CRM.Account account = await _system.GetAccountById(_distributedCache, accountId);
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

				Interfaces.Microsoft.Dynamics.CRM.Account account = await _system.GetAccountById(_distributedCache, accountId);
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
            bool createLegalEntity = false;
            bool createContact = false;

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

            // get the contact record.

            MicrosoftDynamicsCRMcontact userContact = null;

            //Interfaces.Microsoft.Dynamics.CRM.Contact userContact = await _system.GetContactBySiteminderGuid(_distributedCache, contactSiteminderGuid);
            
            if (userContact == null)
            {
                // create the user contact record.
                userContact = new MicrosoftDynamicsCRMcontact();
                // Adoxio_externalid is where we will store the guid from siteminder.
                userContact.AdoxioExternalid = contactSiteminderGuid;
                userContact.Fullname = userSettings.UserDisplayName;
                userContact.Nickname = userSettings.UserDisplayName;
                userContact.Employeeid = userSettings.UserId;
                userContact.Firstname = userSettings.UserDisplayName.GetFirstName();
                userContact.Lastname = userSettings.UserDisplayName.GetLastName();
                userContact.Statuscode = 1;                
            }

            MicrosoftDynamicsCRMaccount account = null;            


            // this may be an existing account, as this service is used during the account confirmation process.
            //Interfaces.Microsoft.Dynamics.CRM.Account account = await _system.GetAccountBySiteminderBusinessGuid(_distributedCache, accountSiteminderGuid);
            if (account == null)
            {
                // create a new account
                account = new MicrosoftDynamicsCRMaccount();
                // ensure that we create an account for the current user.				
				item.externalId = accountSiteminderGuid;

                createLegalEntity = true;
            }
            else // it is an update.
            {
                // do not update fields with null values
                updateIfNull = false;
            }

            account.CopyValues(item, updateIfNull);            

            if (account._primarycontactidValue == null) // we need to add the primary contact.
            {                
                account.Primarycontactid = userContact;
                createContact = false; // the contact will be created at the same time as the account.
            }

            // create the account.
            // account is created at the same time as the legal entity.
            //account = await _dynamicsClient.Accounts.AddnewentitytoaccountsAsync(account);

            // create a legal entity.
            var legalEntity = new MicrosoftDynamicsCRMadoxioLegalentity()
            {
                AdoxioAccount = account,
                AdoxioName = item.name,
                AdoxioIsindividual = 0,
                AdoxioIsapplicant = true
            };
            legalEntity = await _dynamicsClient.Adoxiolegalentities.CreateAsync(legalEntity);

            account.Accountid = legalEntity._adoxioAccountValue;
            


            if (createContact)
            {
                // parent customer id relationship will be created using the method here:
                //https://msdn.microsoft.com/en-us/library/mt607875.aspx
                //userContact.ParentcustomeridAccount = new MicrosoftDynamicsCRMaccount() { Accountid = account.Accountid };
                userContact = await _dynamicsClient.Contacts.CreateAsync(userContact);
                
            }
            else
            {
                // fetch the account and get the created contact.
                var a = await _system.GetAccountById(null, Guid.Parse(account.Accountid));
                userContact.Contactid = a._primarycontactid_value.ToString();
            }           


            //await _dynamicsClient.Contacts.UpdateentityincontactsAsync(userContact.Contactid.ToString(), userContact);

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

            //account.Accountid = id;
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
            if (id == null || id != item.id)
            {
                return BadRequest();
            }

			// verify the currently logged in user has access to this account
			Guid accountId = new Guid(id);
            if (!CurrentUserHasAccessToAccount(accountId))
            {
                return new NotFoundResult();
            }

			DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Account> AccountCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Account>(_system);

            // get the legal entity.
            Interfaces.Microsoft.Dynamics.CRM.Account account = await _system.GetAccountById(_distributedCache, accountId);
            _system.UpdateObject(account);
            // copy values over from the data provided (only when value is not null)
            account.CopyValues(item, false);

            

            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

			DataServiceResponse dsr = _system.SaveChangesSynchronous(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithIndependentOperations);
            foreach (OperationResponse result in dsr)
            {
                if (result.StatusCode == 500) // error
                {
                    return StatusCode(500, result.Error.Message);
                }
            }

            return Json(account.ToViewModel());
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

            try
            {
                MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountById(accountId);
                if (account == null)
                {
                    return new NotFoundResult();
                }

                // clean up dependant Legal Entity record when deleting the account
                if (account.AdoxioAccountAdoxioLegalentityAccount != null)
                {
                    MicrosoftDynamicsCRMadoxioLegalentity legalentity = await _dynamicsClient.GetAdoxioLegalentityByAccountId(accountId);
                    if (legalentity != null)
                    {
                        await _dynamicsClient.Adoxiolegalentities.DeleteAsync(legalentity.AdoxioLegalentityid);
                    }
                }

                await _dynamicsClient.Accounts.DeleteAsync(accountId.ToString());

            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException ex)
            {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new NotFoundResult();
                }
                else
                {
                    return new BadRequestResult();
                }
            }
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
