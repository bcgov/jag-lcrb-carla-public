using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
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
        private readonly IDynamicsClient _dynamicsClient;

        public IndigenousNationsController(ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(IndigenousNationsController));
        }

        [HttpGet()]
        public async Task<IActionResult> Index()
        {
            try
            {
                var nations = await _dynamicsClient.Localgovindigenousnations.GetAsync(filter: "adoxio_isindigenousnation eq true");
                var result = new List<IndigenousNation>();
                foreach (var item in nations.Value)
                {
                    result.Add(item.ToViewModel());
                }
                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating application");
                // fail if we can't create.
                throw (httpOperationException);
            }
        }

    }
}
