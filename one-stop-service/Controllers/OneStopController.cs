using Gov.Lclb.Cllb.OneStopService;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace one_stop_service.Controllers
{
    [Route("api/[controller]")]
    public class OneStopController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;

        public OneStopController(IConfiguration configuration, IMemoryCache cache)
        {
            Configuration = configuration;
            _cache = cache;
            _logger = Log.Logger;


        }

        [HttpGet("SendLicenceCreationMessage/{licenceGuid}")]
        public IActionResult SendLicenceCreationMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendLicenceCreationMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).SendProgramAccountRequestREST(null, licenceGuid, "001"));
            return Ok();
        }

        [HttpGet("SendProgramAccountDetailsBroadcastMessage/{licenceGuid}")]
        public IActionResult SendProgramAccountDetailsBroadcastMessage(string licenceGuid)
        {
            _logger.Information("Reached SendProgramAccountDetailsBroadcastMessage");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).SendProgramAccountDetailsBroadcastMessageRest(null, licenceGuid));
            return Ok();
        }

        [HttpGet("LdbExport")]
        public IActionResult LdbExport()
        {
            _logger.Information("Reached LdbExport");
            BackgroundJob.Enqueue(() => new LdbExport(Configuration).SendLdbExport(null));
            return Ok();
        }

    }
}