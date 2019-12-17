using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public enum InspectionStatus
    {
        Pass = 845280000,
        Fail = 845280001,
        Deficient = 845280002,
        ContraventionsIdentified = 845280003,
        ComplianceDeficiency = 845280004,
        NoContraventions = 845280005,
        Attempted = 845280006,
    }


    public enum TransferRequested
    {
        Yes = 845280001,
        No = 845280000
    }

    public class ApplicationLicenseSummary
    {
        public string LicenseId { get; set; }
        public string ApplicationId { get; set; }
        public string EstablishmentId {get; set; }
        public string EstablishmentName { get; set; }
        public string EstablishmentAddressStreet { get; set; }
        public string EstablishmentAddressCity { get; set; }
        public string EstablishmentAddressPostalCode { get; set; }

        public string EstablishmentPhoneNumber { get; set; }

        public string EstablishmentEmail { get; set; }
        public string LicenseNumber { get; set; }
        public List<ApplicationType> AllowedActions { get; set; }
        public string LicenceTypeName { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }

        public string Status { get; set; }

        public bool StoreInspected { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TransferRequested? TransferRequested { get; set; }
    }
}
