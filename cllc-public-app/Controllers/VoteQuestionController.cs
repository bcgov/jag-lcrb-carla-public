using Gov.Lclb.Cllb.Public.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class VoteQuestionController : Controller
    {        
        private readonly AppDbContext db;
        public VoteQuestionController(AppDbContext db)
        {
            this.db = db;
        }

        [HttpGet()]
        [AllowAnonymous]
        public JsonResult GetVoteQuestions()
        {
            return Json(db.GetVoteQuestions());
        }

        [HttpGet("{slug}")]
        [AllowAnonymous]
        public ActionResult GetVoteQuestion(string slug)
        {
            var result = db.GetViewModelVoteQuestionBySlug(slug);

            if (result == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return Json(result);
            }
        }

        [HttpPost("{slug}/vote")]
        [AllowAnonymous]
        public JsonResult AddVote(string slug, [FromQuery] string option)
        {
            db.AddVote(option);
            return Json(db.GetViewModelVoteQuestionBySlug(slug));
        }

    }
}
