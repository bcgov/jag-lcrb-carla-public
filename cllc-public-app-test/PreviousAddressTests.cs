using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class PreviousAddressTests : ApiIntegrationTestBaseWithLogin
    {
        public PreviousAddressTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory)
        { }

        [Fact]
        public async System.Threading.Tasks.Task TestNoAccessToAnonymousUser()
        {
            string id = "SomeRandomId";

            // first confirm we are not logged in
            await GetCurrentUserIsUnauthorized();

            // try a random GET, should return unauthorized
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/PreviousAddress/by-contactid/" + id);
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            string _discard = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task TestCRUD()
        {

            string initialAddress = "645 Tyee Road";
            string changedAddress = "123 ChangedAddress Ave";
            string service = "contact";

            // register and login as our first user
            var loginUser1 = randomNewUserName("TestServiceCardUser", 6);
            await ServiceCardLogin(loginUser1, loginUser1);

            // C - Create

            //First create the contact
            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/{service}/worker");
            ViewModels.Contact contactVM = new ViewModels.Contact() {
                firstname  = "TestFirst",
                middlename = "TestMiddle",
                lastname = "TestLst"
            };

            string jsonString = JsonConvert.SerializeObject(contactVM);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
          
            contactVM = JsonConvert.DeserializeObject<ViewModels.Contact>(jsonString);

            // Get the worker
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/worker/{contactVM.id}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            jsonString = await response.Content.ReadAsStringAsync();
            var workerVM = JsonConvert.DeserializeObject<ViewModels.Worker>(jsonString);

            var addressVM = new ViewModels.PreviousAddress()
            {
                streetaddress = initialAddress,
                contactId = contactVM.id,
                workerId = workerVM.id
            };
            request = new HttpRequestMessage(HttpMethod.Post, $"/api/PreviousAddress");
            jsonString = JsonConvert.SerializeObject(addressVM);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            addressVM = JsonConvert.DeserializeObject<ViewModels.PreviousAddress>(jsonString);


            // R - Read

            request = new HttpRequestMessage(HttpMethod.Get, "/api/PreviousAddress/by-contactid/" + contactVM.id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            var address2 = (JsonConvert.DeserializeObject<List<ViewModels.PreviousAddress>>(jsonString)).FirstOrDefault(); ;
            Assert.Equal(address2.id, addressVM.id);


            // U - Update            
           address2.streetaddress = changedAddress;
            request = new HttpRequestMessage(HttpMethod.Put, "/api/PreviousAddress/" + address2.id)
            {
                Content = new StringContent(JsonConvert.SerializeObject(address2), Encoding.UTF8, "application/json")
            };
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // verify that the update persisted.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/PreviousAddress/by-contactid/" + contactVM.id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            var address3 = (JsonConvert.DeserializeObject<List<ViewModels.PreviousAddress>>(jsonString)).FirstOrDefault(); ;
            Assert.Equal(changedAddress, address3.streetaddress);

            // D - Delete

            request = new HttpRequestMessage(HttpMethod.Post, "/api/PreviousAddress/" + addressVM.id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // second delete should return a 404.
            request = new HttpRequestMessage(HttpMethod.Post, "/api/PreviousAddress/" + addressVM.id + "/delete");
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/PreviousAddress/by-contactid/" + contactVM.id);
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            var address4 = (JsonConvert.DeserializeObject<List<ViewModels.PreviousAddress>>(jsonString)).FirstOrDefault();
            Assert.Null(address4);
            await Logout();
        }
    }
}
