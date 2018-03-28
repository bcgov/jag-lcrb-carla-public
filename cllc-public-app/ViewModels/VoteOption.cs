using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class VoteOption
    {
        public string id { get; set; }
        public string option { get; set; }        
        public int totalVotes { get; set; }
        public int displayOrder { get; set; }        
    }
}
