using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class ApplicationExtensions
    {

        public static void CopyValues(this MicrosoftDynamicsCRMadoxioApplication to, ViewModels.Application from)
        {
            to.AdoxioName = from.Name;
            //to.Adoxio_jobnumber = from.jobNumber;            
            to.AdoxioEstablishmentpropsedname = from.EstablishmentName;
            to.AdoxioEstablishmentaddressstreet = from.EstablishmentAddressStreet;
            to.AdoxioEstablishmentaddresscity = from.EstablishmentAddressCity;
            to.AdoxioEstablishmentaddresspostalcode = from.EstablishmentAddressPostalCode;
            to.AdoxioAddresscity = from.EstablishmentAddressCity;
            to.AdoxioEstablishmentparcelid = from.EstablishmentParcelId;
            to.AdoxioEstablishmentphone = from.EstablishmentPhone;
            to.AdoxioEstablishmentemail = from.EstablishmentEmail;

            to.AdoxioContactpersonfirstname = from.ContactPersonFirstName;
            to.AdoxioContactpersonlastname = from.ContactPersonLastName;
            to.AdoxioRole = from.ContactPersonRole;
            to.AdoxioEmail = from.ContactPersonEmail;
            to.AdoxioContactpersonphone = from.ContactPersonPhone;
            to.AdoxioAuthorizedtosubmit = from.AuthorizedToSubmit;
            to.AdoxioSignatureagreement = from.SignatureAgreement;
            to.AdoxioAdditionalpropertyinformation = from.AdditionalPropertyInformation;


            to.AdoxioServicehoursstandardhours = from.ServicehHoursStandardHours;
            to.AdoxioServicehourssundayopen = (int?)from.ServiceHoursSundayOpen;
            to.AdoxioServicehourssundayclose = (int?)from.ServiceHoursSundayClose;
            to.AdoxioServicehoursmondayopen = (int?)from.ServiceHoursMondayOpen;
            to.AdoxioServicehoursmondayclose = (int?)from.ServiceHoursMondayClose;
            to.AdoxioServicehourstuesdayopen = (int?)from.ServiceHoursTuesdayOpen;
            to.AdoxioServicehourstuesdayclose = (int?)from.ServiceHoursTuesdayClose;
            to.AdoxioServicehourswednesdayopen = (int?)from.ServiceHoursWednesdayOpen;
            to.AdoxioServicehourswednesdayclose = (int?)from.ServiceHoursWednesdayClose;
            to.AdoxioServicehoursthursdayopen = (int?)from.ServiceHoursThursdayOpen;
            to.AdoxioServicehoursthursdayclose = (int?)from.ServiceHoursThursdayClose;
            to.AdoxioServicehoursfridayopen = (int?)from.ServiceHoursFridayOpen;
            to.AdoxioServicehoursfridayclose = (int?)from.ServiceHoursFridayClose;
            to.AdoxioServicehourssaturdayopen = (int?)from.ServiceHoursSaturdayOpen;
            to.AdoxioServicehourssaturdayclose = (int?)from.ServiceHoursSaturdayClose;
            to.AdoxioAuthorizedtosubmit = from.AuthorizedToSubmit;
             to.AdoxioSignatureagreement = from.SignatureAgreement;


            // comment out this next line as it is causing all application updates to fail (moved to controller)
            //to.AdoxioApplicanttype = (int)Enum.ToObject(typeof(Gov.Lclb.Cllb.Public.ViewModels.Adoxio_applicanttypecodes), from.applicantType);

            //if (from.adoxioInvoiceTrigger == GeneralYesNo.Yes)
            //{
            //	to.AdoxioInvoicetrigger = 1;
            //}

            //var adoxio_licencetype = dynamicsClient.GetAdoxioLicencetypeByName(from.licenseType).Result;
            //to.AdoxioLicenceType = adoxio_licencetype;
            //to._adoxioLicencetypeValue = adoxio_licencetype.AdoxioLicencetypeid;

            //if (!String.IsNullOrEmpty(from.applicationStatus))
            //{
            //    to.Statuscode = int.Parse(from.applicationStatus);
            //}
            //else
            //{
            //    to.Statecode = null;
            //}
        }

        public static void CopyValuesForChangeOfLocation(this MicrosoftDynamicsCRMadoxioApplication to, MicrosoftDynamicsCRMadoxioLicences from)
        {
            to.AdoxioEstablishmentpropsedname = from.AdoxioEstablishment.AdoxioName;
            
            /*
            to.AdoxioAddresscity = from.AdoxioEstablishmentaddresscity;
            to.AdoxioEstablishmentaddressstreet = from.AdoxioEstablishmentaddressstreet;
            to.AdoxioEstablishmentaddresscity = from.AdoxioEstablishmentaddresscity;
            to.AdoxioEstablishmentaddresspostalcode = from.AdoxioEstablishmentaddresspostalcode;
            if (from.AdoxioEstablishment != null)
            {
                to.AdoxioEstablishmentparcelid = from.AdoxioEstablishment.AdoxioParcelid;
            }
            */
            
        }

        public static void PopulateLicenceType(this MicrosoftDynamicsCRMadoxioApplication application, IDynamicsClient dynamicsClient)
        {
            if (application._adoxioLicencetypeValue != null)
            {
                application.AdoxioLicenceType = dynamicsClient.GetAdoxioLicencetypeById(application._adoxioLicencetypeValue);
            }
            
            if (application.AdoxioAssignedLicence != null && application.AdoxioAssignedLicence._adoxioLicencetypeValue != null)
            {
                application.AdoxioAssignedLicence.AdoxioLicenceType = dynamicsClient.GetAdoxioLicencetypeById(application.AdoxioAssignedLicence._adoxioLicencetypeValue);
            }

        }

        public async static Task<Application> ToViewModel(this MicrosoftDynamicsCRMadoxioApplication dynamicsApplication, IDynamicsClient dynamicsClient)
        {
            Application applicationVM = new ViewModels.Application()
            {
                Name = dynamicsApplication.AdoxioName,
                JobNumber = dynamicsApplication.AdoxioJobnumber,
                //get establishment name and address
                EstablishmentName = dynamicsApplication.AdoxioEstablishmentpropsedname,
                EstablishmentAddressStreet = dynamicsApplication.AdoxioEstablishmentaddressstreet,
                EstablishmentAddressCity = dynamicsApplication.AdoxioEstablishmentaddresscity,
                EstablishmentAddressPostalCode = dynamicsApplication.AdoxioEstablishmentaddresspostalcode,
                EstablishmentAddress = dynamicsApplication.AdoxioEstablishmentaddressstreet
                                                    + ", " + dynamicsApplication.AdoxioEstablishmentaddresscity
                                                    + " " + dynamicsApplication.AdoxioEstablishmentaddresspostalcode,
                EstablishmentPhone = dynamicsApplication.AdoxioEstablishmentphone,
                EstablishmentEmail = dynamicsApplication.AdoxioEstablishmentemail,

                ServicehHoursStandardHours = dynamicsApplication.AdoxioServicehoursstandardhours,
                ServiceHoursSundayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehourssundayopen,
                ServiceHoursSundayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehourssundayclose,
                ServiceHoursMondayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehoursmondayopen,
                ServiceHoursMondayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehoursmondayclose,
                ServiceHoursTuesdayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehourstuesdayopen,
                ServiceHoursTuesdayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehourstuesdayclose,
                ServiceHoursWednesdayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehourswednesdayopen,
                ServiceHoursWednesdayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehourswednesdayclose,
                ServiceHoursThursdayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehoursthursdayopen,
                ServiceHoursThursdayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehoursthursdayclose,
                ServiceHoursFridayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehoursfridayopen,
                ServiceHoursFridayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehoursfridayclose,
                ServiceHoursSaturdayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehourssaturdayopen,
                ServiceHoursSaturdayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehourssaturdayclose,

                AuthorizedToSubmit = dynamicsApplication.AdoxioAuthorizedtosubmit,
                SignatureAgreement = dynamicsApplication.AdoxioSignatureagreement,

                LicenceFeeInvoicePaid = (dynamicsApplication.AdoxioLicencefeeinvoicepaid != null && dynamicsApplication.AdoxioLicencefeeinvoicepaid == true),

                // set a couple of read-only flags to indicate status
                IsPaid = (dynamicsApplication.AdoxioPaymentrecieved != null && (bool)dynamicsApplication.AdoxioPaymentrecieved),

                //get parcel id
                EstablishmentParcelId = dynamicsApplication.AdoxioEstablishmentparcelid,

                //get additional property info
                AdditionalPropertyInformation = dynamicsApplication.AdoxioAdditionalpropertyinformation,
                AdoxioInvoiceId = dynamicsApplication._adoxioInvoiceValue,
            
                PaymentReceivedDate = dynamicsApplication.AdoxioPaymentreceiveddate,
            
                //get contact details
                ContactPersonFirstName = dynamicsApplication.AdoxioContactpersonfirstname,
                ContactPersonLastName = dynamicsApplication.AdoxioContactpersonlastname,
                ContactPersonRole = dynamicsApplication.AdoxioRole,
                ContactPersonEmail = dynamicsApplication.AdoxioEmail,
                ContactPersonPhone = dynamicsApplication.AdoxioContactpersonphone,

                //get record audit info
                CreatedOn = dynamicsApplication.Createdon,
                ModifiedOn = dynamicsApplication.Modifiedon

            };


            // id
            if (dynamicsApplication.AdoxioApplicationid != null)
                applicationVM.Id = dynamicsApplication.AdoxioApplicationid.ToString();

            if (dynamicsApplication.Statuscode != null)
            {
                applicationVM.ApplicationStatus = (AdoxioApplicationStatusCodes)dynamicsApplication.Statuscode;
            }

            if (dynamicsApplication.AdoxioApplicanttype != null)
            {
                applicationVM.ApplicantType = (AdoxioApplicantTypeCodes)dynamicsApplication.AdoxioApplicanttype;
            }

            //get applying person from Contact entity
            if (dynamicsApplication._adoxioApplyingpersonValue != null)
            {
                Guid applyingPersonId = Guid.Parse(dynamicsApplication._adoxioApplyingpersonValue);
                var contact = await dynamicsClient.GetContactById(applyingPersonId);
                applicationVM.ApplyingPerson = contact.Fullname;
            }
            if (dynamicsApplication._adoxioApplicantValue != null)
            {
                var applicant = await dynamicsClient.GetAccountById(Guid.Parse(dynamicsApplication._adoxioApplicantValue));
                applicationVM.Applicant = applicant.ToViewModel();
            }

            //get license type from Adoxio_licencetype entity
            if (dynamicsApplication._adoxioLicencetypeValue != null)
            {
                Guid adoxio_licencetypeId = Guid.Parse(dynamicsApplication._adoxioLicencetypeValue);
                var adoxio_licencetype = dynamicsClient.GetAdoxioLicencetypeById(adoxio_licencetypeId);
                applicationVM.LicenseType = adoxio_licencetype.AdoxioName;
            }

            if (dynamicsApplication.AdoxioAppchecklistfinaldecision != null)
            {
                applicationVM.AppChecklistFinalDecision = (AdoxioFinalDecisionCodes)dynamicsApplication.AdoxioAppchecklistfinaldecision;
            }

            //get payment info
            if (dynamicsApplication.AdoxioInvoicetrigger != null && dynamicsApplication.AdoxioInvoicetrigger == 1)
            {
                applicationVM.AdoxioInvoiceTrigger = GeneralYesNo.Yes;
                applicationVM.IsSubmitted = true;
            }
            else
            {
                applicationVM.AdoxioInvoiceTrigger = GeneralYesNo.No;
                applicationVM.IsSubmitted = false;
            }

            if (dynamicsApplication.AdoxioLicenceFeeInvoice != null)
            {
                applicationVM.LicenceFeeInvoice = dynamicsApplication.AdoxioLicenceFeeInvoice.ToViewModel();
            }

            if (dynamicsApplication.AdoxioAssignedLicence != null)
            {
                applicationVM.AssignedLicence = dynamicsApplication.AdoxioAssignedLicence.ToViewModel(dynamicsClient);
            }

            applicationVM.PrevPaymentFailed = (dynamicsApplication._adoxioInvoiceValue != null) && (!applicationVM.IsSubmitted);

            return applicationVM;
        }


        public static ApplicationSummary ToSummaryViewModel(this MicrosoftDynamicsCRMadoxioApplication dynamicsApplication)
        {
            ApplicationSummary applicationSummary = new ViewModels.ApplicationSummary()
            {                
                Name = dynamicsApplication.AdoxioName,
                JobNumber = dynamicsApplication.AdoxioJobnumber,
                //get establishment name and address
                EstablishmentName = dynamicsApplication.AdoxioEstablishmentpropsedname
            };

            // id
            if (dynamicsApplication.AdoxioApplicationid != null)
            {
                applicationSummary.Id = dynamicsApplication.AdoxioApplicationid.ToString();
            }
                
            if (dynamicsApplication.Statuscode != null)
            {
                applicationSummary.ApplicationStatus = StatusUtility.GetTranslatedApplicationStatus (dynamicsApplication);
            }

            return applicationSummary;
        }

        public static ApplicationLicenseSummary ToLicenseSummaryViewModel(this MicrosoftDynamicsCRMadoxioApplication dynamicsApplication)
        {
            ApplicationLicenseSummary licenseSummary = new ViewModels.ApplicationLicenseSummary()
            {
                Name = dynamicsApplication.AdoxioName,
                JobNumber = dynamicsApplication.AdoxioJobnumber,
                //get establishment name and address
                EstablishmentName = dynamicsApplication.AdoxioEstablishmentpropsedname,
                AllowedActions = new List<ApplicationType>()
            };

            // id
            if (dynamicsApplication.AdoxioApplicationid != null)
            {
                licenseSummary.Id = dynamicsApplication.AdoxioApplicationid.ToString();
            }

            if (dynamicsApplication.Statuscode != null)
            {
                licenseSummary.ApplicationStatus = StatusUtility.GetTranslatedApplicationStatus(dynamicsApplication);
            }

            if (dynamicsApplication.AdoxioAssignedLicence != null)
            {
                licenseSummary.LicenseId = dynamicsApplication.AdoxioAssignedLicence.AdoxioLicencesid;
            }

            if (dynamicsApplication.AdoxioAssignedLicence != null &&
                dynamicsApplication.AdoxioAssignedLicence.AdoxioLicenceType != null &&
                dynamicsApplication.AdoxioAssignedLicence.AdoxioLicenceType.AdoxioLicencetypesApplicationtypes != null)
            {
                foreach (var item in dynamicsApplication.AdoxioAssignedLicence.AdoxioLicenceType.AdoxioLicencetypesApplicationtypes)
                {
                    licenseSummary.AllowedActions.Add(item.ToViewModel());
                }
            }

            return licenseSummary;
        }

        

    }
}
