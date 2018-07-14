using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;

namespace Gov.Lclb.Cllb.Public.Test
{
	public class SecurityHelper
	{
		public static async Task<ViewModels.Account> GetAccountRecord(HttpClient _client, string id, bool expectSuccess)
		{
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/account/" + id);
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

		public static async Task<ViewModels.AdoxioLegalEntity> GetLegalEntityRecordForCurrent(HttpClient _client)
        {
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/adoxiolegalentity/applicant");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioLegalEntity responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
            return responseViewModel;
        }

		public static async Task<ViewModels.AdoxioLegalEntity> GetLegalEntityRecord(HttpClient _client, string id, bool expectSuccess)
        {
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/adoxiolegalentity/" + id);
            var response = await _client.SendAsync(request);
            if (expectSuccess)
            {
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
				ViewModels.AdoxioLegalEntity responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);
                return responseViewModel;
            }
            else
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                var _discard = await response.Content.ReadAsStringAsync();
                return null;
            }
        }

		public static async Task<ViewModels.AdoxioLegalEntity> CreateDirectorOrShareholder(HttpClient _client, ViewModels.User user,
		                                                                                  bool isDirectorFlag, bool isOfficerFlag, bool isShareholderFlag) 
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "/api/adoxiolegalentity/child-legal-entity");
            var vmAccount = new ViewModels.Account
            {
                id = user.accountid
            };
            var vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
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
                account = vmAccount
            };
            var jsonString = JsonConvert.SerializeObject(vmAdoxioLegalEntity);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            ViewModels.AdoxioLegalEntity responseDirector = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);

			var responseViewModel = await GetLegalEntityRecord(_client, responseDirector.id, true);
            Assert.Equal(responseDirector.name, responseViewModel.name);
			return responseViewModel;
		}

		public static async Task<ViewModels.AdoxioLegalEntity> CreateOrganizationalShareholder(HttpClient _client, ViewModels.User user, string accountLegalEntityId)
        {
			var request = new HttpRequestMessage(HttpMethod.Post, "/api/adoxiolegalentity/child-legal-entity");
            var vmAccount = new ViewModels.Account
            {
                id = user.accountid
            };
            var vmAdoxioLegalEntity = new ViewModels.AdoxioLegalEntity
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
            ViewModels.AdoxioLegalEntity responseDirector = JsonConvert.DeserializeObject<ViewModels.AdoxioLegalEntity>(jsonString);

            var responseViewModel = await GetLegalEntityRecord(_client, responseDirector.id, true);
            Assert.Equal(responseDirector.name, responseViewModel.name);
            return responseViewModel;
        }

		public static async Task<string> DeleteLegalEntityRecord(HttpClient _client, string id)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "/api/adoxiolegalentity/" + id + "/delete");
            var response = await _client.SendAsync(request);
            var _discard = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
			return id;
		}

        public static async Task<string> UploadFileToAccount(HttpClient _client, string id, string docType)
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
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/adoxiolegalentity/" + id + "/attachments");
            requestMessage.Content = multiPartContent;

            var uploadResponse = await _client.SendAsync(requestMessage);
            uploadResponse.EnsureSuccessStatusCode();

			return filename;
		}

		public static async Task<List<ViewModels.FileSystemItem>> GetFileListForAccount(HttpClient _client, string id, string docType, bool expectSuccess)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/adoxiolegalentity/" + id + "/attachments/" + docType);
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

		public static async Task<string> DownloadFileForAccount(HttpClient _client, string id, string fileId, bool expectSuccess)
        {
			/* not sure if there is a way to download a file
			var request = new HttpRequestMessage(HttpMethod.Get, "/api/adoxiolegalentity/" + id + "/attachments/" + fileId);
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

        public static async Task<string> DeleteFileForAccount(HttpClient _client, string id)
        {
            // TODO
			return null;
        }
	}
}
