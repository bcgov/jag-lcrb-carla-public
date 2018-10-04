using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class OneStopRestClient : IOneStopRestClient
    {
        public Uri BaseUri { get; set; }
        public string AuthorizationHeaderValue { get; set; }
        private readonly HttpClient httpClient = new HttpClient();

        public OneStopRestClient(Uri baseUri, string authorizationHeaderValue)
        {
            BaseUri = baseUri;
            AuthorizationHeaderValue = authorizationHeaderValue;
            httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
        }

        public async Task<string> receiveFromPartner(string inputXml)
        {
            var url = $"{BaseUri}?inputXml={Uri.EscapeDataString(inputXml)}";
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            } else
            {
                var content = await response.Content.ReadAsStringAsync();
                var ex = response.ReasonPhrase + " \n >>>" + content;
                throw new Exception(ex);
            }
        }
    }

    public interface IOneStopRestClient
    {
        Uri BaseUri { get; set; }
        string AuthorizationHeaderValue { get; set; }
        Task<string> receiveFromPartner(string inputXml);
    }
}
