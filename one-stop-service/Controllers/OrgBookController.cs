using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Gov.Lclb.Cllb.OneStopService;
using Microsoft.Extensions.Logging;

namespace one_stop_service.Controllers
{
    [Route("api/[controller]")]
    public class OrgBookController : Controller
    {
        IConfiguration Configuration;
        private readonly ILogger logger;

        public OrgBookController(IConfiguration configuration, ILogger logger)
        {
            Configuration = configuration;
            this.logger = logger;
        }

        [HttpGet("IssueLicenceCredential/{licenceGuid}")]
        public async Task<IActionResult> IssueLicenceCredential(string licenceGuid)
        {
            logger.LogInformation($"Reached IssueLicenceCredential. licenceGuid: {licenceGuid}");
            //BackgroundJob.Enqueue(() => new OrgBookUtils(Configuration, logger).CreateLicenceCredential(null, licenceGuid, ));
            return Ok();
        }
    }
}