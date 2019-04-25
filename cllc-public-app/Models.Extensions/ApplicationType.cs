using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class ApplicationTypeExtensions
    {        

        public static ApplicationType ToViewModel(this MicrosoftDynamicsCRMadoxioApplicationtype dynamicsApplicationType)
        {
            ApplicationType result = null;
            if (dynamicsApplicationType != null)
            {
                result = new ApplicationType()
                {
                    Id = dynamicsApplicationType.AdoxioApplicationtypeid,
                    
                    ActionText = dynamicsApplicationType.AdoxioActiontext,
                    Name = dynamicsApplicationType.AdoxioName,
                    Title = dynamicsApplicationType.AdoxioTitletext,
                    Preamble = dynamicsApplicationType.AdoxioPreamble,
                    BeforeStarting = dynamicsApplicationType.AdoxioBeforestarting,
                    NextSteps = dynamicsApplicationType.AdoxioNextsteps,
                    ShowAssociatesFormUpload = dynamicsApplicationType.AdoxioIsshowassociatesformupload,
                    ShowCurrentProperty = dynamicsApplicationType.AdoxioIsshowcurrentproperty,
                    ShowDeclarations = dynamicsApplicationType.AdoxioIsshowdeclarations,
                    ShowFinancialIntegrityFormUpload = dynamicsApplicationType.AdoxioIsshowfinancialintegrityformupload,
                    ShowHoursOfSale = dynamicsApplicationType.AdoxioIsshowhoursofsale,
                    ShowPropertyDetails = dynamicsApplicationType.AdoxioIsshowpropertydetails,
                    ShowSupportingDocuments = dynamicsApplicationType.AdoxioIsshowsupportingdocuments
                };

                /*
                if (dynamicsApplicationType.AdoxioLicenceTypeId != null)
                {
                    result.LicenseType = dynamicsApplicationType.AdoxioLicenceTypeId.ToViewModel();
                }
                */
            }
            

            return result;
        }
    }
}
