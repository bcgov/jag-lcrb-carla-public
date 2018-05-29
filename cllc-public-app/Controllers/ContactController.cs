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


        /// <summary>
        /// Get a specific legal entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContact(string id)
        {
            ViewModels.Contact result = null;
            // query the Dynamics system to get the contact record.

            Guid? contactId = new Guid(id);
            Contexts.Microsoft.Dynamics.CRM.Contact contact = null;
            if (contactId != null)
            {
                try
                {
                    contact = await _system.Contacts.ByKey(contactId).GetValueAsync();
                    result = contact.ToViewModel();
                }
                catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                {
                    return new NotFoundResult();
                }
            }

            return Json(result);
        }


        /// <summary>
        /// Update a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact([FromBody] ViewModels.Contact item, string id)
        {
            /*
            if (id != item.id)
            {
                return BadRequest();
            }
            */

            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            Contexts.Microsoft.Dynamics.CRM.Contact contact = await _system.Contacts.ByKey(adoxio_legalentityid).GetValueAsync();

            // copy values over from the data provided
            contact.CopyValues(item);
            // TODO - figure out how to avoid having to specify these values.
            contact._accountid_value = new Guid("11b1cd43-2b3b-491d-aa43-8d9bb74edfbf");           
            contact._adoxio_applicationid_value = new Guid("79d9d4cb-7042-e811-a822-0003ff623e3e");
            contact._adoxio_establishment_value = new Guid("eb7f172e-e552-e811-8140-480fcfeac941");
            contact._adoxio_relatedlicence_value = new Guid("0380172e-e552-e811-8140-480fcfeac941");
            _system.UpdateObject(contact);

            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

            //DataServiceResponse dsr = await _system.SaveChangesAsync(); 

            return Json(contact.ToViewModel());
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
