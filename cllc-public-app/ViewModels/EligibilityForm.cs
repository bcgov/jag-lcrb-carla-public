using System;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class EligibilityForm
    {
        public bool? IsConnectedToUnlicencedStore { get; set; }
        public string NameLocationUnlicencedRetailer { get; set; }
        public bool? IsRetailerStillOperating { get; set; }
        public DateTimeOffset? DateOperationsCeased { get; set; }
        public bool? IsInvolvedIllegalDistribution { get; set; }
        public string NameLocationRetailer { get; set; }
        public string IllegalDistributionInvolvementDetails { get; set; }
        public bool? IsInvolvementContinuing { get; set; }
        public DateTimeOffset? DateInvolvementCeased { get; set; }
        public bool IsEligibilityCertified { get; set; }
        public string EligibilitySignature { get; set; }
        public DateTimeOffset? DateSignedOrDismissed { get; set; }
    }
}
