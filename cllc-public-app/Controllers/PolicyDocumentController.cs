using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyDocumentController : ControllerBase
    {
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;
        private IMemoryCache _cache;

        public PolicyDocumentController(IDynamicsClient dynamicsClient, ILoggerFactory loggerFactory, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(PolicyDocumentController));
        }

        /// <summary>
        /// Get a list of all policy documents for a given category
        /// </summary>
        /// <param name="category">The policy document category</param>
        /// <returns></returns>
        [HttpGet()]
        [AllowAnonymous]
        public ActionResult GetPolicyDocuments(string category)
        {
            string cacheKey = CacheKeys.PolicyDocumentCategoryPrefix + category;
            string cacheAgeKey = CacheKeys.PolicyDocumentCategoryPrefix + category + "_dto";
            List<ViewModels.PolicyDocumentSummary> PolicyDocuments = null;
            bool fetchDocument = false;
            if (!_cache.TryGetValue(cacheKey, out PolicyDocuments))
            // item is not in cache at all, fetch.
            {
                fetchDocument = true;
            }
            else
            {
                DateTimeOffset dto = DateTimeOffset.Now;
                // fetch the age of the cache item from the cache
                if (!_cache.TryGetValue(cacheAgeKey, out dto))
                // unable to get cache age, fetch.
                {
                    fetchDocument = true;
                }
                else
                {
                    TimeSpan age = DateTimeOffset.Now - dto;
                    if (age.TotalMinutes > 10)
                    // More than 10 minutes old, fetch.
                    {
                        fetchDocument = true;
                    }
                }

            }

            if (fetchDocument)
            {
                // Key not in cache or cache expired, get data.
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

                    if (PolicyDocuments != null && PolicyDocuments.Count > 0)
                    {
                        // Set cache options.
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            // Set the cache to expire far in the future.                    
                            .SetAbsoluteExpiration(TimeSpan.FromDays(365 * 5));

                        // Save data in cache.
                        _cache.Set(cacheKey, PolicyDocuments, cacheEntryOptions);
                        _cache.Set(cacheAgeKey, DateTimeOffset.Now, cacheEntryOptions);
                    }

                }
                catch (HttpOperationException httpOperationException)
                {
                    // this will gracefully handle situations where Dynamics is not available however we have a cache version.
                    _logger.LogError(httpOperationException, "Error getting policy documents by category");
                }
                catch (Exception e)
                {
                    // this will gracefully handle situations where Dynamics is not available however we have a cache version.
                    _logger.LogError("Unknown error occured");
                    _logger.LogError(e.Message);
                }


            }

            if (PolicyDocuments == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new JsonResult(PolicyDocuments);
            }


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
            bool fetchDocument = false;
            string cacheKey = CacheKeys.PolicyDocumentPrefix + slug;
            string cacheAgeKey = CacheKeys.PolicyDocumentCategoryPrefix + slug + "_dto";
            if (!_cache.TryGetValue(cacheKey, out policyDocument))
            // item is not in cache at all, fetch.
            {
                fetchDocument = true;
            }
            else
            {
                DateTimeOffset dto = DateTimeOffset.Now;
                // fetch the age of the cache item from the cache
                if (!_cache.TryGetValue(cacheAgeKey, out dto))
                // unable to get cache age, fetch.
                {
                    fetchDocument = true;
                }
                else
                {
                    TimeSpan age = DateTimeOffset.Now - dto;
                    if (age.TotalMinutes > 10)
                    // More than 10 minutes old, fetch.
                    {
                        fetchDocument = true;
                    }
                }
            }
            if (fetchDocument)
            {
                try
                {
                    policyDocument = _dynamicsClient.GetPolicyDocumentBySlug(slug);

                    if (policyDocument != null) // handle case where the document is missing.
                    {
                        // Set cache options.
                        var newCacheEntryOptions = new MemoryCacheEntryOptions()
                            // Set the cache to expire far in the future.                    
                            .SetAbsoluteExpiration(TimeSpan.FromDays(365 * 5));

                        // Save data in cache.
                        _cache.Set(cacheKey, policyDocument, newCacheEntryOptions);
                        _cache.Set(cacheAgeKey, DateTimeOffset.Now, newCacheEntryOptions);
                    }
                    else
                    {
                        _logger.LogError($"Unable to get Policy Document {slug} - does it exist?");
                    }
                }
                catch (HttpOperationException httpOperationException)
                {
                    // this will gracefully handle situations where Dynamics is not available however we have a cache version.
                    _logger.LogError(httpOperationException, "Error getting policy document");
                }
                catch (Exception e)
                {
                    // unexpected exception
                    _logger.LogError(e, "Unknown error occured");
                }
            }

            if (policyDocument == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new JsonResult(policyDocument.ToViewModel());
            }
        }
    }
}
