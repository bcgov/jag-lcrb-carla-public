using Microsoft.AspNetCore.Mvc;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("404")]
    public class NotFoundController : Controller
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
