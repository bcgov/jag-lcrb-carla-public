using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class AliasController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public AliasController(IConfiguration configuration, IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;            
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(AliasController));
        }


        /// <summary>
        /// Get a specific legal entity
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        [HttpGet("by-contactid/{contactId}")]
        public IActionResult GetAliasByContactId(string contactId)
        {
             ViewModels.Alias result = null;

            if (!string.IsNullOrEmpty(contactId))
            {
                // query the Dynamics system to get the contact record.
                string filter = $"_adoxio_contactid_value eq {contactId}";
                var fields = new List<string> { "adoxio_ContactId", "adoxio_WorkerId" };
                MicrosoftDynamicsCRMadoxioAlias alias = _dynamicsClient.Aliases.Get(filter: filter, expand: fields).Value.FirstOrDefault();

                if (alias != null)
                {
                    result = alias.ToViewModel();
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            else
            {
                return BadRequest();
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
        public async Task<IActionResult> UpdateContact([FromBody] ViewModels.Alias item, string id)
        {            
            if (id != null && item.id != null && id != item.id)
            {
                return BadRequest();
            }
            
            // get the contact
            Guid aliasId = Guid.Parse(id);
            
            MicrosoftDynamicsCRMadoxioAlias alias = await _dynamicsClient.GetAliasById(aliasId);
            MicrosoftDynamicsCRMcontact contact = await _dynamicsClient.GetContactById(Guid.Parse(item.contact.id));
            MicrosoftDynamicsCRMadoxioWorker worker = await _dynamicsClient.GetWorkerById(Guid.Parse(item.worker.id));

            if (alias == null)
            {
                return new NotFoundResult();
            }
            MicrosoftDynamicsCRMadoxioAlias patchAlias = new MicrosoftDynamicsCRMadoxioAlias();
            MicrosoftDynamicsCRMcontact patchContact = new MicrosoftDynamicsCRMcontact();
            MicrosoftDynamicsCRMadoxioWorker patchWorker = new MicrosoftDynamicsCRMadoxioWorker();
            patchAlias.CopyValues(item);
            try
            {
                await _dynamicsClient.Aliases.UpdateAsync(aliasId.ToString(), patchAlias);
                if (contact != null)
                {
                    patchContact.CopyValues(item.contact);
                    await _dynamicsClient.Contacts.UpdateAsync(contact.Contactid.ToString(), patchContact);
                }
                if (worker != null)
                {
                    patchWorker.CopyValues(item.worker);
                    await _dynamicsClient.Workers.UpdateAsync(worker.AdoxioWorkerid.ToString(), patchWorker);
                }
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating contact");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
            }            

            alias = await _dynamicsClient.GetAliasById(aliasId);
            return Json(alias.ToViewModel());
        }

        /// <summary>
        /// Create a contact
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateContact([FromBody] ViewModels.Contact item)
        {

            // get UserSettings from the session
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

             // first check to see that a contact exists.
            string contactSiteminderGuid = userSettings.SiteMinderGuid;
            if (contactSiteminderGuid == null || contactSiteminderGuid.Length == 0)
            {
                _logger.LogError(LoggingEvents.Error, "No Contact Siteminder Guid exernal id");
                throw new Exception("Error. No ContactSiteminderGuid exernal id");
            }

            // get the contact record.
            MicrosoftDynamicsCRMcontact userContact = null;

            // see if the contact exists.
            try
            {
                userContact = await _dynamicsClient.GetContactBySiteminderGuid(contactSiteminderGuid);
                if(userContact != null){
                    throw new Exception("Contact already Exists");
                }
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError(LoggingEvents.Error, "Error getting contact by Siteminder Guid.");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                throw new OdataerrorException("Error getting contact by Siteminder Guid");
            }

            // create a new contact.
            MicrosoftDynamicsCRMcontact contact = new MicrosoftDynamicsCRMcontact();
            contact.CopyValues(item);
            string sanitizedAccountSiteminderId = GuidUtility.SanitizeGuidString(contactSiteminderGuid);
            contact.Externaluseridentifier = userSettings.UserId;

            //clean externalId    
            var externalId = "";
            var tokens = sanitizedAccountSiteminderId.Split('|');
            if(tokens.Length > 0) {
                externalId = tokens[0];
            }

            if(!string.IsNullOrEmpty(externalId)) {
                tokens = externalId.Split(':');
                externalId  = tokens[tokens.Length -1];
            }

            contact.AdoxioExternalid = externalId;
            try
            {
                contact = await _dynamicsClient.Contacts.CreateAsync(contact);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating contact");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
            }

            // if we have not yet authenticated, then this is the new record for the user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.ContactId = contact.Contactid.ToString();

                // we can now authenticate.
                if (userSettings.AuthenticatedUser == null)
                {
                    Models.User user = new Models.User();
                    user.Active = true;
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
                _logger.LogError(LoggingEvents.Error, "Invalid user registration.");
                throw new Exception("Invalid user registration.");
            }

            return Json(contact.ToViewModel());
        }
    }
}
