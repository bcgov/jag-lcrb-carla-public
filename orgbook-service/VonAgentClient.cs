extern alias DV;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using adoxio_licences = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licences;

namespace Gov.Lclb.Cllb.OrgbookService
{
    public class VonAgentClient
    {
        readonly HttpClient Client;
        readonly ILogger _logger;
        readonly string _schema;
        readonly string _schemaVersion;
        readonly string AGENT_URL;
        readonly string ISSUE_URL = "issue-credential";
        readonly string _apiKey;

        public VonAgentClient(HttpClient client, ILogger logger, string schema, string schemaVersion, string agentURL, string apiKey)
        {
            Client = client;
            _logger = logger;
            _schema = schema;
            _schemaVersion = schemaVersion;
            AGENT_URL = agentURL;
            _apiKey = apiKey;
        }

        public async Task<bool> CreateLicenceCredential(adoxio_licences licence, string registrationId)
        {
            Credential credential = new Credential()
            {
                schema = _schema,
                version = _schemaVersion
            };

            string licenceTypeName = licence.adoxio_LicenceType?.Name;

            if (licenceTypeName == "Marketing")
            {
                credential.attributes = new Attributes()
                {
                    registration_id = registrationId,
                    licence_number = licence.adoxio_LicenceNumber,
                    issue_date = DateTime.UtcNow,
                    effective_date = licence.adoxio_EffectiveDate.HasValue
                        ? new DateTimeOffset(licence.adoxio_EffectiveDate.Value, TimeSpan.Zero)
                        : (DateTimeOffset?)null,
                    expiry_date = licence.adoxio_ExpiryDate.HasValue
                        ? new DateTimeOffset(licence.adoxio_ExpiryDate.Value, TimeSpan.Zero)
                        : (DateTimeOffset?)null
                };
            }
            else if (licenceTypeName == "Cannabis Retail Store")
            {
                credential.attributes = new CRSAttributes()
                {
                    registration_id = registrationId,
                    licence_number = licence.adoxio_LicenceNumber,
                    issue_date = DateTime.UtcNow,
                    effective_date = licence.adoxio_EffectiveDate.HasValue
                        ? new DateTimeOffset(licence.adoxio_EffectiveDate.Value, TimeSpan.Zero)
                        : (DateTimeOffset?)null,
                    expiry_date = licence.adoxio_ExpiryDate.HasValue
                        ? new DateTimeOffset(licence.adoxio_ExpiryDate.Value, TimeSpan.Zero)
                        : (DateTimeOffset?)null,
                    establishment_name = licence.adoxio_establishment?.Name,
                    civic_address = licence.adoxio_EstablishmentAddressStreet,
                    city = licence.adoxio_EstablishmentAddressCity,
                    province = "BC",
                    postal_code = licence.adoxio_EstablishmentAddressPostalCode,
                    country = "Canada"
                };
            }

            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, AGENT_URL + ISSUE_URL);
                request.Headers.Add("x-api-key", _apiKey);
                string json = JsonConvert.SerializeObject(new List<Credential>() { credential });

                request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("x-api-key", _apiKey);

                HttpResponseMessage response = await http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to create verifiable credential for licence {licence.adoxio_LicenceNumber}");
                    _logger.LogError($"Status code from VON Agent: {response.StatusCode}");
                    _logger.LogError($"Response: {await response.Content.ReadAsStringAsync()}");
                    return false;
                }
                else
                {
                    AgentResponse resp = JsonConvert.DeserializeObject<List<AgentResponse>>(await response.Content.ReadAsStringAsync())[0];
                    if (!resp.Success)
                    {
                        _logger.LogError($"Failed to create verifiable credential for licence {licence.adoxio_LicenceNumber}");
                        _logger.LogError($"Status code from VON Agent: {response.StatusCode}");
                        _logger.LogError($"Response: {resp.Result}");
                        return false;
                    }
                    _logger.LogInformation($"Successfully created verifiable credential for licence {licence.adoxio_LicenceNumber}");
                    return true;
                }
            }
            catch (HttpRequestException)
            {
                _logger.LogError($"Failed to make licence issue request to {AGENT_URL + ISSUE_URL}");
                return false;
            }
        }
    }
}
