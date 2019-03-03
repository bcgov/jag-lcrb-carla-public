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
        [HttpGet()]
        [AllowAnonymous]
        public JsonResult GetStats()
        {
            // first get a named saved query.
            string filter = "name eq 'zApproved Applications (Reporting)'";

            var savedQuerySearchResults = _dynamicsClient.Savedqueries.Get(filter: filter);

            var savedQuery = savedQuerySearchResults.Value.FirstOrDefault();

            string savedQueryId = savedQuery.Savedqueryid;

            // now get application data.

            var results = _dynamicsClient.Applications.GetSavedQuery(savedQueryId);

            return Json(results.Value);
        }

        
    }
}
