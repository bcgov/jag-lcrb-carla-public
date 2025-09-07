using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
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
            _logger = loggerFactory.CreateLogger(typeof(TermsAndConditionsController));
        }

        [HttpGet("{licenceId}")]
        public JsonResult GetTermsAndConditionsForLicence(string licenceId)
        {
            var result = new List<ViewModels.TermsAndConditions>();
            IEnumerable<MicrosoftDynamicsCRMadoxioApplicationtermsconditionslimitation> terms = null;
            string licenceFilter = "_adoxio_licence_value eq " + licenceId;
            string[] expand = { "adoxio_TermsConditionsPreset" };


            terms = _dynamicsClient.Applicationtermsconditionslimitations.Get(filter: licenceFilter, expand: expand).Value;

            foreach (var term in terms)
            {
                result.Add(term.ToViewModel());
            }

            return new JsonResult(result);
        }

        [HttpGet("term/{termId}")]
        public TermsAndConditions GetTermsAndCondition(string termId)
        {
            var term =  _dynamicsClient.Applicationtermsconditionslimitations.GetByKey(termId);

            return term.ToViewModel();
        }

    }
}
