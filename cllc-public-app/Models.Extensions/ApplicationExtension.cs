using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class ApplicationTypeExtension
    {


        public static void CopyValues(this MicrosoftDynamicsCRMadoxioApplicationextension to, ViewModels.ApplicationExtension from)
        {
            to.AdoxioApplicationextensionid = from.Id;
            to.AdoxioHasLiquorTiedHouseThirdPartyAssociations = from.HasLiquorTiedHouseThirdPartyAssociations;
            to.AdoxioHasLiquorTiedHouseOwnershipOrControl = from.HasLiquorTiedHouseOwnershipOrControl;
            to.AdoxioHasLiquorTiedHouseFamilyMemberInvolvement = from.HasLiquorTiedHouseFamilyMemberInvolvement;
        }

        public static ViewModels.ApplicationExtension ToViewModel(this MicrosoftDynamicsCRMadoxioApplicationextension applicationExtension)
        {
            ViewModels.ApplicationExtension result = null;
            if (applicationExtension != null)
            {
                result = new ApplicationExtension
                {
                    Id = applicationExtension.AdoxioApplicationextensionid,
                    HasLiquorTiedHouseThirdPartyAssociations = applicationExtension.AdoxioHasLiquorTiedHouseThirdPartyAssociations,
                    HasLiquorTiedHouseOwnershipOrControl = applicationExtension.AdoxioHasLiquorTiedHouseOwnershipOrControl,
                    HasLiquorTiedHouseFamilyMemberInvolvement = applicationExtension.AdoxioHasLiquorTiedHouseFamilyMemberInvolvement,
                    RelatedLeOrPclApplicationId = applicationExtension.AdoxioRelatedLeOrPclApplication?.AdoxioApplicationid
                };
            }
            return result;
        }

    }
}

