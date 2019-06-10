using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class FeaturesController : Controller 
    {
        private readonly IConfiguration Configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public FeaturesController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(AliasController));
        }


        /// <summary>
        /// Get a list of enabled features
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult GetFeatureList()
        {
            var features = new List<string>();
            if (!String.IsNullOrEmpty(Configuration["FEATURE_MARKETER"]))
            {
                features.Add("Marketer");
            }

            return Json(features);
        }

    }
}
