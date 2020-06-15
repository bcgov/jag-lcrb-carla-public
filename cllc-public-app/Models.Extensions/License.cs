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
        public static List<string> GetEndorsements(string licenceId, IDynamicsClient dynamicsClient)
        {
            List<string> endorsementsList = new List<string>();
            string filter = $"_adoxio_licence_value eq {licenceId}";
            string[] expand = { "adoxio_ApplicationType" };
            MicrosoftDynamicsCRMadoxioEndorsementCollection endorsementsCollection = dynamicsClient.Endorsements.Get(filter: filter, expand: expand);
            if (endorsementsCollection.Value.Count > 0)
            {
                foreach (var item in endorsementsCollection.Value)
                {
                    if (item.AdoxioApplicationType != null) {
                        endorsementsList.Add(item.AdoxioApplicationType.AdoxioName);
                    }
                }
            }

            return endorsementsList;
        }

        public static License ToViewModel(this MicrosoftDynamicsCRMadoxioLicences dynamicsLicense, IDynamicsClient dynamicsClient)
        {
            License adoxioLicenseVM = new License();

            adoxioLicenseVM.id = dynamicsLicense.AdoxioLicencesid;
            if (dynamicsLicense.AdoxioLicencesubcategory != null)
            {
                adoxioLicenseVM.licenseSubCategory = EnumExtensions.GetEnumMemberValue((LicenseSubCategory?)dynamicsLicense.AdoxioLicencesubcategory);
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
                adoxioLicenseVM.establishmentId = establishment.AdoxioEstablishmentid;
                adoxioLicenseVM.establishmentName = establishment.AdoxioName;
                adoxioLicenseVM.establishmentEmail = establishment.AdoxioEmail;
                adoxioLicenseVM.establishmentPhone = establishment.AdoxioPhone;
                adoxioLicenseVM.establishmentAddress = establishment.AdoxioAddressstreet
                                                    + ", " + establishment.AdoxioAddresscity
                                                    + " " + establishment.AdoxioAddresspostalcode;
            }
            adoxioLicenseVM.expiryDate = dynamicsLicense.AdoxioExpirydate;

            // fetch the licence status
            int? adoxio_licenceStatusId = dynamicsLicense.Statuscode;
            if (adoxio_licenceStatusId != null)
            {
                adoxioLicenseVM.licenseStatus = dynamicsLicense.Statuscode.ToString();
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
                    adoxioLicenseVM.licenseType = adoxio_licencetype.AdoxioName;
                }
            }

            // fetch license number
            adoxioLicenseVM.licenseNumber = dynamicsLicense.AdoxioLicencenumber;

            adoxioLicenseVM.establishmentAddressCity = dynamicsLicense.AdoxioEstablishmentaddresscity;
            adoxioLicenseVM.establishmentAddressPostalCode = dynamicsLicense.AdoxioEstablishmentaddresspostalcode;
            adoxioLicenseVM.establishmentAddressStreet = dynamicsLicense.AdoxioEstablishmentaddressstreet;

            if (dynamicsLicense.AdoxioEstablishment != null)
            {
                adoxioLicenseVM.establishmentParcelId = dynamicsLicense.AdoxioEstablishment.AdoxioParcelid;
            }
            
            
            adoxioLicenseVM.endorsements = GetEndorsements(adoxioLicenseVM.id, dynamicsClient);
            

            return adoxioLicenseVM;
        }

        public static ApplicationLicenseSummary ToLicenseSummaryViewModel(this MicrosoftDynamicsCRMadoxioLicences licence, IList<MicrosoftDynamicsCRMadoxioApplication> applications, IDynamicsClient dynamicsClient)
        {
            ApplicationLicenseSummary licenseSummary = new ViewModels.ApplicationLicenseSummary()
            {
                LicenseId = licence.AdoxioLicencesid,
                LicenseNumber = licence.AdoxioLicencenumber,
                LicenseSubCategory = (LicenseSubCategory?)licence.AdoxioLicencesubcategory, // TG added for wine stores
                EstablishmentAddressStreet = licence.AdoxioEstablishmentaddressstreet,
                EstablishmentAddressCity = licence.AdoxioEstablishmentaddresscity,
                EstablishmentAddressPostalCode = licence.AdoxioEstablishmentaddresspostalcode,
                EstablishmentPhoneNumber = licence.AdoxioEstablishmentphone,
                EstablishmentEmail = licence.AdoxioEstablishment?.AdoxioEmail,
                EstablishmentId = licence.AdoxioEstablishment?.AdoxioEstablishmentid,
                ExpiryDate = licence.AdoxioExpirydate,
                Status = StatusUtility.GetLicenceStatus(licence, applications),
                AllowedActions = new List<ApplicationType>(),
                TransferRequested = (TransferRequested?)licence.AdoxioTransferrequested,
                ThirdPartyOperatorAccountId = licence._adoxioThirdpartyoperatoridValue,
                TPORequested = (TPORequested?)licence.AdoxioTporequested // indicate whether a third party operator app has been requested
            };

            if(licence.AdoxioThirdPartyOperatorId != null){
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

    public static class LicenseRepresentativeExtensions
    {
        public static LicenseRepresentative ToViewModel(this MicrosoftDynamicsCRMadoxioLicences dynamicsLicense)
        {
            LicenseRepresentative rep = new LicenseRepresentative()
            {
                FullName = dynamicsLicense.AdoxioRepresentativename,
                PhoneNumber = dynamicsLicense.AdoxioRepresentativephone,
                Email = dynamicsLicense.AdoxioRepresentativeemail,
                CanSubmitPermanentChangeApplications = dynamicsLicense.AdoxioCansubmitpermanentchangeapplications,
                CanSignTemporaryChangeApplications = dynamicsLicense.AdoxioCansigntemporarychangeapplications,
                CanObtainLicenceInformation = dynamicsLicense.AdoxioCanobtainlicenceinformation,
                CanSignGroceryStoreProofOfSale = dynamicsLicense.AdoxioCansigntemporarychangeapplications,
                CanAttendEducationSessions = dynamicsLicense.AdoxioCanattendeducationsessions,
                CanAttendComplianceMeetings = dynamicsLicense.AdoxioCanattendcompliancemeetings,
                CanRepresentAtHearings = dynamicsLicense.AdoxioCanrepresentathearings
            };

            return rep;
        }
    }
}
