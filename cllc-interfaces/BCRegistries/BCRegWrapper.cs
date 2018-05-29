using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace BCRegWrapper
{
    public class CompanyQuery
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<Company> ProcessCompanyQuery(string user, string password, string url) 
        {
            var serializer = new DataContractJsonSerializer(typeof(Company));

            var byteArray = Encoding.ASCII.GetBytes(user + ":" + password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "CaRLA");

            try 
            {
                var streamTask = client.GetStreamAsync(url);
                var company = serializer.ReadObject(await streamTask) as Company;
                return company;
            } 
            catch (System.Net.Http.HttpRequestException e) 
            {
                return null;
            }
        }
    }
}
