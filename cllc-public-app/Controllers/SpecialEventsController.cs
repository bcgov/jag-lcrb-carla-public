using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/special-events")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class SpecialEventsController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IWebHostEnvironment _env;
        private readonly FileManagerClient _fileManagerClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IBCEPService _bcep;

        public SpecialEventsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, FileManagerClient fileClient, IBCEPService bcep,
            IWebHostEnvironment env, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(ApplicationsController));
            _fileManagerClient = fileClient;
            _env = env;
            _bcep = bcep;
        }

        /// <summary>
        ///     GET a special event by id
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("{eventId}")]
        public IActionResult GetSpecialEvent(string eventId)
        {
            var expand = new List<string> { };
            MicrosoftDynamicsCRMadoxioSpecialevent specialEvent = null;
            if (string.IsNullOrEmpty(eventId))
            {
                var filter = $"_adoxio_applicant_value eq {eventId}";

                try
                {
                    specialEvent = _dynamicsClient.Specialevents.Get(filter: filter, expand: expand, orderby: new List<string> { "modifiedon desc" }).Value.FirstOrDefault();
                }
                catch (HttpOperationException)
                {
                    specialEvent = null;
                }
            }
            return new JsonResult(specialEvent);
        }

        [HttpPost]
        public IActionResult CreateSpecialEvent([FromBody] ViewModels.SpecialEvent specialEvent)
        {
            if (specialEvent == null)
            {
                return BadRequest();
            }

            var newSpecialEvent = new MicrosoftDynamicsCRMadoxioSpecialevent();
            newSpecialEvent.CopyValues(specialEvent);
            try
            {
                newSpecialEvent = _dynamicsClient.Specialevents.Create(newSpecialEvent);
                specialEvent.SpecialEventId = newSpecialEvent.AdoxioSpecialeventid;
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating special event");
                throw httpOperationException;
            }

            if (specialEvent.EventLocations?.Count > 0)
            {
                newSpecialEvent.AdoxioSpecialeventSpecialeventlocations = new List<MicrosoftDynamicsCRMadoxioSpecialeventlocation>();
                // add locations to the new special event
                specialEvent.EventLocations.ForEach(location =>
                {
                    var newLocation = new MicrosoftDynamicsCRMadoxioSpecialeventlocation();
                    newLocation.CopyValues(location);
                    newLocation.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", newSpecialEvent.AdoxioSpecialeventid);
                    newSpecialEvent.AdoxioSpecialeventSpecialeventlocations.Add(newLocation);
                    try
                    {
                        newLocation = _dynamicsClient.Specialeventlocations.Create(newLocation);
                        location.LocationId = newLocation.AdoxioSpecialeventlocationid;
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error creating special event location");
                        throw httpOperationException;
                    }

                    // Add service areas to new location
                    if (location.ServiceAreas?.Count > 0)
                    {
                        newLocation.AdoxioSpecialeventlocationLicencedareas = new List<MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea>();
                        location.ServiceAreas.ForEach(area =>
                        {
                            var newArea = new MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea();
                            newArea.CopyValues(area);
                            newArea.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", newSpecialEvent.AdoxioSpecialeventid);
                            newArea.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", newLocation.AdoxioSpecialeventlocationid);
                            try
                            {
                                newArea = _dynamicsClient.Specialeventlicencedareas.Create(newArea);
                                area.LicencedAreaId = newArea.AdoxioSpecialeventlicencedareaid;
                            }
                            catch (HttpOperationException httpOperationException)
                            {
                                _logger.LogError(httpOperationException, "Error creating special event location");
                                throw httpOperationException;
                            }
                            newLocation.AdoxioSpecialeventlocationLicencedareas.Add(newArea);

                            // Add event dates to the new Area
                            if (area.EventDates?.Count > 0)
                            {
                                area.EventDates.ForEach(dates =>
                                {
                                    var newDates = new MicrosoftDynamicsCRMadoxioSpecialeventschedule();
                                    newDates.CopyValues(dates);
                                    newDates.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", newSpecialEvent.AdoxioSpecialeventid);
                                    newDates.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", newLocation.AdoxioSpecialeventlocationid);
                                    newDates.AdoxioServiceAreaODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlicencedareas", newArea.AdoxioSpecialeventlicencedareaid);
                                    try
                                    {
                                        newDates = _dynamicsClient.Specialeventschedules.Create(newDates);
                                        dates.SpecialEventId = newDates.AdoxioSpecialeventscheduleid;
                                    }
                                    catch (HttpOperationException httpOperationException)
                                    {
                                        _logger.LogError(httpOperationException, "Error creating special event location");
                                        throw httpOperationException;
                                    }
                                });
                            }
                        });
                    }
                });
            }
            return new JsonResult(specialEvent);
        }

        [HttpPost("old")]
        public IActionResult CreateSpecialEventOld([FromBody] ViewModels.SpecialEvent specialEvent)
        {
            if (specialEvent == null)
            {
                return BadRequest();
            }

            var newSpecialEvent = new MicrosoftDynamicsCRMadoxioSpecialevent();
            newSpecialEvent.CopyValues(specialEvent);

            if (specialEvent.EventLocations?.Count > 0)
            {
                newSpecialEvent.AdoxioSpecialeventSpecialeventlocations = new List<MicrosoftDynamicsCRMadoxioSpecialeventlocation>();
                // add locations to the new special event
                specialEvent.EventLocations.ForEach(location =>
                {
                    var newLocation = new MicrosoftDynamicsCRMadoxioSpecialeventlocation();
                    newLocation.CopyValues(location);
                    newSpecialEvent.AdoxioSpecialeventSpecialeventlocations.Add(newLocation);

                    // Add service areas to new location
                    if (location.ServiceAreas?.Count > 0)
                    {
                        newLocation.AdoxioSpecialeventlocationLicencedareas = new List<MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea>();
                        location.ServiceAreas.ForEach(area =>
                        {
                            var newArea = new MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea()
                            {
                                AdoxioSpecialeventareaEventschedules = new List<MicrosoftDynamicsCRMadoxioSpecialeventschedule>()
                            };
                            newArea.CopyValues(area);
                            newLocation.AdoxioSpecialeventlocationLicencedareas.Add(newArea);

                            // Add event dates to the new Area
                            if (area.EventDates?.Count > 0)
                            {
                                area.EventDates.ForEach(dates =>
                                {
                                    var newDates = new MicrosoftDynamicsCRMadoxioSpecialeventschedule();
                                    newDates.CopyValues(dates);
                                    newArea.AdoxioSpecialeventareaEventschedules.Add(newDates);
                                });
                            }
                        });
                    }
                });
            }

            //// get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // patchEvent.AdoxioApplicantODataBind =
            //         _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId);

            try
            {
                newSpecialEvent = _dynamicsClient.Specialevents.Create(newSpecialEvent);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating special event");
                throw httpOperationException;
            }

            return new JsonResult(newSpecialEvent.ToViewModel());
        }

        [HttpPut("{eventId}")]
        public IActionResult UpdateSpecialEvent(string eventId, [FromBody] ViewModels.SpecialEvent specialEvent)
        {
            if (String.IsNullOrEmpty(eventId) || eventId != specialEvent?.SpecialEventId)
            {
                return BadRequest();
            }

            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent();
            patchEvent.CopyValues(specialEvent);
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.SpecialEventId, patchEvent);
                specialEvent.SpecialEventId = patchEvent.AdoxioSpecialeventid;
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating/updating special event");
                throw httpOperationException;
            }

            if (specialEvent.EventLocations?.Count > 0)
            {
                patchEvent.AdoxioSpecialeventSpecialeventlocations = new List<MicrosoftDynamicsCRMadoxioSpecialeventlocation>();
                // add locations to the new special event
                specialEvent.EventLocations.ForEach(location =>
                {
                    var newLocation = new MicrosoftDynamicsCRMadoxioSpecialeventlocation();
                    newLocation.CopyValues(location);
                    newLocation.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.SpecialEventId);
                    try
                    {

                        if (string.IsNullOrEmpty(location.LocationId))
                        { // create record
                            newLocation = _dynamicsClient.Specialeventlocations.Create(newLocation);
                            location.LocationId = newLocation.AdoxioSpecialeventlocationid;
                        }
                        else
                        { // update record
                            _dynamicsClient.Specialeventlocations.Update(location.LocationId, newLocation);
                        }


                        // Add service areas to new location
                        if (location.ServiceAreas?.Count > 0)
                        {
                            newLocation.AdoxioSpecialeventlocationLicencedareas = new List<MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea>();
                            location.ServiceAreas.ForEach(area =>
                            {
                                var newArea = new MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea();
                                newArea.CopyValues(area);
                                newArea.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.SpecialEventId);
                                newArea.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", location.LocationId);
                                try
                                {
                                    if (string.IsNullOrEmpty(area.LicencedAreaId))
                                    { // create record
                                    newArea = _dynamicsClient.Specialeventlicencedareas.Create(newArea);
                                        area.LicencedAreaId = newArea.AdoxioSpecialeventlicencedareaid;
                                    }
                                    else
                                    { // update record
                                    _dynamicsClient.Specialeventlicencedareas.Update(area.LicencedAreaId, newArea);
                                        newArea.AdoxioSpecialeventlicencedareaid = area.LicencedAreaId;
                                    }


                                // Add event dates to the new Area
                                if (area.EventDates?.Count > 0)
                                    {
                                        area.EventDates.ForEach(dates =>
                                        {
                                            var newDates = new MicrosoftDynamicsCRMadoxioSpecialeventschedule();
                                            newDates.CopyValues(dates);
                                            newDates.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.SpecialEventId);
                                            newDates.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", location.LocationId);
                                            newDates.AdoxioServiceAreaODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlicencedareas", area.LicencedAreaId);
                                            try
                                            {
                                                if (string.IsNullOrEmpty(dates.EventScheduleId))
                                                { // create record
                                                newDates = _dynamicsClient.Specialeventschedules.Create(newDates);
                                                    dates.SpecialEventId = newDates.AdoxioSpecialeventscheduleid;
                                                }
                                                else
                                                { // update record
                                                _dynamicsClient.Specialeventschedules.Update(dates.EventScheduleId, newDates);
                                                }
                                            }
                                            catch (HttpOperationException httpOperationException)
                                            {
                                                _logger.LogError(httpOperationException, "Error creating/updating special event location");
                                                throw httpOperationException;
                                            }
                                        });
                                    }

                                }
                                catch (HttpOperationException httpOperationException)
                                {
                                    _logger.LogError(httpOperationException, "Error creating special event location");
                                    throw httpOperationException;
                                }
                            });
                        }

                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error creating/updating special event location");
                        throw httpOperationException;
                    }
                });
            }
            return new JsonResult(specialEvent);
        }

        [HttpGet("drink-types")]
        public IActionResult GetDrinkTypes()
        {
            List<ViewModels.SepDrinkType> result = new List<ViewModels.SepDrinkType>();
            var drinkTypes = _dynamicsClient.Sepdrinktypes.Get().Value;
            foreach (var item in drinkTypes)
            {
                result.Add(item.ToViewModel());
            }
            return new JsonResult(result);
        }
    }
}
