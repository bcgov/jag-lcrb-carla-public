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
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class PreviousAddressController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public PreviousAddressController(IConfiguration configuration, IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(PreviousAddressController));
        }


        // /// <summary>
        // /// Get a specific previous address
        // /// </summary>
        // /// <param name="contactId"></param>
        // /// <returns></returns>
        [HttpGet("by-contactid/{contactId}")]
        public async Task<IActionResult> GetAddressesByContactId(string contactId)
        {
            List<ViewModels.PreviousAddress> result = new List<ViewModels.PreviousAddress>();

            if (!string.IsNullOrEmpty(contactId))
            {
                // query the Dynamics system to get the Address record.
                List<MicrosoftDynamicsCRMadoxioPreviousaddress> addresses = await _dynamicsClient.GetPreviousAddressByContactId(contactId);

                if (addresses != null)
                {
                    addresses.ForEach(a =>
                    {
                        result.Add(a.ToViewModel());
                    });
                    
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


        // /// <summary>
        // /// Update a legal entity
        // /// </summary>
        // /// <param name="item"></param>
        // /// <param name="id"></param>
        // /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress([FromBody] ViewModels.PreviousAddress item, string id)
        {
            if (id != null && item.id != null && id != item.id)
            {
                return BadRequest();
            }

            // get the Address
            MicrosoftDynamicsCRMadoxioPreviousaddress Address = await _dynamicsClient.GetPreviousAddressById(id);
            if (Address == null)
            {
                return new NotFoundResult();
            }
            MicrosoftDynamicsCRMadoxioPreviousaddress patchAddress = new MicrosoftDynamicsCRMadoxioPreviousaddress();
            patchAddress.CopyValues(item);
            try
            {
                await _dynamicsClient.Previousaddresses.UpdateAsync(id, patchAddress);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating Address");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
            }

            Address = await _dynamicsClient.GetPreviousAddressById(id);
            return Json(Address.ToViewModel());
        }

        // /// <summary>
        // /// Create a Address
        // /// </summary>
        // /// <param name="viewModel"></param>
        // /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateAddress([FromBody] ViewModels.PreviousAddress item)
        {

            // for association with current user
            string userJson = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(userJson);

            MicrosoftDynamicsCRMadoxioPreviousaddress address = new MicrosoftDynamicsCRMadoxioPreviousaddress();
            // copy received values to Dynamics Application
            address.CopyValues(item);
            try
            {
                address = _dynamicsClient.Previousaddresses.Create(address);
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


            MicrosoftDynamicsCRMadoxioPreviousaddress patchAddress = new MicrosoftDynamicsCRMadoxioPreviousaddress();

            // set contact and worker associations
            try
            {
                var contact = _dynamicsClient.GetContactById(Guid.Parse(item.contactId));
                patchAddress.ContactIdAccountODataBind = _dynamicsClient.GetEntityURI("contact", item.contactId);

                var worker = _dynamicsClient.GetWorkerById(Guid.Parse(item.workerId));
                patchAddress.WorkerIdAccountODataBind = _dynamicsClient.GetEntityURI("worker", item.workerId);

                await _dynamicsClient.Previousaddresses.UpdateAsync(address.AdoxioPreviousaddressid, patchAddress);
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

            return Json(address.ToViewModel());
        }

        /// <summary>
        /// Delete an Address.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteAddress(string id)
        {
            MicrosoftDynamicsCRMadoxioPreviousaddress address = await _dynamicsClient.GetPreviousAddressById(id);
            if (address == null)
            {
                return new NotFoundResult();
            }

            await _dynamicsClient.Previousaddresses.DeleteAsync(id);

            return NoContent(); // 204
        }
    }
}
