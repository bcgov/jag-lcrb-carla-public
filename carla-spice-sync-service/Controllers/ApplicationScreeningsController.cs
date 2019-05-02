using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SpdSync;
using System.Collections.Generic;
using SpdSync.models;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.SpdSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationScreeningsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public ApplicationScreeningsController(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(typeof(ApplicationScreeningsController));
        }

        /// <summary>
        /// POST api/spice/receive
        /// Receive a response from the SPICE system
        /// </summary>
        /// <returns>OK if successful</returns>
        [HttpPost("receive")]
        public ActionResult ReceiveApplicationScreeningResult([FromBody] List<ApplicationScreeningResponse> results)
        {
            // Process the updates received from the SPICE system.
            BackgroundJob.Enqueue(() => new SpiceUtils(Configuration, _loggerFactory).ReceiveApplicationImportJob(null, results));
            _logger.LogInformation("Started receive Application Screenings import job");
            return Ok();
        }


        /// <summary>
        /// Send an application record to SPICE for test purposes.  Normally this would occur from a polling process.
        /// </summary>
        /// <returns></returns>
        [HttpPost("send/{applicationId}")]
        public async Task<ActionResult> SendApplicationScreeningResponse(string applicationId)
        {
            // Generate the application request
            SpiceUtils spiceUtils = new SpiceUtils(Configuration, _loggerFactory);
            var applicationRequest = await spiceUtils.GenerateApplicationScreeningRequest(applicationId);

            _logger.LogError("Data to send:");
            _logger.LogError(JsonConvert.SerializeObject(applicationRequest));

            List<Interfaces.Spice.Models.ApplicationScreeningRequest> payload = new List<Interfaces.Spice.Models.ApplicationScreeningRequest>();
            payload.Add(applicationRequest);

            var result = await spiceUtils.SpiceClient.ReceiveApplicationScreeningsWithHttpMessagesAsync(payload);

            _logger.LogError("Response code was");

            _logger.LogError("" + result.Response.StatusCode);
            _logger.LogError("Response text was");
            _logger.LogError(await result.Response.Content.ReadAsStringAsync());

            _logger.LogInformation("Done Send Application Screening");
            return Ok();
        }


    }
}
