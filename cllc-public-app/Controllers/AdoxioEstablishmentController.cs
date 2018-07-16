using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Client;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class AdoxioEstablishmentController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Interfaces.Microsoft.Dynamics.CRM.System _system;

        public AdoxioEstablishmentController(Interfaces.Microsoft.Dynamics.CRM.System context, IConfiguration configuration)
        {
            Configuration = configuration;
            this._system = context;        
        }

        /// <summary>
        /// Get a specific establishment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEstablishment(string id)
        {
            ViewModels.AdoxioEstablishment result = null;
            // query the Dynamics system to get the establishment record.

            Guid? adoxio_establishment_id = new Guid(id);
            Adoxio_establishment establishment = null;
            if (adoxio_establishment_id != null)
            {
                try
                {
                    establishment = await _system.Adoxio_establishments.ByKey(adoxio_establishment_id).GetValueAsync();
                    result = establishment.ToViewModel();
                }
                catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                {
                    return new NotFoundResult();
                }
            }

            return Json(result);
        }

        /// <summary>
        /// Create a establishment
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateEstablishment([FromBody] ViewModels.AdoxioEstablishment item)
        {
            // create a new establishment.
            var adoxioEstablishment = new Adoxio_establishment();

            // create a DataServiceCollection to add the record
			var EstablishmentCollection = new DataServiceCollection<Adoxio_establishment>(_system);

            EstablishmentCollection.Add(adoxioEstablishment);

            adoxioEstablishment.CopyValues(item);

            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

            DataServiceResponse dsr = await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithIndependentOperations);
            foreach (OperationResponse operationResult in dsr)
            {
                if (operationResult.StatusCode == 500) // error
                {
                    return StatusCode(500, operationResult.Error.Message);
                }
            }
            ViewModels.AdoxioEstablishment result = adoxioEstablishment.ToViewModel();
            result.id = ((Guid)dsr.GetAssignedId()).ToString();
            
            return Json(result);
        }

        /// <summary>
        /// Update a establishment
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEstablishment([FromBody] ViewModels.AdoxioEstablishment item, string id)
        {
            if (id != item.id)
            {
                return BadRequest();
            }
            // get the establishment.
            Guid adoxio_establishmetid = new Guid(id);
            DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_establishment> AccountCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_establishment>(_system);

            Adoxio_establishment adoxioEstablishment = await _system.Adoxio_establishments.ByKey(adoxio_establishmetid).GetValueAsync();
            _system.UpdateObject(adoxioEstablishment);
            // copy values over from the data provided
            adoxioEstablishment.CopyValues(item);

           
            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

            DataServiceResponse dsr = await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithIndependentOperations);
            foreach (OperationResponse result in dsr)
            {
                if (result.StatusCode == 500) // error
                {
                    return StatusCode(500, result.Error.Message);
                }
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
            // get the establishment.
            Guid adoxio_establishmetid = new Guid(id);
            try
            {
                Adoxio_establishment adoxioLegalEntity = await _system.Adoxio_establishments.ByKey(adoxio_establishmetid).GetValueAsync();
                _system.DeleteObject(adoxioLegalEntity);
                DataServiceResponse dsr = await _system.SaveChangesAsync();
                foreach (OperationResponse result in dsr)
                {
                    if (result.StatusCode == 500) // error
                    {
                        return StatusCode(500, result.Error.Message);
                    }
                }
            }
            catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
            {
                return new NotFoundResult();
            }            
            return NoContent(); // 204
        }
    }
}
