using Microsoft.Extensions.Logging;
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
        private readonly ILogger logger;

        public OneStopRestClient(Uri baseUri, string authorizationHeaderValue, ILogger logger)
        {
            BaseUri = baseUri;
            AuthorizationHeaderValue = authorizationHeaderValue;
            httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
            this.logger = logger;
        }

        public async Task<string> receiveFromPartner(string inputXml)
        {
            var url = $"{BaseUri}?inputXML={Uri.EscapeDataString(inputXml)}";
            logger.LogInformation($"InputXML to send = {inputXml}");
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
