using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Interfaces.GeoCoder
{
    public static class GeocoderSetupUtil
    {

        class ApiKeyCredentials : ServiceClientCredentials
        {
            string _apiKey;
            public ApiKeyCredentials(string apiKey)
            {
                _apiKey = apiKey;
            }
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("apiKey", _apiKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        public static ServiceClientCredentials GetServiceClientCredentials(IConfiguration Configuration)
        {            
            string apiKey = Configuration["GEOCODER_API_KEY"];

            ServiceClientCredentials serviceClientCredentials = new ApiKeyCredentials(apiKey);
            
            return serviceClientCredentials;
        }

        /// <summary>
        /// Setup a Dynamics client.
        /// </summary>
        /// <param name="Configuration"></param>
        /// <returns></returns>
        public static IGeocoderClient SetupGeocoder(IConfiguration Configuration)
        {
            string geocoderUri = Configuration["GEOCODER_API_URI"]; // URI of the DataBC Geocoder

            if (string.IsNullOrEmpty(geocoderUri))
            {
                throw new Exception("Configuration setting GEOCODER_API_URI is blank.");
            }

            ServiceClientCredentials serviceClientCredentials = GetServiceClientCredentials(Configuration);

            IGeocoderClient client = new GeocoderClient(new Uri(geocoderUri), serviceClientCredentials);            

            return client;
        }
    }
}
