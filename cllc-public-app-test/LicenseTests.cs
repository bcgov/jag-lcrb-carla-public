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
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.ViewModels;

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
            string service = "adoxioapplication";

            // login as default and get account for current user
            string loginUser1 = randomNewUserName("TestAppUser", 6);
            string loginAccount = randomNewUserName(loginUser1, 6);
            loginUser1 = loginUser1 + "-1";
            var strId1 = await LoginAndRegisterAsNewUser(loginUser1, loginAccount);

            ViewModels.User user1 = await GetCurrentUser();
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                name = initialName,
                applyingPerson = "Applying Person",
                applicant = currentAccount1,
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                jobNumber = "123",
                licenseType = "Cannabis Retail Store",
                establishmentName = "Shared Retail Store",
                establishmentAddress = "666 Any Street, Victoria, BC, V1X 1X1",
                establishmentaddressstreet = "666 Any Street",
                establishmentaddresscity = "Victoria, BC",
                establishmentaddresspostalcode = "V1X 1X1",
                applicationStatus = AdoxioApplicationStatusCodes.Approved
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            Assert.Equal("Shared Retail Store", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.applicant.id);

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
            string service = "adoxioapplication";

            // login as default and get account for current user
            string loginUser1 = randomNewUserName("TestAppUser", 6);
            string loginAccount = randomNewUserName(loginUser1, 6);
            loginUser1 = loginUser1 + "-1";
            var strId1 = await LoginAndRegisterAsNewUser(loginUser1, loginAccount);

            ViewModels.User user1 = await GetCurrentUser();
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                name = initialName,
                applyingPerson = "Applying Person",
                applicant = currentAccount1,
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                jobNumber = "123",
                licenseType = "Cannabis Retail Store",
                establishmentName = "Shared Retail Store",
                establishmentAddress = "666 Any Street, Victoria, BC, V1X 1X1",
                establishmentaddressstreet = "666 Any Street",
                establishmentaddresscity = "Victoria, BC",
                establishmentaddresspostalcode = "V1X 1X1",
                applicationStatus = AdoxioApplicationStatusCodes.Approved
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            Assert.Equal("Shared Retail Store", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.applicant.id);

            service = "adoxiolicense";
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service);
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            var responseViewModelList = JsonConvert.DeserializeObject<List<AdoxioLicense>>(jsonString);
            response.EnsureSuccessStatusCode();

            // TODO: License controller is not set up yet. Response passes, however, nothing is returned.

            await LogoutAndCleanupTestUser(strId1);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetDynamicsLicensesByIdTest()
        {
            string initialName = randomNewUserName("License Test ", 6);
            string service = "adoxioapplication";

            // login as default and get account for current user
            string loginUser1 = randomNewUserName("TestAppUser", 6);
            string loginAccount = randomNewUserName(loginUser1, 6);
            loginUser1 = loginUser1 + "-1";
            var strId1 = await LoginAndRegisterAsNewUser(loginUser1, loginAccount);

            ViewModels.User user1 = await GetCurrentUser();
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                name = initialName,
                applyingPerson = "Applying Person",
                applicant = currentAccount1,
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                jobNumber = "123",
                licenseType = "Cannabis Retail Store",
                establishmentName = "Shared Retail Store",
                establishmentAddress = "666 Any Street, Victoria, BC, V1X 1X1",
                establishmentaddressstreet = "666 Any Street",
                establishmentaddresscity = "Victoria, BC",
                establishmentaddresspostalcode = "V1X 1X1",
                applicationStatus = AdoxioApplicationStatusCodes.Approved
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            Assert.Equal("Shared Retail Store", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.applicant.id);

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