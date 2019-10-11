using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gov.Lclb.Cllb.Public.Models
{
    public class Survey
    {
        public Survey()
        {

        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Json { get; set; }
        public string Text { get; set; }
    }
}
