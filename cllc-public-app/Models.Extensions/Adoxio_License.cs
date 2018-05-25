using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class Adoxio_LicenseExtensions
    {

        public async static Task<AdoxioLicense> ToViewModel(this Adoxio_licences dynamicsLicense, Contexts.Microsoft.Dynamics.CRM.System _system)
        {
            AdoxioLicense adoxioLicenseVM = new AdoxioLicense();

            // fetch the establishment and get name and address
            Guid? adoxioEstablishmentId = dynamicsLicense._adoxio_establishment_value;
            if (adoxioEstablishmentId != null)
            {
                Contexts.Microsoft.Dynamics.CRM.Adoxio_establishment establishment = await _system.Adoxio_establishments.ByKey(adoxio_establishmentid: adoxioEstablishmentId).GetValueAsync();
                adoxioLicenseVM.establishmentName = establishment.Adoxio_name;
                adoxioLicenseVM.establishmentAddress = establishment.Adoxio_addressstreet
                                                    + ", " + establishment.Adoxio_addresscity
                                                    + " " + establishment.Adoxio_addresspostalcode;
            }

            // fetch the licence status
            int? adoxio_licenceStatusId = dynamicsLicense.Statuscode;
            if (adoxio_licenceStatusId != null)
            {
                adoxioLicenseVM.licenseStatus = dynamicsLicense.Statuscode.ToString();
            }

            // fetch the licence type
            Guid? adoxio_licencetypeId = dynamicsLicense._adoxio_licencetype_value;
            if (adoxio_licencetypeId != null)
            {
                Adoxio_licencetype adoxio_licencetype = await _system.Adoxio_licencetypes.ByKey(adoxio_licencetypeid: adoxio_licencetypeId).GetValueAsync();
                adoxioLicenseVM.licenseType = adoxio_licencetype.Adoxio_name;
            }

            // fetch license number
            adoxioLicenseVM.licenseNumber = dynamicsLicense.Adoxio_licencenumber;

            return adoxioLicenseVM;
        }
    }
}
