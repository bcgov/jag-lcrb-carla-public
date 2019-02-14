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

        public static void CopyValues(this MicrosoftDynamicsCRMadoxioApplication to, ViewModels.AdoxioApplication from)
        {
            to.AdoxioName = from.name;
            //to.Adoxio_jobnumber = from.jobNumber;            
            to.AdoxioEstablishmentpropsedname = from.establishmentName;
            to.AdoxioEstablishmentaddressstreet = from.establishmentaddressstreet;
            to.AdoxioEstablishmentaddresscity = from.establishmentaddresscity;
            to.AdoxioEstablishmentaddresspostalcode = from.establishmentaddresspostalcode;
            to.AdoxioAddresscity = from.establishmentaddresscity;
            to.AdoxioEstablishmentparcelid = from.establishmentparcelid;
            to.AdoxioContactpersonfirstname = from.contactpersonfirstname;
            to.AdoxioContactpersonlastname = from.contactpersonlastname;
            to.AdoxioRole = from.contactpersonrole;
            to.AdoxioEmail = from.contactpersonemail;
            to.AdoxioContactpersonphone = from.contactpersonphone;
            to.AdoxioAuthorizedtosubmit = from.authorizedtosubmit;
            to.AdoxioSignatureagreement = from.signatureagreement;
            to.AdoxioAdditionalpropertyinformation = from.additionalpropertyinformation;


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



        public async static Task<AdoxioApplication> ToViewModel(this MicrosoftDynamicsCRMadoxioApplication dynamicsApplication, IDynamicsClient dynamicsClient)
        {
            AdoxioApplication adoxioApplicationVM = new ViewModels.AdoxioApplication();

            // id
            if (dynamicsApplication.AdoxioApplicationid != null)
                adoxioApplicationVM.id = dynamicsApplication.AdoxioApplicationid.ToString();

            //get name
            adoxioApplicationVM.name = dynamicsApplication.AdoxioName;

            //get applying person from Contact entity
            if (dynamicsApplication._adoxioApplyingpersonValue != null)
            {
                Guid applyingPersonId = Guid.Parse(dynamicsApplication._adoxioApplyingpersonValue);
                var contact = await dynamicsClient.GetContactById(applyingPersonId);
                adoxioApplicationVM.applyingPerson = contact.Fullname;
            }
            if (dynamicsApplication._adoxioApplicantValue != null)
            {
                var applicant = await dynamicsClient.GetAccountById(Guid.Parse(dynamicsApplication._adoxioApplicantValue));
                adoxioApplicationVM.applicant = applicant.ToViewModel();
            }

            //get job number
            adoxioApplicationVM.jobNumber = dynamicsApplication.AdoxioJobnumber;

            //get license type from Adoxio_licencetype entity
            if (dynamicsApplication._adoxioLicencetypeValue != null)
            {
                Guid adoxio_licencetypeId = Guid.Parse(dynamicsApplication._adoxioLicencetypeValue);
                var adoxio_licencetype = dynamicsClient.GetAdoxioLicencetypeById(adoxio_licencetypeId);
                adoxioApplicationVM.licenseType = adoxio_licencetype.AdoxioName;
            }

            //get establishment name and address
            adoxioApplicationVM.establishmentName = dynamicsApplication.AdoxioEstablishmentpropsedname;
            adoxioApplicationVM.establishmentaddressstreet = dynamicsApplication.AdoxioEstablishmentaddressstreet;
            adoxioApplicationVM.establishmentaddresscity = dynamicsApplication.AdoxioEstablishmentaddresscity;
            adoxioApplicationVM.establishmentaddresspostalcode = dynamicsApplication.AdoxioEstablishmentaddresspostalcode;
            adoxioApplicationVM.establishmentAddress = dynamicsApplication.AdoxioEstablishmentaddressstreet
                                                    + ", " + dynamicsApplication.AdoxioEstablishmentaddresscity
                                                    + " " + dynamicsApplication.AdoxioEstablishmentaddresspostalcode;

            adoxioApplicationVM.ServicehHoursStandardHours = dynamicsApplication.AdoxioServicehoursstandardhours;
            adoxioApplicationVM.ServiceHoursSundayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehourssundayopen;
            adoxioApplicationVM.ServiceHoursSundayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehourssundayclose;
            adoxioApplicationVM.ServiceHoursMondayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehoursmondayopen;
            adoxioApplicationVM.ServiceHoursMondayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehoursmondayclose;
            adoxioApplicationVM.ServiceHoursTuesdayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehourstuesdayopen;
            adoxioApplicationVM.ServiceHoursTuesdayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehourstuesdayclose;
            adoxioApplicationVM.ServiceHoursWednesdayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehourswednesdayopen;
            adoxioApplicationVM.ServiceHoursWednesdayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehourswednesdayclose;
            adoxioApplicationVM.ServiceHoursThursdayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehoursthursdayopen;
            adoxioApplicationVM.ServiceHoursThursdayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehoursthursdayclose;
            adoxioApplicationVM.ServiceHoursFridayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehoursfridayopen;
            adoxioApplicationVM.ServiceHoursFridayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehoursfridayclose;
            adoxioApplicationVM.ServiceHoursSaturdayOpen = (ServiceHours?)dynamicsApplication.AdoxioServicehourssaturdayopen;
            adoxioApplicationVM.ServiceHoursSaturdayClose = (ServiceHours?)dynamicsApplication.AdoxioServicehourssaturdayclose;

            adoxioApplicationVM.licenceFeeInvoicePaid = (dynamicsApplication.AdoxioLicencefeeinvoicepaid == true);

            //get application status
            adoxioApplicationVM.applicationStatus = (AdoxioApplicationStatusCodes)dynamicsApplication.Statuscode;

            if (dynamicsApplication.AdoxioAppchecklistfinaldecision != null)
            {
                adoxioApplicationVM.AppChecklistFinalDecision = (AdoxioFinalDecisionCodes)dynamicsApplication.AdoxioAppchecklistfinaldecision;
            }

            // set a couple of read-only flags to indicate status
            adoxioApplicationVM.isPaid = (dynamicsApplication.AdoxioPaymentrecieved != null && (bool)dynamicsApplication.AdoxioPaymentrecieved);

            //get parcel id
            adoxioApplicationVM.establishmentparcelid = dynamicsApplication.AdoxioEstablishmentparcelid;

            //get additional property info
            adoxioApplicationVM.additionalpropertyinformation = dynamicsApplication.AdoxioAdditionalpropertyinformation;

            //get payment info
            if (dynamicsApplication.AdoxioInvoicetrigger == 1)
            {
                adoxioApplicationVM.adoxioInvoiceTrigger = GeneralYesNo.Yes;
                adoxioApplicationVM.isSubmitted = true;
            }
            else
            {
                adoxioApplicationVM.adoxioInvoiceTrigger = GeneralYesNo.No;
                adoxioApplicationVM.isSubmitted = false;
            }
            adoxioApplicationVM.adoxioInvoiceId = dynamicsApplication._adoxioInvoiceValue;
            //TODO set in autorest
            adoxioApplicationVM.paymentreceiveddate = dynamicsApplication.AdoxioPaymentreceiveddate; //DateTime.Now;
            adoxioApplicationVM.prevPaymentFailed = (dynamicsApplication._adoxioInvoiceValue != null) && (!adoxioApplicationVM.isSubmitted);

            //get declarations
            adoxioApplicationVM.authorizedtosubmit = dynamicsApplication.AdoxioAuthorizedtosubmit;
            adoxioApplicationVM.signatureagreement = dynamicsApplication.AdoxioSignatureagreement;

            //get contact details
            adoxioApplicationVM.contactpersonfirstname = dynamicsApplication.AdoxioContactpersonfirstname;
            adoxioApplicationVM.contactpersonlastname = dynamicsApplication.AdoxioContactpersonlastname;
            adoxioApplicationVM.contactpersonrole = dynamicsApplication.AdoxioRole;
            adoxioApplicationVM.contactpersonemail = dynamicsApplication.AdoxioEmail;
            adoxioApplicationVM.contactpersonphone = dynamicsApplication.AdoxioContactpersonphone;

            adoxioApplicationVM.modifiedOn = dynamicsApplication.Modifiedon;

            //get record audit info
            adoxioApplicationVM.createdon = dynamicsApplication.Createdon;
            adoxioApplicationVM.modifiedon = dynamicsApplication.Modifiedon;

            if (dynamicsApplication.AdoxioLicenceFeeInvoice != null)
            {
                adoxioApplicationVM.licenceFeeInvoice = dynamicsApplication.AdoxioLicenceFeeInvoice.ToViewModel();
            }

            if (dynamicsApplication.AdoxioAssignedLicence != null)
            {
                adoxioApplicationVM.assignedLicence = dynamicsApplication.AdoxioAssignedLicence.ToViewModel(dynamicsClient);
            }

            return adoxioApplicationVM;
        }
    }
}
