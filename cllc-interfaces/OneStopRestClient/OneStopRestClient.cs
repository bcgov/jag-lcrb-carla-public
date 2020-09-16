using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class OneStopRestClient : IOneStopRestClient
    {
        public Uri BaseUri { get; set; }
        public string AuthorizationHeaderValue { get; set; }
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ILogger _logger;

        public OneStopRestClient(Uri baseUri, string authorizationHeaderValue, ILogger logger)
        {
            BaseUri = baseUri;
            AuthorizationHeaderValue = authorizationHeaderValue;
            _httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
            this._logger = logger;
        }

        public async Task<string> ReceiveFromPartner(string inputXml)
        {
            var url = $"{BaseUri}?inputXML={Uri.EscapeDataString(inputXml)}";
            _logger.Debug($"InputXML to send = {inputXml}");
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.Debug("OneStop message sequence completed successfully ");
                return content;
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                string ex = response.ReasonPhrase + " \n >>>" + content;
                _logger.Error($"Error received: {ex}");
                throw new Exception(ex);
            }
        }
    }

    public interface IOneStopRestClient
    {
        Uri BaseUri { get; set; }
        string AuthorizationHeaderValue { get; set; }
        Task<string> ReceiveFromPartner(string inputXml);
    }
}
