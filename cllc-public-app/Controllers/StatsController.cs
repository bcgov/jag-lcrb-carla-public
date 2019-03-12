using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class StatsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;
        private IMemoryCache _cache;

        public StatsController(IDynamicsClient dynamicsClient, IConfiguration configuration, ILoggerFactory loggerFactory, IMemoryCache memoryCache)
        {
            Configuration = configuration;
            _cache = memoryCache;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(StatsController));
        }

        

        /// <summary>
        /// Get a list of all policy documents for a given category
        /// </summary>
        /// <param name="category">The policy document category</param>
        /// <returns></returns>
        [HttpGet("{name}")]
        [AllowAnonymous]
        public async Task<JsonResult> GetStats(string name)
        {

            // first get a named saved query.
            string filter = $"name eq '{name}'";//zApproved Applications (Reporting)

            var savedQuerySearchResults = _dynamicsClient.Savedqueries.Get(filter: filter);

            var savedQuery = savedQuerySearchResults.Value.FirstOrDefault();

            string savedQueryId = savedQuery.Savedqueryid;

            // now get application data.

            var results = _dynamicsClient.Applications.GetSavedQuery(savedQueryId);

            var result = new List<StatsResultModel>();

            foreach (var item in results.Value)
            {
                var keys = item.Keys.ToArray();
                // last item in the object is the commregion.
                string commregion = item[keys[keys.Length - 1]];
                var newItem = new StatsResultModel()
                {
                    adoxio_name = item["adoxio_name"],
                    adoxio_establishmentpropsedname = item["adoxio_establishmentpropsedname"],
                    adoxio_establishmentaddressstreet = item["adoxio_establishmentaddressstreet"],
                    //adoxio_establishmentaddresspostalcode = item["adoxio_establishmentaddresspostalcode"],
                    //adoxio_establishmentaddresscity = item["adoxio_establishmentaddresscity"],
                    adoxio_applicationid = item["adoxio_applicationid"],
                    
                };

                string cacheKey = CacheKeys.ApplicationPrefix + newItem.adoxio_applicationid;
                
                MicrosoftDynamicsCRMadoxioApplication application = null;
                if (!_cache.TryGetValue(cacheKey, out application))
                {
                    application = await _dynamicsClient.GetApplicationByIdWithChildren(Guid.Parse(newItem.adoxio_applicationid));
                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Keep in cache for this time
                        .SetAbsoluteExpiration(TimeSpan.FromHours(24));

                    // Save data in cache.
                    _cache.Set(cacheKey, application, cacheEntryOptions);
                }
                if (application.AdoxioLocalgovindigenousnationid != null)
                {
                    newItem.commregion = (CommRegions)application.AdoxioLocalgovindigenousnationid.AdoxioCommunicationsregion;
                }
                else
                {
                    newItem.commregion = CommRegions.Unknown;
                }
                result.Add(newItem);
            }

            return Json(result);
        }

        
    }
}
