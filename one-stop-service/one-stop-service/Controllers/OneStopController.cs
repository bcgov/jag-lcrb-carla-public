using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Gov.Lclb.Cllb.OneStopService;

namespace one_stop_service.Controllers
{
    [Route("api/[controller]")]
    public class OneStopController : Controller
    {
        IConfiguration Configuration;
        public OneStopController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [Route("[action]")]
        public async Task<IActionResult> SendLicenceCreationMessage()
        {
            BackgroundJob.Enqueue(() => new OneStopUtils(Configuration).SendLicenceCreationMessage(null));
            return Ok();
        }

    }
}