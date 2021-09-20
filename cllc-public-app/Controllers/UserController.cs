using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Gov.Lclb.Cllb.Public.Models;
using System;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;


        public UserController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IDynamicsClient dynamics)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dynamicsClient = dynamics;
        }

        protected ClaimsPrincipal CurrentUser => _httpContextAccessor.HttpContext.User;

        [HttpGet("current")]
        //[RequiresPermission(Permission.Login, Permission.NewUserRegistration)]


        public virtual IActionResult UsersCurrentGet()
        {
            SiteMinderAuthOptions siteMinderAuthOptions = new SiteMinderAuthOptions();
            ViewModels.User user = new ViewModels.User();

            // determine if we are a new registrant.

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            user.id = userSettings.UserId;
            user.contactid = userSettings.ContactId;
            user.accountid = userSettings.AccountId;
            user.businessname = userSettings.BusinessLegalName;
            user.name = userSettings.UserDisplayName;
            user.UserType = userSettings.UserType;

            // if Authenticated User is null, try and fetch it.

            if (userSettings.AuthenticatedUser == null)
            {
                try
                {
                    userSettings.AuthenticatedUser = _dynamicsClient.GetActiveUserBySmGuid(userSettings.SiteMinderGuid);
                    if (userSettings.AuthenticatedUser == null)
                    {
                        userSettings.IsNewUserRegistration = true;
                    }
                }
                catch (Exception)
                {
                    userSettings.IsNewUserRegistration = true;
                }
            }

            if (userSettings.IsNewUserRegistration)
            {
                user.isNewUser = true;
                // get details from the headers.

                user.lastname = user.name.GetLastName();
                user.firstname = user.name.GetFirstName();
                user.accountid = userSettings.AccountId;

                string siteminderBusinessGuid = _httpContextAccessor.HttpContext.Request.Headers[siteMinderAuthOptions.SiteMinderBusinessGuidKey];
                string siteminderUserGuid = _httpContextAccessor.HttpContext.Request.Headers[siteMinderAuthOptions.SiteMinderUserGuidKey];

                user.contactid = string.IsNullOrEmpty(siteminderUserGuid) ? userSettings.ContactId : siteminderUserGuid;
                // handle Basic BCeID
                if (string.IsNullOrEmpty(user.contactid))
                {
                    user.contactid = userSettings.SiteMinderGuid;
                }

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
                    user.isEligibilityRequired = EligibilityController.IsEligibilityCheckRequired(user.accountid, _configuration, _dynamicsClient);
                    user.isPoliceRepresentative = _dynamicsClient.IsAccountSepPoliceRepresentative(user.accountid, _configuration);
                }
            }
            

            return new JsonResult(user);
        }

    }

}
