using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LocalGovernmentsController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPdfService _pdfClient;
        private readonly ILogger _logger;

        public LocalGovernmentsController(IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor,
            IPdfService pdfClient, ILoggerFactory loggerFactory, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _pdfClient = pdfClient;
            _logger = loggerFactory.CreateLogger(typeof(LicensesController));

        }

        /// GET the list of local governments
        [HttpGet("autocomplete")]
        public ActionResult GetLocalGovernments(string name)
        {
            var results = new List<LGListItem>();

            try
            {
                string filter = null;
                // escape any apostophes.
                if (name != null)
                {
                    name = name.Replace("'", "''");
                    filter = $"contains(adoxio_name,'{name}')";
                }

                var localGovernments = _dynamicsClient.Localgovindigenousnations.Get(filter: filter).Value;
                foreach (var lg in localGovernments)
                {
                    var accountFilter = $"_adoxio_lginlinkid_value eq {lg.AdoxioLocalgovindigenousnationid} and websiteurl ne null";
                    var linkedAccount = _dynamicsClient.Accounts.Get(filter: accountFilter).Value.FirstOrDefault();
                    var item = new LGListItem()
                    {
                        Id = lg.AdoxioLocalgovindigenousnationid,
                        Name = lg.AdoxioName,
                        WebsiteUrl = linkedAccount?.Websiteurl
                    };
                    results.Add(item);
                }

            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting Local Governments");
                // fail if we can't create.
                throw (httpOperationException);
            }
            return new JsonResult(results);
        }

    }

    public class LGListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string WebsiteUrl { get; set; }
    }
}
