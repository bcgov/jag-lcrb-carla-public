using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    
    public class ApplicationLicenseSummary
    {
        public string Id { get; set; } 
                
        public string ApplicationStatus { get; set; } 

        public string EstablishmentName { get; set; } //adoxio_establishmentpropsedname
        public string EstablishmentAddressStreet { get; set; }
        public string EstablishmentAddressCity { get; set; }
        public string EstablishmentAddressPostalCode { get; set; }

        public string Name { get; set; } //adoxio_name

        public string JobNumber { get; set; } //adoxio_jobnumber

        public License AssignedLicense { get; set; }

        public string LicenseId { get; set; }
        public string LicenseNumber { get; set; }
        public List<ApplicationType> AllowedActions { get; set; }

    }
}
