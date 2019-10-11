using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public enum LicenseeChangeType
    {
        unchanged  = 0, // never save this kind of change to dynamics
        add = 845280001,
        edit = 845280002,
        delete  = 845280000
    }

    public class LicenseeChangeLog
    {
        public string Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LicenseeChangeType? ChangeType { get; set; }
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
        public int? BusinessAccountType { get; set; }
        public int? NumberofSharesNew { get; set; }
        public int? NumberofSharesOld { get; set; }
        public int? Statecode { get; set; }
        public int? Statuscode { get; set; }
        public string EmailNew { get; set; }
        public string EmailOld { get; set; }
        public string FirstNameNew { get; set; }
        public string FirstNameOld { get; set; }
        public string JobNumber { get; set; }
        public string LastNameNew { get; set; }
        public string LastNameOld { get; set; }
        public string Name { get; set; }
        public System.DateTimeOffset? DateofBirthNew { get; set; }
        public System.DateTimeOffset? DateofBirthOld { get; set; }

        public Account BusinessAccount { get; set; }
        public Contact Contact { get; set; }
        public Account ParentBusinessAccount { get; set; }
        public string ApplicationId { get; set; }
        public string ParentLegalEntityId { get; set; }
        public string LegalEntityId { get; set; }
        public string ParentLinceseeChangeLogId { get; set; }
        public IList<LicenseeChangeLog> Children { get; set; }
    }
}
