using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using System.Net.Http.Headers;
using Gov.Lclb.Cllb.Public.ViewModels;

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
			string shareholders = "shareholder";
			string directors = "director-officer";

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
			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/position/" + shareholders);
			response = await _client.SendAsync(request);
			jsonString = await response.Content.ReadAsStringAsync();
			response.EnsureSuccessStatusCode();
			var responseViewModel = JsonConvert.DeserializeObject<List<ViewModels.AdoxioLegalEntity>>(jsonString);
			Assert.Equal(0, responseViewModel.Count);

            // get directors
			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/position/" + directors);
            response = await _client.SendAsync(request);
			jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
			responseViewModel = JsonConvert.DeserializeObject<List<ViewModels.AdoxioLegalEntity>>(jsonString);
			Assert.Equal(0, responseViewModel.Count);

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
                legalentitytype = ViewModels.Adoxio_applicanttypecodes.PrivateCorporation,
                position = ViewModels.PositionOptions.Director,
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

			var loginUser = randomNewUserName("TestLegalEntityUser", 6);
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
                legalentitytype = ViewModels.Adoxio_applicanttypecodes.PrivateCorporation,
                position = ViewModels.PositionOptions.Shareholder,
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
                legalentitytype = ViewModels.Adoxio_applicanttypecodes.PrivateCorporation,
                position = ViewModels.PositionOptions.Director,
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

			// login as Default user and verify we can't see the Director or Shareholder
			await LoginAsDefault();

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
			await Logout();

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

            await LoginAsDefault();
            ViewModels.User user = await GetCurrentUser();
            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);

            Adoxio_legalentity adoxio_legalentity = new Adoxio_legalentity()
            {
                Adoxio_legalentityid = Guid.NewGuid(),
                Adoxio_legalentitytype = (int?)ViewModels.Adoxio_applicanttypecodes.PrivateCorporation,
                Adoxio_position = (int?)ViewModels.PositionOptions.Director,
                Adoxio_name = initialName
            };

            ViewModels.AdoxioLegalEntity viewmodel_adoxio_legalentity = adoxio_legalentity.ToViewModel();

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

            string accountId = user.accountid;

            // create a new request object for the upload, as we will be using multipart form submission.
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + accountId + "/attachments");
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

			await Logout();
        }

        [Fact]
        public async System.Threading.Tasks.Task VerifyConsentCode__WithAGoodCode()
        {
            await LoginAsDefault();

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

            await Logout();
        }

        [Fact]
        public async System.Threading.Tasks.Task VerifyConsentCode__WithABadCode()
        {
            await LoginAsDefault();

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

            await Logout();
        }

        [Fact]
        public async System.Threading.Tasks.Task TestFileListing()
        {
            string initialName = randomNewUserName("First InitialName", 6);
            string changedName = randomNewUserName("First ChangedName", 6);
            string service = "adoxiolegalentity";

            // Login as default user

            await LoginAsDefault();
            ViewModels.User user = await GetCurrentUser();

            // C - Create a Legal Entity

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service);
            Adoxio_legalentity adoxio_legalentity = new Adoxio_legalentity()
            {
                Adoxio_legalentityid = Guid.NewGuid(),
                Adoxio_legalentitytype = (int?)ViewModels.Adoxio_applicanttypecodes.PrivateCorporation,
                Adoxio_position = (int?)ViewModels.PositionOptions.Director,
                Adoxio_name = initialName
            };
            ViewModels.AdoxioLegalEntity viewmodel_adoxio_legalentity = adoxio_legalentity.ToViewModel();
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

            string accountId = user.accountid;

            // create a new request object for the upload, as we will be using multipart form submission.
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + accountId + "/attachments");
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

            await Logout();
        }

    }
}
