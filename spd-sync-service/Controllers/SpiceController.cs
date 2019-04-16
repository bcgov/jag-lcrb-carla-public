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
    public class SpiceController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public SpiceController(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(typeof(SpiceController));
        }

        /// <summary>
        /// POST api/spice/receive
        /// Receive a response from the SPICE system
        /// </summary>
        /// <returns>OK if successful</returns>
        [HttpPost("receive")]
        public ActionResult Receive([FromBody] List<WorkerResponse> items)
        {
            // Process the updates received from the SPICE system.  
            // This will preferably enqueue a hangfire job that will process the data (as it could take some time).
            // 
            return Ok();
        }        
    }
}
