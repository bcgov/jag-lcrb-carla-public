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
            //
            // Deliberately only 2 domain variants (not every parent-domain level
            // for every cookie): SiteMinder's SMSESSION cookie is large (~2-3KB)
            // and a full combinatorial clear (many cookies x many domains)
            // generates enough Set-Cookie response headers to exceed a proxy's
            // response-header size limit between here and the browser (F5 /
            // OpenShift Route), producing a 502 instead of the redirect.
            //
            // Request.Host is NOT reliable for this — F5/the OpenShift Route can
            // present a different Host than the public URL the browser actually
            // used (observed: the internal *.apps.silver.devops.gov.bc.ca route
            // name instead of dev.justice.gov.bc.ca). Derive the domain from the
            // configured public BASE_URI instead.
            var publicHost = Uri.TryCreate(Configuration["BASE_URI"], UriKind.Absolute, out var baseUri)
                ? baseUri.Host
                : null;
            var domainScopes = new[] { null, GetSsoParentDomain(publicHost) }.Distinct();

            foreach (var cookieName in Request.Cookies.Keys)
            {
                foreach (var domain in domainScopes)
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

            // Only the real Production tier does a SiteMinder global sign-out.
            // ASPNETCORE_ENVIRONMENT can't be used to tell tiers apart: it is
            // pinned to "Production" in EVERY deployed environment (Development
            // mode trips DI ValidateOnBuild and crashes one-stop), so keying off
            // it sent dev/test to the SiteMinder logout for the first time — and
            // logontest.gov.bc.ca doesn't answer for those hostnames, leaving the
            // user on a browser error page. DEPLOYMENT_TIER carries the actual
            // tier. Falls back to the hosting environment when it isn't set.
            var deploymentTier = Configuration["DEPLOYMENT_TIER"];
            var isProductionTier = string.IsNullOrEmpty(deploymentTier)
                ? _env.IsProduction()
                : string.Equals(deploymentTier, "Production", StringComparison.OrdinalIgnoreCase);

            if (!isProductionTier)
            {
                return Redirect($"{Configuration["BASE_PATH"]}");
            }

            string logoutPath = string.IsNullOrEmpty(Configuration["SITEMINDER_LOGOUT_URL"]) ? "/" : Configuration["SITEMINDER_LOGOUT_URL"];
            return Redirect(logoutPath + $"?returl={Configuration["BASE_URI"]}{Configuration["BASE_PATH"]}&retnow=1");
        }

        /// <summary>
        /// The broad parent domain SiteMinder/BCeID SSO cookies are typically
        /// scoped to for a BC government host, e.g. dev.justice.gov.bc.ca
        /// yields ".gov.bc.ca" (last 3 labels). Returns null if the host is too
        /// short to have a meaningful parent (e.g. localhost, an IP, or already
        /// only 3 labels).
        /// </summary>
        private static string GetSsoParentDomain(string host)
        {
            if (string.IsNullOrEmpty(host) || Uri.CheckHostName(host) != UriHostNameType.Dns)
            {
                return null;
            }

            var labels = host.Split('.');
            if (labels.Length <= 3)
            {
                return null; // host is already at or below the target scope
            }

            return "." + string.Join(".", labels.Skip(labels.Length - 3));
        }
    }
}
