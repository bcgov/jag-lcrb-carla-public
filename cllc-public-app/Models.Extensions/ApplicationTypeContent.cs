extern alias DV;
using Gov.Lclb.Cllb.Public.ViewModels;
using DV::Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class ApplicationTypeContentExtensions
    {

        public static ApplicationTypeContent ToViewModel(this adoxio_applicationtypecontent content)
        {
            if (content == null) return null;
            return new ApplicationTypeContent
            {
                Id = content.adoxio_applicationtypecontentId?.ToString(),
                Body = content.adoxio_Body,
                Name = content.adoxio_name,
                Category = (ContentCategory)(int)(content.adoxio_Category ?? adoxio_applicationtypecontent_adoxio_category.Preamble),
                Iscoop = content.adoxio_IsCoop,
                IsEstate = content.adoxio_IsEstate,
                IsGeneralPartnership = content.adoxio_IsGeneralPartnership,
                IsIndigenousNation = content.adoxio_IsIndigenousNation,
                IsLimitedliabilityCorporation = content.adoxio_IsLimitedLiabilityCorporation,
                IsLimitedliabilityPartnership = content.adoxio_IsLimitedLiabilityPartnership,
                IsLimitedPartnership = content.adoxio_IsLimitedPartnership,
                IsLocalGovernment = content.adoxio_IsLocalGovernment,
                IsPartnership = content.adoxio_IsPartnership,
                IsPrivateCorporation = content.adoxio_IsPrivateCorporation,
                IsPublicCorporation = content.adoxio_IsPublicCorporation,
                IsSociety = content.adoxio_IsSociety,
                IsSoleProprietorship = content.adoxio_IsSoleProprietorship,
                IsTrust = content.adoxio_IsTrust,
                IsUniversity = content.adoxio_IsUniversity,
                IsUnlimitedLiabilityCorporation = content.adoxio_IsUnlimitedLiabilityCorporation
            };
        }
    }
}
