using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
