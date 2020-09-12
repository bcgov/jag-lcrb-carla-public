using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using Microsoft.Extensions.Hosting;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Rest;
using Gov.Lclb.Cllb.Interfaces.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Gov.Lclb.Cllb.Public.ViewModels;
using System.Web;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("bcservice")]
    [ApiController]
    public class BCServiceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly SiteMinderAuthOptions _options = new SiteMinderAuthOptions();
        private readonly string _encryptionKey;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;

        public BCServiceController(IConfiguration configuration, IWebHostEnvironment env, IDynamicsClient dynamicsClient, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _env = env;
            _encryptionKey = _configuration["ENCRYPTION_KEY"];
            _logger = loggerFactory.CreateLogger(typeof(ContactController));
            _dynamicsClient = dynamicsClient;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BCServiceLogin(string path, string code)
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
                if (string.IsNullOrEmpty(_configuration["ENABLE_SERVICECARD_TOKEN_TEST"]))
                {
                    string basePath = GetRedirectPath(_configuration, path, code);
                    return Redirect(basePath);
                }
                else
                {
                    string decrypted = EncryptionUtility.DecryptString(code, _encryptionKey);
                    if (decrypted == null)
                    {
                        decrypted = "";
                    }

                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.OK,
                        Content = $"<html><body><h1>Token Parse Test</h1><p>Decrypted text is: {decrypted}</body></html>"
                    };
                }
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
            string temp = HttpContext.Request.Cookies[_options.DevAuthenticationTokenKey];
            if (temp == null)
            {
                temp = "";
            }
            Response.Cookies.Append(
                _options.DevAuthenticationTokenKey,
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
                _options.DevBCSCAuthenticationTokenKey,
                userId,
                new CookieOptions
                {
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            );

            string path = HttpUtility.ParseQueryString(Request.QueryString.ToString()).Get("path");
            string code = HttpUtility.ParseQueryString(Request.QueryString.ToString()).Get("code");
            string basePath = GetDevRedirectPath(_configuration, path, code);
            return Redirect(basePath);
        }

        private string GetRedirectPath(IConfiguration configuration, string path, string code)
        {
            string basePath = string.IsNullOrEmpty(configuration["BASE_PATH"]) ? "" : configuration["BASE_PATH"];
            if (path != null && path.Equals("cannabis-associate-screening"))
            {
                basePath += "/cannabis-associate-screening/" + code;
            }
            else
            {
                basePath += "/worker-qualification/dashboard";
            }

            return basePath;
        }

        private string GetDevRedirectPath(IConfiguration configuration, string path, string code)
        {
            string basePath = string.IsNullOrEmpty(configuration["BASE_PATH"]) ? "" : configuration["BASE_PATH"];
            if (path != null && path.Equals("cannabis-associate-screening"))
            {
                basePath += $"/bcservice?path={path}&code={code}";
            }
            else
            {
                basePath += "/worker-qualification/dashboard";
            }

            return basePath;
        }
    }
}
