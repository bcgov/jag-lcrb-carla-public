extern alias DV;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Collections.Generic;
using DV::Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Public.Utils
{
    public static class StatusUtility
    {
        const string STATUS_ACTIVE = "Active";
        const string STATUS_PAYMENT_REQUIRED = "Payment Required";
        const string STATUS_RENEWAL_DUE = "Renewal Due";
        public static string GetTranslatedApplicationStatusV2(adoxio_application application, string appTypeName)
        {
            if (application.statuscode == null) return null;
            AdoxioApplicationStatusCodes status = (AdoxioApplicationStatusCodes)(int)application.statuscode;
            string shownStatus = Enum.GetName(status.GetType(), status);

            bool paymentRecieved = application.adoxio_PaymentRecieved == true;
            if (appTypeName == "Permanent Change to a Licensee")
            {
                paymentRecieved =
                    (application.adoxio_Invoice != null || application.adoxio_SecondaryApplicationInvoice != null)
                    && (application.adoxio_Invoice == null || application.adoxio_PrimaryApplicationInvoicePaid == adoxio_generalyesno.Yes)
                    && (application.adoxio_SecondaryApplicationInvoice == null || application.adoxio_SecondaryApplicationInvoicePaid == adoxio_generalyesno.Yes);
            }

            if (shownStatus == "Intake")
            {
                shownStatus = (appTypeName == "CRS Transfer of Ownership" || appTypeName == "Liquor Licence Transfer")
                    ? "Transfer Initiated"
                    : "Not Submitted";
            }

            if (shownStatus == "PendingForLGFNPFeedback")
                shownStatus = "Pending External Review";

            if (shownStatus == "InProgress"
                || shownStatus == "Processed"
                || shownStatus == "Under Review" || shownStatus == "UnderReview"
                || shownStatus == "Application Assessment" || shownStatus == "ApplicationAssessment"
                || shownStatus == "Pending Final Inspection" || shownStatus == "PendingFinalInspection"
                || shownStatus == "Reviewing Inspection Results" || shownStatus == "ReviewingInspectionResults"
                || (shownStatus == "Intake" && paymentRecieved))
            {
                shownStatus = "Under Review";
            }

            if (shownStatus == "LicenseeActionRequired")
                shownStatus = "Licensee Action Required";

            return shownStatus;
        }


        public static string GetLicenceStatus(adoxio_licences licence, IList<adoxio_application> applications)
        {
            if (licence.statuscode == null) return null;
            var status = (LicenceStatusCodes)(int)licence.statuscode;
            return Enum.GetName(status.GetType(), status);
        }
    }
}
