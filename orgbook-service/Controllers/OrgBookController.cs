using System.Net.Http;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Linq;

namespace Gov.Lclb.Cllb.OrgbookService
{
    public class OrgBookController : Orgbook.OrgbookBase
    {
        readonly IConfiguration Configuration;
        private readonly ILogger _logger;
        private IDynamicsClient _dynamics;
        private OrgBookClient _orgbookClient;

        public OrgBookController(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            if (Configuration["DYNAMICS_ODATA_URI"] != null)
            {
                _dynamics = DynamicsSetupUtil.SetupDynamics(Configuration);
            }
            _orgbookClient = new OrgBookClient(new HttpClient(), Configuration["ORGBOOK_URL"]);
            _logger = loggerFactory.CreateLogger("OrgbookController");
        }

        public override async Task<MessageResult> IssueLicenceCredential(IssueLicenceCredentialMessage message, ServerCallContext context)
        {
            var (schema, schemaVersion) = OrgBookUtils.GetSchemaFromConfig(message.LicenceType);
            int? orgbookTopicId = await _orgbookClient.GetTopicId(message.RegistrationId);
            if (orgbookTopicId == null)
            {
                _dynamics.Licenceses.Update(message.LicenceId, new MicrosoftDynamicsCRMadoxioLicences() { AdoxioOrgbookcredentialresult = (int)OrgBookCredentialStatus.Fail });
                _logger.LogError($"Failed to issue credential - Registration ID: {message.RegistrationId} does not exist.");
                return  new MessageResult() {
                    Success = false
                };
            }
            else if(schema == null || schemaVersion == null)
            {
                _dynamics.Licenceses.Update(message.LicenceId, new MicrosoftDynamicsCRMadoxioLicences() { AdoxioOrgbookcredentialresult = (int)OrgBookCredentialStatus.Fail });
                _logger.LogError($"Schema {message.LicenceType} not found.");
                return  new MessageResult() {
                    Success = false
                };
            }
            else
            {
                string licenceGuid = Utils.ParseGuid(message.LicenceId);
                var licence = _dynamics.GetLicenceByIdWithChildren(licenceGuid);
                VonAgentClient _vonAgentClient = new VonAgentClient(new HttpClient(), _logger, schema, schemaVersion, Configuration["AGENT_URL"], Configuration["X_API_KEY"]);
                bool issueSuccess = await _vonAgentClient.CreateLicenceCredential(licence, message.RegistrationId);

                if(issueSuccess)
                {
                    _dynamics.Licenceses.Update(message.LicenceId, new MicrosoftDynamicsCRMadoxioLicences()
                    {
                        AdoxioOrgbookcredentialresult = (int)OrgBookCredentialStatus.Pass
                    });
                    _logger.LogInformation($"Successfully issued credential to {message.RegistrationId}.");
                    return  new MessageResult() {
                        Success = true
                    };
                }
                else
                {
                    _dynamics.Licenceses.Update(message.LicenceId, new MicrosoftDynamicsCRMadoxioLicences()
                    {
                        AdoxioOrgbookcredentialresult = (int)OrgBookCredentialStatus.Fail
                    });
                    _logger.LogInformation($"Failed to issue licence credential to {message.RegistrationId}.");
                    return  new MessageResult() {
                        Success = false
                    };
                }
            }
        }

        public override async Task<MessageResult> SyncLicencesToOrgbook(GenericRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Starting SyncLicencesToOrgbook");
            IList<MicrosoftDynamicsCRMadoxioLicences> result;
            try
            {
                // Get active licences missing orgbook credential
                var expand = new List<string> { "adoxio_Licencee", "adoxio_LicenceType" };
                string filter = $"adoxio_orgbookcredentialresult eq null and statuscode eq 1";
                result = _dynamics.Licenceses.Get(filter: filter, expand: expand).Value;
                result = result.Where(l => l.AdoxioLicencee?.AdoxioOrgbookorganizationlink != null).ToList();
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError("Error getting Licences");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);

                // fail if we can't get results.
                return new MessageResult() {
                    Success = false
                };
            }

            // now for each one process it.
            foreach (var item in result)
            {
                string registrationId = item.AdoxioLicencee?.AdoxioBcincorporationnumber;
                string licenceId = item.AdoxioLicencesid;
                string licenceType = item.AdoxioLicenceType?.AdoxioName;
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
                    await IssueLicenceCredential(new IssueLicenceCredentialMessage() {
                        RegistrationId = registrationId,
                        LicenceId = licenceId,
                        LicenceType = licenceType
                    }, null);
                }
            }

            _logger.LogInformation("End of SyncLicencesToOrgbook");
            return new MessageResult() {
                Success = true
            };
        }

        public override async Task<MessageResult> SyncOrgbookToLicences(GenericRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Starting SyncOrgbookToLicences");
            IList<MicrosoftDynamicsCRMadoxioLicences> result;
            try
            {
                var expand = new List<string> { "adoxio_Licencee", "adoxio_LicenceType" };
                string filter = $"adoxio_orgbookcredentialresult eq {(int)OrgBookCredentialStatus.Pass} and adoxio_orgbookcredentialid eq null and statuscode eq 1";
                result = _dynamics.Licenceses.Get(filter: filter, expand: expand).Value;
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError("Error getting Licences");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);

                // fail if we can't get results.
                return new MessageResult() {
                    Success = false
                };
            }

            // now for each one process it.
            foreach (var item in result)
            {
                string registrationId = item.AdoxioLicencee?.AdoxioBcincorporationnumber;
                string licenceId = item.AdoxioLicencesid;
                string licenceNumber = item.AdoxioLicencenumber;
                int? orgbookTopicId = await _orgbookClient.GetTopicId(registrationId);

                if (orgbookTopicId != null)
                {
                    var (schemaName, schemaVersion) = OrgBookUtils.GetSchemaFromConfig(item.AdoxioLicenceType.AdoxioName);
                    
                    var schemaId = await _orgbookClient.GetSchemaId(schemaName, schemaVersion);
                    var credentialId = await _orgbookClient.GetLicenceCredentialId((int)orgbookTopicId, (int)schemaId, licenceNumber);
                    if (credentialId == null)
                    {
                        _logger.LogInformation($"Credential ID for {licenceNumber} not found in the orgbook.");
                        continue;
                    }
                    string credentialLink = _orgbookClient.ORGBOOK_BASE_URL + "/en/organization/" + registrationId + "/cred/" + credentialId.ToString();

                    _dynamics.Licenceses.Update(licenceId, new MicrosoftDynamicsCRMadoxioLicences()
                    {
                        AdoxioOrgbookcredentialid = credentialId.ToString(),
                        AdoxioOrgbookcredentiallink = credentialLink
                    });
                    _logger.LogInformation($"Successfully updated licence - credential ID: {credentialId} to {registrationId}.");
                }
                else
                {
                    _logger.LogError($"Failed to update licence with new credential ID for Registration ID: {registrationId}.");
                }
            }

            _logger.LogInformation("End of SyncOrgbookToLicences");
            return new MessageResult() {
                Success = true
            };
        }

        public override async Task<MessageResult> SyncOrgbookToAccounts(GenericRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Starting SyncOrgbookToAccounts.");
            IList<MicrosoftDynamicsCRMaccount> result;
            try
            {
                var select = new List<string> {"adoxio_bcincorporationnumber", "accountid"};
                string filter = $"adoxio_orgbookorganizationlink eq null and adoxio_businessregistrationnumber eq null and adoxio_bcincorporationnumber ne null and adoxio_bcincorporationnumber ne 'BC1234567'";
                result = _dynamics.Accounts.Get(filter: filter, select: select).Value;
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee,"Error getting accounts");

                // fail if we can't get results.
                return new MessageResult() {
                    Success = false
                };
            }

            _logger.LogInformation($"Found {result.Count} organizations to query orgbook for.");

            // now for each one process it.
            foreach (var item in result)
            {
                string registrationId = item.AdoxioBcincorporationnumber;
                string accountId = item.Accountid;
                int? orgbookTopicId = await _orgbookClient.GetTopicId(registrationId);

                if (orgbookTopicId != null)
                {
                    string orgbookLink = _orgbookClient.ORGBOOK_BASE_URL + "/en/organization/registration.registries.ca/" + item.AdoxioBcincorporationnumber;
                    _dynamics.Accounts.Update(accountId, new MicrosoftDynamicsCRMaccount()
                    {
                        AdoxioOrgbookorganizationlink = orgbookLink,
                        AdoxioIsorgbooklinkfound = 845280000
                    });
                    _logger.LogInformation($"Successfully added orgbook link to account with registration id {registrationId}.");
                }
                else
                {
                    _dynamics.Accounts.Update(accountId, new MicrosoftDynamicsCRMaccount()
                    {
                        AdoxioIsorgbooklinkfound = 845280001
                    });
                    _logger.LogError($"Failed to add orgbook link to account with registration id {registrationId}.");
                }
            }

            _logger.LogInformation($"Ending SyncOrgbookToAccounts");
            return new MessageResult() {
                Success = true
            };
        }

        public override async Task<MessageResult> CompanyExistsInOrgbook(CompanyNameRequest request, ServerCallContext context)
        {
            var result = await _orgbookClient.SearchCompanyName(request.CompanyName);
            return new MessageResult() {
                Success = result != null
            };
        }

        public override async Task<CompaniesNameResult> CompaniesExistInOrgbook(CompaniesNameRequest request, ServerCallContext context)
        {
            List<bool> results = new List<bool>();
            foreach(string name in request.CompanyNames)
            {
                CompanyNameRequest req = new CompanyNameRequest()
                {
                    CompanyName = name
                };
                MessageResult exists = await this.CompanyExistsInOrgbook(req, context);
                if (exists.Success)
                {
                    results.Add(true);
                }
                else
                {
                    results.Add(false);
                }
            }
            CompaniesNameResult result = new CompaniesNameResult();
            result.CompanyNames.AddRange(request.CompanyNames);
            result.Results.AddRange(results);
            return result;
        }
    }
}
