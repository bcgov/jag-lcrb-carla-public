using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Net;
using Xunit;
using Newtonsoft.Json;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;


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

		public string randomNewUserName(string userid, int len)
		{
			return userid + TestUtilities.RandomANString(len);
		}

        // this fellow returns the external id of the new account
		public async System.Threading.Tasks.Task<string> LoginAndRegisterAsNewUser(string loginUser)
		{
			string accountService = "account";

			await Login(loginUser);

			ViewModels.User user = await GetCurrentUser();
            Assert.Equal(user.name, loginUser + " TestUser");
            Assert.Equal(user.businessname, loginUser + " TestBusiness");
            Assert.True(user.isNewUser);

            // create a new account and contact in Dynamics
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService);

            Account account = new Account()
            {
                Name = user.businessname,
                Adoxio_externalid = user.accountid
            };

            ViewModels.Account viewmodel_account = account.ToViewModel();
            Assert.Equal(account.Adoxio_externalid, viewmodel_account.externalId);

            string jsonString2 = JsonConvert.SerializeObject(viewmodel_account);
            request.Content = new StringContent(jsonString2, Encoding.UTF8, "application/json");
			var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.Account responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);

            // name should match.
            Assert.Equal(user.businessname, responseViewModel.name);
            string strId = responseViewModel.externalId;
            string id = responseViewModel.id;
            Assert.Equal(strId, user.accountid);

            // verify we can fetch the account via web service
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + accountService + "/" + id);
            response = await _client.SendAsync(request);
            string _discard = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

			// test that the current user is updated
			user = await GetCurrentUser();
			Assert.NotNull(user.accountid);
			Assert.NotEmpty(user.accountid);
			Assert.Equal(strId, user.accountid);

			return id;
		}

		public async System.Threading.Tasks.Task Logout() 
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "/login/cleartoken");
            var response = await _client.SendAsync(request);
            Assert.Equal(response.StatusCode, HttpStatusCode.Found);
			_client.DefaultRequestHeaders.Remove("DEV-USER");
            string _discard = await response.Content.ReadAsStringAsync();
		}

        public async System.Threading.Tasks.Task LogoutAndCleanupTestUser(string strId)
		{
			string accountService = "account";

			// cleanup - delete the account and contract when we are done
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService + "/" + strId + "/delete");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var _discard = await response.Content.ReadAsStringAsync();

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
		}

		public async System.Threading.Tasks.Task<ViewModels.User> GetCurrentUser()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            var response = await _client.SendAsync(request);
			response.EnsureSuccessStatusCode();
            string resp = await response.Content.ReadAsStringAsync();
			ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(resp);
            return user;
        }

		public async System.Threading.Tasks.Task<ViewModels.Account> GetAccountForCurrentUser()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/account/current");
			var response = await _client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			var jsonString = await response.Content.ReadAsStringAsync();
			var currentAccount = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);
			return currentAccount;
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
