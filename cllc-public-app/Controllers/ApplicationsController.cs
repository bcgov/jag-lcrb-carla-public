extern alias DV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using adoxio_application_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application;
using adoxio_application_statuscode = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_statuscode;
using adoxio_licences_statuscode = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licences_statuscode;
using adoxio_applicationtype_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicationtype;
using adoxio_servicearea_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_servicearea;
using adoxio_hoursofservice_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_hoursofservice;
using adoxio_applicationextension_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicationextension;
using adoxio_tiedhouseconnection_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_tiedhouseconnection;
using adoxio_tiedhouseconnection_adoxio_connectiontype = DV::Gov.Lclb.Cllb.Interfaces.adoxio_tiedhouseconnection_adoxio_connectiontype;
using adoxio_generalyesno_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno;
using adoxio_servicehoursoptionsethours = DV::Gov.Lclb.Cllb.Interfaces.adoxio_servicehoursoptionsethours;
using adoxio_application_adoxio_manufacturerproductionamountunit_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_manufacturerproductionamountunit;
using Microsoft.Xrm.Sdk;
using LicenseModel = Gov.Lclb.Cllb.Public.Models.LicenseExtensions;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Extensions;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Repositories;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using Application = Gov.Lclb.Cllb.Public.ViewModels.Application;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IDataverseClient _dataverse;
        private readonly IWebHostEnvironment _env;
        private readonly FileManagerClient _fileManagerClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IBCEPService _bcep;
        private readonly TiedHouseConnectionsRepository _tiedHouseConnectionsRepository;


        public ApplicationsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory, IDataverseClient dataverse,
            FileManagerClient fileClient, IBCEPService bcep,
            IWebHostEnvironment env, IMemoryCache memoryCache,
            TiedHouseConnectionsRepository tiedHouseConnectionsRepository)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dataverse = dataverse;
            _logger = loggerFactory.CreateLogger(typeof(ApplicationsController));
            _fileManagerClient = fileClient;
            _env = env;
            _bcep = bcep;
            _tiedHouseConnectionsRepository = tiedHouseConnectionsRepository;
        }


        /// <summary>
        ///     Get a license application by applicant id
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        private async Task<List<ApplicationSummary>> GetApplicationSummariesByApplicantAsync(string applicantId)
        {
            var apps = await _dataverse.GetApplicationsByApplicantExpandedAsync(applicantId);

            // Collect all unique referenced IDs upfront so we can fetch in parallel
            var uniqueAppTypeIds = apps
                .Select(a => a.adoxio_ApplicationTypeId?.Id.ToString())
                .OfType<string>().Distinct().ToList();
            var uniqueAssignedLicIds = apps
                .Where(a => a.adoxio_AssignedLicence != null)
                .Select(a => a.adoxio_AssignedLicence.Id.ToString())
                .Distinct().ToList();
            var uniqueLicTypeIds = apps
                .Where(a => a.adoxio_LicenceType != null)
                .Select(a => a.adoxio_LicenceType.Id.ToString())
                .Distinct().ToList();

            // Fetch all three entity sets in 3 parallel batch queries — 1 Dataverse call each instead of N
            var appTypeTask = _dataverse.GetApplicationTypesByIdsAsync(uniqueAppTypeIds);
            var licenceTask = _dataverse.GetLicencesByIdsAsync(uniqueAssignedLicIds);
            var licTypeTask = _dataverse.GetApplicationTypesByLicenceTypeIdsAsync(uniqueLicTypeIds);
            await Task.WhenAll(appTypeTask, licenceTask, licTypeTask);

            var appTypeDict = appTypeTask.Result
                .Where(t => t.adoxio_applicationtypeId.HasValue)
                .ToDictionary(t => t.adoxio_applicationtypeId!.Value.ToString());
            var licenceDict = licenceTask.Result
                .Where(l => l.adoxio_licencesId.HasValue)
                .ToDictionary(l => l.adoxio_licencesId!.Value.ToString());
            var licTypeDict = licTypeTask.Result
                .Where(t => t.adoxio_LicenceType != null)
                .GroupBy(t => t.adoxio_LicenceType!.Id.ToString())
                .ToDictionary(g => g.Key, g => (IList<adoxio_applicationtype_dv>)g.ToList());

            // Loop is now pure dictionary lookups — zero Dataverse calls
            var result = new List<ApplicationSummary>();
            foreach (var app in apps)
            {
                var appTypeId = app.adoxio_ApplicationTypeId?.Id.ToString();
                var appType = appTypeId != null && appTypeDict.TryGetValue(appTypeId, out var at) ? at : null;

                // skip apps for expired licences unless it's a renewal
                if (appType?.adoxio_IsRenewal != true && app.adoxio_AssignedLicence != null)
                {
                    var assignedLicId = app.adoxio_AssignedLicence.Id.ToString();
                    if (licenceDict.TryGetValue(assignedLicId, out var lic) && lic?.statuscode == adoxio_licences_statuscode.Expired)
                        continue;
                }

                var endorsements = new List<string>();
                if ((app.adoxio_LicenceType != null && appType?.adoxio_IsDefault == true ||
                     appType?.adoxio_IsRelocation == true) &&
                    app.adoxio_PaymentRecieved == true)
                {
                    var licTypeId = app.adoxio_LicenceType?.Id.ToString();
                    if (!string.IsNullOrEmpty(licTypeId) && licTypeDict.TryGetValue(licTypeId, out var appTypes))
                    {
                        endorsements = appTypes
                            .Where(t => t.adoxio_IsEndorsement == true || t.adoxio_CopyLicenceTC == true)
                            .Select(t => t.adoxio_name ?? string.Empty)
                            .ToList();
                    }
                }
                var row = app.ToSummaryViewModel(appType);
                row.Endorsements = endorsements;
                result.Add(row);
            }
            return result;
        }

        /// <summary>
        ///     Get a license application by applicant id
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        private async Task<List<Application>> GetApplicationsByApplicant(string applicantId)
        {
            var result = new List<Application>();
            var apps = await _dataverse.GetApplicationsByApplicantExpandedAsync(applicantId);
            foreach (var app in apps)
                result.Add(await app.ToViewModelAsync(_dataverse, _cache, _logger));

            // second pass to determine if location change is in progress
            foreach (var item in result)
                if (item.LicenseType == "Cannabis Retail Store"
                    && item.ApplicationStatus == AdoxioApplicationStatusCodes.Approved
                    && item.AssignedLicence != null
                    && item.AssignedLicence.ExpiryDate > DateTime.Now)
                    item.IsLocationChangeInProgress = FindRelatedApplication(result, item, "CRS Location Change");

            return result;
        }

        private bool FindRelatedApplication(List<Application> applicationList, Application application,
            string licenseType)
        {
            var result = false;
            foreach (var item in applicationList)
                if (item.LicenseType == licenseType && item.AssignedLicence != null &&
                    item.AssignedLicence.Id == application.AssignedLicence.Id)
                {
                    result = true;
                    break;
                }

            return result;
        }

        /// <summary>
        /// Get the count of approved cannabis retail store licences for the given user.
        /// </summary>
        private async Task<int> GetApprovedCannabisRetailStoreLicenceCountByApplicantAsync(string licenceeId)
        {
            if (string.IsNullOrEmpty(licenceeId)) return 0;
            try
            {
                var licences = await _dataverse.GetLicencesByAccountIdAsync(licenceeId);
                return licences.Count(l =>
                    l.statuscode == adoxio_licences_statuscode.Active &&
                    l.adoxio_LicenceType?.Name == "Cannabis Retail Store");
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetApprovedCannabisRetailStoreLicenceCountByApplicant Error");
                return 0;
            }
        }

        /// <summary>
        ///     Gets the number of submitted cannabis retail store applications.
        /// </summary>
        private async Task<int> GetSubmittedCannabisRetailStoreCountByApplicantAsync(string applicantId)
        {
            if (string.IsNullOrEmpty(applicantId)) return 0;
            try
            {
                var appType = await _dataverse.GetApplicationTypeByNameAsync("Cannabis Retail Store");
                var excludeStatuses = new List<int>
                {
                    (int)AdoxioApplicationStatusCodes.Terminated,
                    (int)AdoxioApplicationStatusCodes.Cancelled,
                    (int)AdoxioApplicationStatusCodes.Approved,
                    (int)AdoxioApplicationStatusCodes.Refused,
                    (int)AdoxioApplicationStatusCodes.TerminatedAndRefunded
                };
                var apps = await _dataverse.GetApplicationsByApplicantAndTypeAsync(
                    applicantId, appType?.adoxio_applicationtypeId?.ToString(), excludeStatuses);
                return apps.Count(a => a.adoxio_PaymentRecieved == true);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetSubmittedCannabisRetailStoreCountByApplicant Error");
                return 0;
            }
        }

        /// <summary>
        /// Get the count of approved applications for the current user.
        /// </summary>
        private async Task<int> GetApprovedApplicationsCountByApplicantAsync(string applicantId)
        {
            if (string.IsNullOrEmpty(applicantId)) return 0;
            try
            {
                var apps = await _dataverse.GetApplicationsByApplicantAndTypeAsync(
                    applicantId, null,
                    excludeStatuses: null,
                    requireStatecode0: true);
                return apps.Count(a => a.statuscode == adoxio_application_statuscode.Approved);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetApprovedApplicationsCountByApplicant Error");
                return 0;
            }
        }

        /// <summary>
        ///     GET all applications in Dynamics. Optional parameter for applicant ID. Or all applications if the applicantId is
        ///     null
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetDynamicsApplications(string applicantId)
        {
            var adoxioApplications = await GetApplicationsByApplicant(applicantId);
            return new JsonResult(adoxioApplications);
        }


        /// GET all applications in Dynamics for the current user
        [HttpGet("current")]
        public async Task<JsonResult> GetCurrentUserApplications()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var adoxioApplications = await GetApplicationSummariesByApplicantAsync(userSettings.AccountId);
            return new JsonResult(adoxioApplications);
        }

        /// GET all applications of the given application type in Dynamics for the current user
        [HttpGet("current/by-type")]
        public async Task<JsonResult> GetCurrentUserLgApprovalApplications(string applicationType)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            try
            {
                var appType = await _dataverse.GetApplicationTypeByNameAsync(applicationType);
                var apps = await _dataverse.GetApplicationsByApplicantAndTypeAsync(
                    userSettings.AccountId, appType?.adoxio_applicationtypeId?.ToString(), null);
                var results = apps.Select(a => a.ToSummaryViewModel(appType)).ToList();
                return new JsonResult(results);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting licensee application");
                throw;
            }
        }

        /// GET all local government approval applications in Dynamics for the current user
        [HttpGet("current/lg-approvals")]
        public async Task<IActionResult> GetLgApprovalApplications()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            try
            {
                var account = await _dataverse.GetAccountByIdAsync(userSettings.AccountId);
                var lginId = account?.adoxio_LGINLinkId?.Id.ToString();
                if (string.IsNullOrEmpty(lginId)) return new JsonResult(new List<Application>());

                var includeStatuses = new List<int> { (int)AdoxioApplicationStatusCodes.PendingForLGFNPFeedback };
                var apps = await _dataverse.GetApplicationsByLginAsync(
                    lginId, includeStatuses, lgDecision: (int)LGDecision.Pending);

                var results = new List<Application>();
                foreach (var app in apps)
                    results.Add(await app.ToViewModelAsync(_dataverse, _cache, _logger));
                return new JsonResult(results);
            }
            catch (Exception e)
            {
                var errorText = "Error getting local government approval applications";
                _logger.LogError(e, errorText);
                return StatusCode(StatusCodes.Status500InternalServerError, errorText);
            }
        }

        /// GET local government approval applications decision not made in Dynamics for the current user
        [HttpGet("current/lg-approvals-decision-not-made")]
        public async Task<IActionResult> getLGApprovalApplicationsDecisionNotMade([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation($"getLGApprovalApplicationsDecisionNotMade pageIndex: {pageIndex}, pageSize: {pageSize}");
            var results = new PagingResult<Application> { Value = new List<Application>() };
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            try
            {
                var account = await _dataverse.GetAccountByIdAsync(userSettings.AccountId);
                var lginId = account?.adoxio_LGINLinkId?.Id.ToString();
                if (string.IsNullOrEmpty(lginId)) return new JsonResult(results);

                var showLginTypes = await _dataverse.GetApplicationTypesByFilterAsync(isShowLginApproval: true);
                var zoningTypes = await _dataverse.GetApplicationTypesByFilterAsync(isLgZoningConfirmation: true);

                var includeTypeIds = showLginTypes.Select(t => t.adoxio_applicationtypeId.ToString()).ToList();
                var excludeTypeIds = zoningTypes.Select(t => t.adoxio_applicationtypeId.ToString()).ToList();

                var includeStatuses = new List<int> { (int)AdoxioApplicationStatusCodes.PendingForLGFNPFeedback };
                var (apps, totalCount) = await _dataverse.GetApplicationsByLginPagedAsync(
                    lginId,
                    includeStatuses,
                    lgDecision: (int)LGDecision.Pending,
                    hasDecisionDate: false,
                    includeTypeIds: includeTypeIds.Count > 0 ? includeTypeIds : null,
                    excludeTypeIds: null,
                    pageIndex: pageIndex,
                    pageSize: pageSize);

                results.Count = totalCount;
                foreach (var app in apps)
                {
                    var viewModel = await app.ToViewModelAsync(_dataverse, _cache, _logger);
                    _logger.LogInformation($"getLGApprovalApplicationsDecisionNotMade establishment: {viewModel.EstablishmentName}");
                    results.Value.Add(viewModel);
                }
            }
            catch (Exception e)
            {
                var errorText = "Error getting LG approval applications decision not made";
                _logger.LogError(e, errorText);
                return StatusCode(StatusCodes.Status500InternalServerError, errorText);
            }
            return new JsonResult(results);
        }

        /// GET local government approval applications for zoning in Dynamics for the current user
        [HttpGet("current/lg-approvals-for-zoning")]
        public async Task<IActionResult> getLGApprovalApplicationsForZoning([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var results = new PagingResult<Application> { Value = new List<Application>() };
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            try
            {
                var account = await _dataverse.GetAccountByIdAsync(userSettings.AccountId);
                var lginId = account?.adoxio_LGINLinkId?.Id.ToString();
                if (string.IsNullOrEmpty(lginId)) return new JsonResult(results);

                var zoningTypes = await _dataverse.GetApplicationTypesByFilterAsync(isLgZoningConfirmation: true);
                var includeTypeIds = zoningTypes.Select(t => t.adoxio_applicationtypeId.ToString()).ToList();

                var includeStatuses = new List<int> { (int)AdoxioApplicationStatusCodes.PendingForLGFNPFeedback };
                var (apps, totalCount) = await _dataverse.GetApplicationsByLginPagedAsync(
                    lginId,
                    includeStatuses,
                    lgDecision: (int)LGDecision.Pending,
                    hasDecisionDate: false,
                    includeTypeIds: includeTypeIds.Count > 0 ? includeTypeIds : null,
                    pageIndex: pageIndex,
                    pageSize: pageSize);

                results.Count = totalCount;
                foreach (var app in apps)
                    results.Value.Add(await app.ToViewModelAsync(_dataverse, _cache, _logger));
            }
            catch (Exception e)
            {
                var errorText = "Error getting LG approval applications for zoning";
                _logger.LogError(e, errorText);
                return StatusCode(StatusCodes.Status500InternalServerError, errorText);
            }
            return new JsonResult(results);
        }

        /// GET local government approval applications decision made but no docs in Dynamics for the current user
        [HttpGet("current/lg-approvals-dicision-made-but-no-docs")]
        public async Task<IActionResult> getLGApprovalApplicationsDicisionMadeButNoDocs([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var results = new PagingResult<Application> { Value = new List<Application>() };
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            try
            {
                var account = await _dataverse.GetAccountByIdAsync(userSettings.AccountId);
                var lginId = account?.adoxio_LGINLinkId?.Id.ToString();
                if (string.IsNullOrEmpty(lginId)) return new JsonResult(results);

                var includeStatuses = new List<int> { (int)AdoxioApplicationStatusCodes.PendingForLGFNPFeedback };
                var (apps, totalCount) = await _dataverse.GetApplicationsByLginPagedAsync(
                    lginId,
                    includeStatuses,
                    lgDecision: (int)LGDecision.Pending,
                    hasDecisionDate: true,   // adoxio_lgdecisionsubmissiondate ne null
                    pageIndex: pageIndex,
                    pageSize: pageSize);

                results.Count = totalCount;
                foreach (var app in apps)
                    results.Value.Add(await app.ToViewModelAsync(_dataverse, _cache, _logger));
            }
            catch (Exception e)
            {
                var errorText = "Error getting LG approval applications decision made but no docs";
                _logger.LogError(e, errorText);
                return StatusCode(StatusCodes.Status500InternalServerError, errorText);
            }
            return new JsonResult(results);
        }



        /** GET all local government approval applications in Dynamics for the current user that are resolved
        * pageIndex: 0 based page index
        * pageSize: the number of results per page
        */
        [HttpGet("current/resolved-lg-applications")]
        public async Task<IActionResult> GetResolvedLGApplications([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var results = new PagingResult<Application> { Value = new List<Application>() };
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            try
            {
                var account = await _dataverse.GetAccountByIdAsync(userSettings.AccountId);
                var lginId = account?.adoxio_LGINLinkId?.Id.ToString();
                if (string.IsNullOrEmpty(lginId)) return new JsonResult(results);

                var excludeStatuses = new List<int>
                {
                    (int)AdoxioApplicationStatusCodes.Cancelled,
                    (int)AdoxioApplicationStatusCodes.Refused,
                    (int)AdoxioApplicationStatusCodes.TerminatedAndRefunded
                };
                // no specific status filter — approved lgDecision across any status except excluded
                var includeStatuses = new List<int>();
                var (apps, totalCount) = await _dataverse.GetApplicationsByLginPagedAsync(
                    lginId,
                    includeStatuses,
                    lgDecision: (int)LGDecision.Approved,
                    excludeStatuses: excludeStatuses,
                    pageIndex: pageIndex,
                    pageSize: pageSize);

                results.Count = totalCount;
                foreach (var app in apps)
                    results.Value.Add(await app.ToViewModelAsync(_dataverse, _cache, _logger));
            }
            catch (Exception e)
            {
                var errorText = "Error getting resolved LG applications";
                _logger.LogError(e, errorText);
                return StatusCode(StatusCodes.Status500InternalServerError, errorText);
            }
            return new JsonResult(results);
        }


        /// <summary>
        ///     all in one function that is used on the OrgStructure ( ApplicationLicenseeChangesComponent ) page to get the
        ///     initial data.
        ///     This includes:
        ///     The Application data for the current org structure / licenseeChanges record
        ///     Application Changelogs for the Application record
        ///     Count of NonTerminatedApplications
        ///     Current Hierarachy
        /// </summary>
        /// <returns></returns>
        [HttpGet("licensee-data/{type}")]
        public async Task<OngoingLicenseeData> GetLicenseeData(string type)
        {
            var forceCreate = type == "create";

            var result = new OngoingLicenseeData();
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            try
            {
                var application = await GetCurrentLicenseeApplicationAsync(userSettings, forceCreate);

                if (application != null)
                {
                    result.Application = await application.ToViewModelAsync(_dataverse, _cache, _logger);
                    result.ChangeLogs = await DynamicsExtensions.GetApplicationChangeLogsAsync(_dataverse, result.Application.Id, _logger);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error getting application change logs");
            }

            result.CurrentHierarchy =
                await DynamicsExtensions.GetLegalEntityTreeAsync(_dataverse, userSettings.AccountId, _configuration);

            result.TreeRoot = ProcessLegalEntityTree(result.CurrentHierarchy, result.ChangeLogs);
            result.ChangeLogs = null;

            result.NonTerminatedApplications =
                await DynamicsExtensions.GetNotTerminatedCRSApplicationCountAsync(_dataverse, userSettings.AccountId);

            // Licences — DV
            result.Licenses = await LicenseModel.GetLicenseSummariesByLicenceeAsync(_dataverse, userSettings.AccountId, _cache);
            var transferredLicences = await LicenseModel.GetPaidLicenseSummariesOnTransferAsync(_dataverse, userSettings.AccountId, _cache);
            result.Licenses.AddRange(transferredLicences);

            return result;
        }

        /*
        *  Combines the associate tree with the changelogs
        */
        private LicenseeChangeLog ProcessLegalEntityTree(LegalEntity root, List<LicenseeChangeLog> currentChangeLogs)
        {
            // convert associate tree to licensee change log tree
            var tree = AssociateTreeToChangeLog(root);

            //merge application change logs into the tree that was derived from legal entities
            currentChangeLogs.ForEach(change =>
            {
                if (change.ChangeType == LicenseeChangeType.addLeadership
                    || change.ChangeType == LicenseeChangeType.removeLeadership
                    || change.ChangeType == LicenseeChangeType.updateLeadership)
                {
                    change.IsIndividual = true;
                    change.IsLeadershipIndividual = true;
                }

                if (change.ChangeType == LicenseeChangeType.addIndividualShareholder
                    || change.ChangeType == LicenseeChangeType.removeIndividualShareholder
                    || change.ChangeType == LicenseeChangeType.updateIndividualShareholder)
                {
                    change.IsIndividual = true;
                    change.IsShareholderIndividual = true;
                }

                if (!string.IsNullOrEmpty(change.LegalEntityId))
                {
                    // if changelog is for an existing associate
                    var matchingNode = tree.FindNodeByLegalEntityId(change.LegalEntityId);
                    if (matchingNode != null) matchingNode.UpdateValues(change);
                }
                else if (!string.IsNullOrEmpty(change.ParentLegalEntityId))
                {
                    // if changelog if a child of an existing associate
                    var parentNode = tree.FindNodeByLegalEntityId(change.ParentLegalEntityId);
                    if (parentNode != null) parentNode.Children.Add(change);
                }
                else if (!string.IsNullOrEmpty(change.ParentLicenseeChangeLogId))
                {
                    // if changelog if a child of another change log
                    var parentNode = tree.FindNodeByParentChangeLogId(change.ParentLicenseeChangeLogId);
                    if (parentNode != null) parentNode.Children.Add(change);
                }
            });

            return tree;
        }

        private LicenseeChangeLog AssociateTreeToChangeLog(LegalEntity node)
        {
            var newNode = new LicenseeChangeLog(node);
            if (node?.children != null && node.children.Count > 0)
            {
                var children = new List<LicenseeChangeLog>();
                foreach (var child in node.children)
                {
                    var childNode = AssociateTreeToChangeLog(child);
                    // childNode.ParentLicenseeChangeLog = newNode;

                    var isShareholderIndividual = childNode.IsIndividual == true && childNode.IsShareholderNew == true;
                    var isKeyPersonnel = childNode.IsIndividual == true && (
                        childNode.IsDirectorNew == true ||
                        childNode.IsManagerNew == true ||
                        childNode.IsOfficerNew == true ||
                        childNode.IsTrusteeNew == true ||
                        childNode.IsOwnerNew == true
                    );

                    //split the change log if it is both a shareholder and key-personnel
                    if (isShareholderIndividual && isKeyPersonnel)
                    {
                        var newIndividualNode = new LicenseeChangeLog(childNode)
                        {
                            Id = null, // force it to be a new record.
                            IsShareholderNew = false,
                            IsShareholderOld = false
                        };
                        children.Add(newIndividualNode);

                        childNode.IsManagerNew = false;
                        childNode.IsOfficerNew = false;
                        childNode.IsOwnerNew = false;
                        childNode.IsDirectorNew = false;
                        childNode.IsTrusteeNew = false;
                        childNode.IsManagerOld = false;
                        childNode.IsOfficerOld = false;
                        childNode.IsOwnerOld = false;
                        childNode.IsDirectorOld = false;
                        childNode.IsTrusteeOld = false;
                    }
                    else if (isShareholderIndividual)
                    {
                        childNode.IsShareholderIndividual = true;
                    }
                    else if (isKeyPersonnel)
                    {
                        childNode.IsLeadershipIndividual = true;
                    }

                    children.Add(childNode);
                }

                // sort the list by shares
                children.Sort((a, b) =>
                {
                    if (a.TotalSharesNew == null || b.TotalSharesNew == null) return 0;

                    return a.TotalSharesNew.Value.CompareTo(b.TotalSharesNew);
                });


                newNode.Children = children;
            }

            return newNode;
        }

        private async Task<adoxio_application_dv?> GetCurrentLicenseeApplicationAsync(UserSettings userSettings, bool forceCreate)
        {
            var applicationType = await _dataverse.GetApplicationTypeByNameAsync("Licensee Changes");

            adoxio_application_dv? result = null;

            if (!forceCreate && applicationType != null)
            {
                var excludeStatuses = new List<int>
                {
                    (int)AdoxioApplicationStatusCodes.Processed,
                    (int)AdoxioApplicationStatusCodes.Terminated,
                    (int)AdoxioApplicationStatusCodes.Cancelled,
                    (int)AdoxioApplicationStatusCodes.Approved,
                    (int)AdoxioApplicationStatusCodes.Refused,
                    (int)AdoxioApplicationStatusCodes.TerminatedAndRefunded
                };

                try
                {
                    var applications = await _dataverse.GetApplicationsByApplicantAndTypeAsync(
                        userSettings.AccountId,
                        applicationType.adoxio_applicationtypeId?.ToString(),
                        excludeStatuses
                    );
                    result = applications.OrderByDescending(a => a.CreatedOn).FirstOrDefault();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting licensee application");
                    result = null;
                }
            }

            if (result == null && applicationType != null)
            {
                var dvApp = new adoxio_application_dv
                {
                    adoxio_Applicant = new EntityReference("account", Guid.Parse(userSettings.AccountId)),
                    adoxio_ApplicationTypeId = new EntityReference("adoxio_applicationtype", applicationType.adoxio_applicationtypeId!.Value)
                };

                try
                {
                    var createdId = await _dataverse.CreateApplicationAsync(dvApp);
                    result = await _dataverse.GetApplicationByIdWithChildrenAsync(createdId.ToString());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error creating licensee application");
                    result = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Fetches a "Permanent Change to a Licensee" application.
        ///
        /// Fetches the application using the logged in user's account ID.
        /// If an "applicationId" is provided, will additionally filter results using that specific application id.
        ///
        /// If no application is found, it will create a new "Permanent Change to a Licensee" application and return it.
        /// </summary>
        /// <param name="userSettings"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <summary>
        private async Task<IActionResult> _GetPermanentChangesToLicenseeData(
            UserSettings userSettings,
            string applicationId = null
        )
        {
            PermanentChangesPageData data = new PermanentChangesPageData();

            // Get all licenses for the current user — DV
            data.Licences = await LicenseModel.GetLicenseSummariesByLicenceeAsync(_dataverse, userSettings.AccountId, _cache);

            // Attempt to fetch an existing in-progress application
            var existingApplication = await _GetExistingInProgressPermanentChangeApplication(
                userSettings,
                applicationId
            );

            // If no existing in-progress application is found, create and return a new application
            if (existingApplication == null)
            {
                var createdApplication = await _createPermanentChangeApplication(userSettings);
                data.Application = await createdApplication.ToViewModelAsync(_dataverse, _cache, _logger);
                return new JsonResult(data);
            }

            // If the existing application has an unpaid cannabis (primary) invoice, check/update the payment status
            if (
                existingApplication.adoxio_Invoice?.Id != null
                && existingApplication.adoxio_PrimaryApplicationInvoicePaid != adoxio_generalyesno_dv.Yes
            )
            {
                PaymentResult primaryInvoiceResult = await PaymentController
                    .GetCannabisPaymentStatus(existingApplication, _dataverse, _bcep)
                    .ConfigureAwait(true);

                data.Primary = primaryInvoiceResult?.TrnId == "0" ? null : primaryInvoiceResult;
            }

            // If the existing application has an unpaid liquor (secondary) invoice, check/update the payment status
            if (
                existingApplication.adoxio_SecondaryApplicationInvoice?.Id != null
                && existingApplication.adoxio_SecondaryApplicationInvoicePaid != adoxio_generalyesno_dv.Yes
            )
            {
                PaymentResult secondaryInvoiceResult = await PaymentController
                    .GetLiquorPaymentStatus(existingApplication, _dataverse, _bcep)
                    .ConfigureAwait(true);

                data.Secondary = secondaryInvoiceResult?.TrnId == "0" ? null : secondaryInvoiceResult;
            }

            // Fetch the existing record with all related data
            var existingApplicationData = await _dataverse.GetApplicationByIdWithChildrenAsync(
                existingApplication.adoxio_applicationId!.Value.ToString()
            );

            data.Application = await existingApplicationData.ToViewModelAsync(_dataverse, _cache, _logger);

            return new JsonResult(data);
        }

        /// <summary>
        /// Fetches a "Permanent Change to a Licensee" application as a result of a "Legal Entity Review".
        ///
        /// If no application is found, it will create a new "Permanent Change to a Licensee" application and return it.
        /// </summary>
        /// <param name="userSettings"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <summary>
        private async Task<IActionResult> _GetPermanentChangesToLicenseeDataForLegalEntityReview(
            UserSettings userSettings,
            string applicationId
        )
        {
            PermanentChangesPageData data = new PermanentChangesPageData();

            // Get all licenses — DV
            data.Licences = await LicenseModel.GetLicenseSummariesByLicenceeAsync(_dataverse, userSettings.AccountId, _cache);

            // Fetch the existing record with all related data
            var existingApplication = await _dataverse.GetApplicationByIdWithChildrenAsync(applicationId);

            // If no existing in-progress application is found, create and return a new application
            if (existingApplication == null)
            {
                var createdApplication = await _createPermanentChangeApplication(userSettings);
                data.Application = await createdApplication.ToViewModelAsync(_dataverse, _cache, _logger);
                return new JsonResult(data);
            }

            // If the existing application has an unpaid cannabis (primary) invoice, check/update the payment status
            if (
                existingApplication.adoxio_Invoice?.Id != null
                && existingApplication.adoxio_PrimaryApplicationInvoicePaid != adoxio_generalyesno_dv.Yes
            )
            {
                PaymentResult primaryInvoiceResult = await PaymentController
                    .GetCannabisPaymentStatus(existingApplication, _dataverse, _bcep)
                    .ConfigureAwait(true);

                data.Primary = primaryInvoiceResult?.TrnId == "0" ? null : primaryInvoiceResult;
            }

            // If the existing application has an unpaid liquor (secondary) invoice, check/update the payment status
            if (
                existingApplication.adoxio_SecondaryApplicationInvoice?.Id != null
                && existingApplication.adoxio_SecondaryApplicationInvoicePaid != adoxio_generalyesno_dv.Yes
            )
            {
                PaymentResult secondaryInvoiceResult = await PaymentController
                    .GetLiquorPaymentStatus(existingApplication, _dataverse, _bcep)
                    .ConfigureAwait(true);

                data.Secondary = secondaryInvoiceResult?.TrnId == "0" ? null : secondaryInvoiceResult;
            }

            data.Application = await existingApplication.ToViewModelAsync(_dataverse, _cache, _logger);

            return new JsonResult(data);
        }

        /// <summary>
        /// Fetches a "LE Review" application.
        /// </summary>
        /// <param name="userSettings"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <summary>
        private async Task<IActionResult> _GetLegalEntityReviewData(UserSettings userSettings, string applicationId)
        {
            // TODO: tiedhouse - Replace this type with a new LE Review specific one, as the "permanent change" and 
            // "le review" don't have similar, but not the exact same, data requirements.
            PermanentChangesPageData data = new PermanentChangesPageData();

            // Get all licenses — DV
            data.Licences = await LicenseModel.GetLicenseSummariesByLicenceeAsync(_dataverse, userSettings.AccountId, _cache);

            var application = await _dataverse.GetApplicationByIdWithChildrenAsync(applicationId);

            data.Application = await application.ToViewModelAsync(_dataverse, _cache, _logger);

            return new JsonResult(data);
        }

        /// <summary>
        /// Gets an existing in-progress Permanent Change to a Licensee application or creates a new one.
        ///
        /// If applicationId is provided, will fetch that specific record.
        /// If applicationId is not provided, will fetch the most recent Permanent change application for the user.
        ///
        /// If no application is found, it will create a new application.
        /// </summary>
        /// <remarks>
        /// An in-progress application is one that is not in a final/terminal status AND does not have a paid invoice.
        /// </remarks>
        /// <param name="userSettings"></param>
        /// <param name="applicationId">Filter results by a specific application ID. (Optional)</param>
        /// <returns></returns>
        private async Task<adoxio_application_dv?> _GetExistingInProgressPermanentChangeApplication(
            UserSettings userSettings,
            string applicationId = null
        )
        {
            var applicationType = await _dataverse.GetApplicationTypeByNameAsync("Permanent Change to a Licensee");

            if (applicationType == null)
            {
                _logger.LogError("Application type not found for 'Permanent Change to a Licensee' Application");
                throw new Exception("Application type not found for 'Permanent Change to a Licensee' Application");
            }

            var excludeStatuses = new List<int>
            {
                (int)AdoxioApplicationStatusCodes.Processed,
                (int)AdoxioApplicationStatusCodes.Terminated,
                (int)AdoxioApplicationStatusCodes.Cancelled,
                (int)AdoxioApplicationStatusCodes.Approved,
                (int)AdoxioApplicationStatusCodes.Refused,
                (int)AdoxioApplicationStatusCodes.TerminatedAndRefunded
            };

            try
            {
                var applications = await _dataverse.GetApplicationsByApplicantAndTypeAsync(
                    userSettings.AccountId,
                    applicationType.adoxio_applicationtypeId?.ToString(),
                    excludeStatuses,
                    requireStatecode0: true,
                    specificApplicationId: applicationId
                );

                var existingApplication = applications.OrderByDescending(a => a.CreatedOn).FirstOrDefault();

                // If the application has both invoices paid it is no longer "in-progress"
                bool hasInvoice =
                    existingApplication?.adoxio_Invoice?.Id != null
                    || existingApplication?.adoxio_SecondaryApplicationInvoice?.Id != null;
                bool primaryInvoicePaid =
                    existingApplication?.adoxio_Invoice?.Id == null
                    || existingApplication?.adoxio_PrimaryApplicationInvoicePaid == adoxio_generalyesno_dv.Yes;
                bool secondaryInvoicePaid =
                    existingApplication?.adoxio_SecondaryApplicationInvoice?.Id == null
                    || existingApplication?.adoxio_SecondaryApplicationInvoicePaid == adoxio_generalyesno_dv.Yes;
                bool existingApplicationIsPaid = hasInvoice && primaryInvoicePaid && secondaryInvoicePaid;

                if (existingApplicationIsPaid)
                    return null;

                return existingApplication;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting existing in-progress Permanent Change Application");
                throw;
            }
        }

        /// <summary>
        /// Creates and returns a new permanent change application.
        /// </summary>
        /// <param name="userSettings"></param>
        /// <param name="applicationType"></param>
        /// <returns></returns>
        private async Task<adoxio_application_dv> _createPermanentChangeApplication(UserSettings userSettings)
        {
            try
            {
                var applicationType = await _dataverse.GetApplicationTypeByNameAsync("Permanent Change to a Licensee");

                if (applicationType == null)
                {
                    _logger.LogError("Application type not found for Permanent Change Application");
                    throw new Exception("Application type not found for Permanent Change Application");
                }

                var dvApp = new adoxio_application_dv
                {
                    adoxio_Applicant = new EntityReference("account", Guid.Parse(userSettings.AccountId)),
                    adoxio_ApplicationTypeId = new EntityReference("adoxio_applicationtype", applicationType.adoxio_applicationtypeId!.Value)
                };

                var createdId = await _dataverse.CreateApplicationAsync(dvApp);
                return await _dataverse.GetApplicationByIdWithChildrenAsync(createdId.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating permanent change application");
                throw;
            }
        }

        /// <summary>
        /// Fetches a "Permanent Change to a Licensee" application.
        ///
        /// Fetches the application using the logged in user's account ID.
        /// If an "applicationId" is provided, will additionally filter results using that specific application id.
        ///
        /// If no application is found, it will create a new "Permanent Change to a Licensee" application and return it.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [HttpGet("permanent-change-to-licensee-data")]
        public async Task<IActionResult> GetPermanentChangesToLicenseeData([FromQuery] string applicationId = null)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            return await _GetPermanentChangesToLicenseeData(userSettings, applicationId);
        }

        /// <summary>
        /// Fetches a "Legal Entity Review" application.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <summary>
        [HttpGet("legal-entity-review-data")]
        public async Task<IActionResult> GetLegalEntityReviewData([FromQuery] string applicationId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            return await _GetLegalEntityReviewData(userSettings, applicationId);
        }

        /// GET all applications in Dynamics for the current user
        [HttpGet("ongoing-licensee-application-id")]
        public async Task<IActionResult> GetOngoingLicenseeApplicationId()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var application = await GetCurrentLicenseeApplicationAsync(userSettings, false);
            return new JsonResult(application?.adoxio_applicationId?.ToString());
        }

        /// <summary>
        /// Get the count of submitted and approved cannabis retail store applications for the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("current/cannabis-retail-store/submitted-count")]
        public async Task<JsonResult> GetCountForCurrentUserSubmittedApplications()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            var countCannabisRetailStore = await GetSubmittedCannabisRetailStoreCountByApplicantAsync(userSettings.AccountId);
            countCannabisRetailStore += await GetApprovedCannabisRetailStoreLicenceCountByApplicantAsync(userSettings.AccountId);

            return new JsonResult(countCannabisRetailStore);
        }

        /// <summary>
        /// Get the count of all approved applications for the current user.
        /// </summary>
        /// <returns>Number</returns>
        [HttpGet("current/approved-count")]
        public async Task<JsonResult> GetCountOfSubmittedApplicationsForCurrentUser()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            int count = await GetApprovedApplicationsCountByApplicantAsync(userSettings.AccountId);

            return new JsonResult(count);
        }

        /// <summary>
        ///     GET an Application by ID
        /// </summary>
        /// <param name="id">GUID of the Application to get</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplication(string id)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            _logger.LogDebug($"Application id = {id}, User id = {userSettings.AccountId}");

            var dvApplication = await _dataverse.GetApplicationByIdWithChildrenAsync(id);
            if (dvApplication == null) return NotFound();

            var allowLgAccess = await CurrentUserIsLgForApplicationAsync(dvApplication);
            if (!CurrentUserHasAccessToApplicationOwnedBy(dvApplication.adoxio_Applicant?.Id.ToString()) && !allowLgAccess)
                return new NotFoundResult();

            Application result = await dvApplication.ToViewModelAsync(_dataverse, _cache, _logger);

            // LCSD-8519: hydrate TiedHouse for Marketer apps
            if (result?.ApplicationType?.Name == "Marketing"
                && !string.IsNullOrEmpty(dvApplication.adoxio_Applicant?.Id.ToString()))
            {
                var tiedHouse = await _tiedHouseConnectionsRepository
                    .GetCannabisTiedHouseConnectionForUser(dvApplication.adoxio_Applicant.Id.ToString());
                if (tiedHouse != null)
                    result.TiedHouse = tiedHouse;
            }

            var spDocs = await _dataverse.GetSharePointDocLocsByObjectIdAsync(id);
            if (spDocs.Count == 0)
                await InitializeSharepointAsync(dvApplication);

            return new JsonResult(result);
        }

        private async Task<bool> CurrentUserIsLgForApplicationAsync(adoxio_application_dv application)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var accountId = GuidUtility.SanitizeGuidString(userSettings.AccountId);
            var account = await _dataverse.GetAccountByIdAsync(accountId);
            return application != null &&
                   application.adoxio_localgovindigenousnationid?.Id == account?.adoxio_LGINLinkId?.Id;
        }

        /// <summary>
        ///     Create an Application in Dynamics (POST)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] Application item)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var countCannabisRetailStore = await GetSubmittedCannabisRetailStoreCountByApplicantAsync(userSettings.AccountId);
            countCannabisRetailStore += await GetApprovedCannabisRetailStoreLicenceCountByApplicantAsync(userSettings.AccountId);

            if (countCannabisRetailStore >= 8 && item.ApplicationType.Name == "Cannabis Retail Store")
                return BadRequest("8 applications have already been submitted. Can not create more");

            var dvApp = new adoxio_application_dv();
            dvApp.CopyValues(item);
            dvApp.adoxio_Applicant = new EntityReference("account", Guid.Parse(userSettings.AccountId));

            var applicationType = await _dataverse.GetApplicationTypeByNameAsync(item.ApplicationType.Name);
            if (applicationType == null)
            {
                _logger.LogError($"Application type '{item.ApplicationType.Name}' not found");
                return BadRequest($"Application type '{item.ApplicationType.Name}' not found");
            }

            if (!string.IsNullOrEmpty(item.LicenseType))
            {
                var licenceType = await _dataverse.GetLicenceTypeByNameAsync(item.LicenseType);
                if (licenceType != null)
                    dvApp.adoxio_LicenceType = new EntityReference("adoxio_licencetype", licenceType.adoxio_licencetypeId!.Value);
            }

            if (!string.IsNullOrEmpty(item.LicenceSubCategory))
            {
                var subLicenceType = await _dataverse.GetLicenceSubCategoryByNameAsync(item.LicenceSubCategory);
                if (subLicenceType != null)
                    dvApp.adoxio_LicenceSubCategoryId = new EntityReference("adoxio_licencesubcategory", subLicenceType.adoxio_licencesubcategoryId!.Value);
            }

            // copy more data for endorsements
            // LCSD-5744 - Also copy more data for Change to Hours of Liquor Service (After Midnight) application
            if (applicationType.adoxio_IsEndorsement == true ||
                (applicationType.adoxio_name != null && applicationType.adoxio_name == "Change to Hours of Liquor Service (After Midnight)"))
            {
                dvApp.adoxio_EstablishmentAddressCity = item.EstablishmentAddressCity;
                dvApp.adoxio_EstablishmentAddressStreet = item.EstablishmentAddressStreet;
                dvApp.adoxio_EstablishmentAddressPostalCode = item.EstablishmentAddressPostalCode;
                dvApp.adoxio_EstablishmentParcelID = item.EstablishmentParcelId;
                dvApp.adoxio_EstablishmentEmail = item.EstablishmentEmail;
                dvApp.adoxio_EstablishmentPhone = item.EstablishmentPhone;

                if (!string.IsNullOrEmpty(item?.IndigenousNationId))
                    dvApp.adoxio_localgovindigenousnationid = new EntityReference("adoxio_localgovindigenousnation", Guid.Parse(item.IndigenousNationId));

                if (!string.IsNullOrEmpty(item?.PoliceJurisdictionId))
                    dvApp.adoxio_PoliceJurisdictionId = new EntityReference("adoxio_policejurisdiction", Guid.Parse(item.PoliceJurisdictionId));

                if (!string.IsNullOrEmpty(item?.ParentApplicationId))
                    dvApp.adoxio_ParentApplicationID = new EntityReference("adoxio_application", Guid.Parse(item.ParentApplicationId));
            }

            dvApp.adoxio_ApplicationTypeId = new EntityReference("adoxio_applicationtype", applicationType.adoxio_applicationtypeId!.Value);

            adoxio_application_dv? dvCreated;
            try
            {
                //LCSD-5779 create TiedHouseExemption
                if (item.WillHaveTiedHouseExemption.HasValue && item.WillHaveTiedHouseExemption.Value)
                {
                    var tiedHouseAppType = await _dataverse.GetApplicationTypeByNameAsync(ApplicationTypeNames.TiedHouseExemption);
                    var exemptionApp = new adoxio_application_dv
                    {
                        adoxio_ApplicationTypeId = new EntityReference("adoxio_applicationtype", tiedHouseAppType!.adoxio_applicationtypeId!.Value),
                        adoxio_TiedHouseExemption = true,
                        adoxio_manufacturerproductionamountforprevyear = item.ManufacturerProductionAmountForPrevYear,
                        adoxio_manufacturerproductionamountunit = (adoxio_application_adoxio_manufacturerproductionamountunit_dv?)(int?)item.ManufacturerProductionAmountUnit
                    };

                    if (!string.IsNullOrEmpty(item.AssignedLicenceId))
                    {
                        var assignedLic = await _dataverse.GetLicenceByIdAsync(item.AssignedLicenceId);
                        if (assignedLic != null)
                        {
                            var licRef = new EntityReference("adoxio_licences", assignedLic.adoxio_licencesId!.Value);
                            exemptionApp.adoxio_RelatedLicence = licRef;
                            exemptionApp.adoxio_AssignedLicence = licRef;
                            if (assignedLic.adoxio_Licencee != null)
                                exemptionApp.adoxio_Applicant = new EntityReference("account", assignedLic.adoxio_Licencee.Id);
                            if (assignedLic.adoxio_establishment != null)
                                exemptionApp.adoxio_LicenceEstablishment = new EntityReference("adoxio_establishment", assignedLic.adoxio_establishment.Id);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ParentApplicationId))
                        exemptionApp.adoxio_ParentApplicationID = new EntityReference("adoxio_application", Guid.Parse(item.ParentApplicationId));

                    var exemptionId = await _dataverse.CreateApplicationAsync(exemptionApp);
                    dvCreated = await _dataverse.GetApplicationByIdAsync(exemptionId.ToString());
                }
                else
                {
                    var createdId = await _dataverse.CreateApplicationAsync(dvApp);
                    dvCreated = await _dataverse.GetApplicationByIdAsync(createdId.ToString());

                    // For Marketing: create TiedHouseConnection after app creation
                    if (item.ApplicationType.Name == "Marketing" && dvCreated != null)
                    {
                        var conn = new adoxio_tiedhouseconnection_dv
                        {
                            adoxio_ConnectionType = adoxio_tiedhouseconnection_adoxio_connectiontype.Marketer,
                            adoxio_Application = new EntityReference("adoxio_application", dvCreated.adoxio_applicationId!.Value)
                        };
                        await _dataverse.CreateTiedHouseConnectionAsync(conn);
                    }
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error creating application");
                throw;
            }

            if (dvCreated == null)
            {
                _logger.LogError("Unable to retrieve newly created application.");
                throw new Exception("Error creating Licence Application.");
            }

            if (dvCreated.adoxio_JobNumber == null)
            {
                _logger.LogDebug("Unable to get the Job Number for the Application.");
                throw new Exception("Error creating Licence Application.");
            }

            if (item.ServiceAreas?.Count > 0)
                await AddServiceAreasToApplicationAsync(item.ServiceAreas, dvCreated.adoxio_applicationId!.Value.ToString());

            if (item.OutsideAreas?.Count > 0)
                await AddServiceAreasToApplicationAsync(item.OutsideAreas, dvCreated.adoxio_applicationId!.Value.ToString());

            await InitializeSharepointAsync(dvCreated);

            return new JsonResult(await dvCreated.ToViewModelAsync(_dataverse, _cache, _logger));
        }


        [HttpPost("covid")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCovidApplication([FromBody] CovidApplication item)
        {
            if (string.IsNullOrEmpty(_configuration["FEATURE_COVID_APPLICATION"])) return BadRequest();

            var dvApp = new adoxio_application_dv
            {
                adoxio_name = item.Name,
                adoxio_EstablishmentPropsedName = item.EstablishmentName,
                adoxio_EstablishmentAddressStreet = item.EstablishmentAddressStreet,
                adoxio_EstablishmentAddressCity = item.EstablishmentAddressCity,
                adoxio_EstablishmentAddressPostalCode = item.EstablishmentAddressPostalCode,
                adoxio_EstablishmentParcelID = item.EstablishmentParcelId,
                adoxio_EstablishmentPhone = item.EstablishmentPhone,
                adoxio_EstablishmentEmail = item.EstablishmentEmail,
                adoxio_ContactPersonFirstName = item.ContactPersonFirstName,
                adoxio_ContactPersonLastName = item.ContactPersonLastName,
                adoxio_Role = item.ContactPersonRole,
                adoxio_Email = item.ContactPersonEmail,
                adoxio_ContactPersonPhone = item.ContactPersonPhone,
                adoxio_AuthorizedtoSubmit = item.AuthorizedToSubmit,
                adoxio_AdditionalPropertyInformation = item.AdditionalPropertyInformation,
                adoxio_IsApplicationComplete = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno?)(int?)item.IsApplicationComplete,
                adoxio_ApplicantType = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicanttypecodes.PrivateCorporation,
                adoxio_proposedestablishmentisALR = item.ProposedEstablishmentIsAlr,
                adoxio_NameofApplicant = item.NameOfApplicant,
                adoxio_AddressStreet = item.AddressStreet,
                adoxio_AddressCity = item.AddressCity,
                adoxio_AddressPostalCode = item.AddressPostalCode
            };

            if (!string.IsNullOrEmpty(item.LicenceType))
            {
                var licenceType = await _dataverse.GetLicenceTypeByNameAsync(item.LicenceType);
                if (licenceType != null)
                    dvApp.adoxio_LicenceType = new EntityReference("adoxio_licencetype", licenceType.adoxio_licencetypeId!.Value);
            }

            string applicationTypeName = "Temporary Extension of Licensed Area";
            var applicationType = await _dataverse.GetApplicationTypeByNameAsync(applicationTypeName);
            if (applicationType == null)
            {
                _logger.LogError($"Unable to find the COVID Application Type for {applicationTypeName}");
            }
            else
            {
                dvApp.adoxio_ApplicationTypeId = new EntityReference("adoxio_applicationtype", applicationType.adoxio_applicationtypeId!.Value);
            }

            adoxio_application_dv? dvCreated;
            try
            {
                var createdId = await _dataverse.CreateApplicationAsync(dvApp);
                _logger.LogInformation($"CREATED COVID APPLICATION {createdId}");
                dvCreated = await _dataverse.GetApplicationByIdAsync(createdId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating COVID application");
                throw;
            }

            if (dvCreated == null) throw new Exception("Error creating COVID Application.");

            await InitializeSharepointAsync(dvCreated);

            return new JsonResult(await dvCreated.ToCovidViewModelAsync(_dataverse, _cache, _logger));
        }

        private async Task InitializeSharepointAsync(adoxio_application_dv application)
        {
            var appId = application.adoxio_applicationId?.ToString();
            var jobNumber = application.adoxio_JobNumber;
            var applicationIdCleaned = appId?.ToUpper().Replace("-", "");
            var folderName = $"{jobNumber}_{applicationIdCleaned}";
            _fileManagerClient.CreateFolderIfNotExist(_logger, SharePointConstants.ApplicationFolderInternalName, folderName);
            await _dataverse.CreateEntitySharePointDocumentLocationAsync("adoxio_application", appId, folderName, folderName);
        }

        /// <summary>
        ///     Update a Dynamics Application (PUT)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplication([FromBody] Application item, string id)
        {
            if (id != item.Id)
            {
                _logger.LogError("UpdateApplication - Error updating application");
                return BadRequest();
            }

            var applicationId = new Guid(id);
            var existingApp = await _dataverse.GetApplicationByIdWithChildrenAsync(id);
            if (existingApp == null) return NotFound();

            var allowLgAccess = await CurrentUserIsLgForApplicationAsync(existingApp);
            if (!CurrentUserHasAccessToApplicationOwnedBy(existingApp.adoxio_Applicant?.Id.ToString()) && !allowLgAccess)
                throw new Exception("User does not have access to the application");

            var dvApp = new adoxio_application_dv();
            dvApp.Id = applicationId;
            dvApp.CopyValues(item);

            if (!string.IsNullOrEmpty(item.LicenceSubCategory))
            {
                var subLicenceType = await _dataverse.GetLicenceSubCategoryByNameAsync(item.LicenceSubCategory);
                if (subLicenceType != null)
                    dvApp.adoxio_LicenceSubCategoryId = new EntityReference("adoxio_licencesubcategory", subLicenceType.adoxio_licencesubcategoryId!.Value);
            }

            if (item.ApplicationStatus == AdoxioApplicationStatusCodes.PendingForLGFNPFeedback
                || item.ApplicationStatus == AdoxioApplicationStatusCodes.UnderReview)
            {
                dvApp.statuscode = (adoxio_application_statuscode?)(int?)item.ApplicationStatus;
            }

            dvApp.adoxio_localgovindigenousnationid = !string.IsNullOrEmpty(item?.IndigenousNation?.Id)
                ? new EntityReference("adoxio_localgovindigenousnation", Guid.Parse(item.IndigenousNation.Id))
                : null;

            dvApp.adoxio_PoliceJurisdictionId = !string.IsNullOrEmpty(item?.PoliceJurisdiction?.id)
                ? new EntityReference("adoxio_policejurisdiction", Guid.Parse(item.PoliceJurisdiction.id))
                : null;

            try
            {
                await RemoveServiceAreasFromApplicationAsync(item.Id);

                if (item.ServiceAreas?.Count > 0)
                    await AddServiceAreasToApplicationAsync(item.ServiceAreas, item.Id);

                if (item.OutsideAreas?.Count > 0)
                    await AddServiceAreasToApplicationAsync(item.OutsideAreas, item.Id);

                if (item.CapacityArea?.Count > 0 && item.CapacityArea.FirstOrDefault().Capacity.HasValue)
                    await AddServiceAreasToApplicationAsync(item.CapacityArea, item.Id);

                if ((bool)item.ApplicationType?.ShowHoursOfSale)
                {
                    try
                    {
                        var hoursEntity = await _dataverse.GetHoursOfServiceByApplicationIdAsync(id);
                        var patchHours = new adoxio_hoursofservice_dv
                        {
                            adoxio_SundayClose = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursSundayClose,
                            adoxio_SundayOpen = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursSundayOpen,
                            adoxio_MondayClose = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursMondayClose,
                            adoxio_MondayOpen = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursMondayOpen,
                            adoxio_TuesdayClose = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursTuesdayClose,
                            adoxio_TuesdayOpen = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursTuesdayOpen,
                            adoxio_WednesdayClose = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursWednesdayClose,
                            adoxio_WednesdayOpen = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursWednesdayOpen,
                            adoxio_ThursdayClose = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursThursdayClose,
                            adoxio_ThursdayOpen = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursThursdayOpen,
                            adoxio_FridayClose = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursFridayClose,
                            adoxio_FridayOpen = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursFridayOpen,
                            adoxio_SaturdayClose = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursSaturdayClose,
                            adoxio_SaturdayOpen = (adoxio_servicehoursoptionsethours?)(int?)item.ServiceHoursSaturdayOpen,
                            adoxio_RequestOutsideServiceHours = item.RequestOutsideServiceHours
                        };

                        if (hoursEntity != null)
                        {
                            patchHours.Id = hoursEntity.adoxio_hoursofserviceId!.Value;
                            await _dataverse.UpdateHoursOfServiceAsync(patchHours);
                        }
                        else
                        {
                            patchHours.adoxio_Application = new EntityReference("adoxio_application", applicationId);
                            await _dataverse.CreateHoursOfServiceAsync(patchHours);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating/creating application hours of service");
                        throw;
                    }
                }

                //LCSD-5779 create TiedHouseExemption
                if (string.IsNullOrEmpty(item.Id) && item.WillHaveTiedHouseExemption.HasValue && item.WillHaveTiedHouseExemption.Value && item.TiedHouse == null)
                {
                    var tiedHouseAppType = await _dataverse.GetApplicationTypeByNameAsync(ApplicationTypeNames.TiedHouseExemption);
                    var exemptionApp = new adoxio_application_dv
                    {
                        adoxio_ApplicationTypeId = new EntityReference("adoxio_applicationtype", tiedHouseAppType!.adoxio_applicationtypeId!.Value),
                        adoxio_TiedHouseExemption = true,
                        adoxio_manufacturerproductionamountforprevyear = 0
                    };
                    await _dataverse.CreateApplicationAsync(exemptionApp);
                }

                if (item.ApplicationExtension != null)
                {
                    try
                    {
                        await UpsertApplicationExtensionAsync(item.ApplicationExtension, item.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error upserting application extension");
                        throw;
                    }
                }

                await _dataverse.UpdateApplicationAsync(dvApp);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error updating application");
                throw;
            }

            try
            {
                var updated = await _dataverse.GetApplicationByIdWithChildrenAsync(id);
                return new JsonResult(await updated!.ToViewModelAsync(_dataverse, _cache, _logger));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error getting updated application");
                throw;
            }
        }


        [HttpPut("legal_entity/{id}")]
        public async Task<IActionResult> SubmitLegalEntityApplication([FromBody] Application item, string id)
        {
            if (id != item.Id) return BadRequest();

            var dvApp = await _dataverse.GetApplicationByIdAsync(id);
            if (dvApp == null) return NotFound();

            var allowLgAccess = await CurrentUserIsLgForApplicationAsync(dvApp);
            if (!CurrentUserHasAccessToApplicationOwnedBy(dvApp.adoxio_Applicant?.Id.ToString()) && !allowLgAccess)
                throw new Exception("User doesn't have an access the application");

            var patch = new adoxio_application_dv();
            patch.CopyValues(item);

            if (patch.statuscode == adoxio_application_statuscode.Incompleteinforeq)
            {
                try
                {
                    patch.Id = new Guid(id);
                    patch.statuscode = adoxio_application_statuscode.UnderReview;
                    await _dataverse.UpdateApplicationAsync(patch);
                    var updated = await _dataverse.GetApplicationByIdAsync(id);
                    return new JsonResult(await updated!.ToViewModelAsync(_dataverse, _cache, _logger));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating application");
                    throw;
                }
            }
            else
            {
                throw new Exception("Error submitting Legal entity incorrect Application Status");
            }
        }

        /// <summary>
        ///     Cancel an Application.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelApplication(string id)
        {
            var app = await _dataverse.GetApplicationByIdAsync(id);
            if (app == null) return new NotFoundResult();
            if (!CurrentUserHasAccessToApplicationOwnedBy(app.adoxio_Applicant?.Id.ToString()))
                return new NotFoundResult();

            var patch = new adoxio_application_dv
            {
                Id = new Guid(id),
                statuscode = adoxio_application_statuscode.Terminated
            };
            try
            {
                await _dataverse.UpdateApplicationAsync(patch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling application");
                throw;
            }

            return NoContent(); // 204
        }

        /// <summary>
        ///     Process an application.  Only useful for automated testing.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/process")]
        public async Task<JsonResult> ProcessApplication(string id)
        {
            if (_env.IsProduction()) return new JsonResult("This API is not available outside a development environment.");


            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);


            // query the Dynamics system to get the account record.
            if (userSettings.AccountId != null && !userSettings.IsNewUserRegistration &&
                userSettings.AccountId.Length > 0)
                try
                {
                    await _dataverse.ExecuteWorkflowAsync("0a78e6dc-8d62-480f-909f-c104051cf467", id);
                    return new JsonResult("OK");
                }
                catch (Exception e)
                {
                    throw e;
                }

            return new JsonResult("This API is not available to an unregistered user.");
        }

        [HttpGet("{id}/processEndorsement")]
        public async Task<IActionResult> ProcessEndorsementApplication(string id)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");


            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);


            // query the Dynamics system to get the account record.
            if (userSettings.AccountId != null && !userSettings.IsNewUserRegistration &&
                userSettings.AccountId.Length > 0)
                try
                {
                    await _dataverse.ExecuteWorkflowAsync("e755b96c-1c0d-4893-98dc-53ec980d57a1", id);
                    return new JsonResult("OK");
                }
                catch (Exception e)
                {
                    throw e;
                }

            return BadRequest("This API is not available to an unregistered user.");
        }

        /// <summary>
        ///     Delete an Application.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteApplication(string id)
        {
            var app = await _dataverse.GetApplicationByIdAsync(id);
            if (app == null) return new NotFoundResult();
            if (!CurrentUserHasAccessToApplicationOwnedBy(app.adoxio_Applicant?.Id.ToString()))
                return new NotFoundResult();
            await _dataverse.DeleteApplicationAsync(id);
            return NoContent(); // 204
        }

        /// <summary>
        /// Get Autocomplete data for a JobNumber search
        /// 2024-03-25 LCSD-6368 waynezen; Tied House form autocomplete for Application JobNumber
        /// </summary>
        /// <param name="jobnumber">The name to filter by using startswith</param>
        /// <returns>Dictionary of key value pairs with accountid and name as the pairs</returns>
        [HttpGet("autocomplete")]
        [Authorize(Policy = "Business-User")]
        public async Task<List<RelatedLicence>> GetAutocomplete(string jobnumber)
        {
            var results = new List<RelatedLicence>();
            try
            {
                var excludeStatuses = new List<int>
                {
                    (int)AdoxioApplicationStatusCodes.Terminated,
                    (int)AdoxioApplicationStatusCodes.Cancelled,
                    (int)AdoxioApplicationStatusCodes.Refused,
                    (int)AdoxioApplicationStatusCodes.TerminatedAndRefunded
                };
                var applications = await _dataverse.GetApplicationsByJobNumberContainsAsync(jobnumber, excludeStatuses);

                foreach (var app in applications)
                {
                    if (app.adoxio_JobNumber?.Contains(jobnumber) == true)
                    {
                        // 2024-04-29 LCSD-6368; further filtering to make sure record(s) have a valid Licence #
                        var licNumber = app.GetAttributeValue<AliasedValue>("lic.adoxio_licencenumber")?.Value as string;
                        var licExpiryRaw = app.GetAttributeValue<AliasedValue>("lic.adoxio_expirydate")?.Value;
                        DateTime? expiryDate = licExpiryRaw is DateTime dt ? dt : (DateTime?)null;

                        if (!string.IsNullOrEmpty(licNumber) && expiryDate > DateTime.Now)
                        {
                            results.Add(new RelatedLicence
                            {
                                Id = app.adoxio_JobNumber,
                                Name = app.adoxio_Applicant?.Name,
                                EstablishmentName = app.adoxio_EstablishmentPropsedName,
                                Streetaddress = app.adoxio_EstablishmentAddressStreet,
                                City = app.adoxio_EstablishmentAddressCity,
                                Provstate = "BC",
                                Country = "CANADA",
                                PostalCode = app.adoxio_EstablishmentAddressPostalCode,
                                Licensee = "",
                                JobNumber = app.adoxio_JobNumber,
                                LicenceNumber = licNumber,
                                Valid = expiryDate.HasValue && expiryDate.Value >= DateTime.Now
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting autocomplete data.");
            }
            return results;
        }



        [HttpPost("{id}/covidDelete")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCovidApplication(string id)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");
            var app = await _dataverse.GetApplicationByIdAsync(id);
            if (app == null) return new NotFoundResult();
            await _dataverse.DeleteApplicationAsync(id);
            return NoContent(); // 204
        }

        /// <summary>
        /// Get or Create a Permanent Change to Licensee Application (PCL) as a result of a Legal Entity Review (LE).
        /// <remarks>
        /// A "PCL as a result of an LE Review" is a regular PCL application, which has been created on behalf of an
        /// LE Review application. The PCL application is linked to the LE Review application via the
        /// `RelatedLeOrPclApplication` field.
        /// </remarks>
        /// </summary>
        /// <param name="id">Either the ID of the LE Review application or the PCL application.</param>
        /// <returns></returns>
        [HttpGet("pcl-for-le-review/{id}")]
        public async Task<IActionResult> GetOrCreatePermanentChangeForLegalEntityReviewApplicationAsync(string id)
        {
            try
            {
                UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

                var application = await _dataverse.GetApplicationByIdAsync(id);
                if (application == null)
                    return NotFound();

                var leReviewType = await _dataverse.GetApplicationTypeByNameAsync("LE Review");

                // If the provided `id` does not point to an LE Review Application, assume it is a PCL Application.
                if (leReviewType == null || application.adoxio_ApplicationTypeId?.Id != leReviewType.adoxio_applicationtypeId)
                {
                    return await _GetPermanentChangesToLicenseeDataForLegalEntityReview(
                        userSettings,
                        application.adoxio_applicationId!.Value.ToString()
                    );
                }

                // If LE review application is linked to a PCL application, return the PCL Application data.
                if (application.adoxio_ApplicationExtension?.Id != null)
                {
                    var leExtension = await _dataverse.GetApplicationExtensionByIdAsync(
                        application.adoxio_ApplicationExtension.Id.ToString()
                    );
                    if (leExtension?.adoxio_relatedleorpclapplication?.Id != null)
                    {
                        return await _GetPermanentChangesToLicenseeDataForLegalEntityReview(
                            userSettings,
                            leExtension.adoxio_relatedleorpclapplication.Id.ToString()
                        );
                    }
                }

                // LE Review Application is not linked to a PCL Application — create one and mutually link them.

                var pclType = await _dataverse.GetApplicationTypeByNameAsync("Permanent Change to a Licensee");
                if (pclType == null)
                    throw new Exception("Application type 'Permanent Change to a Licensee' not found");

                var createdPclId = await _dataverse.CreateApplicationAsync(
                    CopyLEReviewApplicationToPCL(application, pclType.adoxio_applicationtypeId!.Value)
                );

                // Link the LE Review application extension to the new PCL application
                var leExt = new adoxio_applicationextension_dv
                {
                    adoxio_relatedleorpclapplication = new EntityReference("adoxio_application", createdPclId)
                };
                if (application.adoxio_ApplicationExtension?.Id is Guid existingLeExtId)
                {
                    leExt.adoxio_applicationextensionId = existingLeExtId;
                    await _dataverse.UpdateApplicationExtensionAsync(leExt);
                    await LinkApplicationExtensionToApplication(application.adoxio_applicationId!.Value.ToString(), existingLeExtId.ToString());
                }
                else
                {
                    var newLeExtId = await _dataverse.CreateApplicationExtensionAsync(leExt);
                    await LinkApplicationExtensionToApplication(application.adoxio_applicationId!.Value.ToString(), newLeExtId.ToString());
                }

                // Link the new PCL application extension back to the LE Review application (no extension exists yet on a new PCL)
                var pclExt = new adoxio_applicationextension_dv
                {
                    adoxio_relatedleorpclapplication = new EntityReference("adoxio_application", application.adoxio_applicationId!.Value)
                };
                var newPclExtId = await _dataverse.CreateApplicationExtensionAsync(pclExt);
                await LinkApplicationExtensionToApplication(createdPclId.ToString(), newPclExtId.ToString());

                return await _GetPermanentChangesToLicenseeDataForLegalEntityReview(
                    userSettings,
                    createdPclId.ToString()
                );
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error getting PCL application for LE Review application");
                throw;
            }
        }

        /// <summary>
        /// Fetches the user's in-progress Legal Entity Review applications.
        /// </summary>
        /// <remarks>
        /// Business rule: The user is only expected to have 1 in-progress Legal Entity Review at a time.
        /// </remarks>
        /// <returns>A list of in-progress Legal Entity Review applications</returns>
        [HttpGet("get-in-progress-legal-entity-review")]
        public async Task<IActionResult> UserHasInProgressLegalEntityReview()
        {
            try
            {
                UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

                var applicationType = await _dataverse.GetApplicationTypeByNameAsync("LE Review");
                if (applicationType == null)
                {
                    _logger.LogWarning("Application type 'LE Review' not found");
                    return new JsonResult(false);
                }

                var inProgressStatuses = new HashSet<int>
                {
                    (int)AdoxioApplicationStatusCodes.Intake,
                    (int)AdoxioApplicationStatusCodes.Incomplete,
                    (int)AdoxioApplicationStatusCodes.Submitted,
                    (int)AdoxioApplicationStatusCodes.UnderReview,
                    (int)AdoxioApplicationStatusCodes.LicenseeActionRequired,
                    (int)AdoxioApplicationStatusCodes.ApplicationAssessment
                };

                var allApps = await _dataverse.GetApplicationsByApplicantAndTypeAsync(
                    userSettings.AccountId,
                    applicationType.adoxio_applicationtypeId?.ToString(),
                    excludeStatuses: null,
                    requireStatecode0: true
                );

                var applicationViewModels = new List<Application>();
                foreach (var app in allApps.Where(a => inProgressStatuses.Contains((int?)a.statuscode ?? -1)))
                    applicationViewModels.Add(await app.ToViewModelAsync(_dataverse, _cache, _logger));

                return new JsonResult(applicationViewModels);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error fetching in-progress LE Review applications");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching in-progress LE Review applications");
            }
        }

        private adoxio_application_dv CopyLEReviewApplicationToPCL(adoxio_application_dv leReview, Guid pclApplicationTypeId)
        {
            return new adoxio_application_dv
            {
                adoxio_Applicant = new EntityReference("account", leReview.adoxio_Applicant.Id),
                adoxio_ApplicationTypeId = new EntityReference("adoxio_applicationtype", pclApplicationTypeId),
                adoxio_CSInternalTransferofShares = leReview.adoxio_CSInternalTransferofShares,
                adoxio_CSExternalTransferofShares = leReview.adoxio_CSExternalTransferofShares,
                adoxio_CSChangeofDirectorsorOfficers = leReview.adoxio_CSChangeofDirectorsorOfficers,
                adoxio_CSNameChangeLicenseeCorporation = leReview.adoxio_CSNameChangeLicenseeCorporation,
                adoxio_CSNameChangeLicenseePartnership = leReview.adoxio_CSNameChangeLicenseePartnership,
                adoxio_CSNameChangeLicenseeSociety = leReview.adoxio_CSNameChangeLicenseeSociety,
                adoxio_CSNameChangePerson = leReview.adoxio_CSNameChangePerson,
                adoxio_CSAdditionofReceiverorExecutor = leReview.adoxio_CSAdditionofReceiverorExecutor,
                adoxio_CSChangeToTiedHouse = leReview.adoxio_CSChangeToTiedHouse,
            };
        }

        /// <summary>
        ///     Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
        private bool CurrentUserHasAccessToApplicationOwnedBy(string accountId)
        {
            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            // For now, check if the account id matches the user's account.
            // TODO there may be some account relationships in the future
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
                return userSettings.AccountId == accountId;

            // if current user doesn't have an account they are probably not logged in
            return false;
        }

        private async Task RemoveServiceAreasFromApplicationAsync(string applicationId)
        {
            try
            {
                var areas = await _dataverse.GetServiceAreasByApplicationIdAsync(applicationId);
                foreach (var area in areas)
                    try
                    {
                        await _dataverse.DeleteServiceAreaAsync(area.adoxio_serviceareaId!.Value.ToString());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected error deleting a service area.");
                    }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error getting service areas.");
            }
        }

        private async Task AddServiceAreasToApplicationAsync(List<CapacityArea> areas, string applicationId)
        {
            var appRef = new EntityReference("adoxio_application", Guid.Parse(applicationId));
            foreach (var area in areas)
            {
                var serviceArea = new adoxio_servicearea_dv
                {
                    adoxio_ApplicationId = appRef,
                    adoxio_areacategory = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_servicearea_adoxio_areacategory?)(int?)area.AreaCategory,
                    adoxio_arealocation = area.AreaLocation,
                    adoxio_areanumber = area.AreaNumber,
                    adoxio_capacity = area.Capacity,
                    adoxio_isindoor = area.IsIndoor,
                    adoxio_isoutdoor = area.IsOutdoor,
                    adoxio_ispatio = area.IsPatio,
                    adoxio_dateadded = DateTime.UtcNow,
                    adoxio_dateupdated = DateTime.UtcNow,
                    adoxio_TemporaryExtensionArea = area.IsTemporaryExtensionArea
                };
                await _dataverse.CreateServiceAreaAsync(serviceArea);
            }
        }

        /// <summary>
        /// Updates or creates an application extension record.
        /// - If the application extension does not exist, it will be created and linked to the application.
        /// - If it exists, it will be updated with the provided values.
        /// </summary>
        /// <param name="applicationExtension"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        private async Task UpsertApplicationExtensionAsync(
            ApplicationExtension applicationExtension,
            string applicationId
        )
        {
            var dvExt = new adoxio_applicationextension_dv
            {
                adoxio_hasLiquortiedhouseownershiporcontrol = (adoxio_generalyesno_dv?)(int?)applicationExtension.HasLiquorTiedHouseOwnershipOrControl,
                adoxio_hasliquortiedhousethirdpartyassociations = (adoxio_generalyesno_dv?)(int?)applicationExtension.HasLiquorTiedHouseThirdPartyAssociations,
                adoxio_hasliquortiedhousefamilymemberinvolvement = (adoxio_generalyesno_dv?)(int?)applicationExtension.HasLiquorTiedHouseFamilyMemberInvolvement
            };

            if (Guid.TryParse(applicationExtension.Id, out var extGuid))
            {
                dvExt.adoxio_applicationextensionId = extGuid;
                await _dataverse.UpdateApplicationExtensionAsync(dvExt);
                await LinkApplicationExtensionToApplication(applicationId, extGuid.ToString());
            }
            else
            {
                var createdId = await _dataverse.CreateApplicationExtensionAsync(dvExt);
                await LinkApplicationExtensionToApplication(applicationId, createdId.ToString());
            }
        }

        /// <summary>
        /// Links an application extension record to an application record.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="extensionId"></param>
        /// <returns></returns>
        private async Task LinkApplicationExtensionToApplication(string applicationId, string extensionId)
        {
            var patch = new adoxio_applicationextension_dv
            {
                adoxio_applicationextensionId = Guid.Parse(extensionId),
                adoxio_Application = new EntityReference("adoxio_application", Guid.Parse(applicationId))
            };
            await _dataverse.UpdateApplicationExtensionAsync(patch);
        }
    }
}
