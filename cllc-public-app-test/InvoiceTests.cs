using Gov.Lclb.Cllb.Public.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class InvoiceTests : ApiIntegrationTestBaseWithLogin
    {
        public InvoiceTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory)
        { }

        [Fact]
        public async System.Threading.Tasks.Task TestNoAccessToAnonymousUser()
        {
            string service = "invoice";
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
            string initialName = "InitialName";
            string changedName = "ChangedName";
			string service = "invoice";

			// first confirm we are not logged in
            await GetCurrentUserIsUnauthorized();

			// register and login as our first user
            var loginUser1 = randomNewUserName("TestInvoiceUser", 6);
			var strId = await LoginAndRegisterAsNewUser(loginUser1);

			// C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);
			ViewModels.Invoice viewmodel_invoice = new ViewModels.Invoice()
			{
				name = initialName,
                invoicenumber = "12345",
                statecode = (int?)Adoxio_invoicestates.New,
                statuscode = (int?)Adoxio_invoicestatuses.New,
                totalamount = 7500.00
			};

			string jsonString = JsonConvert.SerializeObject(viewmodel_invoice);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // parse as JSON.            
            ViewModels.Invoice responseViewModel = JsonConvert.DeserializeObject<ViewModels.Invoice>(jsonString);

            // name should match.
            Assert.Equal(initialName, responseViewModel.name);
            Guid id = new Guid(responseViewModel.id);
			//String strid = responseViewModel.externalId;
			//Assert.Equal(strid, viewmodel_account.externalId);

            // R - Read

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.Invoice>(jsonString);
            Assert.Equal(initialName, responseViewModel.name);

			viewmodel_invoice.id = id.ToString();

            // U - Update            
			viewmodel_invoice.name = changedName;

            request = new HttpRequestMessage(HttpMethod.Put, "/api/" + service + "/" + id)
            {
				Content = new StringContent(JsonConvert.SerializeObject(viewmodel_invoice), Encoding.UTF8, "application/json")
            };
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // verify that the update persisted.

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();

            responseViewModel = JsonConvert.DeserializeObject<ViewModels.Invoice>(jsonString);
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

			await LogoutAndCleanupTestUser(strId);
        }
    }
}
