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
			string loginUser = randomNewUserName("TestAppUser", 6);
			var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();
			ViewModels.Account currentAccount = await GetAccountForCurrentUser();

			// C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

			ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
			{
				name = initialName,
				applyingPerson = "Applying Person",
                applicant = currentAccount,
				jobNumber = "123",
				licenseType = "Cannabis",
				establishmentName = "Not a Dispensary",
				establishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1",
				establishmentaddressstreet = "123 Any Street",
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
            Assert.Equal(initialName + " - Not a Dispensary", responseViewModel.name);
			//Assert.Equal("Applying Person", responseViewModel.applyingPerson);
			Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
			Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
			Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

            // R - Read
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
			responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
			Assert.Equal(initialName + " - Not a Dispensary", responseViewModel.name);
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
    }
}
