extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LocalGovernmentsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDataverseClient _dataverse;

        public LocalGovernmentsController(ILoggerFactory loggerFactory, IDataverseClient dataverse)
        {
            _dataverse = dataverse;
            _logger = loggerFactory.CreateLogger(typeof(LocalGovernmentsController));
        }

        [HttpGet("autocomplete")]
        public async Task<ActionResult> GetLocalGovernments(string name)
        {
            var results = new List<LGListItem>();

            try
            {
                var localGovernments = await _dataverse.GetLginsAsync(nameContains: name);
                foreach (var lg in localGovernments)
                {
                    var linkedAccount = await _dataverse.GetAccountByLginLinkIdAsync(lg.Id.ToString());
                    results.Add(new LGListItem
                    {
                        Id = lg.Id.ToString(),
                        Name = lg.adoxio_name,
                        WebsiteUrl = linkedAccount?.WebSiteURL
                    });
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error getting Local Governments");
                throw;
            }

            return new JsonResult(results);
        }
    }

    public class LGListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string WebsiteUrl { get; set; }
    }
}
