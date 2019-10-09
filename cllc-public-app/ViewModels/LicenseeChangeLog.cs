using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class LicenseeChangeLog
    {
        public string id;
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
        public string LicenseeChangelogid { get; set; }
        public string Name { get; set; }
        public System.DateTimeOffset? DateofBirthNew { get; set; }
        public System.DateTimeOffset? DateofBirthOld { get; set; }

        public Account BusinessAccount { get; set; }
        public Contact Contact { get; set; }
        public Account ParentBusinessAccount { get; set; }
        public Application Application { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public LegalEntity LegalEntity { get; set; }
        public LicenseeChangeLog ParentLinceseeChangeLogId { get; set; }
        public IList<LicenseeChangeLog> Children { get; set; }
    }
}
