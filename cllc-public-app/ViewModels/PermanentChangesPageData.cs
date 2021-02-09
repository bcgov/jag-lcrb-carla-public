using System.Collections.Generic;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.ViewModels;
using Application = Gov.Lclb.Cllb.Public.ViewModels.Application;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class PermanentChangesPageData
    {
        public List<ApplicationLicenseSummary> Licences { get; set; }
        public Application Application { get; set; }

        public PaymentResult Primary { get; set; }
        public PaymentResult Secondary { get; set; }
    }
}
