
using CsvHelper;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class EstablishmentsController : ControllerBase
    {
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;

        private const string LDB_ACCOUNT_NAME = "Liquor Distribution Branch";

        public EstablishmentsController(IDynamicsClient dynamicsClient, ILoggerFactory loggerFactory, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(EstablishmentsController));
        }

        /// <summary>
        /// Get a specific establishment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetEstablishment(string id)
        {

            Guid adoxio_establishment_id;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out adoxio_establishment_id))
            {
                return new NotFoundResult();
            }

            // get the establishment
            var establishment = _dynamicsClient.GetEstablishmentById(adoxio_establishment_id);
            if (establishment == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new JsonResult(establishment.ToViewModel());
            }

        }

        /// <summary>
        /// Get a list of all map data
        /// </summary>
        /// <returns>Establishment map data, or the empty set</returns>
        [HttpGet("map")]
        [AllowAnonymous]
        public IActionResult GetMap(string search)
        {
            var result = GetMapData(search);
            return new JsonResult(result);
        }

        [HttpGet("map-csv")]
        [AllowAnonymous]
        public IActionResult GetMapCSV(string search)
        {
            var data = GetMapData(search);
            StringWriter csvString = new StringWriter();
            using (var csv = new CsvWriter(csvString))
            {                
                // headers
                csv.WriteField("Licence");
                csv.WriteField("Establishment Name");
                csv.WriteField("Phone");
                csv.WriteField("Address");
                csv.WriteField("City");
                csv.WriteField("Postal");
                csv.WriteField("Status");
                csv.NextRecord();

                foreach (var item in data)
                {
                    csv.WriteField(item.License);
                    csv.WriteField(item.Name);
                    csv.WriteField(item.Phone);
                    csv.WriteField(item.AddressStreet);
                    csv.WriteField(item.AddressCity);
                    csv.WriteField(item.AddressPostal);
                    csv.WriteField(item.IsOpen ? "Open": "Coming Soon");
                    csv.NextRecord();
                }
            }
            return File(new System.Text.UTF8Encoding().GetBytes(csvString.ToString()), "text/csv", "BC-Retail-Cannabis-Stores.csv");
        }

        [HttpGet("map-json")]
        [AllowAnonymous]
        public IActionResult GetMapJson(string search)
        {
            var data = GetMapData(search);

            List<object> dataForJson = new List<object>();
            foreach (var item in data)
            {
                dataForJson.Add(new
                {
                    item.License,
                    item.Name,
                    item.Phone,
                    Address = item.AddressStreet,
                    City = item.AddressCity,
                    Postal = item.AddressPostal,
                    Status = item.IsOpen ? "Open" : "Coming Soon"
                });
            }
               
            string jsonData = JsonConvert.SerializeObject(dataForJson);
            return File(new System.Text.UTF8Encoding().GetBytes(jsonData), "application/json", "BC-Retail-Cannabis-Stores.json");
        }
        private List<EstablishmentMapData> GetMapData(string search)
        {
            /*
            string communicationRegionsCacheKey = "LGIN_REGIONS";
            Dictionary<int, string> communicationRegions;
            if (!_cache.TryGetValue(communicationRegionsCacheKey, out communicationRegions))
            {
                // populate the communicationRegions.
                try
                {
                    var regions = _dynamicsClient.Globaloptionsetdefinitions.GetByKey("46b91aec-49a5-4859-bd5b-e9edba00bb9d");
                    foreach (var region in regions.Options)
                    {

                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting communication regions.");
                }
                
            }

            */


            string cacheKey;
            if (string.IsNullOrEmpty(search))
            {
                cacheKey = "MAP_NOSEARCH";
            }
            else
            {
                search = search.ToUpper();
                search = search.Trim();
                cacheKey = $"MAP_SEARCH_{search}";
            }
            List<EstablishmentMapData> establishmentMapData;
            if (!_cache.TryGetValue("S_" + cacheKey, out establishmentMapData))
            {
                try
                {
                    string applicationsFilter = "_adoxio_assignedlicence_value ne null ";
                    // get establishments                                  
                    string licenseFilter = "statuscode eq 1"; // only active licenses
                    string[] licenseExpand = { "adoxio_LicenceType" };

                    // we need to get applications so we can see if the inspection is complete.

                    IList<MicrosoftDynamicsCRMadoxioApplication> applications = null;
                    try
                    {
                        applications = _dynamicsClient.Applications.Get(filter: applicationsFilter).Value;
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError("Error getting applications" + httpOperationException.Request.Content + "\n" + httpOperationException.Response.Content);
                        throw new Exception("Unable to get applications");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Unexpected error getting applications {e.Message}");
                    }

                    // get licenses
                    IList<MicrosoftDynamicsCRMadoxioLicences> licences = null;

                    try
                    {
                        licences = _dynamicsClient.Licenceses.Get(filter: licenseFilter, expand: licenseExpand).Value;
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error getting licenses");
                        throw new Exception("Unable to get licences");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Unexpected error getting establishment map data.");
                    }

                    establishmentMapData = new List<EstablishmentMapData>();
                    if (licences != null)
                    {
                        foreach (var license in licences)
                        {
                            if (license.AdoxioLicenceType != null && license.AdoxioLicenceType.AdoxioName.Equals("Cannabis Retail Store"))
                            {
                                // Change 2019-10-24 - default to add, as we no longer check to see if the establishment has had the final inspection.
                                bool add = true;

                                // only consider the item if the inspection is complete.

                                // note that the Linq query is required because the License does not contain accurate data to show the related applications.

                                //var relatedApplications = applications.Where(app => app._adoxioAssignedlicenceValue == license.AdoxioLicencesid).ToList();

                                // Change 2019-10-24 - no longer filter out establishments that have not passed the final inspection.
                                /*
                                if (relatedApplications != null)
                                {
                                    foreach (var item in relatedApplications)
                                    {
                                        // with the new business flow, we check for a pass (845280000) in AdoxioAppchecklistinspectionresults
                                        if (item.AdoxioAppchecklistinspectionresults != null && item.AdoxioAppchecklistinspectionresults == 845280000)
                                        {
                                            add = true;
                                        }
                                    }
                                }
                                */
                                // do not add LDB stores at this time.
                                if (add && license._adoxioEstablishmentValue != null)
                                {
                                    var establishment = _dynamicsClient.GetEstablishmentById(license._adoxioEstablishmentValue);


                                    if (establishment != null &&
                                        (establishment.AdoxioLicencee == null || establishment.AdoxioLicencee.Name != LDB_ACCOUNT_NAME) &&
                                        establishment.AdoxioLatitude != null && establishment.AdoxioLongitude != null)
                                    {

                                        if (add && !string.IsNullOrEmpty(search) && establishment.AdoxioName != null && establishment.AdoxioAddresscity != null)
                                        {
                                            search = search.ToUpper();
                                            if (!establishment.AdoxioName.ToUpper().StartsWith(search) == true
                                                && !establishment.AdoxioAddresscity.ToUpper().StartsWith(search) == true)
                                            {
                                                // candidate for rejection; check the lgin too.
                                                if (establishment._adoxioLginValue != null)
                                                {
                                                    establishment.AdoxioLGIN = _dynamicsClient.GetLginById(establishment._adoxioLginValue);
                                                    if (establishment.AdoxioLGIN == null
                                                        || establishment.AdoxioLGIN.AdoxioName == null
                                                        || !establishment.AdoxioLGIN.AdoxioName.ToUpper().StartsWith(search))
                                                    //|| !
                                                    {
                                                        add = false;
                                                    }

                                                }
                                                else
                                                {
                                                    add = false;
                                                }
                                            }
                                        }

                                        if (add)
                                        {
                                            establishmentMapData.Add(new EstablishmentMapData()
                                            {
                                                id = establishment.AdoxioEstablishmentid,
                                                Name = establishment.AdoxioName,
                                                License = license.AdoxioLicencenumber,
                                                Phone = establishment.AdoxioPhone,
                                                AddressCity = establishment.AdoxioAddresscity,
                                                AddressPostal = establishment.AdoxioAddresspostalcode,
                                                AddressStreet = establishment.AdoxioAddressstreet,
                                                Latitude = (decimal)establishment.AdoxioLatitude,
                                                Longitude = (decimal)establishment.AdoxioLongitude,
                                                IsOpen = establishment.AdoxioIsopen.HasValue && establishment.AdoxioIsopen.Value
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                               // Set the cache to expire in an hour.                   
                               .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    // Save data in cache.
                    _cache.Set("S_" + cacheKey, establishmentMapData, cacheEntryOptions);
                    cacheEntryOptions = new MemoryCacheEntryOptions()
                               // Set the cache to expire in an hour.                   
                               .SetAbsoluteExpiration(TimeSpan.FromDays(1));
                    // long term cache
                    _cache.Set(cacheKey, establishmentMapData, cacheEntryOptions);
                }
                catch (Exception e)
                {
                    if (!_cache.TryGetValue(cacheKey, out establishmentMapData))
                    {
                        establishmentMapData = new List<EstablishmentMapData>();
                        _logger.LogError(e, "Error getting map data, and nothing in long term cache.");
                    }
                    else
                    {
                        _logger.LogError(e, "Error getting map data, showing long term cache data");
                    }
                }

            }

            // make a copy of the results to guard against accidental cache pollution.
            List<EstablishmentMapData> result = establishmentMapData.ToList();

            // add LDB stores
            result.AddRange(GetLDBStores());

            // sort the establishment list by the city alphabetically 
            result = result.OrderBy(o => o.AddressCity).ToList();

            return result;
        }

        /// <summary>
        /// Get the list of LDB stores.
        /// </summary>
        /// <returns></returns>
        private List<EstablishmentMapData> GetLDBStores()
        {
            List<EstablishmentMapData> result = new List<EstablishmentMapData>();
            // find master account.
            var account = _dynamicsClient.GetAccountByNameWithEstablishments(LDB_ACCOUNT_NAME);
            if (account != null && account.AdoxioAccountAdoxioEstablishmentLicencee != null)
            {
                foreach (var establishment in account.AdoxioAccountAdoxioEstablishmentLicencee)
                {
                    if (establishment.Statuscode != null && establishment.Statuscode.Value == 845280000) // Licensed
                    {
                        EstablishmentMapData data = new EstablishmentMapData()
                        {
                            id = establishment.AdoxioEstablishmentid,
                            Name = "BC Cannabis Store",
                            IsOpen = establishment.AdoxioIsopen.Value,
                            License = "Public Store",
                            AddressStreet = establishment.AdoxioAddressstreet,
                            AddressCity = establishment.AdoxioAddresscity,
                            AddressPostal = establishment.AdoxioAddresspostalcode,
                            Latitude = establishment.AdoxioLatitude.Value,
                            Longitude = establishment.AdoxioLongitude.Value
                        };
                        result.Add(data);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Create a establishment
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateEstablishment([FromBody] ViewModels.Establishment item)
        {
            // create a new legal entity.
            MicrosoftDynamicsCRMadoxioEstablishment adoxio_establishment = new MicrosoftDynamicsCRMadoxioEstablishment();

            // copy received values to Dynamics LegalEntity
            adoxio_establishment.CopyValues(item);
            try
            {
                adoxio_establishment = await _dynamicsClient.Establishments.CreateAsync(adoxio_establishment);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating establishment");
                throw new Exception("Unable to create establishment");
            }

            ViewModels.Establishment result = adoxio_establishment.ToViewModel();

            return new JsonResult(result);
        }

        /// <summary>
        /// Update a establishment
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEstablishment([FromBody] ViewModels.Establishment item, string id)
        {
            if (item == null || string.IsNullOrEmpty(id) || id != item.id)
            {
                return BadRequest();
            }

            // get the legal entity.
            Guid adoxio_establishmentid = GuidUtility.SafeGuidConvert(id);

            MicrosoftDynamicsCRMadoxioEstablishment adoxioEstablishment = _dynamicsClient.GetEstablishmentById(adoxio_establishmentid);
            if (adoxioEstablishment == null)
            {
                return new NotFoundResult();
            }

            // we are doing a patch, so wipe out the record.
            adoxioEstablishment = new MicrosoftDynamicsCRMadoxioEstablishment();

            // copy values over from the data provided
            adoxioEstablishment.CopyValues(item);

            try
            {
                await _dynamicsClient.Establishments.UpdateAsync(adoxio_establishmentid.ToString(), adoxioEstablishment);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating establishment");
                throw new Exception("Unable to update establishment");
            }

            try
            {
                adoxioEstablishment = _dynamicsClient.GetEstablishmentById(adoxio_establishmentid);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting establishment");
                throw new Exception("Unable to get establishment after update");
            }

            return new JsonResult(adoxioEstablishment.ToViewModel());
        }

        /// <summary>
        /// Delete a establishment.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteEstablishment(string id)
        {
            // get the legal entity.
            Guid adoxio_establishmentid = new Guid(id);
            MicrosoftDynamicsCRMadoxioEstablishment establishment = _dynamicsClient.GetEstablishmentById(adoxio_establishmentid);
            if (establishment == null)
            {
                return new NotFoundResult();
            }

            try
            {
                await _dynamicsClient.Establishments.DeleteAsync(adoxio_establishmentid.ToString());
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error delete establishment");
                throw new Exception("Unable to delete establishment");
            }

            return NoContent(); // 204
        }
    }
}
