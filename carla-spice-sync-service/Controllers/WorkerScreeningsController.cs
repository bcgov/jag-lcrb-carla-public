using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SpdSync;
using System.Collections.Generic;
using SpdSync.models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;

namespace Gov.Lclb.Cllb.SpdSync.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerScreeningsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly SpiceUtils _spiceUtils;

        public WorkerScreeningsController (IConfiguration configuration, ILoggerFactory loggerFactory)
        {
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
        public ActionResult ReceiveWorkerScreeningResults([FromBody] List<WorkerScreeningResponse> results)
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
        public async Task<ActionResult> SendWorkerScreeningRequest(string workerIdString, string bearer)
        {
            if (JwtChecker.Check(bearer, Configuration))
            {
                if (Guid.TryParse(workerIdString, out Guid workerId))
                {
                    var workerRequest = new Gov.Lclb.Cllb.Interfaces.Spice.Models.WorkerScreeningRequest();
                    try
                    {
                        // Generate the application request
                        workerRequest = await _spiceUtils.GenerateWorkerScreeningRequest(workerId, _logger);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                        return BadRequest();
                    };

                    if (workerRequest == null)
                    {
                        return NotFound($"Worker {workerId} is not found.");
                    }
                    else
                    {
                        var result = await _spiceUtils.SendWorkerScreeningRequest(workerRequest, _logger);

                        if (result)
                        {
                            return Ok(workerRequest);
                        }
                        else
                        {
                            return BadRequest();
                        }

                    }
                }
                return BadRequest();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
