using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class PreviousAddress
    {
        public string id { get; set; }
        public string adoxio_name {get; set;}
        public string adoxio_streetaddress {get; set;}
        public string adoxio_city {get; set;}
        public string adoxio_provstate {get; set;}
        public string adoxio_country {get; set;}
        public string adoxio_postalcode {get; set;}
        public DateTime adoxio_fromdate {get; set;}
        public DateTime adoxio_todate {get; set;}
    }
}
