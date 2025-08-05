
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
            bool paymentRecieved = (application?.AdoxioPaymentrecieved == true);
            if (application?.AdoxioApplicationTypeId?.AdoxioName == "Permanent Change to a Licensee")
            {
                paymentRecieved = (
                    (!string.IsNullOrEmpty(application?._adoxioInvoiceValue) ||
                    !string.IsNullOrEmpty(application?._adoxioSecondaryapplicationinvoiceValue) // there exist an invoice
                    // and all existing invoices are paid
                    && (string.IsNullOrEmpty(application?._adoxioInvoiceValue) || (application?.AdoxioPrimaryapplicationinvoicepaid == 1))
                    && (string.IsNullOrEmpty(application?._adoxioSecondaryapplicationinvoiceValue) || application?.AdoxioSecondaryapplicationinvoicepaid == 1))
                );
            }

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
                if (shownStatus == "Intake" && !(paymentRecieved))
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
                           || shownStatus == "Pending Final Inspection" || shownStatus == "PendingFinalInspection"
                           || shownStatus == "Reviewing Inspection Results" || shownStatus == "ReviewingInspectionResults"
                           || (shownStatus == "Intake" && paymentRecieved))
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

        public static string GetTranslatedApplicationStatusV2(MicrosoftDynamicsCRMadoxioApplication application)
        {
            // gather the status from the application
            AdoxioApplicationStatusCodes status = (AdoxioApplicationStatusCodes)application.Statuscode;
            // create a string value to be displayed 
            string shownStatus = Enum.GetName(status.GetType(), status);

            // for legacy support, we need the payment received details.
            bool paymentRecieved = (application?.AdoxioPaymentrecieved == true);
            if (application?.AdoxioApplicationTypeId?.AdoxioName == "Permanent Change to a Licensee")
            {
                paymentRecieved = (
                    (!string.IsNullOrEmpty(application?._adoxioInvoiceValue) ||
                    !string.IsNullOrEmpty(application?._adoxioSecondaryapplicationinvoiceValue) // there exist an invoice
                                                                                                // and all existing invoices are paid
                    && (string.IsNullOrEmpty(application?._adoxioInvoiceValue) || (application?.AdoxioPrimaryapplicationinvoicepaid == 1))
                    && (string.IsNullOrEmpty(application?._adoxioSecondaryapplicationinvoiceValue) || application?.AdoxioSecondaryapplicationinvoicepaid == 1))
                );
            }


            // in general* the flow for licence applications should be: 
            // intake -> submitted -> under review -> application assessment ->  pending final inspection -> reviewing inspection results -> pending licence fee -> approved
            // for changes or endorsement applications*: 
            // intake -> submitted -> under review -> application assessment --> approved

            // *if LG review is required:
            // intake -> pending LG review -> submitted...

            if (shownStatus == "Intake") {
                switch (application?.AdoxioApplicationTypeId?.AdoxioName) {
                    case "CRS Transfer of Ownership":
                    case "Liquor Licence Transfer":
                        shownStatus = "Transfer Initiated";
                        break;
                    default:
                        shownStatus = "Not Submitted";
                        break;
                }
            }

            if (shownStatus == "PendingForLGFNPFeedback") {
                shownStatus = "Pending External Review";
            }

            if (shownStatus == "InProgress"
                           || shownStatus == "Processed"        // legacy support
                                                                //|| shownStatus == "Submitted"
                           || shownStatus == "Under Review" || shownStatus == "UnderReview"
                           || shownStatus == "Application Assessment" || shownStatus == "ApplicationAssessment"
                           || shownStatus == "Pending Final Inspection" || shownStatus == "PendingFinalInspection"
                           || shownStatus == "Reviewing Inspection Results" || shownStatus == "ReviewingInspectionResults"
                           || (shownStatus == "Intake" && paymentRecieved)) {  // legacy support

                shownStatus = "Under Review";
            }

            if (shownStatus == "LicenseeActionRequired")
            {
                shownStatus = "Licensee Action Required";
            }

            // some application types get special messages in spe

            // permanent change to a licensee

            // licence transfer

            // renewal



            /*
            bool paymentRecieved = (application?.AdoxioPaymentrecieved == true);
            if (application?.AdoxioApplicationTypeId?.AdoxioName == "Permanent Change to a Licensee")
            {
                paymentRecieved = (
                    (!string.IsNullOrEmpty(application?._adoxioInvoiceValue) ||
                    !string.IsNullOrEmpty(application?._adoxioSecondaryapplicationinvoiceValue) // there exist an invoice
                    // and all existing invoices are paid
                    && (string.IsNullOrEmpty(application?._adoxioInvoiceValue) || (application?.AdoxioPrimaryapplicationinvoicepaid == 1))
                    && (string.IsNullOrEmpty(application?._adoxioSecondaryapplicationinvoiceValue) || application?.AdoxioSecondaryapplicationinvoicepaid == 1))
                );
            }

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
                if (shownStatus == "Intake" && !(paymentRecieved))
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
                           || shownStatus == "Pending Final Inspection" || shownStatus == "PendingFinalInspection"
                           || shownStatus == "Reviewing Inspection Results" || shownStatus == "ReviewingInspectionResults"
                           || (shownStatus == "Intake" && paymentRecieved))
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
            */

            return shownStatus;
        }

        public static string GetLicenceStatus(MicrosoftDynamicsCRMadoxioLicences licence, IList<MicrosoftDynamicsCRMadoxioApplication> applications)
        {
            LicenceStatusCodes status = (LicenceStatusCodes)licence.Statuscode;
            return Enum.GetName(status.GetType(), status);
        }
    }
}
