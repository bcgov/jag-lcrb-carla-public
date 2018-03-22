using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class JurisdictionController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly DataAccess db;
        public JurisdictionController(DataAccess db, IConfiguration configuration)
        {
            Configuration = configuration;
            this.db = db;
        }

        [HttpGet()]
        public JsonResult GetJurisdictions()
        {
            return Json(db.GetJurisdictions());
        }

        [HttpGet("names")]
        public JsonResult GetNames()
        {
            return Json(db.GetJurisdictionNames());
        }
    }
}
