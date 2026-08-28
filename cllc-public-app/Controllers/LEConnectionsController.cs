extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Gov.Lclb.Cllb.Public.Utility;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/le-connections")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LeConnectionsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDataverseClient _dataverse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public LeConnectionsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDataverseClient dataverse)
        {
            _configuration = configuration;
            _dataverse = dataverse;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(LegalEntitiesController));
        }

        private List<SecurityScreeningStatusItem> GetConnectionsScreeningData(IList<DV::Gov.Lclb.Cllb.Interfaces.Contact> contacts)
        {
            var result = new List<SecurityScreeningStatusItem>();
            var addedContacts = new List<string>();

            foreach (var contact in contacts)
            {
                var contactId = contact.Id.ToString();
                if (!addedContacts.Contains(contactId))
                {
                    DateTimeOffset? dateSubmitted = null;
                    bool phsComplete = contact.adoxio_PHSComplete == adoxio_contact_adoxio_phscomplete.Yes;
                    bool casComplete = contact.adoxio_cascomplete == adoxio_contact_adoxio_cascomplete.Yes;

                    if (phsComplete) dateSubmitted = contact.adoxio_PHSDateSubmitted.HasValue ? (DateTimeOffset?)contact.adoxio_PHSDateSubmitted.Value : null;
                    if (casComplete) dateSubmitted = contact.adoxio_casdatesubmitted.HasValue ? (DateTimeOffset?)contact.adoxio_casdatesubmitted.Value : null;

                    result.Add(new SecurityScreeningStatusItem
                    {
                        ContactId = contactId,
                        FirstName = contact.FirstName,
                        MiddleName = contact.MiddleName,
                        LastName = contact.LastName,
                        Birthdate = contact.BirthDate?.Date,
                        PhsLink = GetPhsLink(contactId),
                        CasLink = GetCasLink(contactId),
                        DateSubmitted = dateSubmitted,
                        PhsIsCompleted = phsComplete,
                        CasIsCompleted = casComplete,
                    });
                    addedContacts.Add(contactId);
                }
            }
            return result;
        }

        private string GetPhsLink(string contactId)
        {
            string result = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/personal-history-summary/";
            string encryptionKey = _configuration["ENCRYPTION_KEY"];
            result += HttpUtility.UrlEncode(EncryptionUtility.EncryptStringHex(contactId, encryptionKey));
            return result;
        }

        private string GetCasLink(string contactId)
        {
            string result = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/cannabis-associate-screening/";
            string encryptionKey = _configuration["ENCRYPTION_KEY"];
            result += HttpUtility.UrlEncode(EncryptionUtility.EncryptStringHex(contactId, encryptionKey));
            return result;
        }

        [HttpGet("current-security-summary")]
        public async Task<JsonResult> GetCurrentSecurityScreeningSummaryNew()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            userSettings.Validate();
            string currentAccountId = userSettings.AccountId;

            var contacts = await _dataverse.GetLeConnectionContactsAsync(currentAccountId);
            var securityItems = GetConnectionsScreeningData(contacts);

            var licences = await _dataverse.GetLicencesByAccountIdAsync(currentAccountId);
            var applications = await _dataverse.GetApplicationsForLicenceByApplicantAsync(currentAccountId);

            var result = new SecurityScreeningSummary();

            int cannabisLicenceCount = 0;
            int liquorLicenceCount = 0;
            foreach (var licence in licences)
            {
                if ((int?)licence.statuscode == (int)LicenceStatusCodes.Cancelled || (int?)licence.statuscode == (int)LicenceStatusCodes.Inactive)
                    continue;
                if (licence.adoxio_LicenceType?.Id != null)
                {
                    var licenceType = await _dataverse.GetLicenceTypeByIdAsync(licence.adoxio_LicenceType.Id.ToString());
                    if (licenceType?.adoxio_name?.ToUpper().Contains("CANNABIS") == true)
                        cannabisLicenceCount++;
                    else
                        liquorLicenceCount++;
                }
                else
                {
                    liquorLicenceCount++;
                }
            }

            int cannabisApplicationCount = 0;
            int liquorApplicationCount = 0;
            if (applications?.Count > 0)
            {
                foreach (var app in applications)
                {
                    if (app.adoxio_ApplicationTypeId?.Id != null)
                    {
                        var appType = await _dataverse.GetApplicationTypeByIdAsync(app.adoxio_ApplicationTypeId.Id.ToString());
                        if (appType?.adoxio_name?.ToUpper().Contains("CANNABIS") == true)
                            cannabisApplicationCount++;
                        else
                            liquorApplicationCount++;
                    }
                    else
                    {
                        liquorApplicationCount++;
                    }
                }
            }

            if (cannabisLicenceCount > 0 || cannabisApplicationCount > 0)
            {
                var data = securityItems.Select(item =>
                {
                    item.IsComplete = item.CasIsCompleted;
                    return item;
                });
                result.Cannabis = new SecurityScreeningCategorySummary
                {
                    CompletedItems = data.Where(item => item.IsComplete).ToList(),
                    OutstandingItems = data.Where(item => !item.IsComplete).ToList()
                };
            }

            if (liquorLicenceCount > 0 || liquorApplicationCount > 0)
            {
                var data = securityItems.Select(item =>
                {
                    item.IsComplete = item.PhsIsCompleted;
                    return item;
                });
                result.Liquor = new SecurityScreeningCategorySummary
                {
                    CompletedItems = data.Where(item => item.IsComplete).ToList(),
                    OutstandingItems = data.Where(item => !item.IsComplete).ToList()
                };
            }

            return new JsonResult(result);
        }
    }
}
