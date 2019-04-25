using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class LicenseTypeExtensions
    {        

        public static LicenseType ToViewModel(this MicrosoftDynamicsCRMadoxioLicencetype dynamicsLicenseType)
        {
            LicenseType result = new LicenseType();            

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
