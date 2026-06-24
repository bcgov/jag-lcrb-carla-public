extern alias DV;
using Gov.Lclb.Cllb.Public.ViewModels;
using System.Collections.Generic;
using DV::Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class ApplicationTypeExtensions
    {

        public static ApplicationType ToViewModel(this adoxio_applicationtype applicationType)
        {
            if (applicationType == null) return null;
            var result = new ApplicationType
            {
                Id = applicationType.adoxio_applicationtypeId?.ToString(),
                Name = applicationType.adoxio_name,
                ActionText = applicationType.adoxio_ActionText,
                Category = (ApplicationTypeCategory?)(int?)applicationType.adoxio_Category,
                ConnectedGroceryStore = (FormControlState?)(int?)applicationType.adoxio_ConnectedGroceryStore,
                LGandPoliceSelectors = (FormControlState?)(int?)applicationType.adoxio_LGandPoliceSelectors,
                CurrentEstablishmentAddress = (FormControlState?)(int?)applicationType.adoxio_CurrentEstablishmentAddress,
                EstablishmentName = (FormControlState?)(int?)applicationType.adoxio_EstablishmentName,
                EstablishmetNameIsReadOnly = applicationType.adoxio_IsLockEstablishmentName,
                FloorPlan = (FormControlState?)(int?)applicationType.adoxio_FloorPlan,
                FormReference = applicationType.adoxio_FormReference,
                LetterOfIntent = (FormControlState?)(int?)applicationType.adoxio_LetterofIntent,
                HasLESection = applicationType.adoxio_HasLESection,
                NewEstablishmentAddress = (FormControlState?)(int?)applicationType.adoxio_NewEstablishmentAddress,
                ProofofZoning = (FormControlState?)(int?)applicationType.adoxio_ProofofZoning,
                PublicCooler = (FormControlState?)(int?)applicationType.adoxio_PublicCoolerSpace,
                ShowAssociatesFormUpload = applicationType.adoxio_IsShowAssociatesFormUpload,
                ShowCurrentProperty = applicationType.adoxio_IsShowCurrentProperty,
                ShowDeclarations = applicationType.adoxio_IsShowDeclarations,
                ShowDescription1 = applicationType.adoxio_ShowDescription1,
                IsShowLGINApproval = applicationType.adoxio_IsShowLGINApproval,
                IsShowLGZoningConfirmation = applicationType.adoxio_isLGZoningConfirmation,
                ShowFinancialIntegrityFormUpload = applicationType.adoxio_IsShowFinancialIntegrityFormUpload,
                ShowPoliceInformationCheckUpload = applicationType.adoxio_IsShowPoliceInformationCheckUploaded,
                ShowHoursOfSale = applicationType.adoxio_IsShowHoursOfSale,
                ShowLiquorDeclarations = applicationType.adoxio_IsShowLiquorDeclarations,
                ShowOwnershipDeclaration = applicationType.adoxio_IsOwnershipConfirmation,
                ShowLgNoObjection = applicationType.adoxio_ShowLGNoObjection,
                ShowLiquorSitePlan = (FormControlState?)(int?)applicationType.adoxio_LiquorSitePlan,
                ShowPatio = applicationType.adoxio_ShowPatioSection,
                HasPatio = applicationType.adoxio_ishaspatio,
                ShowPropertyDetails = applicationType.adoxio_IsShowPropertyDetails,
                ShowSupportingDocuments = applicationType.adoxio_IsShowSupportingDocuments,
                Signage = (FormControlState?)(int?)applicationType.adoxio_Signage,
                SitePhotos = (FormControlState?)(int?)applicationType.adoxio_sitephotographs,
                SitePlan = (FormControlState?)(int?)applicationType.adoxio_SitePlan,
                StoreContactInfo = (FormControlState?)(int?)applicationType.adoxio_StoreContactInfo,
                Title = applicationType.adoxio_TitleText,
                ValidInterest = (FormControlState?)(int?)applicationType.adoxio_ValidInterest,
                RequiresSecurityScreening = applicationType.adoxio_RequiresSecurityScreening,
                IsEndorsement = applicationType.adoxio_IsEndorsement,
                IsRelocation = applicationType.adoxio_IsRelocation,
                IsDefault = applicationType.adoxio_IsDefault,
                IsStructural = applicationType.adoxio_IsStructuralChange,
                ServiceAreas = applicationType.adoxio_ServiceAreas ?? false,
                OutsideAreas = applicationType.adoxio_OutsideAreas ?? false,
                CapacityArea = applicationType.adoxio_CapacityArea ?? false,
                HasALRQuestion = applicationType.adoxio_HasALRQuestion ?? false,
                ShowZoningDeclarations = applicationType.adoxio_ShowZoningDeclarations ?? false,
                IsFree = applicationType.adoxio_isfree == adoxio_applicationtype_adoxio_isfree.Yes,
                DiscretionRequest = applicationType.adoxio_discretionrequest == adoxio_yesnoandreadonly.Yes
            };

            var contentNav = applicationType.adoxio_applicationtype_adoxio_applicationtypecontent_ApplicationType;
            if (contentNav != null)
            {
                result.ContentTypes = new List<ApplicationTypeContent>();
                foreach (var content in contentNav)
                    result.ContentTypes.Add(content.ToViewModel());
            }

            return result;
        }
    }
}
