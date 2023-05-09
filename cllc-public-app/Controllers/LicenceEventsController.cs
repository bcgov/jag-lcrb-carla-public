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
using System.Globalization;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;

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
        private readonly FileManagerClient _fileManagerClient;

        public LicenceEventsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, IPdfService pdfClient, FileManagerClient fileClient)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(LicenceEventsController));
            _pdfClient = pdfClient;
            _fileManagerClient = fileClient;
        }


        /// <summary>
        /// Create an event
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateLicenceEvent([FromBody] LicenceEvent item)
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
                if (item.EventClass != EventClass.Authorization || item.EventCategory == EventCategory.Market)
                {
                    item.Status = LicenceEventStatus.Approved;
                }
                else
                {
                    item.Status = LicenceEventStatus.InReview;
                }
            }

            dynamicsEvent.CopyValues(item);

            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            dynamicsEvent.AccountODataBind = _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId);

            if (!string.IsNullOrEmpty(item.LicenceId))
            {
                dynamicsEvent.LicenceODataBind = _dynamicsClient.GetEntityURI("adoxio_licenceses", item.LicenceId);
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
            // create event schedules - if any
            viewModelEvent.Schedules = CreateEventSchedules(item, dynamicsEvent);
            // create TUA event locations - if any
            viewModelEvent.EventLocations = CreateEventLocations(item, dynamicsEvent);

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
                return NotFound();
            }

            if (dynamicsEvent == null || !CurrentUserHasAccessToEventOwnedBy(dynamicsEvent.AdoxioAccount.Accountid))
            {
                return NotFound();
            }

            LicenceEvent result = dynamicsEvent.ToViewModel(_dynamicsClient);

            return new JsonResult(result);
        }

        /// <summary>
        /// Update an event if the user has access
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLicenceEvent([FromBody] LicenceEvent item, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            MicrosoftDynamicsCRMadoxioEvent dynamicsEvent = _dynamicsClient.GetEventByIdWithChildren(id);
            if (dynamicsEvent == null || !CurrentUserHasAccessToEventOwnedBy(dynamicsEvent.AdoxioAccount.Accountid))
            {
                return NotFound();
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
            if (!string.IsNullOrEmpty(item.LicenceId) && item.LicenceId != dynamicsEvent._adoxioLicenceValue)
            {
                patchEvent.LicenceODataBind = _dynamicsClient.GetEntityURI("adoxio_licenceses", item.LicenceId);
            }
            try
            {
                _dynamicsClient.Events.Update(id, patchEvent);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating event");
            }

            // Re-fetch event information from Dynamics to bring in updated properties
            dynamicsEvent = _dynamicsClient.GetEventByIdWithChildren(id);
            if (dynamicsEvent == null)
            {
                return NotFound();
            }

            /* Get current event schedules and delete */
            LicenceEvent viewModelEvent = dynamicsEvent.ToViewModel(_dynamicsClient);
            DeleteEventSchedules(viewModelEvent.Schedules);

            /* Create new event schedules */
            viewModelEvent.Schedules = CreateEventSchedules(item, dynamicsEvent);

            /* Delete current event locations (TUA events only) */
            DeleteEventLocations(viewModelEvent.EventLocations);

            /* Create new event locations (TUA events only) */
            viewModelEvent.EventLocations = CreateEventLocations(item, dynamicsEvent);

            return new JsonResult(dynamicsEvent.ToViewModel(_dynamicsClient));
        }

        private void DeleteEventSchedules(List<LicenceEventSchedule> schedules)
        {
            if (schedules != null && schedules.Count > 0)
            {
                foreach (var eventSchedule in schedules)
                {
                    _dynamicsClient.Eventschedules.Delete(eventSchedule.Id);
                }
            }
        }

        private List<LicenceEventSchedule> CreateEventSchedules(LicenceEvent payload, MicrosoftDynamicsCRMadoxioEvent dynamicsEvent)
        {
            var schedules = new List<LicenceEventSchedule>();
            if (payload.Schedules != null && payload.Schedules.Count > 0 && dynamicsEvent != null)
            {
                foreach (var eventSchedule in payload.Schedules)
                {
                    var patchObject = new MicrosoftDynamicsCRMadoxioEventschedule();
                    patchObject.CopyValues(eventSchedule);
                    patchObject.EventODataBind = _dynamicsClient.GetEntityURI("adoxio_events", dynamicsEvent.AdoxioEventid);
                    var newEventSchedule = _dynamicsClient.Eventschedules.Create(patchObject);
                    schedules.Add(newEventSchedule.ToViewModel());
                }
            }
            return schedules;
        }

        private void DeleteEventLocations(List<LicenceEventLocation> locations)
        {
            if (locations != null && locations.Count > 0)
            {
                foreach (var loc in locations)
                {
                    _dynamicsClient.Eventlocations.Delete(loc.Id);
                }
            }
        }

        private List<LicenceEventLocation> CreateEventLocations(LicenceEvent payload, MicrosoftDynamicsCRMadoxioEvent dynamicsEvent)
        {
            var locations = new List<LicenceEventLocation>();
            if (payload.EventLocations != null && payload.EventLocations.Count > 0 && dynamicsEvent != null)
            {
                foreach (var eventLocation in payload.EventLocations)
                {
                    var patchObject = new MicrosoftDynamicsCRMadoxioEventlocation();
                    patchObject.CopyValues(eventLocation);
                    patchObject.EventODataBind = _dynamicsClient.GetEntityURI("adoxio_events", dynamicsEvent.AdoxioEventid);
                    patchObject.ServiceAreaODataBind = _dynamicsClient.GetEntityURI("adoxio_serviceareas", eventLocation.ServiceAreaId);
                    var newLocation = _dynamicsClient.Eventlocations.Create(patchObject);
                    locations.Add(newLocation.ToViewModel());
                }
            }
            return locations;
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

            if (viewModelEvent.Schedules != null && viewModelEvent.Schedules.Count > 0)
            {
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
            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            MicrosoftDynamicsCRMadoxioEventCollection dynamicsEvents;
            List<LicenceEvent> responseEvents = new List<LicenceEvent>();

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
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

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
            Dictionary<string, string> serviceAreas;

            try
            {
                licenceEvent = _dynamicsClient.Events.GetByKey(eventId, expand: new List<string> { "adoxio_adoxio_event_adoxio_applicationtermscondi" });
                licenceEventVM = licenceEvent.ToViewModel(_dynamicsClient);
                licence = _dynamicsClient.Licenceses.GetByKey(
                    licenceEventVM.LicenceId,
                    expand: new List<string> { "adoxio_adoxio_licences_adoxio_applicationtermsconditionslimitation_Licence" });
                account = _dynamicsClient.Accounts.GetByKey(licence._adoxioLicenceeValue);
                var areas = LicenseExtensions.GetServiceAreas(licence.AdoxioLicencesid, _dynamicsClient);
                // Create lookup dictionary with service areas to speed up lookup times (vs an array)
                serviceAreas = areas.ToDictionary(x => x.Id, x => x.AreaLocation);
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
            TimeZoneInfo hwZone;
            try
            {
                hwZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
            }
            catch (TimeZoneNotFoundException)
            {
                hwZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            }

            foreach (var schedule in licenceEventVM.Schedules)
            {
                // Event times are stored in UTC but we want the printed PDF to reflect times and dates in Pacific Standard Time (PST)
                DateTime? pstStart = schedule.EventStartDateTime.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(schedule.EventStartDateTime.Value.DateTime, hwZone) : (DateTime?)null;
                DateTime? pstEnd = schedule.EventEndDateTime.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(schedule.EventEndDateTime.Value.DateTime, hwZone) : (DateTime?)null;
                DateTime? pstLiquorStart = schedule.ServiceStartDateTime.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(schedule.ServiceStartDateTime.Value.DateTime, hwZone) : (DateTime?)null;
                DateTime? pstLiquorEnd = schedule.ServiceEndDateTime.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(schedule.ServiceEndDateTime.Value.DateTime, hwZone) : (DateTime?)null;

                string eventDate = pstStart.HasValue ? pstStart.Value.ToString("MMMM dd, yyyy") : "";
                string startTime = pstStart.HasValue ? pstStart.Value.ToString("h:mm tt") : "";
                string endTime = pstEnd.HasValue ? pstEnd.Value.ToString("h:mm tt") : "";
                string liquorStartTime = pstLiquorStart.HasValue ? pstLiquorStart.Value.ToString("h:mm tt") : "";
                string liquorEndTime = pstLiquorEnd.HasValue ? pstLiquorEnd.Value.ToString("h:mm tt") : "";
                eventTimings += $@"<tr class='hide-border'>
                        <td style='width: 50%; text-align: left;'>{eventDate} - Event Hours: {startTime} to {endTime}</td>
                    </tr>";
            }

            var eventLocations = "";
            if (licenceEventVM.EventLocations.Count > 0)
            {
                eventLocations += $@"<table style='width: 100%'>
                    <thead>
                        <tr>
                            <th>Location ID</th>
                            <th>Location Name</th>
                            <th>Attendance</th>
                        </tr>
                    </thead>";
                foreach (var location in licenceEventVM.EventLocations)
                {
                    string area = serviceAreas.GetValueOrDefault(location.ServiceAreaId, "");
                    eventLocations += $@"<tr class='hide-border'>
                        <td style='width: 30%; text-align: left;'>{area}</td>
                        <td style='width: 50%; text-align: left;'>{location.Name ?? ""}</td>
                        <td style='width: 20%; text-align: left;'>{location.Attendance ?? 0}</td>
                    </tr>";
                }
                eventLocations += "</table>";
            }

            var termsAndConditions = "";
            //LCSD6247 - Market and Catering Event authorizations use event limitation preset
            if (licenceEventVM.EventCategory == EventCategory.Catering || licenceEventVM.EventCategory == EventCategory.Market)
            {
                if (licenceEvent.AdoxioAdoxioEventAdoxioApplicationtermscondi != null)
                {
                    foreach (var item in licenceEvent.AdoxioAdoxioEventAdoxioApplicationtermscondi)
                    {
                        //Get the preset id value and fetch preset from Dynamics
                        //Dynamics does not allow multi layer expand so needs to be an extra call.
                        if(item._adoxioTermsconditionspresetValue != null)
                        {
                            MicrosoftDynamicsCRMadoxioTermsconditionslimitationspreset tcpreset = _dynamicsClient.Termsconditionslimitationspresets.GetByKey(item._adoxioTermsconditionspresetValue);
                            if(tcpreset != null)
                            {
                                termsAndConditions += $"<li>{tcpreset.AdoxioContents.Replace("\n","<br/>")}</li>";
                            } 
                        }
                    }
                }
            }
            else
            {
                foreach (var item in licence.AdoxioAdoxioLicencesAdoxioApplicationtermsconditionslimitationLicence)
                {
                    termsAndConditions += $"<li>{item.AdoxioTermsandconditions}</li>";
                }
            }

            var parameters = new Dictionary<string, string>
            {
                { "licensee", account.Name },
                { "licenceNumber", licence.AdoxioLicencenumber },
                { "licenceExpiryDate", licence.AdoxioExpirydate?.ToString("MMMM dd, yyyy") },
                { "licenseePhone", account.Telephone1 },
                { "licenseeEmail", account.Emailaddress1 },
                { "contactName", licenceEventVM.ContactName },
                { "contactEmail", licenceEventVM.ContactEmail },
                { "contactPhone", licenceEventVM.ContactPhone },
                { "hostname", licenceEventVM.ClientHostname },
                { "startDate", licenceEventVM.StartDate?.ToString("MMMM dd, yyyy") },
                { "endDate", licenceEventVM.EndDate?.ToString("MMMM dd, yyyy") },
                { "eventName", licenceEventVM.EventName },
                { "eventTimings", eventTimings },
                { "eventNumber", licenceEventVM.EventNumber },
                { "eventType", licenceEvent.AdoxioTuaeventtype.HasValue ? EnumExtensions.GetEnumMemberValue((TuaEventType?)licenceEvent.AdoxioTuaeventtype) : ""},
                { "eventDescription", licenceEventVM.EventTypeDescription },
                { "foodService", licenceEventVM.FoodService.HasValue ? EnumExtensions.GetEnumMemberValue(licenceEventVM.FoodService) : "" },
                { "entertainment", licenceEventVM.Entertainment.HasValue ? EnumExtensions.GetEnumMemberValue(licenceEventVM.Entertainment) : "" },
                { "attendance", licenceEventVM.MaxAttendance.ToString() },
                { "minors", licenceEventVM.MinorsAttending ?? false ? "Yes" : "No" },
                { "location", licenceEventVM.SpecificLocation.ToString() },
                { "addressLine1", licenceEventVM.Street1 },
                { "addressLine2", licenceEventVM.Street2 },
                { "addressLine3", $"{licenceEventVM.City}, BC {licenceEventVM.PostalCode}" },
                { "inspectorName", licenceEvent.AdoxioEventinspectorname },
                { "inspectorPhone", licenceEvent.AdoxioEventinspectorphone },
                { "inspectorEmail", licenceEvent.AdoxioEventinspectoremail },
                { "date", DateTime.Now.ToString("MMMM dd, yyyy") },
                { "marketName", licenceEventVM.MarketName },
                { "marketDuration",  licenceEventVM.MarketDuration.HasValue ? EnumExtensions.GetEnumMemberValue(licenceEventVM.MarketDuration) : "" },
                { "restrictionsText", termsAndConditions },
                // TUA-specific fields
                { "tuaEventType", licenceEventVM.TuaEventType.HasValue ? EnumExtensions.GetEnumMemberValue(licenceEventVM.TuaEventType) : ""},
                { "isClosedToPublic", licenceEventVM.IsClosedToPublic ?? false ? "Yes" : "No" },
                { "isWedding", licenceEventVM.IsWedding ?? false ? "1" : null},
                { "isNetworkingParty", licenceEventVM.IsNetworkingParty ?? false ? "1" : null},
                { "isConcert", licenceEventVM.IsConcert ?? false ? "1" : null},
                { "isNoneOfTheAbove", licenceEventVM.IsNoneOfTheAbove ?? false ? "1" : null},
                { "isBanquet", licenceEventVM.IsBanquet ?? false ? "1" : null},
                { "isAmplifiedSound", licenceEventVM.IsAmplifiedSound ?? false ? "1" : null},
                { "isDancing", licenceEventVM.IsDancing ?? false ? "1" : null},
                { "isReception", licenceEventVM.IsReception ?? false ? "1" : null},
                { "isLiveEntertainment", licenceEventVM.IsLiveEntertainment ?? false ? "1" : null},
                { "isGambling", licenceEventVM.IsGambling ?? false ? "1" : null},
                { "eventLocations", eventLocations },
            };

            byte[] data;
            try
            {
                string pdfType = null;
                if (licenceEventVM.EventCategory == EventCategory.Market)
                {
                    pdfType = "market_event_authorization";
                }
                else if (licenceEventVM.EventCategory == EventCategory.Catering)
                {
                    pdfType = "catering_event_authorization";
                }
                else if (licenceEventVM.EventCategory == EventCategory.TemporaryUseArea)
                {
                    pdfType = "tua_event_authorization";
                }
                if (pdfType != null)
                {
                    data = await _pdfClient.GetPdf(parameters, pdfType).ConfigureAwait(true);

                    // Save copy of generated licence PDF for auditing/logging purposes
                    try
                    {
                        var hash = await _pdfClient.GetPdfHash(parameters, pdfType);
                        var entityName = "event";
                        var entityId = eventId;
                        var folderName = await _dynamicsClient.GetFolderName(entityName, entityId).ConfigureAwait(true);
                        var documentType = "EventAuthorization";
                        _fileManagerClient.UploadPdfIfChanged(_logger, entityName, entityId, folderName, documentType, data, hash);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error uploading PDF");
                    }

                    return File(data, "application/pdf", "authorization.pdf");
                }
                return new NotFoundResult();
            }
            catch (Exception)
            {
                return new NotFoundResult();
            }
        }
    }
}
