extern alias DV;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// TODO implement this with autorest

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class AdoxioLicenceTypeController : ControllerBase
    {
        private readonly IDataverseClient _dataverseClient;

        public AdoxioLicenceTypeController(IDataverseClient dataverseClient)
        {
            _dataverseClient = dataverseClient;
        }

        /// GET all licence types in Dynamics
        [HttpGet]
        public async Task<JsonResult> GetDynamicsLicenseTypes()
        {
            List<LicenseType> adoxioLiceseVMList = new List<LicenseType>();
            // get all licence types in Dynamics
            var adoxioLicenceTypes = await _dataverseClient.GetAllLicenceTypesAsync();

            foreach (var licenceType in adoxioLicenceTypes)
            {
                adoxioLiceseVMList.Add(licenceType.ToViewModel());
            }

            return new JsonResult(adoxioLiceseVMList);
        }

        /// GET a specific licence type
        [HttpGet("{id}")]
        public async Task<ActionResult> GetDynamicsLicenseType(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new NotFoundResult();
            }

            // get specific licence type in Dataverse by Id
            var adoxioLicenceType = await _dataverseClient.GetLicenceTypeByIdAsync(id);
            if (adoxioLicenceType == null)
            {
                return new NotFoundResult();
            }

            return new JsonResult(adoxioLicenceType.ToViewModel());

        }

    }
}
