using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //
            // A clearing Set-Cookie only takes effect if its Domain (and Path)
            // match what the cookie was originally set with. SiteMinder's
            // SMSESSION is commonly scoped to a parent domain (e.g. .gov.bc.ca)
            // so it's shared across the SSO realm — clearing with no Domain only
            // targets the exact host and silently leaves that cookie in place.
            // Clear across every plausible domain scope for the current host.
            foreach (var cookieName in Request.Cookies.Keys)
            {
                foreach (var domain in GetDomainScopes(Request.Host.Host))
                {
                    var options = new CookieOptions
                    {
                        Path = "/",
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(-1)
                    };
                    if (!string.IsNullOrEmpty(domain))
                    {
                        options.Domain = domain;
                    }
                    Response.Cookies.Append(cookieName, "", options);
                }
            }

            if (!_env.IsProduction())
            {
                return Redirect($"{Configuration["BASE_PATH"]}");
            }

            string logoutPath = string.IsNullOrEmpty(Configuration["SITEMINDER_LOGOUT_URL"]) ? "/" : Configuration["SITEMINDER_LOGOUT_URL"];
            return Redirect(logoutPath + $"?returl={Configuration["BASE_URI"]}{Configuration["BASE_PATH"]}&retnow=1");
        }

        /// <summary>
        /// Every domain scope a cookie could plausibly have been set with for the
        /// given host: no Domain (exact host), the exact host as an explicit
        /// Domain, and each parent domain (e.g. dev.justice.gov.bc.ca yields
        /// .dev.justice.gov.bc.ca, .justice.gov.bc.ca, .gov.bc.ca, .bc.ca).
        /// Clearing on all of them guarantees a match regardless of which scope
        /// SiteMinder (or anything else) actually used.
        /// </summary>
        private static IEnumerable<string> GetDomainScopes(string host)
        {
            yield return null; // no Domain attribute — defaults to the exact host

            if (string.IsNullOrEmpty(host) || Uri.CheckHostName(host) != UriHostNameType.Dns)
            {
                yield break; // IP address / unknown host — nothing further to try
            }

            var labels = host.Split('.');
            // Stop at the last two labels (registrable domain, e.g. "bc.ca") —
            // going further would clear cookies for an unrelated public suffix.
            for (var i = 0; i < labels.Length - 2; i++)
            {
                yield return "." + string.Join(".", labels.Skip(i));
            }
            yield return "." + string.Join(".", labels.Skip(labels.Length - 2));
        }
    }
}
