using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class Adoxio_ApplicationExtensions
    {

		/// <summary>
        /// Copy values from a Dynamics Application to another one
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
		public static void CopyValues(this Adoxio_application to, Adoxio_application from)
        {
			to.Adoxio_applicationid = from.Adoxio_applicationid;
			to.Adoxio_name = from.Adoxio_name;
			to.Adoxio_Applicant = from.Adoxio_Applicant;
			to.Adoxio_nameofapplicant = from.Adoxio_nameofapplicant;
			to.Adoxio_jobnumber = from.Adoxio_jobnumber;
			to._adoxio_licencetype_value = from._adoxio_licencetype_value;
			to.Adoxio_establishmentpropsedname = from.Adoxio_establishmentpropsedname;
			to.Adoxio_establishmentaddressstreet = from.Adoxio_establishmentaddressstreet;
			to.Adoxio_establishmentaddresscity = from.Adoxio_establishmentaddresscity;
			to.Adoxio_establishmentaddresspostalcode = from.Adoxio_establishmentaddresspostalcode;
			to.Adoxio_addresscity = from.Adoxio_addresscity;
			to.Statuscode = from.Statuscode;
        }

        public static void CopyValues(this Adoxio_application to, ViewModels.AdoxioApplication from)
        {
			to.Adoxio_name = from.name + " - " + from.establishmentName;
            //to.Adoxio_jobnumber = from.jobNumber;            
            to.Adoxio_establishmentpropsedname = from.establishmentName;
            to.Adoxio_establishmentaddressstreet = from.establishmentaddressstreet;
            to.Adoxio_establishmentaddresscity = from.establishmentaddresscity;
            to.Adoxio_establishmentaddresspostalcode = from.establishmentaddresspostalcode;
            to.Adoxio_addresscity = from.establishmentaddresscity;
            //if (!String.IsNullOrEmpty(from.applicationStatus))
            //{
            //    to.Statuscode = int.Parse(from.applicationStatus);
            //}
            //else
            //{
            //    to.Statecode = null;
            //}
        }

        public async static Task<AdoxioApplication> ToViewModel(this Adoxio_application dynamicsApplication, Interfaces.Microsoft.Dynamics.CRM.System _system)
        {
            AdoxioApplication adoxioApplicationVM = new ViewModels.AdoxioApplication();

			// id
			if (dynamicsApplication.Adoxio_applicationid != null)
			    adoxioApplicationVM.id = dynamicsApplication.Adoxio_applicationid.ToString();

            //get name
            adoxioApplicationVM.name = dynamicsApplication.Adoxio_name;

            //get applying person from Contact entity
            Guid? applyingPersonId = dynamicsApplication._adoxio_applyingperson_value;
            if (applyingPersonId != null)
            {
                Interfaces.Microsoft.Dynamics.CRM.Contact contact = await _system.Contacts.ByKey(contactid: applyingPersonId).GetValueAsync();
                adoxioApplicationVM.applyingPerson = contact.Fullname;
            }
			adoxioApplicationVM.applicant = dynamicsApplication.Adoxio_Applicant.ToViewModel();

            //get job number
            adoxioApplicationVM.jobNumber = dynamicsApplication.Adoxio_jobnumber;

            //get license type from Adoxio_licencetype entity
            Guid? adoxio_licencetypeId = dynamicsApplication._adoxio_licencetype_value;
            if (adoxio_licencetypeId != null)
            {
                Adoxio_licencetype adoxio_licencetype = await _system.Adoxio_licencetypes.ByKey(adoxio_licencetypeid: adoxio_licencetypeId).GetValueAsync();
                adoxioApplicationVM.licenseType = adoxio_licencetype.Adoxio_name;
            }

            //get establishment name and address
            adoxioApplicationVM.establishmentName = dynamicsApplication.Adoxio_establishmentpropsedname;
			adoxioApplicationVM.establishmentaddressstreet = dynamicsApplication.Adoxio_establishmentaddressstreet;
			adoxioApplicationVM.establishmentaddresscity = dynamicsApplication.Adoxio_establishmentaddresscity;
			adoxioApplicationVM.establishmentaddresspostalcode = dynamicsApplication.Adoxio_establishmentaddresspostalcode;
            adoxioApplicationVM.establishmentAddress = dynamicsApplication.Adoxio_establishmentaddressstreet
                                                    + ", " + dynamicsApplication.Adoxio_establishmentaddresscity
                                                    + " " + dynamicsApplication.Adoxio_establishmentaddresspostalcode;

            //get application status
            adoxioApplicationVM.applicationStatus = dynamicsApplication.Statuscode.ToString();

			return adoxioApplicationVM;
        }

        public async static Task<Adoxio_application> ToModel(this AdoxioApplication adoxioApplicationVM, Interfaces.Microsoft.Dynamics.CRM.System _system)
        {
			Adoxio_application result = null;
			if (adoxioApplicationVM != null)
            {
				result = new Adoxio_application();
				if (adoxioApplicationVM.id != null)
					result.Adoxio_applicationid = Guid.Parse(adoxioApplicationVM.id);
				result.Adoxio_name = adoxioApplicationVM.name;
				result.Adoxio_Applicant = adoxioApplicationVM.applicant.ToModel();
				result.Adoxio_nameofapplicant = adoxioApplicationVM.applyingPerson;
				result.Adoxio_jobnumber = adoxioApplicationVM.jobNumber;
				//result._adoxio_licencetype_value = adoxioApplicationVM.licenseType;
				result.Adoxio_establishmentpropsedname = adoxioApplicationVM.establishmentName;
				result.Adoxio_establishmentaddressstreet = adoxioApplicationVM.establishmentaddressstreet;
				result.Adoxio_establishmentaddresscity = adoxioApplicationVM.establishmentaddresscity;
				result.Adoxio_establishmentaddresspostalcode = adoxioApplicationVM.establishmentaddresspostalcode;
				//result.Statuscode = adoxioApplicationVM.Statuscode;
            }
            return result;
        }
    }
}
