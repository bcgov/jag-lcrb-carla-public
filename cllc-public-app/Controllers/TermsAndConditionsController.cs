using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class TermsAndConditionsController : ControllerBase
    {
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;

        public TermsAndConditionsController(ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(TiedHouseConnectionsController));
        }

        [HttpGet("{licenceId}")]
        public JsonResult GetTermsAndConditions(string licenceId)
        {
            var result = new List<ViewModels.TermsAndConditions>();
            IEnumerable<MicrosoftDynamicsCRMadoxioApplicationtermsconditionslimitation> terms = null;
            string licenceFilter = "_adoxio_licence_value eq " + licenceId;
            
            terms = _dynamicsClient.Applicationtermsconditionslimitations.Get(filter: licenceFilter).Value;

            foreach (var term in terms)
            {
                result.Add(term.ToViewModel());
            }

            return new JsonResult(result);
        }
    }
}
