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

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Interfaces.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BCeIDBusinessQuery _bceid;
		private readonly ILogger _logger;        

		public AccountController(Interfaces.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor, BCeIDBusinessQuery bceid, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = null; //distributedCache;                        
            this._httpContextAccessor = httpContextAccessor;
			this._bceid = bceid;
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
            ViewModels.Account result = null;

            // get UserSettings from the session
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Account> AccountCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Account>(_system);
			DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Contact> ContactCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Contact>(_system);
			DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity> legalEntityCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity>(_system);

			// get account siteminder id
			string accountSiteminderGuid = userSettings.SiteMinderBusinessGuid;
			if (accountSiteminderGuid == null || accountSiteminderGuid.Length == 0)
				throw new Exception("Oops no accountSiteminderGuid exernal id");

			// first check to see that a contact exists.
			string contactSiteminderGuid = userSettings.SiteMinderGuid;
			if (contactSiteminderGuid == null || contactSiteminderGuid.Length == 0)
				throw new Exception("Oops no contactSiteminderGuid exernal id");

			//Guid userContactId = new Guid(contactSiteminderGuid);
            Interfaces.Microsoft.Dynamics.CRM.Contact userContact = await _system.GetContactBySiteminderGuid(_distributedCache, contactSiteminderGuid);
            if (userContact == null)
            {
                // create the user contact record.
                userContact = new Interfaces.Microsoft.Dynamics.CRM.Contact();
                ContactCollection.Add(userContact);
                // Adoxio_externalid is where we will store the guid from siteminder.
                userContact.Adoxio_externalid = contactSiteminderGuid;
                userContact.Fullname = userSettings.UserDisplayName;
                userContact.Nickname = userSettings.UserDisplayName;
                userContact.Employeeid = userSettings.UserId;
                userContact.Firstname = userSettings.UserDisplayName.GetFirstName();
                userContact.Lastname = userSettings.UserDisplayName.GetLastName();
                userContact.Statuscode = 1;
            }

            // this may be an existing account, as this service is used during the account confirmation process.
			Interfaces.Microsoft.Dynamics.CRM.Account account = await _system.GetAccountBySiteminderBusinessGuid(_distributedCache, accountSiteminderGuid);
            if (account == null)
            {
                // create a new account
                account = new Interfaces.Microsoft.Dynamics.CRM.Account();
                AccountCollection.Add(account);
                // set the account siteminder guid
				account.Adoxio_externalid = accountSiteminderGuid;
				item.externalId = accountSiteminderGuid;

				// add a new legal entity record for this account
                Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity legalEntity = new Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity();
                legalEntityCollection.Add(legalEntity);
                legalEntity.Adoxio_Account = account;
				legalEntity.Adoxio_name = item.name;
				legalEntity.Adoxio_isindividual = 0;
				legalEntity.Adoxio_isapplicant = true;
            }
            else // it is an update.
            {
                _system.UpdateObject(account);
            }

            account.CopyValues(item);            

            if (account.Primarycontactid == null) // we need to add the primary contact.
            {                
                account.Primarycontactid = userContact;
				userContact.Parentcustomerid_account = account;
            }

			DataServiceResponse dsr = _system.SaveChangesSynchronous(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset);
			foreach (OperationResponse operationResponse in dsr)
            {
				_logger.LogError("dsr.response = " + operationResponse.StatusCode);
                if (operationResponse.StatusCode == 500) // error
                {
					_logger.LogError("dsr.error = " + operationResponse.Error.Message);
                    return StatusCode(500, operationResponse.Error.Message);
                }
            }

			var ida = dsr.GetAssignedIdOfType("accounts");
            if (ida == null)
                throw new Exception("account id is null");
			_logger.LogError("Account id = " + ida.ToString());
			var idc = dsr.GetAssignedIdOfType("contacts");
            if (idc == null)
                throw new Exception("contact id is null");
			_logger.LogError("Contact id = " + idc.ToString());
            
			_logger.LogError("accountSiteminderGuid = " + accountSiteminderGuid);
			_logger.LogError("contactSiteminderGuid = " + contactSiteminderGuid);
			account = await _system.GetAccountBySiteminderBusinessGuid(_distributedCache, accountSiteminderGuid);
			userContact = await _system.GetContactBySiteminderGuid(_distributedCache, contactSiteminderGuid);
            if (account == null && userContact == null)
				throw new Exception("Opps both account and contact are null");
			if (account == null)
                throw new Exception("Opps account is null");
			if (userContact == null)
                throw new Exception("Opps contact is null");
			account.Accountid = ida;
            userContact.Contactid = idc;
			if (account.Accountid == null && userContact.Contactid == null)
                throw new Exception("Opps both account and contact ID's are null");
			if (account.Accountid == null)
				throw new Exception("Opps account.Accountid is null");
			if (userContact.Contactid == null)
				throw new Exception("Opps contact.Contactid is null");

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
			_logger.LogError("AccountController --> id=" + result.id);
			_logger.LogError("AccountController --> externalId=" + result.externalId);

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
            // copy values over from the data provided
            account.CopyValues(item);

            

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

			// get the legal entity.
            //Guid adoxio_legalentityid = new Guid(id);
            try
            {
                Interfaces.Microsoft.Dynamics.CRM.Account account = await _system.GetAccountById(_distributedCache, accountId);
				if (account == null)
				{
					return new NotFoundResult();
				}

                // clean up dependant Legal Entity record when deleting the account
				if (account.Adoxio_account_adoxio_legalentity_Account != null)
				{
					Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity legalentity = await _system.GetGetAdoxioLegalentityByAccountId(_distributedCache, accountId);
					if (legalentity != null)
					{
						_system.DeleteObject(legalentity);
					}
				}
            
                _system.DeleteObject(account);

                DataServiceResponse dsr = _system.SaveChangesSynchronous();
                foreach (OperationResponse result in dsr)
                {
                    if (result.StatusCode == 500) // error
                    {
                        return StatusCode(500, result.Error.Message);
                    }
                }
            }
            catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
            {
                return new NotFoundResult();
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
