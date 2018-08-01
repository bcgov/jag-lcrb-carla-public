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
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Microsoft.Extensions.DependencyInjection;
using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class EstablishmentTests :  ApiIntegrationTestBaseWithLogin
    {
        public EstablishmentTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory) 
        { }

        const string service = "adoxioestablishment";
        [Fact]
        public async System.Threading.Tasks.Task TestNoAccessToAnonymousUser()
        {
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

            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            MicrosoftDynamicsCRMadoxioEstablishment adoxio_establishment = new MicrosoftDynamicsCRMadoxioEstablishment()
            {
                AdoxioEstablishmentid = Guid.NewGuid().ToString(),
                AdoxioName = initialName
            };


            ViewModels.AdoxioEstablishment viewmodel_adoxio_establishment = adoxio_establishment.ToViewModel();

            string jsonString = JsonConvert.SerializeObject(viewmodel_adoxio_establishment);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioEstablishment responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioEstablishment>(jsonString);

            // name should match.
            Assert.Equal(initialName, responseViewModel.Name);
            Guid id = new Guid(responseViewModel.id);

            // R - Read

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioEstablishment>(jsonString);
            Assert.Equal(initialName, responseViewModel.Name);

            // U - Update            
            adoxio_establishment.AdoxioName = changedName;
            adoxio_establishment.AdoxioEstablishmentid = id.ToString();

            request = new HttpRequestMessage(HttpMethod.Put, "/api/" + service + "/" + id)
            {
                Content = new StringContent(JsonConvert.SerializeObject(adoxio_establishment.ToViewModel()), Encoding.UTF8, "application/json")
            };
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            
            // verify that the update persisted.

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();

            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioEstablishment>(jsonString);
            Assert.Equal(changedName, responseViewModel.Name);

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

            await LogoutAndCleanupTestUser(strId);

        }
    }
}
