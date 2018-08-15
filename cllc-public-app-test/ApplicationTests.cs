using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class ApplicationTests : ApiIntegrationTestBaseWithLogin
    {
        public ApplicationTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory)
        { }

        [Fact]
        public async System.Threading.Tasks.Task TestNoAccessToAnonymousUser()
        {
            string service = "adoxioapplication";
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
            string initialName = randomNewUserName("Application Initial Name ", 6);
            string changedName = randomNewUserName("Application Changed Name ", 6);
            string service = "adoxioapplication";

            // login as default and get account for current user
            string loginUser = randomNewUserName("TestAppUser_", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();
            ViewModels.Account currentAccount = await GetAccountForCurrentUser();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                licenseType = "Cannabis Retail Store" //*Mandatory field **This is an entity** E.g.Cannabis Retail Store
                ,
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation //*Mandatory (label=business type)
                ,
                registeredEstablishment = ViewModels.GeneralYesNo.No //*Mandatory (Yes=1, No=0)
                                                                     //,name = initialName
                                                                     //,applyingPerson = "Applying Person" //contact
                ,
                applicant = currentAccount //account
                                           //,jobNumber = "123"
                ,
                establishmentName = "Not a Dispensary"
                ,
                establishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1"
                ,
                establishmentaddressstreet = "123 Any Street"
                ,
                establishmentaddresscity = "Victoria, BC"
                ,
                establishmentaddresspostalcode = "V1X 1X1"
                //,applicationStatus = "0"
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            //Assert.Equal("Applying Person", responseViewModel.applyingPerson);
            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);
            //return;
            // R - Read
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.True(responseViewModel.applicant != null);
            Assert.Equal(currentAccount.id, responseViewModel.applicant.id);


            // U - Update            
            viewmodel_application.establishmentName = changedName;
            viewmodel_application.id = id.ToString();

            request = new HttpRequestMessage(HttpMethod.Put, "/api/" + service + "/" + id)
            {
                Content = new StringContent(JsonConvert.SerializeObject(viewmodel_application), Encoding.UTF8, "application/json")
            };
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // verify that the update persisted.

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();

            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(changedName, responseViewModel.establishmentName);

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

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId);
        }

        // To disable this test run:
        // dotnet test --filter Category!=StressTests
        [Trait("Category", "StressTests")]
        [Fact]
        public async System.Threading.Tasks.Task TestCRUD100Times()
        {
            int i = 100;
            while (i > 0)
            {
                string initialName = randomNewUserName("Application Initial Name ", 6);
                string changedName = randomNewUserName("Application Changed Name ", 6);
                string service = "adoxioapplication";

                // login as default and get account for current user
                string loginUser = randomNewUserName("TestAppUser_", 6);
                var strId = await LoginAndRegisterAsNewUser(loginUser);

                ViewModels.User user = await GetCurrentUser();
                ViewModels.Account currentAccount = await GetAccountForCurrentUser();

                // C - Create
                var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

                ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
                {
                    licenseType = "Cannabis Retail Store" //*Mandatory field **This is an entity** E.g.Cannabis Retail Store
                    ,
                    applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation //*Mandatory (label=business type)
                    ,
                    registeredEstablishment = ViewModels.GeneralYesNo.No //*Mandatory (Yes=1, No=0)
                                                                         //,name = initialName
                                                                         //,applyingPerson = "Applying Person" //contact
                    ,
                    applicant = currentAccount //account
                                               //,jobNumber = "123"
                    ,
                    establishmentName = "Not a Dispensary"
                    ,
                    establishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1"
                    ,
                    establishmentaddressstreet = "123 Any Street"
                    ,
                    establishmentaddresscity = "Victoria, BC"
                    ,
                    establishmentaddresspostalcode = "V1X 1X1"
                    //,applicationStatus = "0"
                };

                var jsonString = JsonConvert.SerializeObject(viewmodel_application);
                request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // parse as JSON.
                jsonString = await response.Content.ReadAsStringAsync();
                ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

                //Assert.Equal("Applying Person", responseViewModel.applyingPerson);
                Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
                Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
                Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

                Guid id = new Guid(responseViewModel.id);
                //return;
                // R - Read
                request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
                response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                jsonString = await response.Content.ReadAsStringAsync();
                responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
                Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
                Assert.True(responseViewModel.applicant != null);
                Assert.Equal(currentAccount.id, responseViewModel.applicant.id);


                // U - Update            
                viewmodel_application.establishmentName = changedName;
                viewmodel_application.id = id.ToString();

                request = new HttpRequestMessage(HttpMethod.Put, "/api/" + service + "/" + id)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(viewmodel_application), Encoding.UTF8, "application/json")
                };
                response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // verify that the update persisted.

                request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
                response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                jsonString = await response.Content.ReadAsStringAsync();

                responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
                Assert.Equal(changedName, responseViewModel.establishmentName);

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

                i--;
                // logout and cleanup (deletes the account and contact created above ^^^)
                await LogoutAndCleanupTestUser(strId);
            }
        }

        [Fact]
        public async System.Threading.Tasks.Task TestFileListing()
        {
            string initialName = randomNewUserName("First InitialName", 6);
            string changedName = randomNewUserName("First ChangedName", 6);
            string service = "adoxioapplication";

            // Login as default user

            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();
            ViewModels.Account currentAccount = await GetAccountForCurrentUser();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                licenseType = "Cannabis Retail Store" //*Mandatory field **This is an entity** E.g.Cannabis Retail Store
                ,
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation //*Mandatory (label=business type)
                ,
                registeredEstablishment = ViewModels.GeneralYesNo.No //*Mandatory (Yes=1, No=0)
                                                                     //,name = initialName
                                                                     //,applyingPerson = "Applying Person" //contact
                ,
                applicant = currentAccount //account
                                           //,jobNumber = "123"
                ,
                establishmentName = "Not a Dispensary"
                ,
                establishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1"
                ,
                establishmentaddressstreet = "123 Any Street"
                ,
                establishmentaddresscity = "Victoria, BC"
                ,
                establishmentaddresspostalcode = "V1X 1X1"
                //,applicationStatus = "0"
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // parse as JSON.

            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            //Assert.Equal("Applying Person", responseViewModel.applyingPerson);
            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

            // Attach a file

            string testData = "This is just a test.";
            byte[] bytes = Encoding.ASCII.GetBytes(testData);
            string documentType = "Test Document Type";
            // Create random filename
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[9];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var randomString = new String(stringChars);
            string filename = randomString + ".txt";

            MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----TestBoundary");
            var fileContent = new MultipartContent { new ByteArrayContent(bytes) };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            fileContent.Headers.ContentDisposition.Name = "File";
            fileContent.Headers.ContentDisposition.FileName = filename;
            multiPartContent.Add(fileContent);
            multiPartContent.Add(new StringContent(documentType), "documentType");   // form input

            string accountId = user.accountid;

            // create a new request object for the upload, as we will be using multipart form submission.
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/attachments");
            requestMessage.Content = multiPartContent;

            var uploadResponse = await _client.SendAsync(requestMessage);
            uploadResponse.EnsureSuccessStatusCode();

            // Cleanup
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            await LogoutAndCleanupTestUser(strId);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestFileUpload()
        {
            // First create a Legal Entity

            string initialName = randomNewUserName("LETest InitialName", 6);
            string changedName = randomNewUserName("LETest ChangedName", 6);
            string service = "adoxioapplication";

            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();
            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                name = initialName,
                applyingPerson = "Applying Person",
                applicant = currentAccount1,
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation //*Mandatory (label=business type)
                ,
                jobNumber = "123",
                licenseType = "Cannabis Retail Store",
                establishmentName = "Private Retail Store",
                establishmentAddress = "666 Any Street, Victoria, BC, V1X 1X1",
                establishmentaddressstreet = "666 Any Street",
                establishmentaddresscity = "Victoria, BC",
                establishmentaddresspostalcode = "V1X 1X1",
                applicationStatus = AdoxioApplicationStatusCodes.Active
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            string id = responseViewModel.id;

            // Attach a file

            string testData = "This is just a test.";
            byte[] bytes = Encoding.ASCII.GetBytes(testData);
            string documentType = "Test Document Type";

            // Create random filename
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[9];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var randomString = new String(stringChars);
            string filename = randomString + ".txt";

            MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----TestBoundary");
            var fileContent = new MultipartContent { new ByteArrayContent(bytes) };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            fileContent.Headers.ContentDisposition.Name = "File";
            fileContent.Headers.ContentDisposition.FileName = filename;
            multiPartContent.Add(fileContent);
            multiPartContent.Add(new StringContent(documentType), "documentType");   // form input

            string applicationId = responseViewModel.id;

            // create a new request object for the upload, as we will be using multipart form submission.
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + applicationId + "/attachments");
            requestMessage.Content = multiPartContent;

            var uploadResponse = await _client.SendAsync(requestMessage);
            uploadResponse.EnsureSuccessStatusCode();

            // Verify that the file Meta Data matches



            // Verify that the file can be downloaded and the contents match            

            // Cleanup the Application



            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            await LogoutAndCleanupTestUser(strId);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestUserCannotAccessAnotherUsersApplication()
        {
            string initialName = randomNewUserName("Application Private ", 6);
            string service = "adoxioapplication";

            // login as default and get account for current user
            string loginUser1 = randomNewUserName("TestAppUser", 6);
            var strId1 = await LoginAndRegisterAsNewUser(loginUser1);

            ViewModels.User user1 = await GetCurrentUser();
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                name = initialName,
                applyingPerson = "Applying Person",
                applicant = currentAccount1,
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation //*Mandatory (label=business type)
                ,
                jobNumber = "123",
                licenseType = "Cannabis Retail Store",
                establishmentName = "Private Retail Store",
                establishmentAddress = "666 Any Street, Victoria, BC, V1X 1X1",
                establishmentaddressstreet = "666 Any Street",
                establishmentaddresscity = "Victoria, BC",
                establishmentaddresspostalcode = "V1X 1X1",
                applicationStatus = AdoxioApplicationStatusCodes.Active
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            // name should match.
            Assert.Equal("Private Retail Store", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

            // R - Read
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.applicant.id);

            await Logout();

            // register and login as a second user
            string loginUser2 = randomNewUserName("TestAppUser", 6);
            var strId2 = await LoginAndRegisterAsNewUser(loginUser2);

            ViewModels.User user2 = await GetCurrentUser();
            ViewModels.Account currentAccount2 = await GetAccountForCurrentUser();

            // R - Read (404, no access by user)
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId2);

            // log back in as first user
            await Login(loginUser1);

            // R - Read - still has access to application
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.applicant.id);

            // D - Delete
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId1);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestUserCanAccessApplicationForTheirAccount()
        {
            string initialName = randomNewUserName("Application Shared ", 6);
            string service = "adoxioapplication";

            // login as default and get account for current user
            string loginUser1 = randomNewUserName("TestAppUser", 6);
            string loginAccount = randomNewUserName(loginUser1, 6);
            string loginUser2 = loginUser1 + "-2";
            loginUser1 = loginUser1 + "-1";
            var strId1 = await LoginAndRegisterAsNewUser(loginUser1, loginAccount);

            ViewModels.User user1 = await GetCurrentUser();
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                name = initialName,
                applyingPerson = "Applying Person",
                applicant = currentAccount1,
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation //*Mandatory (label=business type)
                ,
                jobNumber = "123",
                licenseType = "Cannabis Retail Store",
                establishmentName = "Shared Retail Store",
                establishmentAddress = "666 Any Street, Victoria, BC, V1X 1X1",
                establishmentaddressstreet = "666 Any Street",
                establishmentaddresscity = "Victoria, BC",
                establishmentaddresspostalcode = "V1X 1X1",
                applicationStatus = AdoxioApplicationStatusCodes.InProgress
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            // name should match.
            Assert.Equal("Shared Retail Store", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

            // R - Read
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.applicant.id);

            await Logout();

            // register and login as a second user
            var strId2 = await LoginAndRegisterAsNewUser(loginUser2, loginAccount);

            ViewModels.User user2 = await GetCurrentUser();
            ViewModels.Account currentAccount2 = await GetAccountForCurrentUser();
            // same account as user 1
            Assert.Equal(currentAccount2.id, currentAccount1.id);

            // R - Read (should be able to access by user)
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId2);

            // log back in as first user
            await Login(loginUser1);

            // R - Read - still has access to application
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal(currentAccount1.id, responseViewModel.applicant.id);

            // D - Delete
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId1);
            //await Logout();
        }

        [Fact]
        public async System.Threading.Tasks.Task TestUploadFile()
        {
            // Create application
            string initialName = randomNewUserName("Application Initial Name ", 6);
            string changedName = randomNewUserName("Application Changed Name ", 6);
            string service = "adoxioapplication";

            // login as default and get account for current user
            string loginUser = randomNewUserName("TestAppUser_", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();
            ViewModels.Account currentAccount = await GetAccountForCurrentUser();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                licenseType = "Cannabis Retail Store"
                ,
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation
                ,
                registeredEstablishment = ViewModels.GeneralYesNo.No
                ,
                applicant = currentAccount
                ,
                establishmentName = "Not a Dispensary"
                ,
                establishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1"
                ,
                establishmentaddressstreet = "123 Any Street"
                ,
                establishmentaddresscity = "Victoria, BC"
                ,
                establishmentaddresspostalcode = "V1X 1X1"
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.True(responseViewModel.applicant != null);
            Assert.Equal(currentAccount.id, responseViewModel.applicant.id);

            // Test upload, get, delete attachment
            string documentType = "Licence Application Main";


            using (var formData = new MultipartFormDataContent())
            {
                // Upload
                var fileContent = new ByteArrayContent(new byte[100]);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = "test.pdf"
                };
                formData.Add(fileContent);
                formData.Add(new StringContent(documentType, Encoding.UTF8, "application/json"), "documentType");
                response = _client.PostAsync($"/api/{service}/{id}/attachments", formData).Result;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // Get
                request = new HttpRequestMessage(HttpMethod.Get, $"/api/{service}/{id}/attachments/{documentType}");
                response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                jsonString = await response.Content.ReadAsStringAsync();
                var files = JsonConvert.DeserializeObject<List<FileSystemItem>>(jsonString);
                files.ForEach(async file =>
                {
                    // Delete
                    request = new HttpRequestMessage(HttpMethod.Delete, $"/api/{service}/{id}/attachments?serverRelativeUrl={Uri.EscapeDataString(file.serverrelativeurl)}");
                    response = await _client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                });
            }

            await LogoutAndCleanupTestUser(strId);
        }

        // To disable this test run:
        // dotnet test --filter Category!=StressTests
        [Trait("Category", "StressTests")]
        [Fact]
        public async System.Threading.Tasks.Task Test100UploadFile()
        {
            // Create application
            string initialName = randomNewUserName("Application Initial Name ", 6);
            string changedName = randomNewUserName("Application Changed Name ", 6);
            string service = "adoxioapplication";

            // login as default and get account for current user
            string loginUser = randomNewUserName("TestAppUser_", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();
            ViewModels.Account currentAccount = await GetAccountForCurrentUser();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                licenseType = "Cannabis Retail Store"
                ,
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation
                ,
                registeredEstablishment = ViewModels.GeneralYesNo.No
                ,
                applicant = currentAccount
                ,
                establishmentName = "Not a Dispensary"
                ,
                establishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1"
                ,
                establishmentaddressstreet = "123 Any Street"
                ,
                establishmentaddresscity = "Victoria, BC"
                ,
                establishmentaddresspostalcode = "V1X 1X1"
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.True(responseViewModel.applicant != null);
            Assert.Equal(currentAccount.id, responseViewModel.applicant.id);

            // Test upload, get, delete attachment
            string documentType = "Licence Application Main";


            // Upload
            const int fileLimit = 100;
            var i = fileLimit;
            while (i > 0)
            {
                using (var formData = new MultipartFormDataContent())
                {
                    var randomNum = new Random().Next(1000) + 100;
                    var fileContent = new ByteArrayContent(Encoding.ASCII.GetBytes(randomNewUserName("test data", randomNum)));
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file",
                        FileName = $"test-{Guid.NewGuid().ToString()}.pdf"
                    };
                    formData.Add(fileContent);
                    formData.Add(new StringContent(documentType, Encoding.UTF8, "application/json"), "documentType");
                    response = _client.PostAsync($"/api/{service}/{id}/attachments", formData).Result;
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    i--;
                }
            }

            // Get
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{service}/{id}/attachments/{documentType}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            var files = JsonConvert.DeserializeObject<List<FileSystemItem>>(jsonString);
            var numberOfFiles = files.Count;
            files.ForEach(async file =>
            {
                // Delete
                request = new HttpRequestMessage(HttpMethod.Delete, $"/api/{service}/{id}/attachments?serverRelativeUrl={Uri.EscapeDataString(file.serverrelativeurl)}");
                response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            });
            Assert.Equal(fileLimit, numberOfFiles);

            await LogoutAndCleanupTestUser(strId);
        }

        [Fact]
        public async System.Threading.Tasks.Task Test25MbUploadFile()
        {
            // Create application
            string initialName = randomNewUserName("Application Initial Name ", 6);
            string changedName = randomNewUserName("Application Changed Name ", 6);
            string service = "adoxioapplication";

            // login as default and get account for current user
            string loginUser = randomNewUserName("TestAppUser_", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();
            ViewModels.Account currentAccount = await GetAccountForCurrentUser();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                licenseType = "Cannabis Retail Store"
                ,
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation
                ,
                registeredEstablishment = ViewModels.GeneralYesNo.No
                ,
                applicant = currentAccount
                ,
                establishmentName = "Not a Dispensary"
                ,
                establishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1"
                ,
                establishmentaddressstreet = "123 Any Street"
                ,
                establishmentaddresscity = "Victoria, BC"
                ,
                establishmentaddresspostalcode = "V1X 1X1"
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid id = new Guid(responseViewModel.id);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.True(responseViewModel.applicant != null);
            Assert.Equal(currentAccount.id, responseViewModel.applicant.id);

            // Test upload, get, delete attachment
            string documentType = "Licence Application Main";


            using (var formData = new MultipartFormDataContent())
            {
                // Upload
                var fileContent = new ByteArrayContent(new byte[25 * 1024 * 1024]);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = "test.pdf"
                };
                formData.Add(fileContent);
                formData.Add(new StringContent(documentType, Encoding.UTF8, "application/json"), "documentType");
                response = _client.PostAsync($"/api/{service}/{id}/attachments", formData).Result;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            // Get
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{service}/{id}/attachments/{documentType}");
            response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            var files = JsonConvert.DeserializeObject<List<FileSystemItem>>(jsonString);
            files.ForEach(async file =>
            {
                // Delete
                request = new HttpRequestMessage(HttpMethod.Delete, $"/api/{service}/{id}/attachments?serverRelativeUrl={Uri.EscapeDataString(file.serverrelativeurl)}");
                response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            });

            await LogoutAndCleanupTestUser(strId);
        }
    }
}
