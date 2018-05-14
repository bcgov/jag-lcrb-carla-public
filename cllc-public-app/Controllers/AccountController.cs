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
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Client;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Contexts.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(Contexts.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = distributedCache;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpPost()]
        public async Task<JsonResult> CreateAccount([FromBody] ViewModels.Account item)
        {
            // create a new account
            Contexts.Microsoft.Dynamics.CRM.Account account = new Contexts.Microsoft.Dynamics.CRM.Account();

            DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Account> AccountCollection = new DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Account>(_system);
            DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Contact> ContactCollection = new DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Contact>(_system);

            AccountCollection.Add(account);          
            account.Name = item.name;
            account.Description = item.description;

            if (item.primarycontact != null)
            {
                // get the contact.
                Contexts.Microsoft.Dynamics.CRM.Contact contact = new Contexts.Microsoft.Dynamics.CRM.Contact();
                contact.Fullname = item.primarycontact.name;
                contact.Contactid = new Guid(item.primarycontact.id);
                account.Primarycontactid = contact;
            }


            await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset);

            // if we have not yet authenticated, then this is the new record for the user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            if (userSettings.IsNewUserRegistration)
            {
                
                // we can now authenticate.
                if (userSettings.AuthenticatedUser == null)
                {
                    Models.User user = new Models.User();
                    user.Active = true;
                    user.Guid = userSettings.ContactId;
                    user.SmUserId = userSettings.UserId;
                    userSettings.AuthenticatedUser = user;                        
                }

                userSettings.IsNewUserRegistration = false;

                string userSettingsString = JsonConvert.SerializeObject(userSettings);
                // add the user to the session.
                _httpContextAccessor.HttpContext.Session.SetString("UserSettings", userSettingsString);
                
            }

            return Json(account);
        }
    }
}
