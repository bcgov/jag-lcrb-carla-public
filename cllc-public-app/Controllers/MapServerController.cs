using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    // fix for cors issue with certain map servers.
    [Route("api/[controller]")]
    [ApiController]
    // public API
    public class MapServerController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _cache;

        string _mapserver;
        HttpClient _client;
        public MapServerController(IConfiguration configuration, IWebHostEnvironment env, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
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
            string cacheKey = "TILE_" + a + "_" + b + "_" + c;
            byte[] result;

            if (!_cache.TryGetValue(cacheKey, out result))
            {
                string urlString = _mapserver
                + "/tile/" + a + "/" + b + "/" + c
                + Request.QueryString.ToUriComponent();

                // get the content.
                var request = new HttpRequestMessage(HttpMethod.Get, urlString);
                var response = await _client.SendAsync(request);
                result = await response.Content.ReadAsByteArrayAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                            // Set the cache to expire far in the future.                    
                            .SetAbsoluteExpiration(TimeSpan.FromDays(365 * 5));

                // Save data in cache.
                _cache.Set(cacheKey, result, cacheEntryOptions);
            }

            return File(result, "image/jpeg");
        }

    }
}
