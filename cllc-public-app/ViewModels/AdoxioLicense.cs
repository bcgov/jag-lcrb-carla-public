using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class AdoxioLicense
    {
        public string establishmentName { get; set; }
        public string establishmentAddress { get; set; }
        public string licenseStatus { get; set; }
        public string licenseType { get; set; }
        public string licenseNumber { get; set; }
        public DateTimeOffset? expiryDate { get; set; }
    }
}
