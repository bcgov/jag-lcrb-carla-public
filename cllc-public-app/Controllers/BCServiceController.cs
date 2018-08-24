using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("bcservice")]
    public class BCServiceController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly AppDbContext db;
        private readonly IHostingEnvironment _env;
        private readonly SiteMinderAuthOptions _options = new SiteMinderAuthOptions();

        public BCServiceController(AppDbContext db, IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            this.db = db;
        }

        [HttpGet]
        [Authorize] 
        public ActionResult BCServiceLogin(string path)
        {
            // check to see if we have a local path.  (do not allow a redirect to another website)
            if (!string.IsNullOrEmpty(path) && (Url.IsLocalUrl(path) || (!_env.IsProduction() && path.Equals("headers"))))
            {
                // diagnostic feature for development - echo headers back.
                if ((!_env.IsProduction()) && path.Equals("headers"))
                {
                    StringBuilder html = new StringBuilder();
                    html.AppendLine("<html>");
                    html.AppendLine("<body>");
                    html.AppendLine("<b>Request Headers:</b>");
                    html.AppendLine("<ul style=\"list-style-type:none\">");
                    foreach (var item in Request.Headers)
                    {
                        html.AppendFormat("<li><b>{0}</b> = {1}</li>\r\n", item.Key, ExpandValue(item.Value));
                    }
                    html.AppendLine("</ul>");
                    html.AppendLine("</body>");
                    html.AppendLine("</html>");
                    ContentResult contentResult = new ContentResult();
                    contentResult.Content = html.ToString();
                    contentResult.ContentType = "text/html";
                    return contentResult;
                }
                return LocalRedirect(path);
            }
            else
            {
                string basePath = string.IsNullOrEmpty(Configuration["BASE_PATH"]) ? "" : Configuration["BASE_PATH"];
                // basePath += !String.IsNullOrEmpty(Configuration["IS_LITE_VERSION"]) ? "dashboard" : "dashboard-lite";
                basePath += "/associates-lite";
                return Redirect(basePath);
            }
        }
    

        /// <summary>
        /// Utility function used to expand headers.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static string ExpandValue(IEnumerable<string> values)
        {
            StringBuilder value = new StringBuilder();

            foreach (string item in values)
            {
                if (value.Length > 0)
                {
                    value.Append(", ");
                }
                value.Append(item);
            }
            return value.ToString();
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
                    SameSite = SameSiteMode.None,
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
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            );

            string basePath = string.IsNullOrEmpty(Configuration["BASE_PATH"]) ? "" : Configuration["BASE_PATH"];
            // basePath += !String.IsNullOrEmpty(Configuration["IS_LITE_VERSION"]) ? "dashboard" : "dashboard-lite";
            basePath += "associates-lite";
            return Redirect(basePath);
        }

        [HttpGet]
        [Route("devlogin")]
        [AllowAnonymous]
        public IActionResult DevLogin()
        {
            return Redirect($"https://logontest.gov.bc.ca/clp-cgi/fed/fedLaunch.cgi?TARGET=$SM${Configuration["BASE_URI"]}{Configuration["BASE_PATH"]}bcservice&retnow=1");
        }

	}
}
