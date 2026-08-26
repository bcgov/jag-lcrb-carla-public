extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EligibilityController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDataverseClient _dataverse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        public EligibilityController(IConfiguration configuration, IDataverseClient dataverse, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _dataverse = dataverse;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(EligibilityController));
            _env = env;
        }

        public static async Task<bool> IsEligibilityCheckRequiredAsync(string accountId, IConfiguration config, IDataverseClient dataverse)
        {
            if (config["FEATURE_ELIGIBILITY"] == null || string.IsNullOrEmpty(accountId) || accountId.Equals("00000000-0000-0000-0000-000000000000"))
                return false;

            var applicationType = await dataverse.GetApplicationTypeByNameAsync("Cannabis Retail Store");
            if (applicationType == null) return false;

            var statusCodes = new List<int>
            {
                (int)AdoxioApplicationStatusCodes.InProgress,
                (int)AdoxioApplicationStatusCodes.Intake,
                (int)AdoxioApplicationStatusCodes.UnderReview
            };

            var applications = await dataverse.GetApplicationsByApplicantTypeAndStatusesAsync(
                accountId,
                applicationType.adoxio_applicationtypeId?.ToString() ?? "",
                statusCodes);

            bool cannabisApplicationInProgress = applications.Count > 0;

            if (!string.IsNullOrEmpty(accountId) && Guid.Parse(accountId) != Guid.Empty)
            {
                var account = await dataverse.GetAccountByIdAsync(accountId);
                return (account?.adoxio_iseligibilitycertified == null || account.adoxio_iseligibilitycertified == false)
                    && cannabisApplicationInProgress;
            }
            return false;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitEligibilityForm([FromBody] EligibilityForm form)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            if (string.IsNullOrEmpty(userSettings.AccountId)) return new NotFoundResult();

            Guid accountId = new Guid(userSettings.AccountId);
            var existing = await _dataverse.GetAccountByIdAsync(accountId.ToString());
            if (existing == null)
            {
                _logger.LogError($"Account {accountId} NOT found.");
                return new NotFoundResult();
            }

            var patchAccount = new DV::Gov.Lclb.Cllb.Interfaces.Account { Id = accountId };
            if (form.IsEligibilityCertified)
            {
                patchAccount.adoxio_isconnectiontounlicensedstore = form.IsConnectedToUnlicencedStore;
                patchAccount.adoxio_namelocationunlicensedretailer = form.NameLocationUnlicencedRetailer;
                patchAccount.adoxio_isretailerstilloperating = form.IsRetailerStillOperating;
                patchAccount.adoxio_DateOperationsCeased = form.DateOperationsCeased?.UtcDateTime;
                patchAccount.adoxio_isinvolvedillegaldistribution = form.IsInvolvedIllegalDistribution;
                patchAccount.adoxio_illegaldistributioninvolvementdetails = form.IllegalDistributionInvolvementDetails;
                patchAccount.adoxio_namelocationretailer = form.NameLocationRetailer;
                patchAccount.adoxio_isinvolvementcontinuing = form.IsInvolvementContinuing;
                patchAccount.adoxio_dateinvolvementceased = form.DateInvolvementCeased?.UtcDateTime;
                patchAccount.adoxio_iseligibilitycertified = form.IsEligibilityCertified;
                patchAccount.adoxio_eligibilitysignature = form.EligibilitySignature;
                patchAccount.adoxio_datesignordismissed = form.DateSignedOrDismissed?.UtcDateTime;
            }
            else
            {
                patchAccount.adoxio_iseligibilitycertified = false;
                patchAccount.adoxio_datesignordismissed = form.DateSignedOrDismissed?.UtcDateTime;
            }
            await _dataverse.UpdateAccountAsync(patchAccount);
            return new JsonResult("Ok");
        }
    }
}
