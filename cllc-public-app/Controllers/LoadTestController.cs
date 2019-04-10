using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("websurge-allow.txt")]
    public class LoadTestController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly AppDbContext db;
        private readonly IHostingEnvironment _env;
        private readonly SiteMinderAuthOptions _options = new SiteMinderAuthOptions();

        public LoadTestController(AppDbContext db, IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            this.db = db;
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
