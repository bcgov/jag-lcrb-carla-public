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
            Assert.False(user.isNewUser);

			await Logout();

			await GetCurrentUserIsUnauthorized();
        }

        [Fact]
        public async System.Threading.Tasks.Task NewUserRegistrationProcessWorks()
        {
			string accountService = "account";

			await GetCurrentUserIsUnauthorized();

			// register as a new user
			var loginUser = "NewUser" + TestUtilities.RandomANString(6);
			await Login(loginUser);

			string jsonString = await GetCurrentUser();

			ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);
			Assert.Equal(user.name, loginUser + " TestUser");
			Assert.Equal(user.businessname, loginUser + " TestBusiness");
            Assert.True(user.isNewUser);
            
            // create a new account and contact in Dynamics
			var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService);

			var accountId = new Guid(user.accountid);
            Account account = new Account()
            {
				Accountid = accountId,
				Name = user.businessname,
				Adoxio_externalid = user.accountid
			};
            
            ViewModels.Account viewmodel_account = account.ToViewModel();
			Assert.Equal(account.Accountid, new Guid(viewmodel_account.id));
			Assert.Equal(account.Adoxio_externalid, viewmodel_account.id);

            string jsonString2 = JsonConvert.SerializeObject(viewmodel_account);
            request.Content = new StringContent(jsonString2, Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.Account responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);

            // name should match.
			Assert.Equal(user.businessname, responseViewModel.name);
            Guid id = new Guid(responseViewModel.id);
			Assert.Equal(accountId, id);
			string strId = responseViewModel.externalId;

            // verify we can fetch the account via web service
			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + accountService + "/" + strId);
            response = await _client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			String _discard = await response.Content.ReadAsStringAsync();

            // TODO return account id
			// return id;

            // cleanup - delete the account and contract when we are done
			request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService + "/" + strId + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
			_discard = await response.Content.ReadAsStringAsync();

            // second delete should return a 404.
			request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService + "/" + strId + "/delete");
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			_discard = await response.Content.ReadAsStringAsync();

            // should get a 404 if we try a get now.
			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + accountService + "/" + strId);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			_discard = await response.Content.ReadAsStringAsync();
            
			await Logout();

            await GetCurrentUserIsUnauthorized();
        }
	}
}
