using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class OrgBookUtils
    {
        private IConfiguration Configuration { get; }
        private ILogger _logger;
        private IDynamicsClient _dynamics;
        private OrgBookClient _orgbookClient;

        public OrgBookUtils(IConfiguration Configuration, ILogger logger)
        {
            this.Configuration = Configuration;
            if (Configuration["DYNAMICS_ODATA_URI"] != null)
            {
                _dynamics = DynamicsSetupUtil.SetupDynamics(Configuration);
            }
            _logger = logger;
            _orgbookClient = new OrgBookClient(new HttpClient(), Configuration["ORGBOOK_URL"]);
        }

        /// <summary>
        /// Hangfire job to check for and send recent licences
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForNewLicences(PerformContext hangfireContext)
        {
            if (hangfireContext != null)
            {
                _logger.LogInformation("Starting check for new licences for orgbook job.");
                hangfireContext.WriteLine("Starting check for new licences for orgbook job.");
            }
            IList<MicrosoftDynamicsCRMadoxioLicences> result = null;
            try
            {
                var expand = new List<string> { "adoxio_Licencee", "adoxio_LicenceType" };
                string filter = $"adoxio_orgbookcredentialresult eq null and statuscode eq 1";
                result = _dynamics.Licenceses.Get(filter: filter, expand: expand).Value;
            }
            catch (HttpOperationException odee)
            {
                if (hangfireContext != null)
                {
                    _logger.LogError("Error getting Licences");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                    hangfireContext.WriteLine("Error getting Licences");
                    hangfireContext.WriteLine("Request:");
                    hangfireContext.WriteLine(odee.Request.Content);
                    hangfireContext.WriteLine("Response:");
                    hangfireContext.WriteLine(odee.Response.Content);
                }

                // fail if we can't get results.
                throw (odee);
            }

            // now for each one process it.
            foreach (var item in result)
            {
                string registrationId = item.AdoxioLicencee?.AdoxioBcincorporationnumber;
                string licenceId = item.AdoxioLicencesid;
                int? orgbookTopicId = await _orgbookClient.GetTopicId(registrationId);
                string licenceType = item.AdoxioLicenceType?.AdoxioName;
                var (schema, schemaVersion) = GetSchemaFromConfig(licenceType);

                if (orgbookTopicId == null)
                {
                    _dynamics.Licenceses.Update(licenceId, new MicrosoftDynamicsCRMadoxioLicences() { AdoxioOrgbookcredentialresult = (int)OrgBookCredentialStatus.Fail });
                    _logger.LogError($"Failed to issue credential - Registration ID: {registrationId} does not exist.");
                    hangfireContext.WriteLine($"Failed to issue credential - Registration ID: {registrationId} does not exist.");
                }
                else if(schema == null || schemaVersion == null)
                {
                    _dynamics.Licenceses.Update(licenceId, new MicrosoftDynamicsCRMadoxioLicences() { AdoxioOrgbookcredentialresult = (int)OrgBookCredentialStatus.Fail });
                    _logger.LogError($"Schema {licenceType} not found.");
                    hangfireContext.WriteLine($"Schema {licenceType} not found.");
                }
                else
                {
                    string licenceGuid = Utils.ParseGuid(licenceId);
                    var licence = _dynamics.GetLicenceByIdWithChildren(licenceGuid);
                    VonAgentClient _vonAgentClient = new VonAgentClient(new HttpClient(), _logger, schema, schemaVersion, Configuration["AGENT_URL"]);
                    bool issueSuccess = await _vonAgentClient.CreateLicenceCredential(licence, registrationId);

                    if(issueSuccess)
                    {
                        _dynamics.Licenceses.Update(licenceId, new MicrosoftDynamicsCRMadoxioLicences()
                        {
                            AdoxioOrgbookcredentialresult = (int)OrgBookCredentialStatus.Pass
                        });
                        _logger.LogInformation($"Successfully issued credential to {registrationId}.");
                        hangfireContext.WriteLine($"Successfully issued credential to {registrationId}.");
                    }
                    else
                    {
                        _dynamics.Licenceses.Update(licenceId, new MicrosoftDynamicsCRMadoxioLicences()
                        {
                            AdoxioOrgbookcredentialresult = (int)OrgBookCredentialStatus.Fail
                        });
                        _logger.LogInformation($"Failed to issue licence credential to {registrationId}.");
                        hangfireContext.WriteLine($"Failed to issue licence credential to {registrationId}.");
                    }
                }
            }

            _logger.LogInformation("End of check for new licences for orgbook job.");
            hangfireContext.WriteLine("End of check for new licences for orgbook job.");
        }

        /// <summary>
        /// Hangfire job to check for credentials that have been created but not updated yet
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForMissingCredentials(PerformContext hangfireContext)
        {
            if (hangfireContext != null)
            {
                _logger.LogInformation("Starting check for new issued credentials.");
                hangfireContext.WriteLine("Starting check for new issued credentials.");
            }
            IList<MicrosoftDynamicsCRMadoxioLicences> result = null;
            try
            {
                var expand = new List<string> { "adoxio_Licencee", "adoxio_LicenceType" };
                string filter = $"adoxio_orgbookcredentialresult eq {(int)OrgBookCredentialStatus.Pass} and adoxio_orgbookcredentialid eq null and statuscode eq 1";
                result = _dynamics.Licenceses.Get(filter: filter, expand: expand).Value;
            }
            catch (HttpOperationException odee)
            {
                if (hangfireContext != null)
                {
                    _logger.LogError("Error getting Licences");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                    hangfireContext.WriteLine("Error getting Licences");
                    hangfireContext.WriteLine("Request:");
                    hangfireContext.WriteLine(odee.Request.Content);
                    hangfireContext.WriteLine("Response:");
                    hangfireContext.WriteLine(odee.Response.Content);
                }

                // fail if we can't get results.
                throw (odee);
            }

            // now for each one process it.
            foreach (var item in result)
            {
                string registrationId = item.AdoxioLicencee?.AdoxioBcincorporationnumber;
                string licenceId = item.AdoxioLicencesid;
                int? orgbookTopicId = await _orgbookClient.GetTopicId(registrationId);

                if (orgbookTopicId != null)
                {
                    var (schemaName, schemaVersion) = GetSchemaFromConfig(item.AdoxioLicenceType.AdoxioName);
                    
                    var schemaId = await _orgbookClient.GetSchemaId(schemaName, schemaVersion);
                    var credentialId = await _orgbookClient.GetLicenceCredentialId((int)orgbookTopicId, (int)schemaId);

                    _dynamics.Licenceses.Update(licenceId, new MicrosoftDynamicsCRMadoxioLicences()
                    {
                        AdoxioOrgbookcredentialid = credentialId.ToString()
                    });
                    _logger.LogInformation($"Successfully updated licence - credential ID: {credentialId} to {registrationId}.");
                    hangfireContext.WriteLine($"Successfully updated licence - credential ID: {credentialId} to {registrationId}.");
                }
                else
                {
                    _logger.LogError($"Failed to update licence with new credential ID for Registration ID: {registrationId}.");
                    hangfireContext.WriteLine($"Failed to update licence with new credential ID for Registration ID: {registrationId}.");
                }
            }

            _logger.LogInformation("End of check for new licences for orgbook job.");
            hangfireContext.WriteLine("End of check for new licences for orgbook job.");
        }

        private static (string, string) GetSchemaFromConfig(string licenceType)
        {
            using (StreamReader file = File.OpenText(@"CredentialSchemaConfig.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<Schema> schemas = (List<Schema>)serializer.Deserialize(file, typeof(List<Schema>));
                Schema schema = schemas.Find((obj) => obj.type == licenceType);

                return schema == null ? (null, null) : (schema.name, schema.version);
            }
        }
    }
}
