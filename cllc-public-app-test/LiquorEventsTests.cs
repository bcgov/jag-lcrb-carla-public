using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class LiquorEventsTests :  ApiIntegrationTestBaseWithLogin
    {
        public LiquorEventsTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory) 
        { }

        const string service = "liquorevents";
        
        [Fact]
        public async System.Threading.Tasks.Task TestNoAccessToAnonymousUser()
        {
            string id = "SomeRandomId";

            // first confirm we are not logged in
            await GetCurrentUserIsUnauthorized();

            // try a random GET, should return unauthorized
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            Assert.Equal(request.RequestUri.ToString(), "/api/liquorevents/" + id);

            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            string _discard = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task TestCRUD()
        {
            string initialName = "InitialName";
            string changedName = "ChangedName";

            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.LiquorEvent viewmodel_adoxio_event = new ViewModels.LiquorEvent()
            {
                Name = initialName
            };
            
            string jsonString = JsonConvert.SerializeObject(viewmodel_adoxio_event);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.LiquorEvent responseViewModel = JsonConvert.DeserializeObject<ViewModels.LiquorEvent>(jsonString);

            // name should match.
            Assert.Equal(initialName, responseViewModel.Name);
            Guid id = new Guid(responseViewModel.Id);

            // R - Read

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.LiquorEvent>(jsonString);
            Assert.Equal(initialName, responseViewModel.Name);

            // U - Update            
            ViewModels.LiquorEvent patchModel = new ViewModels.LiquorEvent()
            {
                Id = id.ToString(),
                Name = changedName
            };            

            request = new HttpRequestMessage(HttpMethod.Put, "/api/" + service + "/" + id)
            {
                Content = new StringContent(JsonConvert.SerializeObject(patchModel), Encoding.UTF8, "application/json")
            };
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            
            // verify that the update persisted.

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();

            responseViewModel = JsonConvert.DeserializeObject<ViewModels.LiquorEvent>(jsonString);
            Assert.Equal(changedName, responseViewModel.Name);

            // D - Delete

            request = new HttpRequestMessage(HttpMethod.Delete, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // second delete should return a 404.
            request = new HttpRequestMessage(HttpMethod.Delete, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            await LogoutAndCleanupTestUser(strId);

        }

        [Fact]
        public async System.Threading.Tasks.Task TestList()
        {
            string firstName = "firstName";
            string secondName = "secondName";

            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            // C - Create first
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.LiquorEvent viewmodel_adoxio_event = new ViewModels.LiquorEvent()
            {
                Name = firstName
            };

            string jsonString = JsonConvert.SerializeObject(viewmodel_adoxio_event);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Create second
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.LiquorEvent viewmodel_adoxio_event2 = new ViewModels.LiquorEvent()
            {
                Name = secondName
            };

            jsonString = JsonConvert.SerializeObject(viewmodel_adoxio_event2);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response2 = await _client.SendAsync(request);
            response2.EnsureSuccessStatusCode();

            // List

            var listRequest = new HttpRequestMessage(HttpMethod.Get, "/api/" + service);
            var listResponse = await _client.SendAsync(listRequest);
            listResponse.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            await LogoutAndCleanupTestUser(strId);

        }
    }
}
