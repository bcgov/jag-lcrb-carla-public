extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IndigenousNationsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDataverseClient _dataverse;

        public IndigenousNationsController(ILoggerFactory loggerFactory, IDataverseClient dataverse)
        {
            _dataverse = dataverse;
            _logger = loggerFactory.CreateLogger(typeof(IndigenousNationsController));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var nations = await _dataverse.GetIndigenousNationsAsync();
                var result = new List<IndigenousNation>();
                foreach (var item in nations)
                {
                    var linkedAccount = await _dataverse.GetAccountByLginLinkIdAsync(item.Id.ToString());
                    result.Add(new IndigenousNation
                    {
                        Id = item.Id.ToString(),
                        Name = item.adoxio_name,
                        WebsiteUrl = linkedAccount?.WebSiteURL
                    });
                }
                return new JsonResult(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error getting indigenous nations");
                throw;
            }
        }
    }
}
