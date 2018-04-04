using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class NewsletterController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly AppDbContext db;
        public NewsletterController(AppDbContext db, IConfiguration configuration)
        {
            Configuration = configuration;
            this.db = db;
        }
        [HttpGet("{slug}")]
        public JsonResult Subscribe(string slug)
        {
            Newsletter newsletter = db.GetNewsletterBySlug(slug);
            return Json(newsletter);
        }

        [HttpPost("{slug}/subscribe")]
        public JsonResult Subscribe(string slug, [FromQuery] string email)
        {
            db.AddNewsletterSubscriber(slug, email);
            return Json("Ok");
        }

        [HttpPost("{slug}/unsubscribe")]
        public JsonResult UnSubscribe(string slug, [FromQuery] string email)
        {
            db.RemoveNewsletterSubscriber(slug, email);
            return Json("Ok");
        }


    }
}
