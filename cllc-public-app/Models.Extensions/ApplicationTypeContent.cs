using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class ApplicationTypeContentExtensions
    {        

        public static ApplicationTypeContent ToViewModel(this MicrosoftDynamicsCRMadoxioApplicationtypecontent dynamicsApplicationTypeContent)
        {
            ApplicationTypeContent result = null;
            if (dynamicsApplicationTypeContent != null)
            {
                result = new ApplicationTypeContent()
                {
                    Id = dynamicsApplicationTypeContent.AdoxioApplicationtypecontentid,

                    Body = dynamicsApplicationTypeContent.AdoxioBody,
                    Name = dynamicsApplicationTypeContent.AdoxioName,
                    Category = (ContentCategory)dynamicsApplicationTypeContent.AdoxioCategory,
                    Iscoop = dynamicsApplicationTypeContent.AdoxioIscoop,
                    IsEstate = dynamicsApplicationTypeContent.AdoxioIsestate,
                    IsGeneralPartnership = dynamicsApplicationTypeContent.AdoxioIsgeneralpartnership,
                    IsIndigenousNation = dynamicsApplicationTypeContent.AdoxioIsindigenousnation,
                    IsLimitedliabilityCorporation = dynamicsApplicationTypeContent.AdoxioIslimitedliabilitycorporation,
                    IsLimitedliabilityPartnership = dynamicsApplicationTypeContent.AdoxioIslimitedliabilitypartnership,
                    IsLimitedPartnership = dynamicsApplicationTypeContent.AdoxioIslimitedpartnership,
                    IsLocalGovernment = dynamicsApplicationTypeContent.AdoxioIslocalgovernment,
                    IsPartnership = dynamicsApplicationTypeContent.AdoxioIspartnership,
                    IsPrivateCorporation = dynamicsApplicationTypeContent.AdoxioIsprivatecorporation,
                    IsPublicCorporation = dynamicsApplicationTypeContent.AdoxioIspubliccorporation,
                    IsSociety = dynamicsApplicationTypeContent.AdoxioIssociety,
                    IsSoleProprietorship = dynamicsApplicationTypeContent.AdoxioIssoleproprietorship,
                    IsTrust = dynamicsApplicationTypeContent.AdoxioIstrust,
                    IsUniversity = dynamicsApplicationTypeContent.AdoxioIsuniversity,
                    IsUnlimitedLiabilityCorporation = dynamicsApplicationTypeContent.AdoxioIsunlimitedliabilitycorporation
                };

                /*
                if (dynamicsApplicationTypeContent.AdoxioLicenceTypeId != null)
                {
                    result.LicenseType = dynamicsApplicationTypeContent.AdoxioLicenceTypeId.ToViewModel();
                }
                */
            }
            

            return result;
        }
    }
}
