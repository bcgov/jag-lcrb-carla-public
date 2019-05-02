using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpdSync.models
{
    public class Establishment
    {
        public string Name { get; set; }

        public Address Address { get; set; }
        public string ParcelId { get; set; }

        public string PrimaryPhone { get; set; }
        public string PrimaryEmail { get; set; }
    }
}
