using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Models
{
    public class ChangeSurvey
    {
        [Key]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Json { get; set; }
        public string Text { get; set; }
    }
}
