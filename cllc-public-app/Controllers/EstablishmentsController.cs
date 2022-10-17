
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
        private readonly IWebHostEnvironment _env;

        private const string LDB_ACCOUNT_NAME = "Liquor Distribution Branch";

        public EstablishmentsController(IDynamicsClient dynamicsClient, ILoggerFactory loggerFactory, IMemoryCache memoryCache, IWebHostEnvironment env)
        {
            _cache = memoryCache;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(EstablishmentsController));
            _env = env;
        }

        private string GetLicenceTypeId(string name)
        {
            string sanitized = name.Replace(" ", "_");
            string cacheKey = $"LTI_CODE_{sanitized}";
            string result;
            if (!_cache.TryGetValue(cacheKey, out result))
            {
                try
                {
                    result = _dynamicsClient.GetAdoxioLicencetypeByName(name)?.AdoxioLicencetypeid;
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Set the cache to expire in an hour.                   
                        .SetAbsoluteExpiration(TimeSpan.FromDays(7));

                    // Save data in cache.
                    _cache.Set(cacheKey, result, cacheEntryOptions);
                }
                catch (Exception)
                {
                    result = null;
                }
                
            }

            return result;
        }

        private string GetApplicationTypeId(string name)
        {
            string sanitized = name.Replace(" ", "_");
            string cacheKey = $"LTI_CODE_{sanitized}";
            string result;
            if (!_cache.TryGetValue(cacheKey, out result))
            {
                try
                {
                    result = _dynamicsClient.GetApplicationTypeByName(name)?.AdoxioApplicationtypeid;
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Set the cache to expire in an hour.                   
                        .SetAbsoluteExpiration(TimeSpan.FromDays(7));

                    // Save data in cache.
                    _cache.Set(cacheKey, result, cacheEntryOptions);
                }
                catch (Exception)
                {
                    result = null;
                }

            }

            return result;
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

            return new JsonResult(establishment.ToViewModel());

        }

        private IActionResult GetCSV(List<EstablishmentMapData> data, string filename)
        {
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
                    csv.WriteField(item.IsOpen ? "Open" : "Coming Soon");
                    csv.NextRecord();
                }
            }
            return File(new System.Text.UTF8Encoding().GetBytes(csvString.ToString()), "text/csv", filename);
        }

        private IActionResult GetJson(List<EstablishmentMapData> data, string filename)
        {
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
            return File(new System.Text.UTF8Encoding().GetBytes(jsonData), "application/json", filename );
        }


        /// <summary>
        /// Get a list of all map data
        /// </summary>
        /// <returns>Establishment map data, or the empty set</returns>
        [HttpGet("lrs")]
        [AllowAnonymous]
        public IActionResult GetLrs(string search)
        {
            var result = GetLrsData(search);
            return new JsonResult(result);
        }

        [HttpGet("lrs-csv")]
        [AllowAnonymous]
        public IActionResult GetLrsCSV(string search)
        {
            var data = GetLrsData(search);
            return GetCSV(data, "BC-Licensee-Retail-Stores.csv");
        }

        [HttpGet("lrs-json")]
        [AllowAnonymous]
        public IActionResult GetLrsJson(string search)
        {
            var data = GetLrsData(search);
            return GetJson(data, "BC-Licensee-Retail-Stores.json");
        }

        private List<EstablishmentMapData> GetLrsData(string search)
        {
            string cacheKey;
            if (string.IsNullOrEmpty(search))
            {
                cacheKey = "LRS_NOSEARCH";
            }
            else
            {
                search = search.ToUpper();
                search = search.Trim();
                cacheKey = $"LRS_SEARCH_{search}";
            }

            List<EstablishmentMapData> establishmentMapData;

            if (!_env.IsProduction() || !_cache.TryGetValue("S_" + cacheKey, out establishmentMapData))
            {
                string licenceTypeId = GetLicenceTypeId("Licensee Retail Store");
                if (licenceTypeId == null)
                {
                    Log.Logger.Error("ERROR - Unable to get licence type ID for Licensee Retail Store");
                    establishmentMapData = new List<EstablishmentMapData>();
                }
                else
                {
                    try
                    {
                        // get establishments                                  
                        string licenseFilter = $"statuscode eq 1 and _adoxio_licencetype_value eq {licenceTypeId}"; // only active licenses of the certain type.
                        string[] licenseExpand = { "adoxio_LicenceType", "adoxio_establishment" };

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
                            _logger.LogError(e, "Unexpected error getting establishment map data.");
                        }

                        establishmentMapData = new List<EstablishmentMapData>();
                        if (licences != null)
                        {
                            foreach (var license in licences)
                            {
                                if (license._adoxioEstablishmentValue != null && (
                                    search == null || (license.AdoxioEstablishment.AdoxioAddresscity != null &&
                                                       license.AdoxioEstablishment.AdoxioAddresscity.ToUpper().Contains(search.ToUpper()))
                                ))
                                {
                                    var establishment = license.AdoxioEstablishment;

                                    establishmentMapData.Add(new EstablishmentMapData
                                    {
                                            id = establishment.AdoxioEstablishmentid,
                                            Name = establishment.AdoxioName,
                                            License = license.AdoxioLicencenumber,
                                            Phone = establishment.AdoxioPhone,
                                            AddressCity = establishment.AdoxioAddresscity,
                                            AddressPostal = establishment.AdoxioAddresspostalcode,
                                            AddressStreet = establishment.AdoxioAddressstreet,
                                            IsOpen = establishment.AdoxioIsopen.HasValue && establishment.AdoxioIsopen.Value
                                    });
                                }
                            }
                        }
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                   // Set the cache to expire in an hour.                   
                                   .SetAbsoluteExpiration(TimeSpan.FromDays(1));

                        // Save data in cache.
                        _cache.Set("S_" + cacheKey, establishmentMapData, cacheEntryOptions);
                        cacheEntryOptions = new MemoryCacheEntryOptions()
                                   // Set the cache to expire in an hour.                   
                                   .SetAbsoluteExpiration(TimeSpan.FromDays(2));
                        // long term cache
                        _cache.Set(cacheKey, establishmentMapData, cacheEntryOptions);
                    }
                    catch (Exception e)
                    {
                        if (!_cache.TryGetValue(cacheKey, out establishmentMapData))
                        {
                            establishmentMapData = new List<EstablishmentMapData>();
                            _logger.LogError(e, "Error getting lrs data, and nothing in long term cache.");
                        }
                        else
                        {
                            _logger.LogError(e, "Error getting lrs data, showing long term cache data");
                        }
                    }
                }
            }

            // make a copy of the results to guard against accidental cache pollution.
            List<EstablishmentMapData> result = establishmentMapData.ToList();
            
            // sort the establishment list by the city alphabetically 
            result = result.OrderBy(o => o.AddressCity).ToList();

            return result;
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
            return GetCSV(data, "BC-Retail-Cannabis-Stores.csv");
        }

        [HttpGet("map-json")]
        [AllowAnonymous]
        public IActionResult GetMapJson(string search)
        {
            var data = GetMapData(search);
            return GetJson(data, "BC-Retail-Cannabis-Stores.json");
        }

        private List<EstablishmentMapData> GetMapData(string search)
        {            
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
            if (!_env.IsProduction() || !_cache.TryGetValue("S_" + cacheKey, out establishmentMapData))
            {

                string licenceTypeId = GetLicenceTypeId("Cannabis Retail Store");
                string alternateLicenceTypeId = GetLicenceTypeId("Section 119 Authorization");
                if (licenceTypeId == null)
                {
                    Log.Logger.Error("ERROR - Unable to get licence type ID for Cannabis Retail Store");
                    establishmentMapData = new List<EstablishmentMapData>();
                }
                else
                {
                    try
                    {
                        // get establishments                                  
                        string licenseFilter = $"statuscode eq 1 and _adoxio_licencetype_value eq {licenceTypeId}"; // only active licenses

                        if (alternateLicenceTypeId != null)
                        {
                            licenseFilter += $" or _adoxio_licencetype_value eq {alternateLicenceTypeId} and statuscode eq 1";
                        }


                        string[] licenseExpand = {"adoxio_LicenceType"};

                        // get licenses
                        IList<MicrosoftDynamicsCRMadoxioLicences> licences = null;

                        try
                        {
                            licences = _dynamicsClient.Licenceses.Get(filter: licenseFilter, expand: licenseExpand)
                                .Value;
                        }
                        catch (HttpOperationException httpOperationException)
                        {
                            _logger.LogError(httpOperationException, "Error getting licenses");
                            throw new Exception("Unable to get licences");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Unexpected error getting establishment map data.");
                        }

                        establishmentMapData = new List<EstablishmentMapData>();
                        if (licences != null)
                        {
                            foreach (var license in licences)
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
                                    var establishment =
                                        _dynamicsClient.GetEstablishmentById(license._adoxioEstablishmentValue);

                                    if (establishment != null &&
                                        (establishment.AdoxioLicencee == null ||
                                         establishment.AdoxioLicencee.Name != LDB_ACCOUNT_NAME) &&
                                        establishment.AdoxioLatitude != null &&
                                        establishment.AdoxioLongitude != null)
                                    {

                                        if (add && !string.IsNullOrEmpty(search) &&
                                            establishment.AdoxioName != null &&
                                            establishment.AdoxioAddresscity != null)
                                        {
                                            search = search.ToUpper();
                                            if (!establishment.AdoxioName.ToUpper().StartsWith(search)
                                                && !establishment.AdoxioAddresscity.ToUpper().StartsWith(search))
                                            {
                                                // candidate for rejection; check the lgin too.
                                                if (establishment._adoxioLginValue != null)
                                                {
                                                    establishment.AdoxioLGIN =
                                                        _dynamicsClient.GetLginById(establishment._adoxioLginValue);
                                                    if (establishment.AdoxioLGIN == null
                                                        || establishment.AdoxioLGIN.AdoxioName == null
                                                        || !establishment.AdoxioLGIN.AdoxioName.ToUpper()
                                                            .StartsWith(search))
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
                                            establishmentMapData.Add(new EstablishmentMapData
                                            {
                                                id = establishment.AdoxioEstablishmentid,
                                                Name = establishment.AdoxioName,
                                                License = license.AdoxioLicencenumber,
                                                Phone = establishment.AdoxioPhone,
                                                AddressCity = establishment.AdoxioAddresscity,
                                                AddressPostal = establishment.AdoxioAddresspostalcode,
                                                AddressStreet = establishment.AdoxioAddressstreet,
                                                Latitude = (decimal) establishment.AdoxioLatitude,
                                                Longitude = (decimal) establishment.AdoxioLongitude,
                                                IsOpen = establishment.AdoxioIsopen.HasValue &&
                                                         establishment.AdoxioIsopen.Value
                                            });
                                        }
                                    }
                                }
                            }
                        }
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

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                               // Set the cache to expire in an hour.                   
                               .SetAbsoluteExpiration(TimeSpan.FromDays(1));

                    // Save data in cache.
                    _cache.Set("S_" + cacheKey, establishmentMapData, cacheEntryOptions);

                    cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Set the cache to expire in an hour.                   
                        .SetAbsoluteExpiration(TimeSpan.FromDays(2));
                    // long term cache
                    _cache.Set(cacheKey, establishmentMapData, cacheEntryOptions);

                }
                

            }

            // make a copy of the results to guard against accidental cache pollution.
            List<EstablishmentMapData> result = establishmentMapData.ToList();

            // add LDB stores
            result.AddRange(GetLDBStores(search));

            // sort the establishment list by the city alphabetically 
            result = result.OrderBy(o => o.AddressCity).ToList();

            return result;
        }

        /// <summary>
        /// Get a list of all map data
        /// </summary>
        /// <returns>Establishment map data, or the empty set</returns>
        [HttpGet("proposed-lrs")]
        [AllowAnonymous]
        public IActionResult GetProposedLrs(string search)
        {
            var result = GetProposedLrsData(search);
            return new JsonResult(result);
        }

        [HttpGet("proposed-lrs-csv")]
        [AllowAnonymous]
        public IActionResult GetProposedLrsCSV(string search)
        {
            var data = GetProposedLrsData(search);
            return GetCSV(data, "BC-Proposed-Licensee-Retail-Stores.csv");
        }

        [HttpGet("proposed-lrs-json")]
        [AllowAnonymous]
        public IActionResult GetProposedLrsJson(string search)
        {
            var data = GetProposedLrsData(search);
            return GetJson(data, "BC-Proposed-Licensee-Retail-Stores.json");
        }

        private List<EstablishmentMapData> GetProposedLrsData(string search)
        {
            string cacheKey;
            if (string.IsNullOrEmpty(search))
            {
                cacheKey = "PLRS_NOSEARCH";
            }
            else
            {
                search = search.ToUpper();
                search = search.Trim();
                cacheKey = $"PLRS_SEARCH_{search}";
            }
            List<EstablishmentMapData> establishmentMapData;
            if (!_env.IsProduction() || !_cache.TryGetValue("S_" + cacheKey, out establishmentMapData))
            {
                string applicationTypeId = GetApplicationTypeId("LRS Transfer of Location");
                if (applicationTypeId == null)
                {
                    Log.Logger.Error("ERROR - Unable to get licence type ID for LRS Transfer of Location");
                    establishmentMapData = new List<EstablishmentMapData>();
                }
                else
                {
                    try
                    {
                        string filter = $"adoxio_checklistpsalettersent eq 845280000 and _adoxio_applicationtypeid_value eq {applicationTypeId}";
                        filter += $" and statuscode ne { (int)Public.ViewModels.AdoxioApplicationStatusCodes.Terminated}";
                        // Approved applications need to be passed to the client app
                        filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Refused}";
                        filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Cancelled}";
                        filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Approved}";
                        filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.TerminatedAndRefunded}";

                        // get establishments                                  
                        string[] expand = { "adoxio_ApplicationTypeId" };

                        // we need to get applications so we can see if the inspection is complete.

                        IList<MicrosoftDynamicsCRMadoxioApplication> applications = null;
                        try
                        {
                            applications = _dynamicsClient.Applications.Get(filter: filter, expand: expand).Value;
                        }
                        catch (HttpOperationException httpOperationException)
                        {
                            _logger.LogError(httpOperationException, "Error getting applications");
                            throw new Exception("Unable to get applications");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Unexpected error getting applications");
                        }


                        establishmentMapData = new List<EstablishmentMapData>();
                        if (applications != null)
                        {
                            foreach (var application in applications)
                            {
                                if (search == null || (application.AdoxioAddresscity != null &&
                                    application.AdoxioAddresscity.ToUpper().Contains(search.ToUpper())))
                                {
                                    establishmentMapData.Add(new EstablishmentMapData
                                    {
                                        id = application.AdoxioApplicationid,
                                        Name = application.AdoxioEstablishmentpropsedname,
                                        License = "",
                                        Phone = application.AdoxioPhone,
                                        AddressCity = application.AdoxioEstablishmentaddresscity,
                                        AddressPostal = application.AdoxioEstablishmentaddresspostalcode,
                                        AddressStreet = application.AdoxioEstablishmentaddressstreet
                                    });
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

            }

            // make a copy of the results to guard against accidental cache pollution.
            List<EstablishmentMapData> result = establishmentMapData.ToList();

            // sort the establishment list by the city alphabetically 
            result = result.OrderBy(o => o.AddressCity).ToList();

            return result;
        }

        /// <summary>
        /// Get the list of LDB stores.
        /// </summary>
        /// <returns></returns>
        private List<EstablishmentMapData> GetLDBStores(string search)
        {
            List<EstablishmentMapData> result = new List<EstablishmentMapData>();
            // find master account.
            var account = _dynamicsClient.GetAccountByNameWithEstablishments(LDB_ACCOUNT_NAME);
            if (account != null && account.AdoxioAccountAdoxioEstablishmentLicencee != null)
            {
                foreach (var establishment in account.AdoxioAccountAdoxioEstablishmentLicencee)
                {
                    if (establishment.Statuscode != null && establishment.Statuscode.Value == 845280000 && establishment.AdoxioLatitude != null && establishment.AdoxioLongitude != null
                     &&   (
                            search == null || (establishment.AdoxioAddresscity != null &&
                                               establishment.AdoxioAddresscity.ToUpper().Contains(search.ToUpper()))
                        )
                    ) // Licensed
                    {
                        result.Add(new EstablishmentMapData
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
                            });
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
        [HttpPost]
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
