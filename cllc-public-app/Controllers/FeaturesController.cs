using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class FeaturesController : ControllerBase
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
        public List<string> GetFeatureList()
        {
            var features = new List<string>();

            if (!string.IsNullOrEmpty(_configuration["FEATURE_INDIGENOUS_NATION"]))
            {
                features.Add("IndigenousNation");
            }

            if (!string.IsNullOrEmpty(_configuration["FEATURE_MAPS"]))
            {
                features.Add("Maps");
            }

            if (!string.IsNullOrEmpty(_configuration["FEATURE_LICENSEE_CHANGES"]))
            {
                features.Add("LicenseeChanges");
            }

            if (!string.IsNullOrEmpty(_configuration["FEATURE_NO_WET_SIGNATURE"]))
            {
                features.Add("NoWetSignature");
            }

            if (!string.IsNullOrEmpty(_configuration["FEATURE_SECURITY_SCREENING"]))
            {
                features.Add("SecurityScreening");
            }

            if (!string.IsNullOrEmpty(_configuration["FEATURE_DISABLE_LOGIN"]))
            {
                features.Add("DisableLogin");
            }

            if (!string.IsNullOrEmpty(_configuration["FEATURE_LIQUOR_ONE"]))
            {
                features.Add("LiquorOne");
            }

            if (!string.IsNullOrEmpty(_configuration["FEATURE_LIQUOR_TWO"]))
            {
                features.Add("LiquorTwo");
            }

            if (!string.IsNullOrEmpty(_configuration["FEATURE_COVID_APPLICATION"]))
            {
                features.Add("CovidApplication");
            }

            if (!string.IsNullOrEmpty(_configuration["FEATURE_LG_APPROVALS"]))
            {
                features.Add("LGApprovals");
            }

            return features;
        }

    }
}
