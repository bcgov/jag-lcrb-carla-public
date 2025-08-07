using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class ApplicationSummary
    {
        public string Id { get; set; }
        public string ApplicationStatus { get; set; }
        public string EstablishmentName { get; set; } //adoxio_establishmentpropsedname
        public string Name { get; set; } //adoxio_name
        public string JobNumber { get; set; } //adoxio_jobnumber
        public string ApplicationTypeName { get; set; }

        public System.DateTimeOffset? DateApplicationSubmitted { get; set; }
        public System.DateTimeOffset? DateApplicantSentToLG { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ApplicationTypeCategory? ApplicationTypeCategory { get; set; }
        public bool IsIndigenousNation { get; set; }
        public bool IsPaid { get; set; }
        public bool IsStructuralChange { get; set; }
        public string LicenceId { get; set; }
        public bool IsForLicence { get; set; }
        public string Portallabel { get; set; }
        public bool LGHasApproved { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public GeneralYesNo? IsApplicationComplete { get; set; }
        public List<string> Endorsements { get; set; }
        public string EstablishmentAddress { get; set; }
        public string EstablishmentAddressCity { get; set; }
        public string EstablishmentAddressPostalCode { get; set; }
        public string EstablishmentAddressStreet { get; set; }
        public string EstablishmentEmail { get; set; }
        public string EstablishmentParcelId { get; set; }
        public string EstablishmentPhone { get; set; }
        public string IndigenousNationId { get; set; }
        public string PoliceJurisdictionId { get; set; }
        public ApplicationExtension ApplicationExtension { get; set; }
    }
}
