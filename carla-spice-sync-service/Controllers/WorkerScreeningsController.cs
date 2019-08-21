using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Interfaces.Spice.Models;
using Gov.Lclb.Cllb.Interfaces;
using System.Linq;

namespace Gov.Lclb.Cllb.CarlaSpiceSync.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerScreeningsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly SpiceUtils _spiceUtils;

        public WorkerScreeningsController (IConfiguration configuration, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            _dynamicsClient = dynamicsClient;
            Configuration = configuration;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(typeof(WorkerScreeningsController));
            _spiceUtils = new SpiceUtils(Configuration, _loggerFactory);
        }

        /// <summary>
        /// POST api/spice/receive
        /// Receive a response from the SPICE system
        /// </summary>
        /// <returns>OK if successful</returns>
        [HttpPost("receive")]
        public ActionResult ReceiveWorkerScreeningResults([FromBody] List<CompletedWorkerScreening> results)
        {
            // Process the updates received from the SPICE system.
            BackgroundJob.Enqueue(() => new SpiceUtils(Configuration, _loggerFactory).ReceiveWorkerImportJob(null, results));
            _logger.LogInformation("Started receive worker screening results job");
            return Ok();
        }       

        /// <summary>
        /// Send a worker record to SPICE for event driven processing.
        /// </summary>
        /// <returns></returns>
        [HttpPost("send/{workerIdString}")]
        [AllowAnonymous]
        public async Task<ActionResult> SendWorkerScreeningRequest(string workerId, string bearer)
        {
            if (JwtChecker.Check(bearer, Configuration))
            {
                if (string.IsNullOrEmpty(workerId))
                {
                    return NotFound();
                }
                
                // Get the worker from dynamics
                Guid id = Guid.Parse(workerId);
                string filter = $"adoxio_workerid eq {id}";
                var fields = new List<string> { "adoxio_ContactId" };
                MicrosoftDynamicsCRMadoxioWorker worker = _dynamicsClient.Workers.Get(filter: filter, expand: fields).Value.FirstOrDefault();

                if (worker != null)
                {
                    // Form the request
                    IncompleteWorkerScreening workerRequest = await _spiceUtils.GenerateWorkerScreeningRequest(worker);
                    // Send the request
                    var result = await _spiceUtils.SendWorkerScreeningRequest(workerRequest);

                    if (result)
                    {
                        return Ok(workerRequest);
                    }
                    return BadRequest();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
