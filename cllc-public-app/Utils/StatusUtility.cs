
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Gov.Lclb.Cllb.Public.Utils
{
    public static class StatusUtility
    {
        const string STATUS_ACTIVE = "Active";
        const string STATUS_PAYMENT_REQUIRED = "Payment Required";
        const string STATUS_RENEWAL_DUE = "Renewal Due";

        public static string GetTranslatedApplicationStatus(MicrosoftDynamicsCRMadoxioApplication application)
        {
            AdoxioApplicationStatusCodes status = (AdoxioApplicationStatusCodes)application.Statuscode;

            string shownStatus = Enum.GetName(status.GetType(), status);

            if (application.AdoxioAssignedLicence != null && shownStatus == "Approved")
            {
                shownStatus = STATUS_ACTIVE;
                if (application.AdoxioLicencefeeinvoicepaid != true && application.AdoxioLicenceType != null && application.AdoxioApplicationTypeId.AdoxioName == "Cannabis Retail Store")
                {
                    shownStatus = STATUS_PAYMENT_REQUIRED;
                }
                if (DateTimeOffset.Now > application.AdoxioAssignedLicence.AdoxioExpirydate)
                {
                    shownStatus = STATUS_RENEWAL_DUE;
                }
            }
            else
            {
                if (shownStatus == "Intake" && !(application.AdoxioPaymentrecieved == true))
                {
                    if (application.AdoxioLicenceType != null && application.AdoxioLicenceType.AdoxioName == "CRS Transfer of Ownership")
                    {
                        shownStatus = "Transfer Initiated";
                    }
                    else if (application.AdoxioLicenceType != null && application.AdoxioLicenceType.AdoxioName == "CRS Location Change")
                    {
                        shownStatus = "Relocation Initiated";
                    }
                    else if (application.AdoxioApplicationTypeId != null && application.AdoxioApplicationTypeId.AdoxioName == "Leadership Change")
                    {
                        shownStatus = "Licensee Change Initiated";
                    }
                    else
                    {
                        shownStatus = "Not Submitted";
                    }
                }
                else if (shownStatus == "InProgress" || shownStatus == "Under Review" || shownStatus == "UnderReview" || (shownStatus == "Intake" && application.AdoxioPaymentrecieved == true))
                {
                    if (application.AdoxioLicenceType != null && application.AdoxioLicenceType.AdoxioName == "CRS Transfer of Ownership")
                    {
                        shownStatus = "Transfer Application Under Review";
                    }
                    else if (application.AdoxioLicenceType != null && application.AdoxioLicenceType.AdoxioName == "CRS Location Change")
                    {
                        shownStatus = "Relocation Application Under Review";
                    }
                    else if (application.AdoxioApplicationTypeId != null && application.AdoxioApplicationTypeId.AdoxioName == "CRS Location Change")
                    {
                        shownStatus = "Licensee Change Under Review";
                    }
                    else
                    {
                        shownStatus = "Application Under Review";
                    }
                }
                else if (shownStatus == "Incomplete")
                {
                    shownStatus = "Application Incomplete";
                }
                else if (shownStatus == "PendingForLGFNPFeedback")
                {
                    shownStatus = "Pending External Review";
                }
            }

            return shownStatus;
        }

        public static string GetLicenceStatus(MicrosoftDynamicsCRMadoxioLicences licence, IList<MicrosoftDynamicsCRMadoxioApplication> applications)
        {
            var application = applications
                .OrderByDescending(app => app.AdoxioDatelicenceapproved)
                .Where(app => app.Statuscode == (int)Public.ViewModels.AdoxioApplicationStatusCodes.Approved).FirstOrDefault();
            if (application == null)
            {
                return null;
            }
            LicenceStatusCodes status = (LicenceStatusCodes)licence.Statuscode;

            string shownStatus = Enum.GetName(status.GetType(), status);

            if (licence != null && status == LicenceStatusCodes.Active)
            {
                shownStatus = STATUS_ACTIVE;
                if (DateTimeOffset.Now > licence.AdoxioExpirydate)
                {
                    shownStatus = STATUS_RENEWAL_DUE;
                }
            }

            // moved first year payment logic here
            if (licence != null && status == LicenceStatusCodes.PendingFistYearFee)
            {
                shownStatus = STATUS_PAYMENT_REQUIRED;
            }
            return shownStatus;
        }
    }
}
