extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Jag.Lcrb.OneStopService;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace one_stop_service.Controllers
{
    [Route("api/[controller]")]
    public class OneStopController : Controller
    {
        private readonly ILogger _logger;

        public OneStopController()
        {
            _logger = Log.Logger;
        }

        /// <summary>
        /// Check the queue for items to process.
        /// </summary>
        /// <returns></returns>
        [HttpGet("CheckQueue")]
        public IActionResult CheckQueue()
        {
            _logger.Information($"Reached CheckQueue.");
            BackgroundJob.Enqueue<OneStopUtils>(utils => utils.CheckForNewLicences(null));
            return Ok();
        }

        [HttpGet("SendChangeAddress/{licenceGuid}")]
        public IActionResult SendChangeAddressMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendChangeAddressMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue<OneStopUtils>(utils => utils.SendChangeAddressRest(null, licenceGuid, null));
            return Ok();
        }

        [HttpGet("SendChangeName/{licenceGuid}")]
        public IActionResult SendChangeNameMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendChangeNameMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue<OneStopUtils>(utils => utils.SendChangeNameRest(null, licenceGuid, null, false, ChangeNameType.ChangeName));
            return Ok();
        }


        [HttpGet("SendTransferName/{licenceGuid}")]
        public IActionResult SendTransferMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendChangeNameMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue<OneStopUtils>(utils => utils.SendChangeNameRest(null, licenceGuid, null, false, ChangeNameType.Transfer));
            return Ok();
        }


        [HttpGet("ThirdPartyOperator/{licenceGuid}")]
        public IActionResult SendThirdPartyOperatorMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendChangeNameMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue<OneStopUtils>(utils => utils.SendChangeNameRest(null, licenceGuid, null, false, ChangeNameType.ThirdPartyOperator));
            return Ok();
        }

        [HttpGet("SendChangeStatus/{licenceGuid}")]
        public IActionResult SendChangeStatusMessage(string licenceGuid, OneStopHubStatusChange statusChange)
        {
            _logger.Information($"Reached SendChangeStatusMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue<OneStopUtils>(utils => utils.SendChangeStatusRest(null, licenceGuid, statusChange, null));
            return Ok();
        }


        [HttpGet("SendLicenceCreationMessage/{licenceGuid}")]
        public IActionResult SendLicenceCreationMessage(string licenceGuid)
        {
            _logger.Information($"Reached SendLicenceCreationMessage. licenceGuid: {licenceGuid}");
            BackgroundJob.Enqueue<OneStopUtils>(utils => utils.SendProgramAccountRequestREST(null, licenceGuid, "001", null));
            return Ok();
        }

        [HttpGet("SendProgramAccountDetailsBroadcastMessage/{licenceGuid}")]
        public IActionResult SendProgramAccountDetailsBroadcastMessage(string licenceGuid)
        {
            _logger.Information("Reached SendProgramAccountDetailsBroadcastMessage");
            BackgroundJob.Enqueue<OneStopUtils>(utils => utils.SendProgramAccountDetailsBroadcastMessageRest(null, licenceGuid));
            return Ok();
        }


    }
}
