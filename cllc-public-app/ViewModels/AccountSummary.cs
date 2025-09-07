using System;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class AccountSummaryLicence
    {
        public string licenceId { get; set; }
        public string licenceType { get; set; }
        public LicenceTypeCategory? licenceTypeCategory { get; set; }
        public DateTimeOffset? expiryDate { get; set; }
        public int? statusCode { get; set; }
    }

    public class AccountSummaryApplications
    {
        public string applicationId { get; set; }
        public string name { get; set; }
        public ApplicationType applicationType { get; set; }
        public AdoxioApplicantTypeCodes applicantType { get; set; }
        public AdoxioApplicationStatusCodes applicationStatus { get; set; }
    }

    public class AccountSummary
    {
        public string accountId { get; set; }
        public List<AccountSummaryApplications> applications { get; set; }
        public List<AccountSummaryLicence> licences { get; set; }
    }
}
