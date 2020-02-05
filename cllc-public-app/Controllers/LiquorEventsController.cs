using Gov.Lclb.Cllb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.Rest;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LiquorEventsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;

        public LiquorEventsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(LiquorEventsController));
        }


        /// <summary>
        /// Create an event
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateLiquorEvent([FromBody] ViewModels.LiquorEvent item)
        {
            MicrosoftDynamicsCRMadoxioEvent dynamicsEvent = new MicrosoftDynamicsCRMadoxioEvent();
            dynamicsEvent.CopyValues(item);

            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            dynamicsEvent.AccountODataBind = _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId);

            if (!string.IsNullOrEmpty(item.LicenceId)) {
                dynamicsEvent.LicenceODataBind = _dynamicsClient.GetEntityURI("licenses", item.LicenceId);
            }
            
            try
            {
                dynamicsEvent = _dynamicsClient.Events.Create(dynamicsEvent);
            }
            catch (HttpOperationException ex)
            {
                _logger.LogError(ex, "Error creating event");
                return BadRequest();
            }

            return new JsonResult(dynamicsEvent.ToViewModel());
        }

        /// <summary>
        /// Get an event if the user has access
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLiquorEvent(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            
            MicrosoftDynamicsCRMadoxioEvent dynamicsEvent;
            try
            {
                dynamicsEvent = _dynamicsClient.GetEventByIdWithChildren(id);
            }
            catch (HttpOperationException ex)
            {
                _logger.LogError(ex, "Error retrieving Event");
                return new NotFoundResult();
            }

            if (dynamicsEvent == null || !CurrentUserHasAccessToEventOwnedBy(dynamicsEvent.AdoxioAccount.Accountid))
            {
                return new NotFoundResult();
            }

            return new JsonResult(dynamicsEvent.ToViewModel());
        }

        /// <summary>
        /// Update an event if the user has access
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLiquorEvent([FromBody] ViewModels.LiquorEvent item, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            MicrosoftDynamicsCRMadoxioEvent dynamicsEvent = _dynamicsClient.GetEventByIdWithChildren(id);

            if (dynamicsEvent == null || !CurrentUserHasAccessToEventOwnedBy(dynamicsEvent.AdoxioAccount.Accountid))
            {
                return new NotFoundResult();
            }

            MicrosoftDynamicsCRMadoxioEvent patchEvent = new MicrosoftDynamicsCRMadoxioEvent();
            patchEvent.CopyValues(item);
            if (!string.IsNullOrEmpty(item.LicenceId) && item.LicenceId != dynamicsEvent._adoxioLicenceValue) {
                patchEvent.LicenceODataBind = _dynamicsClient.GetEntityURI("licenses", item.LicenceId);
            }
            try
            {
                _dynamicsClient.Events.Update(id, patchEvent);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating event");
            }

            dynamicsEvent = _dynamicsClient.GetEventByIdWithChildren(id);
            if (dynamicsEvent != null)
            {
                return new JsonResult(dynamicsEvent.ToViewModel());
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Delete an event if the user has access
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLiquorEvent(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestResult();
            }
            
            MicrosoftDynamicsCRMadoxioEvent dynamicsEvent;
            try
            {
                dynamicsEvent = _dynamicsClient.GetEventByIdWithChildren(id);
            }
            catch (HttpOperationException ex)
            {
                _logger.LogError(ex, "Failed to delete event");
                return new NotFoundResult();
            }

            if (dynamicsEvent == null || !CurrentUserHasAccessToEventOwnedBy(dynamicsEvent.AdoxioAccount.Accountid))
            {
                return new NotFoundResult();
            }

            _dynamicsClient.Events.Delete(id);

            return NoContent();
        }

        /// <summary>
        /// Get events that the user has access to
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> GetLiquorEventsList()
        {
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            MicrosoftDynamicsCRMadoxioEventCollection dynamicsEvents;
            List<ViewModels.LiquorEvent> responseEvents = new List<ViewModels.LiquorEvent>();
            
            string filter = "_adoxio_account_value eq " + userSettings.AccountId;
            try
            {
                dynamicsEvents = _dynamicsClient.Events.Get(filter: filter);
            }
            catch (HttpOperationException ex)
            {
                _logger.LogError(ex, "Error retrieving Events");
                return new NotFoundResult();
            }

            if (dynamicsEvents.Value.Count > 0)
            {
                foreach (MicrosoftDynamicsCRMadoxioEvent evt in dynamicsEvents.Value)
                {
                    responseEvents.Add(evt.ToViewModel());
                }
            }

            return new JsonResult(responseEvents);
        }

        private bool CurrentUserHasAccessToEventOwnedBy(string accountId)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // For now, check if the account id matches the user's account.
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return userSettings.AccountId == accountId;
            }

            // if current user doesn't have an account they are probably not logged in
            return false;
        }
    }
}
