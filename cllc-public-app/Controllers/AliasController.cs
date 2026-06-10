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
    public class AliasController : ControllerBase
    {
        private readonly IDataverseClient _dataverse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public AliasController(IDataverseClient dataverse, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            _dataverse = dataverse;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(AliasController));
        }

        [HttpGet("by-contactid/{contactId}")]
        public async Task<IActionResult> GetAliasByContactId(string contactId)
        {
            if (string.IsNullOrEmpty(contactId)) return BadRequest();

            var aliases = await _dataverse.GetAliasesByContactIdAsync(contactId);
            if (aliases == null) return new NotFoundResult();

            var result = new List<ViewModels.Alias>();
            foreach (var item in aliases)
                result.Add(item.ToViewModel());

            return new JsonResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlias([FromBody] ViewModels.Alias item, string id)
        {
            if (id != null && item.id != null && id != item.id) return BadRequest();

            var alias = await _dataverse.GetAliasByIdAsync(id);
            if (alias == null) return new NotFoundResult();

            var patchAlias = new adoxio_alias { Id = Guid.Parse(id) };
            patchAlias.CopyValues(item);
            try { await _dataverse.UpdateAliasAsync(patchAlias); }
            catch (Exception e) { _logger.LogError(e, "Error updating alias"); }

            alias = await _dataverse.GetAliasByIdAsync(id);
            return new JsonResult(alias.ToViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlias([FromBody] ViewModels.Alias item)
        {
            if (item?.contact?.id == null || item?.worker?.id == null) return BadRequest();

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            var alias = new adoxio_alias();
            alias.CopyValues(item);
            Guid aliasId;
            try
            {
                aliasId = await _dataverse.CreateAliasAsync(alias);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating alias");
                throw;
            }

            var patchAlias = new adoxio_alias { Id = aliasId };
            patchAlias.adoxio_ContactId = new EntityReference(Contact.EntityLogicalName, Guid.Parse(item.contact.id));
            patchAlias.adoxio_WorkerId = new EntityReference(adoxio_worker.EntityLogicalName, Guid.Parse(item.worker.id));
            try { await _dataverse.UpdateAliasAsync(patchAlias); }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating alias associations");
                throw;
            }

            var created = await _dataverse.GetAliasByIdAsync(aliasId.ToString());
            return new JsonResult(created?.ToViewModel());
        }

        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteAlias(string id)
        {
            var alias = await _dataverse.GetAliasByIdAsync(id);
            if (alias == null) return new NotFoundResult();

            await _dataverse.DeleteAliasAsync(id);
            return NoContent();
        }
    }
}
