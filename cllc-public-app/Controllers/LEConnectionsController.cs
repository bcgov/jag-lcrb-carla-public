using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/le-connections")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LeConnectionsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;

        public LeConnectionsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(LegalEntitiesController));
        }

        private List<SecurityScreeningStatusItem> GetConnectionsScreeningData(List<MicrosoftDynamicsCRMcontact> contacts)
        {
            var result = new List<SecurityScreeningStatusItem>();
            var addedContacts = new List<string>();


            foreach (var contact in contacts)
            {
                DateTimeOffset? dateSubmitted = null;


                if (contact?.Contactid != null && !addedContacts.Contains(contact.Contactid))
                {
                    // liquor
                    if (contact.AdoxioPhscomplete == (int)YesNoOptions.Yes)
                    {
                        dateSubmitted = contact.AdoxioPhsdatesubmitted;
                    }

                    // cannabis
                    if (contact.AdoxioCascomplete == (int)YesNoOptions.Yes)
                    {
                        dateSubmitted = contact.AdoxioCasdatesubmitted;
                    }

                    var newItem = new SecurityScreeningStatusItem
                    {
                        ContactId = contact.Contactid,
                        Contact = contact,
                        FirstName = contact.Firstname,
                        MiddleName = contact.Middlename,
                        LastName = contact.Lastname,
                        PhsLink = contact.PhsLink,
                        CasLink = contact.CasLink,
                        DateSubmitted = dateSubmitted,
                    };
                    result.Add(newItem);
                    addedContacts.Add(contact.Contactid); // remember added contacts to avoid duplicates
                }

            }

            return result;
        }

        [HttpGet("current-security-summary")]
        public JsonResult GetCurrentSecurityScreeningSummaryNew()
        {
            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // check that the session is setup correctly.
            userSettings.Validate();

            // get data for the current account. 

            string currentAccountId = userSettings.AccountId;

            var contacts = _dynamicsClient.GetLEConnectionsForAccount(userSettings.AccountId, _logger, _configuration);
            List<SecurityScreeningStatusItem> securityItems = GetConnectionsScreeningData(contacts);
            // get the current user's applications and licences
            var licences = _dynamicsClient.GetLicensesByLicencee(_cache, currentAccountId);
            var applications = _dynamicsClient.GetApplicationsForLicenceByApplicant(currentAccountId);

            SecurityScreeningSummary result = new SecurityScreeningSummary();

            // determine how many of each licence there are.
            int cannabisLicenceCount = 0;
            int liquorLicenceCount = 0; 
            

            if (licences != null && licences.Count() > 0)
            {
                cannabisLicenceCount = licences.Count(x => x.AdoxioLicenceType.AdoxioName.ToUpper().Contains("CANNABIS"));
                liquorLicenceCount = licences.Count() - cannabisLicenceCount;
            }

            // determine how many applications of each type there are.
            int cannabisApplicationCount = 0;
            int liquorApplicationCount = 0;

            if (applications != null && applications.Count() > 0)
            {
                cannabisApplicationCount = applications.Count(x => x.AdoxioApplicationTypeId.AdoxioName.ToUpper().Contains("CANNABIS"));
                liquorApplicationCount = applications.Count() - cannabisApplicationCount;
            }
            

            if (cannabisLicenceCount > 0 || cannabisApplicationCount > 0)
            {
                var data = securityItems.Select(item => {
                    item.IsComplete =  (item.Contact?.AdoxioCascomplete == (int)YesNoOptions.Yes);
                    return item;
                });
                result.Cannabis = new SecurityScreeningCategorySummary()
                {
                    CompletedItems = data.Where(item => item.IsComplete).ToList(),
                    OutstandingItems = data.Where(item => !item.IsComplete).ToList()
                };
            }

            if (liquorLicenceCount > 0 || liquorApplicationCount > 0)
            {
                var data = securityItems.Select(item => {
                    item.IsComplete =  (item.Contact?.AdoxioPhscomplete == (int)YesNoOptions.Yes);
                    return item;
                });
                result.Liquor = new SecurityScreeningCategorySummary()
                {
                    CompletedItems = data.Where(item => item.IsComplete).ToList(),
                    OutstandingItems = data.Where(item => !item.IsComplete).ToList()
                };

            }

            return new JsonResult(result);
        }
    }
}
