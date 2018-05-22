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

        [HttpGet()]
        public async Task<JsonResult> GetDynamicsLegalEntities()
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            // fetch all the legal entities.
            var legalEntities = await _system.Adoxio_legalentities.ExecuteAsync();

            foreach (var legalEntity in legalEntities)
            {               
                result.Add (legalEntity.ToViewModel());
            }

            return Json(result);
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetDynamicsLegalEntity (string id)
        {
            ViewModels.AdoxioLegalEntity result = null;
            // query the Dynamics system to get the legal entity record.

            Guid? adoxio_legalentityid = new Guid(id);
            Adoxio_legalentity legalEntity = null;
            if (adoxio_legalentityid != null)
            {
                // fetch a contact
                legalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid: adoxio_legalentityid).GetValueAsync();
                result = legalEntity.ToViewModel();    
            }

            return Json(result);
        }

        [HttpPost()]
        public async Task<JsonResult> CreateApplication([FromBody] Contexts.Microsoft.Dynamics.CRM.Adoxio_application item)
        {
            // create a new contact.
            Contexts.Microsoft.Dynamics.CRM.Adoxio_application adoxioApplication = new Contexts.Microsoft.Dynamics.CRM.Adoxio_application();

            // create a DataServiceCollection to add the record
            DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_application> ContactCollection = new DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_application>(_system);
            // add a new contact.
            ContactCollection.Add(adoxioApplication);

                      // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

            await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset);

            return Json(adoxioApplication);
        }
    }
}
