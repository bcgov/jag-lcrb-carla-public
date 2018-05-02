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
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("login")]
    public class LoginController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly AppDbContext db;
        private readonly IHostingEnvironment _env;
        private readonly SiteMinderAuthOptions _options = new SiteMinderAuthOptions();

        public LoginController(AppDbContext db, IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            this.db = db;
        }
        
        [HttpGet]
        [Authorize]

        public ActionResult Login(string path)
        {
            // check to see if we have a local path.  (do not allow a redirect to another website)
            if (!string.IsNullOrEmpty(path) && (Url.IsLocalUrl(path) || ((_env.IsDevelopment() || _env.IsStaging()) && path.Equals("headers"))))
            {
                // diagnostic feature for development - echo headers back.
                if ((_env.IsDevelopment() || _env.IsStaging()) && path.Equals("headers"))
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
                    return new OkObjectResult(html.ToString());
                }
                return LocalRedirect(path);
            }
            else
            {
                return Redirect("/");
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
            if (! (_env.IsDevelopment() || _env.IsStaging() )) return BadRequest("This API is not available outside a development environment.");

            if (string.IsNullOrEmpty(userId)) return BadRequest("Missing required userid query parameter.");

            if (userId.ToLower() == "default")
                userId = _options.DevDefaultUserId;

            string temp = HttpContext.Request.Cookies[_options.DevAuthenticationTokenKey];

            // clear session
            HttpContext.Session.Clear();

            // crearte new "dev" user cookie
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

            return Ok();
        }

        /// <summary>
        /// Clear out any existing dev authentication tokens
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("cleartoken")]
        [AllowAnonymous]
        public virtual IActionResult ClearDevAuthenticationCookie()
        {
            if (! (_env.IsDevelopment() || _env.IsStaging() )) return BadRequest("This API is not available outside a development environment.");

            string temp = HttpContext.Request.Cookies[_options.DevAuthenticationTokenKey];

            // clear session
            HttpContext.Session.Clear();

            // expire "dev" user cookie
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

            return Ok();
        }
    }
    
}
