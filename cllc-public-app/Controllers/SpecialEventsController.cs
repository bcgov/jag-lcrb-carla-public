using System;
using System.Collections.Generic;
using System.Linq;
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

            return new JsonResult(specialEvent.ToViewModel());
        }


        // TODO - determine if this service "GetSpecialEventByApplicant" is required. 

        /// <summary>
        ///     GET a special event by id
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("applicant/{eventId}")]
        public IActionResult GetSpecialEventByApplicant(string eventId)
        {
            var expand = new List<string> { };
            MicrosoftDynamicsCRMadoxioSpecialevent specialEvent = null;
            if (!string.IsNullOrEmpty(eventId))
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


            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);


            var newSpecialEvent = new MicrosoftDynamicsCRMadoxioSpecialevent();
            newSpecialEvent.CopyValues(specialEvent);

            if (!string.IsNullOrEmpty(userSettings.AccountId))
            {
                newSpecialEvent.AccountODataBind = _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId);
            }
            if (!string.IsNullOrEmpty(userSettings.ContactId))
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
            return new JsonResult(specialEvent);
        }

        [HttpPut("{eventId}")]
        public IActionResult UpdateSpecialEvent(string eventId, [FromBody] ViewModels.SpecialEvent specialEvent)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(eventId) || eventId != specialEvent?.Id)
            {
                return BadRequest();
            }

            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent();
            patchEvent.CopyValues(specialEvent);
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

            if (specialEvent.DrinksSalesForecasts?.Count > 0)
            {
                specialEvent.DrinksSalesForecasts.ForEach(forecast =>
                {
                    var newForecast = new MicrosoftDynamicsCRMadoxioSepdrinksalesforecast();
                    newForecast.CopyValues(forecast);

                    newForecast.SpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.Id);
                    if (!string.IsNullOrEmpty(forecast?.DrinkTypeId))
                    {
                        newForecast.DrinkTypeODataBind = _dynamicsClient.GetEntityURI("adoxio_sepdrinksalesforecasts", forecast.DrinkTypeId);
                    }
                    try
                    {
                        if (string.IsNullOrEmpty((string)forecast.Id))
                        { // create record
                            newForecast = _dynamicsClient.Sepdrinksalesforecasts.Create(newForecast);
                            forecast.Id = newForecast.AdoxioSepdrinksalesforecastid;
                        }
                        else
                        { // update record
                            _dynamicsClient.Sepdrinksalesforecasts.Update((string)forecast.Id, newForecast);
                        }
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error creating/updating sep drinks sales forecast");
                        throw httpOperationException;
                    }
                });
            }

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

        private List<ViewModels.SpecialEventSummary> GetSepSummaries(string filter)
        {
            List<ViewModels.SpecialEventSummary> result = new List<ViewModels.SpecialEventSummary>();

            string[] expand = new[] { "adoxio_PoliceRepresentativeId", "adoxio_PoliceAccountId" };
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


            return Ok();
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
                        filter = $"statecode eq 0 and adoxio_ispreview req true)";
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
