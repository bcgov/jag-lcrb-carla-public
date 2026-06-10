extern alias DV;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using DvLicenceType = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licencetype;

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

        public static LicenseType ToViewModel(this DvLicenceType licenceType)
        {
            if (licenceType == null) return null;
            return new LicenseType
            {
                id = licenceType.adoxio_licencetypeId?.ToString(),
                code = licenceType.adoxio_Code,
                name = licenceType.adoxio_name
            };
        }
    }
}
