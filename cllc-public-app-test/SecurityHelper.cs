using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class SecurityHelper
	{
		public static async Task<ViewModels.Account> GetAccountRecord(HttpClient _client, string id, bool expectSuccess)
		{
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/accounts/" + id);
            var response = await _client.SendAsync(request);
			if (expectSuccess)
			{
				response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                ViewModels.Account responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);
				return responseViewModel;
			}
			else
			{
				Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
				var _discard = await response.Content.ReadAsStringAsync();
				return null;
			}
		}

		public static async Task<ViewModels.Account> UpdateAccountRecord(HttpClient _client, string id, ViewModels.Account account, bool expectSuccess)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "/api/accounts/" + id)
			{
                Content = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);
            if (expectSuccess)
            {
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                ViewModels.Account responseViewModel = JsonConvert.DeserializeObject<ViewModels.Account>(jsonString);
                return responseViewModel;
            }
            else
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                var _discard = await response.Content.ReadAsStringAsync();
                return null;
            }
        }

		public static async Task<ViewModels.LegalEntity> GetLegalEntityRecordForCurrent(HttpClient _client)
        {
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/legalentities/applicant");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.LegalEntity responseViewModel = JsonConvert.DeserializeObject<ViewModels.LegalEntity>(jsonString);
            return responseViewModel;
        }

		public static async Task<ViewModels.LegalEntity> GetLegalEntityRecord(HttpClient _client, string id, bool expectSuccess)
        {
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/legalentities/" + id);
            var response = await _client.SendAsync(request);
            if (expectSuccess)
            {
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
				ViewModels.LegalEntity responseViewModel = JsonConvert.DeserializeObject<ViewModels.LegalEntity>(jsonString);
                return responseViewModel;
            }
            else
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                var _discard = await response.Content.ReadAsStringAsync();
                return null;
            }
        }

		public static async Task<List<ViewModels.LegalEntity>> GetLegalEntitiesByPosition(HttpClient _client, string parentAccountId, string positionType, bool expectSuccess)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/legalentities/position/" + parentAccountId + "/" + positionType);
            var response = await _client.SendAsync(request);
			if (expectSuccess)
            {
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
				List<ViewModels.LegalEntity> responseViewModel = JsonConvert.DeserializeObject<List<ViewModels.LegalEntity>>(jsonString);
                return responseViewModel;
            }
            else
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                var _discard = await response.Content.ReadAsStringAsync();
                return null;
            }
        }

		public static async Task<ViewModels.LegalEntity> CreateDirectorOrShareholder(HttpClient _client, ViewModels.User user, string accountLegalEntityId,
		                                                                                  bool isDirectorFlag, bool isOfficerFlag, bool isShareholderFlag) 
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "/api/legalentities/child-legal-entity");
            var vmAccount = new ViewModels.Account
            {
                id = user.accountid
            };
            var vmAdoxioLegalEntity = new ViewModels.LegalEntity
            {
                legalentitytype = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                firstname = "Test",
                middlename = "The",
                lastname = "Directororshareholder",
                name = "Test Directororshareholder",
                dateofbirth = DateTime.Now,
                isindividual = true,
				isDirector = isDirectorFlag,
				isOfficer = isOfficerFlag,
				isShareholder = isShareholderFlag,
                account = vmAccount,
				parentLegalEntityId = accountLegalEntityId
            };
            var jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            ViewModels.LegalEntity responseDirector = JsonConvert.DeserializeObject<ViewModels.LegalEntity>(jsonString);

			var responseViewModel = await GetLegalEntityRecord(_client, responseDirector.id, true);
            Assert.Equal(responseDirector.name, responseViewModel.name);
			return responseViewModel;
		}

		public static async Task<ViewModels.LegalEntity> CreateOrganizationalShareholder(HttpClient _client, ViewModels.User user, string accountLegalEntityId)
        {
			var request = new HttpRequestMessage(HttpMethod.Post, "/api/legalentities/child-legal-entity");
            var vmAccount = new ViewModels.Account
            {
                id = user.accountid
            };
            var vmAdoxioLegalEntity = new ViewModels.LegalEntity
            {
                legalentitytype = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation,
                firstname = "Test",
                middlename = "The",
                lastname = "Orgshareholder",
                name = "Test Orgshareholder",
                dateofbirth = DateTime.Now,
				isShareholder = true,
                isindividual = false,
                account = vmAccount,
				parentLegalEntityId = accountLegalEntityId
            };
            var jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            ViewModels.LegalEntity responseDirector = JsonConvert.DeserializeObject<ViewModels.LegalEntity>(jsonString);

            var responseViewModel = await GetLegalEntityRecord(_client, responseDirector.id, true);
            Assert.Equal(responseDirector.name, responseViewModel.name);
            return responseViewModel;
        }

		public static async Task<string> DeleteLegalEntityRecord(HttpClient _client, string id)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "/api/legalentities/" + id + "/delete");
            var response = await _client.SendAsync(request);
            var _discard = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
			return id;
		}

        public static async Task<string> UploadFileToLegalEntity(HttpClient _client, string legalentityid, string docType)
		{
			// Attach a file
            string testData = "This is just a test.";
            byte[] bytes = Encoding.ASCII.GetBytes(testData);
            string documentType = docType;

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
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/legalentities/" + legalentityid + "/attachments");
            requestMessage.Content = multiPartContent;

            var uploadResponse = await _client.SendAsync(requestMessage);
            

			return filename;
		}

		public static async Task<List<ViewModels.FileSystemItem>> GetFileListForAccount(HttpClient _client, string id, string docType, bool expectSuccess)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/legalentities/" + id + "/attachments/" + docType);
			var response = await _client.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();
			if (expectSuccess)
			{
				response.EnsureSuccessStatusCode();
				List<ViewModels.FileSystemItem> files = JsonConvert.DeserializeObject<List<ViewModels.FileSystemItem>>(jsonString);
				return files;
			}
			else
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                return null;
            }
		}

        public static string DownloadFileForAccount(HttpClient _client, string id, string fileId, bool expectSuccess)
        {
            /*
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/legalentities/" + id + "/attachment/" + fileId);
			var response = await _client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            if (expectSuccess)
            {
                response.EnsureSuccessStatusCode();
				return responseString;
            }
            else
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                return null;
            }
            */
            return null;
        }

        public static string DeleteFileForAccount(HttpClient _client, string id)
        {
            /*
			var request = new HttpRequestMessage(HttpMethod.Delete, "/api/legalentities/" + id + "/attachments");
			var response = await _client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
			*/
            return null;
        }

        public static async Task<ViewModels.Application> CreateLicenceApplication(HttpClient _client, ViewModels.Account currentAccount)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "/api/Application");

            ViewModels.Application viewmodel_application = new ViewModels.Application()
            {
                LicenseType = "Cannabis Retail Store", //*Mandatory field **This is an entity** E.g.Cannabis Retail Store
                ApplicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation, //*Mandatory (label=business type)
                RegisteredEstablishment = ViewModels.GeneralYesNo.No, //*Mandatory (Yes=1, No=0)
                Applicant = currentAccount, //account
                EstablishmentName = "Not a Dispensary",
                EstablishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1",
                EstablishmentAddressStreet = "123 Any Street",
                EstablishmentAddressCity = "Victoria, BC",
                EstablishmentAddressPostalCode = "V1X 1X1"
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            ViewModels.Application responseViewModel = JsonConvert.DeserializeObject<ViewModels.Application>(jsonString);

            //Assert.Equal("Applying Person", responseViewModel.applyingPerson);
            Assert.Equal("Not a Dispensary", responseViewModel.EstablishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.EstablishmentAddressCity);
            Assert.Equal("V1X 1X1", responseViewModel.EstablishmentAddressPostalCode);

			return responseViewModel;
		}

		public static async Task<ViewModels.Application> GetLicenceApplication(HttpClient _client, string applicationId, bool expectSuccess)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/Application/" + applicationId);
            var response = await _client.SendAsync(request);
			if (expectSuccess)
            {
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
				ViewModels.Application responseViewModel = JsonConvert.DeserializeObject<ViewModels.Application>(jsonString);
                return responseViewModel;
            }
            else
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                var _discard = await response.Content.ReadAsStringAsync();
                return null;
            }
		}

		public static async Task<Dictionary<string, string>> PayLicenceApplicationFee(HttpClient _client, string applicationId, bool accepted, bool expectSuccess)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/payments/submit/" + applicationId);
            var response = await _client.SendAsync(request);
			if (expectSuccess)
			{
				response.EnsureSuccessStatusCode();
				string json = await response.Content.ReadAsStringAsync();
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			}
			else
			{
				Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
				string _discard = await response.Content.ReadAsStringAsync();
			}

			string accept = accepted ? "/ACCEPT" : "/DECLINE";
			request = new HttpRequestMessage(HttpMethod.Get, "/api/payments/verify/" + applicationId + accept);
            response = await _client.SendAsync(request);
            if (expectSuccess)
            {
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
				return values;
            }
            else
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                string _discard = await response.Content.ReadAsStringAsync();
				return null;
            }
		}

		public static async Task<string> DeleteLicenceApplication(HttpClient _client, string applicationId)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "/api/applications/" + applicationId + "/delete");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
			return applicationId;
		}
        
		public static async Task<string> UploadFileToApplication(HttpClient _client, string id, string docType)
        {
            // Attach a file
            string testData = "This is just a test.";
            byte[] bytes = Encoding.ASCII.GetBytes(testData);
            string documentType = docType;

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
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/applications/" + id + "/attachments");
            requestMessage.Content = multiPartContent;

            var uploadResponse = await _client.SendAsync(requestMessage);
            uploadResponse.EnsureSuccessStatusCode();

            return filename;
        }

        public static async Task<List<ViewModels.FileSystemItem>> GetFileListForApplication(HttpClient _client, string id, string docType, bool expectSuccess)
        {
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/applications/" + id + "/attachments/" + docType);
            var response = await _client.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();
            if (expectSuccess)
            {
                response.EnsureSuccessStatusCode();
                List<ViewModels.FileSystemItem> files = JsonConvert.DeserializeObject<List<ViewModels.FileSystemItem>>(jsonString);
                return files;
            }
            else
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                return null;
            }
        }

        public static string DownloadFileForApplication(HttpClient _client, string id, string fileId, bool expectSuccess)
        {
            /*
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Application/" + id + "/attachment/" + fileId);
            var response = await _client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            if (expectSuccess)
            {
                response.EnsureSuccessStatusCode();
                return responseString;
            }
            else
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                return null;
            }
            */
            return null;
        }

        public static string DeleteFileForApplication(HttpClient _client, string id)
        {
            /*
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/Application/" + id + "/attachments");
            var response = await _client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            */
            return null;
        }

    }
}
