extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class TermsAndConditionsController : ControllerBase
    {
        private readonly IDataverseClient _dataverse;
        private readonly ILogger _logger;

        public TermsAndConditionsController(ILoggerFactory loggerFactory, IDataverseClient dataverse)
        {
            _dataverse = dataverse;
            _logger = loggerFactory.CreateLogger(typeof(TermsAndConditionsController));
        }

        [HttpGet("{licenceId}")]
        public async Task<JsonResult> GetTermsAndConditionsForLicence(string licenceId)
        {
            var result = new List<TermsAndConditions>();
            var terms = await _dataverse.GetTermsConditionsByLicenceIdAsync(licenceId);

            foreach (var term in terms)
            {
                bool? isDefault = null;
                if (term.adoxio_TermsConditionsPreset?.Id != null)
                {
                    var preset = await _dataverse.GetTermsConditionsPresetByIdAsync(
                        term.adoxio_TermsConditionsPreset.Id.ToString());
                    isDefault = preset?.adoxio_IsDefault;
                }

                result.Add(new TermsAndConditions
                {
                    Id = term.Id.ToString(),
                    LicenceId = term.adoxio_Licence?.Id.ToString(),
                    Content = term.adoxio_TermsandConditions,
                    IsDefault = isDefault
                });
            }

            return new JsonResult(result);
        }

        [HttpGet("term/{termId}")]
        public async Task<TermsAndConditions> GetTermsAndCondition(string termId)
        {
            var term = await _dataverse.GetTermsConditionsByIdAsync(termId);
            if (term == null) return null;

            bool? isDefault = null;
            if (term.adoxio_TermsConditionsPreset?.Id != null)
            {
                var preset = await _dataverse.GetTermsConditionsPresetByIdAsync(
                    term.adoxio_TermsConditionsPreset.Id.ToString());
                isDefault = preset?.adoxio_IsDefault;
            }

            return new TermsAndConditions
            {
                Id = term.Id.ToString(),
                LicenceId = term.adoxio_Licence?.Id.ToString(),
                Content = term.adoxio_TermsandConditions,
                IsDefault = isDefault
            };
        }
    }
}
