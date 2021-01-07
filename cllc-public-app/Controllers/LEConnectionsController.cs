using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Google.Protobuf;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using Microsoft.Extensions.Caching.Memory;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/le-connections")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LEConnectionsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly string _encryptionKey;
        private readonly FileManagerClient _fileManagerClient;

        public LEConnectionsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, FileManagerClient fileClient, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _encryptionKey = _configuration["ENCRYPTION_KEY"];
            _logger = loggerFactory.CreateLogger(typeof(LegalEntitiesController));
            _fileManagerClient = fileClient;
        }

        private List<SecurityScreeningStatusItem> GetConnectionsScreeningData(List<MicrosoftDynamicsCRMcontact> contacts)
        {
            var result = new List<SecurityScreeningStatusItem>();
            var addedContacts = new List<string>();


            foreach (var contact in contacts)
            {
                DateTimeOffset? dateSubmitted = null;


                if (!addedContacts.Any(item => item == contact.Contactid))
                {
                    // liquor
                    if (contact?.AdoxioPhscomplete == (int)YesNoOptions.Yes)
                    {
                        dateSubmitted = contact.AdoxioPhsdatesubmitted;
                    }

                    // cannabis
                    if (contact?.AdoxioCascomplete == (int)YesNoOptions.Yes)
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
            int cannabisLicenceCount = licences.Count(x => x.AdoxioLicenceType.AdoxioName.ToUpper().Contains("CANNABIS"));
            int liquorLicenceCount = licences.Count() - cannabisLicenceCount;

            // determine how many applications of each type there are.
            int cannabisApplicationCount = applications.Count(x => x.AdoxioApplicationTypeId.AdoxioName.ToUpper().Contains("CANNABIS"));
            int liquorApplicationCount = applications.Count() - cannabisApplicationCount;

            if (cannabisLicenceCount > 0 || cannabisApplicationCount > 0)
            {
                var data = securityItems.Select(item => {
                    item.IsComplete =  (item?.Contact?.AdoxioCascomplete == (int)YesNoOptions.Yes);
                    return item;
                });
                result.Cannabis = new SecurityScreeningCategorySummary();
                result.Cannabis.CompletedItems = data.Where(item => item.IsComplete).ToList();
                result.Cannabis.OutstandingItems = data.Where(item => !item.IsComplete).ToList();
            }

            if (liquorLicenceCount > 0 || liquorApplicationCount > 0)
            {
                var data = securityItems.Select(item => {
                    item.IsComplete =  (item?.Contact?.AdoxioPhscomplete == (int)YesNoOptions.Yes);
                    return item;
                });
                result.Liquor = new SecurityScreeningCategorySummary();
                result.Liquor.CompletedItems = data.Where(item => item.IsComplete).ToList();
                result.Liquor.OutstandingItems = data.Where(item => !item.IsComplete).ToList(); ;
            }

            return new JsonResult(result);
        }
    }
}
