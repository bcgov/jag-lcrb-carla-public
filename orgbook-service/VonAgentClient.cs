using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        readonly string _apiKey ;

        public VonAgentClient(HttpClient client, ILogger logger, string schema, string schemaVersion, string agentURL, string apiKey)
        {
            Client = client;
            _logger = logger;
            _schema = schema;
            _schemaVersion = schemaVersion;
            AGENT_URL = agentURL;
            _apiKey = apiKey;
        }

        public async Task<bool> CreateLicenceCredential(MicrosoftDynamicsCRMadoxioLicences licence, string registrationId)
        {
            Credential credential = new Credential()
            {
                schema = _schema,
                version = _schemaVersion
            };

            if (licence.AdoxioLicenceType.AdoxioName == "Marketing")
            {
                credential.attributes = new Attributes()
                {
                    registration_id = registrationId,
                    licence_number = licence.AdoxioLicencenumber,
                    issue_date = DateTime.UtcNow,
                    effective_date = licence.AdoxioEffectivedate,
                    expiry_date = licence.AdoxioExpirydate
                };
            }
            else if (licence.AdoxioLicenceType.AdoxioName == "Cannabis Retail Store")
            {
                credential.attributes = new CRSAttributes()
                {
                    registration_id = registrationId,
                    licence_number = licence.AdoxioLicencenumber,
                    issue_date = DateTime.UtcNow,
                    effective_date = licence.AdoxioEffectivedate,
                    expiry_date = licence.AdoxioExpirydate,
                    establishment_name = licence.AdoxioEstablishment?.AdoxioName,
                    civic_address = licence.AdoxioEstablishmentaddressstreet,
                    city = licence.AdoxioEstablishmentaddresscity,
                    province = "BC",
                    postal_code = licence.AdoxioEstablishmentaddresspostalcode,
                    country = "Canada"
                };
            }
            
            try
            {
                // can't use PostAsJson in dotnet core

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, AGENT_URL + ISSUE_URL);
                request.Headers.Add("x-api-key", _apiKey);  
                string json = JsonConvert.SerializeObject(new List<Credential>() { credential });

                request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("x-api-key", _apiKey);

                HttpResponseMessage response = await http.SendAsync(request);                

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to create verifiable credential for licence {licence.AdoxioLicencenumber}");
                    _logger.LogError($"Status code from VON Agent: {response.StatusCode}");
                    _logger.LogError($"Response: {await response.Content.ReadAsStringAsync()}");
                    return false;
                }
                else
                {
                    AgentResponse resp = JsonConvert.DeserializeObject<List<AgentResponse>>(await response.Content.ReadAsStringAsync())[0];
                    if (!resp.Success) {
                        _logger.LogError($"Failed to create verifiable credential for licence {licence.AdoxioLicencenumber}");
                        _logger.LogError($"Status code from VON Agent: {response.StatusCode}");
                        _logger.LogError($"Response: {resp.Result}");
                        return false;
                    }
                    _logger.LogInformation($"Successfully created verifiable credential for licence {licence.AdoxioLicencenumber}");
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
