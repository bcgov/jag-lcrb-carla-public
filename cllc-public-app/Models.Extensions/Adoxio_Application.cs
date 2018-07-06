using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

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
            //TODO add to autorest
            //to.AdoxioRole = from.contactpersonrole;
            to.AdoxioEmail = from.contactpersonemail;
            to.AdoxioContactpersonphone = from.contactpersonphone;
            //TODO add to autorest
            //to.AdoxioAuthorizedtosubmit = from.authorizedtosubmit;
            to.AdoxioSignatureagreement = from.signatureagreement;
            //TODO add to autorest
            //to.AdoxioAdditionalpropertyinformation = from.additionalpropertyinformation;
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



        public async static Task<AdoxioApplication> ToViewModel(this MicrosoftDynamicsCRMadoxioApplication dynamicsApplication, IDynamicsClient dynamicsClient )
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
				var adoxio_licencetype = await dynamicsClient.GetAdoxioLicencetypeById(adoxio_licencetypeId);
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

            //get application status
            adoxioApplicationVM.applicationStatus = dynamicsApplication.Statuscode.ToString();

            // set a couple of read-only flags to indicate status
			adoxioApplicationVM.isPaid = (dynamicsApplication.AdoxioPaymentrecieved != null && (bool)dynamicsApplication.AdoxioPaymentrecieved);
            
            //get parcel id
            adoxioApplicationVM.establishmentparcelid = dynamicsApplication.AdoxioEstablishmentparcelid;

            //get additional property info
            //TODO add to autorest
            //adoxioApplicationVM.additionalpropertyinformation = dynamicsApplication.AdoxioAdditionalpropertyinformation;

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

			adoxioApplicationVM.prevPaymentFailed = (dynamicsApplication._adoxioInvoiceValue != null) && (!adoxioApplicationVM.isSubmitted);

			//get declarations
            //TODO add to autorest
            //adoxioApplicationVM.authorizedtosubmit = dynamicsApplication.AdoxioAuthorizedtosubmit;
            adoxioApplicationVM.signatureagreement = dynamicsApplication.AdoxioSignatureagreement;

            //get contact details
            adoxioApplicationVM.contactpersonfirstname = dynamicsApplication.AdoxioContactpersonfirstname;
            adoxioApplicationVM.contactpersonlastname = dynamicsApplication.AdoxioContactpersonlastname;
            //TODO add to autorest
            //adoxioApplicationVM.contactpersonrole = dynamicsApplication.AdoxioRole;
            adoxioApplicationVM.contactpersonemail = dynamicsApplication.AdoxioEmail;
            adoxioApplicationVM.contactpersonphone = dynamicsApplication.AdoxioContactpersonphone;

            return adoxioApplicationVM;
        }

        /***** 
         * To be disabled (Odata) 
         * ****/
        //     public async static Task<MicrosoftDynamicsCRMadoxioapplication> ToModel(this AdoxioApplication adoxioApplicationVM, Interfaces.Microsoft.Dynamics.CRM.System _system)
        //     {
        //MicrosoftDynamicsCRMadoxioapplication result = null;
        //if (adoxioApplicationVM != null)
        //         {
        //	result = new MicrosoftDynamicsCRMadoxioapplication();
        //	if (adoxioApplicationVM.id != null)
        //		result.Adoxio_applicationid = Guid.Parse(adoxioApplicationVM.id);
        //	result.Adoxio_name = adoxioApplicationVM.name;
        //	result.Adoxio_Applicant = adoxioApplicationVM.applicant.ToModel();
        //	result.Adoxio_nameofapplicant = adoxioApplicationVM.applyingPerson;
        //	result.Adoxio_jobnumber = adoxioApplicationVM.jobNumber;
        //	//result._adoxio_licencetype_value = adoxioApplicationVM.licenseType;
        //	result.Adoxio_establishmentpropsedname = adoxioApplicationVM.establishmentName;
        //	result.Adoxio_establishmentaddressstreet = adoxioApplicationVM.establishmentaddressstreet;
        //	result.Adoxio_establishmentaddresscity = adoxioApplicationVM.establishmentaddresscity;
        //	result.Adoxio_establishmentaddresspostalcode = adoxioApplicationVM.establishmentaddresspostalcode;
        //             result.Adoxio_establishmentparcelid = adoxioApplicationVM.establishmentparcelid;
        //             //TODO add to autorest
        //             //result.Adoxio_additionalpropertyinformation = adoxioApplicationVM.additionalpropertyinformation;
        //             //TODO add to autorest
        //             //result.Adoxio_authorizedtosubmit = adoxioApplicationVM.authorizedtosubmit;
        //             result.Adoxio_signatureagreement = adoxioApplicationVM.signatureagreement;
        //             result.Adoxio_contactpersonfirstname = adoxioApplicationVM.contactpersonfirstname;
        //             result.Adoxio_contactpersonlastname = adoxioApplicationVM.contactpersonlastname;
        //             //TODO add to autorest
        //             //result.AdoxioRole = adoxioApplicationVM.contactpersonrole;
        //             result.Adoxio_email = adoxioApplicationVM.contactpersonemail;
        //             result.Adoxio_contactpersonphone = adoxioApplicationVM.contactpersonphone;


        //             // ??? result.Statuscode = adoxioApplicationVM.Statuscode;
        //         }
        //         return result;
        //     }

    }
}
