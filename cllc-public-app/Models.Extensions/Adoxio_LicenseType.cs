using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class Adoxio_LicenseTypeExtensions
    {

        public static AdoxioLicenseType ToViewModel(this Adoxio_licencetype dynamicsLicenseType)
        {
            AdoxioLicenseType result = new AdoxioLicenseType();

            // fetch the establishment and get name and address
            Guid? licenceTypeId = dynamicsLicenseType.Adoxio_licencetypeid;
            
            if (dynamicsLicenseType.Adoxio_licencetypeid != null)
            {
                result.id = dynamicsLicenseType.Adoxio_licencetypeid.ToString();
            }
            result.code = dynamicsLicenseType.Adoxio_code;
            result.name = dynamicsLicenseType.Adoxio_name;

            return result;
        }

        public static AdoxioLicenseType ToViewModel(this MicrosoftDynamicsCRMadoxioLicencetype dynamicsLicenseType)
        {
            AdoxioLicenseType result = new AdoxioLicenseType();            

            if (dynamicsLicenseType.AdoxioLicencetypeid != null)
            {
                result.id = dynamicsLicenseType.AdoxioLicencetypeid;
            }
            result.code = dynamicsLicenseType.AdoxioCode;
            result.name = dynamicsLicenseType.AdoxioName;

            return result;
        }
    }
}
