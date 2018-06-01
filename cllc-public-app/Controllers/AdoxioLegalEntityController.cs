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
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
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
    public class AdoxioLegalEntityController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Contexts.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;

        public AdoxioLegalEntityController(Contexts.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = distributedCache;
        }

        /// <summary>
        /// Get all Legal Entities
        /// </summary>
        /// <param name="shareholder"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsLegalEntities() //bool? shareholder
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            IEnumerable<Adoxio_legalentity> legalEntities = null;
            legalEntities = await _system.Adoxio_legalentities.ExecuteAsync();

            //if (shareholder == null)
            //{
            //    legalEntities = await _system.Adoxio_legalentities.ExecuteAsync();
            //}
            //else
            //{
            //    // Shareholders have an adoxio_position value of x.
            //    legalEntities = await _system.Adoxio_legalentities
            //        .AddQueryOption("$filter", "adoxio_position eq 1") // 1 is a Shareholder
            //        .ExecuteAsync();
            //}

            foreach (var legalEntity in legalEntities)
            {
                result.Add(legalEntity.ToViewModel());
            }

            return Json(result);
        }

        /// <summary>
        /// Get all Legal Entities where the position matches the parameter received
        /// </summary>
        /// <param name="positionType"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("position/{positionType}")]
        public async Task<JsonResult> GetDynamicsLegalEntitiesByPosition(string positionType)
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            IEnumerable<Adoxio_legalentity> legalEntities = null;
            String filter = null;

            if (positionType == null)
            {
                legalEntities = await _system.Adoxio_legalentities.ExecuteAsync();
            }
            else
            {   /*
                Partner     = adoxio_position value of 0
                Shareholder = adoxio_position value of 1
                Trustee     = adoxio_position value of 2
                Director    = adoxio_position value of 3
                Officer     = adoxio_position value of 4
                Owner       = adoxio_position value of 5 
                */
                positionType = positionType.ToLower();
                switch (positionType)
                {
                    case "partner":
                        filter = "adoxio_position eq 0";
                        break;
                    case "shareholder":
                        filter = "adoxio_position eq 1";
                        break;
                    case "trustee":
                        filter = "adoxio_position eq 2";
                        break;
                    case "director":
                        filter = "adoxio_position eq 3";
                        break;
                    case "officer":
                        filter = "adoxio_position eq 4";
                        break;
                    case "owner":
                        filter = "adoxio_position eq 5";
                        break;
                    case "directorofficer":
                        filter = "adoxio_position eq 3 or adoxio_position eq 4";
                        break;
                }

                // Execute query if filter is valid
                if (filter != null)
                {
                    legalEntities = await _system.Adoxio_legalentities
                    .AddQueryOption("$filter", filter)
                    .ExecuteAsync();
                }
            }

            if (legalEntities != null)
            {
                foreach (var legalEntity in legalEntities)
                {
                    result.Add(legalEntity.ToViewModel());
                }
            }

            return Json(result);
        }

        /// <summary>
        /// Get a specific legal entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDynamicsLegalEntity(string id)
        {
            ViewModels.AdoxioLegalEntity result = null;
            // query the Dynamics system to get the legal entity record.

            Guid? adoxio_legalentityid = new Guid(id);
            Adoxio_legalentity legalEntity = null;
            if (adoxio_legalentityid != null)
            {
                try
                {
                    legalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid: adoxio_legalentityid).GetValueAsync();
                    result = legalEntity.ToViewModel();
                }
                catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                {
                    return new NotFoundResult();
                }
            }

            return Json(result);
        }

        [HttpPost("{id}/attachments")]
        public async Task<IActionResult> UploadFile([FromRoute] string id, [FromForm]IFormFile file)
        {
            return Ok(file);
        }

        [HttpGet("{id}/attachments/{fileId}")]
        public async Task<IActionResult> DownloadFile([FromRoute] string id, [FromRoute] string fileId)
        {
            string filename = "";
            byte[] fileContents = new byte[10];
            return new FileContentResult(fileContents, "application/octet-stream")
            {
                FileDownloadName = filename
            };
        }

        /// <summary>
        /// Create a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateDynamicsLegalEntity([FromBody] ViewModels.AdoxioLegalEntity item)
        {
            // create a new legal entity.
            Contexts.Microsoft.Dynamics.CRM.Adoxio_legalentity adoxioLegalEntity = new Contexts.Microsoft.Dynamics.CRM.Adoxio_legalentity();

            // create a DataServiceCollection to add the record
            DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_legalentity> LegalEntityCollection = new DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_legalentity>(_system);

            // add a new contact.
            LegalEntityCollection.Add(adoxioLegalEntity);

            if (item.id == null)
            {
                item.id = Guid.NewGuid().ToString();
            }
                        
            adoxioLegalEntity.CopyValues(item);

            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

            DataServiceResponse dsr = await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithIndependentOperations);
            foreach (OperationResponse result in dsr)
            {
                if (result.StatusCode == 500) // error
                {
                    return StatusCode(500, result.Error.Message);
                }
            }
            
            return Json(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Update a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDynamicsLegalEntity([FromBody] ViewModels.AdoxioLegalEntity item, string id)
        {
            if (id != item.id)
            {
                return BadRequest();
            }

            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            Adoxio_legalentity adoxioLegalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid).GetValueAsync();
                        
            // copy values over from the data provided
            adoxioLegalEntity.CopyValues(item);

            
            _system.UpdateObject(adoxioLegalEntity);
           
            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

            DataServiceResponse dsr = await _system.SaveChangesAsync(); // SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset
            foreach (OperationResponse result in dsr)
            {
                if (result.StatusCode == 500) // error
                {
                    return StatusCode(500, result.Error.Message);
                }
            }
            return Json(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Delete a legal entity.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteDynamicsLegalEntity(string id)
        {
            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            try
            {
                Adoxio_legalentity adoxioLegalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid).GetValueAsync();
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
