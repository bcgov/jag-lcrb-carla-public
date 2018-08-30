using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Authorization;

namespace Gov.Lclb.Cllb.SpdSync.Controllers
{
    [Route("api/spd")]
    public class IndexController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly string accessToken;
        private readonly string baseUri;

        public IndexController(IConfiguration configuration)
        {
            Configuration = configuration;
            accessToken = Configuration["ACCESS_TOKEN"];
            baseUri = Configuration["BASE_URI"];
;
        }

        /// <summary>
        /// GET api/search
        /// </summary>
        /// <param name="query">space delimited query keywords</param>
        /// <param name="_skip">Search Result Offset</param>
        /// <param name="_limit">Maximum number of search results to return</param>
        /// <returns>List of GUID id fields for requests matching the query.</returns>
        [HttpGet("run-export")]
        public ActionResult Send()
        {
            BackgroundJob.Enqueue(() => new SpdUtils(Configuration).SendExportJob(null));
            return Ok();
        }

        [HttpGet("receive")]
        public ActionResult Receive()
        {
            // check the file drop for a file, and then process it.
            return NoContent();

        }
    }
}
