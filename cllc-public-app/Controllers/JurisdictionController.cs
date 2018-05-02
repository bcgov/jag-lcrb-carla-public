using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class JurisdictionController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly AppDbContext db;
        public JurisdictionController(AppDbContext db, IConfiguration configuration)
        {
            Configuration = configuration;
            this.db = db;
        }

        [HttpGet()]
        [AllowAnonymous]
        public JsonResult GetJurisdictions()
        {
            return Json(db.GetJurisdictions());
        }
        
    }
}
