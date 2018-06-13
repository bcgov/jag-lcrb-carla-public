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
		private readonly ILogger _logger;        

		public AccountController(Interfaces.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = null; //distributedCache;                        
            this._httpContextAccessor = httpContextAccessor;
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
        
        /// <summary>
        /// Get all Legal Entities
        /// </summary>
        /// <param name="shareholder"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsAccounts()
        {
            // this method is not required, remove 
			throw new NotImplementedException();

            //List<ViewModels.Account> result = new List<ViewModels.Account>();
            //IEnumerable<Interfaces.Microsoft.Dynamics.CRM.Account> accounts = null;
            //accounts = await _system.Accounts.ExecuteAsync();            
            //foreach (var legalEntity in accounts)
            //{
            //    result.Add(legalEntity.ToViewModel());
            //}
            //return Json(result);
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
            //Guid? id = null;
            //Guid contactId = new Guid();
            //if (item.externalId == null || item.externalId.Length == 0)
			//{
			//	item.externalId = item.id;
			//}
			//var strid = item.externalId;
			//if (strid == null || strid.Length == 0)
			//	throw new Exception("Oops no account exernal id");

            // get UserSettings from the session
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Account> AccountCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Account>(_system);
            DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Contact> ContactCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Contact>(_system);

			// get account siteminder id
			string accountSiteminderGuid = userSettings.SiteMinderBusinessGuid;
			if (accountSiteminderGuid == null || accountSiteminderGuid.Length == 0)
				throw new Exception("Oops no accountSiteminderGuid exernal id");

			// first check to see that a contact exists.
			string contactSiteminderGuid = userSettings.SiteMinderGuid;
			if (contactSiteminderGuid == null || contactSiteminderGuid.Length == 0)
				throw new Exception("Oops no contactSiteminderGuid exernal id");

			//Guid userContactId = new Guid(contactSiteminderGuid);
            Interfaces.Microsoft.Dynamics.CRM.Contact userContact = await _system.GetContactBySiteminderId(_distributedCache, contactSiteminderGuid);
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

                // save the new contact. 
                //DataServiceResponse userContactDsr = await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithIndependentOperations );
                //foreach (OperationResponse operationResponse in userContactDsr)
                //{
                //    if (operationResponse.StatusCode == 500) // error
                //    {
                //        return StatusCode(500, operationResponse.Error.Message);
                //    }
                //}
                //contactId = (Guid) userContactDsr.GetAssignedId();
                //userContact = await _system.GetContactById(_distributedCache, contactId);
            }
            

            // this may be an existing account, as this service is used during the account confirmation process.
			Interfaces.Microsoft.Dynamics.CRM.Account account = await _system.GetAccountBySiteminderId(_distributedCache, accountSiteminderGuid);
            if (account == null)
            {
                // create a new account
                account = new Interfaces.Microsoft.Dynamics.CRM.Account();
                AccountCollection.Add(account);
                // set the account siteminder guid
				account.Adoxio_externalid = accountSiteminderGuid;
				item.externalId = accountSiteminderGuid;
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

			DataServiceResponse dsr = await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset);
            foreach (OperationResponse operationResponse in dsr)
            {
                if (operationResponse.StatusCode == 500) // error
                {
                    return StatusCode(500, operationResponse.Error.Message);
                }
            }

			var ida = dsr.GetAssignedIdOfType("account");
            if (ida == null)
                throw new Exception("account id is null");
			var idc = dsr.GetAssignedIdOfType("contact");
            if (idc == null)
                throw new Exception("contact id is null");
            
			//account = await _system.GetAccountById(_distributedCache, (Guid) id);
			//userContact = await _system.GetContactById(_distributedCache, (Guid)id);

			account = await _system.GetAccountBySiteminderId(_distributedCache, accountSiteminderGuid);
			account.Accountid = ida;
			userContact = await _system.GetContactBySiteminderId(_distributedCache, contactSiteminderGuid);
			userContact.Contactid = idc;
            if (account == null && userContact == null)
				throw new Exception("Opps both account and contact are null");
			if (account == null)
                throw new Exception("Opps account is null");
			if (userContact == null)
                throw new Exception("Opps contact is null");
			if (account.Accountid == null && userContact.Contactid == null)
                throw new Exception("Opps both account and contact ID's are null");
			if (account.Accountid == null)
				throw new Exception("Opps account.Accountid is null");
			if (userContact.Contactid == null)
				throw new Exception("Opps contact.Contactid is null");

            //userContact = await _system.GetContactById(_distributedCache, contactId);
            //_system.UpdateObject(userContact);
            //userContact.Parentcustomerid_account = account;

            //dsr = await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithIndependentOperations );
            //foreach (OperationResponse operationResult in dsr)
            //{
            //    if (operationResult.StatusCode == 500) // error
            //    {
            //        return StatusCode(500, operationResult.Error.Message);
            //    }
            //}
            
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
					user.Guid = userSettings.AccountId;
					user.Id = Guid.Parse(userSettings.ContactId);
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
        /// <param name="accountId"></param>
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

            DataServiceResponse dsr = await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithIndependentOperations);
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
                _system.DeleteObject(account);
                DataServiceResponse dsr = await _system.SaveChangesAsync();
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
        /// Get Directors and Officers for a given Account (Business)
        /// </summary>
        /// <returns>JSON list of directors and officers (Legal entities)</returns>
        [HttpGet("{id}/directorsandofficers")]
        public async Task<IActionResult> GetAccountDirectorsAndOfficers(string id)
        {
			// verify the currently logged in user has access to this account
            Guid accountId = new Guid(id);
            if (!CurrentUserHasAccessToAccount(accountId))
            {
                return new NotFoundResult();
            }

            List<ViewModels.AdoxioLegalEntity> result = new List<ViewModels.AdoxioLegalEntity>();
            var legalEntities = await _system.Adoxio_legalentities
                 // select all records for which there is a matching account and the position is director or officer.
                 // 3 is Director, 4 is Officer
                 .AddQueryOption("$filter", "_adoxio_account_value eq " + id + " and (adoxio_position eq 3 or adoxio_position eq 4)")
                 .ExecuteAsync();

            foreach (var legalEntity in legalEntities)
            {
                result.Add(legalEntity.ToViewModel());
            }
            return Json(result);
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
