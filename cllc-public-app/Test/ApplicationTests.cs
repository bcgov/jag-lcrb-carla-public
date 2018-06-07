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
            Assert.Equal(response.StatusCode, HttpStatusCode.Unauthorized);
            string _discard = await response.Content.ReadAsStringAsync();
        }
        
        [Fact]
        public async System.Threading.Tasks.Task TestCRUD()
        {
            string initialName = "InitialName";
            string changedName = "ChangedName";
			string service = "adoxioapplication";

            // TODO once the unit tests are fixed (session management) change this test to use a newly registered user
			// login as default and get account for current user
			//string loginName = randomNewUserName("AppUser", 6);
			//var strId = await LoginAndRegisterAsNewUser(loginName);
			await LoginAsDefault();

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
				applicationStatus = "Active"
			};

			var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
			ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            // name should match.
            Assert.Equal(initialName, responseViewModel.name);
            // TODO figure out where data is getting stored and do the validations
			//Assert.Equal("Applying Person", responseViewModel.applyingPerson);
			//Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
			//Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
			//Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);
            
            Guid id = new Guid(responseViewModel.id);

            /* TODO the following code assumes we fetch a single application by id, but the service takes an applicant id
            // R - Read
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
			responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(initialName, responseViewModel.name);
			Assert.Equal(currentAccount.id, responseViewModel.applicant.id);
            */
            /*
            // U - Update            
            account.Name = changedName;

            request = new HttpRequestMessage(HttpMethod.Put, "/api/" + service + "/" + id)
            {
                Content = new StringContent(JsonConvert.SerializeObject(account.ToViewModel()), Encoding.UTF8, "application/json")
            };
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // verify that the update persisted.

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();

            responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);
            Assert.Equal(changedName, responseViewModel.name);

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

			await Logout();
			*/

            // TODO include this once it works with a newly registered user
			// logout and cleanup (deletes the account and contact created above ^^^)
            //await LogoutAndCleanupTestUser(strId);
        }
        /*
        [Fact]
        public async System.Threading.Tasks.Task TestDirectorsAndOfficers()
        {
            string initialName = "InitialName";
            string changedName = "ChangedName";
            string accountService = "account";
            string legalEntityService = "adoxiolegalentity";

			await LoginAsDefault();

			// Create an account.
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService);

            Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM.Account account = new Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM.Account()
            {
                Accountid = Guid.NewGuid(),
                Name = initialName
            };

            ViewModels.Account viewmodel_account = account.ToViewModel();

            string jsonString = JsonConvert.SerializeObject(viewmodel_account);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.Account responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);

            // name should match.
            Assert.Equal(initialName, responseViewModel.name);
            Guid id = new Guid(responseViewModel.id);

            // Add a Director.


            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService);

            Adoxio_legalentity adoxio_legalentity = new Adoxio_legalentity()
            {
                Adoxio_legalentityid = Guid.NewGuid(),
                Adoxio_name = initialName,
                Adoxio_Account = account,
                Adoxio_position = (int?)ViewModels.PositionOptions.Director,
                Adoxio_legalentitytype = (int?)ViewModels.Adoxio_applicanttypecodes.PrivateCorporation
            };

            ViewModels.AdoxioLegalEntity viewmodel_adoxio_legalentity = adoxio_legalentity.ToViewModel();

            jsonString = JsonConvert.SerializeObject(viewmodel_adoxio_legalentity);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioLegalEntity responseLegalEntityViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);

            // name should match.
            Assert.Equal(initialName, responseLegalEntityViewModel.name);
            Guid directorId = new Guid(responseLegalEntityViewModel.id);


            // fetch the directors and officers.

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + accountService + "/" + id + "/directorsandofficers");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            

            // D - Delete

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

			await Logout();
        }
        */
    }
}
