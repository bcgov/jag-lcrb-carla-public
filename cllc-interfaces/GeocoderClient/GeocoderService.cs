using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class GeocoderService : IGeocoderService
    {
        public string BaseUri { get; set; }

        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;


        public GeocoderService(HttpClient httpClient, IConfiguration configuration)
        {

            _configuration = configuration;
            // create the HttpClient that is used for our direct REST calls.
            _client = httpClient;

            if (!string.IsNullOrEmpty(_configuration["GEOCODER_SERVICE_BASE_URI"]) && !string.IsNullOrEmpty(_configuration["GEOCODER_JWT_TOKEN"]))
            {
                BaseUri = _configuration["GEOCODER_SERVICE_BASE_URI"];
                string bearer_token = $"Bearer {_configuration["GEOCODER_JWT_TOKEN"]}";

                _client.DefaultRequestHeaders.Add("Accept", "application/json");
                _client.DefaultRequestHeaders.Add("Authorization", bearer_token);
            }

        }

        /// <summary>
        /// GeocodeEstablishment
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task GeocodeEstablishment(string establishmentId, ILogger logger)
        {

            if (String.IsNullOrEmpty (BaseUri))
            {
                logger.LogError($"Unable to gecode establishment {establishmentId} because geocoder service is not configured.");
            }
            else
            {
                HttpRequestMessage endpointRequest =
                new HttpRequestMessage(HttpMethod.Get, BaseUri + "/api/geocoder/GeocodeEstablishment/" + establishmentId);

                // make the request.
                var response = await _client.SendAsync(endpointRequest);
                HttpStatusCode _statusCode = response.StatusCode;

                if (_statusCode == HttpStatusCode.OK)
                {
                    logger.LogInformation("Geocoded establishment " + establishmentId);
                }
                else
                {
                    logger.LogError("Unable to gecode establishment " + establishmentId);

                }
            }            
        }


        /// <summary>
        /// TestAuthentication
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> TestAuthentication()
        {
            bool result = false;

            HttpRequestMessage endpointRequest =
                new HttpRequestMessage(HttpMethod.Get, BaseUri + "/api/authentication/test");

            // make the request.
            try
            {
                var response = await _client.SendAsync(endpointRequest);
                HttpStatusCode _statusCode = response.StatusCode;

                if (_statusCode == HttpStatusCode.OK)
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                // ignore the authentication issue.
                result = false;
            }


            return result;

        }
    }
}
