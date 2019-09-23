using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class GeocoderClient
    {

        public string BaseUri { get; set; }
        
        private HttpClient client;
        private IConfiguration _configuration;
        private ILogger _logger;
        

        public GeocoderClient(IConfiguration configuration)
        {

            _configuration = configuration;
            
            if (! string.IsNullOrEmpty (_configuration["GEOCODER_SERVICE_BASE_URI"]) && !string.IsNullOrEmpty(_configuration["GEOCODER_JWT_TOKEN"]))
            {


                BaseUri = _configuration["GEOCODER_SERVICE_BASE_URI"];
                string bearer_token = $"Bearer {_configuration["GEOCODER_JWT_TOKEN"]}";

                // create the HttpClient that is used for our direct REST calls.
                client = new HttpClient();

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", bearer_token);
            }
            
        }
        
        /// <summary>
        /// GetPDF
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task GeocodeEstablishment(string establishmentId, ILogger logger)
        {


            HttpRequestMessage endpointRequest =
                new HttpRequestMessage(HttpMethod.Get, BaseUri + "/api/geocoder/GeocodeEstablishment/" + establishmentId);
            
            // make the request.
            var response = await client.SendAsync(endpointRequest);
            HttpStatusCode _statusCode = response.StatusCode;

            if (_statusCode == HttpStatusCode.OK)
            {
                logger.LogInformation("Geocoded establishment " + establishmentId);
            } else {
                logger.LogError("Unable to gecode establishment " + establishmentId);

            }
            
        }


        /// <summary>
        /// GetPDF
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
                var response = await client.SendAsync(endpointRequest);
                HttpStatusCode _statusCode = response.StatusCode;

                if (_statusCode == HttpStatusCode.OK)
                {
                    result = true;
                }
            }
            catch ( Exception )
            {

            }
            

            return result;

        }
    }
}
