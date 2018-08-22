using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

// TODO implement this with autorest

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = "Business-User")]
    public class AdoxioLicenceTypeController : Controller
    {
        private readonly IConfiguration Configuration;      
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDynamicsClient _dynamicsClient;

        public AdoxioLicenceTypeController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IDynamicsClient dynamicsClient)
        {
            Configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
            this._dynamicsClient = dynamicsClient;
        }

        /// GET all licence types in Dynamics
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsLicenseTypes()
        {
            List<AdoxioLicenseType> adoxioLiceseVMList = new List<AdoxioLicenseType>();
            // get all licence types in Dynamics
            var adoxioLicenceTypes = await _dynamicsClient.AdoxioLicencetypes.GetAsync();

            foreach (var licenceType in adoxioLicenceTypes.Value)
            {
                adoxioLiceseVMList.Add(licenceType.ToViewModel());
            }

            return Json(adoxioLiceseVMList);
        }

        /// GET a specific licence type
        [HttpGet("{id}")]
        public ActionResult GetDynamicsLicenseType(string id)
        {
            Guid licenceTypeId;
            if (string.IsNullOrEmpty (id) || !Guid.TryParse(id, out licenceTypeId))
            {
                return new NotFoundResult();
            }

            // get all licenses in Dynamics by Licencee Id
            var adoxioLicenceType =_dynamicsClient.GetAdoxioLicencetypeById(licenceTypeId);
            if (adoxioLicenceType == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return Json(adoxioLicenceType.ToViewModel());
            }
            
        }

    }
}
