using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;


namespace Gov.Lclb.Cllb.Public.Test
{
    public abstract class ApiIntegrationTestBaseAnonymous
    {
        protected readonly TestServer _server;
        protected readonly HttpClient _client;

        /// <summary>
        /// Setup the test
        /// </summary>        
		protected ApiIntegrationTestBaseAnonymous()
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
    }
}
