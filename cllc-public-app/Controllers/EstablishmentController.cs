using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "Business-User")]
    public class EstablishmentsController : Controller
    {        
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;

        public EstablishmentsController(IDynamicsClient dynamicsClient, ILoggerFactory loggerFactory)
        {            
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
                return Json(establishment.ToViewModel());
            }
            
        }

        /// <summary>
        /// Get a list of all map data
        /// </summary>
        /// <returns>Establishment map data, or the empty set</returns>
        [HttpGet("map")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMap()
        {
            // get establishments                      
            var expand = new List<string> { "adoxio_establishment" }; // get establishment data at the same time.
            string filter =  "statuscode eq 1";  // only active licenses

            IList<MicrosoftDynamicsCRMadoxioLicences> licences = null;

            try
            {
                licences = _dynamicsClient.Licenceses.Get(filter: filter, expand: expand).Value;

            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error creating establishment");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                throw new Exception("Unable to establishment");
            }
            catch (Exception e)
            {
                _logger.LogError("Unexpected error getting establishment map data");                
                _logger.LogError(e.Message);                
            }
            List<EstablishmentMapData> establishmentMapData = new List<EstablishmentMapData>();
            if (licences != null)
            {                
                foreach (var license in licences)
                {
                    if (license.AdoxioEstablishment != null && license.AdoxioEstablishment.AdoxioLatitude != null && license.AdoxioEstablishment.AdoxioLongitude != null)
                    {
                        establishmentMapData.Add(new EstablishmentMapData()
                        {
                            id = license.AdoxioEstablishment.AdoxioEstablishmentid.ToString(),
                            Name = license.AdoxioEstablishment.AdoxioName,
                            License = license.AdoxioLicencenumber,
                            Phone = license.AdoxioEstablishment.AdoxioPhone,
                            AddressCity = license.AdoxioEstablishment.AdoxioAddresscity,
                            AddressPostal = license.AdoxioEstablishment.AdoxioAddresspostalcode,
                            AddressStreet = license.AdoxioEstablishment.AdoxioAddressstreet,
                            Latitude = (decimal)license.AdoxioEstablishment.AdoxioLatitude,
                            Longitude = (decimal)license.AdoxioEstablishment.AdoxioLongitude,
                        });
                    }

                }               
            }
            return Json(establishmentMapData);
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
                       
            return Json(result);
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

            return Json(adoxioEstablishment.ToViewModel());
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
