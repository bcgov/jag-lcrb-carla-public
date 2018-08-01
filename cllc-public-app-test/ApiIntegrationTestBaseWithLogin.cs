using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public abstract class ApiIntegrationTestBaseWithLogin : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected readonly CustomWebApplicationFactory<Startup> _factory;

		public HttpClient _client { get; }


        public ApiIntegrationTestBaseWithLogin(CustomWebApplicationFactory<Startup> fixture)
        {
            _factory = fixture;
			_client = _factory
				.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
        }

        public async System.Threading.Tasks.Task Login(string userid)
		{
			await Login(userid, userid);
		}

        public async System.Threading.Tasks.Task Login(string userid, string businessName)
        {
			string loginAs = userid + "::" + businessName;
			_client.DefaultRequestHeaders.Add("DEV-USER", loginAs);
			var request = new HttpRequestMessage(HttpMethod.Get, "/cannabislicensing/login/token/" + loginAs);
            var response = await _client.SendAsync(request);
            string _discard = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            
        }
		
		public string randomNewUserName(string userid, int len)
		{
			return userid + TestUtilities.RandomANString(len);
		}

        // this fellow returns the external id of the new account
		public async System.Threading.Tasks.Task<string> LoginAndRegisterAsNewUser(string loginUser)
		{
			return await LoginAndRegisterAsNewUser(loginUser, loginUser);
		}

        // this fellow returns the external id of the new account
        public async System.Threading.Tasks.Task<string> LoginAndRegisterAsNewUser(string loginUser, string businessName, string businessType = "PublicCorporation")
		{
			string accountService = "account";

			await Login(loginUser + "::" + businessName);

			ViewModels.User user = await GetCurrentUser();
            Assert.Equal(user.name, loginUser + " TestUser");
			Assert.Equal(user.businessname, businessName + " TestBusiness");
            Assert.True(user.isNewUser);

            // create a new account and contact in Dynamics
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService);

            MicrosoftDynamicsCRMaccount account = new MicrosoftDynamicsCRMaccount()
            {
                Name = user.businessname,
                AdoxioExternalid = user.accountid
            };

            ViewModels.Account viewmodel_account = account.ToViewModel();

			viewmodel_account.businessType = businessType;

            Assert.Equal(account.AdoxioExternalid, viewmodel_account.externalId);

            string jsonString2 = JsonConvert.SerializeObject(viewmodel_account);
            request.Content = new StringContent(jsonString2, Encoding.UTF8, "application/json");
			var response = await _client.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();
			response.EnsureSuccessStatusCode();

			ViewModels.Account responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);

            // name should match.
            Assert.Equal(user.businessname, responseViewModel.name);
            string strId = responseViewModel.externalId;
            string id = responseViewModel.id;
			Assert.Equal(strId, responseViewModel.externalId);

            // verify we can fetch the account via web service
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + accountService + "/" + id);
            response = await _client.SendAsync(request);
            string _discard = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

			// test that the current user is updated
			user = await GetCurrentUser();
			Assert.NotNull(user.accountid);
			Assert.NotEmpty(user.accountid);
			Assert.Equal(id, user.accountid);

			return id;
		}

		public async System.Threading.Tasks.Task Logout() 
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "/login/cleartoken");
            var response = await _client.SendAsync(request);
			string _discard = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
			_client.DefaultRequestHeaders.Remove("DEV-USER");
		}

        public async System.Threading.Tasks.Task LogoutAndCleanupTestUser(string strId)
		{
			string accountService = "account";

            // get the account and check if our current user is the primary contact
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/" + accountService + "/" + strId);
            var response = await _client.SendAsync(request);
			string jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

			ViewModels.Account responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);

			ViewModels.User user = await GetCurrentUser();

			// TODO once AccountController is cleaned up restore this test
			Console.WriteLine(">>> responseViewModel.primarycontact.id=" + responseViewModel.primarycontact.id);
			Console.WriteLine(">>>                      user.contactid=" + user.contactid);
			Console.WriteLine(">>>                           user.name=" + user.name);
			if (responseViewModel.primarycontact.id.Equals(user.contactid))
			{
				// cleanup - delete the account and contact when we are done
				request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService + "/" + strId + "/delete");
				response = await _client.SendAsync(request);
				var _discard = await response.Content.ReadAsStringAsync();
				response.EnsureSuccessStatusCode();

				// second delete should return a 404.
				request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService + "/" + strId + "/delete");
				response = await _client.SendAsync(request);
				_discard = await response.Content.ReadAsStringAsync();
				Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

				// should get a 404 if we try a get now.
				request = new HttpRequestMessage(HttpMethod.Get, "/api/" + accountService + "/" + strId);
				response = await _client.SendAsync(request);
				_discard = await response.Content.ReadAsStringAsync();
				Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			}
			else
			{
				// TODO delete the non-primary contact
			}

            await Logout();
		}

		public async System.Threading.Tasks.Task<ViewModels.User> GetCurrentUser()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            var response = await _client.SendAsync(request);
            string resp = await response.Content.ReadAsStringAsync();
			response.EnsureSuccessStatusCode();
			ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(resp);

			return user;
        }

		public async System.Threading.Tasks.Task<ViewModels.Account> GetAccountForCurrentUser()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/account/current");
			var response = await _client.SendAsync(request);
			var jsonString = await response.Content.ReadAsStringAsync();
			response.EnsureSuccessStatusCode();
			var currentAccount = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);
			return currentAccount;
		}

		public async System.Threading.Tasks.Task GetCurrentUserIsUnauthorized()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            var response = await _client.SendAsync(request);
			string _discard = await response.Content.ReadAsStringAsync();
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
