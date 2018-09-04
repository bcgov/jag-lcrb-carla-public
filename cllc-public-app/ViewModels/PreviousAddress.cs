using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class PreviousAddress
    {
        public string id { get; set; }
        public string name {get; set;}
        public string streetaddress {get; set;}
        public string city {get; set;}
        public string provstate {get; set;}
        public string country {get; set;}
        public string postalcode {get; set;}
        public DateTime fromdate {get; set;}
        public DateTime todate {get; set;}
        public string contactId { get; set; }
        public string workerId { get; set; }
    }
}
