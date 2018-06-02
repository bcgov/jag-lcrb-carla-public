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
using Logos.Utility;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class LoginControllerTests : ApiIntegrationTestBaseWithLogin
    {
        [Fact]
        public async System.Threading.Tasks.Task DefaultUserIsAnonymous()
        {
			await GetCurrentUserIsUnauthorized();
        }

        [Fact]
        public async System.Threading.Tasks.Task LoginSetsCurrentUserThenLogoutIsAnonymous()
        {
			await LoginAsDefault();

			string jsonString = await GetCurrentUser();
            
            // Verify the Default development user.
			ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);
            Assert.Equal(user.name, "TMcTesterson TestUser");
            Assert.True(user.isNewUser);

			await Logout();

			await GetCurrentUserIsUnauthorized();
        }

        [Fact]
        public async System.Threading.Tasks.Task NewUserRegistrationProcessWorks()
        {
			string accountService = "account";

			await GetCurrentUserIsUnauthorized();

			// register as a new user
			var loginUser = "iannewuser000"; // "NewUser" + TestUtilities.RandomANString(6);
			await Login(loginUser);

			string jsonString = await GetCurrentUser();

			ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);
			Assert.Equal(user.name, loginUser + " TestUser");
			Assert.Equal(user.businessname, loginUser + " TestBusiness");
            Assert.True(user.isNewUser);
            /*
            // now create an account and contact in Dynamics
			var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService);

			var accountId = GuidUtility.CreateIdForDynamics(accountService, user.businessname);
			//Assert.Equal(new Guid("c23089ef-19dc-59d0-8aef-db9521785a94"), accountId);
            Account account = new Account()
            {
				Accountid = accountId,
				Name = user.businessname
            };

            ViewModels.Account viewmodel_account = account.ToViewModel();

            string jsonString2 = JsonConvert.SerializeObject(viewmodel_account);

            request.Content = new StringContent(jsonString2, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.Account responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);

            // name should match.
			Assert.Equal(user.businessname, responseViewModel.name);
            Guid id = new Guid(responseViewModel.id);
			Assert.Equal(accountId, id);

			request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // second delete should return a 404.
			request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // should get a 404 if we try a get now.
			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + accountService + "/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            */
			await Logout();

            await GetCurrentUserIsUnauthorized();
        }
	}
}
