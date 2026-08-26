extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Gov.Lclb.Cllb.Public.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IDataverseClient _dataverse;
        private readonly ILogger _logger;
        private readonly IPdfService _pdfClient;
        private readonly FileManagerClient _fileManagerClient;

        public LicenceEventsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDataverseClient dataverse, IPdfService pdfClient, FileManagerClient fileClient)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dataverse = dataverse;
            _logger = loggerFactory.CreateLogger(typeof(LicenceEventsController));
            _pdfClient = pdfClient;
            _fileManagerClient = fileClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLicenceEvent([FromBody] LicenceEvent item)
        {
            if (item?.Status == LicenceEventStatus.Submitted)
            {
                bool alwaysAuthorization;
                try
                {
                    var licence = await _dataverse.GetLicenceByIdAsync(item.LicenceId);
                    if (licence == null) return BadRequest();
                    alwaysAuthorization = licence.adoxio_IsEventApprovalAlwaysRequired ?? false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating event");
                    return BadRequest();
                }
                item.EventClass = item.DetermineEventClass(alwaysAuthorization);
                if (item.EventClass != EventClass.Authorization || item.EventCategory == EventCategory.Market)
                    item.Status = LicenceEventStatus.Approved;
                else
                    item.Status = LicenceEventStatus.InReview;
            }

            var dynamicsEvent = new adoxio_event();
            dynamicsEvent.CopyValues(item);

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            dynamicsEvent.adoxio_Account = new EntityReference(DV::Gov.Lclb.Cllb.Interfaces.Account.EntityLogicalName, Guid.Parse(userSettings.AccountId));

            if (!string.IsNullOrEmpty(item.LicenceId))
                dynamicsEvent.adoxio_Licence = new EntityReference(adoxio_licences.EntityLogicalName, Guid.Parse(item.LicenceId));

            Guid eventId;
            try
            {
                eventId = await _dataverse.CreateEventAsync(dynamicsEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return BadRequest();
            }

            await CreateEventSchedulesAsync(item, eventId);
            await CreateEventLocationsAsync(item, eventId);

            var createdEvent = await _dataverse.GetEventByIdAsync(eventId.ToString());
            var createdSchedules = await _dataverse.GetEventSchedulesByEventIdAsync(eventId.ToString());
            var createdLocations = await _dataverse.GetEventLocationsByEventIdAsync(eventId.ToString());

            return new JsonResult(createdEvent?.ToViewModel(createdSchedules, createdLocations));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLicenceEvent(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            adoxio_event dynamicsEvent;
            try
            {
                dynamicsEvent = await _dataverse.GetEventByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Event");
                return NotFound();
            }

            if (dynamicsEvent == null || !CurrentUserHasAccessToEventOwnedBy(dynamicsEvent.adoxio_Account?.Id.ToString()))
                return NotFound();

            var schedules = await _dataverse.GetEventSchedulesByEventIdAsync(id);
            var locations = await _dataverse.GetEventLocationsByEventIdAsync(id);
            var result = dynamicsEvent.ToViewModel(schedules, locations);

            return new JsonResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLicenceEvent([FromBody] LicenceEvent item, string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var dynamicsEvent = await _dataverse.GetEventByIdAsync(id);
            if (dynamicsEvent == null || !CurrentUserHasAccessToEventOwnedBy(dynamicsEvent.adoxio_Account?.Id.ToString()))
                return NotFound();

            if (item?.SecurityPlanSubmitted == null && item?.Status == LicenceEventStatus.Submitted)
            {
                bool alwaysAuthorization;
                try
                {
                    var licence = await _dataverse.GetLicenceByIdAsync(item.LicenceId);
                    if (licence == null) return BadRequest();
                    alwaysAuthorization = licence.adoxio_IsEventApprovalAlwaysRequired ?? false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating event");
                    return BadRequest();
                }
                item.EventClass = item.DetermineEventClass(alwaysAuthorization);
                if (item.EventClass != EventClass.Authorization)
                    item.Status = LicenceEventStatus.Approved;
                else
                    item.Status = LicenceEventStatus.InReview;
            }

            var patchEvent = new adoxio_event { Id = Guid.Parse(id) };
            patchEvent.CopyValues(item);
            if (!string.IsNullOrEmpty(item.LicenceId) && item.LicenceId != dynamicsEvent.adoxio_Licence?.Id.ToString())
                patchEvent.adoxio_Licence = new EntityReference(adoxio_licences.EntityLogicalName, Guid.Parse(item.LicenceId));

            try
            {
                await _dataverse.UpdateEventAsync(patchEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event");
            }

            var currentSchedules = await _dataverse.GetEventSchedulesByEventIdAsync(id);
            await DeleteEventSchedulesAsync(currentSchedules);
            await CreateEventSchedulesAsync(item, Guid.Parse(id));

            var currentLocations = await _dataverse.GetEventLocationsByEventIdAsync(id);
            await DeleteEventLocationsAsync(currentLocations);
            await CreateEventLocationsAsync(item, Guid.Parse(id));

            var updatedEvent = await _dataverse.GetEventByIdAsync(id);
            if (updatedEvent == null) return NotFound();

            var freshSchedules = await _dataverse.GetEventSchedulesByEventIdAsync(id);
            var freshLocations = await _dataverse.GetEventLocationsByEventIdAsync(id);
            return new JsonResult(updatedEvent.ToViewModel(freshSchedules, freshLocations));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLicenceEvent(string id)
        {
            if (string.IsNullOrEmpty(id)) return new BadRequestResult();

            adoxio_event dynamicsEvent;
            try
            {
                dynamicsEvent = await _dataverse.GetEventByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete event");
                return new NotFoundResult();
            }

            if (dynamicsEvent == null || !CurrentUserHasAccessToEventOwnedBy(dynamicsEvent.adoxio_Account?.Id.ToString()))
                return new NotFoundResult();

            var schedules = await _dataverse.GetEventSchedulesByEventIdAsync(id);
            await DeleteEventSchedulesAsync(schedules);

            await _dataverse.DeleteEventAsync(id);
            return NoContent();
        }

        [HttpGet("list/{licenceId}/{num}")]
        public async Task<IActionResult> GetLicenceEventsList(string licenceId, int num)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            IList<adoxio_event> dynamicsEvents;
            try
            {
                dynamicsEvents = await _dataverse.GetEventsByAccountAndLicenceAsync(userSettings.AccountId, licenceId, num);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Events");
                return new NotFoundResult();
            }

            var responseEventTasks = dynamicsEvents.Select(async evt =>
            {
                var evtId = evt.adoxio_eventId?.ToString();
                var schedulesTask = evtId != null ? _dataverse.GetEventSchedulesByEventIdAsync(evtId) : Task.FromResult<IList<adoxio_eventschedule>>(new List<adoxio_eventschedule>());
                var locationsTask = evtId != null ? _dataverse.GetEventLocationsByEventIdAsync(evtId) : Task.FromResult<IList<adoxio_eventlocation>>(new List<adoxio_eventlocation>());
                await Task.WhenAll(schedulesTask, locationsTask);
                return evt.ToViewModel(schedulesTask.Result, locationsTask.Result);
            });

            return new JsonResult(await Task.WhenAll(responseEventTasks));
        }

        [HttpPost("list/batch/{num}")]
        public async Task<IActionResult> GetLicenceEventsListBatch(int num, [FromBody] List<string> licenceIds)
        {
            if (licenceIds == null || licenceIds.Count == 0) return new JsonResult(new List<LicenceEvent>());

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            IList<adoxio_event> dynamicsEvents;
            try
            {
                dynamicsEvents = await _dataverse.GetEventsByAccountAndLicencesAsync(userSettings.AccountId, licenceIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Events");
                return new NotFoundResult();
            }

            // cap at `num` most-recent events per licence, matching the per-licence endpoint's behaviour
            var trimmedEvents = dynamicsEvents
                .GroupBy(evt => evt.adoxio_Licence?.Id)
                .SelectMany(g => g.OrderByDescending(evt => evt.ModifiedOn).Take(num));

            var responseEventTasks = trimmedEvents.Select(async evt =>
            {
                var evtId = evt.adoxio_eventId?.ToString();
                var schedulesTask = evtId != null ? _dataverse.GetEventSchedulesByEventIdAsync(evtId) : Task.FromResult<IList<adoxio_eventschedule>>(new List<adoxio_eventschedule>());
                var locationsTask = evtId != null ? _dataverse.GetEventLocationsByEventIdAsync(evtId) : Task.FromResult<IList<adoxio_eventlocation>>(new List<adoxio_eventlocation>());
                await Task.WhenAll(schedulesTask, locationsTask);
                return evt.ToViewModel(schedulesTask.Result, locationsTask.Result);
            });

            return new JsonResult(await Task.WhenAll(responseEventTasks));
        }

        [HttpGet("{eventId}/authorization.pdf")]
        public async Task<IActionResult> GetAuthorizationPdf(string eventId)
        {
            adoxio_event licenceEvent;
            LicenceEvent licenceEventVM;
            adoxio_licences licence;
            DV::Gov.Lclb.Cllb.Interfaces.Account account;
            Dictionary<string, string> serviceAreas;

            try
            {
                licenceEvent = await _dataverse.GetEventByIdAsync(eventId);
                if (licenceEvent == null) return new NotFoundResult();

                var schedules = await _dataverse.GetEventSchedulesByEventIdAsync(eventId);
                var locations = await _dataverse.GetEventLocationsByEventIdAsync(eventId);
                licenceEventVM = licenceEvent.ToViewModel(schedules, locations);

                licence = await _dataverse.GetLicenceByIdAsync(licenceEventVM.LicenceId);
                if (licence == null) return new NotFoundResult();

                var licenceeId = licence.adoxio_Licencee?.Id.ToString();
                account = await _dataverse.GetAccountByIdAsync(licenceeId);
                if (account == null) return new NotFoundResult();

                var areas = await LicenseExtensions.GetServiceAreasAsync(licence.adoxio_licencesId?.ToString(), _dataverse);
                serviceAreas = areas.ToDictionary(x => x.Id, x => x.AreaLocation);
            }
            catch (Exception)
            {
                return new NotFoundResult();
            }

            if (!CurrentUserHasAccessToEventOwnedBy(licence.adoxio_Licencee?.Id.ToString()))
                return new NotFoundResult();

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
                DateTime? pstStart = schedule.EventStartDateTime.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(schedule.EventStartDateTime.Value.DateTime, hwZone) : (DateTime?)null;
                DateTime? pstEnd = schedule.EventEndDateTime.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(schedule.EventEndDateTime.Value.DateTime, hwZone) : (DateTime?)null;

                string eventDate = pstStart.HasValue ? pstStart.Value.ToString("MMMM dd, yyyy") : "";
                string startTime = pstStart.HasValue ? pstStart.Value.ToString("h:mm tt") : "";
                string endTime = pstEnd.HasValue ? pstEnd.Value.ToString("h:mm tt") : "";
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
            if (licenceEventVM.EventCategory == EventCategory.Catering || licenceEventVM.EventCategory == EventCategory.Market)
            {
                var eventTCs = await _dataverse.GetTermsConditionsByEventIdAsync(eventId);
                foreach (var item in eventTCs)
                {
                    if (item.adoxio_TermsConditionsPreset?.Id != null)
                    {
                        var tcpreset = await _dataverse.GetTermsConditionsPresetByIdAsync(item.adoxio_TermsConditionsPreset.Id.ToString());
                        if (tcpreset != null)
                            termsAndConditions += $"<li>{tcpreset.adoxio_Contents?.Replace("\n", "<br/>")}</li>";
                    }
                }
            }
            else
            {
                var licenceTCs = await _dataverse.GetTermsConditionsByLicenceIdAsync(licence.adoxio_licencesId?.ToString());
                foreach (var item in licenceTCs)
                    termsAndConditions += $"<li>{item.adoxio_TermsandConditions}</li>";
            }

            var parameters = new Dictionary<string, string>
            {
                { "licensee", account.Name },
                { "licenceNumber", licence.adoxio_LicenceNumber },
                { "licenceExpiryDate", licence.adoxio_ExpiryDate?.ToString("MMMM dd, yyyy") },
                { "licenseePhone", account.Telephone1 },
                { "licenseeEmail", account.EMailAddress1 },
                { "contactName", licenceEventVM.ContactName },
                { "contactEmail", licenceEventVM.ContactEmail },
                { "contactPhone", licenceEventVM.ContactPhone },
                { "hostname", licenceEventVM.ClientHostname },
                { "startDate", licenceEventVM.StartDate?.ToString("MMMM dd, yyyy") },
                { "endDate", licenceEventVM.EndDate?.ToString("MMMM dd, yyyy") },
                { "eventName", licenceEventVM.EventName },
                { "eventTimings", eventTimings },
                { "eventNumber", licenceEventVM.EventNumber },
                { "eventType", licenceEvent.adoxio_TUAEventType.HasValue ? EnumExtensions.GetEnumMemberValue((TuaEventType?)(int)licenceEvent.adoxio_TUAEventType.Value) : ""},
                { "eventDescription", licenceEventVM.EventTypeDescription },
                { "foodService", licenceEventVM.FoodService.HasValue ? EnumExtensions.GetEnumMemberValue(licenceEventVM.FoodService) : "" },
                { "entertainment", licenceEventVM.Entertainment.HasValue ? EnumExtensions.GetEnumMemberValue(licenceEventVM.Entertainment) : "" },
                { "attendance", licenceEventVM.MaxAttendance.ToString() },
                { "minors", licenceEventVM.MinorsAttending ?? false ? "Yes" : "No" },
                { "location", licenceEventVM.SpecificLocation.ToString() },
                { "addressLine1", licenceEventVM.Street1 },
                { "addressLine2", licenceEventVM.Street2 },
                { "addressLine3", $"{licenceEventVM.City}, BC {licenceEventVM.PostalCode}" },
                { "inspectorName", licenceEvent.adoxio_EventInspectorName },
                { "inspectorPhone", licenceEvent.adoxio_EventInspectorPhone },
                { "inspectorEmail", licenceEvent.adoxio_EventInspectorEmail },
                { "date", DateTime.Now.ToString("MMMM dd, yyyy") },
                { "marketName", licenceEventVM.MarketName },
                { "marketDuration", licenceEventVM.MarketDuration.HasValue ? EnumExtensions.GetEnumMemberValue(licenceEventVM.MarketDuration) : "" },
                { "restrictionsText", termsAndConditions },
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
                    pdfType = "market_event_authorization";
                else if (licenceEventVM.EventCategory == EventCategory.Catering)
                    pdfType = "catering_event_authorization";
                else if (licenceEventVM.EventCategory == EventCategory.TemporaryUseArea)
                    pdfType = "tua_event_authorization";

                if (pdfType != null)
                {
                    data = await _pdfClient.GetPdf(parameters, pdfType).ConfigureAwait(true);

                    try
                    {
                        var hash = await _pdfClient.GetPdfHash(parameters, pdfType);
                        var entityName = "event";
                        var folderName = await _dataverse.GetFolderNameAsync(entityName, eventId);
                        var documentType = "EventAuthorization";
                        _fileManagerClient.UploadPdfIfChanged(_logger, entityName, eventId, folderName, documentType, data, hash);
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

        private bool CurrentUserHasAccessToEventOwnedBy(string accountId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
                return userSettings.AccountId == accountId;
            return false;
        }

        private async Task DeleteEventSchedulesAsync(IList<adoxio_eventschedule> schedules)
        {
            if (schedules == null) return;
            foreach (var s in schedules)
                await _dataverse.DeleteEventScheduleAsync(s.adoxio_eventscheduleId?.ToString());
        }

        private async Task<List<LicenceEventSchedule>> CreateEventSchedulesAsync(LicenceEvent payload, Guid eventId)
        {
            var result = new List<LicenceEventSchedule>();
            if (payload.Schedules == null || payload.Schedules.Count == 0) return result;

            foreach (var schedule in payload.Schedules)
            {
                var entity = new adoxio_eventschedule();
                entity.CopyValues(schedule);
                entity.adoxio_EventId = new EntityReference(adoxio_event.EntityLogicalName, eventId);
                var newId = await _dataverse.CreateEventScheduleAsync(entity);
                entity.adoxio_eventscheduleId = newId;
                result.Add(entity.ToViewModel());
            }
            return result;
        }

        private async Task DeleteEventLocationsAsync(IList<adoxio_eventlocation> locations)
        {
            if (locations == null) return;
            foreach (var l in locations)
                await _dataverse.DeleteEventLocationAsync(l.adoxio_eventlocationId?.ToString());
        }

        private async Task<List<LicenceEventLocation>> CreateEventLocationsAsync(LicenceEvent payload, Guid eventId)
        {
            var result = new List<LicenceEventLocation>();
            if (payload.EventLocations == null || payload.EventLocations.Count == 0) return result;

            foreach (var location in payload.EventLocations)
            {
                var entity = new adoxio_eventlocation();
                entity.CopyValues(location);
                entity.adoxio_EventId = new EntityReference(adoxio_event.EntityLogicalName, eventId);
                if (!string.IsNullOrEmpty(location.ServiceAreaId))
                    entity.adoxio_ServiceAreaId = new EntityReference(adoxio_servicearea.EntityLogicalName, Guid.Parse(location.ServiceAreaId));
                var newId = await _dataverse.CreateEventLocationAsync(entity);
                entity.adoxio_eventlocationId = newId;
                result.Add(entity.ToViewModel());
            }
            return result;
        }
    }
}
