extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class ApplicationTypesController : ControllerBase
    {
        private readonly IDataverseClient _dataverse;

        public ApplicationTypesController(IDataverseClient dataverse)
        {
            _dataverse = dataverse;
        }

        [HttpGet]
        public async Task<JsonResult> GetApplicationTypes()
        {
            var applicationTypeVMList = new List<ApplicationType>();
            var applicationTypes = await _dataverse.GetApplicationTypesAsync();
            foreach (var applicationType in applicationTypes)
                applicationTypeVMList.Add(applicationType.ToViewModel());
            return new JsonResult(applicationTypeVMList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetApplicationType([FromRoute] string id)
        {
            var applicationType = await _dataverse.GetApplicationTypeByIdAsync(id);
            if (applicationType == null) return new NotFoundResult();
            return new JsonResult(applicationType.ToViewModel());
        }

        [HttpGet("GetByName/{name}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetApplicationTypeByName([FromRoute] string name)
        {
            var applicationType = await _dataverse.GetApplicationTypeByNameAsync(name);
            if (applicationType == null) return new NotFoundResult();
            return new JsonResult(applicationType.ToViewModel());
        }
    }
}
