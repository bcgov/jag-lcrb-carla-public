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
using Gov.Lclb.Cllb.Public.ViewModels;
using Gov.Lclb.Cllb.Public.Extensions;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LicenceEventsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;
        private readonly IPdfService _pdfClient;

        public LicenceEventsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, IPdfService pdfClient)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(LicenceEventsController));
            _pdfClient = pdfClient;
        }


        /// <summary>
        /// Create an event
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateLicenceEvent([FromBody] ViewModels.LicenceEvent item)
        {
            MicrosoftDynamicsCRMadoxioEvent dynamicsEvent = new MicrosoftDynamicsCRMadoxioEvent();
            if (item?.Status == LicenceEventStatus.Submitted)
            {
                bool alwaysAuthorization;
                try
                {
                    var licence = _dynamicsClient.Licenceses.GetByKey(item.LicenceId);
                    alwaysAuthorization = licence.AdoxioIseventapprovalalwaysrequired == null ? false : (bool)licence.AdoxioIseventapprovalalwaysrequired;
                }
                catch (HttpOperationException ex)
                {
                    _logger.LogError(ex, "Error creating event");
                    return BadRequest();
                }
                item.EventClass = item.DetermineEventClass(alwaysAuthorization);
                if (item.EventClass != EventClass.Authorization)
                {
                    item.Status = LicenceEventStatus.Approved;
                }
                else
                {
                    item.Status = LicenceEventStatus.InReview;
                }
            }
            
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

            LicenceEvent viewModelEvent = dynamicsEvent.ToViewModel(_dynamicsClient);

            if (item.Schedules != null && item.Schedules.Count > 0) {
                viewModelEvent.Schedules = new List<LicenceEventSchedule>();
                foreach (LicenceEventSchedule eventSchedule in item.Schedules)
                {
                    MicrosoftDynamicsCRMadoxioEventschedule dynamicsSchedule = new MicrosoftDynamicsCRMadoxioEventschedule();
                    dynamicsSchedule.CopyValues(eventSchedule);
                    dynamicsSchedule.EventODataBind = _dynamicsClient.GetEntityURI("adoxio_events", dynamicsEvent.AdoxioEventid);
                    MicrosoftDynamicsCRMadoxioEventschedule newEventSchedule = _dynamicsClient.Eventschedules.Create(dynamicsSchedule);
                    viewModelEvent.Schedules.Add(newEventSchedule.ToViewModel());
                }
            }

            return new JsonResult(viewModelEvent);
        }

        /// <summary>
        /// Get an event if the user has access
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLicenceEvent(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            
            MicrosoftDynamicsCRMadoxioEvent dynamicsEvent;
            MicrosoftDynamicsCRMadoxioEventscheduleCollection dynamicsEventScheduleCollection;
            try
            {
                dynamicsEvent = _dynamicsClient.GetEventByIdWithChildren(id);
                // dynamicsEventScheduleCollection = _dynamicsClient.GetEventSchedulesByEventId(id);
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

            LicenceEvent result = dynamicsEvent.ToViewModel(_dynamicsClient);

            return new JsonResult(result);
        }

        /// <summary>
        /// Update an event if the user has access
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLicenceEvent([FromBody] ViewModels.LicenceEvent item, string id)
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

            // not updating security plan
            if (item?.SecurityPlanSubmitted == null && item?.Status == LicenceEventStatus.Submitted)
            {
                // determine event class
                bool alwaysAuthorization;
                try
                {
                    var licence = _dynamicsClient.Licenceses.GetByKey(item.LicenceId);
                    alwaysAuthorization = licence.AdoxioIseventapprovalalwaysrequired == null ? false : (bool)licence.AdoxioIseventapprovalalwaysrequired;
                }
                catch (HttpOperationException ex)
                {
                    _logger.LogError(ex, "Error updating event");
                    return BadRequest();
                }
                item.EventClass = item.DetermineEventClass(alwaysAuthorization);
                if (item.EventClass != EventClass.Authorization)
                {
                    item.Status = LicenceEventStatus.Approved;
                }
                else
                {
                    item.Status = LicenceEventStatus.InReview;
                }
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

            /* Get current event schedules and delete */
            LicenceEvent viewModelEvent = dynamicsEvent.ToViewModel(_dynamicsClient);
            
            if (viewModelEvent.Schedules != null && viewModelEvent.Schedules.Count > 0) {
                foreach (LicenceEventSchedule eventSchedule in viewModelEvent.Schedules)
                {
                    _dynamicsClient.Eventschedules.Delete(eventSchedule.Id);
                }
            }

            /* Create new event schedules */
            if (item.Schedules != null && item.Schedules.Count > 0) {
                viewModelEvent.Schedules = new List<LicenceEventSchedule>();
                foreach (LicenceEventSchedule eventSchedule in item.Schedules)
                {
                    MicrosoftDynamicsCRMadoxioEventschedule dynamicsSchedule = new MicrosoftDynamicsCRMadoxioEventschedule();
                    dynamicsSchedule.CopyValues(eventSchedule);
                    dynamicsSchedule.EventODataBind = _dynamicsClient.GetEntityURI("adoxio_events", dynamicsEvent.AdoxioEventid);
                    MicrosoftDynamicsCRMadoxioEventschedule newEventSchedule = _dynamicsClient.Eventschedules.Create(dynamicsSchedule);
                    viewModelEvent.Schedules.Add(newEventSchedule.ToViewModel());
                }
            }

            if (dynamicsEvent != null)
            {
                return new JsonResult(dynamicsEvent.ToViewModel(_dynamicsClient));
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Delete an event if the user has access
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLicenceEvent(string id)
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

            /* Get current event schedules and delete */
            LicenceEvent viewModelEvent = dynamicsEvent.ToViewModel(_dynamicsClient);
            
            if (viewModelEvent.Schedules != null && viewModelEvent.Schedules.Count > 0) {
                foreach (LicenceEventSchedule eventSchedule in viewModelEvent.Schedules)
                {
                    _dynamicsClient.Eventschedules.Delete(eventSchedule.Id);
                }
            }

            

            _dynamicsClient.Events.Delete(id);

            return NoContent();
        }

        /// <summary>
        /// Get events that the user has access to
        /// </summary>
        /// <returns></returns>
        [HttpGet("list/{licenceId}/{num}")]
        public async Task<IActionResult> GetLicenceEventsList(string licenceId, int num)
        {
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            MicrosoftDynamicsCRMadoxioEventCollection dynamicsEvents;
            List<ViewModels.LicenceEvent> responseEvents = new List<ViewModels.LicenceEvent>();
            
            string filter = $"_adoxio_account_value eq {userSettings.AccountId} and _adoxio_licence_value eq {licenceId}";
            try
            {
                dynamicsEvents = _dynamicsClient.Events.Get(filter: filter, top: num, orderby: new List<string> { "modifiedon desc" });
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
                    responseEvents.Add(evt.ToViewModel(_dynamicsClient));
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

        /// GET an authorization as PDF.
        [HttpGet("{eventId}/authorization.pdf")]
        public async Task<IActionResult> GetAuthorizationPdf(string eventId)
        {
            MicrosoftDynamicsCRMadoxioEvent licenceEvent;
            LicenceEvent licenceEventVM;
            MicrosoftDynamicsCRMadoxioLicences licence;
            MicrosoftDynamicsCRMaccount account;

            try
            {
                licenceEvent = _dynamicsClient.Events.GetByKey(eventId);
                licenceEventVM = licenceEvent.ToViewModel(_dynamicsClient);
                licence = _dynamicsClient.Licenceses.GetByKey(licenceEventVM.LicenceId);
                account = _dynamicsClient.Accounts.GetByKey(licence._adoxioLicenceeValue);
            }
            catch (HttpOperationException)
            {
                return new NotFoundResult();
            }

            if (!CurrentUserHasAccessToEventOwnedBy(licence._adoxioLicenceeValue))
            {
                return new NotFoundResult();
            }

            string eventTimings = "";
            foreach (var schedule in licenceEventVM.Schedules)
            {
                eventTimings += $@"<tr class='hide-border'>
                        <td style='width: 50%; text-align: left;'>{schedule.EventStartDateTime?.ToString("MMMM dd, yyyy")} - Event Hours: {schedule.EventStartDateTime?.ToString("h:mm tt")} to {schedule.EventEndDateTime?.ToString("h:mm tt")}</td>
                        <td style='width: 50%; text-align: left;'>Service Hours: {schedule.ServiceStartDateTime?.ToString("h:mm tt")} to {schedule.ServiceEndDateTime?.ToString("h:mm tt")}</td>
                    </tr>";
            }
            Dictionary<string, string> parameters;
            parameters = new Dictionary<string, string>
            {
                { "licensee", account.Name },
                { "licenceNumber", licence.AdoxioLicencenumber },
                { "licenceExpiryDate", licence.AdoxioExpirydate?.ToString("MMMM dd, yyyy") },
                { "licenseePhone", account.Telephone1 },
                { "licenseeEmail", account.Emailaddress1 },
                { "contactName", licenceEventVM.ContactName },
                { "contactPhone", licenceEventVM.ContactPhone },
                { "hostname", licenceEventVM.ClientHostname },
                { "startDate", licenceEventVM.StartDate?.ToString("MMMM dd, yyyy") },
                { "endDate", licenceEventVM.EndDate?.ToString("MMMM dd, yyyy") },
                { "eventTimings", eventTimings },
                { "eventType", EnumExtensions.GetEnumMemberValue(licenceEventVM.EventType)},
                { "eventDescription", licenceEventVM.EventTypeDescription },
                { "foodService", EnumExtensions.GetEnumMemberValue(licenceEventVM.FoodService) },
                { "entertainment", EnumExtensions.GetEnumMemberValue(licenceEventVM.Entertainment) },
                { "attendance", licenceEventVM.MaxAttendance.ToString() },
                { "minors", licenceEventVM.MinorsAttending ?? false ? "Yes" : "No" },
                { "location", licenceEventVM.SpecificLocation.ToString() },
                { "addressLine1", licenceEventVM.Street1 },
                { "addressLine2", licenceEventVM.Street2 },
                { "addressLine3", $"{licenceEventVM.City}, BC {licenceEventVM.PostalCode}" },
                { "inspectorName", licenceEvent.AdoxioEventinspectorname },
                { "inspectorPhone", licenceEvent.AdoxioEventinspectorphone },
                { "inspectorEmail", licenceEvent.AdoxioEventinspectoremail },
                { "date", DateTime.Now.ToString("MMMM dd, yyyy") }
            };

            byte[] data;
            try
            {
                data = await _pdfClient.GetPdf(parameters, "event_authorization");
                return File(data, "application/pdf", $"authorization.pdf");
            }
            catch (Exception)
            {
                return new NotFoundResult();
            }
        }
    }
}
