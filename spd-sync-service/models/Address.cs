using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpdSync.models
{
    public class Address
    {
        public string AddressStreet1 { get; set; }
        public string AddressStreet2 { get; set; }
        public string AddressStreet3 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Postal { get; set; }
        public string Country { get; set; }
    }
}
