using Microsoft.AspNetCore.Mvc;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("404")]
    [ApiController]
    public class NotFoundController : ControllerBase
    {

        public NotFoundController()
        {

        }

        [HttpGet]

        public ActionResult NotFound(string path)
        {
            return NotFound();
        }

    }
    
}
