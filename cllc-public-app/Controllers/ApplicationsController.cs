using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Extensions;
using Gov.Lclb.Cllb.Public.Models;
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
using Microsoft.Rest;
using Newtonsoft.Json;
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
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IWebHostEnvironment _env;
        private readonly FileManagerClient _fileManagerClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public ApplicationsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, FileManagerClient fileClient,
            IWebHostEnvironment env, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(ApplicationsController));
            _fileManagerClient = fileClient;
            _env = env;
        }


        /// <summary>
        ///     Get a license application by applicant id
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        private List<ApplicationSummary> GetApplicationSummariesByApplicant(string applicantId)
        {
            var result = new List<ApplicationSummary>();

            var dynamicsApplicationList = _dynamicsClient.GetApplicationListByApplicant(applicantId);
            if (dynamicsApplicationList != null)
                foreach (var dynamicsApplication in dynamicsApplicationList)
                {
                    var endorsements = new List<string>();
                    //get endorsement application types
                    if (dynamicsApplication.AdoxioLicenceType != null &&
                        dynamicsApplication.AdoxioPaymentrecieved == true)
                    {
                        var expand = new List<string> {"adoxio_licencetypes_applicationtypes"};
                        var licenceType =
                            _dynamicsClient.Licencetypes.GetByKey(dynamicsApplication._adoxioLicencetypeValue,
                                expand: expand);
                        if (licenceType?.AdoxioLicencetypesApplicationtypes != null)
                            endorsements = licenceType.AdoxioLicencetypesApplicationtypes
                                .Where(type => type.AdoxioIsendorsement == true)
                                .Select(type => type.AdoxioName)
                                .ToList();
                    }

                    // hide terminated applications from view.
                    if (dynamicsApplication.Statuscode == null || dynamicsApplication.Statuscode !=
                        (int) AdoxioApplicationStatusCodes.Terminated
                        && dynamicsApplication.Statuscode != (int) AdoxioApplicationStatusCodes.Refused
                        && dynamicsApplication.Statuscode != (int) AdoxioApplicationStatusCodes.Cancelled
                        && dynamicsApplication.Statuscode != (int) AdoxioApplicationStatusCodes.TerminatedAndRefunded)
                    {
                        var row = dynamicsApplication.ToSummaryViewModel();
                        row.Endorsements = endorsements;
                        result.Add(row);
                    }
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

            var dynamicsApplicationList = _dynamicsClient.GetApplicationListByApplicant(applicantId);
            if (dynamicsApplicationList != null)
            {
                foreach (var dynamicsApplication in dynamicsApplicationList)
                    // hide terminated applications from view.
                    if (dynamicsApplication.Statuscode == null || dynamicsApplication.Statuscode !=
                        (int) AdoxioApplicationStatusCodes.Terminated
                        && dynamicsApplication.Statuscode != (int) AdoxioApplicationStatusCodes.Refused
                        && dynamicsApplication.Statuscode != (int) AdoxioApplicationStatusCodes.Cancelled
                        && dynamicsApplication.Statuscode != (int) AdoxioApplicationStatusCodes.TerminatedAndRefunded)
                        result.Add(await dynamicsApplication.ToViewModel(_dynamicsClient, _cache, _logger));

                // second pass to determine if transfer or location change is in progress.

                foreach (var item in result)
                    if (item.LicenseType == "Cannabis Retail Store" && item.ApplicationStatus ==
                                                                    AdoxioApplicationStatusCodes.Approved
                                                                    && item.AssignedLicence != null &&
                                                                    item.AssignedLicence.ExpiryDate > DateTime.Now
                        )
                        // determine if there is a transfer in progress.
                        item.IsLocationChangeInProgress = FindRelatedApplication(result, item, "CRS Location Change");
                    // item.isTransferInProgress = FindRelatedApplication(result, item, "CRS Transfer of Ownership");
            }

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
        ///     Gets the number of active licences
        /// </summary>
        /// <param name="licenceeId"></param>
        /// <returns></returns>
        private int GetApprovedLicenceCountByApplicant(string licenceeId)
        {
            var result = 0;
            if (!string.IsNullOrEmpty(licenceeId))
            {
                var filter = $"_adoxio_licencee_value eq {licenceeId}";
                filter += $" and statuscode eq {(int) LicenceStatusCodes.Active}";
                var expand = new List<string> {"adoxio_LicenceType"};
                try
                {
                    result = _dynamicsClient.Licenceses.Get(filter: filter, expand: expand).Value
                        .Count(licence => licence.AdoxioLicenceType.AdoxioName == "Cannabis Retail Store");
                }
                catch (HttpOperationException)
                {
                    result = 0;
                }
            }

            return result;
        }


        /// <summary>
        ///     Gets the number of applications that are submitted
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        private int GetSubmittedCountByApplicant(string applicantId)
        {
            var result = 0;
            if (!string.IsNullOrEmpty(applicantId))
            {
                var filter =
                    $"_adoxio_applicant_value eq {applicantId} and adoxio_paymentrecieved eq true and statuscode ne {(int) AdoxioApplicationStatusCodes.Terminated}";
                filter += $" and statuscode ne {(int) AdoxioApplicationStatusCodes.Cancelled}";
                filter += $" and statuscode ne {(int) AdoxioApplicationStatusCodes.Approved}";
                filter += $" and statuscode ne {(int) AdoxioApplicationStatusCodes.Refused}";
                filter += $" and statuscode ne {(int) AdoxioApplicationStatusCodes.TerminatedAndRefunded}";

                var applicationType = _dynamicsClient.GetApplicationTypeByName("Cannabis Retail Store");
                if (applicationType != null)
                    filter += $" and _adoxio_applicationtypeid_value eq {applicationType.AdoxioApplicationtypeid} ";

                try
                {
                    result = _dynamicsClient.Applications.Get(filter: filter).Value.Count;
                }
                catch (HttpOperationException)
                {
                    result = 0;
                }
            }

            return result;
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
        public JsonResult GetCurrentUserApplications()
        {
            // get the current user.
            var temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // GET all applications in Dynamics by applicant using the account Id assigned to the user logged in
            var adoxioApplications = GetApplicationSummariesByApplicant(userSettings.AccountId);
            return new JsonResult(adoxioApplications);
        }

        /// GET all applications of the given application type in Dynamics for the current user
        [HttpGet("current/by-type/{applicationType}")]
        public JsonResult GetCurrentUserLgApprovalApplications(string applicationType)
        {
            var results = new List<ApplicationSummary>();
            // get the current user.
            var temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            var filter = $"_adoxio_applicant_value eq {userSettings.AccountId}";
            var appType = _dynamicsClient.GetApplicationTypeByName(applicationType);
            if (appType != null)
                filter += $" and _adoxio_applicationtypeid_value eq {appType.AdoxioApplicationtypeid} ";

            try
            {
                var applications = _dynamicsClient.Applications.Get(filter: filter).Value.ToList();
                if (applications != null)
                    foreach (var dynamicsApplication in applications)
                        results.Add(dynamicsApplication.ToSummaryViewModel());
            }
            catch (HttpOperationException e)
            {
                _logger.LogError(e, "Error getting licensee application");
                throw;
            }

            return new JsonResult(results);
        }

        /// GET all local government approval applications in Dynamics for the current user
        [HttpGet("current/lg-approvals")]
        public async Task<JsonResult> GetLgApprovalApplications()
        {
            var results = new List<Application>();
            // get the current user.
            var temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            try
            {
                // get user account
                var accountId = GuidUtility.SanitizeGuidString(userSettings.AccountId);
                var account = await _dynamicsClient.GetAccountByIdAsync(new Guid(accountId));

                var filter = $"_adoxio_localgovindigenousnationid_value eq {account._adoxioLginlinkidValue}";
                filter += $" and statuscode eq {(int) AdoxioApplicationStatusCodes.PendingForLGFNPFeedback}";

                var expand = new List<string>
                {
                    "adoxio_Applicant",
                    "adoxio_localgovindigenousnationid",
                    "adoxio_application_SharePointDocumentLocations",
                    "adoxio_application_adoxio_tiedhouseconnection_Application",
                    "adoxio_AssignedLicence",
                    "adoxio_ApplicationTypeId",
                    "adoxio_LicenceFeeInvoice",
                    "adoxio_Invoice",
                    "adoxio_application_SharePointDocumentLocations"
                };

                var applications = _dynamicsClient.Applications.Get(filter: filter, expand: expand).Value.ToList();
                if (applications != null)
                    foreach (var dynamicsApplication in applications)
                    {
                        var viewModel = await dynamicsApplication.ToViewModel(_dynamicsClient, _cache, _logger);
                        var resolutionFiles = await FileController.GetListFilesInFolder(
                            dynamicsApplication.AdoxioApplicationid, "application", "LGIN Resolution", _dynamicsClient,
                            _fileManagerClient, _logger);
                        if (resolutionFiles.Count > 0) viewModel.ResolutionDocsUploaded = true;
                        results.Add(viewModel);
                    }
            }
            catch (HttpOperationException e)
            {
                _logger.LogError(e, "Error getting licensee applications by type");
                throw;
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
            var temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            try
            {
                var application = GetCurrentLicenseeApplication(userSettings, forceCreate);

                if (!string.IsNullOrEmpty(application._adoxioApplicationtypeidValue))
                {
                    application.AdoxioApplicationTypeId = await _dynamicsClient
                        .GetApplicationTypeById(application._adoxioApplicationtypeidValue).ConfigureAwait(true);
                    if (application.AdoxioApplicationTypeId != null)
                    {
                        var filter = $"_adoxio_applicationtype_value eq {application._adoxioApplicationtypeidValue}";
                        try
                        {
                            var typeContents = _dynamicsClient.Applicationtypecontents.Get(filter: filter).Value;
                            application.AdoxioApplicationTypeId
                                .AdoxioApplicationtypeAdoxioApplicationtypecontentApplicationType = typeContents;
                        }
                        catch (HttpOperationException e)
                        {
                            _logger.LogError(e, "Error getting type contents");
                        }
                    }
                }

                result.Application =
                    await application.ToViewModel(_dynamicsClient, _cache, _logger).ConfigureAwait(true);


                result.ChangeLogs = _dynamicsClient.GetApplicationChangeLogs(result.Application.Id, _logger);
            }
            catch (HttpOperationException e)
            {
                _logger.LogError(e, "Error getting application change logs");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error getting application change logs");
            }


            result.CurrentHierarchy =
                _dynamicsClient.GetLegalEntityTree(userSettings.AccountId, _logger, _configuration);

            result.TreeRoot = ProcessLegalEntityTree(result.CurrentHierarchy, result.ChangeLogs);

            // no need to send change logs to the client side
            result.ChangeLogs = null;

            result.NonTerminatedApplications =
                _dynamicsClient.GetNotTerminatedCRSApplicationCount(userSettings.AccountId);

            // get all licenses in Dynamics by Licencee using the account Id assigned to the user logged in
            result.Licenses = _dynamicsClient.GetLicensesByLicencee(userSettings.AccountId, _cache);
            var transferredLicences = _dynamicsClient.GetPaidLicensesOnTransfer(userSettings.AccountId);
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

        private MicrosoftDynamicsCRMadoxioApplication GetCurrentLicenseeApplication(UserSettings userSettings,
            bool forceCreate)
        {
            MicrosoftDynamicsCRMadoxioApplication result = null;
            var applicationType = _dynamicsClient.GetApplicationTypeByName("Licensee Changes");

            if (!forceCreate)
            {
                string[] expand =
                {
                    "adoxio_localgovindigenousnationid",
                    "adoxio_application_SharePointDocumentLocations",
                    "adoxio_application_adoxio_tiedhouseconnection_Application",
                    "adoxio_AssignedLicence",
                    "adoxio_ApplicationTypeId",
                    "adoxio_LicenceFeeInvoice",
                    "adoxio_Invoice",
                    "adoxio_application_SharePointDocumentLocations"
                };

                // GET all licensee change applications in Dynamics by applicant using the account Id assigned to the user logged in
                var filter =
                    $"_adoxio_applicant_value eq {userSettings.AccountId} and statuscode ne {(int) AdoxioApplicationStatusCodes.Processed} and statuscode ne {(int) AdoxioApplicationStatusCodes.Terminated}";
                // filter += $" and adoxio_isapplicationcomplete ne 1";
                filter += $" and statuscode ne {(int) AdoxioApplicationStatusCodes.Cancelled}";
                filter += $" and statuscode ne {(int) AdoxioApplicationStatusCodes.Approved}";
                filter += $" and statuscode ne {(int) AdoxioApplicationStatusCodes.Refused}";
                filter += $" and statuscode ne {(int) AdoxioApplicationStatusCodes.TerminatedAndRefunded}";
        
                if (applicationType != null)
                    filter += $" and _adoxio_applicationtypeid_value eq {applicationType.AdoxioApplicationtypeid} ";

                try
                {
                    var applications = _dynamicsClient.Applications.Get(filter: filter, expand: expand).Value
                        .OrderByDescending(app => app.Createdon);
                    var application = applications.FirstOrDefault(); // Get the latest application
                    if (application != null)
                        result = application;
                    else
                        result = null;
                }
                catch (HttpOperationException e)
                {
                    _logger.LogError(e, "Error getting licensee application");
                    result = null;
                }
            }

            if (result == null && applicationType != null)
            {
                // create one.
                var account = _dynamicsClient.GetAccountById(userSettings.AccountId);
                result = new MicrosoftDynamicsCRMadoxioApplication
                {
                    AdoxioApplicanttype = account.AdoxioBusinesstype,
                    AdoxioApplicantODataBind = _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId),
                    // set application type relationship 
                    AdoxioApplicationTypeIdODataBind = _dynamicsClient.GetEntityURI("adoxio_applicationtypes",
                        applicationType.AdoxioApplicationtypeid)
                };

                try
                {
                    result = _dynamicsClient.Applications.Create(result);
                    result = _dynamicsClient.GetApplicationByIdWithChildren(result.AdoxioApplicationid).GetAwaiter()
                        .GetResult();
                }
                catch (HttpOperationException e)
                {
                    _logger.LogError(e, "Error creating licensee application");
                    result = null;
                }
            }

            return result;
        }
        
        [HttpGet("permanent-changes-to-licensee-data")]
        public IActionResult GetPermanetChangesToLicenseeData(){
            var result = new OngoingLicenseeData();
            var temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // get all licenses in Dynamics by Licencee using the account Id assigned to the user logged in
            var result = _dynamicsClient.GetLicensesByLicencee(userSettings.AccountId, _cache);

            return result;

   
        }

        /// GET all applications in Dynamics for the current user
        [HttpGet("ongoing-licensee-application-id")]
        public IActionResult GetOngoingLicenseeApplicationId()
        {
            JsonResult result;
            var temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            try
            {
                var application = GetCurrentLicenseeApplication(userSettings, false);
                if (application != null)
                    result = new JsonResult(application.AdoxioApplicationid);
                else
                    result = new JsonResult(null);
            }
            catch (HttpOperationException e)
            {
                _logger.LogError(e, "Error getting licensee application");
                throw;
            }

            return result;
        }

        /// GET submitted applications in Dynamics for the current user
        [HttpGet("current/submitted-count")]
        public JsonResult GetCountForCurrentUserSubmittedApplications()
        {
            // get the current user.
            var temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // GET all applications in Dynamics by applicant using the account Id assigned to the user logged in
            var count = GetSubmittedCountByApplicant(userSettings.AccountId);
            count += GetApprovedLicenceCountByApplicant(userSettings.AccountId);
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
            // get the current user.
            var temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            _logger.LogDebug("Application id = " + id);
            _logger.LogDebug("User id = " + userSettings.AccountId);

            Application result;
            var dynamicsApplication = await _dynamicsClient.GetApplicationByIdWithChildren(Guid.Parse(id));
            if (dynamicsApplication == null) return NotFound();

            var allowLgAccess = await CurrentUserIsLgForApplication(dynamicsApplication);
            if (!CurrentUserHasAccessToApplicationOwnedBy(dynamicsApplication._adoxioApplicantValue)
                && !allowLgAccess)
                return new NotFoundResult();
            result = await dynamicsApplication.ToViewModel(_dynamicsClient, _cache, _logger);


            if (dynamicsApplication.AdoxioApplicationSharePointDocumentLocations.Count == 0)
                await InitializeSharepoint(dynamicsApplication);

            return new JsonResult(result);
        }

        private async Task<bool> CurrentUserIsLgForApplication(MicrosoftDynamicsCRMadoxioApplication application)
        {
            var temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // get user account
            var accountId = GuidUtility.SanitizeGuidString(userSettings.AccountId);
            var account = await _dynamicsClient.GetAccountByIdAsync(new Guid(accountId));

            // make sure the application and account have matching local government values
            var isLgForApplication = application != null &&
                                     application._adoxioLocalgovindigenousnationidValue ==
                                     account._adoxioLginlinkidValue;
            return isLgForApplication;
        }

        private string GetApplicationFolderName(MicrosoftDynamicsCRMadoxioApplication application)
        {
            var applicationIdCleaned = application.AdoxioApplicationid.ToUpper().Replace("-", "");
            var folderName = $"{application.AdoxioJobnumber}_{applicationIdCleaned}";
            return folderName;
        }

        


        /// <summary>
        ///     Create an Application in Dynamics (POST)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] Application item)
        {
            // for association with current user
            var userJson = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(userJson);
            var count = GetSubmittedCountByApplicant(userSettings.AccountId);
            count += GetApprovedLicenceCountByApplicant(userSettings.AccountId);

            if (count >= 8 && item.ApplicationType.Name == "Cannabis Retail Store")
                return BadRequest("8 applications have already been submitted. Can not create more");
            var adoxioApplication = new MicrosoftDynamicsCRMadoxioApplication();
            // copy received values to Dynamics Application
            adoxioApplication.CopyValues(item);
            adoxioApplication.AdoxioApplicanttype = (int?) item.ApplicantType;

            // fix for an invalid licence sub category
            if (adoxioApplication.AdoxioLicencesubcategory == 0) adoxioApplication.AdoxioLicencesubcategory = null;

            try
            {
                // set license type relationship 
                if (!string.IsNullOrEmpty(item.LicenseType))
                {
                    var adoxioLicencetype = _dynamicsClient.GetAdoxioLicencetypeByName(item.LicenseType);
                    adoxioApplication.AdoxioLicenceTypeODataBind = _dynamicsClient.GetEntityURI("adoxio_licencetypes",
                        adoxioLicencetype.AdoxioLicencetypeid);
                }

                // set licence subtype
                if (!string.IsNullOrEmpty(item.LicenceSubCategory))
                {
                    var adoxioSubLicencetype = _dynamicsClient.GetAdoxioSubLicencetypeByName(item.LicenseType);
                    adoxioApplication.AdoxioLicenceSubCategoryODataBind =
                        _dynamicsClient.GetEntityURI("adoxio_licencesubcategories",
                            adoxioSubLicencetype.AdoxioLicencesubcategoryid);
                }


                // set account relationship
                adoxioApplication.AdoxioApplicantODataBind =
                    _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId);

                // set application type relationship 
                var applicationType = _dynamicsClient.GetApplicationTypeByName(item.ApplicationType.Name);

                // copy more data for endorsements
                if (applicationType.AdoxioIsendorsement == true)
                {
                    adoxioApplication.AdoxioEstablishmentaddress = item.EstablishmentAddress;
                    adoxioApplication.AdoxioEstablishmentaddresscity = item.EstablishmentAddressCity;
                    adoxioApplication.AdoxioEstablishmentaddressstreet = item.EstablishmentAddressStreet;
                    adoxioApplication.AdoxioEstablishmentaddresspostalcode = item.EstablishmentAddressPostalCode;
                    adoxioApplication.AdoxioEstablishmentparcelid = item.EstablishmentParcelId;
                    adoxioApplication.AdoxioEstablishmentemail = item.EstablishmentEmail;
                    adoxioApplication.AdoxioEstablishmentphone = item.EstablishmentPhone;

                    // Indigenous nation association
                    if (!string.IsNullOrEmpty(item?.IndigenousNationId))
                        adoxioApplication.AdoxioLocalgovindigenousnationidODataBind =
                            _dynamicsClient.GetEntityURI("adoxio_localgovindigenousnations", item.IndigenousNationId);

                    // Police Jurisdiction association
                    if (!string.IsNullOrEmpty(item?.PoliceJurisdictionId))
                        adoxioApplication.AdoxioPoliceJurisdictionIdODataBind =
                            _dynamicsClient.GetEntityURI("adoxio_policejurisdictions", item.PoliceJurisdictionId);

                    if (!string.IsNullOrEmpty(item?.ParentApplicationId))
                        adoxioApplication.AdoxioParentApplicationIDODataBind =
                            _dynamicsClient.GetEntityURI("adoxio_applications", item.ParentApplicationId);
                }

                adoxioApplication.AdoxioApplicationTypeIdODataBind =
                    _dynamicsClient.GetEntityURI("adoxio_applicationtypes", applicationType.AdoxioApplicationtypeid);

                if (item.ApplicationType.Name == "Marketing")
                    // create tiedhouse relationship
                    adoxioApplication.AdoxioApplicationAdoxioTiedhouseconnectionApplication =
                        new List<MicrosoftDynamicsCRMadoxioTiedhouseconnection>
                        {
                            new MicrosoftDynamicsCRMadoxioTiedhouseconnection
                            {
                                AdoxioConnectiontype = (int?) TiedHouseConnectionType.Marketer
                            }
                        };

                // create application
                adoxioApplication = _dynamicsClient.Applications.Create(adoxioApplication);

                if (item.ServiceAreas != null && item.ServiceAreas.Count > 0)
                    AddServiceAreasToApplication(item.ServiceAreas, adoxioApplication.AdoxioApplicationid);

                if (item.OutsideAreas != null && item.OutsideAreas.Count > 0)
                    AddServiceAreasToApplication(item.OutsideAreas, adoxioApplication.AdoxioApplicationid);
            }
            catch (HttpOperationException httpOperationException)
            {
                var applicationId = _dynamicsClient.GetCreatedRecord(httpOperationException, null);
                if (!string.IsNullOrEmpty(applicationId) && Guid.TryParse(applicationId, out var applicationGuid))
                {
                    adoxioApplication = await _dynamicsClient.GetApplicationById(applicationGuid);
                }
                else
                {
                    _logger.LogError(httpOperationException, "Error creating application");
                    // fail if we can't create.
                    throw httpOperationException;
                }
            }

            // in case the job number is not there, try getting the record from the server.
            if (adoxioApplication.AdoxioJobnumber == null)
            {
                _logger.LogDebug("AdoxioJobnumber is null, fetching record again.");
                var id = Guid.Parse(adoxioApplication.AdoxioApplicationid);
                adoxioApplication = await _dynamicsClient.GetApplicationById(id);
            }

            if (adoxioApplication.AdoxioJobnumber == null)
            {
                _logger.LogDebug("Unable to get the Job Number for the Application.");
                throw new Exception("Error creating Licence Application.");
            }

            await InitializeSharepoint(adoxioApplication);

            return new JsonResult(await adoxioApplication.ToViewModel(_dynamicsClient, _cache, _logger));
        }


        [HttpPost("covid")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCovidApplication([FromBody] CovidApplication item)
        {
            // check to see if the feature is on.
            if (string.IsNullOrEmpty(_configuration["FEATURE_COVID_APPLICATION"])) return BadRequest();


            var adoxioApplication = new MicrosoftDynamicsCRMadoxioApplication();
            adoxioApplication.CopyValuesForCovidApplication(item);

            adoxioApplication.AdoxioApplicanttype = 845280000; // private corp - change to public user.

            // set license type relationship 
            if (!string.IsNullOrEmpty(item.LicenceType))
            {
                var adoxioLicencetype = _dynamicsClient.GetAdoxioLicencetypeByName(item.LicenceType);
                adoxioApplication.AdoxioLicenceTypeODataBind =
                    _dynamicsClient.GetEntityURI("adoxio_licencetypes", adoxioLicencetype.AdoxioLicencetypeid);
            }

            // set application type relationship 
            var applicationType = _dynamicsClient.GetApplicationTypeByName("Temporary Extension of Licensed Area");
            adoxioApplication.AdoxioApplicationTypeIdODataBind = _dynamicsClient.GetEntityURI("adoxio_applicationtypes",
                applicationType.AdoxioApplicationtypeid);

            try
            {
                // create application
                adoxioApplication = _dynamicsClient.Applications.Create(adoxioApplication);
                _logger.LogInformation($"CREATED COVID APPLICATION {adoxioApplication.AdoxioApplicationid}");
            }
            catch (HttpOperationException httpOperationException)
            {
                var applicationId = _dynamicsClient.GetCreatedRecord(httpOperationException, null);
                if (!string.IsNullOrEmpty(applicationId) && Guid.TryParse(applicationId, out var applicationGuid))
                {
                    adoxioApplication = await _dynamicsClient.GetApplicationById(applicationGuid);
                }
                else
                {
                    _logger.LogError(httpOperationException, "Error creating COVID application");
                    // fail if we can't create.
                    throw httpOperationException;
                }
            }

            await InitializeSharepoint(adoxioApplication);

            return new JsonResult(await adoxioApplication.ToCovidViewModel(_dynamicsClient, _cache, _logger));
        }

        private async Task InitializeSharepoint(MicrosoftDynamicsCRMadoxioApplication application)
        {
            // create a SharePointDocumentLocation link
            var folderName = application.GetDocumentFolderName();
            //var name = application.AdoxioApplicationid + " Files";

            _fileManagerClient.CreateFolderIfNotExist(_logger, ApplicationDocumentUrlTitle, folderName);

            _dynamicsClient.CreateEntitySharePointDocumentLocation("application", application.AdoxioApplicationid, folderName, folderName);
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
            if (id != item.Id) return BadRequest();

            //Prepare application for update
            var applicationId = new Guid(id);
            var application = await _dynamicsClient.GetApplicationById(applicationId);

            var allowLgAccess = await CurrentUserIsLgForApplication(application);
            if (!CurrentUserHasAccessToApplicationOwnedBy(application._adoxioApplicantValue) && !allowLgAccess)
                return new NotFoundResult();

            application = new MicrosoftDynamicsCRMadoxioApplication();

            application.CopyValues(item);

            // set licence subtype
            if (!string.IsNullOrEmpty(item.LicenceSubCategory))
            {
                var subLicencetype = _dynamicsClient.GetAdoxioSubLicencetypeByName(item.LicenceSubCategory);
                application.AdoxioLicenceSubCategoryODataBind =
                    _dynamicsClient.GetEntityURI("adoxio_licencesubcategories",
                        subLicencetype.AdoxioLicencesubcategoryid);
            }

            if (item.ApplicationStatus == AdoxioApplicationStatusCodes.PendingForLGFNPFeedback)
                application.Statuscode = (int?) item.ApplicationStatus;

            try
            {
                // Indigenous nation association
                if (!string.IsNullOrEmpty(item?.IndigenousNation?.Id))
                    application.AdoxioLocalgovindigenousnationidODataBind =
                        _dynamicsClient.GetEntityURI("adoxio_localgovindigenousnations", item.IndigenousNation.Id);
                else
                    //remove reference
                    await _dynamicsClient.Applications.DeleteReferenceAsync(item.Id,
                        "adoxio_localgovindigenousnationid");

                // Police Jurisdiction association
                if (!string.IsNullOrEmpty(item?.PoliceJurisdiction?.id))
                    application.AdoxioPoliceJurisdictionIdODataBind =
                        _dynamicsClient.GetEntityURI("adoxio_policejurisdictions", item.PoliceJurisdiction.id);
                else
                    //remove reference
                    await _dynamicsClient.Applications.DeleteReferenceAsync(item.Id, "adoxio_PoliceJurisdictionId");

                RemoveServiceAreasFromApplication(item.Id);
                if (item.ServiceAreas != null && item.ServiceAreas.Count > 0)
                    AddServiceAreasToApplication(item.ServiceAreas, item.Id);
                if (item.OutsideAreas != null && item.OutsideAreas.Count > 0)
                    AddServiceAreasToApplication(item.OutsideAreas, item.Id);
                if (item.CapacityArea != null && item.CapacityArea.Count > 0)
                    AddServiceAreasToApplication(item.CapacityArea, item.Id);

                if ((bool) item.ApplicationType?.ShowHoursOfSale)
                    try
                    {
                        // get entityid
                        var filter = $"_adoxio_application_value eq {id}";
                        var hoursEntity = _dynamicsClient.Hoursofservices.Get(filter: filter).Value.FirstOrDefault();
                        var patchHoursEntity = new MicrosoftDynamicsCRMadoxioHoursofservice
                        {
                            AdoxioSundayclose = (int?) item.ServiceHoursSundayClose,
                            AdoxioSundayopen = (int?) item.ServiceHoursSundayOpen,
                            AdoxioMondayclose = (int?) item.ServiceHoursMondayClose,
                            AdoxioMondayopen = (int?) item.ServiceHoursMondayOpen,
                            AdoxioTuesdayclose = (int?) item.ServiceHoursTuesdayClose,
                            AdoxioTuesdayopen = (int?) item.ServiceHoursTuesdayOpen,
                            AdoxioWednesdayclose = (int?) item.ServiceHoursWednesdayClose,
                            AdoxioWednesdayopen = (int?) item.ServiceHoursWednesdayOpen,
                            AdoxioThursdayclose = (int?) item.ServiceHoursThursdayClose,
                            AdoxioThursdayopen = (int?) item.ServiceHoursThursdayOpen,
                            AdoxioFridayclose = (int?) item.ServiceHoursFridayClose,
                            AdoxioFridayopen = (int?) item.ServiceHoursFridayOpen,
                            AdoxioSaturdayclose = (int?) item.ServiceHoursSaturdayClose,
                            AdoxioSaturdayopen = (int?) item.ServiceHoursSaturdayOpen
                        };

                        if (hoursEntity != null)
                        {
                            _dynamicsClient.Hoursofservices.Update(hoursEntity.AdoxioHoursofserviceid,
                                patchHoursEntity);
                        }
                        else
                        {
                            // Create hours of service
                            var applicationUri = _dynamicsClient.GetEntityURI("adoxio_applications", id);
                            patchHoursEntity.ApplicationODataBind = applicationUri;
                            _dynamicsClient.Hoursofservices.Create(patchHoursEntity);
                        }
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException,
                            "Error updating/creating application hours of service");
                        throw;
                    }

                _dynamicsClient.Applications.Update(id, application);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating application");
                // fail if we can't create.
                throw httpOperationException;
            }

            application = await _dynamicsClient.GetApplicationByIdWithChildren(applicationId);

            return new JsonResult(await application.ToViewModel(_dynamicsClient, _cache, _logger));
        }

        /// <summary>
        ///     Cancel an Application.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelApplication(string id)
        {
            // get the application.
            var applicationId = new Guid(id);

            var adoxioApplication = await _dynamicsClient.GetApplicationById(applicationId);
            if (adoxioApplication == null) return new NotFoundResult();

            if (!CurrentUserHasAccessToApplicationOwnedBy(adoxioApplication._adoxioApplicantValue))
                return new NotFoundResult();

            // set the status to Terminated.
            var patchRecord = new MicrosoftDynamicsCRMadoxioApplication
            {
                //StatusCodeODataBind = ((int)AdoxioApplicationStatusCodes.Terminated).ToString()
                Statuscode = (int) AdoxioApplicationStatusCodes.Terminated
            };

            try
            {
                _dynamicsClient.Applications.Update(id, patchRecord);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error cancelling application");
                // fail if we can't create.
                throw httpOperationException;
            }


            return NoContent(); // 204
        }

        /// <summary>
        ///     Process an application.  Only useful for automated testing.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/process")]
        public async Task<IActionResult> ProcessApplication(string id)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");


            // get the current user.
            var sessionSettings = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(sessionSettings);


            // query the Dynamics system to get the account record.
            if (userSettings.AccountId != null && !userSettings.IsNewUserRegistration &&
                userSettings.AccountId.Length > 0)
                // call the bpf to process the application.
                try
                {
                    // this needs to be the guid for the published workflow.
                    await _dynamicsClient.Workflows.ExecuteWorkflowWithHttpMessagesAsync(
                        "0a78e6dc-8d62-480f-909f-c104051cf467", id);
                    return Ok("OK");
                }
                catch (HttpOperationException httpOperationException)
                {
                    var error = httpOperationException.Response.Content;
                    return BadRequest(error);
                }
                catch (Exception e)
                {
                    throw e;
                }

            return BadRequest("This API is not available to an unregistered user.");
        }

        [HttpGet("{id}/processEndorsement")]
        public async Task<IActionResult> ProcessEndorsementApplication(string id)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");


            // get the current user.
            var sessionSettings = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(sessionSettings);


            // query the Dynamics system to get the account record.
            if (userSettings.AccountId != null && !userSettings.IsNewUserRegistration &&
                userSettings.AccountId.Length > 0)
                // call the bpf to process the application.
                try
                {
                    // this needs to be the guid for the published workflow.
                    await _dynamicsClient.Workflows.ExecuteWorkflowWithHttpMessagesAsync(
                        "e755b96c-1c0d-4893-98dc-53ec980d57a1", id);
                    return Ok("OK");
                }
                catch (HttpOperationException httpOperationException)
                {
                    var error = httpOperationException.Response.Content;
                    return BadRequest(error);
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
            // get the application.
            var applicationId = new Guid(id);

            var adoxioApplication = await _dynamicsClient.GetApplicationById(applicationId);
            if (adoxioApplication == null) return new NotFoundResult();

            if (!CurrentUserHasAccessToApplicationOwnedBy(adoxioApplication._adoxioApplicantValue))
                return new NotFoundResult();


            await _dynamicsClient.Applications.DeleteAsync(applicationId.ToString());

            return NoContent(); // 204
        }

        [HttpPost("{id}/covidDelete")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCovidApplication(string id)
        {
            if (_env.IsProduction()) return BadRequest("This API is not available outside a development environment.");

            // get the application.
            var applicationId = new Guid(id);

            var adoxioApplication = await _dynamicsClient.GetApplicationById(applicationId);
            if (adoxioApplication == null) return new NotFoundResult();


            await _dynamicsClient.Applications.DeleteAsync(applicationId.ToString());

            return NoContent(); // 204
        }

        /// <summary>
        ///     Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
        private bool CurrentUserHasAccessToApplicationOwnedBy(string accountId)
        {
            // get the current user.
            var temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // For now, check if the account id matches the user's account.
            // TODO there may be some account relationships in the future
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
                return userSettings.AccountId == accountId;

            // if current user doesn't have an account they are probably not logged in
            return false;
        }

        private void RemoveServiceAreasFromApplication(string applicationId)
        {
            var filter = $"_adoxio_applicationid_value eq {applicationId}";
            try
            {
                var areas = _dynamicsClient.Serviceareas.Get(filter: filter).Value;
                foreach (var area in areas)
                    try
                    {
                        _dynamicsClient.Serviceareas.Delete(area.AdoxioServiceareaid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Unexpected error deleting a service area.");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected error deleting a service area.");
                    }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Unexpected error getting service areas.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error getting service areas.");
            }
        }

        private void AddServiceAreasToApplication(List<CapacityArea> areas, string applicationId)
        {
            var applicationUri = _dynamicsClient.GetEntityURI("adoxio_applications", applicationId);
            foreach (var area in areas)
            {
                var serviceArea = new MicrosoftDynamicsCRMadoxioServicearea
                {
                    ApplicationOdataBind = applicationUri,
                    AdoxioAreacategory = area.AreaCategory,
                    AdoxioArealocation = area.AreaLocation,
                    AdoxioAreanumber = area.AreaNumber,
                    AdoxioCapacity = area.Capacity,
                    AdoxioIsindoor = area.IsIndoor,
                    AdoxioIsoutdoor = area.IsOutdoor,
                    AdoxioIspatio = area.IsPatio,
                    AdoxioDateadded = DateTimeOffset.Now,
                    AdoxioDateupdated = DateTimeOffset.Now
                };
                _dynamicsClient.Serviceareas.Create(serviceArea);
            }
        }
    }
}
