using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Models
{
    public class PostSurveyResult
    {
        [Key]
        public ObjectId Id { get; set; }
        public string postId { get; set; }

        public string surveyResult { get; set; }
    }
}
