using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Public.Extensions;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class LicenseExtensions
    {
        public static List<ViewModels.Endorsement> GetEndorsements(string licenceId, IDynamicsClient dynamicsClient)
        {
            var endorsementsList = new List<ViewModels.Endorsement>();
            string filter = $"_adoxio_licence_value eq {licenceId}";
            string[] expand = { "adoxio_ApplicationType" };
            MicrosoftDynamicsCRMadoxioEndorsementCollection endorsementsCollection = dynamicsClient.Endorsements.Get(filter: filter, expand: expand);
            if (endorsementsCollection.Value.Count > 0)
            {
                foreach (var item in endorsementsCollection.Value)
                {
                    if (item.AdoxioApplicationType != null) {
                        var endorsement = new ViewModels.Endorsement()
                        {
                            ApplicationTypeId = item.AdoxioApplicationType.AdoxioApplicationtypeid,
                            ApplicationTypeName = item.AdoxioApplicationType.AdoxioName,
                            EndorsementId = item.AdoxioEndorsementid,
                            EndorsementName = item.AdoxioName
                        };
                        endorsementsList.Add(endorsement);
                    }
                }
            }

            return endorsementsList;
        }

        public static License ToViewModel(this MicrosoftDynamicsCRMadoxioLicences dynamicsLicense, IDynamicsClient dynamicsClient)
        {
            License adoxioLicenseVM = new License();

            adoxioLicenseVM.Id = dynamicsLicense.AdoxioLicencesid;
            
            if (dynamicsLicense._adoxioLicencesubcategoryidValue != null)
            {
                try
                {
                    var adoxioLicencesubcategory = dynamicsClient.Licencesubcategories.GetByKey(dynamicsLicense._adoxioLicencesubcategoryidValue);
                    adoxioLicenseVM.LicenseSubCategory = adoxioLicencesubcategory.AdoxioName;
                }
                catch (Exception e)
                {
                    adoxioLicenseVM.LicenseSubCategory = null;
                }
            }
            
            // fetch the establishment and get name and address
            Guid? adoxioEstablishmentId = null;
            if (!string.IsNullOrEmpty(dynamicsLicense._adoxioEstablishmentValue))
            {
                adoxioEstablishmentId = Guid.Parse(dynamicsLicense._adoxioEstablishmentValue);
            }
            if (adoxioEstablishmentId != null)
            {
                var establishment = dynamicsClient.Establishments.GetByKey(adoxioEstablishmentId.ToString());
                adoxioLicenseVM.EstablishmentId = establishment.AdoxioEstablishmentid;
                adoxioLicenseVM.EstablishmentName = establishment.AdoxioName;
                adoxioLicenseVM.EstablishmentEmail = establishment.AdoxioEmail;
                adoxioLicenseVM.EstablishmentPhone = establishment.AdoxioPhone;
                adoxioLicenseVM.EstablishmentAddress = establishment.AdoxioAddressstreet
                                                    + ", " + establishment.AdoxioAddresscity
                                                    + " " + establishment.AdoxioAddresspostalcode;
            }
            adoxioLicenseVM.ExpiryDate = dynamicsLicense.AdoxioExpirydate;

            // fetch the licence status
            int? adoxio_licenceStatusId = dynamicsLicense.Statuscode;
            if (adoxio_licenceStatusId != null)
            {
                adoxioLicenseVM.LicenseStatus = dynamicsLicense.Statuscode.ToString();
            }

            // fetch the licence type
            Guid? adoxio_licencetypeId = null;
            if (!string.IsNullOrEmpty(dynamicsLicense._adoxioLicencetypeValue))
            {
                adoxio_licencetypeId = Guid.Parse(dynamicsLicense._adoxioLicencetypeValue);
            }
            if (adoxio_licencetypeId != null)
            {
                var adoxio_licencetype = dynamicsClient.Licencetypes.GetByKey(adoxio_licencetypeId.ToString());
                if (adoxio_licencetype != null)
                {
                    adoxioLicenseVM.LicenseType = adoxio_licencetype.AdoxioName;
                }
            }

            // fetch license number
            adoxioLicenseVM.LicenseNumber = dynamicsLicense.AdoxioLicencenumber;

            adoxioLicenseVM.EstablishmentAddressCity = dynamicsLicense.AdoxioEstablishmentaddresscity;
            adoxioLicenseVM.EstablishmentAddressPostalCode = dynamicsLicense.AdoxioEstablishmentaddresspostalcode;
            adoxioLicenseVM.EstablishmentAddressStreet = dynamicsLicense.AdoxioEstablishmentaddressstreet;

            if (dynamicsLicense.AdoxioEstablishment != null)
            {
                adoxioLicenseVM.EstablishmentParcelId = dynamicsLicense.AdoxioEstablishment.AdoxioParcelid;
            }
            
            
            adoxioLicenseVM.Endorsements = GetEndorsements(adoxioLicenseVM.Id, dynamicsClient);

            adoxioLicenseVM.RepresentativeFullName = dynamicsLicense.AdoxioRepresentativename;
            adoxioLicenseVM.RepresentativeEmail = dynamicsLicense.AdoxioRepresentativeemail;
            adoxioLicenseVM.RepresentativePhoneNumber = dynamicsLicense.AdoxioRepresentativephone;
            adoxioLicenseVM.RepresentativeCanSubmitPermanentChangeApplications = dynamicsLicense.AdoxioCansubmitpermanentchangeapplications;
            adoxioLicenseVM.RepresentativeCanSignTemporaryChangeApplications = dynamicsLicense.AdoxioCansigntemporarychangeapplications;
            adoxioLicenseVM.RepresentativeCanObtainLicenceInformation = dynamicsLicense.AdoxioCanobtainlicenceinformation;
            adoxioLicenseVM.RepresentativeCanSignGroceryStoreProofOfSale = dynamicsLicense.AdoxioCansigngrocerystoreproofofsales;
            adoxioLicenseVM.RepresentativeCanAttendEducationSessions = dynamicsLicense.AdoxioCanattendeducationsessions;
            adoxioLicenseVM.RepresentativeCanAttendComplianceMeetings = dynamicsLicense.AdoxioCanattendcompliancemeetings;
            adoxioLicenseVM.RepresentativeCanRepresentAtHearings = dynamicsLicense.AdoxioCanrepresentathearings;

            return adoxioLicenseVM;
        }

        public static ApplicationLicenseSummary ToLicenseSummaryViewModel(this MicrosoftDynamicsCRMadoxioLicences licence, IList<MicrosoftDynamicsCRMadoxioApplication> applications, IDynamicsClient dynamicsClient)
        {
            ApplicationLicenseSummary licenseSummary = new ViewModels.ApplicationLicenseSummary()
            {
                LicenseId = licence.AdoxioLicencesid,
                LicenseNumber = licence.AdoxioLicencenumber,
                EstablishmentAddressStreet = licence.AdoxioEstablishmentaddressstreet,
                EstablishmentAddressCity = licence.AdoxioEstablishmentaddresscity,
                EstablishmentAddressPostalCode = licence.AdoxioEstablishmentaddresspostalcode,
                EstablishmentPhoneNumber = licence.AdoxioEstablishmentphone,
                EstablishmentEmail = licence.AdoxioEstablishment?.AdoxioEmail,
                EstablishmentId = licence.AdoxioEstablishment?.AdoxioEstablishmentid,
                ExpiryDate = licence.AdoxioExpirydate,
                Status = StatusUtility.GetLicenceStatus(licence, applications),
                AllowedActions = new List<ApplicationType>(),
                TransferRequested = licence.AdoxioTransferrequested.HasValue && (EnumYesNo?)licence.AdoxioTransferrequested == EnumYesNo.Yes,
                Dormant = licence.AdoxioDormant.HasValue && (EnumYesNo?)licence.AdoxioDormant == EnumYesNo.Yes,
                Suspended = licence.AdoxioSuspended.HasValue && (EnumYesNo?)licence.AdoxioSuspended == EnumYesNo.Yes,
                Operated = licence.AdoxioOperated.HasValue && (EnumYesNo?)licence.AdoxioOperated == EnumYesNo.Yes,
                ThirdPartyOperatorAccountId = licence._adoxioThirdpartyoperatoridValue,
                TPORequested = licence.AdoxioTporequested.HasValue && (EnumYesNo?)licence.AdoxioTporequested == EnumYesNo.Yes, // indicate whether a third party operator app has been requested
                RepresentativeFullName = licence.AdoxioRepresentativename,
                RepresentativeEmail = licence.AdoxioRepresentativeemail,
                RepresentativePhoneNumber = licence.AdoxioRepresentativephone,
                RepresentativeCanSubmitPermanentChangeApplications = licence.AdoxioCansubmitpermanentchangeapplications,
                RepresentativeCanSignTemporaryChangeApplications = licence.AdoxioCansigntemporarychangeapplications,
                RepresentativeCanObtainLicenceInformation = licence.AdoxioCanobtainlicenceinformation,
                RepresentativeCanSignGroceryStoreProofOfSale = licence.AdoxioCansigngrocerystoreproofofsales,
                RepresentativeCanAttendEducationSessions = licence.AdoxioCanattendeducationsessions,
                RepresentativeCanAttendComplianceMeetings = licence.AdoxioCanattendcompliancemeetings,
                RepresentativeCanRepresentAtHearings = licence.AdoxioCanrepresentathearings
            };

            if (licence._adoxioLicencesubcategoryidValue != null)
            {
                try
                {
                    var adoxioLicencesubcategory = dynamicsClient.Licencesubcategories.GetByKey(licence._adoxioLicencesubcategoryidValue);
                    licenseSummary.LicenseSubCategory = adoxioLicencesubcategory.AdoxioName;
                }
                catch (Exception e)
                {
                    licenseSummary.LicenseSubCategory = null;
                }
            }

            if (licence.AdoxioThirdPartyOperatorId != null){
                licenseSummary.ThirdPartyOperatorAccountName = licence.AdoxioThirdPartyOperatorId.Name;
            }

            if (licence.AdoxioEstablishment != null)
            {
                licenseSummary.EstablishmentName = licence.AdoxioEstablishment.AdoxioName;
                licenseSummary.EstablishmentIsOpen = licence.AdoxioEstablishment.AdoxioIsopen;
            }

            var mainApplication = applications.Where(app => app.Statuscode == (int)Public.ViewModels.AdoxioApplicationStatusCodes.Approved).FirstOrDefault();

            // 2-13-2020 adjust criteria so that we get the first approved CRS rather than simply the first CRS, to ensure that we get the correct record.
            var crsApplication = applications.Where(app => app.Statuscode == (int)Public.ViewModels.AdoxioApplicationStatusCodes.Approved && app.AdoxioApplicationTypeId.AdoxioName == "Cannabis Retail Store").FirstOrDefault();
            if (mainApplication != null)
            {
                licenseSummary.ApplicationId = mainApplication.AdoxioApplicationid;
                if (mainApplication.AdoxioApplicationTypeId != null)
                {
                    licenseSummary.ApplicationTypeName = mainApplication.AdoxioApplicationTypeId.AdoxioName;
                    licenseSummary.ApplicationTypeCategory = (ApplicationTypeCategory?)mainApplication.AdoxioApplicationTypeId.AdoxioCategory;
                }
            }
            if (crsApplication != null)
            {
                // 2-7-2020 set the criteria to be No Contraventions rather than Pass.  LCSD-2524
                licenseSummary.StoreInspected = crsApplication.AdoxioAppchecklistinspectionresults == (int)InspectionStatus.NoContraventions;
            }

            if (licence.AdoxioLicenceType != null)
            {
                licenseSummary.LicenceTypeName = licence.AdoxioLicenceType.AdoxioName;
                licenseSummary.LicenceTypeCategory = (LicenceTypeCategory?)licence.AdoxioLicenceType.AdoxioCategory;
            }

            if (licence != null &&
                licence.AdoxioLicenceType != null &&
                licence.AdoxioLicenceType.AdoxioLicencetypesApplicationtypes != null)
            {
                foreach (var item in licence.AdoxioLicenceType.AdoxioLicencetypesApplicationtypes)
                {
                    // check to see if there is an existing action on this licence.
                    licenseSummary.AllowedActions.Add(item.ToViewModel());
                }

            }

            licenseSummary.Endorsements = GetEndorsements(licenseSummary.LicenseId, dynamicsClient);
            
            return licenseSummary;
        }
    }
}
