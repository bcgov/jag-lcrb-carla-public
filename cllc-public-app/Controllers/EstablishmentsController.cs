
extern alias DV;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DV::Gov.Lclb.Cllb.Interfaces;
using CsvHelper;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly IDataverseClient _dataverse;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _env;

        private const string LDB_ACCOUNT_NAME = "Liquor Distribution Branch";

        public EstablishmentsController(IDataverseClient dataverse, ILoggerFactory loggerFactory, IMemoryCache memoryCache, IWebHostEnvironment env)
        {
            _cache = memoryCache;
            _dataverse = dataverse;
            _logger = loggerFactory.CreateLogger(typeof(EstablishmentsController));
            _env = env;
        }

        private async Task<string> GetLicenceTypeId(string name)
        {
            string sanitized = name.Replace(" ", "_");
            string cacheKey = $"LTI_CODE_{sanitized}";
            if (_cache.TryGetValue(cacheKey, out string result))
                return result;

            try
            {
                var licenceType = await _dataverse.GetLicenceTypeByNameAsync(name);
                result = licenceType?.Id == Guid.Empty ? null : licenceType?.Id.ToString();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(7));
                _cache.Set(cacheKey, result, cacheEntryOptions);
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        private async Task<string> GetApplicationTypeId(string name)
        {
            string sanitized = name.Replace(" ", "_");
            string cacheKey = $"ATI_CODE_{sanitized}";
            if (_cache.TryGetValue(cacheKey, out string result))
                return result;

            try
            {
                var appType = await _dataverse.GetApplicationTypeByNameAsync(name);
                result = appType?.Id == Guid.Empty ? null : appType?.Id.ToString();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(7));
                _cache.Set(cacheKey, result, cacheEntryOptions);
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Get a specific establishment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEstablishment(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out _))
            {
                return new NotFoundResult();
            }

            var establishment = await _dataverse.GetEstablishmentByIdAsync(id);
            if (establishment == null)
            {
                return new NotFoundResult();
            }

            return new JsonResult(establishment.ToViewModel());
        }

        private IActionResult GetCSV(List<EstablishmentMapData> data, string filename)
        {
            StringWriter csvString = new StringWriter();
            using (var csv = new CsvWriter(csvString, CultureInfo.InvariantCulture))
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
            return File(new System.Text.UTF8Encoding().GetBytes(jsonData), "application/json", filename);
        }


        /// <summary>
        /// Get a list of all map data
        /// </summary>
        /// <returns>Establishment map data, or the empty set</returns>
        [HttpGet("lrs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLrs(string search)
        {
            var result = await GetLrsData(search);
            return new JsonResult(result);
        }

        [HttpGet("lrs-csv")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLrsCSV(string search)
        {
            var data = await GetLrsData(search);
            return GetCSV(data, "BC-Licensee-Retail-Stores.csv");
        }

        [HttpGet("lrs-json")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLrsJson(string search)
        {
            var data = await GetLrsData(search);
            return GetJson(data, "BC-Licensee-Retail-Stores.json");
        }

        private async Task<List<EstablishmentMapData>> GetLrsData(string search)
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
                string licenceTypeId = await GetLicenceTypeId("Licensee Retail Store");
                if (licenceTypeId == null)
                {
                    Log.Logger.Error("ERROR - Unable to get licence type ID for Licensee Retail Store");
                    establishmentMapData = new List<EstablishmentMapData>();
                }
                else
                {
                    try
                    {
                        IList<adoxio_licences> licences = null;
                        try
                        {
                            licences = await _dataverse.GetActiveLicencesByTypeIdsAsync(new[] { licenceTypeId });
                        }
                        catch (Exception httpOperationException)
                        {
                            _logger.LogError(httpOperationException, "Error getting licenses");
                            throw new Exception("Unable to get licences");
                        }

                        establishmentMapData = new List<EstablishmentMapData>();
                        if (licences != null)
                        {
                            foreach (var licence in licences)
                            {
                                if (licence.adoxio_establishment == null) continue;

                                var establishment = await _dataverse.GetEstablishmentByIdAsync(licence.adoxio_establishment.Id.ToString());
                                if (establishment == null) continue;

                                if (search == null || (establishment.adoxio_AddressCity != null &&
                                    establishment.adoxio_AddressCity.ToUpper().Contains(search.ToUpper())))
                                {
                                    establishmentMapData.Add(new EstablishmentMapData
                                    {
                                        id = establishment.Id.ToString(),
                                        Name = establishment.adoxio_name,
                                        License = licence.adoxio_LicenceNumber,
                                        Phone = establishment.adoxio_Phone,
                                        AddressCity = establishment.adoxio_AddressCity,
                                        AddressPostal = establishment.adoxio_AddressPostalCode,
                                        AddressStreet = establishment.adoxio_AddressStreet,
                                        IsOpen = establishment.adoxio_IsOpen == true
                                    });
                                }
                            }
                        }
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                   .SetAbsoluteExpiration(TimeSpan.FromDays(1));
                        _cache.Set("S_" + cacheKey, establishmentMapData, cacheEntryOptions);
                        cacheEntryOptions = new MemoryCacheEntryOptions()
                                   .SetAbsoluteExpiration(TimeSpan.FromDays(2));
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
        public async Task<IActionResult> GetMap(string search)
        {
            var result = await GetMapData(search);
            return new JsonResult(result);
        }

        [HttpGet("map-csv")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMapCSV(string search)
        {
            var data = await GetMapData(search);
            return GetCSV(data, "BC-Retail-Cannabis-Stores.csv");
        }

        [HttpGet("map-json")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMapJson(string search)
        {
            var data = await GetMapData(search);
            return GetJson(data, "BC-Retail-Cannabis-Stores.json");
        }

        private async Task<List<EstablishmentMapData>> GetMapData(string search)
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
                string licenceTypeId = await GetLicenceTypeId("Cannabis Retail Store");
                string alternateLicenceTypeId = await GetLicenceTypeId("S119 CRS Authorization");
                if (string.IsNullOrEmpty(alternateLicenceTypeId))
                {
                    alternateLicenceTypeId = await GetLicenceTypeId("Section 119 Authorization");
                }
                string prsTypeId = await GetLicenceTypeId("Producer Retail Store");
                string s119PrsTypeId = await GetLicenceTypeId("S119 PRS Authorization");

                if (licenceTypeId == null)
                {
                    Log.Logger.Error("ERROR - Unable to get licence type ID for Cannabis Retail Store");
                    establishmentMapData = new List<EstablishmentMapData>();
                }
                else
                {
                    try
                    {
                        // Collect all relevant licence type IDs
                        var typeIds = new List<string> { licenceTypeId };
                        if (alternateLicenceTypeId != null) typeIds.Add(alternateLicenceTypeId);
                        if (prsTypeId != null) typeIds.Add(prsTypeId);
                        if (s119PrsTypeId != null) typeIds.Add(s119PrsTypeId);

                        // Look up LDB account ID once for exclusion check.
                        var ldbAccount = await _dataverse.GetAccountByNameAsync(LDB_ACCOUNT_NAME);
                        string ldbAccountId = ldbAccount?.Id == Guid.Empty ? null : ldbAccount?.Id.ToString();

                        IList<adoxio_licences> licences = null;
                        try
                        {
                            licences = await _dataverse.GetActiveLicencesByTypeIdsAsync(typeIds);
                        }
                        catch (Exception httpOperationException)
                        {
                            _logger.LogError(httpOperationException, "Error getting licenses");
                            throw new Exception("Unable to get licences");
                        }

                        establishmentMapData = new List<EstablishmentMapData>();
                        if (licences != null)
                        {
                            foreach (var licence in licences)
                            {
                                // Change 2019-10-24 - default to add, as we no longer check for final inspection.
                                bool add = true;

                                if (add && licence.adoxio_establishment != null)
                                {
                                    var establishment = await _dataverse.GetEstablishmentByIdAsync(licence.adoxio_establishment.Id.ToString());
                                    if (establishment == null) continue;

                                    // Do not add LDB stores here — they are added separately via GetLDBStores().
                                    // Only include establishments that are open.
                                    bool isOpen = establishment.adoxio_IsOpen == true;
                                    string licenceeId = licence.adoxio_Licencee?.Id.ToString();
                                    if (isOpen &&
                                        (ldbAccountId == null || licenceeId != ldbAccountId) &&
                                        establishment.adoxio_Latitude != null &&
                                        establishment.adoxio_Longitude != null)
                                    {
                                        if (add && !string.IsNullOrEmpty(search))
                                        {
                                            var upperSearch = search.ToUpper();
                                            bool matchesName = establishment.adoxio_name != null &&
                                                establishment.adoxio_name.ToUpper().StartsWith(upperSearch);
                                            bool matchesCity = establishment.adoxio_AddressCity != null &&
                                                establishment.adoxio_AddressCity.ToUpper().StartsWith(upperSearch);

                                            if (!matchesName && !matchesCity)
                                            {
                                                add = false;
                                            }
                                        }

                                        if (add)
                                        {
                                            establishmentMapData.Add(new EstablishmentMapData
                                            {
                                                id = establishment.Id.ToString(),
                                                Name = establishment.adoxio_name,
                                                License = licence.adoxio_LicenceNumber,
                                                Phone = establishment.adoxio_Phone,
                                                AddressCity = establishment.adoxio_AddressCity,
                                                AddressPostal = establishment.adoxio_AddressPostalCode,
                                                AddressStreet = establishment.adoxio_AddressStreet,
                                                Latitude = establishment.adoxio_Latitude.HasValue ? (decimal)establishment.adoxio_Latitude.Value : 0m,
                                                Longitude = establishment.adoxio_Longitude.HasValue ? (decimal)establishment.adoxio_Longitude.Value : 0m,
                                                IsOpen = isOpen
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
                               .SetAbsoluteExpiration(TimeSpan.FromDays(1));
                    _cache.Set("S_" + cacheKey, establishmentMapData, cacheEntryOptions);

                    cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromDays(2));
                    _cache.Set(cacheKey, establishmentMapData, cacheEntryOptions);
                }
            }

            // make a copy of the results to guard against accidental cache pollution.
            List<EstablishmentMapData> result = establishmentMapData.ToList();

            // add LDB stores
            result.AddRange(await GetLDBStores(search));

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
        public async Task<IActionResult> GetProposedLrs(string search)
        {
            var result = await GetProposedLrsData(search);
            return new JsonResult(result);
        }

        [HttpGet("proposed-lrs-csv")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProposedLrsCSV(string search)
        {
            var data = await GetProposedLrsData(search);
            return GetCSV(data, "BC-Proposed-Licensee-Retail-Stores.csv");
        }

        [HttpGet("proposed-lrs-json")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProposedLrsJson(string search)
        {
            var data = await GetProposedLrsData(search);
            return GetJson(data, "BC-Proposed-Licensee-Retail-Stores.json");
        }

        private async Task<List<EstablishmentMapData>> GetProposedLrsData(string search)
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
                string applicationTypeId = await GetApplicationTypeId("LRS Transfer of Location");
                if (applicationTypeId == null)
                {
                    Log.Logger.Error("ERROR - Unable to get licence type ID for LRS Transfer of Location");
                    establishmentMapData = new List<EstablishmentMapData>();
                }
                else
                {
                    try
                    {
                        var excludeStatuses = new[]
                        {
                            (int)AdoxioApplicationStatusCodes.Terminated,
                            (int)AdoxioApplicationStatusCodes.Refused,
                            (int)AdoxioApplicationStatusCodes.Cancelled,
                            (int)AdoxioApplicationStatusCodes.Approved,
                            (int)AdoxioApplicationStatusCodes.TerminatedAndRefunded
                        };

                        IList<adoxio_application> applications = null;
                        try
                        {
                            applications = await _dataverse.GetProposedLrsApplicationsAsync(applicationTypeId, excludeStatuses);
                        }
                        catch (Exception httpOperationException)
                        {
                            _logger.LogError(httpOperationException, "Error getting applications");
                            throw new Exception("Unable to get applications");
                        }

                        establishmentMapData = new List<EstablishmentMapData>();
                        if (applications != null)
                        {
                            foreach (var application in applications)
                            {
                                if (search == null || (application.adoxio_EstablishmentAddressCity != null &&
                                    application.adoxio_EstablishmentAddressCity.ToUpper().Contains(search.ToUpper())))
                                {
                                    establishmentMapData.Add(new EstablishmentMapData
                                    {
                                        id = application.Id.ToString(),
                                        Name = application.adoxio_EstablishmentPropsedName,
                                        License = "",
                                        Phone = application.adoxio_Phone,
                                        AddressCity = application.adoxio_EstablishmentAddressCity,
                                        AddressPostal = application.adoxio_EstablishmentAddressPostalCode,
                                        AddressStreet = application.adoxio_EstablishmentAddressStreet
                                    });
                                }
                            }
                        }
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                   .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                        _cache.Set("S_" + cacheKey, establishmentMapData, cacheEntryOptions);
                        cacheEntryOptions = new MemoryCacheEntryOptions()
                                   .SetAbsoluteExpiration(TimeSpan.FromDays(1));
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
        private async Task<List<EstablishmentMapData>> GetLDBStores(string search)
        {
            List<EstablishmentMapData> result = new List<EstablishmentMapData>();
            var account = await _dataverse.GetAccountByNameAsync(LDB_ACCOUNT_NAME);
            if (account == null) return result;

            var establishments = await _dataverse.GetEstablishmentsByAccountIdAsync(account.Id.ToString());
            foreach (var establishment in establishments)
            {
                bool isLdbOpen = establishment.adoxio_IsOpen == true;
                bool isActive = establishment.statecode == adoxio_establishment_statecode.Active;
                if (isActive
                    && isLdbOpen
                    && establishment.adoxio_Latitude != null && establishment.adoxio_Longitude != null
                    && (search == null || (establishment.adoxio_AddressCity != null &&
                                           establishment.adoxio_AddressCity.ToUpper().Contains(search.ToUpper()))))
                {
                    result.Add(new EstablishmentMapData
                    {
                        id = establishment.Id.ToString(),
                        Name = "BC Cannabis Store",
                        IsOpen = isLdbOpen,
                        License = "Public Store",
                        AddressStreet = establishment.adoxio_AddressStreet,
                        AddressCity = establishment.adoxio_AddressCity,
                        AddressPostal = establishment.adoxio_AddressPostalCode,
                        Latitude = (decimal)establishment.adoxio_Latitude.Value,
                        Longitude = (decimal)establishment.adoxio_Longitude.Value
                    });
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
            var dvEstablishment = new adoxio_establishment();
            dvEstablishment.CopyValues(item);
            try
            {
                var newId = await _dataverse.CreateEstablishmentAsync(dvEstablishment);
                var created = await _dataverse.GetEstablishmentByIdAsync(newId.ToString());
                return new JsonResult(created.ToViewModel());
            }
            catch (Exception httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating establishment");
                throw new Exception("Unable to create establishment");
            }
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

            Guid adoxio_establishmentid = GuidUtility.SafeGuidConvert(id);

            var existing = await _dataverse.GetEstablishmentByIdAsync(id);
            if (existing == null)
            {
                return new NotFoundResult();
            }

            // patch only the allowed fields
            var patch = new adoxio_establishment();
            patch.Id = adoxio_establishmentid;
            patch.CopyValues(item);

            try
            {
                await _dataverse.UpdateEstablishmentAsync(patch);
            }
            catch (Exception httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating establishment");
                throw new Exception("Unable to update establishment");
            }

            var updated = await _dataverse.GetEstablishmentByIdAsync(id);
            return new JsonResult(updated.ToViewModel());
        }

        /// <summary>
        /// Delete a establishment.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteEstablishment(string id)
        {
            var establishment = await _dataverse.GetEstablishmentByIdAsync(id);
            if (establishment == null)
            {
                return new NotFoundResult();
            }

            try
            {
                await _dataverse.DeleteEstablishmentAsync(id);
            }
            catch (Exception httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error delete establishment");
                throw new Exception("Unable to delete establishment");
            }

            return NoContent(); // 204
        }
    }
}
