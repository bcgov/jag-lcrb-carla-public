using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("login")]
    public class LoginController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly DataAccess db;
        public LoginController(DataAccess db, IConfiguration configuration)
        {
            Configuration = configuration;
            this.db = db;
        }
        [Authorize]
        [HttpGet]
        public ActionResult Login(string path)
        {
            // check to see if we have a local path.  (do not allow a redirect to another website)
            if (!string.IsNullOrEmpty(path) && Url.IsLocalUrl(path))
            {
                return LocalRedirect(path);
            }
            else
            {
                return Redirect("/");
            }            
        }
    }    
}
