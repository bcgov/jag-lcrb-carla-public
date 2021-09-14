using System;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SpecialEventPoliceJobSummary
    {
        public List<SpecialEventSummary> InProgress { get; set; }
        public List<SpecialEventSummary> PoliceApproved { get; set; }
        public List<SpecialEventSummary> PoliceDenied { get; set; }
    }
}
