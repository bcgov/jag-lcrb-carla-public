using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

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
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            string serviceUri = _configuration["GEOCODER_SERVICE_BASE_URI"];
            string serviceSecret = _configuration["GEOCODER_JWT_SECRET"];
            string serviceToken = _configuration["GEOCODER_JWT_TOKEN"];
            if (!string.IsNullOrEmpty(serviceUri))
            {
                BaseUri = serviceUri;
                if (!string.IsNullOrEmpty(serviceSecret))
                {
                    // do a handshake with the REST service to get a token.
                    string token = GetToken(serviceSecret).GetAwaiter().GetResult();
                    if (token != null)
                    {
                        // remove the expires from the end.
                        if (token.IndexOf(" Expires") != -1)
                        {
                            token = token.Substring(0, token.IndexOf(" Expires"));
                            token = token.Replace("\"", "");
                        }
                        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    }
                }
            }
            // legacy method
            else if (!string.IsNullOrEmpty(serviceToken))
            {
                string bearer_token = $"Bearer {serviceToken}";
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
                logger.Error($"Unable to gecode establishment {establishmentId} because geocoder service is not configured.");
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
                    logger.Information("Geocoded establishment " + establishmentId);
                }
                else
                {
                    logger.Error("Unable to gecode establishment " + establishmentId);

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


        /// <summary>
        /// TestAuthentication
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<string> GetToken(string secret)
        {
            string result = null;
            HttpRequestMessage endpointRequest =
                new HttpRequestMessage(HttpMethod.Get, BaseUri + "/api/authentication/token/" + HttpUtility.UrlEncode(secret));

            // make the request.
            try
            {
                var response = await _client.SendAsync(endpointRequest);
                HttpStatusCode _statusCode = response.StatusCode;

                if (_statusCode == HttpStatusCode.OK)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception)
            {
                // ignore the authentication issue.
                result = null;
            }

            return result;

        }
    }
}
