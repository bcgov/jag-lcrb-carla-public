using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

    public enum EnumYesNo
    {
        Yes = 845280001,
        No = 845280000
    }

    public enum ApplicationTypeCategory
    {
        Cannabis = 845280000,
        Liquor = 845280001
    }

    public enum LicenceTypeCategory
    {
        Cannabis = 845280000,
        Liquor = 845280001
    }

    public class ApplicationLicenseSummary
    {
        public string LicenseId { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationTypeName { get; set; }
        public int? TemporaryRelocationStatus { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ApplicationTypeCategory? ApplicationTypeCategory { get; set; }
        public string EstablishmentId { get; set; }
        public string EstablishmentName { get; set; }
        public string EstablishmentAddressStreet { get; set; }
        public string EstablishmentAddressCity { get; set; }
        public string EstablishmentAddressPostalCode { get; set; }

        public string EstablishmentPhoneNumber { get; set; }

        public string LicenseSubCategory { get; set; }

        public string EstablishmentEmail { get; set; }
        public bool? EstablishmentIsOpen { get; set; }
        public string LicenseNumber { get; set; }
        public List<ApplicationType> AllowedActions { get; set; }
        public string LicenceTypeName { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }

        public string Status { get; set; }

        public bool MissingFirstYearLicenceFee { get; set; }
        public string CurrentOwner { get; set; }
        public bool ChecklistConclusivelyDeem { get; set; }
        public bool StoreInspected { get; set; }
        public bool TransferRequested { get; set; }

        public bool Dormant { get; set; }
        public bool Operated { get; set; }
        public bool Suspended { get; set; }

        public bool? AutoRenewal { get; set; }

        public bool TPORequested { get; set; }


        public string ThirdPartyOperatorAccountName { get; set; }
        public string ThirdPartyOperatorAccountId { get; set; }
        public List<Endorsement> Endorsements { get; set; }
        public List<string> TermsAndConditions { get; set; }
        public List<OffsiteStorage> OffsiteStorageLocations { get; set; }
        public List<CapacityArea> ServiceAreas { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LicenceTypeCategory? LicenceTypeCategory { get; set; }
        public string RepresentativeFullName { get; set; }
        public string RepresentativePhoneNumber { get; set; }
        public string RepresentativeEmail { get; set; }
        public bool? RepresentativeCanSubmitPermanentChangeApplications { get; set; }
        public bool? RepresentativeCanSignTemporaryChangeApplications { get; set; }
        public bool? RepresentativeCanObtainLicenceInformation { get; set; }
        public bool? RepresentativeCanSignGroceryStoreProofOfSale { get; set; }
        public bool? RepresentativeCanAttendEducationSessions { get; set; }
        public bool? RepresentativeCanAttendComplianceMeetings { get; set; }
        public bool? RepresentativeCanRepresentAtHearings { get; set; }
    }
}
