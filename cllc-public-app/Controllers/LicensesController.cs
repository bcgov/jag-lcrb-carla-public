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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LicensesController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPdfService _pdfClient;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;
        private readonly FileManagerClient _fileManagerClient;

        public LicensesController(IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor,
            IPdfService pdfClient, ILoggerFactory loggerFactory, IMemoryCache memoryCache, IWebHostEnvironment env, FileManagerClient fileClient)
        {
            _cache = memoryCache;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _pdfClient = pdfClient;
            _logger = loggerFactory.CreateLogger(typeof(LicensesController));
            _env = env;
            _fileManagerClient = fileClient;
        }

        /// GET licence by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLicence(string id)
        {
            MicrosoftDynamicsCRMadoxioLicences licence = null;

            try
            {
                // check access to licence
                licence = _dynamicsClient.GetLicenceByIdWithChildren(id);
                if (licence == null)
                {
                    return NotFound();
                }

                if (!CurrentUserHasAccessToLicenseOwnedBy(licence.AdoxioLicencee.Accountid) &&
                    (licence.AdoxioProposedOwner != null && !CurrentUserHasAccessToLicenseTransferredTo(licence.AdoxioProposedOwner.Accountid)))
                {
                    return Forbid();
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting licence by id");
                // fail if we can't create.
                throw (httpOperationException);
            }

            // Create link to sharepoint folder if needed
            if (licence.AdoxioLicencesSharePointDocumentLocations.Count == 0)
            {
                await InitializeSharepoint(licence);
            }

            return new JsonResult(licence.ToViewModel(_dynamicsClient));
        }

        private async Task InitializeSharepoint(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            // create a SharePointDocumentLocation link
            var folderName = licence.GetDocumentFolderName();
            _fileManagerClient.CreateFolderIfNotExist(_logger, LicenceDocumentUrlTitle, folderName);
            _dynamicsClient.CreateEntitySharePointDocumentLocation("licence", licence.AdoxioLicencesid, folderName, folderName);
        }

        [HttpPut("{licenceId}/representative")]
        public async Task<IActionResult> UpdateLicenseeRepresentative([FromBody] ViewModels.ApplicationLicenseSummary item, string licenceId)
        {
            if (item == null || string.IsNullOrEmpty(licenceId) || licenceId != item.LicenseId)
            {
                return BadRequest();
            }

            MicrosoftDynamicsCRMadoxioLicences licence = _dynamicsClient.GetLicenceByIdWithChildren(licenceId);
            if (licence == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(licence.AdoxioLicencee.Accountid))
            {
                return Forbid();
            }

            MicrosoftDynamicsCRMadoxioLicences patchObject = new MicrosoftDynamicsCRMadoxioLicences()
            {
                AdoxioRepresentativename = item.RepresentativeFullName,
                AdoxioRepresentativephone = item.RepresentativePhoneNumber,
                AdoxioRepresentativeemail = item.RepresentativeEmail,
                AdoxioCansubmitpermanentchangeapplications = item.RepresentativeCanSubmitPermanentChangeApplications,
                AdoxioCansigntemporarychangeapplications = item.RepresentativeCanSignTemporaryChangeApplications,
                AdoxioCanobtainlicenceinformation = item.RepresentativeCanObtainLicenceInformation,
                AdoxioCansigngrocerystoreproofofsales = item.RepresentativeCanSignGroceryStoreProofOfSale,
                AdoxioCanattendeducationsessions = item.RepresentativeCanAttendEducationSessions,
                AdoxioCanattendcompliancemeetings = item.RepresentativeCanAttendComplianceMeetings,
                AdoxioCanrepresentathearings = item.RepresentativeCanRepresentAtHearings
            };

            try
            {
                await _dynamicsClient.Licenceses.UpdateAsync(licenceId, patchObject);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating licence representative");
                throw new Exception("Unable to update licence representative");
            }

            try
            {
                licence = _dynamicsClient.GetLicenceByIdWithChildren(licenceId);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting licence");
                throw new Exception("Unable to get licence after update");
            }

            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> applicationsInProgress = _dynamicsClient.GetApplicationsForLicenceByApplicant(licence.AdoxioLicencee.Accountid);
            var applications = applicationsInProgress.Where(app => app._adoxioAssignedlicenceValue == licence.AdoxioLicencesid).ToList();

            licence.AdoxioLicenceType = Models.ApplicationExtensions.GetCachedLicenceType(licence._adoxioLicencetypeValue, _dynamicsClient, _cache);
            return new JsonResult(licence.ToLicenseSummaryViewModel(applications, _dynamicsClient));
        }

        [HttpPut("{licenceId}/offsite-storage")]
        public IActionResult UpdateOffsiteStorageLocations([FromBody] ViewModels.ApplicationLicenseSummary item, string licenceId)
        {
            if (item == null || string.IsNullOrEmpty(licenceId) || licenceId != item.LicenseId)
            {
                return BadRequest();
            }

            MicrosoftDynamicsCRMadoxioLicences licence = _dynamicsClient.GetLicenceByIdWithChildren(licenceId);
            if (licence == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(licence.AdoxioLicencee.Accountid))
            {
                return Forbid();
            }

            try
            {
                // UPDATE the offsite storage locations for this licence
                if (item.OffsiteStorageLocations != null && item.OffsiteStorageLocations.Count > 0)
                {
                    var existingLocations = GetOffsiteLocationsFromLicence(licenceId);
                    foreach (var loc in item.OffsiteStorageLocations.Where(x => x != null))
                    {
                        if (loc.Id == null)
                        {
                            CreateOffsiteStorage(loc, licenceId);
                        }
                        else if (existingLocations.Any(x => x.AdoxioOffsitestorageid == loc.Id))
                        {
                            UpdateOffsiteStorage(loc, licenceId);
                        }
                    }
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating offsite storage");
                throw new Exception("Unable to update offsite storage");
            }

            try
            {
                licence = _dynamicsClient.GetLicenceByIdWithChildren(licenceId);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting licence");
                throw new Exception("Unable to get licence after update");
            }

            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> applicationsInProgress = _dynamicsClient.GetApplicationsForLicenceByApplicant(licence.AdoxioLicencee.Accountid);
            var applications = applicationsInProgress.Where(app => app._adoxioAssignedlicenceValue == licence.AdoxioLicencesid).ToList();

            licence.AdoxioLicenceType = Models.ApplicationExtensions.GetCachedLicenceType(licence._adoxioLicencetypeValue, _dynamicsClient, _cache);
            return new JsonResult(licence.ToLicenseSummaryViewModel(applications, _dynamicsClient));
        }

        // TODO: Remove if not useful
        private void RemoveOffsiteLocationsFromLicence(string licenceId)
        {
            var filter = $"_adoxio_licenceid_value eq {licenceId}";
            try
            {
                var locations = _dynamicsClient.Offsitestorages.Get(filter: filter).Value;
                foreach (var loc in locations)
                {
                    try
                    {
                        _dynamicsClient.Offsitestorages.Delete(loc.AdoxioOffsitestorageid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Unexpected error deleting an offsite location.");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected error deleting a offsite location.");
                    }
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Unexpected error getting offsite locations.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error getting offsite locations.");
            }
        }

        private IList<MicrosoftDynamicsCRMadoxioOffsitestorage> GetOffsiteLocationsFromLicence(string licenceId)
        {
            var locations = new List<MicrosoftDynamicsCRMadoxioOffsitestorage>();
            var filter = $"_adoxio_licenceid_value eq {licenceId}";
            try
            {
                locations.AddRange(_dynamicsClient.Offsitestorages.Get(filter: filter).Value);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Unexpected error getting offsite locations.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error getting offsite locations.");
            }
            return locations;
        }

        private void CreateOffsiteStorage(OffsiteStorage item, string licenceId)
        {
            // We are only interested in new entities here
            if (item.Id != null)
            {
                return;
            }
            var licenceUri = _dynamicsClient.GetEntityURI("adoxio_licenceses", licenceId);
            var dynamicsOffsiteStorage = new MicrosoftDynamicsCRMadoxioOffsitestorage
            {
                LicenceODataBind = licenceUri,
                Statuscode = (int?)OffsiteStorageStatus.Added,
                AdoxioDateadded = DateTimeOffset.Now,
            };
            dynamicsOffsiteStorage.CopyValues(item);
            _dynamicsClient.Offsitestorages.Create(dynamicsOffsiteStorage);
        }

        private void UpdateOffsiteStorage(OffsiteStorage item, string licenceId)
        {
            // We are only interested in existing entities here
            if (item.Id == null)
            {
                return;
            }
            var patchObject = new MicrosoftDynamicsCRMadoxioOffsitestorage();
            patchObject.CopyValues(item);
            _dynamicsClient.Offsitestorages.Update(item.Id, patchObject);
        }

        [HttpPost("cancel-transfer")]
        public ActionResult CancelTransfer(LicenceTransfer item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // check access to licence
            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.GetLicenceByIdWithChildren(item.LicenceId);
            if (adoxioLicense == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.AdoxioLicencee.Accountid) &&
                !CurrentUserHasAccessToLicenseTransferredTo(adoxioLicense.AdoxioProposedOwner.Accountid))
            {
                return Forbid();
            }

            try
            {
                var no = 845280000;
                var patchLicence = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioTransferrequested = no
                };

                // create application
                _dynamicsClient.Licenceses.Update(item.LicenceId, patchLicence);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error cancelling licence transfer");
                // fail if we can't create.
                throw;
            }

            // Delete the Proposed Owner (ProposedOwnerODataBind)
            try
            {
                _dynamicsClient.Licenceses.DeleteReferenceWithHttpMessagesAsync(item.LicenceId, "adoxio_ProposedOwner").GetAwaiter().GetResult();
            }
            catch (HttpOperationException httpOperationException)
            {
                if (httpOperationException.Response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError(httpOperationException, "Error deleting proposed owner");
                    throw;
                }
            }

            // find the related application and delete it.
            foreach (var application in adoxioLicense.AdoxioAdoxioLicencesAdoxioApplicationAssignedLicence)
            {
                // get the full application type.]
                var applicationType = _dynamicsClient.GetApplicationTypeById(application._adoxioApplicationtypeidValue).GetAwaiter().GetResult();
                if (applicationType.AdoxioName.Contains("CRS Transfer of Ownership"))
                {
                    var patchApplication = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        //Statecode = (int?)AdoxioApplicationStatusCodes.Cancelled,
                        Statuscode = (int?)AdoxioApplicationStatusCodes.Terminated
                    };
                    try
                    {
                        _dynamicsClient.Applications.Update(application.AdoxioApplicationid, patchApplication);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error cancelling related application");
                        // fail if we can't create.
                        throw;
                    }

                }
            }


            return Ok();
        }

        [HttpPost("initiate-transfer")]
        public ActionResult InitiateTransfer(LicenceTransfer item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // check access to licence
            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.GetLicenceByIdWithChildren(item.LicenceId);
            if (adoxioLicense == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.AdoxioLicencee.Accountid))
            {
                return Forbid();
            }

            try
            {
                var yes = 845280001;
                var patchLicence = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    ProposedOwnerODataBind = _dynamicsClient.GetEntityURI("accounts", item.AccountId),
                    AdoxioTransferrequested = yes
                };

                // create application
                _dynamicsClient.Licenceses.Update(item.LicenceId, patchLicence);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error initiating licence transfer");
                // fail if we can't create.
                throw (httpOperationException);
            }
            return Ok();
        }

        /// <summary>
        /// Set expiry for a given licence to different dates as specified by workflow GUIDs.  Only useful for automated testing.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{workflowGUID}/setexpiry/{licenceID}")]
        public async Task<IActionResult> SetExpiry(string workflowGUID, string licenceID)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");


            // get the current user.
            string sessionSettings = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(sessionSettings);


            // query the Dynamics system to get the account record.
            if (userSettings.AccountId != null && !userSettings.IsNewUserRegistration && userSettings.AccountId.Length > 0)
            {

                // call the bpf to process the application.
                try
                {
                    // this needs to be the guid for the published workflow.
                    await _dynamicsClient.Workflows.ExecuteWorkflowWithHttpMessagesAsync(workflowGUID, licenceID);
                    return Ok("OK");
                }
                catch (HttpOperationException httpOperationException)
                {
                    string error = httpOperationException.Response.Content;
                    return BadRequest(error);
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            else
            {
                return BadRequest("This API is not available to an unregistered user.");
            }
        }

        /// <summary>
        /// Set autorenewal to 'No' to deny licence renewal for a given licence. Must be preceded by setting licence to 'Expired'. Only useful for automated testing.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("denyautorenew/{licenceID}")]
        public async Task<IActionResult> DenyAutoRenew(string licenceID)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");

            // get the current user.
            string sessionSettings = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(sessionSettings);

            // query the Dynamics system to get the account record.
            if (userSettings.AccountId != null && !userSettings.IsNewUserRegistration && userSettings.AccountId.Length > 0)
            {

                // call the bpf to process the application.
                try
                {
                    // this needs to be the guid for the published workflow.
                    await _dynamicsClient.Workflows.ExecuteWorkflowWithHttpMessagesAsync("e1792ccf-e40b-491f-9a9a-ee8e977749e6", licenceID);
                    return Ok("OK");
                }
                catch (HttpOperationException httpOperationException)
                {
                    string error = httpOperationException.Response.Content;
                    return BadRequest(error);
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            else
            {
                return BadRequest("This API is not available to an unregistered user.");
            }
        }

        [HttpPost("set-third-party-operator")]
        public ActionResult SetThirdPartyOperator(LicenceTransfer item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // check access to licence
            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.GetLicenceByIdWithChildren(item.LicenceId);
            if (adoxioLicense == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.AdoxioLicencee.Accountid) &&
                !CurrentUserHasAccessToLicenseTransferredTo(adoxioLicense.AdoxioProposedOwner.Accountid))
            {
                return Forbid();
            }

            try
            {
                var patchLicence = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    adoxio_ThirdPartyOperatorIdODataBind = _dynamicsClient.GetEntityURI("accounts", item.AccountId),
                    AdoxioTporequested = (int)EnumYesNo.Yes
                };

                // create application
                _dynamicsClient.Licenceses.Update(item.LicenceId, patchLicence);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error initiating licence transfer");
                // fail if we can't create.
                throw (httpOperationException);
            }
            return Ok();
        }

        // handle cancel-operator-application
        // to: validate working
        [HttpPost("cancel-operator-application")]
        public ActionResult CancelTPO(LicenceTransfer item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // check access to licence
            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.GetLicenceByIdWithChildren(item.LicenceId);
            if (adoxioLicense == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.AdoxioLicencee.Accountid) &&
                !CurrentUserHasAccessToLicenseTransferredTo(adoxioLicense.AdoxioProposedOwner.Accountid))
            {
                return Forbid();
            }

            try
            {
                var patchLicence = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioTporequested = (int)EnumYesNo.No
                };

                // create application
                _dynamicsClient.Licenceses.Update(item.LicenceId, patchLicence);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error cancelling Third Party Application");
                // fail if we can't create.
                throw;
            }

            // Delete the Proposed Owner (ProposedOwnerODataBind)
            try
            {
                _dynamicsClient.Licenceses.DeleteReferenceWithHttpMessagesAsync(item.LicenceId, "adoxio_proposedoperator").GetAwaiter().GetResult();
            }
            catch (HttpOperationException httpOperationException)
            {
                if (httpOperationException.Response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError(httpOperationException, "Error deleting proposed operator");
                    throw;
                }
            }

            // find the related application and delete it.
            foreach (var application in adoxioLicense.AdoxioAdoxioLicencesAdoxioApplicationAssignedLicence)
            {
                // get the full application type.]
                var applicationType = _dynamicsClient.GetApplicationTypeById(application._adoxioApplicationtypeidValue).GetAwaiter().GetResult();
                if (applicationType.AdoxioName.Contains("Third Party Operator"))
                {
                    var patchApplication = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        //Statecode = (int?)AdoxioApplicationStatusCodes.Cancelled,
                        Statuscode = (int?)AdoxioApplicationStatusCodes.Terminated
                    };
                    try
                    {
                        _dynamicsClient.Applications.Update(application.AdoxioApplicationid, patchApplication);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error cancelling related application");
                        // fail if we can't create.
                        throw;
                    }

                }
            }


            return Ok();
        }

        // handle terminate-operator-application
        [HttpPost("terminate-operator-relationship")]
        public ActionResult TerminateTPORelationship(LicenceTransfer item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // check access to licence
            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.GetLicenceByIdWithChildren(item.LicenceId);
            if (adoxioLicense == null)
            {
                return NotFound();
            }
            bool hasAccess = CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.AdoxioLicencee.Accountid);
            hasAccess |= (adoxioLicense.AdoxioThirdPartyOperatorId != null && CurrentUserHasAccessToLicenseTransferredTo(adoxioLicense.AdoxioThirdPartyOperatorId.Accountid));
            if (!hasAccess)
            {
                return Forbid();
            }

            // Delete the ThirdPartyOperator (ThirdPartyOperatorId)
            try
            {
                _dynamicsClient.Licenceses.DeleteReferenceWithHttpMessagesAsync(item.LicenceId, "adoxio_ThirdPartyOperatorId").GetAwaiter().GetResult();
            }
            catch (HttpOperationException httpOperationException)
            {
                if (httpOperationException.Response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError(httpOperationException, "Error deleting third party operator");
                    throw;
                }
            }
            return Ok();
        }


        /// Create a change of location application
        [HttpPost("{licenceId}/create-action-application/{applicationTypeName}")]
        public async Task<JsonResult> CreateApplicationForAction(string licenceId, string applicationTypeName)
        {
            // for association with current user
            string userJson = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(userJson);

            var expand = new List<string> {
                "adoxio_Licencee",
                "adoxio_LicenceType",
                "adoxio_adoxio_licences_adoxio_applicationtermsconditionslimitation_Licence",
                "adoxio_adoxio_licences_adoxio_application_AssignedLicence",
                "adoxio_establishment",
                "adoxio_LicenceSubCategoryId"
            };

            // grab the licence record
            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.Licenceses.GetByKey(licenceId, expand: expand);
            if (adoxioLicense == null)
            {
                // exit if we don't find one
                throw new Exception("Error getting license.");
            }
            else
            {
                // create a blank application
                MicrosoftDynamicsCRMadoxioApplication application = new MicrosoftDynamicsCRMadoxioApplication();

                // copy some standard values
                application.CopyValuesForChangeOfLocation(adoxioLicense, applicationTypeName != "CRS Location Change");

                // get the previous application for the licence.

                application.AdoxioApplicanttype = adoxioLicense.AdoxioLicencee.AdoxioBusinesstype;

                // set application type relationship
                var applicationType = _dynamicsClient.GetApplicationTypeByName(applicationTypeName);
                application.AdoxioApplicationTypeIdODataBind = _dynamicsClient.GetEntityURI("adoxio_applicationtypes", applicationType.AdoxioApplicationtypeid);

                // set licence type relationship
                if (adoxioLicense.AdoxioLicenceType != null)
                {

                    application.AdoxioLicenceTypeODataBind = _dynamicsClient.GetEntityURI("adoxio_licencetypes", adoxioLicense.AdoxioLicenceType.AdoxioLicencetypeid);
                }

                // set the licence subtype if we have one

                if (adoxioLicense.AdoxioLicenceSubCategoryId != null)
                {
                    application.AdoxioLicenceSubCategoryODataBind =
                        _dynamicsClient.GetEntityURI("adoxio_licencesubcategories",
                            adoxioLicense.AdoxioLicenceSubCategoryId.AdoxioLicencesubcategoryid);
                }

                // set the applicant
                application.AdoxioApplicantODataBind = _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId);

                // if the licence has an establishment (from CopyValuesForChangeOfLocation)
                if (adoxioLicense.AdoxioEstablishment != null)
                {
                    application.AdoxioLicenceEstablishmentODataBind = _dynamicsClient.GetEntityURI("adoxio_establishments", adoxioLicense.AdoxioEstablishment.AdoxioEstablishmentid);
                }

                try
                {
                    // try finding a licence application
                    var licenceApp = adoxioLicense?.AdoxioAdoxioLicencesAdoxioApplicationAssignedLicence?.Where(app => !string.IsNullOrEmpty(app._adoxioLocalgovindigenousnationidValue)).FirstOrDefault();
                    string lginvalue = "";

                    // if we don't find it
                    if (licenceApp == null)
                    {
                        // check if there is a LGIN value on the Licence Record
                        if (adoxioLicense?._adoxioLginValue != null)
                        {
                            lginvalue = adoxioLicense?._adoxioLginValue;
                        }
                        // otherwise check if there is an LGIN value on the Establishment
                        else
                        {
                            if (adoxioLicense?.AdoxioEstablishment != null)
                            {
                                lginvalue = adoxioLicense?.AdoxioEstablishment._adoxioLginValue;
                            }
                        }
                        // note there will be no LGIN for Marketers or Agent, but we initialized to an empty string so we're all good
                    }
                    else
                    {
                        lginvalue = licenceApp._adoxioLocalgovindigenousnationidValue;

                    }

                    // if we found an LGIN value
                    if (!string.IsNullOrEmpty(lginvalue))
                    {
                        // set the value on the application
                        application.AdoxioLocalgovindigenousnationidODataBind = _dynamicsClient.GetEntityURI("adoxio_localgovindigenousnations", lginvalue);
                    }

                    // look for a Police Jurisdiction value on the licence application
                    licenceApp = adoxioLicense?.AdoxioAdoxioLicencesAdoxioApplicationAssignedLicence?.Where(app => !string.IsNullOrEmpty(app._adoxioPolicejurisdictionidValue)).FirstOrDefault();
                    // if we find one
                    if (!string.IsNullOrEmpty(licenceApp?._adoxioPolicejurisdictionidValue))
                    {
                        // update the application with that value
                        application.AdoxioPoliceJurisdictionIdODataBind = _dynamicsClient.GetEntityURI("adoxio_policejurisdictions", licenceApp?._adoxioPolicejurisdictionidValue);
                    }
                    // create the application with the data we've brought over
                    application = _dynamicsClient.Applications.Create(application);
                }
                catch (HttpOperationException httpOperationException)
                {
                    string applicationId = _dynamicsClient.GetCreatedRecord(httpOperationException, null);
                    if (!string.IsNullOrEmpty(applicationId) && Guid.TryParse(applicationId, out Guid applicationGuid))
                    {
                        application = await _dynamicsClient.GetApplicationById(applicationGuid);
                    }
                    else
                    {

                        _logger.LogError(httpOperationException, "Error creating application");
                        // fail if we can't create.
                        throw httpOperationException;
                    }

                }

                // copy service areas from licence
                /*  TG- Removing for now; will result in service areas being copied across endorsement types.

                try
                {
                    string filter = $"_adoxio_licenceid_value eq {licenceId}";

                    string applicationUri = _dynamicsClient.GetEntityURI("adoxio_applications", application.AdoxioApplicationid);

                    IList<MicrosoftDynamicsCRMadoxioServicearea> areas = _dynamicsClient.Serviceareas.Get(filter: filter).Value;
                    foreach (MicrosoftDynamicsCRMadoxioServicearea area in areas)
                    {
                        MicrosoftDynamicsCRMadoxioServicearea newArea = new MicrosoftDynamicsCRMadoxioServicearea()
                        {
                            ApplicationOdataBind = applicationUri,
                            AdoxioAreacategory = area.AdoxioAreacategory,
                            AdoxioArealocation = area.AdoxioArealocation,
                            AdoxioAreanumber = area.AdoxioAreanumber,
                            AdoxioCapacity = area.AdoxioCapacity,
                            AdoxioIsindoor = area.AdoxioIsindoor,
                            AdoxioIsoutdoor = area.AdoxioIsoutdoor,
                            AdoxioIspatio = area.AdoxioIspatio,
                            AdoxioDateadded = DateTimeOffset.Now,
                            AdoxioDateupdated = DateTimeOffset.Now
                        };
                        _dynamicsClient.Serviceareas.Create(newArea);
                    }
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.LogError(httpOperationException, "Error adding service areas from licence to application");
                }
                */

                // now bind the new application to the given licence.

                var patchApplication = new MicrosoftDynamicsCRMadoxioApplication()
                {
                    AdoxioAssignedLicenceODataBind = _dynamicsClient.GetEntityURI("adoxio_licenceses", licenceId)
                };

                try
                {
                    _dynamicsClient.Applications.Update(application.AdoxioApplicationid, patchApplication);
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.LogError(httpOperationException, "Error updating application");
                }

                return new JsonResult(await application.ToViewModel(_dynamicsClient, _cache, _logger));

            }
        }

        /// GET all licenses in Dynamics by Licencee using the account Id assigned to the user logged in
        [HttpGet("current")]
        public List<ApplicationLicenseSummary> GetCurrentUserLicences()
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // get all licenses in Dynamics by Licencee using the account Id assigned to the user logged in
            List<ApplicationLicenseSummary> adoxioLicences = _dynamicsClient.GetLicensesByLicencee(userSettings.AccountId, _cache);
            List<ApplicationLicenseSummary> transferredLicences = _dynamicsClient.GetPaidLicensesOnTransfer(userSettings.AccountId);
            adoxioLicences.AddRange(transferredLicences);
            adoxioLicences.ForEach(lic =>
            {
                lic.ChecklistConclusivelyDeem = isConclusivelyDeemed(lic);
            });


            return adoxioLicences;
        }
        private bool isConclusivelyDeemed(ApplicationLicenseSummary lic)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            var result = false;
            var filter = $"_adoxio_applicant_value eq {userSettings.AccountId}";
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
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // get all third party operator licenses
            List<ApplicationLicenseSummary> adoxioLicenses = await GetThirdPartyOperatedLicencesForAccountAsync(userSettings.AccountId.ToString());
            adoxioLicenses.ForEach(lic =>
            {
                lic.ChecklistConclusivelyDeem = isConclusivelyDeemed(lic);
            });

            return new JsonResult(adoxioLicenses);
        }


        /// GET all proposed licenses in Dynamics by Licencee using the account Id assigned to the user logged in
        [HttpGet("proposed-owner")]
        public async Task<JsonResult> GetProposedLicenseeLicences()
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // get all proposed operator licenses
            List<ApplicationLicenseSummary> adoxioLicenses = await GetProposedOwnerLicencesForAccountAsync(userSettings.AccountId.ToString());
            adoxioLicenses.ForEach(lic =>
            {
                lic.ChecklistConclusivelyDeem = isConclusivelyDeemed(lic);
            });
            return new JsonResult(adoxioLicenses);
        }

        private async Task<List<ApplicationLicenseSummary>> GetThirdPartyOperatedLicencesForAccountAsync(string thirdPartyOperatorId)
        {
            List<ApplicationLicenseSummary> result;
            try
            {
                string[] expand = { "adoxio_thirdpartyoperator_licences" };
                // fetch from Dynamics.
                var account = await _dynamicsClient.Accounts.GetByKeyAsync(accountid: thirdPartyOperatorId, expand: expand);
                result = account.AdoxioThirdpartyoperatorLicences
                .Select(licence => _dynamicsClient.GetLicenceByIdWithChildren(licence.AdoxioLicencesid))
                .Select(licence => licence.ToLicenseSummaryViewModel(new List<MicrosoftDynamicsCRMadoxioApplication>(), _dynamicsClient))
                .ToList();
            }
            catch (HttpOperationException)
            {
                result = null;
            }

            return result;
        }

        private async Task<List<ApplicationLicenseSummary>> GetProposedOwnerLicencesForAccountAsync(string accountId)
        {
            List<ApplicationLicenseSummary> result = new List<ApplicationLicenseSummary>();
            try
            {
                string[] expand = { "adoxio_account_adoxio_licences_ProposedOwner" };
                // fetch from Dynamics.
                var account = await _dynamicsClient.Accounts.GetByKeyAsync(accountid: accountId, expand: expand);
                var licences = account.AdoxioAccountAdoxioLicencesProposedOwner
                .Select(licence => _dynamicsClient.GetLicenceByIdWithChildren(licence.AdoxioLicencesid))
                .Select(licence =>
                {
                    licence.AdoxioLicenceType = Gov.Lclb.Cllb.Public.Models.ApplicationExtensions.GetCachedLicenceType(licence._adoxioLicencetypeValue, _dynamicsClient, _cache);
                    return licence;
                })
                .ToList();
                if (licences != null)
                {
                    foreach (var licence in licences)
                    {
                        IEnumerable<MicrosoftDynamicsCRMadoxioApplication> applicationsInProgress = _dynamicsClient.GetApplicationsForLicenceByApplicant(licence.AdoxioLicencee.Accountid);
                        var applications = new List<MicrosoftDynamicsCRMadoxioApplication>();
                        if (applicationsInProgress != null)
                        {
                            applications = applicationsInProgress.Where(app => app._adoxioAssignedlicenceValue == licence.AdoxioLicencesid).ToList();
                        }
                        result.Add(licence.ToLicenseSummaryViewModel(applications, _dynamicsClient));
                    }
                }
            }
            catch (HttpOperationException)
            {
                result = null;
            }

            return result;
        }

        /// GET all licenses in Dynamics filtered by the GUID of the licencee
        [HttpGet("licencee/{licenceeId}")]
        public JsonResult GetDynamicsLicenses(string licenceeId)
        {
            // get all licenses in Dynamics by Licencee Id
            var result = _dynamicsClient.GetLicensesByLicencee(_cache, licenceeId);



            return new JsonResult(result);
        }

        /// GET a licence as PDF.
        [HttpGet("{licenceId}/pdf/{filename}")]
        public async Task<IActionResult> GetLicencePDF(string licenceId, string filename)
        {

            var expand = new List<string> {
                "adoxio_Licencee",
                "adoxio_adoxio_licences_adoxio_applicationtermsconditionslimitation_Licence",
                "adoxio_adoxio_licences_adoxio_application_AssignedLicence",
                "adoxio_LicenceType",
                "adoxio_establishment",
                "adoxio_ProposedOwner",
                "adoxio_LicenceSubCategoryId"
            };

            MicrosoftDynamicsCRMadoxioLicences adoxioLicense = _dynamicsClient.Licenceses.GetByKey(licenceId, expand: expand);
            if (adoxioLicense == null)
            {
                throw new Exception("Error getting license.");
            }

            if (CurrentUserHasAccessToLicenseOwnedBy(adoxioLicense.AdoxioLicencee.Accountid) ||
                (adoxioLicense.AdoxioProposedOwner != null && CurrentUserHasAccessToLicenseTransferredTo(adoxioLicense.AdoxioProposedOwner.Accountid)))
            {
                var effectiveDateParam = "";
                if (adoxioLicense.AdoxioEffectivedate.HasValue)
                {
                    DateTime effectiveDate = adoxioLicense.AdoxioEffectivedate.Value.DateTime;
                    effectiveDateParam = effectiveDate.ToString("MMMM dd, yyyy");
                }

                var expiraryDateParam = "";
                if (adoxioLicense.AdoxioExpirydate.HasValue)
                {
                    DateTime expiryDate = adoxioLicense.AdoxioExpirydate.Value.DateTime;
                    expiraryDateParam = expiryDate.ToString("MMMM dd, yyyy");
                }

                var termsAndConditions = "";
                foreach (var item in adoxioLicense.AdoxioAdoxioLicencesAdoxioApplicationtermsconditionslimitationLicence)
                {
                    termsAndConditions += $"<li>{item.AdoxioTermsandconditions}</li>";
                }

                var endorsementsText = "";
                License licenceVM = adoxioLicense.ToViewModel(_dynamicsClient);

                if (licenceVM.Endorsements != null && licenceVM.Endorsements.Count > 0)
                {

                    var licenceHasSEA = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Special Event Area Endorsement");
                    var licenceHasLounge = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Lounge Area Endorsement");
                    var licenceHasStore = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "On-Site Store Endorsement");
                    var licenceHasCatering = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Catering Endorsement");
                    var licenceHasOffsite = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Off-Site Store Endorsement");
                    var licenceHasPicnic = licenceVM.Endorsements.FindIndex(x => x.EndorsementName == "Picnic Area Endorsement"); ;

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

                    if (licenceHasPicnic > -1)
                    {
                        endorsementsText += licenceVM.Endorsements[licenceHasPicnic].SimpleHeader();
                    }

                }
                var storeHours = "";

                MicrosoftDynamicsCRMadoxioHoursofserviceCollection hours = _dynamicsClient.Hoursofservices.Get(filter: $"_adoxio_licence_value eq {licenceId} and _adoxio_endorsement_value eq null");

                if (hours.Value.Count > 0)
                {

                    MicrosoftDynamicsCRMadoxioHoursofservice hoursVal = hours.Value.First();

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
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioMondayopen)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioTuesdayopen)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioWednesdayopen)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioThursdayopen)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioFridayopen)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSaturdayopen)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSundayopen)}</td>
                            </tr>
                            <tr>
                                <td class='hours'>End</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioMondayclose)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioTuesdayclose)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioWednesdayclose)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioThursdayclose)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioFridayclose)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSaturdayclose)}</td>
                                <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSundayclose)}</td>
                            </tr></table>";
                }
                else
                {
                    // to do: log when we expect to find hours of sale, but don't
                    // wine stores, ubrew, lrs.
                }

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                if (adoxioLicense.AdoxioLicenceType.AdoxioName == "Cannabis Retail Store")
                {
                    parameters = new Dictionary<string, string>
                    {
                        { "title", "Cannabis_Licence" },
                        { "licenceNumber", adoxioLicense.AdoxioLicencenumber },
                        { "establishmentName", adoxioLicense.AdoxioEstablishment?.AdoxioName },
                        { "establishmentStreet", adoxioLicense.AdoxioEstablishment?.AdoxioAddressstreet },
                        { "establishmentCity", adoxioLicense.AdoxioEstablishment?.AdoxioAddresscity + ", B.C." },
                        { "establishmentPostalCode", adoxioLicense.AdoxioEstablishment?.AdoxioAddresspostalcode },
                        { "licenceType", adoxioLicense.AdoxioLicenceType?.AdoxioName },
                        { "licencee", adoxioLicense.AdoxioLicencee?.Name },
                        { "effectiveDate", effectiveDateParam },
                        { "expiryDate", expiraryDateParam },
                        { "restrictionsText", termsAndConditions },
                        { "endorsementsText", endorsementsText },
                        { "storeHours", storeHours}
                    };
                }
                else if (adoxioLicense.AdoxioLicenceType.AdoxioName == "Marketing")
                {
                    parameters = new Dictionary<string, string>
                    {
                        { "title", "Cannabis_Licence" },
                        { "licenceNumber", adoxioLicense.AdoxioLicencenumber },
                        { "establishmentName", adoxioLicense.AdoxioLicencee?.Name  },
                        { "establishmentStreet", adoxioLicense.AdoxioLicencee?.Address1Line1 },
                        { "establishmentCity", adoxioLicense.AdoxioLicencee?.Address1City + ", B.C." },
                        { "establishmentPostalCode", adoxioLicense.AdoxioLicencee?.Address1Postalcode },
                        { "licencee", adoxioLicense.AdoxioLicencee?.Name },
                        { "licenceType", adoxioLicense.AdoxioLicenceType?.AdoxioName },
                        { "effectiveDate", effectiveDateParam },
                        { "expiryDate", expiraryDateParam },
                        { "restrictionsText", termsAndConditions },
                        { "endorsementsText", endorsementsText },
                        { "storeHours", storeHours }
                    };
                }

                else // handle other types such as catering
                {
                    String typeLabel = adoxioLicense?.AdoxioLicenceSubCategoryId?.AdoxioName != null ? adoxioLicense.AdoxioLicenceSubCategoryId?.AdoxioName : adoxioLicense.AdoxioLicenceType?.AdoxioName;

                    // adoxioLicense.AdoxioLicenceType?.AdoxioName

                    //adoxioLicense.AdoxioLicenceSubCategoryId?

                    parameters = new Dictionary<string, string>
                    {
                        { "title", "Liquor_License" },
                        { "licenceNumber", adoxioLicense.AdoxioLicencenumber },
                        { "establishmentName", adoxioLicense.AdoxioEstablishment?.AdoxioName },
                        { "licenceName", adoxioLicense.AdoxioEstablishment?.AdoxioName},                        // we may need another field for this...
                        { "establishmentStreet", adoxioLicense.AdoxioEstablishment?.AdoxioAddressstreet },
                        { "establishmentCity", adoxioLicense.AdoxioEstablishment?.AdoxioAddresscity + ", B.C." },
                        { "establishmentPostalCode", adoxioLicense.AdoxioEstablishment?.AdoxioAddresspostalcode },
                        { "licencee", adoxioLicense.AdoxioLicencee?.Name },
                        { "licenceType", typeLabel},
                        { "effectiveDate", effectiveDateParam },
                        { "expiryDate", expiraryDateParam },
                        { "restrictionsText", termsAndConditions },
                        { "endorsementsText", endorsementsText },
                        { "storeHours", storeHours },
                        { "printDate", DateTime.Today.ToString("MMMM dd, yyyy")} // will be based on the users machine
                    };
                }
                try
                {
                    var templateName = "cannabis_licence";

                    switch (adoxioLicense.AdoxioLicenceType.AdoxioName)
                    {
                        case "Marketing":
                            templateName = "cannabis_marketer_licence";
                            break;
                        /*
                        case "Catering":
                            templateName = "catering_licence";
                            break;

                        case "UBrew and UVin":
                            templateName = "wine_store_licence";
                            break;

                        case "Licensee Retail Store":
                            templateName = "wine_store_licence";
                            break;

                        case "Wine Store":
                            templateName = "wine_store_licence";
                            break;

                        case "Manufacturer":
                            templateName = "manufacturer_licence";
                            break;
                        */
                        case "Catering":
                        case "UBrew and UVin":
                        case "Licensee Retail Store":
                        case "Wine Store":
                        case "Manufacturer":
                            templateName = "liquor_licence";
                            break;
                    }

                    byte[] data = await _pdfClient.GetPdf(parameters, templateName);

                    // Save copy of generated licence PDF for auditing/logging purposes
                    try
                    {
                        var hash = await _pdfClient.GetPdfHash(parameters, templateName);
                        var entityName = "licence";
                        var entityId = adoxioLicense.AdoxioLicencesid;
                        var folderName = await _dynamicsClient.GetFolderName(entityName, entityId).ConfigureAwait(true);
                        var documentType = "Licence";
                        _fileManagerClient.UploadPdfIfChanged(_logger, entityName, entityId, folderName, documentType, data, hash);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error uploading PDF");
                    }

                    return File(data, "application/pdf", $"{adoxioLicense.AdoxioLicencenumber}.pdf");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error returning PDF response");

                    return new NotFoundResult();
                }
            }
            else
            {
                return new UnauthorizedResult();
            }
        }

        [HttpPut("{licenceId}/ldbordertotals")]
        public async Task<IActionResult> UpdateLicenceLDBOrderTotals([FromBody] int total, string licenceId)
        {
            if (total == null || string.IsNullOrEmpty(licenceId))
            {
                return BadRequest();
            }

            MicrosoftDynamicsCRMadoxioLicences licence = _dynamicsClient.GetLicenceByIdWithChildren(licenceId);
            if (licence == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(licence.AdoxioLicencee.Accountid) &&
                !CurrentUserHasAccessToLicenseTransferredTo(licence.AdoxioProposedOwner.Accountid))
            {
                return Forbid();
            }

            MicrosoftDynamicsCRMadoxioLicences patchObject = new MicrosoftDynamicsCRMadoxioLicences()
            {
                AdoxioLdbordertotals = total
            };

            try
            {
                await _dynamicsClient.Licenceses.UpdateAsync(licenceId, patchObject);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating licence ldb order totals");
                throw new Exception("Unable to update licence ldb order totals");
            }

            return Ok();
        }

        [HttpPut("{licenceId}/establishment")]
        public async Task<IActionResult> UpdateLicenceEstablishment([FromBody] ViewModels.ApplicationLicenseSummary item, string licenceId)
        {
            if (item == null || string.IsNullOrEmpty(licenceId) || licenceId != item.LicenseId)
            {
                return BadRequest();
            }

            MicrosoftDynamicsCRMadoxioLicences licence = _dynamicsClient.GetLicenceByIdWithChildren(licenceId);
            if (licence == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(licence.AdoxioLicencee.Accountid))
            {
                return Forbid();
            }

            MicrosoftDynamicsCRMadoxioLicences patchObject = new MicrosoftDynamicsCRMadoxioLicences()
            {
                AdoxioEstablishmentphone = item.EstablishmentPhoneNumber,
                AdoxioEstablishmentaddresscity = item.EstablishmentAddressCity,
                AdoxioEstablishmentaddressstreet = item.EstablishmentAddressStreet,
                AdoxioEstablishmentaddresspostalcode = item.EstablishmentAddressPostalCode
            };

            try
            {
                await _dynamicsClient.Licenceses.UpdateAsync(licenceId, patchObject);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating licence establishment");
                throw new Exception("Unable to update licence establishment");
            }

            try
            {
                licence = _dynamicsClient.GetLicenceByIdWithChildren(licenceId);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting licence");
                throw new Exception("Unable to get licence after update");
            }

            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> applicationsInProgress = _dynamicsClient.GetApplicationsForLicenceByApplicant(licence.AdoxioLicencee.Accountid);
            var applications = applicationsInProgress.Where(app => app._adoxioAssignedlicenceValue == licence.AdoxioLicencesid).ToList();

            licence.AdoxioLicenceType = Models.ApplicationExtensions.GetCachedLicenceType(licence._adoxioLicencetypeValue, _dynamicsClient, _cache);
            return new JsonResult(licence.ToLicenseSummaryViewModel(applications, _dynamicsClient));
        }

        [HttpPut("{licenceId}/representative")]
        public async Task<IActionResult> UpdateOffsiteLocations([FromBody] ViewModels.ApplicationLicenseSummary item, string licenceId)
        {
            if (item == null || string.IsNullOrEmpty(licenceId) || licenceId != item.LicenseId)
            {
                return BadRequest();
            }

            MicrosoftDynamicsCRMadoxioLicences licence = _dynamicsClient.GetLicenceByIdWithChildren(licenceId);
            if (licence == null)
            {
                return NotFound();
            }

            if (!CurrentUserHasAccessToLicenseOwnedBy(licence.AdoxioLicencee.Accountid))
            {
                return Forbid();
            }

            MicrosoftDynamicsCRMadoxioLicences patchObject = new MicrosoftDynamicsCRMadoxioLicences()
            {
                AdoxioRepresentativename = item.RepresentativeFullName,
                AdoxioRepresentativephone = item.RepresentativePhoneNumber,
                AdoxioRepresentativeemail = item.RepresentativeEmail,
                AdoxioCansubmitpermanentchangeapplications = item.RepresentativeCanSubmitPermanentChangeApplications,
                AdoxioCansigntemporarychangeapplications = item.RepresentativeCanSignTemporaryChangeApplications,
                AdoxioCanobtainlicenceinformation = item.RepresentativeCanObtainLicenceInformation,
                AdoxioCansigngrocerystoreproofofsales = item.RepresentativeCanSignGroceryStoreProofOfSale,
                AdoxioCanattendeducationsessions = item.RepresentativeCanAttendEducationSessions,
                AdoxioCanattendcompliancemeetings = item.RepresentativeCanAttendComplianceMeetings,
                AdoxioCanrepresentathearings = item.RepresentativeCanRepresentAtHearings
            };

            try
            {
                await _dynamicsClient.Licenceses.UpdateAsync(licenceId, patchObject);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating licence representative");
                throw new Exception("Unable to update licence representative");
            }

            try
            {
                licence = _dynamicsClient.GetLicenceByIdWithChildren(licenceId);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting licence");
                throw new Exception("Unable to get licence after update");
            }

            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> applicationsInProgress = _dynamicsClient.GetApplicationsForLicenceByApplicant(licence.AdoxioLicencee.Accountid);
            var applications = applicationsInProgress.Where(app => app._adoxioAssignedlicenceValue == licence.AdoxioLicencesid).ToList();

            licence.AdoxioLicenceType = Models.ApplicationExtensions.GetCachedLicenceType(licence._adoxioLicencetypeValue, _dynamicsClient, _cache);
            return new JsonResult(licence.ToLicenseSummaryViewModel(applications, _dynamicsClient));
        }

        /// <summary>
        /// Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
        private bool CurrentUserHasAccessToLicenseOwnedBy(string accountId)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // For now, check if the account id matches the user's account.
            // TODO there may be some account relationships in the future
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return userSettings.AccountId == accountId;
            }

            // if current user doesn't have an account they are probably not logged in
            return false;
        }

        /// <summary>
        /// Verify whether currently logged in user has access to the account of a proposed owner of a licence
        /// </summary>
        /// <returns>boolean</returns>
        private bool CurrentUserHasAccessToLicenseTransferredTo(string accountId)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // For now, check if the account id matches the user's account.
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return userSettings.AccountId == accountId;
            }

            // if current user doesn't have an account they are probably not logged in
            return false;
        }
    }

    public class LicenceTransfer
    {
        public string AccountId { get; set; }
        public string LicenceId { get; set; }
    }
}