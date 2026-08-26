extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PreviousAddressController : ControllerBase
    {
        private readonly IDataverseClient _dataverse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public PreviousAddressController(IDataverseClient dataverse, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            _dataverse = dataverse;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(PreviousAddressController));
        }

        [HttpGet("by-contactid/{contactId}")]
        public async Task<IActionResult> GetAddressesByContactId(string contactId)
        {
            if (string.IsNullOrEmpty(contactId)) return BadRequest();

            var addresses = await _dataverse.GetPreviousAddressesByContactIdAsync(contactId);
            addresses = addresses.OrderByDescending(a => a.adoxio_FromDate).ToList();

            var result = new List<ViewModels.PreviousAddress>();
            foreach (var a in addresses)
                result.Add(a.ToViewModel());

            return new JsonResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress([FromBody] ViewModels.PreviousAddress item, string id)
        {
            if (id != null && item.id != null && id != item.id) return BadRequest();

            var address = await _dataverse.GetPreviousAddressByIdAsync(id);
            if (address == null) return new NotFoundResult();

            var patchAddress = new adoxio_previousaddress { Id = Guid.Parse(id) };
            patchAddress.CopyValues(item);
            try { await _dataverse.UpdatePreviousAddressAsync(patchAddress); }
            catch (Exception e) { _logger.LogError(e, "Error updating previous address"); }

            address = await _dataverse.GetPreviousAddressByIdAsync(id);
            return new JsonResult(address.ToViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] ViewModels.PreviousAddress item)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            var address = new adoxio_previousaddress();
            address.CopyValues(item);
            Guid addressId;
            try
            {
                addressId = await _dataverse.CreatePreviousAddressAsync(address);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating previous address");
                throw;
            }

            var patchAddress = new adoxio_previousaddress { Id = addressId };
            patchAddress.adoxio_ContactId = new EntityReference(Contact.EntityLogicalName, Guid.Parse(item.contactId));
            if (!string.IsNullOrEmpty(item.workerId))
                patchAddress.adoxio_WorkerId = new EntityReference(adoxio_worker.EntityLogicalName, Guid.Parse(item.workerId));
            try { await _dataverse.UpdatePreviousAddressAsync(patchAddress); }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating previous address associations");
                throw;
            }

            var created = await _dataverse.GetPreviousAddressByIdAsync(addressId.ToString());
            return new JsonResult(created?.ToViewModel());
        }

        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteAddress(string id)
        {
            var address = await _dataverse.GetPreviousAddressByIdAsync(id);
            if (address == null) return new NotFoundResult();

            await _dataverse.DeletePreviousAddressAsync(id);
            return NoContent();
        }
    }
}
