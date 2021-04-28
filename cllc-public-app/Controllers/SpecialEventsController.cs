using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Extensions;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using Application = Gov.Lclb.Cllb.Public.ViewModels.Application;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
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

            var newSpecialEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                AdoxioSpecialeventSpecialeventlocations = new List<MicrosoftDynamicsCRMadoxioSpecialeventlocation>()
            };
            newSpecialEvent.CopyValues(specialEvent);

            if (specialEvent.EventLocations?.Count > 0)
            {
                // add locations to the new special event
                specialEvent.EventLocations.ForEach(location =>
                {
                    var newLocation = new MicrosoftDynamicsCRMadoxioSpecialeventlocation()
                    {
                        AdoxioSpecialeventlocationSchedule = new List<MicrosoftDynamicsCRMadoxioSpecialeventschedule>()
                    };
                    newLocation.CopyValues(location);
                    newSpecialEvent.AdoxioSpecialeventSpecialeventlocations.Add(newLocation);

                    // Add service areas to new location
                    if (location.ServiceAreas?.Count > 0)
                    {
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
            if (String.IsNullOrEmpty(eventId) || eventId != specialEvent?.Id)
            {
                return BadRequest();
            }

            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent();
            patchEvent.CopyValues(specialEvent);

            try
            {
                _dynamicsClient.Specialevents.Update(eventId, patchEvent);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating special event");
                throw httpOperationException;
            }

            patchEvent = _dynamicsClient.Specialevents.GetByKey(eventId);

            return new JsonResult(patchEvent.ToViewModel());
        }

    }
}
