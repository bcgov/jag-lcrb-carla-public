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
                var endorsementTasks = endorsements.Select(async item =>
                {
                    var endorsementId = item.adoxio_endorsementId?.ToString();
                    var hoursOfServiceTask = GetHoursOfServiceListAsync(endorsementId, dataverse);
                    var areaCapacityTask = GetAreaCapacityAsync(endorsementId, dataverse);
                    await Task.WhenAll(hoursOfServiceTask, areaCapacityTask);
                    return new ViewModels.Endorsement
                    {
                        ApplicationTypeId = item.adoxio_ApplicationType?.Id.ToString(),
                        ApplicationTypeName = item.adoxio_ApplicationType?.Name,
                        EndorsementId = endorsementId,
                        EndorsementName = item.adoxio_name,
                        HoursOfServiceList = hoursOfServiceTask.Result,
                        AreaCapacity = areaCapacityTask.Result
                    };
                });
                endorsementsList.AddRange(await Task.WhenAll(endorsementTasks));
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

            var mainApplication = applications.FirstOrDefault(app => (int?)app.statuscode == 845280004); // Approved
            var licTypeId = licence.adoxio_LicenceType?.Id.ToString();
            var mainAppTypeId = mainApplication?.adoxio_ApplicationTypeId?.Id.ToString();

            // Phase 1: 5 independent cached lookups in parallel (max 5 concurrent per licence)
            var estTask = licence.adoxio_establishment != null
                ? GetCachedEstablishmentAsync(licence.adoxio_establishment.Id.ToString(), dataverse, cache)
                : Task.FromResult<adoxio_establishment?>(null);
            var subCatTask = licence.adoxio_LicenceSubCategoryId != null
                ? GetCachedLicenceSubCategoryAsync(licence.adoxio_LicenceSubCategoryId.Id.ToString(), dataverse, cache)
                : Task.FromResult<adoxio_licencesubcategory?>(null);
            var mainAppTypeTask = !string.IsNullOrEmpty(mainAppTypeId)
                ? GetCachedApplicationTypeAsync(mainAppTypeId, dataverse, cache)
                : Task.FromResult<adoxio_applicationtype?>(null);
            var licTypeTask = !string.IsNullOrEmpty(licTypeId)
                ? GetCachedLicenceTypeAsync(licTypeId, dataverse, cache)
                : Task.FromResult<adoxio_licencetype?>(null);
            var appTypesTask = !string.IsNullOrEmpty(licTypeId)
                ? GetCachedApplicationTypesByLicenceTypeIdAsync(licTypeId, dataverse, cache)
                : Task.FromResult<IList<adoxio_applicationtype>>(new List<adoxio_applicationtype>());
            await Task.WhenAll(estTask, subCatTask, mainAppTypeTask, licTypeTask, appTypesTask);

            var est = estTask.Result;
            if (est != null)
            {
                licenseSummary.EstablishmentName = est.adoxio_name;
                licenseSummary.EstablishmentIsOpen = est.adoxio_IsOpen;
                licenseSummary.EstablishmentId = est.Id.ToString();
                licenseSummary.EstablishmentEmail = est.adoxio_Email;
            }

            licenseSummary.LicenseSubCategory = subCatTask.Result?.adoxio_name;

            if (mainApplication != null)
            {
                licenseSummary.ApplicationId = mainApplication.Id.ToString();
                licenseSummary.ApplicationTypeName = mainApplication.adoxio_ApplicationTypeId?.Name;
                licenseSummary.ApplicationTypeCategory = (ApplicationTypeCategory?)(int?)mainAppTypeTask.Result?.adoxio_Category;
            }

            var licenceType = licTypeTask.Result;
            if (licenceType != null)
            {
                licenseSummary.LicenceTypeName = licenceType.adoxio_name;
                licenseSummary.LicenceTypeCategory = (LicenceTypeCategory?)(int?)licenceType.adoxio_Category;

                foreach (var item in appTypesTask.Result.OrderBy(at => at.adoxio_ActionText))
                {
                    bool isEndorsementThatIsProcessed = item.adoxio_IsEndorsement == true
                        && (licenseSummary.Endorsements?.Any(e => e.ApplicationTypeId == item.Id.ToString()) ?? false);
                    if (!isEndorsementThatIsProcessed)
                        licenseSummary.AllowedActions.Add(item.ToViewModel());
                }
            }

            // Phase 2: 3 cached per-licence queries in parallel (short TTL - see GetCached* methods below)
            var endorsementsTask = GetCachedEndorsementsAsync(licenceId, dataverse, cache);
            var offsiteTask = GetCachedOffsiteStorageAsync(licenceId, dataverse, cache);
            var serviceAreasTask = GetCachedServiceAreasAsync(licenceId, dataverse, cache);
            await Task.WhenAll(endorsementsTask, offsiteTask, serviceAreasTask);

            licenseSummary.Endorsements = endorsementsTask.Result;
            licenseSummary.OffsiteStorageLocations = offsiteTask.Result;
            licenseSummary.ServiceAreas = serviceAreasTask.Result;

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
            var proposedTask = dataverse.GetLicencesByProposedOwnerAsync(accountId);
            var tpoTask = dataverse.GetLicencesByThirdPartyOperatorAsync(accountId);
            await Task.WhenAll(proposedTask, tpoTask);
            var all = proposedTask.Result.Concat(tpoTask.Result)
                .GroupBy(l => l.adoxio_licencesId).Select(g => g.First()).ToList();
            var summaryTasks = all.Select(lic =>
                lic.ToLicenseSummaryViewModelAsync(new List<adoxio_application>(), dataverse, cache));
            return new List<ApplicationLicenseSummary>(await Task.WhenAll(summaryTasks));
        }

        private static async Task<adoxio_establishment?> GetCachedEstablishmentAsync(string id, IDataverseClient dataverse, IMemoryCache cache)
        {
            if (cache == null) return await dataverse.GetEstablishmentByIdAsync(id);
            string key = "Establishment_" + id;
            if (!cache.TryGetValue(key, out adoxio_establishment result))
            {
                result = await dataverse.GetEstablishmentByIdAsync(id);
                if (result != null)
                    cache.Set(key, result, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));
            }
            return result;
        }

        private static async Task<IList<adoxio_applicationtype>> GetCachedApplicationTypesByLicenceTypeIdAsync(string licenceTypeId, IDataverseClient dataverse, IMemoryCache cache)
        {
            if (cache == null) return await dataverse.GetApplicationTypesByLicenceTypeIdAsync(licenceTypeId);
            string key = CacheKeys.ApplicationTypePrefix + "ByLicenceType_" + licenceTypeId;
            if (!cache.TryGetValue(key, out IList<adoxio_applicationtype> result))
            {
                result = await dataverse.GetApplicationTypesByLicenceTypeIdAsync(licenceTypeId);
                if (result != null)
                    cache.Set(key, result, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));
            }
            return result ?? new List<adoxio_applicationtype>();
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

        // Phase 2 per-licence data (endorsements, offsite storage, service areas) uses a short TTL,
        // unlike the near-static reference data cached above. Endorsements have no in-app write path
        // (created out-of-band by back-office processes) so a short TTL is the only way to bound
        // staleness. Offsite storage and service areas ARE written to in-app, so callers that mutate
        // them must remove the corresponding cache entry - see LicensesController/ApplicationsController.
        private static readonly TimeSpan Phase2CacheDuration = TimeSpan.FromMinutes(2);

        public static async Task<List<ViewModels.Endorsement>> GetCachedEndorsementsAsync(string licenceId, IDataverseClient dataverse, IMemoryCache cache)
        {
            if (cache == null) return await GetEndorsementsAsync(licenceId, dataverse);
            string key = CacheKeys.EndorsementsByLicencePrefix + licenceId;
            if (!cache.TryGetValue(key, out List<ViewModels.Endorsement> result))
            {
                result = await GetEndorsementsAsync(licenceId, dataverse);
                cache.Set(key, result, new MemoryCacheEntryOptions().SetSlidingExpiration(Phase2CacheDuration));
            }
            return result;
        }

        public static async Task<List<OffsiteStorage>> GetCachedOffsiteStorageAsync(string licenceId, IDataverseClient dataverse, IMemoryCache cache)
        {
            if (cache == null) return await GetOffsiteStorageAsync(licenceId, dataverse);
            string key = CacheKeys.OffsiteStorageByLicencePrefix + licenceId;
            if (!cache.TryGetValue(key, out List<OffsiteStorage> result))
            {
                result = await GetOffsiteStorageAsync(licenceId, dataverse);
                cache.Set(key, result, new MemoryCacheEntryOptions().SetSlidingExpiration(Phase2CacheDuration));
            }
            return result;
        }

        public static async Task<List<CapacityArea>> GetCachedServiceAreasAsync(string licenceId, IDataverseClient dataverse, IMemoryCache cache)
        {
            if (cache == null) return await GetServiceAreasAsync(licenceId, dataverse);
            string key = CacheKeys.ServiceAreasByLicencePrefix + licenceId;
            if (!cache.TryGetValue(key, out List<CapacityArea> result))
            {
                result = await GetServiceAreasAsync(licenceId, dataverse);
                cache.Set(key, result, new MemoryCacheEntryOptions().SetSlidingExpiration(Phase2CacheDuration));
            }
            return result;
        }
    }
}
