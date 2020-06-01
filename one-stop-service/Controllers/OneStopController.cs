using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Gov.Lclb.Cllb.OneStopService;
using Serilog;

namespace one_stop_service.Controllers
{
    [Route("api/[controller]")]
    public class OneStopController : Controller
    {
        IConfiguration Configuration;
        private readonly ILogger _logger;

        public OneStopController(IConfiguration configuration)
        {
            Configuration = configuration;
            _logger = Log.Logger;
            
            
        }

        [HttpGet("SendLicenceCreationMessage/{licenceGuid}")]
        public async Task<IActionResult> SendLicenceCreationMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendLicenceCreationMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _logger).SendLicenceCreationMessageREST(null, licenceGuid, "001"));
            return Ok();
        }

        [HttpGet("SendProgramAccountDetailsBroadcastMessage/{licenceGuid}")]
        public IActionResult SendProgramAccountDetailsBroadcastMessage(string licenceGuid)
        {
            _logger.Information("Reached SendProgramAccountDetailsBroadcastMessage");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _logger).SendProgramAccountDetailsBroadcastMessageREST(null, licenceGuid));
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