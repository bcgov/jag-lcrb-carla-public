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
    public class PolicyDocumentController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;
        private IMemoryCache _cache;

        public PolicyDocumentController(IDynamicsClient dynamicsClient, IConfiguration configuration, ILoggerFactory loggerFactory, IMemoryCache memoryCache)
        {
            Configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(PolicyDocumentController));
            _cache = memoryCache;
        }

        /// <summary>
        /// Get a list of all policy documents for a given category
        /// </summary>
        /// <param name="category">The policy document category</param>
        /// <returns></returns>
        [HttpGet()]
        [AllowAnonymous]
        public JsonResult GetPolicyDocuments(string category)
        {
            string cacheKey = CacheKeys.PolicyDocumentCategoryPrefix + category;
            List<ViewModels.PolicyDocumentSummary> PolicyDocuments = null;

            if (!_cache.TryGetValue(cacheKey, out PolicyDocuments))
            {
                // Key not in cache, so get data.
                try
                {
                    if (string.IsNullOrEmpty(category))
                    {
                        PolicyDocuments = _dynamicsClient.Policydocuments.Get().Value
                        .OrderBy(x => x.AdoxioDisplayorder)
                        .Select(x => x.ToSummaryViewModel())
                        .ToList();
                    }
                    else
                    {
                        category = category.Replace("'", "''");
                        string filter = "adoxio_category eq '" + category + "'";
                        PolicyDocuments = _dynamicsClient.Policydocuments.Get(filter: filter).Value
                        .OrderBy(x => x.AdoxioDisplayorder)
                        .Select(x => x.ToSummaryViewModel())
                        .ToList();
                    }
                }
                catch (OdataerrorException odee)
                {
                    _logger.LogError("Error getting policy documents by category");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                }

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                // Save data in cache.
                _cache.Set(cacheKey, PolicyDocuments, cacheEntryOptions);
            }
            

            return Json(PolicyDocuments);
        }

        /// <summary>
        /// Get a specific policy document
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [HttpGet("{slug}")]
        [AllowAnonymous]
        public ActionResult GetPolicy(string slug)
        {
            MicrosoftDynamicsCRMadoxioPolicydocument policyDocument = null;

            string cacheKey = CacheKeys.PolicyDocumentPrefix + slug;
            if (!_cache.TryGetValue(cacheKey, out policyDocument))
            {
                // Key not in cache, so get data.
                policyDocument = _dynamicsClient.GetPolicyDocumentBySlug(slug);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                // Save data in cache.
                _cache.Set(cacheKey, policyDocument, cacheEntryOptions);

            }

            if (policyDocument == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return Json(policyDocument.ToViewModel());
            }            
        }
    }
}
