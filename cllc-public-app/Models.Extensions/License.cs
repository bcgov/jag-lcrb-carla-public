using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Public.Extensions;
using Microsoft.Rest;
using Serilog;

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
            string filter = $"_adoxio_licence_value eq {licenceId} and statuscode ne {(int)EndorsementStatus.Cancelled}";
            string[] expand = { "adoxio_ApplicationType" };
            try
            {
                MicrosoftDynamicsCRMadoxioEndorsementCollection endorsementsCollection = dynamicsClient.Endorsements.Get(filter: filter, expand: expand);
                if (endorsementsCollection.Value.Count > 0)
                {
                    foreach (var item in endorsementsCollection.Value)
                    {
                        if (item.AdoxioApplicationType != null)
                        {
                            var endorsement = new ViewModels.Endorsement
                            {
                                ApplicationTypeId = item.AdoxioApplicationType.AdoxioApplicationtypeid,
                                ApplicationTypeName = item.AdoxioApplicationType.AdoxioName,
                                EndorsementId = item.AdoxioEndorsementid,
                                EndorsementName = item.AdoxioName,
                                HoursOfServiceList = GetHoursOfServiceList(item.AdoxioEndorsementAdoxioHoursofserviceEndorsement, item.AdoxioEndorsementid, dynamicsClient),
                                AreaCapacity = GetAreaCapacity(item.AdoxioEndorsementid, dynamicsClient)

                            };
                            endorsementsList.Add(endorsement);
                        }

                    }
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                Log.Logger.Error(httpOperationException, $"Error getting endorsements for licence {licenceId}");
            }

            return endorsementsList;
        }

        private static List<HoursOfService> GetHoursOfServiceList(IList<MicrosoftDynamicsCRMadoxioHoursofservice> adoxioEndorsementAdoxioHoursofserviceEndorsement,
            string endorsementId, IDynamicsClient dynamicsClient)
        {
            var hoursOfServiceList = new List<HoursOfService>();
            var hours = dynamicsClient.Hoursofservices.Get(filter: $"_adoxio_endorsement_value eq {endorsementId}");
            if (hours.Value.Count > 0)
            {
                var hoursVal = hours.Value.FirstOrDefault();
                hoursOfServiceList.Add(GetHourService(0, hoursVal.AdoxioSundayopen, hoursVal.AdoxioSundayclose));
                hoursOfServiceList.Add(GetHourService(1, hoursVal.AdoxioMondayopen, hoursVal.AdoxioMondayclose));
                hoursOfServiceList.Add(GetHourService(2, hoursVal.AdoxioTuesdayopen, hoursVal.AdoxioTuesdayclose));
                hoursOfServiceList.Add(GetHourService(3, hoursVal.AdoxioWednesdayopen, hoursVal.AdoxioWednesdayclose));
                hoursOfServiceList.Add(GetHourService(4, hoursVal.AdoxioThursdayopen, hoursVal.AdoxioThursdayclose));
                hoursOfServiceList.Add(GetHourService(5, hoursVal.AdoxioFridayopen, hoursVal.AdoxioFridayclose));
                hoursOfServiceList.Add(GetHourService(6, hoursVal.AdoxioSaturdayopen, hoursVal.AdoxioSaturdayclose));

            }
            return hoursOfServiceList;
        }
        private static int GetAreaCapacity(string endorsementId, IDynamicsClient dynamicsClient)
        {
            var capacity = 0;
            var allServiceAreas = dynamicsClient.Serviceareas.Get(filter: $"_adoxio_endorsement_value eq {endorsementId}");
            if (allServiceAreas.Value.Count > 0)
            {
                foreach (var serviceArea in allServiceAreas.Value)
                {
                    capacity += serviceArea.AdoxioCapacity.HasValue ? serviceArea.AdoxioCapacity.Value : 0;
                }
            }
            return capacity;
        }
        private static HoursOfService GetHourService(int dayOfWeek, int? open, int? close)
        {
            var opening = StoreHoursUtility.ConvertOpenHoursToString(open);
            var openingList = opening.Split(':');
            var closing = StoreHoursUtility.ConvertOpenHoursToString(close);
            var closingList = closing.Split(':');
            return new HoursOfService
            {
                DayOfWeek = dayOfWeek,
                StartTimeHour = int.Parse(openingList[0]),
                StartTimeMinute = int.Parse(openingList[1]),
                EndTimeHour = int.Parse(closingList[0]),
                EndTimeMinute = int.Parse(closingList[1]),

            };
        }
        public static List<OffsiteStorage> GetOffsiteStorage(string licenceId, IDynamicsClient dynamicsClient)
        {
            string filter = $"_adoxio_licenceid_value eq {licenceId}";
            var entities = dynamicsClient.Offsitestorages.Get(filter: filter).Value;
            var offsiteList = entities.Select(item => item.ToViewModel()).ToList();
            return offsiteList;
        }

        public static List<CapacityArea> GetServiceAreas(string licenceId, IDynamicsClient dynamicsClient)
        {
            try
            {
                string filter = $"_adoxio_licenceid_value eq {licenceId}";
                var entities = dynamicsClient.Serviceareas.Get(filter: filter).Value;
                var serviceAreas = entities.Select(item => item.ToViewModel()).ToList();
                return serviceAreas;
            }
            catch (Exception)
            {
                return new List<CapacityArea>();
            }
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
            adoxioLicenseVM.OffsiteStorageLocations = GetOffsiteStorage(adoxioLicenseVM.Id, dynamicsClient);
            adoxioLicenseVM.ServiceAreas = GetServiceAreas(adoxioLicenseVM.Id, dynamicsClient);

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
            bool missingLicenceFee = applications.Where(app =>
                    app._adoxioLicencefeeinvoiceValue != null
                    && app?.AdoxioApplicationTypeId?.AdoxioIsdefault == true
                    && app?.AdoxioLicenceFeeInvoice?.Statuscode != (int)Adoxio_invoicestatuses.Paid
                ).Any();

            ApplicationLicenseSummary licenseSummary = new ApplicationLicenseSummary
            {
                LicenseId = licence.AdoxioLicencesid,
                LicenseNumber = licence.AdoxioLicencenumber,
                LicenceTypeCategory = (LicenceTypeCategory)licence?.AdoxioLicenceType?.AdoxioCategory,
                MissingFirstYearLicenceFee = missingLicenceFee,
                CurrentOwner = licence?.AdoxioLicencee?.Name,
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
                RepresentativeCanRepresentAtHearings = licence.AdoxioCanrepresentathearings,
                AutoRenewal = (licence.AdoxioAutorenewal == true)
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

            if (licence.AdoxioThirdPartyOperatorId != null)
            {
                licenseSummary.ThirdPartyOperatorAccountName = licence.AdoxioThirdPartyOperatorId.Name;
            }

            if (licence.AdoxioEstablishment != null)
            {
                licenseSummary.EstablishmentName = licence.AdoxioEstablishment.AdoxioName;
                licenseSummary.EstablishmentIsOpen = licence.AdoxioEstablishment.AdoxioIsopen;
            }

            var mainApplication = applications.Where(app => app.Statuscode == (int)AdoxioApplicationStatusCodes.Approved).FirstOrDefault();

            if (mainApplication != null)
            {
                licenseSummary.ApplicationId = mainApplication.AdoxioApplicationid;
                if (mainApplication.AdoxioApplicationTypeId != null)
                {
                    licenseSummary.ApplicationTypeName = mainApplication.AdoxioApplicationTypeId.AdoxioName;
                    licenseSummary.ApplicationTypeCategory = (ApplicationTypeCategory?)mainApplication.AdoxioApplicationTypeId.AdoxioCategory;
                }
            }

            if (licence.AdoxioLicenceType != null)
            {
                licenseSummary.LicenceTypeName = licence.AdoxioLicenceType.AdoxioName;
                licenseSummary.LicenceTypeCategory = (LicenceTypeCategory?)licence.AdoxioLicenceType.AdoxioCategory;
            }

            licenseSummary.Endorsements = GetEndorsements(licenseSummary.LicenseId, dynamicsClient);

            if (licence?.AdoxioLicenceType?.AdoxioLicencetypesApplicationtypes != null)
            {
                //Filter out application types that are not ACTIVE
                const int ACTIVE = 0;
                var applicationTypes = licence.AdoxioLicenceType.AdoxioLicencetypesApplicationtypes
                                        .Where(appType => appType.Statecode == ACTIVE)
                                        .ToList()
                                        .OrderBy(appType => appType.AdoxioActiontext);

                foreach (var item in applicationTypes)
                {
                    // we don't want to allow you to apply for an endorsement you already have...
                    bool isEndorsementThatIsProcessed = (
                        item.AdoxioIsendorsement != null &&
                        item.AdoxioIsendorsement == true &&
                        licenseSummary.Endorsements.Any(e => e.ApplicationTypeId == item.AdoxioApplicationtypeid)
                    );
                    if (!isEndorsementThatIsProcessed)
                    {
                        licenseSummary.AllowedActions.Add(item.ToViewModel());
                    }
                }
            }

            licenseSummary.OffsiteStorageLocations = GetOffsiteStorage(licenseSummary.LicenseId, dynamicsClient);
            licenseSummary.ServiceAreas = GetServiceAreas(licenseSummary.LicenseId, dynamicsClient);

            return licenseSummary;
        }
    }
}
