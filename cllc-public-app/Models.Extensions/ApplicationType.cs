using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class ApplicationTypeExtensions
    {

        public static ApplicationType ToViewModel(this MicrosoftDynamicsCRMadoxioApplicationtype applicationType)
        {
            ApplicationType result = null;
            if (applicationType != null)
            {
                result = new ApplicationType()
                {
                    Id = applicationType.AdoxioApplicationtypeid,

                    ActionText = applicationType.AdoxioActiontext,

                    Name = applicationType.AdoxioName,
                    Title = applicationType.AdoxioTitletext,
                    ShowPropertyDetails = applicationType.AdoxioIsshowpropertydetails,
                    EstablishmentName = (FormControlState?)applicationType.AdoxioEstablishmentname,
                    EstablishmetNameIsReadOnly = applicationType.AdoxioIslockestablishmentname,
                    CurrentEstablishmentAddress = (FormControlState?)applicationType.AdoxioCurrentestablishmentaddress,
                    ShowCurrentProperty = applicationType.AdoxioIsshowcurrentproperty,
                    newEstablishmentAddress = (FormControlState?)applicationType.AdoxioNewestablishmentaddress,
                    StoreContactInfo = (FormControlState?)applicationType.AdoxioStorecontactinfo,
                    ShowDescription1 = applicationType.AdoxioShowdescription1,
                    Signage = (FormControlState?)applicationType.AdoxioSignage,
                    ValidInterest = (FormControlState?)applicationType.AdoxioValidinterest,
                    FloorPlan = (FormControlState?)applicationType.AdoxioFloorplan,
                    SitePlan = (FormControlState?)applicationType.AdoxioSiteplan,                    
                    ShowHoursOfSale = applicationType.AdoxioIsshowhoursofsale,
                    ShowAssociatesFormUpload = applicationType.AdoxioIsshowassociatesformupload,
                    ShowFinancialIntegrityFormUpload = applicationType.AdoxioIsshowfinancialintegrityformupload, 
                    ShowSupportingDocuments = applicationType.AdoxioIsshowsupportingdocuments,
                    ShowDeclarations = applicationType.AdoxioIsshowdeclarations
            
                };

                if (applicationType.AdoxioApplicationtypeAdoxioApplicationtypecontentApplicationType != null)
                {
                    result.contentTypes = new List<ApplicationTypeContent>();
                    foreach (var content in applicationType.AdoxioApplicationtypeAdoxioApplicationtypecontentApplicationType)
                    {
                        result.contentTypes.Add(content.ToViewModel());
                    }
                }
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
