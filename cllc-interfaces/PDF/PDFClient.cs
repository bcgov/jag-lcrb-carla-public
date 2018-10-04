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
    public class PdfClient
    {

        public string BaseUri { get; set; }
        
        private HttpClient client;

        public PdfClient ( string baseUri, string Authorization )
        {
            BaseUri = baseUri;                       

            // create the HttpClient that is used for our direct REST calls.
            client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", Authorization);
            
        }
        
        
        /// <summary>
        /// GetPDF
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<byte[]> GetPdf(Dictionary<string,string> parameters, string template)
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
            var response = await client.SendAsync(endpointRequest);
            HttpStatusCode _statusCode = response.StatusCode;

            if (_statusCode == HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsByteArrayAsync();
            }

            return result;
        }
    }
}
