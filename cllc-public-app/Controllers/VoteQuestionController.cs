using Gov.Lclb.Cllb.Public.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteQuestionController : ControllerBase
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
            return new JsonResult(db.GetVoteQuestions());
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
                return new JsonResult(result);
            }
        }

        [HttpPost("{slug}/vote")]
        [AllowAnonymous]
        public JsonResult AddVote(string slug, [FromQuery] string option)
        {
            db.AddVote(option);
            return new JsonResult(db.GetViewModelVoteQuestionBySlug(slug));
        }

    }
}
