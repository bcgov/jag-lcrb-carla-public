using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {           
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;            
        }
        
        protected ClaimsPrincipal CurrentUser => _httpContextAccessor.HttpContext.User;

        [HttpGet("current")]        
        //[RequiresPermission(Permission.Login, Permission.NewUserRegistration)]


        public virtual IActionResult UsersCurrentGet()
        {
            SiteMinderAuthOptions siteMinderAuthOptions = new SiteMinderAuthOptions();
            ViewModels.User user = new ViewModels.User();

            // determine if we are a new registrant.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            user.id = userSettings.UserId;
            user.contactid = userSettings.ContactId;
            user.accountid = userSettings.AccountId;
            user.businessname = userSettings.BusinessLegalName;
            user.name = userSettings.UserDisplayName;
            user.UserType = userSettings.UserType;

            if (userSettings.IsNewUserRegistration)
            {
                user.isNewUser = true;
                // get details from the headers.
            
                
                user.lastname = DynamicsExtensions.GetLastName(user.name);
                user.firstname = DynamicsExtensions.GetFirstName(user.name);
                user.accountid = userSettings.AccountId;

                string siteminderBusinessGuid = _httpContextAccessor.HttpContext.Request.Headers[siteMinderAuthOptions.SiteMinderBusinessGuidKey];
                string siteminderUserGuid = _httpContextAccessor.HttpContext.Request.Headers[siteMinderAuthOptions.SiteMinderUserGuidKey];

                user.contactid = string.IsNullOrEmpty (siteminderUserGuid) ? userSettings.ContactId : siteminderUserGuid;                
                user.accountid = string.IsNullOrEmpty(siteminderBusinessGuid) ? userSettings.AccountId : siteminderBusinessGuid;
                
            }
            else
            {
                user.lastname = userSettings.AuthenticatedUser.Surname;
                user.firstname = userSettings.AuthenticatedUser.GivenName;
                user.email = userSettings.AuthenticatedUser.Email;                
                user.isNewUser = false;  
            
            }

            return new JsonResult(user);
        }

    }
    
}
