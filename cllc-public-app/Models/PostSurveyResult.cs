using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gov.Lclb.Cllb.Public.Models
{
    public class PostSurveyResult
    {
        public PostSurveyResult()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string postId { get; set; }

        public string clientId { get; set; }

        public string surveyResult { get; set; }
    }
}
