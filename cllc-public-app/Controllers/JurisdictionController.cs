using Gov.Lclb.Cllb.Public.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class JurisdictionController : Controller
    {        
        private readonly AppDbContext _db;
        public JurisdictionController(AppDbContext db, IConfiguration configuration)
        {            
            _db = db;
        }

        [HttpGet()]
        [AllowAnonymous]
        public JsonResult GetJurisdictions()
        {
            return Json(_db.GetJurisdictions());
        }
        
    }
}
