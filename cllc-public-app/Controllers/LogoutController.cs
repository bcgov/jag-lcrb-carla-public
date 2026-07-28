using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Hosting;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment _env;

        public LogoutController(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout(string path)
        {
            // clear session server-side
            HttpContext.Session.Clear();

            // Expire every cookie the browser sent us (session cookie, SiteMinder
            // cookies, dev tokens, etc.) so the next visit starts from a clean
            // slate instead of silently re-authenticating off a stale cookie.
            // This only runs on an explicit sign-out request.
            foreach (var cookieName in Request.Cookies.Keys)
            {
                Response.Cookies.Append(
                    cookieName,
                    "",
                    new CookieOptions
                    {
                        Path = "/",
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(-1)
                    }
                );
            }

            if (!_env.IsProduction())
            {
                return Redirect($"{Configuration["BASE_PATH"]}");
            }

            string logoutPath = string.IsNullOrEmpty(Configuration["SITEMINDER_LOGOUT_URL"]) ? "/" : Configuration["SITEMINDER_LOGOUT_URL"];
            return Redirect(logoutPath + $"?returl={Configuration["BASE_URI"]}{Configuration["BASE_PATH"]}&retnow=1");
        }
    }
}
