extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System;
using Gov.Lclb.Cllb.Public.Utils;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IDataverseClient _dataverse;
        private readonly BCeIDBusinessQuery _bceid;
        private readonly ILogger _logger;

        public UserController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IDataverseClient dataverse, BCeIDBusinessQuery bceid, ILoggerFactory loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dataverse = dataverse;
            _bceid = bceid;
            _logger = loggerFactory.CreateLogger(typeof(UserController));
        }

        protected ClaimsPrincipal CurrentUser => _httpContextAccessor.HttpContext.User;

        [HttpGet("current")]
        public async virtual Task<IActionResult> UsersCurrentGet()
        {
            SiteMinderAuthOptions siteMinderAuthOptions = new SiteMinderAuthOptions();
            ViewModels.User user = new ViewModels.User();

            bool sessionHadUserSettings = _httpContextAccessor.HttpContext.Session.GetString("UserSettings") != null;
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            _logger.LogInformation(
                "UsersCurrentGet: sessionHadUserSettings={SessionHad}, AuthenticatedUser==null:{AuthNull}, IsNewUserRegistration(from session)={IsNew}, SiteMinderGuid={Guid}, SessionId={SessionId}",
                sessionHadUserSettings, userSettings.AuthenticatedUser == null, userSettings.IsNewUserRegistration,
                userSettings.SiteMinderGuid, _httpContextAccessor.HttpContext.Session.Id);

            user.id = userSettings.UserId;
            TempAuthDebug.LogHeaders(_logger, nameof(UsersCurrentGet), _httpContextAccessor.HttpContext.Request.Headers);
            TempAuthDebug.Log(_logger, nameof(UsersCurrentGet),
                "Session userSettings: " + TempAuthDebug.Json(userSettings));
            user.contactid = userSettings.ContactId;
            user.accountid = userSettings.AccountId;
            user.businessname = userSettings.BusinessLegalName;
            user.name = userSettings.UserDisplayName;
            user.UserType = userSettings.UserType;

            // Bridge login can have an account before a Dynamics contact exists.
            var contactId = Guid.TryParse(userSettings.ContactId, out var id) && id != Guid.Empty
                ? id : userSettings.AuthenticatedUser?.ContactId ?? Guid.Empty;
            var contact = contactId != Guid.Empty
                ? await _dataverse.GetContactByIdAsync(contactId.ToString())
                : await _dataverse.GetContactByExternalIdAsync(userSettings.SiteMinderGuid);
            if (contact != null)
            {
                user.contactid = contact.Id.ToString();
                user.firstname = contact.FirstName;
                user.lastname = contact.LastName;
                user.email = contact.EMailAddress1;
                user.name = string.IsNullOrWhiteSpace(contact.FullName)
                    ? $"{contact.FirstName} {contact.LastName}".Trim()
                    : contact.FullName;
            }
            else
            {
                userSettings.IsNewUserRegistration = true;
            }

            if (userSettings.IsNewUserRegistration)
            {
                user.isNewUser = true;
                user.accountid = userSettings.AccountId;

                if (contact == null)
                {
                    var headers = _httpContextAccessor.HttpContext.Request.Headers;
                    var tokenContact = new ViewModels.Contact();
                    tokenContact.CopyHeaderValues(headers, useDisplayName: false);
                    user.email = tokenContact.emailaddress1;
                    user.lastname = tokenContact.lastname;
                    user.firstname = tokenContact.firstname;

                    string siteminderBusinessGuid = _httpContextAccessor.HttpContext.Request.Headers[siteMinderAuthOptions.SiteMinderBusinessGuidKey];
                    string siteminderUserGuid = _httpContextAccessor.HttpContext.Request.Headers[siteMinderAuthOptions.SiteMinderUserGuidKey];

                    if ((string.IsNullOrWhiteSpace(user.firstname) || string.IsNullOrWhiteSpace(user.lastname)) &&
                        Guid.TryParse(userSettings.SiteMinderGuid, out _))
                    {
                        var business = await _bceid.ProcessBusinessQuery(userSettings.SiteMinderGuid);
                        var basic = business == null ? await _bceid.ProcessBasicQuery(userSettings.SiteMinderGuid) : null;
                        if (string.IsNullOrWhiteSpace(user.firstname)) user.firstname = business?.individualFirstname ?? basic?.individualFirstname;
                        if (string.IsNullOrWhiteSpace(user.lastname)) user.lastname = business?.individualSurname ?? basic?.individualSurname;
                    }
                    if (string.IsNullOrWhiteSpace(user.firstname)) user.firstname = user.name?.GetFirstName();
                    if (string.IsNullOrWhiteSpace(user.lastname)) user.lastname = user.name?.GetLastName();
                    user.name = $"{user.firstname} {user.lastname}".Trim();

                    TempAuthDebug.Log(_logger, nameof(UsersCurrentGet),
                        "New user after BCeID lookup: " + TempAuthDebug.Json(user));

                    user.contactid = string.IsNullOrEmpty(siteminderUserGuid) ? userSettings.ContactId : siteminderUserGuid;
                    if (string.IsNullOrEmpty(user.contactid))
                        user.contactid = userSettings.SiteMinderGuid;

                    user.accountid = userSettings.AccountId ?? siteminderBusinessGuid;
                }
                user.isEligibilityRequired = true;
            }
            else
            {
                user.isNewUser = false;
                if (!string.IsNullOrEmpty(user.accountid))
                {
                    user.isEligibilityRequired = await EligibilityController.IsEligibilityCheckRequiredAsync(user.accountid, _configuration, _dataverse);
                    user.isPoliceRepresentative = await _dataverse.IsAccountSepPoliceRepresentativeAsync(user.accountid);
                }
            }

            TempAuthDebug.Log(_logger, nameof(UsersCurrentGet),
                "Returning to frontend: " + TempAuthDebug.Json(user));
            return new JsonResult(user);
        }
    }
}
