extern alias DV;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Gov.Lclb.Cllb.Public.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using FolderSegment = Gov.Lclb.Cllb.Interfaces.FolderSegment;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using Account = DV::Gov.Lclb.Cllb.Interfaces.Account;
using adoxio_licences = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licences;
using adoxio_application = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application;
using adoxio_offsitestorage = DV::Gov.Lclb.Cllb.Interfaces.adoxio_offsitestorage;
using adoxio_offsitestorage_statuscode = DV::Gov.Lclb.Cllb.Interfaces.adoxio_offsitestorage_statuscode;
using adoxio_licences_adoxio_transferrequested = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licences_adoxio_transferrequested;
using adoxio_licences_adoxio_tporequested = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licences_adoxio_tporequested;
using adoxio_application_statuscode = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_statuscode;
using adoxio_applicationtype = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicationtype;
using adoxio_establishment = DV::Gov.Lclb.Cllb.Interfaces.adoxio_establishment;
using adoxio_application_adoxio_isoninland = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_isoninland;
using adoxio_application_adoxio_manufacturerproductionamountunit = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_manufacturerproductionamountunit;
using adoxio_application_adoxio_checklistconclusivelydeem = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_checklistconclusivelydeem;
using invoice_statuscode = DV::Gov.Lclb.Cllb.Interfaces.invoice_statuscode;
using DvInvoice = DV::Gov.Lclb.Cllb.Interfaces.Invoice;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LicensesController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IDataverseClient _dataverse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPdfService _pdfClient;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;
        private readonly FileManagerClient _fileManagerClient;

        public LicensesController(IDataverseClient dataverse,
            IHttpContextAccessor httpContextAccessor, IPdfService pdfClient,
            ILoggerFactory loggerFactory, IMemoryCache memoryCache, IWebHostEnvironment env,
            FileManagerClient fileClient)
        {
            _cache = memoryCache;
            _dataverse = dataverse;
            _httpContextAccessor = httpContextAccessor;
            _pdfClient = pdfClient;
            _logger = loggerFactory.CreateLogger(typeof(LicensesController));
            _env = env;
            _fileManagerClient = fileClient;
        }

        /// <summary>
        /// Get autocomplete data for a licence search, by name or licence number.
        /// Returns an empty list if no search criteria is provided.
        /// </summary>
        [HttpGet("autocomplete")]
        [Authorize(Policy = "Business-User")]
        public async Task<List<RelatedLicence>> GetAutocomplete(string name = null, string licenceNumber = null)
        {
            var results = new List<RelatedLicence>();
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(licenceNumber))
                return results;
            try
            {
                var licences = await _dataverse.GetLicencesByNameOrNumberAsync(name, licenceNumber);
                foreach (var licence in licences)
                {
                    results.Add(new RelatedLicence
                    {
                        Id = licence.adoxio_licencesId?.ToString(),
                        Name = licence.adoxio_name,
                        EstablishmentName = licence.adoxio_establishment?.Name,
                        Streetaddress = licence.adoxio_EstablishmentAddressStreet,
                        City = licence.adoxio_EstablishmentAddressCity,
                        Provstate = "BC",
                        Country = "CANADA",
                        PostalCode = licence.adoxio_EstablishmentAddressPostalCode,
                        Licensee = licence.adoxio_Licencee?.Name,
                        LicenceNumber = licence.adoxio_LicenceNumber,
                        Valid = true
                    });
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error while getting autocomplete data.");
            }
            return results;
        }

        /// GET licence by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLicence(string id)
        {
            var licence = await _dataverse.GetLicenceByIdWithChildrenAsync(id);
            if (licence == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(licence.adoxio_Licencee?.Id.ToString()) &&
                (licence.adoxio_ProposedOwner == null || !CurrentUserHasAccessToLicenseTransferredTo(licence.adoxio_ProposedOwner?.Id.ToString())))
            {
                return Forbid();
            }

            var docLocs = await _dataverse.GetSharePointDocLocsByObjectIdAsync(id);
            if (docLocs == null || docLocs.Count == 0)
            {
                await InitializeSharepointAsync(licence, id);
            }

            return new JsonResult(await licence.ToViewModelAsync(_dataverse));
        }

        private async Task InitializeSharepointAsync(adoxio_licences licence, string licenceId)
        {
            var folderName = $"{licence.adoxio_name}_{licenceId.ToUpper().Replace("-", "")}";
            _fileManagerClient.CreateFolderIfNotExist(_logger, SharePointConstants.LicenceFolderInternalName, folderName);
            await _dataverse.CreateLicenceSharePointDocLocAsync(licenceId, folderName, folderName);
        }

        [HttpPut("{licenceId}/representative")]
        public async Task<IActionResult> UpdateLicenseeRepresentative([FromBody] ApplicationLicenseSummary item, string licenceId)
        {
            if (item == null || string.IsNullOrEmpty(licenceId) || licenceId != item.LicenseId)
            {
                return BadRequest();
            }

            var licence = await _dataverse.GetLicenceByIdWithChildrenAsync(licenceId);
            if (licence == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(licence.adoxio_Licencee?.Id.ToString()))
            {
                return Forbid();
            }

            var patch = new adoxio_licences
            {
                Id = new Guid(licenceId),
                adoxio_RepresentativeName = item.RepresentativeFullName,
                adoxio_RepresentativePhone = item.RepresentativePhoneNumber,
                adoxio_RepresentativeEmail = item.RepresentativeEmail,
                adoxio_CanSubmitPermanentChangeApplications = item.RepresentativeCanSubmitPermanentChangeApplications,
                adoxio_CanSignTemporaryChangeApplications = item.RepresentativeCanSignTemporaryChangeApplications,
                adoxio_CanObtainLicenceInformation = item.RepresentativeCanObtainLicenceInformation,
                adoxio_CanSignGroceryStoreProofofSales = item.RepresentativeCanSignGroceryStoreProofOfSale,
                adoxio_CanAttendEducationSessions = item.RepresentativeCanAttendEducationSessions,
                adoxio_CanAttendComplianceMeetings = item.RepresentativeCanAttendComplianceMeetings,
                adoxio_CanRepresentatHearings = item.RepresentativeCanRepresentAtHearings
            };

            await _dataverse.UpdateLicenceAsync(patch);

            licence = await _dataverse.GetLicenceByIdWithChildrenAsync(licenceId);
            var allApps = await _dataverse.GetApplicationsForLicenceByApplicantAsync(licence.adoxio_Licencee?.Id.ToString() ?? "");
            var licenceApps = allApps.Where(app => app.adoxio_AssignedLicence?.Id.ToString() == licenceId).ToList();

            return new JsonResult(await licence.ToLicenseSummaryViewModelAsync(licenceApps, _dataverse, _cache));
        }

        [HttpPut("{licenceId}/offsite-storage")]
        public async Task<IActionResult> UpdateOffsiteStorageLocations([FromBody] ApplicationLicenseSummary item, string licenceId)
        {
            if (item == null || string.IsNullOrEmpty(licenceId) || licenceId != item.LicenseId)
            {
                return BadRequest();
            }

            var licence = await _dataverse.GetLicenceByIdWithChildrenAsync(licenceId);
            if (licence == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(licence.adoxio_Licencee?.Id.ToString()))
            {
                return Forbid();
            }

            if (item.OffsiteStorageLocations != null && item.OffsiteStorageLocations.Count > 0)
            {
                var existingLocations = await _dataverse.GetOffSiteStorageByLicenceIdAsync(licenceId);
                foreach (var loc in item.OffsiteStorageLocations.Where(x => x != null))
                {
                    if (loc.Id == null)
                    {
                        await CreateOffsiteStorageAsync(loc, licenceId);
                    }
                    else if (existingLocations.Any(x => x.adoxio_offsitestorageId?.ToString() == loc.Id))
                    {
                        await UpdateOffsiteStorageAsync(loc);
                    }
                }
            }

            licence = await _dataverse.GetLicenceByIdWithChildrenAsync(licenceId);
            var allApps = await _dataverse.GetApplicationsForLicenceByApplicantAsync(licence.adoxio_Licencee?.Id.ToString() ?? "");
            var licenceApps = allApps.Where(app => app.adoxio_AssignedLicence?.Id.ToString() == licenceId).ToList();

            return new JsonResult(await licence.ToLicenseSummaryViewModelAsync(licenceApps, _dataverse, _cache));
        }

        private async Task CreateOffsiteStorageAsync(OffsiteStorage item, string licenceId)
        {
            if (item.Id != null) return;
            var storage = new adoxio_offsitestorage
            {
                adoxio_LicenceId = new EntityReference(adoxio_licences.EntityLogicalName, new Guid(licenceId)),
                statuscode = adoxio_offsitestorage_statuscode.Added,
                adoxio_name = item.Name,
                adoxio_Street1 = item.Street1,
                adoxio_City = item.City,
                adoxio_PostalCode = item.PostalCode,
                adoxio_DateAdded = DateTime.Now
            };
            await _dataverse.CreateOffSiteStorageAsync(storage);
        }

        private async Task UpdateOffsiteStorageAsync(OffsiteStorage item)
        {
            if (item.Id == null) return;
            var storage = new adoxio_offsitestorage
            {
                Id = new Guid(item.Id),
                adoxio_name = item.Name,
                adoxio_Street1 = item.Street1,
                adoxio_City = item.City,
                adoxio_PostalCode = item.PostalCode
            };
            await _dataverse.UpdateOffSiteStorageAsync(storage);
        }

        [HttpPost("cancel-transfer")]
        public async Task<ActionResult> CancelTransfer(LicenceTransfer item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var adoxioLicense = await _dataverse.GetLicenceByIdWithChildrenAsync(item.LicenceId);
            if (adoxioLicense == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.adoxio_Licencee?.Id.ToString()) &&
                !CurrentUserHasAccessToLicenseTransferredTo(adoxioLicense.adoxio_ProposedOwner?.Id.ToString()))
            {
                return Forbid();
            }

            var patchLicence = new adoxio_licences
            {
                Id = new Guid(item.LicenceId),
                adoxio_TransferRequested = adoxio_licences_adoxio_transferrequested.No
            };
            await _dataverse.UpdateLicenceAsync(patchLicence);

            await _dataverse.ClearLicenceProposedOwnerAsync(item.LicenceId);

            var activeApps = await _dataverse.GetActiveApplicationsByAssignedLicenceIdAsync(item.LicenceId);
            foreach (var app in activeApps)
            {
                var appType = await _dataverse.GetApplicationTypeByIdAsync(app.adoxio_ApplicationTypeId?.Id.ToString());
                if (appType?.adoxio_name?.Contains("CRS Transfer of Ownership") == true)
                {
                    var appPatch = new adoxio_application
                    {
                        Id = app.adoxio_applicationId.Value,
                        statuscode = adoxio_application_statuscode.Terminated
                    };
                    await _dataverse.UpdateApplicationAsync(appPatch);
                }
            }

            return Ok();
        }

        [HttpPost("initiate-transfer")]
        public async Task<ActionResult> InitiateTransfer(LicenceTransfer item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var adoxioLicense = await _dataverse.GetLicenceByIdWithChildrenAsync(item.LicenceId);
            if (adoxioLicense == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.adoxio_Licencee?.Id.ToString()))
            {
                return Forbid();
            }

            var patchLicence = new adoxio_licences
            {
                Id = new Guid(item.LicenceId),
                adoxio_ProposedOwner = new EntityReference(Account.EntityLogicalName, new Guid(item.AccountId)),
                adoxio_TransferRequested = adoxio_licences_adoxio_transferrequested.Yes
            };
            await _dataverse.UpdateLicenceAsync(patchLicence);

            return Ok();
        }

        [HttpPost("initiate-tied-house-excemption")]
        public async Task<IActionResult> InitiateTiedHouseExcemption(TiedHouseExcemptionRequest item)
        {
            if (!ModelState.IsValid ||
                string.IsNullOrEmpty(item.LicenceId) ||
                string.IsNullOrEmpty(item.RelatedLicenceId))
            {
                return BadRequest();
            }

            var dvLicence = await _dataverse.GetLicenceByIdAsync(item.LicenceId);
            if (dvLicence == null)
                return NotFound();

            var relatedLicence = await _dataverse.GetLicenceByIdAsync(item.RelatedLicenceId);
            if (!CurrentUserHasAccessToLicenseOwnedBy(relatedLicence?.adoxio_Licencee?.Id.ToString()))
                return Forbid();

            await CreateApplicationAsync(item.LicenceId, ApplicationTypeNames.TiedHouseExemption, item.RelatedLicenceId, item.ManufacturerProductionAmountforPrevYear, item.ManufacturerProductionAmountUnit);

            return Ok();
        }

        /// <summary>
        /// Set expiry for a given licence to different dates as specified by workflow GUIDs.  Only useful for automated testing.
        /// </summary>
        [HttpGet("{workflowGUID}/setexpiry/{licenceID}")]
        public async Task<IActionResult> SetExpiry(string workflowGUID, string licenceID)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            if (userSettings.AccountId != null && !userSettings.IsNewUserRegistration && userSettings.AccountId.Length > 0)
            {
                try
                {
                    await _dataverse.ExecuteWorkflowAsync(workflowGUID, licenceID);
                    return new JsonResult("OK");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error executing workflow");
                    return BadRequest(e.Message);
                }
            }

            return BadRequest("This API is not available to an unregistered user.");
        }

        /// <summary>
        /// Set autorenewal to 'No' to deny licence renewal for a given licence. Only useful for automated testing.
        /// </summary>
        [HttpGet("denyautorenew/{licenceID}")]
        public async Task<IActionResult> DenyAutoRenew(string licenceID)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            if (userSettings.AccountId != null && !userSettings.IsNewUserRegistration && userSettings.AccountId.Length > 0)
            {
                try
                {
                    await _dataverse.ExecuteWorkflowAsync("e1792ccf-e40b-491f-9a9a-ee8e977749e6", licenceID);
                    return new JsonResult("OK");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error executing workflow");
                    return BadRequest(e.Message);
                }
            }

            return BadRequest("This API is not available to an unregistered user.");
        }

        [HttpPost("set-third-party-operator")]
        public async Task<ActionResult> SetThirdPartyOperator(LicenceTransfer item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var adoxioLicense = await _dataverse.GetLicenceByIdWithChildrenAsync(item.LicenceId);
            if (adoxioLicense == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.adoxio_Licencee?.Id.ToString()) &&
                !CurrentUserHasAccessToLicenseTransferredTo(adoxioLicense.adoxio_ProposedOwner?.Id.ToString()))
            {
                return Forbid();
            }

            var patchLicence = new adoxio_licences
            {
                Id = new Guid(item.LicenceId),
                adoxio_ThirdPartyOperatorId = new EntityReference(Account.EntityLogicalName, new Guid(item.AccountId)),
                adoxio_TPORequested = adoxio_licences_adoxio_tporequested.Yes
            };
            await _dataverse.UpdateLicenceAsync(patchLicence);

            return Ok();
        }

        [HttpPost("cancel-operator-application")]
        public async Task<ActionResult> CancelTPO(LicenceTransfer item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var adoxioLicense = await _dataverse.GetLicenceByIdWithChildrenAsync(item.LicenceId);
            if (adoxioLicense == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.adoxio_Licencee?.Id.ToString()) &&
                !CurrentUserHasAccessToLicenseTransferredTo(adoxioLicense.adoxio_ProposedOwner?.Id.ToString()))
            {
                return Forbid();
            }

            var patchLicence = new adoxio_licences
            {
                Id = new Guid(item.LicenceId),
                adoxio_TPORequested = adoxio_licences_adoxio_tporequested.No
            };
            await _dataverse.UpdateLicenceAsync(patchLicence);

            await _dataverse.ClearAccountProposedOperatorAsync(item.AccountId);

            var activeApps = await _dataverse.GetActiveApplicationsByAssignedLicenceIdAsync(item.LicenceId);
            foreach (var app in activeApps)
            {
                var appType = await _dataverse.GetApplicationTypeByIdAsync(app.adoxio_ApplicationTypeId?.Id.ToString());
                if (appType?.adoxio_name?.Contains("Third Party Operator") == true)
                {
                    var appPatch = new adoxio_application
                    {
                        Id = app.adoxio_applicationId.Value,
                        statuscode = adoxio_application_statuscode.Terminated
                    };
                    await _dataverse.UpdateApplicationAsync(appPatch);
                }
            }

            return Ok();
        }

        [HttpPost("terminate-operator-relationship")]
        public async Task<ActionResult> TerminateTPORelationship(LicenceTransfer item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var adoxioLicense = await _dataverse.GetLicenceByIdWithChildrenAsync(item.LicenceId);
            if (adoxioLicense == null)
            {
                return NotFound();
            }

            bool hasAccess = CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.adoxio_Licencee?.Id.ToString());
            hasAccess |= (adoxioLicense.adoxio_ThirdPartyOperatorId != null &&
                          CurrentUserHasAccessToLicenseTransferredTo(adoxioLicense.adoxio_ThirdPartyOperatorId?.Id.ToString()));
            if (!hasAccess)
            {
                return Forbid();
            }

            await _dataverse.ClearLicenceThirdPartyOperatorAsync(item.LicenceId);

            return Ok();
        }

        private async Task<adoxio_application> CreateApplicationAsync(string licenceId, string applicationTypeName, string relatedLicenceId = null, int? prodAmount = null, int? prodUnit = null)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            var dvLic = await _dataverse.GetLicenceByIdAsync(licenceId);
            if (dvLic == null) throw new Exception("Error getting license.");

            var appType = await _dataverse.GetApplicationTypeByNameAsync(applicationTypeName);
            if (appType == null) throw new Exception($"Application type '{applicationTypeName}' not found.");

            var account = await _dataverse.GetAccountByIdAsync(userSettings.AccountId);

            adoxio_establishment dvEst = null;
            if (dvLic.adoxio_establishment?.Id != null)
                dvEst = await _dataverse.GetEstablishmentByIdAsync(dvLic.adoxio_establishment.Id.ToString());

            var application = new adoxio_application();

            bool copyAddress = applicationTypeName != "CRS Location Change";
            if (copyAddress)
            {
                application.adoxio_EstablishmentAddressCity = dvLic.adoxio_EstablishmentAddressCity;
                application.adoxio_EstablishmentAddressStreet = dvLic.adoxio_EstablishmentAddressStreet;
                application.adoxio_EstablishmentAddressPostalCode = dvLic.adoxio_EstablishmentAddressPostalCode;
            }

            if (dvEst != null)
            {
                application.adoxio_EstablishmentPropsedName = dvEst.adoxio_name;
                application.adoxio_EstablishmentEmail = dvEst.adoxio_Email;
                application.adoxio_EstablishmentPhone = dvEst.adoxio_Phone;
                application.adoxio_EstablishmentParcelID = dvEst.adoxio_ParcelID;
                application.adoxio_IsonINLand = (adoxio_application_adoxio_isoninland?)(int?)dvEst.adoxio_IsonINLand;
                if (dvEst.adoxio_PDJurisdiction != null)
                    application.adoxio_PoliceJurisdictionId = dvEst.adoxio_PDJurisdiction;
                if (dvEst.adoxio_LGIN != null)
                    application.adoxio_localgovindigenousnationid = dvEst.adoxio_LGIN;
            }

            application.adoxio_ApplicantType = account?.adoxio_BusinessType;
            application.adoxio_ApplicationTypeId = new EntityReference(adoxio_applicationtype.EntityLogicalName, appType.adoxio_applicationtypeId!.Value);
            application.adoxio_AssignedLicence = new EntityReference(adoxio_licences.EntityLogicalName, dvLic.Id);

            if (dvLic.adoxio_LicenceType != null)
                application.adoxio_LicenceType = dvLic.adoxio_LicenceType;
            if (dvLic.adoxio_LicenceSubCategoryId != null)
                application.adoxio_LicenceSubCategoryId = dvLic.adoxio_LicenceSubCategoryId;

            application.adoxio_Applicant = new EntityReference(Account.EntityLogicalName, new Guid(userSettings.AccountId));

            if (dvLic.adoxio_establishment != null)
                application.adoxio_LicenceEstablishment = dvLic.adoxio_establishment;

            application.adoxio_manufacturerproductionamountforprevyear = prodAmount;
            if (prodUnit.HasValue)
                application.adoxio_manufacturerproductionamountunit = (adoxio_application_adoxio_manufacturerproductionamountunit?)(int?)prodUnit;

            if (relatedLicenceId != null && dvEst != null)
            {
                application.adoxio_EstablishmentAddressStreet = dvEst.adoxio_AddressStreet;
                application.adoxio_EstablishmentAddressCity = dvEst.adoxio_AddressCity;
                application.adoxio_EstablishmentAddressPostalCode = dvEst.adoxio_AddressPostalCode;
                application.adoxio_RelatedLicence = new EntityReference(adoxio_licences.EntityLogicalName, new Guid(relatedLicenceId));
                if (dvLic.adoxio_Licencee != null)
                    application.adoxio_Applicant = dvLic.adoxio_Licencee;
            }

            var activeApps = await _dataverse.GetActiveApplicationsByAssignedLicenceIdAsync(licenceId);
            var lginRef = activeApps.FirstOrDefault(a => a.adoxio_localgovindigenousnationid != null)?.adoxio_localgovindigenousnationid
                          ?? dvLic.adoxio_LGIN
                          ?? dvEst?.adoxio_LGIN;
            if (lginRef != null)
                application.adoxio_localgovindigenousnationid = lginRef;

            var policeRef = activeApps.FirstOrDefault(a => a.adoxio_PoliceJurisdictionId != null)?.adoxio_PoliceJurisdictionId;
            if (policeRef != null)
                application.adoxio_PoliceJurisdictionId = policeRef;

            var appId = await _dataverse.CreateApplicationAsync(application);
            return await _dataverse.GetApplicationByIdAsync(appId.ToString()) ?? application;
        }

        /// Create a change of location application
        [HttpPost("{licenceId}/create-action-application")]
        public async Task<IActionResult> CreateApplicationForAction(string licenceId, [FromQuery] string applicationType)
        {
            if (string.IsNullOrEmpty(applicationType)) return BadRequest();

            var application = await CreateApplicationAsync(licenceId, applicationType);
            var result = await application.ToViewModelAsync(_dataverse, _cache, _logger);
            return new JsonResult(result);
        }

        private async Task<adoxio_application?> GetTermChangeApplicationAsync(string licenceId, string termId, string applicationTypeName)
        {
            var appType = await _dataverse.GetApplicationTypeByNameAsync(applicationTypeName);
            if (appType == null) return null;

            var excludeStatuses = new List<int>
            {
                (int)AdoxioApplicationStatusCodes.Processed,
                (int)AdoxioApplicationStatusCodes.Terminated,
                (int)AdoxioApplicationStatusCodes.Cancelled,
                (int)AdoxioApplicationStatusCodes.Approved,
                (int)AdoxioApplicationStatusCodes.Refused,
                (int)AdoxioApplicationStatusCodes.TerminatedAndRefunded
            };

            var candidates = await _dataverse.GetApplicationsByTypeAndAssignedLicenceAsync(
                appType.adoxio_applicationtypeId!.Value.ToString(), licenceId, excludeStatuses);

            foreach (var candidate in candidates)
            {
                var tc = await _dataverse.GetTermsConditionsByIdAsync(termId);
                if (tc?.adoxio_Application?.Id == candidate.Id)
                    return candidate;
            }

            return null;
        }

        /// Create a change of location application
        [HttpPost("{licenceId}/create-action-application-term/{termId}")]
        public async Task<IActionResult> CreateOrGetApplicationWithTerm(string licenceId, string termId,
            [FromQuery] string applicationType)
        {
            if (string.IsNullOrEmpty(applicationType)) return BadRequest();

            var application = await GetTermChangeApplicationAsync(licenceId, termId, applicationType);

            if (application == null)
            {
                application = await CreateApplicationAsync(licenceId, applicationType);

                if (!string.IsNullOrEmpty(termId) && application.Id != Guid.Empty)
                {
                    try
                    {
                        await _dataverse.AssociateTermsConditionsToApplicationAsync(application.Id.ToString(), termId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating application with reference to term");
                    }
                }
            }

            var result = await application.ToViewModelAsync(_dataverse, _cache, _logger);
            return new JsonResult(result);
        }

        /// GET all licenses in Dynamics by Licencee using the account Id assigned to the user logged in
        [HttpGet("current")]
        public async Task<List<ApplicationLicenseSummary>> GetCurrentUserLicences()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            var dvLicences = await _dataverse.GetLicencesByAccountIdAsync(userSettings.AccountId);
            var allApps = await _dataverse.GetApplicationsForLicenceByApplicantAsync(userSettings.AccountId);

            var adoxioLicences = new List<ApplicationLicenseSummary>();
            foreach (var lic in dvLicences)
            {
                var licId = lic.adoxio_licencesId?.ToString();
                var licApps = allApps.Where(app => app.adoxio_AssignedLicence?.Id.ToString() == licId).ToList();
                adoxioLicences.Add(await lic.ToLicenseSummaryViewModelAsync(licApps, _dataverse, _cache));
            }

            var transferredLicences = await LicenseExtensions.GetPaidLicenseSummariesOnTransferAsync(_dataverse, userSettings.AccountId, _cache);
            adoxioLicences.AddRange(transferredLicences);

            foreach (var lic in adoxioLicences)
                lic.ChecklistConclusivelyDeem = await isConclusivelyDeemedAsync(lic);

            return adoxioLicences;
        }

        [HttpGet("outstanding-prior-balance-invoice")]
        public async Task<JsonResult> GetCurrentUserOutstandingPriorBalanceInvoices()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var adoxioApplications = await GetCurrentUserOutstandingPriorBalanceInvoiceApplicationAsync(userSettings.AccountId);
            return new JsonResult(adoxioApplications);
        }

        private async Task<List<OutstandingParioBalanceInvoice>> GetCurrentUserOutstandingPriorBalanceInvoiceApplicationAsync(string applicantId)
        {
            var results = new List<OutstandingParioBalanceInvoice>();
            var appType = await _dataverse.GetApplicationTypeByNameAsync("Outstanding Prior Balance Invoice - LIQ");
            if (appType == null) return results;

            var applications = await _dataverse.GetApplicationsByApplicantTypeAndStatusesAsync(
                applicantId,
                appType.adoxio_applicationtypeId!.Value.ToString(),
                new List<int> { (int)AdoxioApplicationStatusCodes.PendingForLicenceFee });

            DateTime today = DateTime.Now;
            foreach (var app in applications)
            {
                if (app.adoxio_Invoice == null) continue;
                var invoice = await _dataverse.GetInvoiceByIdAsync(app.adoxio_Invoice.Id.ToString());
                if (invoice == null || invoice.StatusCode == invoice_statuscode.Complete) continue;

                var temp = new OutstandingParioBalanceInvoice
                {
                    invoice = invoice.ToViewModel(),
                    applicationId = app.Id.ToString()
                };

                if (invoice.DueDate.HasValue)
                {
                    var d = invoice.DueDate.Value;
                    var offset = today.IsDaylightSavingTime() ? "-08:00" : "-07:00";
                    temp.invoice.duedate = DateTime.Parse($"{d.Year}-{d.Month}-{d.Day}T00:00:00.0000000{offset}");
                    temp.overdue = temp.invoice.duedate <= today;
                }

                if (app.adoxio_AssignedLicence != null)
                {
                    var lic = await _dataverse.GetLicenceByIdAsync(app.adoxio_AssignedLicence.Id.ToString());
                    temp.licenceNumber = lic?.adoxio_LicenceNumber;
                }

                results.Add(temp);
            }

            return results;
        }

        private async Task<bool> isConclusivelyDeemedAsync(ApplicationLicenseSummary lic)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            var appType = await _dataverse.GetApplicationTypeByNameAsync("Liquor Licence Transfer");
            if (appType == null) return false;

            var excludeStatuses = new List<int>
            {
                (int)AdoxioApplicationStatusCodes.Processed,
                (int)AdoxioApplicationStatusCodes.Terminated,
                (int)AdoxioApplicationStatusCodes.Cancelled,
                (int)AdoxioApplicationStatusCodes.Approved,
                (int)AdoxioApplicationStatusCodes.Refused,
                (int)AdoxioApplicationStatusCodes.TerminatedAndRefunded
            };

            var apps = await _dataverse.GetApplicationsByApplicantAndTypeAsync(
                userSettings.AccountId, appType.adoxio_applicationtypeId!.Value.ToString(), excludeStatuses, requireStatecode0: true);

            var transferApp = apps.FirstOrDefault(a => a.adoxio_AssignedLicence?.Id.ToString() == lic.LicenseId);
            return transferApp?.adoxio_ChecklistConclusivelyDeem == adoxio_application_adoxio_checklistconclusivelydeem.Yes;
        }

        /// GET all licenses in Dynamics by Licencee using the account Id assigned to the user logged in
        [HttpGet("third-party-operator")]
        public async Task<JsonResult> GetThirdPartyOperatedLicencesAsync()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            var licences = await _dataverse.GetLicencesByThirdPartyOperatorAsync(userSettings.AccountId);
            var summaries = new List<ApplicationLicenseSummary>();
            foreach (var lic in licences)
            {
                summaries.Add(await lic.ToLicenseSummaryViewModelAsync(new List<adoxio_application>(), _dataverse, _cache));
            }
            foreach (var lic in summaries)
                lic.ChecklistConclusivelyDeem = await isConclusivelyDeemedAsync(lic);

            return new JsonResult(summaries);
        }

        /// GET all proposed licenses in Dynamics by Licencee using the account Id assigned to the user logged in
        [HttpGet("proposed-owner")]
        public async Task<JsonResult> GetProposedLicenseeLicences()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            var licences = await _dataverse.GetLicencesByProposedOwnerAsync(userSettings.AccountId);
            var summaries = new List<ApplicationLicenseSummary>();
            foreach (var lic in licences)
            {
                var licId = lic.adoxio_licencesId?.ToString();
                var allApps = await _dataverse.GetApplicationsForLicenceByApplicantAsync(lic.adoxio_Licencee?.Id.ToString() ?? "");
                var licApps = allApps.Where(app => app.adoxio_AssignedLicence?.Id.ToString() == licId).ToList();
                summaries.Add(await lic.ToLicenseSummaryViewModelAsync(licApps, _dataverse, _cache));
            }
            foreach (var lic in summaries)
                lic.ChecklistConclusivelyDeem = await isConclusivelyDeemedAsync(lic);

            return new JsonResult(summaries);
        }

        /// GET all licenses in Dynamics filtered by the GUID of the licencee
        [HttpGet("licencee/{licenceeId}")]
        public async Task<JsonResult> GetDynamicsLicenses(string licenceeId)
        {
            var licences = await _dataverse.GetLicencesByAccountIdAsync(licenceeId);
            var allApps = await _dataverse.GetApplicationsForLicenceByApplicantAsync(licenceeId);
            var summaries = new List<ApplicationLicenseSummary>();
            foreach (var lic in licences)
            {
                var licId = lic.adoxio_licencesId?.ToString();
                var licApps = allApps.Where(app => app.adoxio_AssignedLicence?.Id.ToString() == licId).ToList();
                summaries.Add(await lic.ToLicenseSummaryViewModelAsync(licApps, _dataverse, _cache));
            }
            return new JsonResult(summaries);
        }

        /// GET a licence as PDF.
        [AllowAnonymous]
        [HttpGet("{licenceId}/pdf/{filename}")]
        public async Task<IActionResult> GetLicencePDF(string licenceId, string filename)
        {
            var adoxioLicense = await _dataverse.GetLicenceByIdWithChildrenAsync(licenceId);
            if (adoxioLicense == null)
            {
                throw new Exception("Error getting license.");
            }

            var effectiveDateParam = "";
            if (adoxioLicense.adoxio_EffectiveDate.HasValue)
            {
                effectiveDateParam = adoxioLicense.adoxio_EffectiveDate.Value.ToString("MMMM dd, yyyy");
            }

            var expiraryDateParam = "";
            if (adoxioLicense.adoxio_ExpiryDate.HasValue)
            {
                expiraryDateParam = adoxioLicense.adoxio_ExpiryDate.Value.ToString("MMMM dd, yyyy");
            }

            var licenceTermsAndConditions = await _dataverse.GetTermsConditionsByLicenceIdAsync(licenceId);

            var termsAndConditions = "";
            foreach (var tcItem in licenceTermsAndConditions)
            {
                termsAndConditions += $"<li>{tcItem.adoxio_TermsandConditions}</li>";
            }

            var thirdPartyText = "";
            if (adoxioLicense.adoxio_ThirdPartyOperatorId != null)
            {
                thirdPartyText = $"<tr><td>Third Party Operator</td><td>{adoxioLicense.adoxio_ThirdPartyOperatorId.Name}</td></tr>";
            }

            var serviceAreaText = "";
            var endorsementsText = "";
            License licenceVM = await adoxioLicense.ToViewModelAsync(_dataverse);

            var licenceHasSEA = -1;
            var licenceHasLounge = -1;
            var licenceHasStore = -1;
            var licenceHasCatering = -1;
            var licenceHasOffsite = -1;
            var licenceHasPPEE = -1;
            var licenceHasTUA = -1;
            var licenceHasPicnic = -1;
            var licenceHasTempOffsite = -1;

            if (licenceVM.Endorsements != null && licenceVM.Endorsements.Count > 0)
            {
                licenceHasSEA = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Special Event Area Endorsement");
                licenceHasLounge = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Lounge Area Endorsement");
                licenceHasStore = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "On-Site Store Endorsement");
                licenceHasCatering = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Catering Endorsement");
                licenceHasOffsite = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Off-Site Store Endorsement");
                licenceHasPPEE = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Patron Participation Entertainment Endorsement");
                licenceHasTUA = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Temporary Use Area Endorsement");
                licenceHasPicnic = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Picnic Area Endorsement");
                licenceHasTempOffsite = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Temporary Off-Site Sales Endorsement");

                if (licenceHasSEA > -1)
                {
                    endorsementsText += await licenceVM.Endorsements[licenceHasSEA].ToHtmlAsync(_dataverse);
                }

                if (licenceHasLounge > -1)
                {
                    endorsementsText += await licenceVM.Endorsements[licenceHasLounge].ToHtmlAsync(_dataverse);
                }

                if (licenceHasStore > -1)
                {
                    endorsementsText += licenceVM.Endorsements[licenceHasStore].SimpleHeader();
                }

                if (licenceHasCatering > -1)
                {
                    endorsementsText += licenceVM.Endorsements[licenceHasCatering].SimpleHeader();
                }

                if (licenceHasOffsite > -1)
                {
                    endorsementsText += licenceVM.Endorsements[licenceHasCatering].SimpleHeader();
                }

                if (licenceHasPPEE > -1)
                {
                    endorsementsText += licenceVM.Endorsements[licenceHasPPEE].SimpleHeader();
                }

                if (licenceHasTUA > -1)
                {
                    endorsementsText += licenceVM.Endorsements[licenceHasTUA].SimpleHeader();
                }

                if (licenceHasPicnic > -1)
                {
                    endorsementsText += licenceVM.Endorsements[licenceHasPicnic].SimpleHeader();
                }

                if (licenceHasTempOffsite > -1)
                {
                    endorsementsText += licenceVM.Endorsements[licenceHasTempOffsite].SimpleHeader();
                }
            }

            if (licenceHasSEA < 0 && licenceHasLounge < 0)
            {
                IList<DV::Gov.Lclb.Cllb.Interfaces.adoxio_servicearea> allServiceAreas = null;

                try
                {
                    allServiceAreas = await _dataverse.GetServiceAreasByLicenceIdAsync(licenceId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error loading service areas for {adoxioLicense.adoxio_name}");
                }

                if (allServiceAreas != null && allServiceAreas.Count > 0)
                {
                    var filteredServiceAreas = allServiceAreas
                        .Where(area => area.adoxio_areacategory != DV::Gov.Lclb.Cllb.Interfaces.adoxio_servicearea_adoxio_areacategory.No)
                        .Where(area => area.adoxio_arealocation != null && area.adoxio_capacity != null)
                        .Where(area => area.adoxio_TemporaryExtensionArea != true)
                        .OrderBy(area => area.adoxio_areanumber);

                    serviceAreaText += $@"<h3 style=""text-align: center;"">CAPACITY</h3>";
                    serviceAreaText += "<table style='border: black 0px; padding:2px; border-collapse: separate; border-spacing: 2px;'><tr>";

                    var cells = 0;
                    var leftover = 0;

                    foreach (var area in filteredServiceAreas)
                    {
                        cells++;

                        serviceAreaText += $@"<td class='area'><table style='padding:0px; margin: 0px; width:100%; border: 0px solid white;'><tr><td>{area.adoxio_arealocation}</td><td>{area.adoxio_capacity}</td></tr></table></td>";

                        leftover = cells % 4;

                        if (leftover == 0)
                        {
                            serviceAreaText += "</tr><tr>";
                        }
                    }

                    for (int i = 0; i < leftover; i++)
                    {
                        serviceAreaText += "<td class='space'>&nbsp;</td>";
                    }

                    serviceAreaText += "</tr></table>";
                }
            }

            var storeHours = "";

            var hoursList = await _dataverse.GetHoursOfSaleByLicenceIdNoEndorsementAsync(licenceId);

            if (hoursList.Count > 0 &&
                adoxioLicense.adoxio_LicenceType?.Name != "Wine Store" &&
                adoxioLicense.adoxio_LicenceType?.Name != "Licensee Retail Store" &&
                adoxioLicense.adoxio_LicenceType?.Name != "Rural Licensee Retail Store")
            {
                var hoursVal = hoursList.First();

                storeHours = $@"<h3 style=""text-align: center;"">HOURS OF SALE</h3>
                        <table style=""width: 100%"">
                        <tr>
                            <th></th>
                            <th>Monday</th>
                            <th>Tuesday</th>
                            <th>Wednesday</th>
                            <th>Thursday</th>
                            <th>Friday</th>
                            <th>Saturday</th>
                            <th>Sunday</th>
                        </tr>

                        <tr>
                            <td class='hours'>Start</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_MondayOpen)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_TuesdayOpen)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_WednesdayOpen)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_ThursdayOpen)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_FridayOpen)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_SaturdayOpen)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_SundayOpen)}</td>
                        </tr>
                        <tr>
                            <td class='hours'>End</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_MondayClose)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_TuesdayClose)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_WednesdayClose)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_ThursdayClose)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_FridayClose)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_SaturdayClose)}</td>
                            <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_SundayClose)}</td>
                        </tr></table>";
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            var licenceType = adoxioLicense.adoxio_LicenceType?.Name;
            if (licenceType == LicenceType.Manufacturer.ToString() && !string.IsNullOrEmpty(licenceVM.LicenseSubCategory))
            {
                licenceType = licenceVM.LicenseSubCategory;
            }

            parameters.Add("licenceNumber", adoxioLicense.adoxio_LicenceNumber);
            parameters.Add("licencee", adoxioLicense.adoxio_Licencee?.Name);
            parameters.Add("thirdPartyText", thirdPartyText);
            parameters.Add("serviceAreaText", serviceAreaText);
            parameters.Add("licenceType", licenceType);
            parameters.Add("effectiveDate", effectiveDateParam);
            parameters.Add("expiryDate", expiraryDateParam);
            parameters.Add("restrictionsText", termsAndConditions);
            parameters.Add("endorsementsText", endorsementsText);
            parameters.Add("storeHours", storeHours);
            parameters.Add("printDate", DateTime.Today.ToString("MMMM dd, yyyy"));

            switch (adoxioLicense.adoxio_LicenceType?.Name)
            {
                case "Marketing":
                case "Agent":
                    var licenceeAcct = await _dataverse.GetAccountByIdAsync(adoxioLicense.adoxio_Licencee?.Id.ToString());
                    parameters.Add("establishmentName", "N/A");
                    parameters.Add("establishmentStreet", licenceeAcct?.Address1_Line1);
                    parameters.Add("establishmentCity", licenceeAcct?.Address1_City);
                    parameters.Add("establishmentPostalCode", licenceeAcct?.Address1_PostalCode);
                    break;
                default:
                    parameters.Add("establishmentName", adoxioLicense.adoxio_establishment?.Name);
                    parameters.Add("licenceName", adoxioLicense.adoxio_establishment?.Name);
                    parameters.Add("establishmentStreet", adoxioLicense.adoxio_EstablishmentAddressStreet);
                    parameters.Add("establishmentCity", adoxioLicense.adoxio_EstablishmentAddressCity + ", B.C.");
                    parameters.Add("establishmentPostalCode", adoxioLicense.adoxio_EstablishmentAddressPostalCode);
                    break;
            }

            switch (adoxioLicense.adoxio_LicenceType?.Name)
            {
                case "Section 119 Authorization":
                case "S119 CRS Authorization":
                case "Marketing":
                case "Cannabis Retail Store":
                    parameters.Add("keyWord", "Cannabis");
                    break;
                default:
                    parameters.Add("keyWord", "Liquor");
                    break;
            }

            switch (adoxioLicense.adoxio_LicenceType?.Name)
            {
                case "Section 119 Authorization":
                case "S119 CRS Authorization":
                    parameters.Add("dType", "Authorization");
                    break;
                default:
                    parameters.Add("dType", "Licence");
                    break;
            }

            try
            {
                var templateName = "liquor_licence";
                byte[] data = await _pdfClient.GetPdf(parameters, templateName);

                try
                {
                    var hash = await _pdfClient.GetPdfHash(parameters, templateName);
                    var entityName = "licence";
                    var entityId = adoxioLicense.adoxio_licencesId?.ToString();
                    var folderName = await _dataverse.GetFolderNameAsync(entityName, entityId);
                    var documentType = "Licence";
                    _fileManagerClient.UploadPdfIfChanged(_logger, entityName, entityId, folderName, documentType, data, hash);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error uploading PDF");
                }

                return File(data, "application/pdf", $"{adoxioLicense.adoxio_LicenceNumber}.pdf");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error returning PDF response");
                return new NotFoundResult();
            }
        }

        [HttpPut("{licenceId}/ldbordertotals")]
        public async Task<IActionResult> UpdateLicenceLDBOrderTotals([FromBody] int total, string licenceId)
        {
            if (string.IsNullOrEmpty(licenceId))
            {
                return BadRequest();
            }

            var licence = await _dataverse.GetLicenceByIdWithChildrenAsync(licenceId);
            if (licence == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(licence.adoxio_Licencee?.Id.ToString()) &&
                !CurrentUserHasAccessToLicenseTransferredTo(licence.adoxio_ProposedOwner?.Id.ToString()))
            {
                return Forbid();
            }

            var patch = new adoxio_licences
            {
                Id = new Guid(licenceId),
                adoxio_LDBOrderTotals = (decimal?)total
            };
            await _dataverse.UpdateLicenceAsync(patch);

            return Ok();
        }

        [HttpPut("{licenceId}/establishment")]
        public async Task<IActionResult> UpdateLicenceEstablishment([FromBody] ApplicationLicenseSummary item, string licenceId)
        {
            if (item == null || string.IsNullOrEmpty(licenceId) || licenceId != item.LicenseId)
            {
                return BadRequest();
            }

            var licence = await _dataverse.GetLicenceByIdWithChildrenAsync(licenceId);
            if (licence == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(licence.adoxio_Licencee?.Id.ToString()))
            {
                return Forbid();
            }

            var patch = new adoxio_licences
            {
                Id = new Guid(licenceId),
                adoxio_EstablishmentPhone = item.EstablishmentPhoneNumber,
                adoxio_EstablishmentAddressCity = item.EstablishmentAddressCity,
                adoxio_EstablishmentAddressStreet = item.EstablishmentAddressStreet,
                adoxio_EstablishmentAddressPostalCode = item.EstablishmentAddressPostalCode
            };
            await _dataverse.UpdateLicenceAsync(patch);

            licence = await _dataverse.GetLicenceByIdWithChildrenAsync(licenceId);
            var allApps = await _dataverse.GetApplicationsForLicenceByApplicantAsync(licence.adoxio_Licencee?.Id.ToString() ?? "");
            var licenceApps = allApps.Where(app => app.adoxio_AssignedLicence?.Id.ToString() == licenceId).ToList();

            return new JsonResult(await licence.ToLicenseSummaryViewModelAsync(licenceApps, _dataverse, _cache));
        }

        private bool CurrentUserHasAccessToLicenseOwnedBy(string accountId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return userSettings.AccountId == accountId;
            }

            return false;
        }

        private bool CurrentUserHasAccessToLicenseTransferredTo(string accountId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return userSettings.AccountId == accountId;
            }

            return false;
        }
    }

    public class LicenceTransfer
    {
        public string AccountId { get; set; }
        public string LicenceId { get; set; }
    }

    public class TiedHouseExcemptionRequest
    {
        public string RelatedLicenceId { get; set; }
        public string LicenceId { get; set; }

        public int? ManufacturerProductionAmountforPrevYear { get; set; }
        public int? ManufacturerProductionAmountUnit { get; set; }
    }
}
