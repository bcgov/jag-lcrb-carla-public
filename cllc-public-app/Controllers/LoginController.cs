using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Hosting;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly SiteMinderAuthOptions _options = new SiteMinderAuthOptions();

        public LoginController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Login(string path)
        {
            // check to see if we have a local path.  (do not allow a redirect to another website)
            if (!string.IsNullOrEmpty(path) && (Url.IsLocalUrl(path) || (!_env.IsProduction() && path.Equals("headers"))))
            {
                // diagnostic feature for development - echo headers back.
                if ((!_env.IsProduction()) && path.Equals("headers"))
                {
                    ContentResult contentResult = new ContentResult();
                    contentResult.Content = LoggingEvents.GetHeaders(Request);
                    contentResult.ContentType = "text/html";
                    return contentResult;
                }
                return LocalRedirect(path);
            }
            else
            {
                string basePath = string.IsNullOrEmpty(_configuration["BASE_PATH"]) ? "/" : _configuration["BASE_PATH"];
                // we want to redirect to the dashboard.
                string url = "dashboard";

                return Redirect(basePath + "/" + url);
            }
        }



        /// <summary>
        /// Injects an authentication token cookie into the response for use with the 
        /// SiteMinder authentication middleware
        /// </summary>
        [HttpGet]
        [Route("token/{userid}")]
        [AllowAnonymous]
        public virtual IActionResult GetDevAuthenticationCookie(string userId)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");

            if (string.IsNullOrEmpty(userId)) return BadRequest("Missing required userid query parameter.");

            if (userId.ToLower() == "default")
                userId = _options.DevDefaultUserId;

            // clear session
            HttpContext.Session.Clear();

            // expire "dev" user cookie
            string temp = HttpContext.Request.Cookies[_options.DevBCSCAuthenticationTokenKey];
            if (temp == null)
            {
                temp = "";
            }
            Response.Cookies.Append(
                _options.DevBCSCAuthenticationTokenKey,
                temp,
                new CookieOptions
                {
                    Path = "/",
                    SameSite = SameSiteMode.None,
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
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            );

            string basePath = string.IsNullOrEmpty(_configuration["BASE_PATH"]) ? "" : _configuration["BASE_PATH"];

            string url = "dashboard";

            basePath += "/" + url;

            return Redirect(basePath);
        }

    }
}
