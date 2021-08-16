using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SiteMinderAuthOptions _options = new SiteMinderAuthOptions();

        public LoginController(IConfiguration configuration, IWebHostEnvironment env, IDynamicsClient dynamicsClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Login(string path, [FromQuery] string source)
        {
            // check to see if we have a local path.  (do not allow a redirect to another website)
            if (!string.IsNullOrEmpty(path) && (Url.IsLocalUrl(path) || !_env.IsProduction() && path.Equals("headers")))
            {
                // diagnostic feature for development - echo headers back.
                if (!_env.IsProduction() && path.Equals("headers"))
                {
                    var contentResult = new ContentResult();
                    contentResult.Content = LoggingEvents.GetHeaders(Request);
                    contentResult.ContentType = "text/html";
                    return contentResult;
                }

                return LocalRedirect(path);
            }


            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            bool isPoliceRep = userSettings?.ContactId != null &&
                ContactController.IsSepPoliceRepresentative(userSettings?.ContactId, _configuration, _dynamicsClient);

            var basePath = string.IsNullOrEmpty(_configuration["BASE_PATH"]) ? "/" : _configuration["BASE_PATH"];
            // we want to redirect to the dashboard.
            var url = "/dashboard";
            
            if (isPoliceRep) {
                url = "/sep/dashboard";
            }

            // if source is specified, redirect to source
            if(!string.IsNullOrEmpty(source)){
                url = source;
            }

            return Redirect(basePath + url);
        }


        /// <summary>
        ///     Injects an authentication token cookie into the response for use with the
        ///     SiteMinder authentication middleware
        /// </summary>
        [HttpGet]
        [Route("token/{userid}")]
        [AllowAnonymous]
        public virtual IActionResult GetDevAuthenticationCookie(string userId, [FromQuery] string source)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");

            if (string.IsNullOrEmpty(userId)) return BadRequest("Missing required userid query parameter.");

            if (userId.ToLower() == "default")
                userId = _options.DevDefaultUserId;

            // clear session
            HttpContext.Session.Clear();

            // expire "dev" user cookie
            var temp = HttpContext.Request.Cookies[_options.DevBCSCAuthenticationTokenKey];
            if (temp == null) temp = "";
            Response.Cookies.Append(
                _options.DevBCSCAuthenticationTokenKey,
                temp,
                new CookieOptions
                {
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(-1)
                }
            );
            // create new "dev" user cookie
            Response.Cookies.Append(
                _options.DevAuthenticationTokenKey,
                userId,
                new CookieOptions
                {
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            );


            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            bool isSep = source != null && source == "sep" ||
                        userSettings?.ContactId != null &&
                        ContactController.IsSepPoliceRepresentative(userSettings?.ContactId, _configuration, _dynamicsClient);

            var basePath = string.IsNullOrEmpty(_configuration["BASE_PATH"]) ? "/" : _configuration["BASE_PATH"];
            // we want to redirect to the dashboard.
            var url = "dashboard";
            if (isSep) url = "sep/dashboard";

            basePath += "/" + url;

            return Redirect(basePath);
        }
    }
}