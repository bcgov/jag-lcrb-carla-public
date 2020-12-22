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
        /// Gets a PDF file from the supplied inputs.
        /// </summary>
        /// <param name="parameters">Object holding the data to pass to the template for rendering a PDF file</param>
        /// <param name="template">The template to render as PDF (e.g. "cannabis_licence")</param>
        /// <returns>Binary data representing the PDF file</returns>
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

        /// <summary>
        /// Gets a hash of the generated PDF so that consumer code can determine whether to re-download the same file or not.
        /// </summary>
        /// <param name="parameters">Object holding the data to pass to the template for rendering a PDF file</param>
        /// <param name="template">The template to render as PDF (e.g. "cannabis_licence")</param>
        /// <returns>A string with the hash value</returns>
        public async Task<string> GetPdfHash(Dictionary<string, string> parameters, string template)
        {
            string result = null;

            HttpRequestMessage endpointRequest =
                new HttpRequestMessage(HttpMethod.Post, BaseUri + "/api/pdf/GetHash/" + template);

            string jsonString = JsonConvert.SerializeObject(parameters);
            StringContent strContent = new StringContent(jsonString, Encoding.UTF8);
            strContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            endpointRequest.Content = strContent;

            // make the request.
            var response = await _client.SendAsync(endpointRequest);
            var _statusCode = response.StatusCode;

            if (_statusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                result = dict["hash"];
            }
            else
            {
                throw new Exception("PDF service did not return OK result.");
            }

            return result;
        }
    }
}
