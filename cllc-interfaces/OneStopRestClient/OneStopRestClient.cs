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
                return response.Content.ToString();
            } else
            {
                var ex = response.ReasonPhrase + " \n" + response.Content.ToString();
                throw new Exception();
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
