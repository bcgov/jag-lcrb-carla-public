﻿using Gov.Lclb.Cllb.Public.Authentication;
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
        private readonly SiteMinderAuthOptions _options = new SiteMinderAuthOptions();

        public LogoutController(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout(string path)
        {
            // clear session
            HttpContext.Session.Clear();
            string tempSession = HttpContext.Request.Cookies[".AspNetCore.Session"];
            if (tempSession == null)
            {
                tempSession = "";
            }
            if (! string.IsNullOrEmpty(tempSession))
            {
                // expire session user cookie
                Response.Cookies.Append(
                    ".AspNetCore.Session",
                    tempSession,
                    new CookieOptions
                    {
                        Path = "/",
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(-1)
                    }
                );
            }


            if (!_env.IsProduction()) // clear dev tokens
            {
                string temp = HttpContext.Request.Cookies[_options.DevAuthenticationTokenKey];
                if (temp == null)
                {
                    temp = "";
                }
                // expire "dev" user cookie
                Response.Cookies.Append(
                    _options.DevAuthenticationTokenKey,
                    temp,
                    new CookieOptions
                    {
                        Path = "/",
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(-1)
                    }
                );
                // expire "dev" user cookie
                temp = HttpContext.Request.Cookies[_options.DevBCSCAuthenticationTokenKey];
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
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(-1)
                    }
                );
                return Redirect($"{Configuration["BASE_PATH"]}");
            }

            string logoutPath = string.IsNullOrEmpty(Configuration["SITEMINDER_LOGOUT_URL"]) ? "/" : Configuration["SITEMINDER_LOGOUT_URL"];
            return Redirect(logoutPath + $"?returl={Configuration["BASE_URI"]}{Configuration["BASE_PATH"]}&retnow=1");


        }
    }
}
