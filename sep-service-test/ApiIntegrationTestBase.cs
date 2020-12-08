
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore;

using System.Net.Http;
using Microsoft.Extensions.Configuration;


namespace Gov.Lclb.Cllb.Public.Test
{
    public abstract class ApiIntegrationTestBase
    {
        protected readonly TestServer _server;
        protected readonly HttpClient _client;

        /// <summary>
        /// Setup the test
        /// </summary>        
        protected ApiIntegrationTestBase()
        {
            var testConfig = new ConfigurationBuilder()
                .AddUserSecrets<ApiIntegrationTestBase>()
                .AddEnvironmentVariables()
                .Build();

            var builder = new WebHostBuilder()               
                .UseEnvironment("Staging")
                .UseConfiguration(testConfig)
                .UseStartup<Startup>();

            _server = new TestServer(builder);
            
            string testUserName = "TMcTesterson";
            _client = _server.CreateClient();
            _client.DefaultRequestHeaders.Add("DEV-USER", testUserName);
        }
    }
}
