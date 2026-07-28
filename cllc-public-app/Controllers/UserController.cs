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
            user.contactid = userSettings.ContactId;
            user.accountid = userSettings.AccountId;
            user.businessname = userSettings.BusinessLegalName;
            user.name = userSettings.UserDisplayName;
            user.UserType = userSettings.UserType;

            if (userSettings.AuthenticatedUser == null)
            {
                try
                {
                    var contact = await _dataverse.GetContactByExternalIdAsync(userSettings.SiteMinderGuid);
                    if (contact != null)
                    {
                        userSettings.AuthenticatedUser = new Models.User();
                        userSettings.AuthenticatedUser.FromContact(contact);
                        _logger.LogInformation("UsersCurrentGet: contact FOUND via GetContactByExternalIdAsync for SiteMinderGuid={Guid}, ContactId={ContactId}", userSettings.SiteMinderGuid, contact.Id);
                    }
                    else
                    {
                        userSettings.IsNewUserRegistration = true;
                        _logger.LogWarning("UsersCurrentGet: contact NOT FOUND via GetContactByExternalIdAsync for SiteMinderGuid={Guid} — treating as new user", userSettings.SiteMinderGuid);
                    }
                }
                catch (Exception ex)
                {
                    userSettings.IsNewUserRegistration = true;
                    _logger.LogError(ex, "UsersCurrentGet: exception looking up contact for SiteMinderGuid={Guid} — treating as new user", userSettings.SiteMinderGuid);
                }
            }

            if (userSettings.IsNewUserRegistration)
            {
                user.isNewUser = true;
                user.lastname = user.name.GetLastName();
                user.firstname = user.name.GetFirstName();
                user.accountid = userSettings.AccountId;

                string siteminderBusinessGuid = _httpContextAccessor.HttpContext.Request.Headers[siteMinderAuthOptions.SiteMinderBusinessGuidKey];
                string siteminderUserGuid = _httpContextAccessor.HttpContext.Request.Headers[siteMinderAuthOptions.SiteMinderUserGuidKey];

                Gov.Lclb.Cllb.Interfaces.BCeIDBusiness bceidBusiness = await _bceid.ProcessBusinessQuery(userSettings.SiteMinderGuid);
                if (bceidBusiness != null)
                {
                    user.firstname = bceidBusiness.individualFirstname;
                    user.lastname = bceidBusiness.individualSurname;
                }
                else
                {
                    Gov.Lclb.Cllb.Interfaces.BCeIDBasic bceidBasic = await _bceid.ProcessBasicQuery(userSettings.SiteMinderGuid);
                    if (bceidBasic != null)
                    {
                        user.firstname = bceidBasic.individualFirstname;
                        user.lastname = bceidBasic.individualSurname;
                    }
                }

                user.contactid = string.IsNullOrEmpty(siteminderUserGuid) ? userSettings.ContactId : siteminderUserGuid;
                if (string.IsNullOrEmpty(user.contactid))
                    user.contactid = userSettings.SiteMinderGuid;

                user.accountid = string.IsNullOrEmpty(siteminderBusinessGuid) ? userSettings.AccountId : siteminderBusinessGuid;
                user.isEligibilityRequired = true;
            }
            else
            {
                user.lastname = userSettings.AuthenticatedUser.Surname;
                user.firstname = userSettings.AuthenticatedUser.GivenName;
                user.email = userSettings.AuthenticatedUser.Email;
                user.isNewUser = false;
                if (!string.IsNullOrEmpty(user.accountid))
                {
                    user.isEligibilityRequired = await EligibilityController.IsEligibilityCheckRequiredAsync(user.accountid, _configuration, _dataverse);
                    user.isPoliceRepresentative = await _dataverse.IsAccountSepPoliceRepresentativeAsync(user.accountid);
                }
            }

            return new JsonResult(user);
        }
    }
}
