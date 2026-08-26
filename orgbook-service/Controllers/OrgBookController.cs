extern alias DV;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DataverseClient = DV::Gov.Lclb.Cllb.Interfaces.DataverseClient;
using adoxio_licences = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licences;
using adoxio_licences_adoxio_orgbookcredentialresult = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licences_adoxio_orgbookcredentialresult;
using Account = DV::Gov.Lclb.Cllb.Interfaces.Account;
using adoxio_account_adoxio_isorgbooklinkfound = DV::Gov.Lclb.Cllb.Interfaces.adoxio_account_adoxio_isorgbooklinkfound;

namespace Gov.Lclb.Cllb.OrgbookService
{
    public class OrgBookController : Orgbook.OrgbookBase
    {
        readonly IConfiguration Configuration;
        private readonly ILogger _logger;
        private readonly IDataverseClient _dataverse;
        private OrgBookClient _orgbookClient;

        public OrgBookController(IConfiguration configuration, ILoggerFactory loggerFactory, IDataverseClient dataverse)
        {
            Configuration = configuration;
            _dataverse = dataverse;
            _orgbookClient = new OrgBookClient(new HttpClient(), Configuration["ORGBOOK_URL"]);
            _logger = loggerFactory.CreateLogger("OrgbookController");
        }

        public OrgBookController(IConfiguration configuration, ILoggerFactory loggerFactory)
            : this(configuration, loggerFactory, new DataverseClient(configuration))
        {
        }

        public override async Task<MessageResult> IssueLicenceCredential(IssueLicenceCredentialMessage message, ServerCallContext context)
        {
            var (schema, schemaVersion) = OrgBookUtils.GetSchemaFromConfig(message.LicenceType);
            int? orgbookTopicId = await _orgbookClient.GetTopicId(message.RegistrationId);

            if (orgbookTopicId == null)
            {
                await UpdateLicenceOrgBookResult(message.LicenceId, adoxio_licences_adoxio_orgbookcredentialresult.Fail);
                _logger.LogError($"Failed to issue credential - Registration ID: {message.RegistrationId} does not exist.");
                return new MessageResult() { Success = false };
            }

            if (schema == null || schemaVersion == null)
            {
                await UpdateLicenceOrgBookResult(message.LicenceId, adoxio_licences_adoxio_orgbookcredentialresult.Fail);
                _logger.LogError($"Schema {message.LicenceType} not found.");
                return new MessageResult() { Success = false };
            }

            string licenceGuid = Utils.ParseGuid(message.LicenceId);
            var licence = await _dataverse.GetLicenceByIdWithChildrenAsync(licenceGuid);
            var vonAgentClient = new VonAgentClient(new HttpClient(), _logger, schema, schemaVersion, Configuration["AGENT_URL"], Configuration["X_API_KEY"]);
            bool issueSuccess = await vonAgentClient.CreateLicenceCredential(licence, message.RegistrationId);

            await UpdateLicenceOrgBookResult(message.LicenceId, issueSuccess
                ? adoxio_licences_adoxio_orgbookcredentialresult.Pass
                : adoxio_licences_adoxio_orgbookcredentialresult.Fail);

            if (issueSuccess)
                _logger.LogInformation($"Successfully issued credential to {message.RegistrationId}.");
            else
                _logger.LogInformation($"Failed to issue licence credential to {message.RegistrationId}.");

            return new MessageResult() { Success = issueSuccess };
        }

        private async Task UpdateLicenceOrgBookResult(string licenceId, adoxio_licences_adoxio_orgbookcredentialresult result)
        {
            if (!Guid.TryParse(licenceId, out var guid)) return;
            await _dataverse.UpdateLicenceAsync(new adoxio_licences
            {
                Id = guid,
                adoxio_OrgBookCredentialResult = result
            });
        }

        public override async Task<MessageResult> SyncLicencesToOrgbook(GenericRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Starting SyncLicencesToOrgbook");
            IList<adoxio_licences> result;
            try
            {
                result = await _dataverse.GetActiveLicencesMissingOrgBookCredentialAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting Licences");
                return new MessageResult() { Success = false };
            }

            // Pre-fetch licencee accounts and filter to those with an orgbook link
            var accountCache = new Dictionary<Guid, Account?>();
            async Task<Account?> GetCachedAccount(Guid accountId)
            {
                if (!accountCache.TryGetValue(accountId, out var acc))
                {
                    acc = await _dataverse.GetAccountByIdAsync(accountId.ToString());
                    accountCache[accountId] = acc;
                }
                return acc;
            }

            var filteredResult = new List<adoxio_licences>();
            foreach (var item in result)
            {
                if (item.adoxio_Licencee == null) continue;
                var account = await GetCachedAccount(item.adoxio_Licencee.Id);
                if (account?.adoxio_OrgBookOrganizationLink != null)
                    filteredResult.Add(item);
            }

            foreach (var item in filteredResult)
            {
                var account = item.adoxio_Licencee != null
                    ? await GetCachedAccount(item.adoxio_Licencee.Id)
                    : null;
                string registrationId = account?.adoxio_BCIncorporationNumber;
                string licenceId = item.Id.ToString();
                string licenceType = item.adoxio_LicenceType?.Name;

                if (string.IsNullOrEmpty(registrationId))
                {
                    _logger.LogError($"No registration id (incorporation number), Not issuing licence credential to {licenceId}");
                }
                else if (string.IsNullOrEmpty(licenceId))
                {
                    _logger.LogError($"No licenceId, Not issuing licence credential to {licenceId}");
                }
                else if (string.IsNullOrEmpty(licenceType))
                {
                    _logger.LogError($"No licence type, Not issuing licence credential to {licenceId}");
                }
                else
                {
                    await IssueLicenceCredential(new IssueLicenceCredentialMessage()
                    {
                        RegistrationId = registrationId,
                        LicenceId = licenceId,
                        LicenceType = licenceType
                    }, null);
                }
            }

            _logger.LogInformation("End of SyncLicencesToOrgbook");
            return new MessageResult() { Success = true };
        }

        public override async Task<MessageResult> SyncOrgbookToLicences(GenericRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Starting SyncOrgbookToLicences");
            IList<adoxio_licences> result;
            try
            {
                result = await _dataverse.GetActiveLicencesWithOrgBookCredentialPendingSyncAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting Licences");
                return new MessageResult() { Success = false };
            }

            foreach (var item in result)
            {
                string licenceId = item.Id.ToString();
                string licenceNumber = item.adoxio_LicenceNumber;
                string registrationId = null;

                if (item.adoxio_Licencee != null)
                {
                    var account = await _dataverse.GetAccountByIdAsync(item.adoxio_Licencee.Id.ToString());
                    registrationId = account?.adoxio_BCIncorporationNumber;
                }

                int? orgbookTopicId = await _orgbookClient.GetTopicId(registrationId);

                if (orgbookTopicId != null)
                {
                    string licenceTypeName = item.adoxio_LicenceType?.Name;
                    var (schemaName, schemaVersion) = OrgBookUtils.GetSchemaFromConfig(licenceTypeName);
                    var schemaId = await _orgbookClient.GetSchemaId(schemaName, schemaVersion);
                    var credentialId = await _orgbookClient.GetLicenceCredentialId((int)orgbookTopicId, (int)schemaId, licenceNumber);

                    if (credentialId == null)
                    {
                        _logger.LogInformation($"Credential ID for {licenceNumber} not found in the orgbook.");
                        continue;
                    }

                    string credentialLink = _orgbookClient.ORGBOOK_BASE_URL + "/entity/" + registrationId + "/credential/" + credentialId.ToString();

                    if (Guid.TryParse(licenceId, out var licGuid))
                    {
                        await _dataverse.UpdateLicenceAsync(new adoxio_licences
                        {
                            Id = licGuid,
                            adoxio_OrgBookCredentialID = credentialId.ToString(),
                            adoxio_OrgBookCredentialLink = credentialLink
                        });
                    }
                    _logger.LogInformation($"Successfully updated licence - credential ID: {credentialId} to {registrationId}.");
                }
                else
                {
                    _logger.LogError($"Failed to update licence with new credential ID for Registration ID: {registrationId}.");
                }
            }

            _logger.LogInformation("End of SyncOrgbookToLicences");
            return new MessageResult() { Success = true };
        }

        public override async Task<MessageResult> SyncOrgbookToAccounts(GenericRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Starting SyncOrgbookToAccounts.");
            IList<Account> result;
            try
            {
                result = await _dataverse.GetAccountsMissingOrgBookLinkAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting accounts");
                return new MessageResult() { Success = false };
            }

            _logger.LogInformation($"Found {result.Count} organizations to query orgbook for.");

            foreach (var item in result)
            {
                string registrationId = item.adoxio_BCIncorporationNumber;
                Guid accountGuid = item.Id;
                int? orgbookTopicId = await _orgbookClient.GetTopicId(registrationId);

                if (orgbookTopicId != null)
                {
                    string orgbookLink = _orgbookClient.ORGBOOK_BASE_URL + "/entity/" + registrationId;
                    await _dataverse.UpdateAccountAsync(new Account
                    {
                        Id = accountGuid,
                        adoxio_OrgBookOrganizationLink = orgbookLink,
                        adoxio_IsOrgbookLinkFound = adoxio_account_adoxio_isorgbooklinkfound.Yes
                    });
                    _logger.LogInformation($"Successfully added orgbook link to account with registration id {registrationId}.");
                }
                else
                {
                    await _dataverse.UpdateAccountAsync(new Account
                    {
                        Id = accountGuid,
                        adoxio_IsOrgbookLinkFound = adoxio_account_adoxio_isorgbooklinkfound.No
                    });
                    _logger.LogError($"Failed to add orgbook link to account with registration id {registrationId}.");
                }
            }

            _logger.LogInformation($"Ending SyncOrgbookToAccounts");
            return new MessageResult() { Success = true };
        }

        public override async Task<MessageResult> CompanyExistsInOrgbook(CompanyNameRequest request, ServerCallContext context)
        {
            var result = await _orgbookClient.SearchCompanyName(request.CompanyName);
            return new MessageResult() { Success = result != null };
        }

        public override async Task<CompaniesNameResult> CompaniesExistInOrgbook(CompaniesNameRequest request, ServerCallContext context)
        {
            List<bool> results = new List<bool>();
            foreach (string name in request.CompanyNames)
            {
                CompanyNameRequest req = new CompanyNameRequest() { CompanyName = name };
                MessageResult exists = await this.CompanyExistsInOrgbook(req, context);
                results.Add(exists.Success);
            }
            CompaniesNameResult result = new CompaniesNameResult();
            result.CompanyNames.AddRange(request.CompanyNames);
            result.Results.AddRange(results);
            return result;
        }
    }
}
