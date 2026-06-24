extern alias DV;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Extensions;
using Serilog;
using Microsoft.Extensions.Caching.Memory;
using DV::Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class LicenseExtensions
    {

        private static HoursOfService GetHourService(int dayOfWeek, int? open, int? close)
        {
            const int dayClosedValue = 845280096;
            if (open == null || close == null || open == dayClosedValue || close == dayClosedValue)
            {
                return new HoursOfService
                {
                    DayOfWeek = dayOfWeek,
                    StartTimeHour = null,
                    StartTimeMinute = null,
                    EndTimeHour = null,
                    EndTimeMinute = null
                };
            }
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

        // -----------------------------------------------------------------------
        // Dataverse SDK async overloads (adoxio_licences)
        // -----------------------------------------------------------------------

        public static async Task<List<ViewModels.Endorsement>> GetEndorsementsAsync(string licenceId, IDataverseClient dataverse)
        {
            var endorsementsList = new List<ViewModels.Endorsement>();
            try
            {
                var endorsements = await dataverse.GetEndorsementsByLicenceIdAsync(licenceId);
                foreach (var item in endorsements)
                {
                    var endorsementId = item.adoxio_endorsementId?.ToString();
                    var hoursOfServiceList = await GetHoursOfServiceListAsync(endorsementId, dataverse);
                    var areaCapacity = await GetAreaCapacityAsync(endorsementId, dataverse);
                    var endorsement = new ViewModels.Endorsement
                    {
                        ApplicationTypeId = item.adoxio_ApplicationType?.Id.ToString(),
                        ApplicationTypeName = item.adoxio_ApplicationType?.Name,
                        EndorsementId = endorsementId,
                        EndorsementName = item.adoxio_name,
                        HoursOfServiceList = hoursOfServiceList,
                        AreaCapacity = areaCapacity
                    };
                    endorsementsList.Add(endorsement);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Error getting endorsements for licence {licenceId}");
            }
            return endorsementsList;
        }

        private static async Task<List<HoursOfService>> GetHoursOfServiceListAsync(string endorsementId, IDataverseClient dataverse)
        {
            var list = new List<HoursOfService>();
            if (string.IsNullOrEmpty(endorsementId)) return list;
            var hours = await dataverse.GetHoursOfSaleByEndorsementIdAsync(endorsementId);
            if (hours.Count > 0)
            {
                var h = hours.First();
                list.Add(GetHourService(0, (int?)h.adoxio_SundayOpen, (int?)h.adoxio_SundayClose));
                list.Add(GetHourService(1, (int?)h.adoxio_MondayOpen, (int?)h.adoxio_MondayClose));
                list.Add(GetHourService(2, (int?)h.adoxio_TuesdayOpen, (int?)h.adoxio_TuesdayClose));
                list.Add(GetHourService(3, (int?)h.adoxio_WednesdayOpen, (int?)h.adoxio_WednesdayClose));
                list.Add(GetHourService(4, (int?)h.adoxio_ThursdayOpen, (int?)h.adoxio_ThursdayClose));
                list.Add(GetHourService(5, (int?)h.adoxio_FridayOpen, (int?)h.adoxio_FridayClose));
                list.Add(GetHourService(6, (int?)h.adoxio_SaturdayOpen, (int?)h.adoxio_SaturdayClose));
            }
            return list;
        }

        private static async Task<int> GetAreaCapacityAsync(string endorsementId, IDataverseClient dataverse)
        {
            if (string.IsNullOrEmpty(endorsementId)) return 0;
            var areas = await dataverse.GetServiceAreasByEndorsementIdAsync(endorsementId);
            return areas.Sum(a => a.adoxio_capacity ?? 0);
        }

        public static async Task<List<OffsiteStorage>> GetOffsiteStorageAsync(string licenceId, IDataverseClient dataverse)
        {
            var items = await dataverse.GetOffSiteStorageByLicenceIdAsync(licenceId);
            return items.Select(item => item.ToViewModel()).ToList();
        }

        public static async Task<List<CapacityArea>> GetServiceAreasAsync(string licenceId, IDataverseClient dataverse)
        {
            try
            {
                var items = await dataverse.GetServiceAreasByLicenceIdAsync(licenceId);
                return items.Select(item => item.ToViewModel()).ToList();
            }
            catch (Exception)
            {
                return new List<CapacityArea>();
            }
        }

        public static async Task<License> ToViewModelAsync(this adoxio_licences licence, IDataverseClient dataverse)
        {
            var vm = new License
            {
                Id = licence.Id.ToString(),
                LicenseNumber = licence.adoxio_LicenceNumber,
                ExpiryDate = licence.adoxio_ExpiryDate.HasValue ? (DateTimeOffset?)licence.adoxio_ExpiryDate.Value : null,
                EstablishmentAddressCity = licence.adoxio_EstablishmentAddressCity,
                EstablishmentAddressPostalCode = licence.adoxio_EstablishmentAddressPostalCode,
                EstablishmentAddressStreet = licence.adoxio_EstablishmentAddressStreet,
                RepresentativeFullName = licence.adoxio_RepresentativeName,
                RepresentativeEmail = licence.adoxio_RepresentativeEmail,
                RepresentativePhoneNumber = licence.adoxio_RepresentativePhone,
                RepresentativeCanSubmitPermanentChangeApplications = licence.adoxio_CanSubmitPermanentChangeApplications,
                RepresentativeCanSignTemporaryChangeApplications = licence.adoxio_CanSignTemporaryChangeApplications,
                RepresentativeCanObtainLicenceInformation = licence.adoxio_CanObtainLicenceInformation,
                RepresentativeCanSignGroceryStoreProofOfSale = licence.adoxio_CanSignGroceryStoreProofofSales,
                RepresentativeCanAttendEducationSessions = licence.adoxio_CanAttendEducationSessions,
                RepresentativeCanAttendComplianceMeetings = licence.adoxio_CanAttendComplianceMeetings,
                RepresentativeCanRepresentAtHearings = licence.adoxio_CanRepresentatHearings
            };

            if (licence.statuscode != null)
                vm.LicenseStatus = ((LicenceStatusCodes)(int)licence.statuscode).ToString();

            if (licence.adoxio_LicenceType != null)
            {
                var licenceType = await dataverse.GetLicenceTypeByIdAsync(licence.adoxio_LicenceType.Id.ToString());
                vm.LicenseType = licenceType?.adoxio_name;
            }

            if (licence.adoxio_LicenceSubCategoryId != null)
            {
                var subCat = await dataverse.GetLicenceSubCategoryByIdAsync(licence.adoxio_LicenceSubCategoryId.Id.ToString());
                vm.LicenseSubCategory = subCat?.adoxio_name;
            }

            if (licence.adoxio_establishment != null)
            {
                var est = await dataverse.GetEstablishmentByIdAsync(licence.adoxio_establishment.Id.ToString());
                if (est != null)
                {
                    vm.EstablishmentId = est.Id.ToString();
                    vm.EstablishmentName = est.adoxio_name;
                    vm.EstablishmentEmail = est.adoxio_Email;
                    vm.EstablishmentPhone = est.adoxio_Phone;
                    vm.EstablishmentAddress = $"{est.adoxio_AddressStreet}, {est.adoxio_AddressCity} {est.adoxio_AddressPostalCode}";
                    vm.EstablishmentParcelId = est.adoxio_ParcelID;
                }
            }

            vm.Endorsements = await GetEndorsementsAsync(vm.Id, dataverse);
            vm.OffsiteStorageLocations = await GetOffsiteStorageAsync(vm.Id, dataverse);
            vm.ServiceAreas = await GetServiceAreasAsync(vm.Id, dataverse);

            return vm;
        }

        public static async Task<ApplicationLicenseSummary> ToLicenseSummaryViewModelAsync(
            this adoxio_licences licence,
            IList<adoxio_application> applications,
            IDataverseClient dataverse,
            IMemoryCache cache = null)
        {
            bool missingLicenceFee = applications.Any(app =>
                app.adoxio_LicenceFeeInvoice != null
                && app.adoxio_LicenceFeeInvoicePaid != true);

            var licenceId = licence.Id.ToString();

            var licenseSummary = new ApplicationLicenseSummary
            {
                LicenseId = licenceId,
                LicenseNumber = licence.adoxio_LicenceNumber,
                MissingFirstYearLicenceFee = missingLicenceFee,
                CurrentOwner = licence.adoxio_Licencee?.Name,
                EstablishmentAddressStreet = licence.adoxio_EstablishmentAddressStreet,
                EstablishmentAddressCity = licence.adoxio_EstablishmentAddressCity,
                EstablishmentAddressPostalCode = licence.adoxio_EstablishmentAddressPostalCode,
                EstablishmentPhoneNumber = licence.adoxio_EstablishmentPhone,
                ExpiryDate = licence.adoxio_ExpiryDate.HasValue ? (DateTimeOffset?)licence.adoxio_ExpiryDate.Value : null,
                Status = StatusUtility.GetLicenceStatus(licence, applications),
                AllowedActions = new List<ApplicationType>(),
                TransferRequested = licence.adoxio_TransferRequested == adoxio_licences_adoxio_transferrequested.Yes,
                Dormant = licence.adoxio_Dormant == adoxio_licences_adoxio_dormant.Yes,
                Suspended = licence.adoxio_Suspended == adoxio_licences_adoxio_suspended.Yes,
                Operated = licence.adoxio_Operated == adoxio_licences_adoxio_operated.Yes,
                ThirdPartyOperatorAccountId = licence.adoxio_ThirdPartyOperatorId?.Id.ToString(),
                TPORequested = licence.adoxio_TPORequested == adoxio_licences_adoxio_tporequested.Yes,
                RepresentativeFullName = licence.adoxio_RepresentativeName,
                RepresentativeEmail = licence.adoxio_RepresentativeEmail,
                RepresentativePhoneNumber = licence.adoxio_RepresentativePhone,
                RepresentativeCanSubmitPermanentChangeApplications = licence.adoxio_CanSubmitPermanentChangeApplications,
                RepresentativeCanSignTemporaryChangeApplications = licence.adoxio_CanSignTemporaryChangeApplications,
                RepresentativeCanObtainLicenceInformation = licence.adoxio_CanObtainLicenceInformation,
                RepresentativeCanSignGroceryStoreProofOfSale = licence.adoxio_CanSignGroceryStoreProofofSales,
                RepresentativeCanAttendEducationSessions = licence.adoxio_CanAttendEducationSessions,
                RepresentativeCanAttendComplianceMeetings = licence.adoxio_CanAttendComplianceMeetings,
                RepresentativeCanRepresentAtHearings = licence.adoxio_CanRepresentatHearings,
                TemporaryRelocationStatus = (int?)licence.adoxio_TRLstatus,
                AutoRenewal = licence.adoxio_AutoRenewal == true
            };

            if (licence.adoxio_ThirdPartyOperatorId != null)
                licenseSummary.ThirdPartyOperatorAccountName = licence.adoxio_ThirdPartyOperatorId.Name;

            if (licence.adoxio_establishment != null)
            {
                var est = await dataverse.GetEstablishmentByIdAsync(licence.adoxio_establishment.Id.ToString());
                if (est != null)
                {
                    licenseSummary.EstablishmentName = est.adoxio_name;
                    licenseSummary.EstablishmentIsOpen = est.adoxio_IsOpen;
                    licenseSummary.EstablishmentId = est.Id.ToString();
                    licenseSummary.EstablishmentEmail = est.adoxio_Email;
                }
            }

            if (licence.adoxio_LicenceSubCategoryId != null)
            {
                var subCat = await GetCachedLicenceSubCategoryAsync(licence.adoxio_LicenceSubCategoryId.Id.ToString(), dataverse, cache);
                licenseSummary.LicenseSubCategory = subCat?.adoxio_name;
            }

            var mainApplication = applications.FirstOrDefault(app => (int?)app.statuscode == 845280004); // Approved
            if (mainApplication != null)
            {
                licenseSummary.ApplicationId = mainApplication.Id.ToString();
                licenseSummary.ApplicationTypeName = mainApplication.adoxio_ApplicationTypeId?.Name;
                if (mainApplication.adoxio_ApplicationTypeId != null)
                {
                    var appType = await GetCachedApplicationTypeAsync(mainApplication.adoxio_ApplicationTypeId.Id.ToString(), dataverse, cache);
                    licenseSummary.ApplicationTypeCategory = (ApplicationTypeCategory?)(int?)appType?.adoxio_Category;
                }
            }

            if (licence.adoxio_LicenceType != null)
            {
                var licenceType = await GetCachedLicenceTypeAsync(licence.adoxio_LicenceType.Id.ToString(), dataverse, cache);
                if (licenceType != null)
                {
                    licenseSummary.LicenceTypeName = licenceType.adoxio_name;
                    licenseSummary.LicenceTypeCategory = (LicenceTypeCategory?)(int?)licenceType.adoxio_Category;

                    var appTypes = await dataverse.GetApplicationTypesByLicenceTypeIdAsync(licenceType.Id.ToString());
                    foreach (var item in appTypes.OrderBy(at => at.adoxio_ActionText))
                    {
                        bool isEndorsementThatIsProcessed = item.adoxio_IsEndorsement == true
                            && (licenseSummary.Endorsements?.Any(e => e.ApplicationTypeId == item.Id.ToString()) ?? false);
                        if (!isEndorsementThatIsProcessed)
                            licenseSummary.AllowedActions.Add(item.ToViewModel());
                    }
                }
            }

            licenseSummary.Endorsements = await GetEndorsementsAsync(licenceId, dataverse);
            licenseSummary.OffsiteStorageLocations = await GetOffsiteStorageAsync(licenceId, dataverse);
            licenseSummary.ServiceAreas = await GetServiceAreasAsync(licenceId, dataverse);

            return licenseSummary;
        }

        private static async Task<adoxio_licencetype?> GetCachedLicenceTypeAsync(string id, IDataverseClient dataverse, IMemoryCache cache)
        {
            if (cache == null) return await dataverse.GetLicenceTypeByIdAsync(id);
            string key = CacheKeys.LicenceTypePrefix + id;
            if (!cache.TryGetValue(key, out adoxio_licencetype result))
            {
                result = await dataverse.GetLicenceTypeByIdAsync(id);
                if (result != null)
                    cache.Set(key, result, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(365)));
            }
            return result;
        }

        public static async Task<List<ViewModels.License>> GetLicensesByLicenceeAsync(IDataverseClient dataverse, string accountId, IMemoryCache? cache = null)
        {
            var licences = await dataverse.GetLicencesByAccountIdAsync(accountId);
            var result = new List<ViewModels.License>();
            foreach (var lic in licences)
                result.Add(await lic.ToViewModelAsync(dataverse));
            return result;
        }

        public static async Task<List<ViewModels.License>> GetPaidLicensesOnTransferAsync(IDataverseClient dataverse, string accountId)
        {
            var proposed = await dataverse.GetLicencesByProposedOwnerAsync(accountId);
            var tpo = await dataverse.GetLicencesByThirdPartyOperatorAsync(accountId);
            var all = proposed.Concat(tpo).GroupBy(l => l.adoxio_licencesId).Select(g => g.First()).ToList();
            var result = new List<ViewModels.License>();
            foreach (var lic in all)
                result.Add(await lic.ToViewModelAsync(dataverse));
            return result;
        }

        public static async Task<List<ApplicationLicenseSummary>> GetLicenseSummariesByLicenceeAsync(IDataverseClient dataverse, string accountId, IMemoryCache? cache = null)
        {
            var licences = await dataverse.GetLicencesByAccountIdAsync(accountId);
            var result = new List<ApplicationLicenseSummary>();
            foreach (var lic in licences)
                result.Add(await lic.ToLicenseSummaryViewModelAsync(new List<adoxio_application>(), dataverse, cache));
            return result;
        }

        public static async Task<List<ApplicationLicenseSummary>> GetPaidLicenseSummariesOnTransferAsync(IDataverseClient dataverse, string accountId, IMemoryCache? cache = null)
        {
            var proposed = await dataverse.GetLicencesByProposedOwnerAsync(accountId);
            var tpo = await dataverse.GetLicencesByThirdPartyOperatorAsync(accountId);
            var all = proposed.Concat(tpo).GroupBy(l => l.adoxio_licencesId).Select(g => g.First()).ToList();
            var result = new List<ApplicationLicenseSummary>();
            foreach (var lic in all)
                result.Add(await lic.ToLicenseSummaryViewModelAsync(new List<adoxio_application>(), dataverse, cache));
            return result;
        }

        private static async Task<adoxio_licencesubcategory?> GetCachedLicenceSubCategoryAsync(string id, IDataverseClient dataverse, IMemoryCache cache)
        {
            if (cache == null) return await dataverse.GetLicenceSubCategoryByIdAsync(id);
            string key = "LicenceSubCategory_" + id;
            if (!cache.TryGetValue(key, out adoxio_licencesubcategory result))
            {
                result = await dataverse.GetLicenceSubCategoryByIdAsync(id);
                if (result != null)
                    cache.Set(key, result, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(365)));
            }
            return result;
        }

        private static async Task<adoxio_applicationtype?> GetCachedApplicationTypeAsync(string id, IDataverseClient dataverse, IMemoryCache cache)
        {
            if (cache == null) return await dataverse.GetApplicationTypeByIdAsync(id);
            string key = "ApplicationType_" + id;
            if (!cache.TryGetValue(key, out adoxio_applicationtype result))
            {
                result = await dataverse.GetApplicationTypeByIdAsync(id);
                if (result != null)
                    cache.Set(key, result, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(365)));
            }
            return result;
        }
    }
}
