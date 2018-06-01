using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using Xunit;


namespace Gov.Lclb.Cllb.Public.Test
{
    public abstract class ApiIntegrationTestBaseWithLogin
    {
        protected readonly TestServer _server;
        protected readonly HttpClient _client;

        /// <summary>
        /// Setup the test
        /// </summary>        
		protected ApiIntegrationTestBaseWithLogin()
        {
            var testConfig = new ConfigurationBuilder()
                .AddUserSecrets<ApiIntegrationTestBase>()
                .AddEnvironmentVariables()
                .Build();

            var builder = WebHost.CreateDefaultBuilder()               
                .UseEnvironment("Staging")
                .UseConfiguration(testConfig)
                .UseStartup<Startup>();

            _server = new TestServer(builder);
			_client = _server.CreateClient();
        }

		public async System.Threading.Tasks.Task Login(string userid)
        {
			var request = new HttpRequestMessage(HttpMethod.Get, "/login/token/" + userid);
            var response = await _client.SendAsync(request);
            Assert.Equal(response.StatusCode, HttpStatusCode.Found);
            string _discard = await response.Content.ReadAsStringAsync();
            _client.DefaultRequestHeaders.Add("DEV-USER", userid);
        }

		public async System.Threading.Tasks.Task LoginAsDefault()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/login/token/default");
            var response = await _client.SendAsync(request);
            Assert.Equal(response.StatusCode, HttpStatusCode.Found);
			_client.DefaultRequestHeaders.Add("DEV-USER", "TMcTesterson");
            string _discard = await response.Content.ReadAsStringAsync();
        }

		public async System.Threading.Tasks.Task Logout() 
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "/login/cleartoken");
            var response = await _client.SendAsync(request);
            Assert.Equal(response.StatusCode, HttpStatusCode.Found);
			_client.DefaultRequestHeaders.Remove("DEV-USER");
            string _discard = await response.Content.ReadAsStringAsync();
		}

        public async System.Threading.Tasks.Task<string> GetCurrentUser()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            var response = await _client.SendAsync(request);
			response.EnsureSuccessStatusCode();
            string resp = await response.Content.ReadAsStringAsync();
            return resp;
        }

        public async System.Threading.Tasks.Task GetCurrentUserIsUnauthorized()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            var response = await _client.SendAsync(request);
            Assert.Equal(response.StatusCode, HttpStatusCode.Unauthorized);
			string _discard = await response.Content.ReadAsStringAsync();
        }
    }
}
