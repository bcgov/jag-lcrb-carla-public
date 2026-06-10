extern alias DV;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
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
using Microsoft.Rest;
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

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LicensesController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IDataverseClient _dataverse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPdfService _pdfClient;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;
        private readonly FileManagerClient _fileManagerClient;

        public LicensesController(IDynamicsClient dynamicsClient, IDataverseClient dataverse,
            IHttpContextAccessor httpContextAccessor, IPdfService pdfClient,
            ILoggerFactory loggerFactory, IMemoryCache memoryCache, IWebHostEnvironment env,
            FileManagerClient fileClient)
        {
            _cache = memoryCache;
            _dynamicsClient = dynamicsClient;
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
        public List<RelatedLicence> GetAutocomplete(string name = null, string licenceNumber = null)
        {
            var results = new List<RelatedLicence>();

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(licenceNumber))
            {
                return results;
            }

            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var filter = $"statecode eq 0";

                    List<string> orClauses = new List<string>();

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        orClauses.Add($"contains(adoxio_name,'{name.Replace("'", "''")}')");
                    }

                    if (!string.IsNullOrWhiteSpace(licenceNumber))
                    {
                        orClauses.Add($"contains(adoxio_LicenceNumber,'{licenceNumber.Replace("'", "''")}')");
                    }

                    string orClause = string.Join(" or ", orClauses);
                    filter = $"{filter} and ({orClause})";

                    var expand = new List<string> { "adoxio_Licencee", "adoxio_establishment" };

                    var licences = _dynamicsClient.Licenceses.Get(filter: filter, expand: expand, top: 10).Value;

                    foreach (var licence in licences)
                    {
                        var relatedLicence = new RelatedLicence
                        {
                            Id = licence.AdoxioLicencesid,
                            Name = licence.AdoxioName,
                            EstablishmentName = licence.AdoxioEstablishment?.AdoxioName,
                            Streetaddress = licence.AdoxioEstablishment?.AdoxioAddressstreet,
                            City = licence.AdoxioEstablishment?.AdoxioAddresscity,
                            Provstate = "BC",
                            Country = "CANADA",
                            PostalCode = licence.AdoxioEstablishment?.AdoxioAddresspostalcode,
                            Licensee = licence.AdoxioLicencee?.Name,
                            LicenceNumber = licence.AdoxioLicencenumber,
                            Valid = true
                        };

                        results.Add(relatedLicence);
                    }
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error while getting autocomplete data.");
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
        public ActionResult InitiateTiedHouseExcemption(TiedHouseExcemptionRequest item)
        {
            if (!ModelState.IsValid ||
                string.IsNullOrEmpty(item.LicenceId) ||
                string.IsNullOrEmpty(item.RelatedLicenceId))
            {
                return BadRequest();
            }

            // check access to licence
            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.GetLicenceByIdWithChildren(item.LicenceId);
            if (adoxioLicense == null)
            {
                return NotFound();
            }
            MicrosoftDynamicsCRMadoxioLicences relatedLicence = _dynamicsClient.GetLicenceByIdWithChildren(item.RelatedLicenceId);
            if (!CurrentUserHasAccessToLicenseOwnedBy(relatedLicence._adoxioLicenceeValue))
            {
                return Forbid();
            }

            var application = CreateApplication(item.LicenceId, ApplicationTypeNames.TiedHouseExemption, item.RelatedLicenceId, item.ManufacturerProductionAmountforPrevYear, item.ManufacturerProductionAmountUnit);

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

        private MicrosoftDynamicsCRMadoxioApplication CreateApplication(string licenceId, string applicationTypeName, string relatedLicenceId = null, int? prodAmount = null, int? prodUnit = null)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.GetLicenceByIdWithChildren(licenceId);
            if (adoxioLicense == null)
            {
                throw new Exception("Error getting license.");
            }

            MicrosoftDynamicsCRMadoxioApplication application = new MicrosoftDynamicsCRMadoxioApplication();

            application.CopyValuesForChangeOfLocation(adoxioLicense, applicationTypeName != "CRS Location Change");

            application.AdoxioApplicanttype = adoxioLicense.AdoxioLicencee.AdoxioBusinesstype;

            var applicationType = _dynamicsClient.GetApplicationTypeByName(applicationTypeName);
            application.AdoxioApplicationTypeIdODataBind = _dynamicsClient.GetEntityURI("adoxio_applicationtypes", applicationType.AdoxioApplicationtypeid);

            if (adoxioLicense.AdoxioLicenceType != null)
            {
                application.AdoxioLicenceTypeODataBind = _dynamicsClient.GetEntityURI("adoxio_licencetypes", adoxioLicense.AdoxioLicenceType.AdoxioLicencetypeid);
            }

            if (adoxioLicense.AdoxioLicencesid != null)
            {
                application.AdoxioAssignedLicenceODataBind = _dynamicsClient.GetEntityURI("adoxio_licenceses", adoxioLicense.AdoxioLicencesid);
            }

            if (adoxioLicense.AdoxioLicenceSubCategoryId != null)
            {
                application.AdoxioLicenceSubCategoryODataBind =
                    _dynamicsClient.GetEntityURI("adoxio_licencesubcategories",
                        adoxioLicense.AdoxioLicenceSubCategoryId.AdoxioLicencesubcategoryid);
            }

            application.AdoxioApplicantODataBind = _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId);

            if (adoxioLicense.AdoxioEstablishment != null)
            {
                application.AdoxioLicenceEstablishmentODataBind = _dynamicsClient.GetEntityURI("adoxio_establishments", adoxioLicense.AdoxioEstablishment.AdoxioEstablishmentid);
            }

            application.AdoxioManufacturerproductionamountforprevyear = prodAmount;
            application.AdoxioManufacturerproductionamountunit = prodUnit;

            if (relatedLicenceId != null)
            {
                application.AdoxioEstablishmentaddressstreet = adoxioLicense.AdoxioEstablishment.AdoxioAddressstreet;
                application.AdoxioEstablishmentaddresscity = adoxioLicense.AdoxioEstablishment.AdoxioAddresscity;
                application.AdoxioEstablishmentaddresspostalcode = adoxioLicense.AdoxioEstablishment.AdoxioAddresspostalcode;

                application.AdoxioRelatedLicenceODataBind = _dynamicsClient.GetEntityURI("adoxio_licenceses", relatedLicenceId);

                application.AdoxioApplicantODataBind = _dynamicsClient.GetEntityURI("accounts", adoxioLicense._adoxioLicenceeValue);
            }

            try
            {
                var licenceApp = adoxioLicense?.AdoxioAdoxioLicencesAdoxioApplicationAssignedLicence?.Where(app => !string.IsNullOrEmpty(app._adoxioLocalgovindigenousnationidValue)).FirstOrDefault();
                string lginvalue = "";

                if (licenceApp == null)
                {
                    if (adoxioLicense?._adoxioLginValue != null)
                    {
                        lginvalue = adoxioLicense?._adoxioLginValue;
                    }
                    else
                    {
                        if (adoxioLicense?.AdoxioEstablishment != null)
                        {
                            lginvalue = adoxioLicense?.AdoxioEstablishment._adoxioLginValue;
                        }
                    }
                }
                else
                {
                    lginvalue = licenceApp._adoxioLocalgovindigenousnationidValue;
                }

                if (!string.IsNullOrEmpty(lginvalue))
                {
                    application.AdoxioLocalgovindigenousnationidODataBind = _dynamicsClient.GetEntityURI("adoxio_localgovindigenousnations", lginvalue);
                }

                licenceApp = adoxioLicense?.AdoxioAdoxioLicencesAdoxioApplicationAssignedLicence?.Where(app => !string.IsNullOrEmpty(app._adoxioPolicejurisdictionidValue)).FirstOrDefault();
                if (!string.IsNullOrEmpty(licenceApp?._adoxioPolicejurisdictionidValue))
                {
                    application.AdoxioPoliceJurisdictionIdODataBind = _dynamicsClient.GetEntityURI("adoxio_policejurisdictions", licenceApp?._adoxioPolicejurisdictionidValue);
                }

                application = _dynamicsClient.Applications.Create(application);
            }
            catch (HttpOperationException httpOperationException)
            {
                string applicationId = _dynamicsClient.GetCreatedRecord(httpOperationException, null);
                if (!string.IsNullOrEmpty(applicationId) && Guid.TryParse(applicationId, out Guid applicationGuid))
                {
                    application = _dynamicsClient.GetApplicationById(applicationGuid).GetAwaiter().GetResult();
                }
                else
                {
                    _logger.LogError(httpOperationException, "Error creating application");
                    throw httpOperationException;
                }
            }

            return application;
        }

        /// Create a change of location application
        [HttpPost("{licenceId}/create-action-application")]
        public async Task<IActionResult> CreateApplicationForAction(string licenceId, [FromQuery] string applicationType)
        {
            if (string.IsNullOrEmpty(applicationType)) return BadRequest();

            var application = CreateApplication(licenceId, applicationType);
            var result = await application.ToViewModel(_dynamicsClient, _cache, _logger);
            return new JsonResult(result);
        }

        private MicrosoftDynamicsCRMadoxioApplication GetTermChangeApplication(string licenceId, string termId, string applicationTypeName)
        {
            MicrosoftDynamicsCRMadoxioApplication result = null;

            var applicationType = _dynamicsClient.GetApplicationTypeByName(applicationTypeName);

            if (applicationType != null)
            {
                string filter =
                      $"_adoxio_applicationtypeid_value eq {applicationType.AdoxioApplicationtypeid}"
                    + $" and _adoxio_assignedlicence_value eq {licenceId}"
                    + " and statecode eq 0"
                    + $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Processed}"
                    + $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Terminated}"
                    + $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Cancelled}"
                    + $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Approved}"
                    + $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Refused}"
                    + $" and statuscode ne {(int)AdoxioApplicationStatusCodes.TerminatedAndRefunded}";

                try
                {
                    var items = _dynamicsClient.Applications.Get(filter: filter).Value;
                    foreach (var item in items)
                    {
                        var candidate = _dynamicsClient.GetApplicationByIdWithChildren(item.AdoxioApplicationid).GetAwaiter().GetResult();
                        if (candidate.AdoxioAdoxioApplicationAdoxioApplicationtermsconditionslimitationApplication != null && candidate.AdoxioAdoxioApplicationAdoxioApplicationtermsconditionslimitationApplication.Count > 0)
                        {
                            foreach (var term in candidate
                                .AdoxioAdoxioApplicationAdoxioApplicationtermsconditionslimitationApplication)
                            {
                                if (termId == term.AdoxioApplicationtermsconditionslimitationid)
                                {
                                    result = candidate;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.LogError(httpOperationException, "Error getting application");
                }
            }

            return result;
        }

        /// Create a change of location application
        [HttpPost("{licenceId}/create-action-application-term/{termId}")]
        public async Task<IActionResult> CreateOrGetApplicationWithTerm(string licenceId, string termId,
            [FromQuery] string applicationType)
        {
            if (string.IsNullOrEmpty(applicationType)) return BadRequest();

            var application = GetTermChangeApplication(licenceId, termId, applicationType);

            if (application == null)
            {
                application = CreateApplication(licenceId, applicationType);

                if (!string.IsNullOrEmpty(termId))
                {
                    Odataid odataId = new Odataid()
                    {
                        OdataidProperty =
                            _dynamicsClient.GetEntityURI("adoxio_applicationtermsconditionslimitations", termId)
                    };

                    try
                    {
                        await _dynamicsClient.Applications.AddReferenceWithHttpMessagesAsync(
                            application.AdoxioApplicationid,
                            "adoxio_adoxio_application_adoxio_applicationtermsconditionslimitation_Application",
                            odataid: odataId);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error updating application with reference to term");
                    }
                }
            }
            var result = await application.ToViewModel(_dynamicsClient, _cache, _logger);
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

            List<ApplicationLicenseSummary> transferredLicences = _dynamicsClient.GetPaidLicensesOnTransfer(userSettings.AccountId);
            adoxioLicences.AddRange(transferredLicences);

            adoxioLicences.ForEach(lic =>
            {
                lic.ChecklistConclusivelyDeem = isConclusivelyDeemed(lic);
            });

            return adoxioLicences;
        }

        [HttpGet("outstanding-prior-balance-invoice")]
        public JsonResult GetCurrentUserOutstandingPriorBalanceInvoices()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var adoxioApplications = GetCurrentUserOutstandingPriorBalanceInvoiceApplication(userSettings.AccountId);
            return new JsonResult(adoxioApplications);
        }

        private List<OutstandingParioBalanceInvoice> GetCurrentUserOutstandingPriorBalanceInvoiceApplication(string applicantId)
        {
            var results = new List<OutstandingParioBalanceInvoice>();
            var filter = $"_adoxio_applicant_value eq {applicantId}";
            var appType = _dynamicsClient.GetApplicationTypeByName("Outstanding Prior Balance Invoice - LIQ");
            if (appType == null) return results;
            filter += $" and _adoxio_applicationtypeid_value eq {appType.AdoxioApplicationtypeid} ";
            filter += $" and statuscode eq {(int)AdoxioApplicationStatusCodes.PendingForLicenceFee}";
            var expand = new List<string>
                    {
                        "adoxio_Invoice",
                        "adoxio_AssignedLicence"
                    };
            try
            {
                var applications = _dynamicsClient.Applications.Get(filter: filter, expand: expand).Value.ToList();
                if (applications != null)
                {
                    DateTime today = DateTime.Now;
                    foreach (var dynamicsApplication in applications)
                    {
                        if (dynamicsApplication.AdoxioInvoice != null && dynamicsApplication.AdoxioInvoice.Statuscode != 100001)
                        {
                            var temp = new OutstandingParioBalanceInvoice();
                            temp.invoice = dynamicsApplication.AdoxioInvoice.ToViewModel();
                            if (dynamicsApplication.AdoxioInvoice.Duedate != null)
                            {
                                if (today.IsDaylightSavingTime())
                                {
                                    temp.invoice.duedate = DateTime.Parse(dynamicsApplication.AdoxioInvoice.Duedate.Value.Year + "-" + dynamicsApplication.AdoxioInvoice.Duedate.Value.Month + "- " + dynamicsApplication.AdoxioInvoice.Duedate.Value.Day + "T00:00:00.0000000-08:00");
                                }
                                else
                                {
                                    temp.invoice.duedate = DateTime.Parse(dynamicsApplication.AdoxioInvoice.Duedate.Value.Year + "-" + dynamicsApplication.AdoxioInvoice.Duedate.Value.Month + "- " + dynamicsApplication.AdoxioInvoice.Duedate.Value.Day + "T00:00:00.0000000-07:00");
                                }
                                temp.overdue = temp.invoice.duedate <= today;
                            }
                            temp.applicationId = dynamicsApplication.AdoxioApplicationid;
                            if (dynamicsApplication.AdoxioAssignedLicence != null)
                            {
                                temp.licenceNumber = dynamicsApplication.AdoxioAssignedLicence.AdoxioLicencenumber;
                            }
                            results.Add(temp);
                        }
                    }
                }
            }
            catch (HttpOperationException e)
            {
                _logger.LogError(e, "Error getting licensee application");
                throw;
            }

            return results;
        }

        private bool isConclusivelyDeemed(ApplicationLicenseSummary lic)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            var result = false;
            var filter = $"_adoxio_applicant_value eq {userSettings.AccountId}";
            filter += " and statecode eq 0";
            filter += $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Processed}";
            filter += $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Terminated}";
            filter += $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Cancelled}";
            filter += $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Approved}";
            filter += $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Refused}";
            filter += $" and statuscode ne {(int)AdoxioApplicationStatusCodes.TerminatedAndRefunded}";

            var applicationType = _dynamicsClient.GetApplicationTypeByName("Liquor Licence Transfer");
            if (applicationType != null)
            {
                filter += $" and _adoxio_assignedlicence_value eq {lic.LicenseId}";
                filter += $" and _adoxio_applicationtypeid_value eq {applicationType.AdoxioApplicationtypeid} ";
                var transferApp = _dynamicsClient.Applications.Get(filter: filter).Value.FirstOrDefault();
                const int yes = 845280000;
                if (transferApp?.AdoxioChecklistconclusivelydeem == yes)
                {
                    result = true;
                }
            }

            return result;
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
            summaries.ForEach(lic =>
            {
                lic.ChecklistConclusivelyDeem = isConclusivelyDeemed(lic);
            });

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
            summaries.ForEach(lic =>
            {
                lic.ChecklistConclusivelyDeem = isConclusivelyDeemed(lic);
            });

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
                    endorsementsText += licenceVM.Endorsements[licenceHasSEA].ToHtml(_dynamicsClient);
                }

                if (licenceHasLounge > -1)
                {
                    endorsementsText += licenceVM.Endorsements[licenceHasLounge].ToHtml(_dynamicsClient);
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
                    // TODO: migrate GetFolderName to Dataverse SDK
                    var folderName = await _dynamicsClient.GetFolderName(entityName, entityId).ConfigureAwait(true);
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
