using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Gov.Lclb.Cllb.Interfaces.Spice.Models;
using Gov.Lclb.Cllb.CarlaSpiceSync;

namespace Gov.Lclb.Cllb.CarlaSpiceSync.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationScreeningsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly SpiceUtils _spiceUtils;

        public ApplicationScreeningsController(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(typeof(ApplicationScreeningsController));
            _spiceUtils = new SpiceUtils(Configuration, _loggerFactory);
        }

        /// <summary>
        /// POST api/spice/receive
        /// Receive a response from the SPICE system
        /// </summary>
        /// <returns>OK if successful</returns>
        [HttpPost("receive")]
        public ActionResult ReceiveApplicationScreeningResult([FromBody] List<CompletedApplicationScreening> results)
        {
            // Process the updates received from the SPICE system.
            BackgroundJob.Enqueue(() => new SpiceUtils(Configuration, _loggerFactory).ReceiveApplicationImportJob(null, results));
            _logger.LogInformation("Started receive completed Application Screening job");
            return Ok();
        }


        /// <summary>
        /// Send an application record to SPICE for event driven processing
        /// </summary>
        /// <returns></returns>
        [HttpPost("send/{applicationIdString}")]
        [AllowAnonymous]
        public async Task<ActionResult> SendApplicationScreeningRequest(string applicationIdString, string bearer)
        {
            if (JwtChecker.Check(bearer, Configuration))
            {
                if (Guid.TryParse(applicationIdString, out Guid applicationId))
                {
                    var applicationRequest = new IncompleteApplicationScreening();
                    try
                    {
                        // Generate the application request
                        applicationRequest = _spiceUtils.GenerateApplicationScreeningRequest(applicationId);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return NotFound($"Application {applicationId} is not found.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                        return BadRequest();
                    }

                    if (applicationRequest == null)
                    {
                        return NotFound($"Application {applicationId} is not found.");
                    }
                    
                    var result = _spiceUtils.SendApplicationScreeningRequest(applicationId, applicationRequest);

                    if (result)
                    {
                        return Ok(applicationRequest);
                    }
                }
                return BadRequest();
            }
            return Unauthorized();
        }
    }
}
