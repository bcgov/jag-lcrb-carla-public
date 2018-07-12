using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Client;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public ContactController(IConfiguration configuration, IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;            
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(ContactController));
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
                        
            if (!string.IsNullOrEmpty (id))
            {
                Guid contactId = Guid.Parse(id);
                // query the Dynamics system to get the contact record.
                MicrosoftDynamicsCRMcontact contact = await _dynamicsClient.GetContactById(contactId);

                if (contact != null)
                {                    
                    result = contact.ToViewModel();
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
        public async Task<IActionResult> UpdateContact([FromBody] ViewModels.Contact item, string id)
        {            
            if (id != null && item.id != null && id != item.id)
            {
                return BadRequest();
            }
            
            // get the contact
            Guid contactId = Guid.Parse(id);
            
            MicrosoftDynamicsCRMcontact contact = await _dynamicsClient.GetContactById(contactId);
            if (contact == null)
            {
                return new NotFoundResult();
            }
            MicrosoftDynamicsCRMcontact patchContact = new MicrosoftDynamicsCRMcontact();
            patchContact.CopyValues(item);
            try
            {
                await _dynamicsClient.Contacts.UpdateAsync(contactId.ToString(), patchContact);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating contact");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
            }            

            contact = await _dynamicsClient.GetContactById(contactId);
            return Json(contact.ToViewModel());
        }

        /// <summary>
        /// Create a contact
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateContact([FromBody] ViewModels.Contact item)
        {

            // create a new contact.
            MicrosoftDynamicsCRMcontact contact = new MicrosoftDynamicsCRMcontact();
            contact.CopyValues(item);
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

            return Json(contact.ToViewModel());
        }
    }
}
