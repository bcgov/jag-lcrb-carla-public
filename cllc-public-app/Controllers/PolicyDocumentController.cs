extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyDocumentController : ControllerBase
    {
        private readonly IDataverseClient _dataverse;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;

        public PolicyDocumentController(IDataverseClient dataverse, ILoggerFactory loggerFactory, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _dataverse = dataverse;
            _logger = loggerFactory.CreateLogger(typeof(PolicyDocumentController));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetPolicyDocuments(string category)
        {
            string cacheKey = CacheKeys.PolicyDocumentCategoryPrefix + category;
            string cacheAgeKey = CacheKeys.PolicyDocumentCategoryPrefix + category + "_dto";
            List<ViewModels.PolicyDocumentSummary> policyDocuments = null;
            bool fetchDocument = false;

            if (!_cache.TryGetValue(cacheKey, out policyDocuments))
            {
                fetchDocument = true;
            }
            else
            {
                if (!_cache.TryGetValue(cacheAgeKey, out DateTimeOffset dto))
                {
                    fetchDocument = true;
                }
                else if ((DateTimeOffset.Now - dto).TotalMinutes > 10)
                {
                    fetchDocument = true;
                }
            }

            if (fetchDocument)
            {
                try
                {
                    var docs = await _dataverse.GetPolicyDocumentsAsync(string.IsNullOrEmpty(category) ? null : category);
                    policyDocuments = docs
                        .OrderBy(x => x.adoxio_DisplayOrder)
                        .Select(x => x.ToSummaryViewModel())
                        .ToList();

                    if (policyDocuments?.Count > 0)
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromDays(365 * 5));
                        _cache.Set(cacheKey, policyDocuments, cacheEntryOptions);
                        _cache.Set(cacheAgeKey, DateTimeOffset.Now, cacheEntryOptions);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting policy documents by category");
                }
            }

            if (policyDocuments == null) return new NotFoundResult();
            return new JsonResult(policyDocuments);
        }

        [HttpGet("{slug}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetPolicy(string slug)
        {
            string cacheKey = CacheKeys.PolicyDocumentPrefix + slug;
            string cacheAgeKey = CacheKeys.PolicyDocumentCategoryPrefix + slug + "_dto";
            adoxio_policydocument policyDocument = null;
            bool fetchDocument = false;

            if (!_cache.TryGetValue(cacheKey, out policyDocument))
            {
                fetchDocument = true;
            }
            else
            {
                if (!_cache.TryGetValue(cacheAgeKey, out DateTimeOffset dto))
                {
                    fetchDocument = true;
                }
                else if ((DateTimeOffset.Now - dto).TotalMinutes > 10)
                {
                    fetchDocument = true;
                }
            }

            if (fetchDocument)
            {
                try
                {
                    policyDocument = await _dataverse.GetPolicyDocumentBySlugAsync(slug);
                    if (policyDocument != null)
                    {
                        var newCacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromDays(365 * 5));
                        _cache.Set(cacheKey, policyDocument, newCacheEntryOptions);
                        _cache.Set(cacheAgeKey, DateTimeOffset.Now, newCacheEntryOptions);
                    }
                    else
                    {
                        _logger.LogError($"Unable to get Policy Document {slug} - does it exist?");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting policy document");
                }
            }

            if (policyDocument == null) return new NotFoundResult();
            return new JsonResult(policyDocument.ToViewModel());
        }
    }
}
