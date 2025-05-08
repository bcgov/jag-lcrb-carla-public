using Gov.Jag.Lcrb.OneStopService;
using Gov.Lclb.Cllb.Interfaces.Models;
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

        /// <summary>
        /// Check the queue for items to process.
        /// </summary>
        /// <param name="licenceGuid"></param>
        /// <returns></returns>
        [HttpGet("CheckQueue")]
        public IActionResult CheckQueue()
        {
            _logger.Information($"Reached CheckQueue.");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).CheckForNewLicences(null));
            return Ok();
        }

        [HttpGet("SendChangeAddress/{licenceGuid}")]
        public IActionResult SendChangeAddressMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendChangeAddressMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).SendChangeAddressRest(null, licenceGuid, null));
            return Ok();
        }

        [HttpGet("SendChangeName/{licenceGuid}")]
        public IActionResult SendChangeNameMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendChangeNameMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).SendChangeNameRest(null, licenceGuid, null, false, ChangeNameType.ChangeName));
            return Ok();
        }


        [HttpGet("SendTransferName/{licenceGuid}")]
        public IActionResult SendTransferMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendChangeNameMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).SendChangeNameRest(null, licenceGuid, null, false, ChangeNameType.Transfer));
            return Ok();
        }


        [HttpGet("ThirdPartyOperator/{licenceGuid}")]
        public IActionResult SendThirdPartyOperatorMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendChangeNameMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).SendChangeNameRest(null, licenceGuid, null, false, ChangeNameType.ThirdPartyOperator));
            return Ok();
        }

        [HttpGet("SendChangeStatus/{licenceGuid}")]
        public IActionResult SendChangeStatusMessage(string licenceGuid, OneStopHubStatusChange statusChange)
        {
            _logger.Information($"Reached SendChangeStatusMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).SendChangeStatusRest(null, licenceGuid, statusChange, null));
            return Ok();
        }


        [HttpGet("SendLicenceCreationMessage/{licenceGuid}")]
        public IActionResult SendLicenceCreationMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendLicenceCreationMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).SendProgramAccountRequestREST(null, licenceGuid, "001", null));
            return Ok();
        }

        [HttpGet("SendProgramAccountDetailsBroadcastMessage/{licenceGuid}")]
        public IActionResult SendProgramAccountDetailsBroadcastMessage(string licenceGuid)
        {
            _logger.Information("Reached SendProgramAccountDetailsBroadcastMessage");
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).SendProgramAccountDetailsBroadcastMessageRest(null, licenceGuid));
            return Ok();
        }


    }
}