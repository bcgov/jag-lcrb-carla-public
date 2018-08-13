using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class LegalEntityTests : ApiIntegrationTestBaseWithLogin
    {
        private const string service = "adoxiolegalentity";
        public LegalEntityTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory)
        { }

        [Fact]
        public async System.Threading.Tasks.Task TestNoAccessToAnonymousUser()
        {
            string service = "adoxiolegalentity";
            string id = "SomeRandomId";

            // first confirm we are not logged in
            await GetCurrentUserIsUnauthorized();

            // try a random GET, should return unauthorized
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id);
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized ,response.StatusCode);
            string _discard = await response.Content.ReadAsStringAsync();
        }

		[Fact]
		public async System.Threading.Tasks.Task TestNewAccountHasNoShareholdersOrDirectors()
		{
			string service = "adoxiolegalentity";
			string shareholders = "/shareholders";
			string directors = "/director-officer-shareholder";

			var loginUser = randomNewUserName("TestLegalEntityUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

			// get the current account.
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            var response = await _client.SendAsync(request);
            string jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);
            string accountId = user.accountid;


            // get shareholders
			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/position/" + user.accountid + shareholders);
			response = await _client.SendAsync(request);
			jsonString = await response.Content.ReadAsStringAsync();
			response.EnsureSuccessStatusCode();
			var responseViewModel = JsonConvert.DeserializeObject<List<ViewModels.AdoxioLegalEntity>>(jsonString);
			Assert.Empty(responseViewModel);

            // get directors
			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/position/" + user.accountid + directors);
            response = await _client.SendAsync(request);
			jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
			responseViewModel = JsonConvert.DeserializeObject<List<ViewModels.AdoxioLegalEntity>>(jsonString);
			Assert.Empty(responseViewModel);

			await LogoutAndCleanupTestUser(strId);
		}

        [Fact]
        public async System.Threading.Tasks.Task TestCRUD()
        {
			string changedName = randomNewUserName("LETest ChangedName", 6);
            string service = "adoxiolegalentity";
            string firstName = "LETFirst";
            string middleName = "LETMiddle";
            string lastName = "LETLast";
			string initialName = randomNewUserName(firstName + " " + lastName, 6);
            DateTime dateOfBirth = DateTime.Now;
            int commonNonVotingshares = 3000;
            int commonVotingshares = 2018;
            bool isIndividual = true;

			var loginUser = randomNewUserName("TestLegalEntityUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            // get the current account.

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            var response = await _client.SendAsync(request);
            string jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);

            string accountId = user.accountid;



            // C - Create
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.Account vmAccount = new ViewModels.Account
            {
                id = accountId
            };

            ViewModels.AdoxioLegalEntity vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
            {
                legalentitytype = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                //position = ViewModels.PositionOptions.Director,
                firstname = firstName,
                middlename = middleName,
                lastname = lastName,
                name = initialName,
                dateofbirth = dateOfBirth,
                isindividual = isIndividual,
                commonvotingshares = commonVotingshares,
                commonnonvotingshares = commonNonVotingshares,
                account = vmAccount 
            };

            jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            try
            {
                response = await _client.SendAsync(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            
            ViewModels.AdoxioLegalEntity responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);

            // name should match.
            Assert.Equal(firstName + " " + lastName, responseViewModel.name);
            var newId = responseViewModel.id;
            
            // R - Read

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + newId);
            response = await _client.SendAsync(request);
			jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
            Assert.Equal(firstName + " " + lastName, responseViewModel.name);

            // U - Update            
            vmAdoxioLegalEntity.firstname = changedName;
            vmAdoxioLegalEntity.id = responseViewModel.id;
            request = new HttpRequestMessage(HttpMethod.Put, "/api/" + service + "/" + newId)
            {
                Content = new StringContent(JsonConvert.SerializeObject(vmAdoxioLegalEntity), Encoding.UTF8, "application/json")
            };
            response = await _client.SendAsync(request);
			var _discard = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // verify that the update persisted.

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + newId);
            response = await _client.SendAsync(request);
			_discard = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();

            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
            Assert.Equal(changedName, responseViewModel.firstname);

            // D - Delete

            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + newId + "/delete");
            response = await _client.SendAsync(request);
			_discard = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // second delete should return a 404.
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + newId + "/delete");
            response = await _client.SendAsync(request);
			_discard = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + newId);
            response = await _client.SendAsync(request);
			_discard = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

			await LogoutAndCleanupTestUser(strId);
        }

		[Fact]
		public async System.Threading.Tasks.Task TestAddShareholderAndDirector()
		{
			string service = "adoxiolegalentity";

			var loginUser = randomNewUserName("TestAddSD", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            // get the current account.
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            var response = await _client.SendAsync(request);
            string jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);

            string accountId = user.accountid;

			// create a Shareholder and fetch it to verify
			request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);
            ViewModels.Account vmAccount = new ViewModels.Account
            {
				id = user.accountid
            };
            ViewModels.AdoxioLegalEntity vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
            {
                legalentitytype = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                //position = ViewModels.PositionOptions.Shareholder,
                firstname = "Test",
                middlename = "The",
                lastname = "Shareholder",
				name = "Test Shareholder",
				commonvotingshares = 100,
                isindividual = true,
                account = vmAccount 
            };
            jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            try
            {
                response = await _client.SendAsync(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            ViewModels.AdoxioLegalEntity responseShareholder = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);

			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + responseShareholder.id);
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
			Assert.Equal(responseShareholder.name, responseViewModel.name);

			// create a Director and fetch it to verify
			request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);
            vmAccount = new ViewModels.Account
            {
                id = user.accountid
            };
            vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
            {
                legalentitytype = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                //position = ViewModels.PositionOptions.Director,
                firstname = "Test",
                middlename = "The",
                lastname = "Director",
                name = "Test Director",
                dateofbirth = DateTime.Now,
                isindividual = true,
                account = vmAccount
            };
            jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            try
            {
                response = await _client.SendAsync(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            ViewModels.AdoxioLegalEntity responseDirector = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);

			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + responseDirector.id);
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
			Assert.Equal(responseDirector.name, responseViewModel.name);

			// logout
			await Logout();

            // login as new user and verify we can't see the Director or Shareholder
            var newLoginUser = randomNewUserName("TestAddSD2", 6);
            var newStrId = await LoginAndRegisterAsNewUser(newLoginUser);

            // try to fetch LegalEntity records of other account
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + responseShareholder.id);
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            //response status code should be 404
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + responseDirector.id);
            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            //response status code should be 404
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // logout
            await LogoutAndCleanupTestUser(newStrId);

            // log back in as user from above ^^^
            await Login(loginUser);

            // delete Director and Shareholder
			request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + responseDirector.id + "/delete");
            response = await _client.SendAsync(request);
            var _discard = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

			request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + responseShareholder.id + "/delete");
            response = await _client.SendAsync(request);
            _discard = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // logout and leanup account
			await LogoutAndCleanupTestUser(strId);
		}

        [Fact]
        public async System.Threading.Tasks.Task TestFileUpload()
        {
            // First create a Legal Entity

			string initialName = randomNewUserName("LETest InitialName", 6);
			string changedName = randomNewUserName("LETest ChangedName", 6);
            string service = "adoxiolegalentity";

            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();
            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            ViewModels.AdoxioLegalEntity viewmodel_adoxio_legalentity = new ViewModels.AdoxioLegalEntity()
            {
                legalentitytype = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                isDirector = true,
                name = initialName
            };
            

            string jsonString = JsonConvert.SerializeObject(viewmodel_adoxio_legalentity);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioLegalEntity responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);

            // name should match.
            Assert.Equal(initialName, responseViewModel.name);
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

            

            // create a new request object for the upload, as we will be using multipart form submission.
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/attachments");
            requestMessage.Content = multiPartContent;

            var uploadResponse = await _client.SendAsync(requestMessage);
            uploadResponse.EnsureSuccessStatusCode();

            // Verify that the file Meta Data matches

            // Verify that the file can be downloaded and the contents match            

            // Cleanup the Legal Entity

            

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
        public async System.Threading.Tasks.Task VerifyConsentCode__WithAGoodCode()
        {
            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            var id = Guid.NewGuid();
            var individualId = Guid.NewGuid();
            ViewModels.SecurityConsentConfirmation securityConsentConfirmation = new ViewModels.SecurityConsentConfirmation()
            {
                email = "",
                parentid = id.ToString(),
                individualid = individualId.ToString()
            };
            string _encryptionKey = this._factory.Configuration["ENCRYPTION_KEY"];
            string json = JsonConvert.SerializeObject(securityConsentConfirmation);
            string code = System.Net.WebUtility.UrlEncode(Utility.EncryptionUtility.EncryptString(json, _encryptionKey));

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/{service}/{id}/verifyconsentcode/{individualId}/?code={code}");
            var response = await _client.SendAsync(request);
            var jsonResult = await response.Content.ReadAsStringAsync();
            string result = JsonConvert.DeserializeObject<String>(jsonResult);
            response.EnsureSuccessStatusCode();
            Assert.Equal("success", result, true);

            await LogoutAndCleanupTestUser(strId);
        }

        [Fact]
        public async System.Threading.Tasks.Task VerifyConsentCode__WithABadCode()
        {
            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            var id = Guid.NewGuid();
            var individualId = Guid.NewGuid();
            ViewModels.SecurityConsentConfirmation securityConsentConfirmation = new ViewModels.SecurityConsentConfirmation()
            {
                email = "",
                parentid = id.ToString(),
                individualid = individualId.ToString()
            };

            //Use a random encryption key
            string _encryptionKey = Guid.NewGuid().ToString();
            string json = JsonConvert.SerializeObject(securityConsentConfirmation);
            string code = System.Net.WebUtility.UrlEncode(Utility.EncryptionUtility.EncryptString(json, _encryptionKey));

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/{service}/{id}/verifyconsentcode/{individualId}/?code={code}");
            var response = await _client.SendAsync(request);
            var jsonResult = await response.Content.ReadAsStringAsync();
            string result = JsonConvert.DeserializeObject<String>(jsonResult);
            response.EnsureSuccessStatusCode();
            Assert.Equal("error", result, true);

            await LogoutAndCleanupTestUser(strId);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestFileListing()
        {
            string initialName = randomNewUserName("First InitialName", 6);
            string changedName = randomNewUserName("First ChangedName", 6);
            string service = "adoxiolegalentity";

            // Login as default user

            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();

            // C - Create a Legal Entity

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);
            
            ViewModels.AdoxioLegalEntity viewmodel_adoxio_legalentity = new ViewModels.AdoxioLegalEntity()
            {
                legalentitytype = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                isDirector = true,
                name = initialName
            };

            string jsonString = JsonConvert.SerializeObject(viewmodel_adoxio_legalentity);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioLegalEntity responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
            // name should match.
            Assert.Equal(initialName, responseViewModel.name);
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

            // create a new request object for the upload, as we will be using multipart form submission.
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/attachments");
            requestMessage.Content = multiPartContent;

            var uploadResponse = await _client.SendAsync(requestMessage);
            uploadResponse.EnsureSuccessStatusCode();

            // Verify that the file Meta Data matches
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/" + id + "/delete");
            response = await _client.SendAsync(request);

            // Verify that the file can be downloaded and the contents match            

            // Cleanup the Legal Entity

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
		public async System.Threading.Tasks.Task TestGetDynamicsLegalEntitiesByPosition()
		{
            var loginUser = randomNewUserName("LegalEntityByPosTest", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser, "LegalEntityByPosTest", "PrivateCorporation");

            // Creating parent
            var levelOneAccount = await AccountFactory();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/api/{service}/business-profile-summary");
            var response = await _client.SendAsync(request);
            String jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var responseViewModelList = JsonConvert.DeserializeObject<List<ViewModels.AdoxioLegalEntity>>(jsonString);

            Assert.Equal("LegalEntityByPosTest TestBusiness", responseViewModelList.First().name);
            var levelOneLegalEntityId = responseViewModelList.First().id;

            // Creating child
            ViewModels.AdoxioLegalEntity vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
            {
                legalentitytype = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                name = "Cannabis Test Investor",
                commonvotingshares = 100,
                account = levelOneAccount,
                isShareholder = true,
                isindividual = false,
                // Parent's id must be populated
                parentLegalEntityId = levelOneLegalEntityId
            };

            jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/child-legal-entity");
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            await _client.SendAsync(request);

            // Get legal entity by position
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{service}/position/{levelOneLegalEntityId}/shareholders");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            await LogoutAndCleanupTestUser(strId);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestThreeTierShareholders()
        {
            string service = "adoxiolegalentity";

            var loginUser = randomNewUserName("TestThreeTierShareholders", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser, "Cybertron Commercial Goods", "PrivateCorporation");
            // Creating parent
            var levelOneAccount = await AccountFactory();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/api/{service}/business-profile-summary");
            var response = await _client.SendAsync(request);
            String jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var responseViewModelList = JsonConvert.DeserializeObject<List<ViewModels.AdoxioLegalEntity>>(jsonString);

            Assert.Equal("Cybertron Commercial Goods TestBusiness", responseViewModelList.First().name);
            var levelOneLegalEntityId = responseViewModelList.First().id;

            // First tier director
            ViewModels.AdoxioLegalEntity vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
            {
                firstname = "Ms.",
                middlename = "Test",
                lastname = "Director",
                commonvotingshares = 100,
                account = levelOneAccount,
                isDirector = true,
                isindividual = true,
                // Parent's id must be populated
                parentLegalEntityId = levelOneLegalEntityId
            };

            jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/child-legal-entity");
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            jsonString = await response.Content.ReadAsStringAsync();
            var responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
            Assert.Equal("Ms. Director", responseViewModel.name);

            // First tier officer
            vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
            {
                firstname = "Andrew",
                middlename = "Test",
                lastname = "Officer",
                commonvotingshares = 100,
                account = levelOneAccount,
                isOfficer = true,
                isindividual = true,
                // Parent's id must be populated
                parentLegalEntityId = levelOneLegalEntityId
            };

            jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/child-legal-entity");
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
            Assert.Equal("Andrew Officer", responseViewModel.name);

            // Creating child
            vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
            {
                legalentitytype = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                name = "Cannabis Test Investor",
                commonvotingshares = 100,
                account = levelOneAccount,
                isShareholder = true,
                isindividual = false,
                // Parent's id must be populated
                parentLegalEntityId = levelOneLegalEntityId
            };

            jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/child-legal-entity");
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
            Assert.Equal("Cannabis Test Investor", responseViewModel.name);
            var levelTwoLegalEntityId = responseViewModel.id;
            var levelTwoAccountId = responseViewModel.shareholderAccountId;
            var levelTwoAccount = new ViewModels.Account {id = levelTwoAccountId };

            // Creating child 2
            vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
            {
                legalentitytype = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                name = "Green Group Investments",
                commonvotingshares = 100,
                account = levelTwoAccount,
                isShareholder = true,
                isindividual = false,
                // Parent's id must be populated
                parentLegalEntityId = levelTwoLegalEntityId
            };

            jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/child-legal-entity");
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
            Assert.Equal("Green Group Investments", responseViewModel.name);
            var levelThreeLegalEntityId = responseViewModel.id;
            var levelThreeAccountId = responseViewModel.shareholderAccountId;
            var levelThreeAccount = new ViewModels.Account { id = levelThreeAccountId };

            // Second tier Officer
            vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
            {
                firstname = "Carlos",
                middlename = "Test",
                lastname = "Officer",
                commonvotingshares = 100,
                account = levelTwoAccount,
                isOfficer = true,
                isindividual = true,
                // Parent's id must be populated
                parentLegalEntityId = levelTwoLegalEntityId
            };

            jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/child-legal-entity");
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
            Assert.Equal("Carlos Officer", responseViewModel.name);

            // Third tier shareholder
            vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
            {
                firstname = "Doug",
                middlename = "Test",
                lastname = "Baldwin",
                commonvotingshares = 100,
                account = levelThreeAccount,
                isShareholder = true,
                isindividual = true,
                // Parent's id must be populated
                parentLegalEntityId = levelThreeLegalEntityId
            };

            jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/child-legal-entity");
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
            Assert.Equal("Doug Baldwin", responseViewModel.name);

            await LogoutAndCleanupTestUser(strId);
        }

        private async Task<ViewModels.Account> AccountFactory()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/current");
            var response = await _client.SendAsync(request);
            string jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);
            ViewModels.Account vmAccount = new ViewModels.Account
            {
                id = user.accountid
            };
            return vmAccount;
        }
    }
}
