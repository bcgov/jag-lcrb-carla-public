using Gov.Lclb.Cllb.Public.Models;
using System.Collections.Generic;
using System.Linq;


namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class AppDbContextSurveyExtenstions
    {
        public static Dictionary<string, string> GetSurveys(this AppDbContext context)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            return result;
        }

        public static Dictionary<string, List<string>> GetResults(this AppDbContext context)
        {
            // create the result object
            Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();


            return results;
        }


        public static string GetSurvey(this AppDbContext context, string surveyId)
        {
            string survey = "";
            return survey;
        }

        public static void StoreSurvey(this AppDbContext context, string surveyId, string jsonString)
        {
            /*
            ChangeSurvey survey = _db.GetCollection<Models.ChangeSurvey>(SURVEY_COLLECTION).Find(x => x.Name == surveyId).FirstOrDefault();
            if (survey == null)
            {
                survey = new ChangeSurvey();
                survey.Name = surveyId;
            }
            survey.Json = jsonString;
            */
        }

        public static void ChangeName(this AppDbContext context, string id, string name)
        {
            //var survey = _db.GetCollection<Models.ChangeSurvey>(SURVEY_COLLECTION).Find(x => x.Name == name);
        }

        public static void DeleteSurvey(this AppDbContext context, string surveyId)
        {
            //var survey = _db.GetCollection<Models.ChangeSurvey>(SURVEY_COLLECTION).DeleteOne(x => x.Name == surveyId);
        }

        public static void PostResults(this AppDbContext context, string postId, string clientId, string resultJson)
        {
            // create a new result.
            PostSurveyResult psr = new PostSurveyResult();
            psr.postId = postId;
            psr.clientId = clientId;
            psr.surveyResult = resultJson;
            context.PostSurveyResults.Add(psr);
            context.SaveChanges();
        }

        /// <summary>
        /// Get survey results for a given survey
        /// </summary>
        /// <param name="postId">The name of a survey</param>
        /// <returns></returns>
        public static List<string> GetResults(this AppDbContext context, string postId)
        {
            List<string> result = new List<string>();
            List<Models.PostSurveyResult> items = context.PostSurveyResults.Where(x => x.postId == postId).ToList();
            foreach (var item in items)
            {
                result.Add(item.surveyResult);
            }
            return result;
        }


        /// <summary>
        /// Get survey results for a given survey
        /// </summary>
        /// <param name="postId">The name of a survey</param>
        /// <returns></returns>
        public static string GetSurveyResultByClientId(this AppDbContext context, string clientId)
        {
            string result = "";
            Models.PostSurveyResult item = context.PostSurveyResults.FirstOrDefault(x => x.clientId == clientId);
            // add error handling
            result = item.surveyResult;
            return result;
        }
    }
}
