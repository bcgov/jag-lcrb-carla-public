using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("websurge-allow.txt")]
    [ApiController]
    public class LoadTestController : ControllerBase
    {
        private readonly IHostingEnvironment _env;

        public LoadTestController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LoadTest(string path)
        {
            // check to see if we have a local path.  (do not allow a redirect to another website)
            if (!_env.IsProduction())
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

    }
}
