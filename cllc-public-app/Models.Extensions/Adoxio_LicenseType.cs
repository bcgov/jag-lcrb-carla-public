using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class Adoxio_LicenseTypeExtensions
    {        

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
