
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Collections.Generic;

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
            }
            else
            {
                if (shownStatus == "Intake" && !(application.AdoxioPaymentrecieved == true))
                {
                    if (application.AdoxioApplicationTypeId != null && 
                    (application.AdoxioApplicationTypeId.AdoxioName == "CRS Transfer of Ownership" ||
                    application.AdoxioApplicationTypeId.AdoxioName == "Liquor Licence Transfer"))
                    {
                        shownStatus = "Transfer Initiated";
                    }
                    else if (application.AdoxioApplicationTypeId != null && application.AdoxioApplicationTypeId.AdoxioName == "CRS Location Change")
                    {
                        shownStatus = "Relocation Initiated";
                    }
                    else if (application.AdoxioApplicationTypeId != null && application.AdoxioApplicationTypeId.AdoxioName == "Licensee Changes")
                    {
                        shownStatus = "Licensee Change Initiated";
                    }
                    else
                    {
                        shownStatus = "Not Submitted";
                    }
                }
                else if (shownStatus == "InProgress" || shownStatus == "Under Review" || shownStatus == "UnderReview"
                           || shownStatus == "Pending Final Inspection" ||shownStatus == "PendingFinalInspection"
                           || shownStatus == "Reviewing Inspection Results" || shownStatus == "ReviewingInspectionResults"
                           || (shownStatus == "Intake" && application.AdoxioPaymentrecieved == true))
                {
                    if (application?.AdoxioApplicationTypeId?.AdoxioName == "CRS Transfer of Ownership" ||
                    application?.AdoxioApplicationTypeId?.AdoxioName == "Liquor Licence Transfer")
                    {
                        shownStatus = "Transfer Application Under Review";
                    }
                    else if (application.AdoxioApplicationTypeId != null && application.AdoxioApplicationTypeId.AdoxioName == "CRS Location Change")
                    {
                        shownStatus = "Relocation Application Under Review";
                    }
                    else if (application.AdoxioApplicationTypeId != null && application.AdoxioApplicationTypeId.AdoxioName == "Licensee Changes")
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
                else if (shownStatus == "PendingForLicenceFee" || shownStatus == "Pending For Licence Fee")
                {
                    shownStatus = "Pending Licence Fee";
                }
            }

            return shownStatus;
        }

        public static string GetLicenceStatus(MicrosoftDynamicsCRMadoxioLicences licence, IList<MicrosoftDynamicsCRMadoxioApplication> applications)
        {
            LicenceStatusCodes status = (LicenceStatusCodes)licence.Statuscode;
            return Enum.GetName(status.GetType(), status);
        }
    }
}
