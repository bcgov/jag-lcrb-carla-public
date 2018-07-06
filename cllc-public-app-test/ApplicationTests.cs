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
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Interfaces;


namespace Gov.Lclb.Cllb.Public.Test
{
	public class ApplicationTests : ApiIntegrationTestBaseWithLogin
    {
        public ApplicationTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory)
        { }

        [Fact]
        public async System.Threading.Tasks.Task TestNoAccessToAnonymousUser()
        {
			string service = "adoxioapplication";
            string id = "SomeRandomId";

            // first confirm we are not logged in
            await GetCurrentUserIsUnauthorized();

            // try a random GET, should return unauthorized
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            string _discard = await response.Content.ReadAsStringAsync();
        }
        
        [Fact]
        public async System.Threading.Tasks.Task TestCRUD()
        {
			string initialName = randomNewUserName("Application Initial Name ", 6);
			string changedName = randomNewUserName("Application Changed Name ", 6);
			string service = "adoxioapplication";

			// login as default and get account for current user
			string loginUser = randomNewUserName("TestAppUser_", 6);
			var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();
			ViewModels.Account currentAccount = await GetAccountForCurrentUser();

			// C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
				licenseType = "Cannabis Retail Store" //*Mandatory field **This is an entity** E.g.Cannabis Retail Store
                ,applicantType = ViewModels.Adoxio_applicanttypecodes.PrivateCorporation //*Mandatory (label=business type)
                ,registeredEstablishment = ViewModels.GeneralYesNo.No //*Mandatory (Yes=1, No=0)
                //,name = initialName
				//,applyingPerson = "Applying Person" //contact
                ,applicant = currentAccount //account
				//,jobNumber = "123"
				,establishmentName = "Not a Dispensary"
				,establishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1"
				,establishmentaddressstreet = "123 Any Street"
				,establishmentaddresscity = "Victoria, BC"
				,establishmentaddresspostalcode = "V1X 1X1"
				//,applicationStatus = "0"
			};

			var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
			ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            
			//Assert.Equal("Applying Person", responseViewModel.applyingPerson);
			Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
			Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
			Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);
            //return;
            // R - Read
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
			responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.True(responseViewModel.applicant != null);
            Assert.Equal(currentAccount.id, responseViewModel.applicant.id);


            // U - Update            
            viewmodel_application.establishmentName = changedName;
            viewmodel_application.id = id.ToString();

            request = new HttpRequestMessage(HttpMethod.Put, "/api/" + service + "/" + id)
            {
                Content = new StringContent(JsonConvert.SerializeObject(viewmodel_application), Encoding.UTF8, "application/json")
            };
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // verify that the update persisted.

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();

            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
			Assert.Equal(changedName, responseViewModel.establishmentName);

            // D - Delete

            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // second delete should return a 404.
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // logout and cleanup (deletes the account and contact created above ^^^)
			await LogoutAndCleanupTestUser(strId);
        }

		[Fact]
		public async System.Threading.Tasks.Task TestUserCannotAccessAnotherUsersApplication()
		{
			string initialName = randomNewUserName("Application Private ", 6);
            string service = "adoxioapplication";

            // login as default and get account for current user
            string loginUser1 = randomNewUserName("TestAppUser", 6);
            var strId1 = await LoginAndRegisterAsNewUser(loginUser1);

            ViewModels.User user1 = await GetCurrentUser();
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                name = initialName,
                applyingPerson = "Applying Person",
                applicant = currentAccount1,
                jobNumber = "123",
                licenseType = "Cannabis",
                establishmentName = "Private Retail Store",
                establishmentAddress = "666 Any Street, Victoria, BC, V1X 1X1",
                establishmentaddressstreet = "666 Any Street",
                establishmentaddresscity = "Victoria, BC",
                establishmentaddresspostalcode = "V1X 1X1",
                applicationStatus = "0"
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            // name should match.
			Assert.Equal("Private Retail Store", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

			// R - Read
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.applicant.id);

			await Logout();

            // register and login as a second user
            string loginUser2 = randomNewUserName("TestAppUser", 6);
            var strId2 = await LoginAndRegisterAsNewUser(loginUser2);

            ViewModels.User user2 = await GetCurrentUser();
            ViewModels.Account currentAccount2 = await GetAccountForCurrentUser();

			// R - Read (404, no access by user)
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

			// logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId2);

			// log back in as first user
			await Login(loginUser1);

			// R - Read - still has access to application
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.applicant.id);

			// D - Delete
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

			// logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId1);
		}

		[Fact]
		public async System.Threading.Tasks.Task TestUserCanAccessApplicationForTheirAccount()
		{
			string initialName = randomNewUserName("Application Shared ", 6);
            string service = "adoxioapplication";

            // login as default and get account for current user
            string loginUser1 = randomNewUserName("TestAppUser", 6);
			string loginAccount = randomNewUserName(loginUser1, 6);
			string loginUser2 = loginUser1 + "-2";
			loginUser1 = loginUser1 + "-1";
			var strId1 = await LoginAndRegisterAsNewUser(loginUser1, loginAccount);

            ViewModels.User user1 = await GetCurrentUser();
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                name = initialName,
                applyingPerson = "Applying Person",
                applicant = currentAccount1,
                jobNumber = "123",
                licenseType = "Cannabis",
                establishmentName = "Shared Retail Store",
                establishmentAddress = "666 Any Street, Victoria, BC, V1X 1X1",
                establishmentaddressstreet = "666 Any Street",
                establishmentaddresscity = "Victoria, BC",
                establishmentaddresspostalcode = "V1X 1X1",
				applicationStatus = "845280000"
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            // name should match.
			Assert.Equal("Shared Retail Store", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

            // R - Read
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.applicant.id);

            await Logout();

            // register and login as a second user
			var strId2 = await LoginAndRegisterAsNewUser(loginUser2, loginAccount);

            ViewModels.User user2 = await GetCurrentUser();
            ViewModels.Account currentAccount2 = await GetAccountForCurrentUser();
            // same account as user 1
			Assert.Equal(currentAccount2.id, currentAccount1.id);

            // R - Read (should be able to access by user)
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
			response.EnsureSuccessStatusCode();

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId2);

            // log back in as first user
            await Login(loginUser1);

            // R - Read - still has access to application
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.applicant.id);

			// D - Delete
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId1);
			//await Logout();
		}
    }
}
