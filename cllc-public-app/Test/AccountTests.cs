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

namespace Gov.Lclb.Cllb.Public.Test
{
	public class AccountTests : ApiIntegrationTestBaseWithLogin
    {
		[Fact]
        public async System.Threading.Tasks.Task TestNoAccessToAnonymousUser()
        {
            string service = "account";
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
            string service = "account";

			await LoginAsDefault();

			// C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

			Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM.Account account = new Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM.Account()
			{				
				Name = initialName,
				Adoxio_externalid = Guid.NewGuid().ToString()
            };

            ViewModels.Account viewmodel_account = account.ToViewModel();

            string jsonString = JsonConvert.SerializeObject(viewmodel_account);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            string createText = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.Account responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);

            // name should match.
            Assert.Equal(initialName, responseViewModel.name);
            //Guid id = new Guid(responseViewModel.id);
            Guid id = new Guid(responseViewModel.id);
			String strid = responseViewModel.externalId;
			Assert.Equal(strid, viewmodel_account.externalId);

            // R - Read

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);
            Assert.Equal(initialName, responseViewModel.name);

            account.Accountid = id;

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
            string responseText = await response.Content.ReadAsStringAsync();
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
        }

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
                //Accountid = Guid.NewGuid(),
                Name = initialName,
				Adoxio_externalid = Guid.NewGuid().ToString()
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
            //Guid id = new Guid(responseViewModel.id);
			var strid = responseViewModel.externalId;
            string id = responseViewModel.id;

			// R - Read

			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + accountService + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);
			

            // Add a Director.


            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + accountService);

            Adoxio_legalentity adoxio_legalentity = new Adoxio_legalentity()
            {
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
    }
}
