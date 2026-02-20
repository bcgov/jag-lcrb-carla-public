using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class FeaturesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public class FeatureFlag
        {
            public string Name { get; set; }
            public bool Enabled { get; set; }
            public string Value { get; set; }
        }

        // The list of all application features
        // The first item of the tuple is the feature in in configuration
        // The second item is the name of the feature used by the Angular client
        readonly List<(string, string)> features = new List<(string, string)>
        {
            ("FEATURE_INDIGENOUS_NATION", "IndigenousNation"),
            ("FEATURE_MAPS", "Maps"),
            ("FEATURE_LICENSEE_CHANGES", "LicenseeChanges"),
            ("FEATURE_NO_WET_SIGNATURE", "NoWetSignature"),
            ("FEATURE_SECURITY_SCREENING", "SecurityScreening"),
            ("FEATURE_DISABLE_LOGIN", "DisableLogin"),
            ("FEATURE_LIQUOR_ONE", "LiquorOne"),
            ("FEATURE_LIQUOR_TWO", "LiquorTwo"),
            ("FEATURE_LIQUOR_THREE", "LiquorThree"),
            ("FEATURE_F2G", "F2G"),
            ("FEATURE_COVID_APPLICATION", "CovidApplication"),
            ("FEATURE_LG_APPROVALS", "LGApprovals"),
            ("FEATURE_CASS", "CASS"),
            ("FEATURE_MARKET_EVENTS", "MarketEvents"),
            ("FEATURE_PERMANENT_CHANGES_TO_LICENSEE", "PermanentChangesToLicensee"),
            ("FEATURE_LEGAL_ENTITY_REVIEW", "LegalEntityReview"),
            ("FEATURE_PERMANENT_CHANGES_TO_APPLICANT", "PermanentChangesToApplicant"),
            ("FEATURE_NOTICES", "Notices"),
            ("FEATURE_LE_CONNECTIONS", "LEConnections"),
            ("FEATURE_TUA_EVENTS", "TemporaryUseAreaEvents"),
            ("FEATURE_ELIGIBILITY", "Eligibility"),
            ("FEATURE_RLRS", "RLRS"), // used to switch to the RLRS and disable the RAS intake
            ("FEATURE_LIQUOR_FREE_EVENTS", "LiquorFreeEvents"), // All-ages Liquor-Free events for Liquor Primary and Liquor Primary Club
            ("FEATURE_SEP", "Sep"), // Controls Special Event Permits content
            ("FEATURE_DISABLE_WORKER_QUALIFICATION", "DisableWorkerQualification"), // Removes worker qualification from the portal
            ("FEATURE_TAKE_HOME_EVENTS", "TakeHomeEvents"), // Take Home Public Sampling events - covers samples handed out in public for home consumption
            ("FEATURE_BRIDGE_LOGIN", "BridgeLogin"), // Login using bridge entity
            ("FEATURE_PRS_ENABLED", "PrsEnabled"),
            ("FEATURE_ORV_ENABLED", "ORVEnabled"), // Online Retailer Verification (ORV) enabled for the portal
            ("FEATURE_AI_ASSISTANT", "AIAssistant"), // AI Assistant sidebar for logged-in users
            ("FEATURE_DISABLE_LICENCE_RENEWAL_DATE", "DisableLicenceRenewalDate"), // Hides the renewal action for licences that expire on or after the specified date.
            // CONFIG SETTINGS - MONTHLY_REPORTS_MAX_MONTHS - number of months that will be used on the monthly reports.
        };

        public FeaturesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Returns an array of all feature flags.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<FeatureFlag> GetFeatureList()
        {
            var featureFlags = new List<FeatureFlag>();

            foreach (var feature in this.features)
            {
                var featureFlagValue = _configuration[feature.Item1];

                featureFlags.Add(
                    new FeatureFlag
                    {
                        Name = feature.Item2,
                        Enabled = !string.IsNullOrEmpty(featureFlagValue),
                        Value = featureFlagValue,
                    }
                );
            }

            return featureFlags;
        }
    }
}
