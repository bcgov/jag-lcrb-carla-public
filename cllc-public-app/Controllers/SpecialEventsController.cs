using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System.Threading.Tasks;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/special-events")]
    [ApiController]
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
        private readonly IPdfService _pdfClient;

        public SpecialEventsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, FileManagerClient fileClient, IBCEPService bcep,
            IWebHostEnvironment env, IMemoryCache memoryCache, IPdfService pdfClient)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(ApplicationsController));
            _fileManagerClient = fileClient;
            _env = env;
            _bcep = bcep;
            _pdfClient = pdfClient;
        }

        // get summary list of applications past submission status
        [HttpGet("current/submitted")]
        public IActionResult GetCurrentSubmitted()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            string filter = $"(_adoxio_contactid_value eq {userSettings.ContactId}";
            filter += $" or _adoxio_accountid_value eq {userSettings.AccountId})";
            filter += $" and statuscode ne {(int)ViewModels.EventStatus.Draft}";
            filter += $" and statuscode ne {(int)ViewModels.EventStatus.Cancelled}";

            var result = GetSepSummaries(filter);

            return new JsonResult(result);
        }


        /// <summary>
        /// GET a special event by id.  Used by the police view event feature.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("{eventId}")]
        public IActionResult GetSpecialEvent(string eventId)
        {
            string[] expand = new[] { "adoxio_PoliceRepresentativeId",
                "adoxio_PoliceAccountId","adoxio_specialevent_specialeventlocations"
            };
            MicrosoftDynamicsCRMadoxioSpecialevent specialEvent = null;
            if (!string.IsNullOrEmpty(eventId))
            {
                try
                {
                    specialEvent = _dynamicsClient.Specialevents.GetByKey(eventId, expand: expand);
                }
                catch (HttpOperationException)
                {
                    //_logger.LogError($"Error retrieving special event: {eventId}.");
                    specialEvent = null;
                }
            }

            // get the applicant.

            if (specialEvent._adoxioContactidValue != null)
            {
                specialEvent.AdoxioContactId = _dynamicsClient.GetContactById(specialEvent._adoxioContactidValue).GetAwaiter().GetResult();
            }

            // get the city

            if (specialEvent._adoxioSpecialeventcitydistrictidValue != null)
            {
                specialEvent.AdoxioSpecialEventCityDistrictId = _dynamicsClient.GetSepCityById(specialEvent
                    ._adoxioSpecialeventcitydistrictidValue);
            }



            // event locations.

            foreach (var location in specialEvent.AdoxioSpecialeventSpecialeventlocations)
            {
                // get child elements.
                string filter = $"_adoxio_specialeventlocationid_value eq {location.AdoxioSpecialeventlocationid}";
                try
                {
                    location.AdoxioSpecialeventlocationLicencedareas = _dynamicsClient.Specialeventlicencedareas.Get(filter: filter).Value;

                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting location service areas.");
                }

                filter = $"_adoxio_specialeventlocationid_value eq {location.AdoxioSpecialeventlocationid}";
                try
                {
                    location.AdoxioSpecialeventlocationSchedule = _dynamicsClient.Specialeventschedules.Get(filter: filter).Value;

                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting location schedule.");
                }

            }

            var result = specialEvent.ToViewModel();

            if (specialEvent._adoxioLcrbrepresentativeidValue != null)
            {
                var lcrbDecisionBy = _dynamicsClient.GetUserAsViewModelContact(specialEvent._adoxioLcrbrepresentativeidValue);
                result.LcrbApprovalBy = lcrbDecisionBy;
                result.LcrbApproval = (ApproverStatus?)specialEvent.AdoxioLcrbapproval;
            }

            return new JsonResult(result);
        }


        /// <summary>
        ///     GET a special event by id. The detailed view of the application used by the client before submission and by the summary page
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("applicant/{eventId}")]
        public IActionResult GetSpecialEventForTheApplicant(string eventId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var specialEvent = this.getSpecialEventData(eventId);
            if (specialEvent._adoxioContactidValue != userSettings.ContactId && specialEvent._adoxioAccountidValue != userSettings.AccountId)
            {
                return Unauthorized();
            }
            return new JsonResult(specialEvent.ToViewModel());
        }

        /// <summary>
        ///     endpoint for a pdf
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("applicant/{eventId}/pdf/{filename}")]
        public async Task<IActionResult> GetSEPPDF(string eventId, string filename)
        {
            MicrosoftDynamicsCRMadoxioSpecialevent specialEvent = getSpecialEventData(eventId);

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            var title = "LCRB SPECIAL EVENTS PERMIT";
            var heading = "<h1>SPECIAL EVENT PERMIT</h1>";
            
            // if special event is submitted or approved, but not issued...
            if( 1 == 2) {
                heading = "<h1 class='error'>This is not your special event permit</h1>";
            }

            var locationDetails = "";

            // loop through the locations and show the related dates and service areas
            foreach(var location in specialEvent.AdoxioSpecialeventSpecialeventlocations) {

                // draw the location
                locationDetails += $"<h2 class='info'>Event Location: {location.AdoxioLocationname}</h2>";
                locationDetails += "<table class='info'>";
                locationDetails += $"<tr><th class='heading'>Location Name:</td><td class='field'>{location.AdoxioLocationname}</td></tr>";
                locationDetails += $"<tr><th class='heading'>Location Description:</td><td class='field'>{location.AdoxioLocationdescription}</td></tr>";
                locationDetails += $"<tr><th class='heading'>Event Address:</td><td class='field'>{location.AdoxioEventlocationstreet2} {location.AdoxioEventlocationstreet1}, {location.AdoxioEventlocationcity} BC, {location.AdoxioEventlocationpostalcode}</td></tr>";
                locationDetails += $"<tr><th class='heading'>No. Guests:</td><td class='field'>{location.AdoxioMaximumnumberofguestslocation}</td></tr>";
                locationDetails += $"<tr><th class='heading'>No. Minors:</td><td class='field'>{location.AdoxioNumberofminors}</td></tr>";
                locationDetails += "</table>";

                // show all service areas
                locationDetails += "<h3 class='info'>Service Area(s):</h2>";
                foreach(var sched in location.AdoxioSpecialeventlocationLicencedareas) {

                    var minors = sched.AdoxioMinorpresent.HasValue && sched.AdoxioMinorpresent == true ? "Yes" : "No";

                    locationDetails += "<table class='info'>";
                    locationDetails += $"<tr><th class='heading'>Description:</td><td class='field'>{sched.AdoxioEventname}</td></tr>";
                    locationDetails += $"<tr><th class='heading'>No. Guests in Service Area:</td><td class='field'>{sched.AdoxioLicencedareamaxnumberofguests}</td></tr>";
                    locationDetails += $"<tr><th class='heading'>Minors Present?:</td><td class='field'>{minors}</td></tr>";
                    locationDetails += $"<tr><th class='heading'>Setting:</td><td class='field'>{(ViewModels.ServiceAreaSetting?)sched.AdoxioSetting}</td></tr>";
                    locationDetails += "</table>";
                }

                // show all event dates
                locationDetails += "<h3 class='info'>Event Date(s):</h2>";
                foreach(var sched in location.AdoxioSpecialeventlocationSchedule) {

                    var startDateParam = "";
                    if (sched.AdoxioEventstart.HasValue) {
                        DateTime startDate = sched.AdoxioEventstart.Value.LocalDateTime;
                        startDateParam = startDate.ToString("MMMM dd, yyyy");
                    }

                    var eventTimeParam = "";
                    if(sched.AdoxioEventstart.HasValue && sched.AdoxioEventend.HasValue){
                        DateTime startTime = sched.AdoxioEventstart.Value.LocalDateTime;
                        DateTime endTime = sched.AdoxioEventend.Value.LocalDateTime;
                        eventTimeParam = startTime.ToString("t", CultureInfo.CreateSpecificCulture("en-US")) + " - " + endTime.ToString("t",CultureInfo.CreateSpecificCulture("en-US"));
                    }

                    var serviceTimeParam = "";
                    
                    if(sched.AdoxioServicestart.HasValue && sched.AdoxioServiceend.HasValue){
                        DateTime startTime = sched.AdoxioServicestart.Value.LocalDateTime;
                        DateTime endTime = sched.AdoxioServiceend.Value.LocalDateTime;
                        serviceTimeParam = startTime.ToString("t",CultureInfo.CreateSpecificCulture("en-US")) + " - " + endTime.ToString("t",CultureInfo.CreateSpecificCulture("en-US"));
                    }
                


                    locationDetails += "<table class='info'>";
                    locationDetails += $"<tr><th class='heading'>Date:</td><td class='field'>{startDateParam}</td></tr>";
                    locationDetails += $"<tr><th class='heading'>Event Times:</td><td class='field'>{eventTimeParam}</td></tr>";
                    locationDetails += $"<tr><th class='heading'>Service Times:</td><td class='field'>{serviceTimeParam}</td></tr>";
                    locationDetails += "</table>";
                }

            }

            parameters.Add("title", title);
            parameters.Add("heading", heading);
            parameters.Add("printDate", DateTime.Today.ToString("MMMM dd, yyyy"));
            parameters.Add("locationDetails", locationDetails);
            var templateName = "sep";

            byte[] data = await _pdfClient.GetPdf(parameters, templateName);

            // To Do; Save copy of generated sep PDF for auditing/logging purposes
            /*
            try
            {
                var hash = await _pdfClient.GetPdfHash(parameters, templateName);
                var entityName = "special event";
                var entityId = adoxioLicense.AdoxioLicencesid;
                var folderName = await _dynamicsClient.GetFolderName(entityName, entityId).ConfigureAwait(true);
                var documentType = "Licence";
                _fileManagerClient.UploadPdfIfChanged(_logger, entityName, entityId, folderName, documentType, data, hash);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error uploading PDF");
            }
            */

            return File(data, "sep/pdf", $"Special Event Permit - {specialEvent.AdoxioSpecialeventpermitnumber}.pdf");


            //return new UnauthorizedResult();
        }

        private MicrosoftDynamicsCRMadoxioSpecialevent getSpecialEventData(string eventId)
        {
            string[] expand = new[] {
                "adoxio_Invoice",
                "adoxio_specialevent_licencedarea",
                "adoxio_specialevent_schedule",
                "adoxio_specialevent_specialeventlocations",
                "adoxio_SpecialEventCityDistrictId",
                "adoxio_ContactId",
                "adoxio_AccountId",
                "adoxio_specialevent_adoxio_sepdrinksalesforecast_SpecialEvent"
            };
            MicrosoftDynamicsCRMadoxioSpecialevent specialEvent = null;
            if (!string.IsNullOrEmpty(eventId))
            {
                try
                {
                    specialEvent = _dynamicsClient.Specialevents.GetByKey(eventId, expand: expand);
                    var locations = specialEvent.AdoxioSpecialeventSpecialeventlocations;
                    var areas = specialEvent.AdoxioSpecialeventLicencedarea;
                    var schedules = specialEvent.AdoxioSpecialeventSchedule;

                    foreach (var schedule in schedules)
                    {
                        var parentLocation = locations.Where(loc => loc.AdoxioSpecialeventlocationid == schedule._adoxioSpecialeventlocationidValue).FirstOrDefault();
                        if (parentLocation.AdoxioSpecialeventlocationSchedule == null)
                        {
                            parentLocation.AdoxioSpecialeventlocationSchedule = new List<MicrosoftDynamicsCRMadoxioSpecialeventschedule>();
                        }
                        parentLocation.AdoxioSpecialeventlocationSchedule.Add(schedule);
                    }

                    foreach (var area in areas)
                    {
                        var parentLocation = locations.Where(loc => loc.AdoxioSpecialeventlocationid == area._adoxioSpecialeventlocationidValue).FirstOrDefault();
                        if (parentLocation.AdoxioSpecialeventlocationLicencedareas == null)
                        {
                            parentLocation.AdoxioSpecialeventlocationLicencedareas = new List<MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea>();
                        }
                        parentLocation.AdoxioSpecialeventlocationLicencedareas.Add(area);
                    }
                }
                catch (HttpOperationException)
                {
                    specialEvent = null;
                }
            }
            return specialEvent;
        }

        [HttpPost]
        public IActionResult CreateSpecialEvent([FromBody] ViewModels.SpecialEvent specialEvent)
        {
            if (specialEvent == null)
            {
                return BadRequest();
            }


            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);


            var newSpecialEvent = new MicrosoftDynamicsCRMadoxioSpecialevent();
            newSpecialEvent.CopyValues(specialEvent);
            newSpecialEvent.Statuscode = (int?)EventStatus.Draft;

            if (!string.IsNullOrEmpty(userSettings.AccountId) && userSettings.AccountId != "00000000-0000-0000-0000-000000000000")
            {
                newSpecialEvent.AccountODataBind = _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId);
            }
            if (!string.IsNullOrEmpty(userSettings.ContactId) && userSettings.ContactId != "00000000-0000-0000-0000-000000000000")
            {
                newSpecialEvent.ContactODataBind = _dynamicsClient.GetEntityURI("contacts", userSettings.ContactId);
            }

            if (!string.IsNullOrEmpty(specialEvent?.SepCity?.Id))
            {
                newSpecialEvent.SepCityODataBind = _dynamicsClient.GetEntityURI("adoxio_sepcities", specialEvent.SepCity.Id);
            }
            try
            {
                newSpecialEvent = _dynamicsClient.Specialevents.Create(newSpecialEvent);
                specialEvent.Id = newSpecialEvent.AdoxioSpecialeventid;
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating special event");
                throw httpOperationException;
            }

            if (specialEvent.EventLocations?.Count > 0)
            {
                // newSpecialEvent.AdoxioSpecialeventSpecialeventlocations = new List<MicrosoftDynamicsCRMadoxioSpecialeventlocation>();
                // add locations to the new special event
                specialEvent.EventLocations.ForEach((Action<ViewModels.SepEventLocation>)(location =>
                {
                    var newLocation = new MicrosoftDynamicsCRMadoxioSpecialeventlocation();
                    newLocation.CopyValues(location);
                    newLocation.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", newSpecialEvent.AdoxioSpecialeventid);
                    // newSpecialEvent.AdoxioSpecialeventSpecialeventlocations.Add(newLocation);
                    try
                    {
                        newLocation = _dynamicsClient.Specialeventlocations.Create(newLocation);
                        location.Id = newLocation.AdoxioSpecialeventlocationid;
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
                        location.ServiceAreas.ForEach((Action<ViewModels.SepServiceArea>)(area =>
                        {
                            var newArea = new MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea();
                            newArea.CopyValues(area);
                            newArea.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", newSpecialEvent.AdoxioSpecialeventid);
                            newArea.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", newLocation.AdoxioSpecialeventlocationid);
                            try
                            {
                                newArea = _dynamicsClient.Specialeventlicencedareas.Create(newArea);
                                area.Id = newArea.AdoxioSpecialeventlicencedareaid;
                            }
                            catch (HttpOperationException httpOperationException)
                            {
                                _logger.LogError(httpOperationException, "Error creating special event location");
                                throw httpOperationException;
                            }
                            newLocation.AdoxioSpecialeventlocationLicencedareas.Add(newArea);
                        }));
                    }

                    // Add event dates to location
                    if (location.EventDates?.Count > 0)
                    {
                        location.EventDates.ForEach(dates =>
                        {
                            var newDates = new MicrosoftDynamicsCRMadoxioSpecialeventschedule();
                            newDates.CopyValues(dates);
                            newDates.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", newSpecialEvent.AdoxioSpecialeventid);
                            newDates.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", newLocation.AdoxioSpecialeventlocationid);
                            try
                            {
                                newDates = _dynamicsClient.Specialeventschedules.Create(newDates);
                                dates.Id = newDates.AdoxioSpecialeventscheduleid;
                            }
                            catch (HttpOperationException httpOperationException)
                            {
                                _logger.LogError(httpOperationException, "Error creating special event location");
                                throw httpOperationException;
                            }
                        });
                    }
                }));
            }
            var result = this.getSpecialEventData(newSpecialEvent.AdoxioSpecialeventid).ToViewModel();
            result.LocalId = specialEvent.LocalId;
            return new JsonResult(specialEvent);
        }

        [HttpPut("{eventId}")]
        public IActionResult UpdateSpecialEvent(string eventId, [FromBody] ViewModels.SpecialEvent specialEvent)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(eventId) || eventId != specialEvent?.Id)
            {
                return BadRequest();
            }

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var existingEvent = getSpecialEventData(eventId);
            if (existingEvent._adoxioAccountidValue != userSettings.AccountId &&
               existingEvent._adoxioContactidValue != userSettings.ContactId)
            {
                return Unauthorized();
            }

            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent();
            patchEvent.CopyValues(specialEvent);
            // Only allow these status to be set by the portal. Any other status change is ignored
            if (specialEvent.EventStatus == EventStatus.Cancelled ||
                specialEvent.EventStatus == EventStatus.Draft ||
                specialEvent.EventStatus == EventStatus.Submitted
               )
            {
                patchEvent.Statuscode = (int?)specialEvent.EventStatus;
            }

            if (!string.IsNullOrEmpty(specialEvent?.SepCity?.Id))
            {
                patchEvent.SepCityODataBind = _dynamicsClient.GetEntityURI("adoxio_sepcities", specialEvent.SepCity.Id);
            }
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.Id, patchEvent);
                specialEvent.Id = patchEvent.AdoxioSpecialeventid;
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating/updating special event");
                throw httpOperationException;
            }

            saveTotalServings(specialEvent, existingEvent);



            if (specialEvent.EventLocations?.Count > 0)
            {
                // patchEvent.AdoxioSpecialeventSpecialeventlocations = new List<MicrosoftDynamicsCRMadoxioSpecialeventlocation>();
                // add locations to the new special event
                specialEvent.EventLocations.ForEach((Action<ViewModels.SepEventLocation>)(location =>
                {
                    var newLocation = new MicrosoftDynamicsCRMadoxioSpecialeventlocation();
                    newLocation.CopyValues(location);
                    newLocation.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.Id);
                    try
                    {
                        if (string.IsNullOrEmpty(location.Id))
                        { // create record
                            newLocation = _dynamicsClient.Specialeventlocations.Create(newLocation);
                            location.Id = newLocation.AdoxioSpecialeventlocationid;
                        }
                        else
                        { // update record
                            _dynamicsClient.Specialeventlocations.Update(location.Id, newLocation);
                        }

                        // Add service areas to new location
                        if (location.ServiceAreas?.Count > 0)
                        {
                            newLocation.AdoxioSpecialeventlocationLicencedareas = new List<MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea>();
                            location.ServiceAreas.ForEach((Action<ViewModels.SepServiceArea>)(area =>
                            {
                                var newArea = new MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea();
                                newArea.CopyValues(area);
                                newArea.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.Id);
                                newArea.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", location.Id);
                                try
                                {
                                    if (string.IsNullOrEmpty((string)area.Id))
                                    { // create record
                                        newArea = _dynamicsClient.Specialeventlicencedareas.Create(newArea);
                                        area.Id = newArea.AdoxioSpecialeventlicencedareaid;
                                    }
                                    else
                                    { // update record
                                        _dynamicsClient.Specialeventlicencedareas.Update((string)area.Id, newArea);
                                        newArea.AdoxioSpecialeventlicencedareaid = area.Id;
                                    }
                                }
                                catch (HttpOperationException httpOperationException)
                                {
                                    _logger.LogError(httpOperationException, "Error creating/updating special event location");
                                    throw httpOperationException;
                                }
                            }));
                        }

                        // Add event dates to the location
                        if (location.EventDates?.Count > 0)
                        {
                            location.EventDates.ForEach(dates =>
                            {
                                var newDates = new MicrosoftDynamicsCRMadoxioSpecialeventschedule();
                                newDates.CopyValues(dates);
                                newDates.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.Id);
                                newDates.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", location.Id);
                                try
                                {
                                    if (string.IsNullOrEmpty(dates.Id))
                                    { // create record
                                        newDates = _dynamicsClient.Specialeventschedules.Create(newDates);
                                        dates.Id = newDates.AdoxioSpecialeventscheduleid;
                                    }
                                    else
                                    { // update record
                                        _dynamicsClient.Specialeventschedules.Update(dates.Id, newDates);
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
                        _logger.LogError(httpOperationException, "Error creating/updating special event location");
                        throw httpOperationException;
                    }
                }));
            }
            var result = this.getSpecialEventData(eventId).ToViewModel();
            result.LocalId = specialEvent.LocalId;
            return new JsonResult(result);
        }

        private void saveTotalServings(ViewModels.SpecialEvent specialEvent, MicrosoftDynamicsCRMadoxioSpecialevent existingEvent)
        {
            // get drink types
            var filter = "adoxio_name eq 'Beer/Cider/Cooler' or ";
            filter += "adoxio_name eq 'Wine' or ";
            filter += "adoxio_name eq 'Spirits'";
            var drinkTypes = _dynamicsClient.Sepdrinktypes.Get().Value
                            .ToList();

            // calculate serving amounts from percentages
            int totalServings = specialEvent.TotalServings == null ? 0 : (int)specialEvent.TotalServings;
            var typeData = new List<(string, int)>{
                ("Beer/Cider/Cooler", (int)((specialEvent.beer * totalServings / 100) + 0.5)),
                ("Wine", (int)((specialEvent.wine * totalServings / 100) + 0.5)),
                ("Spirits", (int)((specialEvent.spirits * totalServings / 100) + 0.5)),
            };

            // Create or Update Drink Sale Forecast with the serving amounts
            typeData.ForEach(data =>
            {
                string drinkTypeName = data.Item1;
                int estimatedServings = data.Item2;
                var drinkType = drinkTypes.Where(drinkType => drinkType.AdoxioName == drinkTypeName).FirstOrDefault();
                var existingForecast = existingEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                    .Where(drink => drink._adoxioTypeValue == drinkType.AdoxioSepdrinktypeid)
                    .FirstOrDefault();
                createOrUpdateForecast(specialEvent, existingForecast, drinkType, estimatedServings);
            });
        }

        private void createOrUpdateForecast(ViewModels.SpecialEvent specialEvent, MicrosoftDynamicsCRMadoxioSepdrinksalesforecast existingBeerForecast, MicrosoftDynamicsCRMadoxioSepdrinktype beerType, int estimatedServings)
        {
            try
            {
                var newForecast = new MicrosoftDynamicsCRMadoxioSepdrinksalesforecast()
                {
                    AdoxioIscharging = true,
                    AdoxioPriceperserving = beerType.AdoxioMaxprice,
                    AdoxioEstimatedservings = estimatedServings,
                };
                if (existingBeerForecast == null)
                { // create record
                    newForecast.SpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.Id);
                    if (!string.IsNullOrEmpty(beerType?.AdoxioSepdrinktypeid))
                    {
                        newForecast.DrinkTypeODataBind = _dynamicsClient.GetEntityURI("adoxio_sepdrinksalesforecasts", beerType.AdoxioSepdrinktypeid);
                    }
                    _dynamicsClient.Sepdrinksalesforecasts.Create(newForecast);
                }
                else
                { // update record
                    _dynamicsClient.Sepdrinksalesforecasts.Update((string)existingBeerForecast.AdoxioSepdrinksalesforecastid, newForecast);
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating/updating sep drinks sales forecast");
                throw httpOperationException;
            }
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

        private List<ViewModels.SpecialEventSummary> GetSepSummaries(string filter)
        {
            List<ViewModels.SpecialEventSummary> result = new List<ViewModels.SpecialEventSummary>();

            string[] expand = new[] { "adoxio_PoliceRepresentativeId", "adoxio_PoliceAccountId", "" };
            IList<MicrosoftDynamicsCRMadoxioSpecialevent> items = null;
            try
            {
                items = _dynamicsClient.Specialevents.Get(filter: filter, expand: expand).Value;

                foreach (var item in items)
                {
                    result.Add(item.ToSummaryViewModel());
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting special events");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unexpected Error getting special events");
            }

            return result;
        }

        // police get summary list of applications waiting approval
        [HttpGet("police/all")]
        public IActionResult GetPoliceCurrent()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }

            string filter = $"_adoxio_policejurisdictionid_value eq {userAccount._adoxioPolicejurisdictionidValue}";

            var result = GetSepSummaries(filter);

            return new JsonResult(result);
        }

        // police get summary list of applications for the current user
        [HttpGet("police/my")]
        public IActionResult GetPoliceMy()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }

            var result = new SpecialEventPoliceMyJobs()
            {
                InProgress = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and adoxio_policeapproval eq 100000001"), // Under review
                PoliceApproved = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and adoxio_policeapproval eq 845280000"),  // Approved
                Issued = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and statuscode eq 845280003") // status is issued
            };

            return new JsonResult(result);
        }

        [HttpPost("police/{id}/assign")]
        public IActionResult PoliceAssign([FromBody] string assignee, string id)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }
            // get the special event.

            var specialEvent = _dynamicsClient.Specialevents.GetByKey(id);
            if (userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)  // ensure the current account has a matching police jurisdiction.
            {
                return Unauthorized();
            }

            // get the assignee.


            var contact = _dynamicsClient.GetContactById(assignee).GetAwaiter().GetResult();
            if (contact == null || contact.ParentcustomeridAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)
            {
                return Unauthorized();
            }

            // update the given special event.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                PoliceRepresentativeIdODataBind = _dynamicsClient.GetEntityURI("contacts", assignee)
            };
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.AdoxioSpecialeventid, patchEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error updating special event");
                return StatusCode(500);
            }


            return new JsonResult(contact.ToViewModel());
        }

        [HttpPost("police/{id}/approve")]
        public IActionResult PoliceApprove(string id)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }
            // get the special event.

            var specialEvent = _dynamicsClient.Specialevents.GetByKey(id);
            if (userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)  // ensure the current account has a matching police jurisdiction.
            {
                return Unauthorized();
            }


            // update the given special event.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                AdoxioPoliceapproval = 845280000 // Approved
            };
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.AdoxioSpecialeventid, patchEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error updating special event");
                return StatusCode(500);
            }


            return Ok();
        }

        [HttpPost("police/{id}/deny")]
        public IActionResult PoliceDeny(string id)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }
            // get the special event.

            var specialEvent = _dynamicsClient.Specialevents.GetByKey(id);
            if (userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)  // ensure the current account has a matching police jurisdiction.
            {
                return Unauthorized();
            }


            // update the given special event.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                AdoxioPoliceapproval = 845280001 // Denied  
            };
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.AdoxioSpecialeventid, patchEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error updating special event");
                return StatusCode(500);
            }


            return Ok();
        }

        [HttpPost("police/{id}/cancel")]
        public IActionResult PoliceCancel(string id)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }
            // get the special event.

            var specialEvent = _dynamicsClient.Specialevents.GetByKey(id);
            if (userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)  // ensure the current account has a matching police jurisdiction.
            {
                return Unauthorized();
            }

            // update the given special event.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                AdoxioPoliceapproval = 845280002 // Cancelled
            };
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.AdoxioSpecialeventid, patchEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error updating special event");
                return StatusCode(500);
            }


            return Ok();
        }

        [HttpPost("police/{id}/setMunicipality/{cityId}")]
        public IActionResult PoliceSetMunicipality(string id, string cityId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }
            // get the special event.

            var specialEvent = _dynamicsClient.Specialevents.GetByKey(id);
            if (userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)  // ensure the current account has a matching police jurisdiction.
            {
                return Unauthorized();
            }

            // update the given special event.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                SepCityODataBind = _dynamicsClient.GetEntityURI("adoxio_sepcities", cityId)
            };
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.AdoxioSpecialeventid, patchEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error updating special event");
                return StatusCode(500);
            }


            return Ok();
        }

        [HttpGet("police/home")]
        public IActionResult GetPoliceHome()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }

            var result = new SpecialEventPoliceHome()
            {
                AssignedJobsInProgress = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and adoxio_policeapproval eq 100000001").Count, // Under review
                // TODO - revise the filter for this query.
                AssignedJobsInProgressWithExceptions = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and adoxio_policeapproval eq 100000001").Count,  // Approved
                AssignedApplicationsIssued = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and statuscode eq 845280003").Count // status is issued
            };

            return new JsonResult(result);
        }

        /// <summary>
        /// Gets SepCity Autocomplete data for a given name using startswith
        /// </summary>
        /// <param name="defaults">If set to true, the name parameter is ignored and a list of `preview` cities is returned instead</param>
        /// <param name="name">The name to filter by using startswith</param>
        /// <returns>Dictionary of key value pairs with accountid and name as the pairs</returns>
        [HttpGet("sep-city/autocomplete")]
        // [Authorize(Policy = "Business-User")]
        public IActionResult GetAutocomplete(string name, bool defaults)
        {
            var results = new List<ViewModels.SepCity>();
            try
            {
                string filter = null;
                // escape any apostophes.
                if (name != null)
                {
                    name = name.Replace("'", "''");
                    // select active accounts that match the given name
                    if (defaults)
                    {
                        filter = $"statecode eq 0 and adoxio_ispreview eq true";
                    }
                    else
                    {
                        filter = $"statecode eq 0 and contains(adoxio_name,'{name}')";
                    }
                }
                var expand = new List<string> { "adoxio_PoliceJurisdictionId", "adoxio_LGINId" };
                var cities = _dynamicsClient.Sepcities.Get(filter: filter, expand: expand, top: 20).Value;
                foreach (var city in cities)
                {
                    var newCity = new ViewModels.SepCity
                    {
                        Id = city.AdoxioSepcityid,
                        Name = city.AdoxioName,
                        PoliceJurisdictionName = city?.AdoxioPoliceJurisdictionId?.AdoxioName,
                        LGINName = city?.AdoxioLGINId?.AdoxioName
                    };
                    results.Add(newCity);
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error while getting sep city autocomplete data.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting sep city autocomplete data.");
            }

            return new JsonResult(results);
        }
    }
}
