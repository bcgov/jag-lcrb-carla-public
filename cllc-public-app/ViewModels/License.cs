using System;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum LicenceStatusCodes
    {
        Active = 1,
        Inactive = 2,
        Expired = 845280000,
        Cancelled = 845280001,
        PendingLicenceFee = 845280005,
        PreInspection = 845280006
    }

    public class License
    {
        public string Id { get; set; }
        public string EstablishmentId { get; set; }
        public string EstablishmentName { get; set; }
        public string EstablishmentEmail { get; set; }
        public string EstablishmentPhone { get; set; }
        public string EstablishmentAddress { get; set; }

        public string EstablishmentAddressStreet { get; set; }
        public string EstablishmentAddressCity { get; set; }
        public string EstablishmentAddressPostalCode { get; set; }
        public string EstablishmentParcelId { get; set; }

        public string LicenseStatus { get; set; }
        public string LicenseType { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseSubCategory { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }

        public List<Endorsement> Endorsements { get; set; }
        public List<string> TermsAndConditions { get; set; }
        public List<OffsiteStorage> OffsiteStorageLocations { get; set; }

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
