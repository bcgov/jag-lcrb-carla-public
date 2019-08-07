using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// TODO implement this with autorest

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = "Business-User")]
    public class AdoxioLicenceTypeController : Controller
    {                  
        private readonly IDynamicsClient _dynamicsClient;

        public AdoxioLicenceTypeController(IDynamicsClient dynamicsClient)
        {                      
            _dynamicsClient = dynamicsClient;
        }

        /// GET all licence types in Dynamics
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsLicenseTypes()
        {
            List<LicenseType> adoxioLiceseVMList = new List<LicenseType>();
            // get all licence types in Dynamics
            var adoxioLicenceTypes = await _dynamicsClient.Licencetypes.GetAsync();

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
