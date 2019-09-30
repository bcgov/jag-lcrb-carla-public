using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
                    string licenseFilter = "statuscode eq 1";  // only active licenses
                    string[] licenseExpand = { "adoxio_LicenceType" };

                    // we need to get applications so we can see if the inspection is complete.

                    IList<MicrosoftDynamicsCRMadoxioApplication> applications = null;
                    try
                    {
                        applications = _dynamicsClient.Applications.Get(filter: applicationsFilter).Value;
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error getting applications" + odee.Request.Content + "\n" + odee.Response.Content);
                        throw new Exception("Unable to get applications");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Unexpected error getting applications");
                        _logger.LogError(e.Message);
                    }


                    // get licenses
                    IList<MicrosoftDynamicsCRMadoxioLicences> licences = null;

                    try
                    {
                        licences = _dynamicsClient.Licenceses.Get(filter: licenseFilter, expand: licenseExpand).Value;
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error getting licenses" + odee.Request.Content + "\n" + odee.Response.Content);
                        throw new Exception("Unable to get licences");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Unexpected error getting establishment map data");
                        _logger.LogError(e.Message);
                    }

                    establishmentMapData = new List<EstablishmentMapData>();
                    if (licences != null)
                    {
                        foreach (var license in licences)
                        {
                            if (license.AdoxioLicenceType != null && license.AdoxioLicenceType.AdoxioName.Equals("Cannabis Retail Store"))
                            {
                                bool add = false;

                                // only consider the item if the inspection is complete.

                                // note that the Linq query is required because the License does not contain accurate data to show the related applications.

                                var relatedApplications = applications.Where(app => app._adoxioAssignedlicenceValue == license.AdoxioLicencesid).ToList();


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


                                if (add && license._adoxioEstablishmentValue != null)
                                {
                                    var establishment = _dynamicsClient.GetEstablishmentById(license._adoxioEstablishmentValue);
                                    if (establishment != null && establishment.AdoxioLatitude != null && establishment.AdoxioLongitude != null)
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
                                                id = establishment.AdoxioEstablishmentid.ToString(),
                                                Name = establishment.AdoxioName,
                                                License = license.AdoxioLicencenumber,
                                                Phone = establishment.AdoxioPhone,
                                                AddressCity = establishment.AdoxioAddresscity,
                                                AddressPostal = establishment.AdoxioAddresspostalcode,
                                                AddressStreet = establishment.AdoxioAddressstreet,
                                                Latitude = (decimal)establishment.AdoxioLatitude,
                                                Longitude = (decimal)establishment.AdoxioLongitude,
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
            return new JsonResult(establishmentMapData);
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
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error creating establishment");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
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
            if (string.IsNullOrEmpty(id) || id != item.id)
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
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating establishment");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                throw new Exception("Unable to update establishment");
            }

            try
            {
                adoxioEstablishment = _dynamicsClient.GetEstablishmentById(adoxio_establishmentid);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error getting establishment");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
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
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error delete establishment");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                throw new Exception("Unable to delete establishment");
            }

            return NoContent(); // 204
        }
    }
}
