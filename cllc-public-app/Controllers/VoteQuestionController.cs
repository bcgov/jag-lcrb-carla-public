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
    public class VoteQuestionController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly AppDbContext db;
        public VoteQuestionController(AppDbContext db, IConfiguration configuration)
        {
            Configuration = configuration;
            this.db = db;
        }

        [HttpGet()]
        public JsonResult GetVoteQuestions()
        {
            return Json(db.GetVoteQuestions());
        }

        [HttpGet("{slug}")]
        public JsonResult GetVoteQuestion(string slug)
        {
            return Json(db.GetViewModelVoteQuestionBySlug(slug));
        }

        [HttpPost("{slug}/vote")]
        public JsonResult AddVote(string slug, [FromQuery] string option)
        {
            db.AddVote(option);
            return Json(db.GetViewModelVoteQuestionBySlug(slug));
        }

    }
}
