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
    public class LicenceEventsTests :  ApiIntegrationTestBaseWithLogin
    {
        public LicenceEventsTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory) 
        { }

        const string service = "LicenceEvents";
        
        [Fact]
        public async System.Threading.Tasks.Task TestNoAccessToAnonymousUser()
        {
            string id = "SomeRandomId";

            // first confirm we are not logged in
            await GetCurrentUserIsUnauthorized();

            // try a random GET, should return unauthorized
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            Assert.Equal(request.RequestUri.ToString(), "/api/LicenceEvents/" + id);

            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            string _discard = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task TestCRUD()
        {
            string venue = "Venue Description";
            string changedVenue = "New Venue Description";
            string city = "Victoria";
            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.LicenceEvent viewmodel_adoxio_event = new ViewModels.LicenceEvent()
            {
                City=city,
                VenueDescription = venue
            };

            string jsonString = JsonConvert.SerializeObject(viewmodel_adoxio_event);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.LicenceEvent responseViewModel = JsonConvert.DeserializeObject<ViewModels.LicenceEvent>(jsonString);

            Assert.Equal(venue, responseViewModel.VenueDescription);
            Guid id = new Guid(responseViewModel.Id);

            // R - Read

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.LicenceEvent>(jsonString);
            Assert.Equal(venue, responseViewModel.VenueDescription);

            // U - Update            
            ViewModels.LicenceEvent patchModel = new ViewModels.LicenceEvent()
            {
                Id = id.ToString(),
                City=city,
                VenueDescription = changedVenue
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

            responseViewModel = JsonConvert.DeserializeObject<ViewModels.LicenceEvent>(jsonString);
            Assert.Equal(responseViewModel.VenueDescription, changedVenue);

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
        public async System.Threading.Tasks.Task TestDates()
        {

            string venue = "Venue Description";
            string city = "Victoria";
            // January 1, 1970 - 00:00 with Pacific time.
            // note that this field expects no time.

            DateTimeOffset startDate = DateTimeOffset.Parse("01/01/1970 00:00 -8:00");
            DateTimeOffset endDate = DateTimeOffset.Parse("02/02/1970 00:00 -8:00");
            List<ViewModels.LicenceEventSchedule> schedule = new List<ViewModels.LicenceEventSchedule>();
            schedule.Add(new ViewModels.LicenceEventSchedule()
            {                
                EventStartDateTime = DateTimeOffset.Parse("01/01/1970 09:00 -8:00"),
                EventEndDateTime = DateTimeOffset.Parse("02/02/1970 17:00 -8:00"),
                ServiceStartDateTime = DateTimeOffset.Parse("01/01/1970 09:00 -8:00"),
                ServiceEndDateTime = DateTimeOffset.Parse("02/02/1970 17:00 -8:00")
            });

            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.LicenceEvent viewmodel_adoxio_event = new ViewModels.LicenceEvent()
            {
                VenueDescription = venue,
                City = city,
                StartDate = startDate,
                EndDate = endDate,
                Schedules = schedule
            };

            string jsonString = JsonConvert.SerializeObject(viewmodel_adoxio_event);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            viewmodel_adoxio_event = JsonConvert.DeserializeObject<ViewModels.LicenceEvent>(jsonString);

            Assert.Equal(((DateTimeOffset)viewmodel_adoxio_event.StartDate).Month, startDate.Month);
            Assert.Equal(((DateTimeOffset)viewmodel_adoxio_event.StartDate).Day, startDate.Day);
            Assert.Equal(((DateTimeOffset)viewmodel_adoxio_event.StartDate).Year, startDate.Year);
            Assert.Equal(((DateTimeOffset)viewmodel_adoxio_event.EndDate).Month, endDate.Month);
            Assert.Equal(((DateTimeOffset)viewmodel_adoxio_event.EndDate).Day, endDate.Day);
            Assert.Equal(((DateTimeOffset)viewmodel_adoxio_event.EndDate).Year, endDate.Year);
            


            // R -Read
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{service}/{viewmodel_adoxio_event.Id}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            jsonString = await response.Content.ReadAsStringAsync();
            viewmodel_adoxio_event = JsonConvert.DeserializeObject<ViewModels.LicenceEvent>(jsonString);
            Assert.NotNull(viewmodel_adoxio_event?.Id);
            
            Assert.Equal(startDate.Month, ((DateTimeOffset)viewmodel_adoxio_event.StartDate).Month);
            Assert.Equal(startDate.Year, ((DateTimeOffset)viewmodel_adoxio_event.StartDate).Year);
            Assert.Equal(startDate.Day, ((DateTimeOffset)viewmodel_adoxio_event.StartDate).Day);

            Assert.Equal(endDate.Month, ((DateTimeOffset)viewmodel_adoxio_event.EndDate).Month);
            Assert.Equal(endDate.Year, ((DateTimeOffset)viewmodel_adoxio_event.EndDate).Year);
            Assert.Equal(endDate.Day, ((DateTimeOffset)viewmodel_adoxio_event.EndDate).Day);

            // D - Delete

            request = new HttpRequestMessage(HttpMethod.Delete, $"/api/{service}/" + viewmodel_adoxio_event.Id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // second delete should return a 404.
            request = new HttpRequestMessage(HttpMethod.Delete, $"/api/{service}/" + viewmodel_adoxio_event.Id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);


            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{service}/" + viewmodel_adoxio_event.Id);
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            await Logout();
        }

        [Fact]
        public async System.Threading.Tasks.Task TestList()
        {
            string firstName = "firstName";
            string secondName = "secondName";
            string city = "Victoria";
            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);
            List<ViewModels.LicenceEvent> lists = new List<ViewModels.LicenceEvent>();

            // C - Create first
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.LicenceEvent viewmodel_adoxio_event = new ViewModels.LicenceEvent()
            {
                City=city,
                Name = firstName
            };

            string jsonString = JsonConvert.SerializeObject(viewmodel_adoxio_event);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode(); 
            string responseJsonString = await response.Content.ReadAsStringAsync();   

            viewmodel_adoxio_event = JsonConvert.DeserializeObject<ViewModels.LicenceEvent>(responseJsonString);
            //add loaded first event.
            lists.Add(viewmodel_adoxio_event);
            
            // Create second
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.LicenceEvent viewmodel_adoxio_event2 = new ViewModels.LicenceEvent()
            {
                City=city,
                Name = secondName
            };

            jsonString = JsonConvert.SerializeObject(viewmodel_adoxio_event2);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response2 = await _client.SendAsync(request);
            response2.EnsureSuccessStatusCode();
            responseJsonString = await response2.Content.ReadAsStringAsync();

            viewmodel_adoxio_event2 = JsonConvert.DeserializeObject<ViewModels.LicenceEvent>(responseJsonString);
            lists.Add(viewmodel_adoxio_event2);
            Assert.Equal(2, lists.Count);

            // List

            //var listRequest = new HttpRequestMessage(HttpMethod.Get, "/api/" + service);
            //var listResponse = await _client.SendAsync(listRequest);
            //listResponse.EnsureSuccessStatusCode();

            //jsonString = await response.Content.ReadAsStringAsync();

            //response.EnsureSuccessStatusCode();

            await LogoutAndCleanupTestUser(strId);

        }
    }
}
