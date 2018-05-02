using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class SurveyController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly AppDbContext db;
        public SurveyController(AppDbContext db, IConfiguration configuration)
        {
            Configuration = configuration;
            this.db = db;
        }
        [HttpGet("getActive")]
        [AllowAnonymous]
        public JsonResult GetActive()
        {
            
            return Json(db.GetSurveys());
        }

        [HttpGet("getSurvey")]
        [AllowAnonymous]
        public string GetSurvey(string surveyId)
        {
            return db.GetSurvey(surveyId);
        }

        [HttpGet("create")]
        public JsonResult Create(string name)
        {
            db.StoreSurvey(name, "{}");
            return Json("Ok");
        }

        [HttpGet("changeName")]
        public JsonResult ChangeName(string id, string name)
        {
            db.ChangeName(id, name);
            return Json("Ok");
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
            return Json("Ok");
        }

        [HttpPost("post")]
        public JsonResult PostResult([FromBody]ViewModels.PostSurveyResult model)
        {
            db.PostResults(model.postId, model.clientId, model.surveyResult);
            return Json("Ok");
        }

        [HttpGet("results")]
        [AllowAnonymous]
        public JsonResult GetResults(string postId)
        {
            return Json(db.GetResults(postId));
        }

        [HttpGet("getResultByClient/{clientId}")]
        [AllowAnonymous]
        public JsonResult GetResultByClient(string clientId)
        {
            string result = db.GetSurveyResultByClientId(clientId);
            return Json(result);
        }
    }
}
