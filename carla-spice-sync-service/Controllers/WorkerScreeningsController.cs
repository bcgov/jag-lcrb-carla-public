using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SpdSync;
using System.Collections.Generic;
using SpdSync.models;

namespace Gov.Lclb.Cllb.SpdSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerScreeningsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public WorkerScreeningsController (IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(typeof(WorkerScreeningsController));
        }

        /// <summary>
        /// POST api/spice/receive
        /// Receive a response from the SPICE system
        /// </summary>
        /// <returns>OK if successful</returns>
        [HttpPost("receive")]
        public ActionResult ReceiveWorkerScreeningResults([FromBody] List<WorkerScreeningResponse> results)
        {
            // Process the updates received from the SPICE system.
            BackgroundJob.Enqueue(() => new SpiceUtils(Configuration, _loggerFactory).ReceiveWorkerImportJob(null, results));
            _logger.LogInformation("Started receive worker screening results job");
            return Ok();
        }       

        /// <summary>
        /// Send a worker record to SPICE for test purposes.  Normally this would occur from a polling process.
        /// </summary>
        /// <returns></returns>
        [HttpPost("send/{workerId}")]
        public ActionResult SendWorkerScreeningRequest(string workerId )
        {
            // Process the updates received from the SPICE system.
            //BackgroundJob.Enqueue(() => new SpiceUtils(Configuration, _loggerFactory).ReceiveImportJob(null, responses));
            _logger.LogInformation("Started SendWorkerScreening job");
            return Ok();
        }


    }
}
