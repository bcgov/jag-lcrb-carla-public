using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class Adoxio_ApplicationExtensions
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
        

            public async static Task<Application> ToViewModel(this MicrosoftDynamicsCRMadoxioApplication dynamicsApplication, IDynamicsClient dynamicsClient)
        {
            Application adoxioApplicationVM = new ViewModels.Application()
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

                LicenceFeeInvoicePaid = (dynamicsApplication.AdoxioLicencefeeinvoicepaid == true),

                //get application status
                ApplicationStatus = (AdoxioApplicationStatusCodes)dynamicsApplication.Statuscode,
                ApplicantType = (AdoxioApplicantTypeCodes)dynamicsApplication.AdoxioApplicanttype,

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
                adoxioApplicationVM.Id = dynamicsApplication.AdoxioApplicationid.ToString();

            
            //get applying person from Contact entity
            if (dynamicsApplication._adoxioApplyingpersonValue != null)
            {
                Guid applyingPersonId = Guid.Parse(dynamicsApplication._adoxioApplyingpersonValue);
                var contact = await dynamicsClient.GetContactById(applyingPersonId);
                adoxioApplicationVM.ApplyingPerson = contact.Fullname;
            }
            if (dynamicsApplication._adoxioApplicantValue != null)
            {
                var applicant = await dynamicsClient.GetAccountById(Guid.Parse(dynamicsApplication._adoxioApplicantValue));
                adoxioApplicationVM.Applicant = applicant.ToViewModel();
            }

            //get license type from Adoxio_licencetype entity
            if (dynamicsApplication._adoxioLicencetypeValue != null)
            {
                Guid adoxio_licencetypeId = Guid.Parse(dynamicsApplication._adoxioLicencetypeValue);
                var adoxio_licencetype = dynamicsClient.GetAdoxioLicencetypeById(adoxio_licencetypeId);
                adoxioApplicationVM.LicenseType = adoxio_licencetype.AdoxioName;
            }

            if (dynamicsApplication.AdoxioAppchecklistfinaldecision != null)
            {
                adoxioApplicationVM.AppChecklistFinalDecision = (AdoxioFinalDecisionCodes)dynamicsApplication.AdoxioAppchecklistfinaldecision;
            }

            //get payment info
            if (dynamicsApplication.AdoxioInvoicetrigger == 1)
            {
                adoxioApplicationVM.AdoxioInvoiceTrigger = GeneralYesNo.Yes;
                adoxioApplicationVM.IsSubmitted = true;
            }
            else
            {
                adoxioApplicationVM.AdoxioInvoiceTrigger = GeneralYesNo.No;
                adoxioApplicationVM.IsSubmitted = false;
            }

            if (dynamicsApplication.AdoxioLicenceFeeInvoice != null)
            {
                adoxioApplicationVM.LicenceFeeInvoice = dynamicsApplication.AdoxioLicenceFeeInvoice.ToViewModel();
            }

            if (dynamicsApplication.AdoxioAssignedLicence != null)
            {
                adoxioApplicationVM.AssignedLicence = dynamicsApplication.AdoxioAssignedLicence.ToViewModel(dynamicsClient);
            }

            adoxioApplicationVM.PrevPaymentFailed = (dynamicsApplication._adoxioInvoiceValue != null) && (!adoxioApplicationVM.IsSubmitted);

            return adoxioApplicationVM;
        }
    }
}
