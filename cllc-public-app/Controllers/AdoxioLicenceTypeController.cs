using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OData.Client;
using Gov.Lclb.Cllb.Public.ViewModels;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Http;
using Gov.Lclb.Cllb.Public.Authentication;
using Newtonsoft.Json;
using System.Linq;
using System;
using Gov.Lclb.Cllb.Interfaces;

// TODO implement this with autorest

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AdoxioLicenceTypeController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Interfaces.Microsoft.Dynamics.CRM.System _system;        
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdoxioLicenceTypeController(Interfaces.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            this._system = context;            
            this._httpContextAccessor = httpContextAccessor;
        }

        /// GET all licence types in Dynamics
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsLicenseTypes()
        {
            // get all licenses in Dynamics
            List<AdoxioLicenseType> adoxioLicenses = await GetLicensesTypes(null);

            return Json(adoxioLicenses);
        }

        /// GET a specific licence type
        [HttpGet("{id}")]
        public async Task<ActionResult> GetDynamicsLicenseType(string id)
        {
            Guid licenceTypeId;
            if (string.IsNullOrEmpty (id) || !Guid.TryParse(id, out licenceTypeId))
            {
                return new NotFoundResult();
            }

            // get all licenses in Dynamics by Licencee Id
            var adoxioLicenceType = await _system.GetLicenceTypeById(licenceTypeId);
            if (adoxioLicenceType == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return Json(adoxioLicenceType.ToViewModel());
            }
            
        }

        private async Task<List<AdoxioLicenseType>> GetLicensesTypes(string licenceeId)
        {
            List<AdoxioLicenseType> adoxioLiceseVMList = new List<AdoxioLicenseType>();
            IEnumerable<Adoxio_licencetype> dynamicsLicenseList = null;
            if (string.IsNullOrEmpty(licenceeId))
            {
                dynamicsLicenseList = await _system.Adoxio_licencetypes.ExecuteAsync();
            }
            else
            {
                // get all licenses in Dynamics filtered by the GUID of the licencee
                var filter = "adoxio_licencetypeId eq " + licenceeId;

                try
                {
                    dynamicsLicenseList = await _system.Adoxio_licencetypes
                        .AddQueryOption("$filter", filter).ExecuteAsync();
                }
                catch (Exception e)
                {

                }
                
            }

            if (dynamicsLicenseList != null)
            {
                foreach (Adoxio_licencetype licenceType in dynamicsLicenseList)
                {
                    adoxioLiceseVMList.Add(licenceType.ToViewModel());
                }
            }
            return adoxioLiceseVMList;
        }


    }
}
