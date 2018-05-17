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
    public class ContactController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Contexts.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public ContactController(Contexts.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            this._system = context;
            this._httpContextAccessor = httpContextAccessor;
            this._distributedCache = distributedCache;
        }
        [HttpPost()]
        public async Task<JsonResult> CreateContact([FromBody] ViewModels.Contact viewModel)
        {
            Contexts.Microsoft.Dynamics.CRM.Contact item = viewModel.ToModel();

            // create a new contact.
            Contexts.Microsoft.Dynamics.CRM.Contact contact = new Contexts.Microsoft.Dynamics.CRM.Contact();

            // create a DataServiceCollection to add the record
            DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Contact> ContactCollection = new DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Contact>(_system);
            // add a new contact.
            ContactCollection.Add(contact);

            // changes need to made after the add in order for them to be saved.
            contact.CopyValues(item);
            contact.Contactid = Guid.NewGuid();

            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

            await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset);

            // if we have not yet authenticated, then this is the new record for the user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            if (userSettings.IsNewUserRegistration)
            {
                if (string.IsNullOrEmpty (userSettings.ContactId))
                {
                    userSettings.ContactId = contact.Contactid.ToString();
                    string userSettingsString = JsonConvert.SerializeObject(userSettings);
                    // add the user to the session.
                    _httpContextAccessor.HttpContext.Session.SetString("UserSettings", userSettingsString);
                }
            }            

            return Json(contact);
        }
    }
}
