using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SpdSync;

namespace Gov.Lclb.Cllb.SpdSync.Controllers
{
    [Route("api/spd")]
    public class SpdController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly string accessToken;
        private readonly string baseUri;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public SpdController(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            accessToken = Configuration["ACCESS_TOKEN"];
            baseUri = Configuration["BASE_URI"];
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(typeof(SpdController));
        }

        /// <summary>
        /// GET api/spd/send
        /// Send export to SPD.
        /// </summary>
        /// <returns>OK if successful</returns>
        [HttpGet("send")]
        public ActionResult Send()
        {
            BackgroundJob.Enqueue(() => new SpdUtils(Configuration, _loggerFactory).SendExportJob(null));
            _logger.LogInformation("Started send export job");
            return Ok();
        }

        /// <summary>
        /// GET api/apd/receive
        /// Start a receive import job
        /// </summary>
        /// <returns>OK if successful</returns>
        [HttpGet("receive")]
        public ActionResult Receive()
        {
            // check the file drop for a file, and then process it.
            BackgroundJob.Enqueue(() => new SpdUtils(Configuration, _loggerFactory).ReceiveImportJob(null));
            _logger.LogInformation("Started receive import job");
            return Ok();

        }

        /// <summary>
        /// GET api/apd/receive
        /// Start a receive import job
        /// </summary>
        /// <returns>OK if successful</returns>
        [HttpGet("update-worker")]
        public async System.Threading.Tasks.Task<ActionResult> UpdateWorkerAsync()
        {
            // check the file drop for a file, and then process it.
            await new WorkerUpdater(Configuration, _loggerFactory, SpdUtils.SetupSharepoint(Configuration)).ReceiveImportJob(null);
            _logger.LogInformation("Started receive import job");
            return Ok();

        }
    }
}
