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
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class WorkerController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public WorkerController(IConfiguration configuration, IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(WorkerController));
        }


        /// <summary>
        /// Get a specific worker
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        [HttpGet("{contactId}")]
        public async Task<IActionResult> GetWorker(string contactId)
        {
            ViewModels.Worker result = null;

            if (!string.IsNullOrEmpty(contactId))
            {
                Guid id = Guid.Parse(contactId);
                // query the Dynamics system to get the contact record.
                string filter = $"_adoxio_contactid_value eq {contactId}";
                var fields = new List<string> { "adoxio_ContactId" };
                MicrosoftDynamicsCRMadoxioWorker worker = _dynamicsClient.Workers.Get(filter: filter, expand: fields).Value.FirstOrDefault();

                if (worker != null)
                {
                    result = worker.ToViewModel();
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
        /// Update a worker
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
          public async Task<IActionResult> UpdateWorker([FromBody] ViewModels.Worker item, string id)
        {            
            if (id != null && item.id != null && id != item.id)
            {
                return BadRequest();
            }
            
            // get the contact
            Guid workerId = Guid.Parse(id);
            
            MicrosoftDynamicsCRMadoxioWorker worker = await _dynamicsClient.GetWorkerById(Guid.Parse(item.id));

            if (worker == null)
            {
                return new NotFoundResult();
            }
            MicrosoftDynamicsCRMadoxioWorker patchWorker = new MicrosoftDynamicsCRMadoxioWorker();
            patchWorker.CopyValues(item);
            try
            {
                    await _dynamicsClient.Workers.UpdateAsync(worker.AdoxioWorkerid.ToString(), patchWorker);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating contact");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
            }            

            worker = await _dynamicsClient.GetWorkerById(workerId);
            return Json(worker.ToViewModel());
        }

        /// <summary>
        /// Create a worker    
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateWorker([FromBody] ViewModels.Worker item)
        {

            // get UserSettings from the session
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // create a new worker.
            MicrosoftDynamicsCRMadoxioWorker worker = new MicrosoftDynamicsCRMadoxioWorker();
            worker.CopyValues(item);

            if(item?.contact?.id == null)
            {
                return BadRequest();
            }

            try
            {

                worker = await _dynamicsClient.Workers.CreateAsync(worker);
                var patchWorker = new MicrosoftDynamicsCRMadoxioWorker();
                patchWorker.ContactIdAccountODataBind = _dynamicsClient.GetEntityURI("contact", item.contact.id);
                await _dynamicsClient.Workers.UpdateAsync(worker.AdoxioWorkerid.ToString(), patchWorker);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating contact");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
            }

            return Json(worker);
        }


        /// <summary>
        /// Delete an Address.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteWorker(string id)
        {
            MicrosoftDynamicsCRMadoxioWorker worker = await _dynamicsClient.GetWorkerById(Guid.Parse(id));
            if (worker == null)
            {
                return new NotFoundResult();
            }

            await _dynamicsClient.Workers.DeleteAsync(id);

            return NoContent(); // 204
        }
    }
}
