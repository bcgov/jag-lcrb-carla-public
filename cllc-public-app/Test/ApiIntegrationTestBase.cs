using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.IO;
using System.Net.Http;

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
            var builder = WebHost.CreateDefaultBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>();

            _server = new TestServer(builder);
            
            string testUserName = "TMcTesterson";
            _client = _server.CreateClient();
            _client.DefaultRequestHeaders.Add("DEV-USER", testUserName);
        }
    }
}
