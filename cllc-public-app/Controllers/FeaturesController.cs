using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class FeaturesController : Controller 
    {
        private readonly IConfiguration _configuration;                

        public FeaturesController(IConfiguration configuration)
        {
            _configuration = configuration;            
        }


        /// <summary>
        /// Get a list of enabled features
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult GetFeatureList()
        {
            var features = new List<string>();
            if (!String.IsNullOrEmpty(_configuration["FEATURE_CRS_RENEWAL"]))
            {
                features.Add("CRS-Renewal");
            }

            if (!String.IsNullOrEmpty(_configuration["FEATURE_INDIGENOUS_NATION"]))
            {
                features.Add("IndigenousNation");
            }

            if (!String.IsNullOrEmpty(_configuration["FEATURE_MAPS"]))
            {
                features.Add("Maps");
            }

            return Json(features);
        }

    }
}
