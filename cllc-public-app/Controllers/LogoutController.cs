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
        /// <summary>SiteMinder's cookie family - SMSESSION, SMIDENTITY,
        /// SMCHALLENGE and whatever else the SSO configuration adds.</summary>
        private const string SiteMinderCookiePrefix = "SM";

        /// <summary>This app's own cookies: the session cookie, antiforgery
        /// and auth. Startup sets no custom session cookie name, so these are
        /// the framework defaults (.AspNetCore.Session and friends).</summary>
        private const string AppCookiePrefix = ".AspNetCore.";

        /// <summary>Upper bound on Set-Cookie headers in the sign-out
        /// response, so it can never again outgrow a proxy's response-header
        /// buffer.</summary>
        private const int MaxSetCookieHeaders = 24;

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

            // Expire the cookies that actually hold the session, so the next
            // visit starts from a clean slate instead of silently
            // re-authenticating off a stale cookie.
            //
            // ONLY the session-bearing families, not every cookie the browser
            // sent. On justice.gov.bc.ca a user carries cookies belonging to
            // other .gov.bc.ca applications, analytics and consent banners;
            // none of them sign anyone in or out, so clearing them achieved
            // nothing except response headers. Clearing all of them - two
            // Set-Cookie headers each - pushed the response past nginx's
            // default 4KB proxy_buffer_size, which answered with a 502 and
            // never delivered the clearing cookies at all. The user's browser
            // kept SMSESSION, SiteMinder re-authenticated, and logout
            // appeared to do nothing.
            //
            // Prefix matching rather than an explicit list: SiteMinder adds
            // cookies depending on configuration (SMSESSION, SMIDENTITY,
            // SMCHALLENGE, SMUSRMSG...) and an enumerated list silently
            // misses whichever one is added next.
            //
            // A clearing Set-Cookie only takes effect if its Domain and Path
            // match what the cookie was set with. SMSESSION is scoped to a
            // parent domain (.gov.bc.ca) so it is shared across the SSO
            // realm, and needs both the host-scoped and parent-scoped clear.
            // The app's own cookies are host-scoped, so a parent-domain
            // variant for those would do nothing at all - which is half of
            // what the old code was emitting.
            //
            // Request.Host is NOT reliable for deriving the parent domain -
            // F5 or the OpenShift Route can present a different Host than the
            // public URL the browser used (observed: the internal
            // *.apps.silver.devops.gov.bc.ca route name instead of
            // dev.justice.gov.bc.ca). Use the configured public BASE_URI.
            var publicHost = Uri.TryCreate(Configuration["BASE_URI"], UriKind.Absolute, out var baseUri)
                ? baseUri.Host
                : null;
            var ssoParentDomain = GetSsoParentDomain(publicHost);

            var hostScopeOnly = new string[] { null };
            var ssoScopes = new[] { null, ssoParentDomain }.Distinct().ToArray();

            var emitted = 0;
            foreach (var cookieName in Request.Cookies.Keys)
            {
                var isSso = cookieName.StartsWith(SiteMinderCookiePrefix, StringComparison.OrdinalIgnoreCase);
                var isApp = cookieName.StartsWith(AppCookiePrefix, StringComparison.OrdinalIgnoreCase);
                if (!isSso && !isApp)
                {
                    continue;
                }

                foreach (var domain in isSso ? ssoScopes : hostScopeOnly)
                {
                    // A ceiling, so this failure mode cannot return quietly.
                    // It depended on the USER's cookie count, which is why it
                    // reproduced for some people and not others and survived
                    // more than one round of fixes.
                    if (emitted >= MaxSetCookieHeaders)
                    {
                        break;
                    }

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
                    emitted++;
                }
            }

            // Every tier returns to the app's own landing page after clearing the
            // session, rather than redirecting to SiteMinder's logoff endpoint.
            // Those hosts are not reachable from a user's browser — logontest
            // .gov.bc.ca times out for dev/test and logon.gov.bc.ca refuses the
            // connection for prod — so that redirect only ever produced a browser
            // error page. Previously this was gated on the tier, which just moved
            // the failure from one environment to another.
            //
            // The teardown above is what actually signs the user out: the
            // server-side session is cleared and the session-bearing cookies are
            // expired, including SMSESSION at the .gov.bc.ca SSO scope. What is
            // given up is SiteMinder's own global sign-out, so a user holding a
            // live SMSESSION for a DIFFERENT .gov.bc.ca application may remain
            // signed in over there.
            //
            // To restore the global sign-out once a reachable logoff endpoint is
            // confirmed, redirect to SITEMINDER_LOGOUT_URL with
            //   ?returl={BASE_URI}{BASE_PATH}&retnow=1
            // Guarded like LoginController does with the same value:
            // ControllerBase.Redirect throws ArgumentException on a null or
            // empty url, so an unset BASE_PATH turns sign-out into a 500.
            var basePath = string.IsNullOrEmpty(Configuration["BASE_PATH"])
                ? "/"
                : Configuration["BASE_PATH"];
            return Redirect(basePath);
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
