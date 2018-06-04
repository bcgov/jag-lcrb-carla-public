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

        public async static Task<AdoxioApplication> ToViewModel(this Adoxio_application dynamicsApplication, Interfaces.Microsoft.Dynamics.CRM.System _system)
        {
            AdoxioApplication adoxioApplicationVM = new ViewModels.AdoxioApplication();

            //get name
            adoxioApplicationVM.name = dynamicsApplication.Adoxio_name;

            //get applying person from Contact entity
            Guid? applyingPersonId = dynamicsApplication._adoxio_applyingperson_value;
            if (applyingPersonId != null)
            {
                Interfaces.Microsoft.Dynamics.CRM.Contact contact = await _system.Contacts.ByKey(contactid: applyingPersonId).GetValueAsync();
                adoxioApplicationVM.applyingPerson = contact.Fullname;
            }

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
            adoxioApplicationVM.establishmentAddress = dynamicsApplication.Adoxio_establishmentaddressstreet
                                                    + ", " + dynamicsApplication.Adoxio_establishmentaddresscity
                                                    + " " + dynamicsApplication.Adoxio_establishmentaddresspostalcode;

            //get application status
            adoxioApplicationVM.applicationStatus = dynamicsApplication.Statuscode.ToString();
            return adoxioApplicationVM;
        }
    }
}
