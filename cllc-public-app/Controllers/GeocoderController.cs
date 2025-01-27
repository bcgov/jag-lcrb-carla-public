using System; 
using System.Net.Http;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;

namespace Gov.Lclb.Cllb.Public.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class GeocoderController : ControllerBase
    {

        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IWebHostEnvironment _env; 
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IBCEPService _bcep;


        public GeocoderController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient,  IBCEPService bcep,
            IWebHostEnvironment env, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(ApplicationsController));
            _env = env;
            _bcep = bcep;
        }
        [HttpGet("get-civic-address")]
        public async Task<IActionResult>GetExternalData([FromQuery] string queryParam){
            var baseUrl = $"{_configuration["GEOCODER_BASE_URL"]}/addresses.json?addressString={Uri.EscapeDataString(queryParam)}&locationDescriptor=any&maxResults=3&interpolation=adaptive&echo=true&brief=false&autoComplete=true&exactSpelling=false&setBack=0&outputSRS=4326&minScore=1&provinceCode=BC";
            using var httpClient = new HttpClient();
            try
            {
                string apikey =  _configuration["GEOCODER_API_KEY"];
                var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);
                request.Headers.Add("apikey", apikey);
                var response = await httpClient.SendAsync(request);
                var jsonData = await response.Content.ReadAsStringAsync();
                return Content(jsonData, "application/json");
            }
            catch (HttpRequestException ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }


        [HttpGet("get-pid")]
        public async Task<IActionResult>GetPID([FromQuery] string siteId){
            var baseUrl = $"{_configuration["GEOCODER_BASE_URL"]}/parcels/pids/{Uri.EscapeDataString(siteId)}.json";
            using var httpClient = new HttpClient();
            try
            {
               string apikey =  _configuration["GEOCODER_API_KEY"];
                var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);
                request.Headers.Add("apikey",apikey);
                var response = await httpClient.SendAsync(request);

                var jsonData = await response.Content.ReadAsStringAsync();

                return Content(jsonData, "application/json");
            }
            catch (HttpRequestException ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

    }


}