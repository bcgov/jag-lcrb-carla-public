extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class PoliceJurisdictionsController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IDataverseClient _dataverse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPdfService _pdfClient;
        private readonly ILogger _logger;

        public PoliceJurisdictionsController(IDataverseClient dataverse, IHttpContextAccessor httpContextAccessor,
            IPdfService pdfClient, ILoggerFactory loggerFactory, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _dataverse = dataverse;
            _httpContextAccessor = httpContextAccessor;
            _pdfClient = pdfClient;
            _logger = loggerFactory.CreateLogger(typeof(PoliceJurisdictionsController));
        }

        [HttpGet("autocomplete")]
        public async Task<ActionResult> GetPoliceJurisdictions(string name)
        {
            var results = new List<AutoCompleteListItem>();
            try
            {
                var policeJurisdictions = await _dataverse.GetPoliceJurisdictionsAsync(nameContains: name);
                foreach (var pj in policeJurisdictions)
                {
                    results.Add(new AutoCompleteListItem
                    {
                        Id = pj.Id.ToString(),
                        Name = pj.adoxio_name
                    });
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error getting Police Jurisdictions");
                throw;
            }

            return new JsonResult(results);
        }
    }

    public class AutoCompleteListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
