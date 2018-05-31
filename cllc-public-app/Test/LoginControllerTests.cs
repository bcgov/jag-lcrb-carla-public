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
    public class LoginControllerTests : ApiIntegrationTestBaseAnonymous
    {
        [Fact]
        public async System.Threading.Tasks.Task DefaultUserIsAnonymous()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            
            var response = await _client.SendAsync(request);
			Assert.Equal(response.StatusCode, HttpStatusCode.Unauthorized);
        }

		[Fact]
        public async System.Threading.Tasks.Task LoginAsDefaultUserSetsCurrentUser()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/login/token/default");
            var response = await _client.SendAsync(request);
			Assert.Equal(response.StatusCode, HttpStatusCode.Found);
			string _discard = await response.Content.ReadAsStringAsync();

			_client.DefaultRequestHeaders.Add("DEV-USER", "TMcTesterson");

			var request2 = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            var response2 = await _client.SendAsync(request2);
            response2.EnsureSuccessStatusCode();
            string jsonString = await response2.Content.ReadAsStringAsync();
            // account and contact id's will be different ...
			//Assert.Equal(jsonString, "{\n  \"id\": \"TMcTesterson\",\n  \"name\": \"TMcTesterson TestUser\",\n  \"firstname\": \"TMcTesterson\",\n  \"lastname\": \"TestUser\",\n  \"email\": null,\n  \"businessname\": \"TMcTesterson TestBusiness\",\n  \"isNewUser\": true,\n  \"isContactCreated\": false,\n  \"isAccountCreated\": false,\n  \"isBceidConfirmed\": false,\n  \"contactid\": \"e901654e-34e6-4e16-9a5f-980bc08bbee7\",\n  \"accountid\": \"3ebb1d3a-0dfb-4fd2-b6f2-10cc08d95283\"\n}");

            ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);

            // Verify the Default development user.
			Assert.Equal(user.name, "TMcTesterson TestUser");
			Assert.True(user.isNewUser);
        }
	}
}
