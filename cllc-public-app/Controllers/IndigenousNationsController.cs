using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Gov.Lclb.Cllb.Interfaces.SharePointFileManager;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class IndigenousNationsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IDynamicsClient _dynamicsClient;

        public IndigenousNationsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            Configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(IndigenousNationsController));
        }



        [HttpGet()]
        public async Task<IActionResult> Index()
        {
            try
            {
                var nations = await _dynamicsClient.Localgovindigenousnations.GetAsync(filter: "adoxio_isindigenousnation eq true");
                var result = new List<IndigenousNation>();
                foreach (var item in nations.Value)
                {
                    result.Add(item.ToViewModel());
                }
                return new JsonResult(result);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating application");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail if we can't create.
                throw (odee);
            }
        }

    }
}
