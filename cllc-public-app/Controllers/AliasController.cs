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
        /// Get Aliases associated with the contact id
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        [HttpGet("by-contactid/{contactId}")]
        public IActionResult GetAliasByContactId(string contactId)
        {
             var result = new List<ViewModels.Alias>();

            if (!string.IsNullOrEmpty(contactId))
            {
                // query the Dynamics system to get the contact record.
                string filter = $"_adoxio_contactid_value eq {contactId}";
                List<MicrosoftDynamicsCRMadoxioAlias> aliases = _dynamicsClient.Aliases.Get(filter: filter).Value.ToList();

                if (aliases != null)
                {
                    foreach (var item in aliases)
                    {
                        result.Add(item.ToViewModel());
                    }
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
        /// Update an Alias
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlias([FromBody] ViewModels.Alias item, string id)
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
        /// Create an Alias
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateAlias([FromBody] ViewModels.Alias item)
        {
            if(item == null || item.contact.id == null || item.worker.id == null){
                return BadRequest();
            }

            // for association with current user
            string userJson = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(userJson);

            MicrosoftDynamicsCRMadoxioAlias alias = new MicrosoftDynamicsCRMadoxioAlias();
            // copy received values to Dynamics Application
            alias.CopyValues(item);
            try
            {
                alias = _dynamicsClient.Aliases.Create(alias);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error creating application");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail if we can't create.
                throw (odee);
            }


            MicrosoftDynamicsCRMadoxioAlias patchAlias = new MicrosoftDynamicsCRMadoxioAlias();

            // set contact and worker associations
            try
            {
                var worker = _dynamicsClient.GetWorkerById(Guid.Parse(item.worker.id));
                patchAlias.WorkerIdAccountODataBind = _dynamicsClient.GetEntityURI("worker", item.worker.id);
                
                var contact = _dynamicsClient.GetContactById(Guid.Parse(item.contact.id));
                patchAlias.ContactIdAccountODataBind = _dynamicsClient.GetEntityURI("contact", item.contact.id);

                await _dynamicsClient.Aliases.UpdateAsync(alias.AdoxioAliasid, patchAlias);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating application");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail if we can't create.
                throw (odee);
            }

            return Json(alias.ToViewModel());
        }

        /// <summary>
        /// Delete an Address.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteAlias(string id)
        {
            MicrosoftDynamicsCRMadoxioAlias alias = await _dynamicsClient.GetAliasById(Guid.Parse(id));
            if (alias == null)
            {
                return new NotFoundResult();
            }

            await _dynamicsClient.Aliases.DeleteAsync(id);

            return NoContent(); // 204
        }
    }
}
