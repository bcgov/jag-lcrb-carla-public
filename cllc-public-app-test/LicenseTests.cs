using Gov.Lclb.Cllb.Public.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class LicenseTests : ApiIntegrationTestBaseWithLogin
    {
        public LicenseTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory)
        { }

        [Fact]
        public async System.Threading.Tasks.Task GetCurrentUserDyanamicsApplicationsTest()
        {
            string initialName = randomNewUserName("License Test ", 6);
            string service = "Applications";

            // login as default and get account for current user
            string loginUser1 = randomNewUserName("TestAppUser", 6);
            string loginAccount = randomNewUserName(loginUser1, 6);
            loginUser1 = loginUser1 + "-1";
            var strId1 = await LoginAndRegisterAsNewUser(loginUser1, loginAccount);

            ViewModels.User user1 = await GetCurrentUser();
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.Application viewmodel_application = new ViewModels.Application()
            {
                Name = initialName,
                ApplyingPerson = "Applying Person",
                Applicant = currentAccount1,
                ApplicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                JobNumber = "123",
                LicenseType = "Cannabis Retail Store",
                EstablishmentName = "Shared Retail Store",
                EstablishmentAddress = "666 Any Street, Victoria, BC, V1X 1X1",
                EstablishmentAddressStreet = "666 Any Street",
                EstablishmentAddressCity = "Victoria, BC",
                EstablishmentAddressPostalCode = "V1X 1X1",
                ApplicationStatus = AdoxioApplicationStatusCodes.Approved
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.Application responseViewModel = JsonConvert.DeserializeObject<ViewModels.Application>(jsonString);

            Assert.Equal("Shared Retail Store", responseViewModel.EstablishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.EstablishmentAddressCity);
            Assert.Equal("V1X 1X1", responseViewModel.EstablishmentAddressPostalCode);

            Guid id = new Guid(responseViewModel.Id);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.Application>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.Applicant.id);

            service = "adoxiolicense";
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/current");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // TODO: License controller is not set up yet. Response passes, however, nothing is returned.

            await LogoutAndCleanupTestUser(strId1);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAllLicensesTest()
        {
            string initialName = randomNewUserName("License Test ", 6);
            string service = "Application";

            // login as default and get account for current user
            string loginUser1 = randomNewUserName("TestAppUser", 6);
            string loginAccount = randomNewUserName(loginUser1, 6);
            loginUser1 = loginUser1 + "-1";
            var strId1 = await LoginAndRegisterAsNewUser(loginUser1, loginAccount);

            ViewModels.User user1 = await GetCurrentUser();
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.Application viewmodel_application = new ViewModels.Application()
            {
                Name = initialName,
                ApplyingPerson = "Applying Person",
                Applicant = currentAccount1,
                ApplicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                JobNumber = "123",
                LicenseType = "Cannabis Retail Store",
                EstablishmentName = "Shared Retail Store",
                EstablishmentAddress = "666 Any Street, Victoria, BC, V1X 1X1",
                EstablishmentAddressStreet = "666 Any Street",
                EstablishmentAddressCity = "Victoria, BC",
                EstablishmentAddressPostalCode = "V1X 1X1",
                ApplicationStatus = AdoxioApplicationStatusCodes.Approved
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.Application responseViewModel = JsonConvert.DeserializeObject<ViewModels.Application>(jsonString);

            Assert.Equal("Shared Retail Store", responseViewModel.EstablishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.EstablishmentAddressCity);
            Assert.Equal("V1X 1X1", responseViewModel.EstablishmentAddressPostalCode);

            Guid id = new Guid(responseViewModel.Id);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.Application>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.Applicant.id);

            service = "adoxiolicense";
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service);
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            var responseViewModelList = JsonConvert.DeserializeObject<List<License>>(jsonString);
            response.EnsureSuccessStatusCode();

            // TODO: License controller is not set up yet. Response passes, however, nothing is returned.

            await LogoutAndCleanupTestUser(strId1);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetDynamicsLicensesByIdTest()
        {
            string initialName = randomNewUserName("License Test ", 6);
            string service = "Application";

            // login as default and get account for current user
            string loginUser1 = randomNewUserName("TestAppUser", 6);
            string loginAccount = randomNewUserName(loginUser1, 6);
            loginUser1 = loginUser1 + "-1";
            var strId1 = await LoginAndRegisterAsNewUser(loginUser1, loginAccount);

            ViewModels.User user1 = await GetCurrentUser();
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.Application viewmodel_application = new ViewModels.Application()
            {
                Name = initialName,
                ApplyingPerson = "Applying Person",
                Applicant = currentAccount1,
                ApplicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                JobNumber = "123",
                LicenseType = "Cannabis Retail Store",
                EstablishmentName = "Shared Retail Store",
                EstablishmentAddress = "666 Any Street, Victoria, BC, V1X 1X1",
                EstablishmentAddressStreet = "666 Any Street",
                EstablishmentAddressCity = "Victoria, BC",
                EstablishmentAddressPostalCode = "V1X 1X1",
                ApplicationStatus = AdoxioApplicationStatusCodes.Approved
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.Application responseViewModel = JsonConvert.DeserializeObject<ViewModels.Application>(jsonString);

            Assert.Equal("Shared Retail Store", responseViewModel.EstablishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.EstablishmentAddressCity);
            Assert.Equal("V1X 1X1", responseViewModel.EstablishmentAddressPostalCode);

            Guid id = new Guid(responseViewModel.Id);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.Application>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.Applicant.id);

            service = "adoxiolicense";
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/current");
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // TODO: License controller is not set up yet. Response passes, however, nothing is returned.

            await LogoutAndCleanupTestUser(strId1);
        }
    }
}