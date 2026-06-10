extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
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
        private readonly IDataverseClient _dataverse;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SiteMinderAuthOptions _options = new SiteMinderAuthOptions();

        public LoginController(IConfiguration configuration, IWebHostEnvironment env, IDataverseClient dataverse,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dataverse = dataverse;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Login(string path, [FromQuery] string source)
        {
            if (!string.IsNullOrEmpty(path) && (Url.IsLocalUrl(path) || !_env.IsProduction() && path.Equals("headers")))
            {
                if (!_env.IsProduction() && path.Equals("headers"))
                {
                    var contentResult = new ContentResult();
                    contentResult.Content = LoggingEvents.GetHeaders(Request);
                    contentResult.ContentType = "text/html";
                    return contentResult;
                }
                return LocalRedirect(path);
            }

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            bool isPoliceRep = false;
            try
            {
                if (!string.IsNullOrEmpty(userSettings?.AccountId) && Guid.Parse(userSettings?.AccountId) != Guid.Empty)
                {
                    isPoliceRep = _dataverse.IsAccountSepPoliceRepresentativeAsync(userSettings?.AccountId)
                        .GetAwaiter().GetResult();
                }
            }
            catch (Exception)
            {
                isPoliceRep = false;
            }

            var basePath = string.IsNullOrEmpty(_configuration["BASE_PATH"]) ? "/" : _configuration["BASE_PATH"];
            var url = "/dashboard";
            if (isPoliceRep) url = "/sep/dashboard";
            if (!string.IsNullOrEmpty(source)) url = source;

            return Redirect(basePath + url);
        }

        [HttpGet]
        [Route("token/{userid}")]
        [AllowAnonymous]
        public virtual IActionResult GetDevAuthenticationCookie(string userId, [FromQuery] string source)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");
            if (string.IsNullOrEmpty(userId)) return BadRequest("Missing required userid query parameter.");

            if (userId.ToLower() == "default") userId = _options.DevDefaultUserId;

            HttpContext.Session.Clear();

            var temp = HttpContext.Request.Cookies[_options.DevBCSCAuthenticationTokenKey] ?? "";
            Response.Cookies.Append(_options.DevBCSCAuthenticationTokenKey, temp,
                new CookieOptions { Path = "/", SameSite = SameSiteMode.Lax, Expires = DateTime.UtcNow.AddDays(-1) });
            Response.Cookies.Append(_options.DevAuthenticationTokenKey, userId,
                new CookieOptions { Path = "/", SameSite = SameSiteMode.Lax, Expires = DateTime.UtcNow.AddDays(7) });

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            bool isSep = source != null && source == "sep" ||
                         userSettings?.ContactId != null &&
                         _dataverse.IsAccountSepPoliceRepresentativeAsync(userSettings?.AccountId)
                             .GetAwaiter().GetResult();

            var basePath = string.IsNullOrEmpty(_configuration["BASE_PATH"]) ? "/" : _configuration["BASE_PATH"];
            var url = "dashboard";
            if (isSep) url = "sep/dashboard";

            basePath += "/" + url;
            return Redirect(basePath);
        }
    }
}
