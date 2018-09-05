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
    public class WorkerTests : ApiIntegrationTestBaseWithLogin
    {
        public WorkerTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory)
        { }

        [Fact]
        public async System.Threading.Tasks.Task TestNoAccessToAnonymousUser()
        {
            string id = "SomeRandomId";

            // first confirm we are not logged in
            await GetCurrentUserIsUnauthorized();

            // try a random GET, should return unauthorized
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/worker/by-contactid/" + id);
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            string _discard = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task TestCRUD()
        {

            string initialName = "InitialName";
            string changedName = "ChangedName";
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

            var aliasVM = new ViewModels.Alias()
            {
                firstname = "TestFirst",
                middlename = "TestMiddle",
                lastname = "TestLst",
                contact = new ViewModels.Contact()
                {
                    id = contactVM.id
                },
                worker = new ViewModels.Worker()
                {
                    id = workerVM.id
                }
            };
            request = new HttpRequestMessage(HttpMethod.Post, $"/api/alias");
            jsonString = JsonConvert.SerializeObject(aliasVM);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            aliasVM = JsonConvert.DeserializeObject<ViewModels.Alias>(jsonString);


            // R - Read

            request = new HttpRequestMessage(HttpMethod.Get, "/api/alias/by-contactid/" + contactVM.id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            var alias2 = (JsonConvert.DeserializeObject<List<ViewModels.Alias>>(jsonString)).FirstOrDefault(); ;
            Assert.Equal(alias2.id, aliasVM.id);


            // U - Update            
           alias2.firstname = changedName;
            request = new HttpRequestMessage(HttpMethod.Put, "/api/alias/" + alias2.id)
            {
                Content = new StringContent(JsonConvert.SerializeObject(alias2), Encoding.UTF8, "application/json")
            };
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // verify that the update persisted.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/alias/by-contactid/" + contactVM.id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            var alias3 = (JsonConvert.DeserializeObject<List<ViewModels.Alias>>(jsonString)).FirstOrDefault(); ;
            Assert.Equal(alias3.firstname, changedName);

            // D - Delete

            request = new HttpRequestMessage(HttpMethod.Post, "/api/alias/" + aliasVM.id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // second delete should return a 404.
            request = new HttpRequestMessage(HttpMethod.Post, "/api/alias/" + aliasVM.id + "/delete");
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/alias/by-contactid/" + contactVM.id);
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            var alias4 = (JsonConvert.DeserializeObject<List<ViewModels.Alias>>(jsonString)).FirstOrDefault();
            Assert.Null(alias4);
            await Logout();
        }
    }
}
