using Gov.Lclb.Cllb.Public.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // public controller.
    public class SurveyController : ControllerBase
    {
        private readonly AppDbContext db;
        public SurveyController(AppDbContext db)
        {
            this.db = db;
        }
        [HttpGet("getActive")]
        [AllowAnonymous]
        public JsonResult GetActive()
        {

            return new JsonResult(db.GetSurveys());
        }

        [HttpGet("getSurvey")]
        [AllowAnonymous]
        public ActionResult GetSurvey(string surveyId)
        {
            string result = db.GetSurvey(surveyId);
            if (result != null)
            {
                return base.Content(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("create")]
        public JsonResult Create(string name)
        {
            db.StoreSurvey(name, "{}");
            return new JsonResult("Ok");
        }

        [HttpGet("changeName")]
        public JsonResult ChangeName(string id, string name)
        {
            db.ChangeName(id, name);
            return new JsonResult("Ok");
        }

        [HttpPost("changeJson")]
        public string ChangeJson([FromBody]ViewModels.Survey model)
        {
            db.StoreSurvey(model.Id, model.Json);
            return db.GetSurvey(model.Id);
        }

        [HttpGet("delete")]
        public JsonResult Delete(string id)
        {
            db.DeleteSurvey(id);
            return new JsonResult("Ok");
        }

        [HttpPost("post")]
        [AllowAnonymous]
        public JsonResult PostResult([FromBody]ViewModels.PostSurveyResult model)
        {
            db.PostResults(model.postId, model.clientId, model.surveyResult);
            return new JsonResult("Ok");
        }

        [HttpGet("results")]
        [AllowAnonymous]
        public JsonResult GetResults(string postId)
        {
            return new JsonResult(db.GetResults(postId));
        }

        [HttpGet("getResultByClient/{clientId}")]
        [AllowAnonymous]
        public JsonResult GetResultByClient(string clientId)
        {
            string result = db.GetSurveyResultByClientId(clientId);
            return new JsonResult(result);
        }
    }
}
