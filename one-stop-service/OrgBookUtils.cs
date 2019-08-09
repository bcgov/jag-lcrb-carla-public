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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class Schema
    {
        public string type { get; set; }
        public string name { get; set; }
        public string version { get; set; }
    }

    public class Attributes
    {
        public string registration_id { get; set; }
        public string licence_number { get; set; }
        public string establishment_name { get; set; }
        public DateTimeOffset issue_date { get; set; }
        public DateTimeOffset? effective_date { get; set; }
        public DateTimeOffset? expiry_date { get; set; }
        public string civic_address { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
    }

    public class Credential
    {
        public string schema { get; set; }
        public string version { get; set; }
        public Attributes attributes { get; set; }
    }
     

    public class OrgBookUtils
    {
        private static readonly HttpClient Client = new HttpClient();

        private IConfiguration Configuration { get; }

        private string ORGBOOK_API_BASE_URL;
        private string ORGBOOK_API_REGISTRATION_ENDPOINT = "/api/v2/topic/ident/registration/";
        private string ORGBOOK_API_SCHEMA_ENDPOINT = "/api/v2/schema";
        private string ORGBOOK_API_CREDENTIAL_ENDPOINT = "/api/v2/search/credential/topic";

        private IDynamicsClient _dynamics;

        private ILogger _logger;

        public OrgBookUtils(IConfiguration Configuration, ILogger logger)
        {
            this.Configuration = Configuration;
            _dynamics = DynamicsSetupUtil.SetupDynamics(Configuration);
            _logger = logger;
            ORGBOOK_API_BASE_URL = Configuration["ORGBOOK_URL"];
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
                var expand = new List<string> { "adoxio_Licencee"};
                string filter = $"adoxio_orgbookcredentialresult eq null";
                result = _dynamics.Licenceses.Get(filter: filter, expand: expand).Value;
            }
            catch (OdataerrorException odee)
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
                int? orgbookTopicId = await GetOrgBookTopicId(registrationId);

                if (orgbookTopicId != null)
                {
                    var (schema, schemaVersion) = await CreateLicenceCredential(hangfireContext, licenceId, registrationId);

                    _dynamics.Licenceses.Update(licenceId, new MicrosoftDynamicsCRMadoxioLicences()
                    {
                        AdoxioOrgbookcredentialresult = (int)OrgBookCredentialStatus.Pass
                    });
                    _logger.LogInformation($"Successfully issued credential to {registrationId}.");
                    hangfireContext.WriteLine($"Successfully issued credential to {registrationId}.");
                }
                else
                {
                    _dynamics.Licenceses.Update(licenceId, new MicrosoftDynamicsCRMadoxioLicences() { AdoxioOrgbookcredentialresult = (int)OrgBookCredentialStatus.Fail });
                    _logger.LogError($"Failed to issue credential - Registration ID: {registrationId} does not exist.");
                    hangfireContext.WriteLine($"Failed to issue credential - Registration ID: {registrationId} does not exist.");
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
                string filter = $"adoxio_orgbookcredentialresult eq {(int)OrgBookCredentialStatus.Pass} and adoxio_orgbookcredentialid eq null";
                result = _dynamics.Licenceses.Get(filter: filter, expand: expand).Value;
            }
            catch (OdataerrorException odee)
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
                int? orgbookTopicId = await GetOrgBookTopicId(registrationId);

                if (orgbookTopicId != null)
                {
                    var (schemaName, schemaVersion) = GetSchemaFromConfig(item.AdoxioLicenceType.AdoxioName);
                    
                    var schemaId = await GetSchemaId(schemaName, schemaVersion);
                    var credentialId = await GetLicenceCredentialId((int)orgbookTopicId, (int)schemaId);

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

        /// <summary>
        /// Hangfire job to create licence credential in OrgBook.
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task<(string, string)> CreateLicenceCredential(PerformContext hangfireContext, string licenceGuidRaw, string registrationId)
        {
            hangfireContext.WriteLine("Starting OrgBook CreateLicenceCredential Job.");
            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            hangfireContext.WriteLine($"Getting Licence {licenceGuid}");
            var licence = _dynamics.GetLicenceByIdWithChildren(licenceGuid);

            var (schema, schemaVersion) = GetSchemaFromConfig(licence.AdoxioLicenceType.AdoxioName);

            Credential credential = new Credential()
            {
                attributes = new Attributes()
                {
                    registration_id = registrationId,
                    licence_number = licence.AdoxioLicencenumber,
                    establishment_name = licence.AdoxioEstablishment?.AdoxioName,
                    issue_date = DateTime.UtcNow,
                    effective_date = licence.AdoxioEffectivedate,
                    expiry_date = licence.AdoxioExpirydate,
                    civic_address = licence.AdoxioEstablishmentaddressstreet,
                    city = licence.AdoxioEstablishmentaddresscity,
                    province = "BC",
                    postal_code = licence.AdoxioEstablishmentaddresspostalcode,
                    country = "Canada",
                },
                schema = schema,
                version = schemaVersion
            };

            HttpResponseMessage response = await Client.PostAsJsonAsync(
                Configuration["AGENT_URL"] + "lcrb/issue-credential", new List<Credential>() { credential });

            if (!response.IsSuccessStatusCode)
            {
                string _responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogError($"Failed to create verifiable credential for licence {licence.AdoxioLicencenumber}");
                hangfireContext.WriteLine($"Failed to create verifiable credential for licence {licence.AdoxioLicencenumber}");
            }

            _logger.LogInformation($"Successfully created verifiable credential for licence {licence.AdoxioLicencenumber}");
            hangfireContext.WriteLine($"Successfully created verifiable credential for licence {licence.AdoxioLicencenumber}");

            return (schema, schemaVersion);
        }

        public async Task<int?> GetOrgBookTopicId(string registrationId)
        {
            HttpResponseMessage resp = await Client.GetAsync(ORGBOOK_API_BASE_URL + ORGBOOK_API_REGISTRATION_ENDPOINT + registrationId);
            if (resp.IsSuccessStatusCode)
            {
                string _responseContent = await resp.Content.ReadAsStringAsync();
                var response = (JObject)JsonConvert.DeserializeObject(_responseContent);
                return (int)response.GetValue("id");
            }
            return null;
        }

        public async Task<int?> GetSchemaId(string schemaName, string schemaVersion)
        {
            HttpResponseMessage resp = await Client.GetAsync(ORGBOOK_API_BASE_URL + ORGBOOK_API_SCHEMA_ENDPOINT + "?name=" + schemaName + "&version=" + schemaVersion);
            if (resp.IsSuccessStatusCode)
            {
                string _responseContent = await resp.Content.ReadAsStringAsync();
                var response = (JObject)JsonConvert.DeserializeObject(_responseContent);
                int count = (int)response.GetValue("total");
                JArray results = (JArray)response.GetValue("results");
                if (count == 1)
                {
                    return results.First.Value<int>("id");
                }
            }
            return null;
        }

        public async Task<int?> GetLicenceCredentialId(int topicId, int schemaId)
        {
            HttpResponseMessage resp = await Client.GetAsync(ORGBOOK_API_BASE_URL + ORGBOOK_API_CREDENTIAL_ENDPOINT + $"?inactive=false&latest=true&revoked=false&credential_type_id={schemaId}&topic_id={topicId}");
            if (resp.IsSuccessStatusCode)
            {
                string _responseContent = await resp.Content.ReadAsStringAsync();
                var response = (JObject)JsonConvert.DeserializeObject(_responseContent);
                int count = (int)response.GetValue("total");
                JArray results = (JArray)response.GetValue("results");
                if (count == 1)
                {
                    return results.First.Value<int>("id");
                }
            }
            return null;
        }

        private static (string, string) GetSchemaFromConfig(string licenceType)
        {
            using (StreamReader file = File.OpenText(@"CredentialSchemaConfig.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<Schema> schemas = (List<Schema>)serializer.Deserialize(file, typeof(List<Schema>));
                Schema schema = schemas.Find((obj) => obj.type == licenceType);
                return (schema.name, schema.version);
            }
        }
    }
}
