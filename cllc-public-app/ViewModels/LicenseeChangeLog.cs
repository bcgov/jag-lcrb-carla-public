using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public enum LicenseeChangeType
    {
        unchanged = 0, // never save this kind of change to dynamics
        addLeadership = 845280003,
        updateLeadership = 845280004,
        removeLeadership = 845280005,
        addBusinessShareholder = 845280006,
        updateBusinessShareholder = 845280007,
        removeBusinessShareholder = 845280008,
        addIndividualShareholder = 845280009,
        updateIndividualShareholder = 845280010,
        removeIndividualShareholder = 845280011,
    }

    public class LicenseeChangeLog
    {
        public string Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LicenseeChangeType? ChangeType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public AdoxioApplicantTypeCodes? BusinessType { get; set; }
        public bool? IsDirectorNew { get; set; }
        public bool? IsDirectorOld { get; set; }
        public bool? IsManagerNew { get; set; }
        public bool? IsManagerOld { get; set; }
        public bool? IsOfficerNew { get; set; }
        public bool? IsOfficerOld { get; set; }
        public bool? IsShareholderNew { get; set; }
        public bool? IsShareholderOld { get; set; }
        public bool? IsTrusteeNew { get; set; }
        public bool? IsTrusteeOld { get; set; }
        public int? NumberofSharesNew { get; set; }
        public int? NumberofSharesOld { get; set; }
        public int? TotalSharesNew { get; set; }
        public int? TotalSharesOld { get; set; }
        public int? Statecode { get; set; }
        public int? Statuscode { get; set; }
        public string EmailNew { get; set; }
        public string EmailOld { get; set; }
        public string FirstNameNew { get; set; }
        public string FirstNameOld { get; set; }
        public string JobNumber { get; set; }
        public string LastNameNew { get; set; }
        public string LastNameOld { get; set; }
        public string BusinessNameNew { get; set; }
        public string BusinessNameOld { get; set; }
        public string TitleNew { get; set; }
        public string TitleOld { get; set; }
        public System.DateTimeOffset? DateofBirthNew { get; set; }
        public System.DateTimeOffset? DateofBirthOld { get; set; }

        public Account BusinessAccount { get; set; }
        public Contact Contact { get; set; }
        public Account ParentBusinessAccount { get; set; }
        public string ApplicationId { get; set; }
        public string BusinessAccountId { get; set; }
        public string ParentLegalEntityId { get; set; }
        public string LegalEntityId { get; set; }
        public string ParentLinceseeChangeLogId { get; set; }
        public IList<LicenseeChangeLog> Children { get; set; }

        public decimal? InterestPercentageOld { get; set; }
        public decimal? InterestPercentageNew { get; set; }
    }
}
