using Microsoft.Extensions.Configuration;
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
    public class PdfService : IPdfService
    {

        public string BaseUri { get; set; }

        private HttpClient _client;

        public PdfService(HttpClient httpClient, IConfiguration configuration)
        {
            string pdf_service_base_uri = configuration["PDF_SERVICE_BASE_URI"];
            string bearer_token = $"Bearer {configuration["PDF_JWT_TOKEN"]}";

            BaseUri = pdf_service_base_uri;

            // configure the HttpClient that is used for our direct REST calls.
            _client = httpClient;
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("Authorization", bearer_token);
        }


        /// <summary>
        /// GetPDF
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<byte[]> GetPdf(Dictionary<string, string> parameters, string template)
        {
            byte[] result = null;

            HttpRequestMessage endpointRequest =
                new HttpRequestMessage(HttpMethod.Post, BaseUri + "/api/pdf/GetPdf/" + template);

            //HttpRequestMessage endpointRequest =
            //    new HttpRequestMessage(HttpMethod.Get, BaseUri + "/api/pdf/GetTestPDF");

            string jsonString = JsonConvert.SerializeObject(parameters);
            StringContent strContent = new StringContent(jsonString, Encoding.UTF8);
            strContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            endpointRequest.Content = strContent;

            // make the request.
            var response = await _client.SendAsync(endpointRequest);
            HttpStatusCode _statusCode = response.StatusCode;

            if (_statusCode == HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                throw new Exception("PDF service did not return OK result.");
            }

            return result;
        }
    }
}
