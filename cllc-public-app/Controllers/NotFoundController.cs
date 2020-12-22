using Microsoft.AspNetCore.Mvc;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("404")]
    [ApiController]
    public class NotFoundController : ControllerBase
    {
        [HttpGet]

        public ActionResult NotFound(string path)
        {
            return NotFound();
        }

    }

}
