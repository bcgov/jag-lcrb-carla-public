using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    // fix for cors issue with certain map servers.
    [Route("api/[controller]")]
    public class MapServerController : Controller
    {        
        private readonly IHostingEnvironment _env;

        string _mapserver;
        HttpClient _client;
        public MapServerController(IConfiguration configuration, IHostingEnvironment env)
        {
            _client = new HttpClient();
            _env = env;
            _mapserver = configuration["MAPSERVER_URI"];
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> MapServer()
        {
            string urlString = _mapserver
                + Request.QueryString.ToUriComponent();
            

            // get the content.
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, urlString);
            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();

            return Content(result);

        }

        [AllowAnonymous]
        [HttpGet("tile/{a}/{b}/{c}")]
        public async Task<ActionResult> MapServer(string a, string b, string c)
        {
            string urlString = _mapserver
                + "/tile/" + a + "/" + b + "/" + c
                + Request.QueryString.ToUriComponent();

            // get the content.
            var request = new HttpRequestMessage(HttpMethod.Get, urlString);
            var response = await _client.SendAsync(request);
            var result = await response.Content.ReadAsByteArrayAsync();

            return File( result, "image/jpeg");            
        }

    }
}
