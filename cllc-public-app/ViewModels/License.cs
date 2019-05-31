using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
        public enum LicenceStatusCodes
    {
        Active = 1,
        Expired = 845280000,
        Cancelled = 845280001,
        Suspended = 845280002,
        Dormant = 845280003,
        TransferRequested = 845280004,

    }

    public class License
    {
        public string id { get; set; }
        public string establishmentName { get; set; }
        public string establishmentAddress { get; set; }
        
        public string establishmentAddressStreet { get; set; }
        public string establishmentAddressCity { get; set; }
        public string establishmentAddressPostalCode { get; set; }
        public string establishmentParcelId { get; set; }

        public string licenseStatus { get; set; }
        public string licenseType { get; set; }
        public string licenseNumber { get; set; }
        public DateTimeOffset? expiryDate { get; set; }
    }
}
