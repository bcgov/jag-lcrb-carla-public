using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public class OngoingLicenseeData
    {
        public Application Application { get; set; }
        public List<LicenseeChangeLog> ChangeLogs { get; set; }
        public int NonTerminatedApplications { get; set; }
        public LegalEntity CurrentHierarchy { get; set; }
        public List<ApplicationLicenseSummary> Licenses { get; set; }
    }
}
