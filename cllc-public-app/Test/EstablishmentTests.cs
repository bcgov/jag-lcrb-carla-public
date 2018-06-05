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
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class EstablishmentTests : ApiIntegrationTestBaseWithLogin
    {
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
            Assert.Equal(response.StatusCode, HttpStatusCode.Unauthorized);
            string _discard = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task TestCreate()
        {
            string initialName = "InitialName";
            await LoginAsDefault();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);
            var  establishment = new ViewModels.AdoxioEstablishment()
            {
                Name = initialName
            };

            string jsonString = JsonConvert.SerializeObject(establishment);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
        }

        [Fact]
        public async System.Threading.Tasks.Task TestCRUD()
        {
            string initialName = "InitialName";
            string changedName = "ChangedName";

            await LoginAsDefault();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            Adoxio_establishment adoxio_establishment = new Adoxio_establishment()
            {
                Adoxio_establishmentid = Guid.NewGuid(),
                Adoxio_name = initialName
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
            adoxio_establishment.Adoxio_name = changedName;

            request = new HttpRequestMessage(HttpMethod.Put, "/api/" + service + "/" + id)
            {
                Content = new StringContent(JsonConvert.SerializeObject(adoxio_establishment.ToViewModel()), Encoding.UTF8, "application/json")
            };
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // verify that the update persisted.

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();

            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioEstablishment>(jsonString);
            Assert.Equal(changedName, responseViewModel.Name);

            await Logout();

            return;
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

        }
    }
}
