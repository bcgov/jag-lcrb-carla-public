extern alias DV;
using System.Collections.Generic;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.ViewModels;
using DV::Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class ApplicationTypeExtension
    {
        public static ViewModels.ApplicationExtension ToViewModel(this adoxio_applicationextension applicationExtension)
        {
            if (applicationExtension == null) return null;
            return new ApplicationExtension
            {
                Id = applicationExtension.adoxio_applicationextensionId?.ToString(),
                HasLiquorTiedHouseOwnershipOrControl = (int?)applicationExtension.adoxio_hasLiquortiedhouseownershiporcontrol,
                HasLiquorTiedHouseThirdPartyAssociations = (int?)applicationExtension.adoxio_hasliquortiedhousethirdpartyassociations,
                HasLiquorTiedHouseFamilyMemberInvolvement = (int?)applicationExtension.adoxio_hasliquortiedhousefamilymemberinvolvement,
                RelatedLeOrPclApplicationId = applicationExtension.adoxio_relatedleorpclapplication?.Id.ToString()
            };
        }
    }
}
