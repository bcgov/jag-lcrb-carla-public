extern alias DV;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Google.Protobuf;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using Microsoft.Extensions.Caching.Memory;
using DvLegalEntity = DV::Gov.Lclb.Cllb.Interfaces.adoxio_legalentity;
using DvChangelog = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licenseechangelog;
using DvAccount = DV::Gov.Lclb.Cllb.Interfaces.Account;
using DvAccountType = DV::Gov.Lclb.Cllb.Interfaces.adoxio_accounttype;
using DvApplicantType = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicanttypecodes;
using DvTiedHouse = DV::Gov.Lclb.Cllb.Interfaces.adoxio_tiedhouseconnection;
using DvPhsComplete = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_phscomplete;
using DvCasComplete = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_cascomplete;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LegalEntitiesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDataverseClient _dataverse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly string _encryptionKey;
        private readonly FileManagerClient _fileManagerClient;

        public LegalEntitiesController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDataverseClient dataverse, FileManagerClient fileClient, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _dataverse = dataverse;
            _httpContextAccessor = httpContextAccessor;
            _encryptionKey = _configuration["ENCRYPTION_KEY"];
            _logger = loggerFactory.CreateLogger(typeof(LegalEntitiesController));
            _fileManagerClient = fileClient;
        }

        /// <summary>
        /// Get all Dynamics Legal Entities
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetDynamicsLegalEntities()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            userSettings.Validate();
            var legalEntities = await _dataverse.GetLegalEntitiesByAccountIdAsync(userSettings.AccountId);
            return new JsonResult(legalEntities.Select(le => le.ToViewModel()).ToList());
        }

        /// <summary>
        /// Get all Dynamics Legal Entities for the current Business Profile Summary
        /// </summary>
        [HttpGet("business-profile-summary")]
        public async Task<JsonResult> GetBusinessProfileSummary()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            userSettings.Validate();
            var legalEntities = await GetAccountLegalEntitiesAsync(userSettings.AccountId);
            return new JsonResult(legalEntities.Select(le => le.ToViewModel()).ToList());
        }

        [HttpGet("current-hierarchy")]
        public async Task<JsonResult> GetCurrentHierarchy()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            userSettings.Validate();
            LegalEntity legalEntity = await GetLegalEntityTreeAsync(userSettings.AccountId);
            return new JsonResult(legalEntity);
        }

        private async Task GetScreeningData(SecurityScreeningCategorySummary summary, LegalEntity legalEntity, bool isLiquor, List<string> addedContacts = null)
        {
            if (addedContacts == null)
                addedContacts = new List<string>();

            if (legalEntity.isindividual == true && !string.IsNullOrEmpty(legalEntity.contactId))
            {
                bool isComplete = false;
                DateTimeOffset? dateSubmitted = null;

                var contact = await _dataverse.GetContactByIdAsync(legalEntity.contactId);
                if (isLiquor && contact?.adoxio_PHSComplete == DvPhsComplete.Yes)
                {
                    isComplete = true;
                    dateSubmitted = contact.adoxio_PHSDateSubmitted.HasValue
                        ? (DateTimeOffset?)contact.adoxio_PHSDateSubmitted.Value
                        : null;
                }
                if (!isLiquor && contact?.adoxio_cascomplete == DvCasComplete.Yes)
                {
                    isComplete = true;
                    dateSubmitted = contact.adoxio_casdatesubmitted.HasValue
                        ? (DateTimeOffset?)contact.adoxio_casdatesubmitted.Value
                        : null;
                }

                var newItem = new SecurityScreeningStatusItem
                {
                    FirstName = legalEntity.firstname,
                    MiddleName = legalEntity.middlename,
                    LastName = legalEntity.lastname,
                    PhsLink = legalEntity.PhsLink,
                    CasLink = legalEntity.CasLink,
                    DateSubmitted = dateSubmitted,
                    ContactId = legalEntity.contactId
                };

                if (isComplete)
                {
                    if (summary.CompletedItems == null)
                        summary.CompletedItems = new List<SecurityScreeningStatusItem>();
                    if (newItem.ContactId != null && !addedContacts.Any(c => c == newItem.ContactId))
                        addedContacts.Add(newItem.ContactId);
                    summary.CompletedItems.Add(newItem);
                }
                else
                {
                    if (summary.OutstandingItems == null)
                        summary.OutstandingItems = new List<SecurityScreeningStatusItem>();
                    if (newItem.ContactId != null && !addedContacts.Any(c => c == newItem.ContactId))
                        addedContacts.Add(newItem.ContactId);
                    summary.OutstandingItems.Add(newItem);
                }
            }

            if (legalEntity.children != null)
            {
                foreach (var item in legalEntity.children)
                    await GetScreeningData(summary, item, isLiquor, addedContacts);
            }
        }

        [HttpGet("current-security-summary")]
        public async Task<JsonResult> GetCurrentSecurityScreeningSummary()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            userSettings.Validate();
            string currentAccountId = userSettings.AccountId;

            LegalEntity legalEntity = await GetLegalEntityTreeAsync(currentAccountId);
            var licences = await _dataverse.GetLicencesByAccountIdAsync(currentAccountId);
            var applications = await _dataverse.GetApplicationsByAccountIdAsync(currentAccountId);

            SecurityScreeningSummary result = new SecurityScreeningSummary();

            int cannabisLicenceCount = licences.Count(x => x.adoxio_LicenceType?.Name?.ToUpper().Contains("CANNABIS") == true);
            int liquorLicenceCount = licences.Count() - cannabisLicenceCount;
            int cannabisApplicationCount = applications.Count(x => x.adoxio_ApplicationTypeId?.Name?.ToUpper().Contains("CANNABIS") == true);
            int liquorApplicationCount = applications.Count() - cannabisApplicationCount;

            if (cannabisLicenceCount > 0 || cannabisApplicationCount > 0)
            {
                var cannabisSummary = new SecurityScreeningCategorySummary();
                await GetScreeningData(cannabisSummary, legalEntity, false);
                result.Cannabis = cannabisSummary;
            }

            if (liquorLicenceCount > 0 || liquorApplicationCount > 0)
            {
                var liquorSummary = new SecurityScreeningCategorySummary();
                await GetScreeningData(liquorSummary, legalEntity, true);
                result.Liquor = liquorSummary;
            }

            return new JsonResult(result);
        }

        [HttpGet("legal-entity-change-logs/application/{applicationId}")]
        public async Task<List<LicenseeChangeLog>> GetChangeLogsForApplication(string applicationId)
        {
            var changelogs = await _dataverse.GetLicenseeChangelogsByApplicationIdAsync(applicationId);
            return changelogs.Select(c => c.ToViewModel()).ToList();
        }

        [HttpGet("legal-entity-change-logs/account/{accountId}")]
        public async Task<ActionResult> GetChangeLogsForAccount(string accountId)
        {
            var changelogs = await _dataverse.GetLicenseeChangelogsByAccountIdAsync(accountId);
            return new JsonResult(changelogs.Select(c => c.ToViewModel()).ToList());
        }

        private async Task<LegalEntity> GetLegalEntityTreeAsync(string accountId)
        {
            var allEntities = await _dataverse.GetLegalEntitiesByAccountIdAsync(accountId);
            var root = allEntities.FirstOrDefault(e => e.adoxio_LegalEntityOwned == null);
            if (root == null) return null;

            var result = root.ToViewModel();
            if (!string.IsNullOrEmpty(result.contactId))
            {
                result.PhsLink = ContactController.GetPhsLink(result.contactId, _configuration, _encryptionKey);
                result.CasLink = ContactController.GetCASSLink(result.contactId, _configuration, _encryptionKey);
            }

            var processedEntities = new List<string>();
            result.children = await GetLegalEntityChildrenAsync(result.id, processedEntities);
            return result;
        }

        private async Task<List<LegalEntity>> GetLegalEntityChildrenAsync(string parentLegalEntityId, List<string> processedEntities)
        {
            var result = new List<LegalEntity>();
            if (processedEntities == null)
                processedEntities = new List<string>();

            IList<DvLegalEntity> children;
            try
            {
                children = await _dataverse.GetLegalEntitiesByParentEntityIdAsync(parentLegalEntityId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Exception while getting child legal entities");
                return result;
            }

            foreach (var child in children)
            {
                var viewModel = child.ToViewModel();
                if (!string.IsNullOrEmpty(viewModel.id) && !processedEntities.Contains(viewModel.id))
                {
                    processedEntities.Add(viewModel.id);
                    viewModel.children = await GetLegalEntityChildrenAsync(viewModel.id, processedEntities);
                }
                if (!string.IsNullOrEmpty(viewModel.contactId))
                {
                    viewModel.PhsLink = ContactController.GetPhsLink(viewModel.contactId, _configuration, _encryptionKey);
                    viewModel.CasLink = ContactController.GetCASSLink(viewModel.contactId, _configuration, _encryptionKey);
                }
                result.Add(viewModel);
            }
            return result;
        }

        private async Task<List<DvLegalEntity>> GetAccountLegalEntitiesAsync(string accountId, List<string> shareHolders = null)
        {
            if (shareHolders == null)
                shareHolders = new List<string>();

            IList<DvLegalEntity> legalEntities;
            try
            {
                var all = await _dataverse.GetLegalEntitiesByAccountIdAsync(accountId);
                legalEntities = all.Where(le => le.adoxio_IsIndividual != DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.Yes).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Exception while getting account legal entities.");
                return new List<DvLegalEntity>();
            }

            var children = new List<DvLegalEntity>();
            foreach (var le in legalEntities)
            {
                if (le.adoxio_ShareholderAccountID != null)
                {
                    var shareholderId = le.adoxio_ShareholderAccountID.Id.ToString();
                    if (!shareHolders.Contains(shareholderId))
                    {
                        shareHolders.Add(shareholderId);
                        children.AddRange(await GetAccountLegalEntitiesAsync(shareholderId, shareHolders));
                    }
                }
            }
            legalEntities = legalEntities.Concat(children).Distinct().ToList();
            return legalEntities.ToList();
        }

        /// <summary>
        /// Get all Legal Entities where the position matches the parameter received
        /// </summary>
        [HttpGet]
        [Route("position/{parentLegalEntityId}/{positionType}")]
        public async Task<IActionResult> GetDynamicsLegalEntitiesByPosition(string parentLegalEntityId, string positionType)
        {
            if (!Guid.TryParse(parentLegalEntityId, out _))
                return NotFound();

            IList<DvLegalEntity> legalEntities;
            try
            {
                legalEntities = await _dataverse.GetLegalEntitiesByParentEntityIdAsync(parentLegalEntityId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Exception while getting legal entities by position.");
                return StatusCode(500);
            }

            legalEntities = positionType switch
            {
                "shareholders" or "partners" => legalEntities.Where(le => le.adoxio_IsPartner == true || le.adoxio_IsShareholder == true).ToList(),
                "key-personnel" => legalEntities.Where(le => le.adoxio_IsKeyPersonnel == true).ToList(),
                "directors-officers-management" => legalEntities.Where(le => le.adoxio_IsDirector == true || le.adoxio_IsSeniorManagement == true || le.adoxio_IsOfficer == true).ToList(),
                "director-officer-shareholder" => legalEntities.Where(le => le.adoxio_IsIndividual == DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.Yes).ToList(),
                _ => new List<DvLegalEntity>()
            };

            var result = new List<LegalEntity>();
            foreach (var le in legalEntities)
            {
                if (le.adoxio_Account == null)
                    continue;
                if (!await DynamicsExtensions.CurrentUserHasAccessToAccountAsync(le.adoxio_Account.Id, _httpContextAccessor, _dataverse))
                    return NotFound();
                result.Add(le.ToViewModel());
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Get the special applicant legal entity for the current user
        /// </summary>
        [HttpGet("applicant")]
        public async Task<IActionResult> GetApplicantDynamicsLegalEntity()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            userSettings.Validate();

            _logger.LogDebug("Find legal entity for applicant = " + userSettings.AccountId);
            var legalEntity = await _dataverse.GetLegalEntityByAccountIdAsync(userSettings.AccountId);
            if (legalEntity == null)
                return new NotFoundResult();

            var result = legalEntity.ToViewModel();
            if (result.account == null)
            {
                var account = await _dataverse.GetAccountByIdAsync(userSettings.AccountId);
                result.account = account?.ToViewModel();
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Get a specific legal entity
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDynamicsLegalEntity(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new NotFoundResult();

            var le = await _dataverse.GetLegalEntityByIdAsync(id);
            if (le == null || le.adoxio_Account == null)
                return new NotFoundResult();

            if (!await DynamicsExtensions.CurrentUserHasAccessToAccountAsync(le.adoxio_Account.Id, _httpContextAccessor, _dataverse))
                return new NotFoundResult();

            return new JsonResult(le.ToViewModel());
        }

        /// <summary>
        /// Create a legal entity
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateDynamicsLegalEntity([FromBody] LegalEntity item)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            userSettings.Validate();

            var entity = new DvLegalEntity();
            entity.CopyValues(item);
            entity.adoxio_Account = new EntityReference("account", Guid.Parse(userSettings.AccountId));

            var parentEntity = await _dataverse.GetLegalEntityByAccountIdAsync(userSettings.AccountId);
            if (parentEntity != null)
                entity.adoxio_LegalEntityOwned = new EntityReference("adoxio_legalentity", parentEntity.Id);

            Guid newId;
            try
            {
                newId = await _dataverse.CreateLegalEntityAsync(entity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Exception while creating legal entity");
                throw new Exception("Unable to create legal entity");
            }

            var created = await _dataverse.GetLegalEntityByIdAsync(newId.ToString());
            return new JsonResult(created?.ToViewModel());
        }

        [HttpPost("save-change-tree/{applicationId}")]
        public async Task<IActionResult> SaveLicenseeChangeTree(string applicationId, LicenseeChangeLog treeRoot)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await SaveChangeObjectsAsync(treeRoot, applicationId);
            return Ok();
        }

        [HttpPost("save-change-tree/account/{accountId}")]
        public async Task<IActionResult> SaveAccountLicenseeChangeTree(string accountId, LicenseeChangeLog treeRoot)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await SaveAccountChangeObjectsAsync(treeRoot, accountId);
            return Ok();
        }

        [HttpPost("cancel-change-logs")]
        public async Task<IActionResult> CancelLicenseeChangeLogs(List<LicenseeChangeLog> changeLogs)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            foreach (var change in changeLogs)
            {
                if (!string.IsNullOrEmpty(change.Id))
                {
                    try
                    {
                        await _dataverse.DeleteLicenseeChangelogAsync(change.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected Exception while deleting LicenseeChangeLog");
                    }
                }
            }
            return Ok();
        }

        private async Task SaveChangeObjectsAsync(LicenseeChangeLog node, string applicationId, string parentLegalEntityId = null, string parentChangeLogId = null)
        {
            if (node.ChangeType != LicenseeChangeType.unchanged)
            {
                var entity = new DvChangelog();
                entity.CopyValues(node);

                if (parentLegalEntityId != null)
                    node.ParentLegalEntityId = parentLegalEntityId;
                if (parentChangeLogId != null)
                    node.ParentLicenseeChangeLogId = parentChangeLogId;

                if (string.IsNullOrEmpty(node.Id)) // create
                {
                    if (!string.IsNullOrEmpty(node.ParentLegalEntityId))
                        entity.adoxio_ParentLegalEntityId = new EntityReference("adoxio_legalentity", Guid.Parse(node.ParentLegalEntityId));
                    if (!string.IsNullOrEmpty(node.LegalEntityId))
                        entity.adoxio_LegalEntityId = new EntityReference("adoxio_legalentity", Guid.Parse(node.LegalEntityId));
                    if (!string.IsNullOrEmpty(node.ParentLicenseeChangeLogId))
                        entity.adoxio_ParentLinceseeChangeLogId = new EntityReference("adoxio_licenseechangelog", Guid.Parse(node.ParentLicenseeChangeLogId));
                    if (!string.IsNullOrEmpty(applicationId))
                        entity.adoxio_Application = new EntityReference("adoxio_application", Guid.Parse(applicationId));
                    if (!string.IsNullOrEmpty(node.ParentBusinessAccountId))
                        entity.adoxio_ParentBusinessAccount = new EntityReference("account", Guid.Parse(node.ParentBusinessAccountId));
                    if (!string.IsNullOrEmpty(node.BusinessAccountId))
                        entity.adoxio_BusinessAccount = new EntityReference("account", Guid.Parse(node.BusinessAccountId));
                    try
                    {
                        var newId = await _dataverse.CreateLicenseeChangelogAsync(entity);
                        parentChangeLogId = newId.ToString();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected Exception while saving LicenseeChangeLog");
                    }
                }
                else if (!string.IsNullOrEmpty(node.Id) && string.IsNullOrEmpty(node.LegalEntityId) && (
                    node.ChangeType == LicenseeChangeType.removeBusinessShareholder ||
                    node.ChangeType == LicenseeChangeType.removeIndividualShareholder ||
                    node.ChangeType == LicenseeChangeType.removeLeadership))
                {
                    try
                    {
                        await _dataverse.DeleteLicenseeChangelogAsync(node.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected Exception while deleting LicenseeChangeLog");
                    }
                }
                else // update
                {
                    entity.Id = Guid.Parse(node.Id);
                    if (!string.IsNullOrEmpty(node.ParentBusinessAccountId))
                        entity.adoxio_ParentBusinessAccount = new EntityReference("account", Guid.Parse(node.ParentBusinessAccountId));
                    if (!string.IsNullOrEmpty(node.BusinessAccountId))
                        entity.adoxio_BusinessAccount = new EntityReference("account", Guid.Parse(node.BusinessAccountId));
                    try
                    {
                        await _dataverse.UpdateLicenseeChangelogAsync(entity);
                        parentChangeLogId = node.Id;
                        parentLegalEntityId = node.LegalEntityId;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected Exception while saving LicenseeChangeLog");
                    }
                }
            }
            else
            {
                parentChangeLogId = node.Id;
            }

            if (node.Children != null)
            {
                foreach (var item in node.Children)
                    await SaveChangeObjectsAsync(item, applicationId, node.LegalEntityId, parentChangeLogId);
            }
        }

        private async Task SaveAccountChangeObjectsAsync(LicenseeChangeLog node, string accountId, string parentLegalEntityId = null, string parentChangeLogId = null)
        {
            if (node.ChangeType != LicenseeChangeType.unchanged)
            {
                var entity = new DvChangelog();
                entity.CopyValues(node);
                node.ParentBusinessAccountId = accountId;

                if (parentLegalEntityId != null)
                    node.ParentLegalEntityId = parentLegalEntityId;
                if (parentChangeLogId != null)
                    node.ParentLicenseeChangeLogId = parentChangeLogId;

                if (string.IsNullOrEmpty(node.Id)) // create
                {
                    if (!string.IsNullOrEmpty(node.ParentLegalEntityId))
                        entity.adoxio_ParentLegalEntityId = new EntityReference("adoxio_legalentity", Guid.Parse(node.ParentLegalEntityId));
                    if (!string.IsNullOrEmpty(node.LegalEntityId))
                        entity.adoxio_LegalEntityId = new EntityReference("adoxio_legalentity", Guid.Parse(node.LegalEntityId));
                    if (!string.IsNullOrEmpty(node.ParentLicenseeChangeLogId))
                        entity.adoxio_ParentLinceseeChangeLogId = new EntityReference("adoxio_licenseechangelog", Guid.Parse(node.ParentLicenseeChangeLogId));
                    if (!string.IsNullOrEmpty(node.BusinessAccountId))
                    {
                        entity.adoxio_BusinessAccount = new EntityReference("account", Guid.Parse(node.BusinessAccountId));
                        parentLegalEntityId = node.LegalEntityId;
                    }
                    if (!string.IsNullOrEmpty(node.ParentBusinessAccountId))
                        entity.adoxio_ParentBusinessAccount = new EntityReference("account", Guid.Parse(node.ParentBusinessAccountId));
                    try
                    {
                        var newId = await _dataverse.CreateLicenseeChangelogAsync(entity);
                        parentChangeLogId = newId.ToString();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected Exception while saving LicenseeChangeLog for Account");
                    }
                }
                else // update
                {
                    entity.Id = Guid.Parse(node.Id);
                    if (!string.IsNullOrEmpty(node.ParentBusinessAccountId))
                        entity.adoxio_ParentBusinessAccount = new EntityReference("account", Guid.Parse(node.ParentBusinessAccountId));
                    if (!string.IsNullOrEmpty(node.BusinessAccountId))
                        entity.adoxio_BusinessAccount = new EntityReference("account", Guid.Parse(node.BusinessAccountId));
                    try
                    {
                        await _dataverse.UpdateLicenseeChangelogAsync(entity);
                        parentChangeLogId = node.Id;
                        parentLegalEntityId = node.LegalEntityId;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected Exception while saving LicenseeChangeLog for Account");
                    }
                }
            }
            else
            {
                parentChangeLogId = node.Id;
            }

            if (node.Children != null)
            {
                foreach (var item in node.Children)
                    await SaveAccountChangeObjectsAsync(item, node.ParentBusinessAccountId, node.LegalEntityId, parentChangeLogId);
            }
        }

        /// <summary>
        /// Create a child (shareholder) legal entity
        /// </summary>
        [HttpPost]
        [Route("child-legal-entity")]
        public async Task<IActionResult> CreateDynamicsShareholderLegalEntity([FromBody] LegalEntity item)
        {
            if (item == null)
                return BadRequest();

            var entity = new DvLegalEntity();
            entity.CopyValues(item);

            if (item.isindividual != true)
            {
                var account = new DvAccount { Name = item.name };

                if (item.isShareholder == true)
                    account.adoxio_AccountType = (DvAccountType)(int)AdoxioAccountTypeCodes.Shareholder;
                else if (item.isPartner == true)
                    account.adoxio_AccountType = (DvAccountType)(int)AdoxioAccountTypeCodes.Partner;

                if (item.legalentitytype != null)
                    account.adoxio_BusinessType = (DvApplicantType)(int)item.legalentitytype.Value;

                Guid accountId;
                try
                {
                    accountId = await _dataverse.CreateAccountAsync(account);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected Exception while creating account");
                    return StatusCode(500);
                }

                var tiedHouse = new DvTiedHouse
                {
                    adoxio_AccountId = new EntityReference("account", accountId)
                };
                try
                {
                    await _dataverse.CreateTiedHouseConnectionAsync(tiedHouse);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected Exception while creating tied house connection");
                }

                entity.adoxio_ShareholderAccountID = new EntityReference("account", accountId);
            }

            if (item.account?.id != null)
                entity.adoxio_Account = new EntityReference("account", Guid.Parse(item.account.id));
            if (item.parentLegalEntityId != null)
                entity.adoxio_LegalEntityOwned = new EntityReference("adoxio_legalentity", Guid.Parse(item.parentLegalEntityId));

            DvLegalEntity created;
            try
            {
                var newId = await _dataverse.CreateLegalEntityAsync(entity);
                created = await _dataverse.GetLegalEntityByIdAsync(newId.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Exception while creating legal entity");
                return StatusCode(500);
            }

            return new JsonResult(created?.ToViewModel());
        }

        /// <summary>
        /// Update a legal entity
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDynamicsLegalEntity([FromBody] LegalEntity item, string id)
        {
            if (id != item.id)
                return BadRequest();

            var existing = await _dataverse.GetLegalEntityByIdAsync(id);
            if (existing == null)
                return new NotFoundResult();

            var patch = new DvLegalEntity { Id = existing.Id };
            patch.CopyValues(item);

            try
            {
                await _dataverse.UpdateLegalEntityAsync(patch);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Exception while updating legal entity");
            }

            var updated = await _dataverse.GetLegalEntityByIdAsync(id);
            return new JsonResult(updated?.ToViewModel());
        }

        /// <summary>
        /// Delete a legal entity.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteDynamicsLegalEntity(string id)
        {
            var existing = await _dataverse.GetLegalEntityByIdAsync(id);
            if (existing == null)
                return new NotFoundResult();

            try
            {
                await _dataverse.DeleteLegalEntityAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Exception while deleting legal entity");
            }

            return NoContent();
        }

        private string GetConsentLink(string email, string individualId, string parentId)
        {
            string result = _configuration["BASE_URI"] + _configuration["BASE_PATH"];
            result += "/bcservice?path=/security-consent/" + parentId + "/" + individualId + "?code=";

            var confirmation = new SecurityConsentConfirmation
            {
                email = email,
                parentid = parentId,
                individualid = individualId
            };
            string json = JsonConvert.SerializeObject(confirmation);
            result += System.Net.WebUtility.UrlEncode(EncryptionUtility.EncryptString(json, _encryptionKey));
            return result;
        }

        [HttpGet("{id}/verifyconsentcode/{individualid}")]
        public JsonResult VerifyConsentCode(string id, string individualid, string code)
        {
            string result = "Error";
            string decrypted = EncryptionUtility.DecryptString(code, _encryptionKey);
            if (decrypted != null)
            {
                var consentConfirmation = JsonConvert.DeserializeObject<SecurityConsentConfirmation>(decrypted);
                if (id.Equals(consentConfirmation.parentid) && individualid.Equals(consentConfirmation.individualid))
                    result = "Success";
            }
            return new JsonResult(result);
        }

        /// <summary>
        /// Send consent requests to the supplied list of legal entities.
        /// </summary>
        [HttpPost("{id}/sendconsentrequests")]
        public async Task<IActionResult> SendConsentRequests(string id, [FromBody] List<string> recipientIds)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            userSettings.Validate();

            var adoxioLegalEntity = await _dataverse.GetLegalEntityByIdAsync(id);

            foreach (string recipientId in recipientIds)
            {
                var recipientEntity = await _dataverse.GetLegalEntityByIdAsync(recipientId);
                string email = recipientEntity?.adoxio_Email;
                string firstname = recipientEntity?.adoxio_FirstName;
                string lastname = recipientEntity?.adoxio_LastName;

                string confirmationEmailLink = GetConsentLink(email, recipientId, id);
                string bclogo = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/assets/bc-logo.svg";

                string body =
                        "<img src='" + bclogo + "'/><br><h2>Security Screening and Financial Integrity Checks</h2>"
                    + "<p>Dear " + firstname + " " + lastname + ",</p>"
                    + "<p>An application from [TBD Company Name] has been submitted for a non-medical retail cannabis licence in British Columbia. "
                    + "As a [TBD Position] of [TBD Company Name] you are required to authorize a security screening — "
                    + "including criminal and police record checks — and financial integrity checks as part of the application process.</p>"
                    + "<p>Where you reside will determine how you are able to authorize the security screening.</p>"
                    + "<p><strong>B.C. Residents</strong></p>"
                    + "<p>Residents of B.C. require a Photo B.C. Services Card to login to the application.</p>"
                    + "<p>After you receive your verified Photo B.C. Services Card, login through this unique link:</p>"
                    + "<p><a href='" + confirmationEmailLink + "'>" + confirmationEmailLink + "</a></p>"
                    + "<p><strong>Out of Province Residents</strong></p><p>TBD</p>"
                    + "<p><strong>Residents Outside of Canada</strong></p><p>TBD</p>"
                    + "<p>If you have any questions about the security authorization, contact helpdesk@lclbc.ca</p>"
                    + "<p>Do not reply to this email address</p>";

                SmtpClient client = new SmtpClient(_configuration["SMTP_HOST"]);
                MailMessage message = new MailMessage("no-reply@gov.bc.ca", email);
                message.Subject = "BC LCLB Cannabis Licensing Security Consent";
                message.Body = body;
                message.IsBodyHtml = true;
                client.Send(message);

                if (adoxioLegalEntity != null)
                {
                    var patch = new DvLegalEntity
                    {
                        Id = adoxioLegalEntity.Id,
                        adoxio_DateEmailSent = DateTime.Now
                    };
                    try
                    {
                        await _dataverse.UpdateLegalEntityAsync(patch);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected Exception while updating date email sent.");
                    }
                }
            }

            return NoContent();
        }
    }
}
