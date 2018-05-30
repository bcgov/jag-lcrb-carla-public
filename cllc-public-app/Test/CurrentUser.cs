using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

using System.Text;
using Newtonsoft.Json;
using System.Net;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class CurrentUserTests : ApiIntegrationTestBase
    {
        [Fact]
        public async System.Threading.Tasks.Task DefaultDevelopmentUserIsValid()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string jsonString = await response.Content.ReadAsStringAsync();

            ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);

            // The Default development user should not be a new user.
            Assert.False(user.isNewUser);
        }
    }
}
