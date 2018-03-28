using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class VoteQuestion
    {
        public string id { get; set; }
        public string title { get; set; }
        public string question { get; set; }
        public string slug { get; set; }
        public ViewModels.VoteOption [] options { get; set; }
    }
}
