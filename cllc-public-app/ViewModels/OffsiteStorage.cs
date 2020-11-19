using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum OffsiteStorageStatus
    {
        [EnumMember(Value = "Added")]
        Added = 1,
        [EnumMember(Value = "Removed")]
        Removed = 845280000,
    }

    public class OffsiteStorage
    {
        // string form of the guid.
        public string Id { get; set; }
        public string Name { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public OffsiteStorageStatus? Status { get; set; }
    }
}
