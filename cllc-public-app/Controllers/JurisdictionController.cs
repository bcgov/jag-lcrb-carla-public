using Gov.Lclb.Cllb.Public.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JurisdictionController : ControllerBase
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
            return new JsonResult(_db.GetJurisdictions());
        }
        
    }
}
