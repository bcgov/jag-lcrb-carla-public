using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gov.Lclb.Cllb.OrgbookService
{
    public class OrgBookClient
    {
        private readonly HttpClient Client;
        public readonly string ORGBOOK_BASE_URL;
        public readonly string ORGBOOK_API_REGISTRATION_ENDPOINT = "/api/v2/topic/ident/registration/";
        public readonly string ORGBOOK_API_SCHEMA_ENDPOINT = "/api/v2/schema";
        public readonly string ORGBOOK_API_CREDENTIAL_ENDPOINT = "/api/v2/search/credential/topic";
        
        public OrgBookClient(HttpClient client, string BASE_URL)
        {
            ORGBOOK_BASE_URL = BASE_URL;
            Client = client;
        }

        public async Task<int?> GetTopicId(string registrationId)
        {
            HttpResponseMessage resp = await Client.GetAsync(ORGBOOK_BASE_URL + ORGBOOK_API_REGISTRATION_ENDPOINT + registrationId);
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
            HttpResponseMessage resp = await Client.GetAsync(ORGBOOK_BASE_URL + ORGBOOK_API_SCHEMA_ENDPOINT + "?name=" + schemaName + "&version=" + schemaVersion);
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

        public async Task<int?> GetLicenceCredentialId(int topicId, int schemaId, string licenceNumber)
        {
            HttpResponseMessage resp = await Client.GetAsync(ORGBOOK_BASE_URL + ORGBOOK_API_CREDENTIAL_ENDPOINT + $"?inactive=false&latest=true&revoked=false&credential_type_id={schemaId}&topic_id={topicId}");
            if (resp.IsSuccessStatusCode)
            {
                string _responseContent = await resp.Content.ReadAsStringAsync();
                dynamic response = JObject.Parse(_responseContent);
                foreach (JObject result in response.results)
                {
                    foreach (JObject attribute in result["attributes"])
                    {
                        if (attribute["type"].ToString() == "licence_number" && attribute["value"].ToString() == licenceNumber)
                        {
                            return int.Parse(result["id"].ToString());
                        }
                    }
                }
            }
            return null;
        }
    }
}
