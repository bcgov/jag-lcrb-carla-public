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
    public class ApplicationTypesController : Controller
    {                  
        private readonly IDynamicsClient _dynamicsClient;

        public ApplicationTypesController(IDynamicsClient dynamicsClient)
        {                      
            _dynamicsClient = dynamicsClient;
        }

        /// GET all licence types in Dynamics
        [HttpGet()]
        public async Task<JsonResult> GetApplicationTypes()
        {
            List<ApplicationType> applicationTypeVMList = new List<ApplicationType>();
            // get all licence types in Dynamics
            var adoxioApplicationTypes = await _dynamicsClient.Applicationtypes.GetAsync();

            foreach (var applicationType in adoxioApplicationTypes.Value)
            {
                applicationTypeVMList.Add(applicationType.ToViewModel());
            }

            return Json(applicationTypeVMList);
        }

        /// GET a specific application type
        [HttpGet("{id}")]
        public async Task<ActionResult> GetApplicationType([FromRoute] string id)
        {
            var applicationType = await _dynamicsClient.GetApplicationTypeById(id);
            if (applicationType == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return Json(applicationType.ToViewModel());
            }

        }

        /// GET an application by name
        [HttpGet("GetByName/{name}")]
        public ActionResult GetApplicationTypeByName([FromRoute] string name)
        {
            var applicationType =_dynamicsClient.GetApplicationTypeByName(name);
            if (applicationType == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return Json(applicationType.ToViewModel());
            }
            
        }

    }
}
