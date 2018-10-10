using Gov.Lclb.Cllb.Public.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class FileTests : ApiIntegrationTestBaseWithLogin
    {
        public FileTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory)
        { }

        [Fact]
        public async System.Threading.Tasks.Task TestLicenseApplicationUpload()
        {
            // First create a Legal Entity

            string initialName = randomNewUserName("LETest InitialName", 6);
            string changedName = randomNewUserName("LETest ChangedName", 6);
            const string applicationService = "adoxioapplication";
            const string fileService = "file";

            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            User user = await GetCurrentUser();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + applicationService);
            Account currentAccount1 = await GetAccountForCurrentUser();
            AdoxioApplication viewmodel_application = new AdoxioApplication()
            {
                name = initialName,
                applyingPerson = "Applying Person",
                applicant = currentAccount1,
                applicantType = AdoxioApplicantTypeCodes.PrivateCorporation //*Mandatory (label=business type)
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
            AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<AdoxioApplication>(jsonString);

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
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/" + fileService + "/" + applicationId + "/attachments/application");
            requestMessage.Content = multiPartContent;

            var uploadResponse = await _client.SendAsync(requestMessage);
            uploadResponse.EnsureSuccessStatusCode();

            // Verify that the file Meta Data matches
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{fileService}/{id}/attachments/application/{System.Uri.EscapeDataString(documentType)}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            var files = JsonConvert.DeserializeObject<List<FileSystemItem>>(jsonString);
            Assert.True(files.Count > 0);

            string serverrelativeurl = files[0].serverrelativeurl;
            string fileName = files[0].name;

            // Verify that the file can be downloaded and the contents match
            // {entityId}/download-file/{entityName}"
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{fileService}/{id}/download-file/application/{fileName}?serverRelativeUrl={serverrelativeurl}&documentType={documentType}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Cleanup the Application

            request = new HttpRequestMessage(HttpMethod.Delete, "/api/" + fileService + "/" + id + $"/attachments/application/?serverRelativeUrl={serverrelativeurl}&documentType={documentType}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{fileService}/{id}/attachments/application/{filename}?serverRelativeUrl={serverrelativeurl}&documentType={documentType}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            files = JsonConvert.DeserializeObject<List<FileSystemItem>>(jsonString);
            Assert.True(files.Count == 0);

            // Cleanup the Application

            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + applicationService + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            await LogoutAndCleanupTestUser(strId);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestLicenseApplicationInvalidFile()
        {
            // First create a Legal Entity

            string initialName = randomNewUserName("LETest InitialName", 6);
            string changedName = randomNewUserName("LETest ChangedName", 6);
            const string applicationService = "adoxioapplication";
            const string fileService = "file";

            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            User user = await GetCurrentUser();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + applicationService);
            Account currentAccount1 = await GetAccountForCurrentUser();
            AdoxioApplication viewmodel_application = new AdoxioApplication()
            {
                name = initialName,
                applyingPerson = "Applying Person",
                applicant = currentAccount1,
                applicantType = AdoxioApplicantTypeCodes.PrivateCorporation //*Mandatory (label=business type)
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
            AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<AdoxioApplication>(jsonString);

            string id = responseViewModel.id;

            // Attach a file

            string testData = "This is just a test.";
            byte[] bytes = Encoding.ASCII.GetBytes(testData);
            string documentType = "Test Document Type";

            // Create random filename
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[261];
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
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/" + fileService + "/" + applicationId + "/attachments/application");
            requestMessage.Content = multiPartContent;

            var uploadResponse = await _client.SendAsync(requestMessage);

            // should be a 404.
            Assert.Equal(HttpStatusCode.NotFound, uploadResponse.StatusCode);

            // Cleanup the Application

            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + applicationService + "/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            await LogoutAndCleanupTestUser(strId);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestFileUploadWithContact()
        {
            // First create a Legal Entity

            string initialName = randomNewUserName("LETest InitialName", 6);
            string changedName = randomNewUserName("LETest ChangedName", 6);
            const string applicationService = "adoxioapplication";
            const string fileService = "file";

            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            User user = await GetCurrentUser();

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + applicationService);
            Account currentAccount1 = await GetAccountForCurrentUser();
            AdoxioApplication viewmodel_application = new AdoxioApplication()
            {
                name = initialName,
                applyingPerson = "Applying Person",
                applicant = currentAccount1,
                applicantType = AdoxioApplicantTypeCodes.PrivateCorporation //*Mandatory (label=business type)
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
            AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<AdoxioApplication>(jsonString);

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


            string contactId = currentAccount1.primarycontact.id;

            // create a new request object for the upload, as we will be using multipart form submission.
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/" + fileService + "/" + contactId + "/attachments/contact");
            requestMessage.Content = multiPartContent;

            var uploadResponse = await _client.SendAsync(requestMessage);
            uploadResponse.EnsureSuccessStatusCode();

            // Verify that the file Meta Data matches
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{fileService}/{contactId}/attachments/contact/{Uri.EscapeDataString(documentType)}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            var files = JsonConvert.DeserializeObject<List<FileSystemItem>>(jsonString);
            Assert.True(files.Count > 0);

            // Verify that the file can be downloaded and the contents match
            // {entityId}/download-file/{entityName}"
            string serverrelativeurl = Uri.EscapeDataString( files[0].serverrelativeurl );
            string fileName = files[0].name;

            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{fileService}/{contactId}/download-file/contact/{fileName}?serverRelativeUrl={serverrelativeurl}&documentType={documentType}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Cleanup the Application Files

            request = new HttpRequestMessage(HttpMethod.Delete, "/api/" + fileService + "/" + contactId + $"/attachments/contact?serverRelativeUrl={serverrelativeurl}&documentType={documentType}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{fileService}/{contactId}/attachments/contact/{System.Uri.EscapeDataString(documentType)}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            files = JsonConvert.DeserializeObject<List<FileSystemItem>>(jsonString);
            Assert.True(files.Count == 0);

           

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
            const string applicationService = "adoxioapplication";
            const string fileService = "file";

            // login as default and get account for current user
            string loginUser = randomNewUserName("TestAppUser_", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            User user = await GetCurrentUser();
            Account currentAccount = await GetAccountForCurrentUser();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + applicationService);

            AdoxioApplication viewmodel_application = new AdoxioApplication()
            {
                licenseType = "Cannabis Retail Store"
                ,
                applicantType = AdoxioApplicantTypeCodes.PrivateCorporation
                ,
                registeredEstablishment = GeneralYesNo.No
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
            AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<AdoxioApplication>(jsonString);

            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid applicationId = new Guid(responseViewModel.id);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + applicationService + "/" + applicationId);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<AdoxioApplication>(jsonString);
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
                    response = _client.PostAsync("/api/" + fileService + "/" + applicationId + "/attachments/application", formData).Result;
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    i--;
                }
            }

            // Get
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{fileService}/{applicationId}/attachments/application/{System.Uri.EscapeDataString(documentType)}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            var files = JsonConvert.DeserializeObject<List<FileSystemItem>>(jsonString);
            var numberOfFiles = files.Count;
            files.ForEach(async file =>
            {
                // Delete
                request = new HttpRequestMessage(HttpMethod.Delete, "/api/" + fileService + "/" + applicationId + $"/attachments/application?serverRelativeUrl={files[0].serverrelativeurl}");
                response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            });
            Assert.Equal(fileLimit, numberOfFiles);

            await LogoutAndCleanupTestUser(strId);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestFileUploadTypes()
        {
            // Create application
            string initialName = randomNewUserName("Application Initial Name ", 6);
            string changedName = randomNewUserName("Application Changed Name ", 6);
            const string applicationService = "adoxioapplication";
            const string fileService = "file";

            // login as default and get account for current user
            string loginUser = randomNewUserName("TestAppUser_", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            User user = await GetCurrentUser();
            Account currentAccount = await GetAccountForCurrentUser();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + applicationService);

            AdoxioApplication viewmodel_application = new AdoxioApplication()
            {
                licenseType = "Cannabis Retail Store"
                ,
                applicantType = AdoxioApplicantTypeCodes.PrivateCorporation
                ,
                registeredEstablishment = GeneralYesNo.No
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
            AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<AdoxioApplication>(jsonString);

            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            Guid applicationId = new Guid(responseViewModel.id);

            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + applicationService + "/" + applicationId);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<AdoxioApplication>(jsonString);
            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.True(responseViewModel.applicant != null);
            Assert.Equal(currentAccount.id, responseViewModel.applicant.id);

            // Test upload, get, delete attachment
            string documentType = "Licence Application Main";


            // Upload
            string[] fileTypes = new string[] { ".jpg", ".jpeg", ".png", ".word", ".xls", ".pdf" };
            foreach (string fileType in fileTypes)
            {
                using (var formData = new MultipartFormDataContent())
                {
                    var randomNum = new Random().Next(1000) + 100;
                    var fileContent = new ByteArrayContent(Encoding.ASCII.GetBytes(randomNewUserName("test data", randomNum)));
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file",
                        FileName = $"test-{Guid.NewGuid().ToString()}{fileType}"
                    };
                    formData.Add(fileContent);
                    formData.Add(new StringContent(documentType, Encoding.UTF8, "application/json"), "documentType");
                    response = _client.PostAsync("/api/" + fileService + "/" + applicationId + "/attachments/application", formData).Result;
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }

            // Get
            request = new HttpRequestMessage(HttpMethod.Get, $"/api/{fileService}/{applicationId}/attachments/application/{System.Uri.EscapeDataString(documentType)}");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            var files = JsonConvert.DeserializeObject<List<FileSystemItem>>(jsonString);
            var numberOfFiles = files.Count;
            files.ForEach(async file =>
            {
                // Delete
                request = new HttpRequestMessage(HttpMethod.Delete, "/api/" + fileService + "/" + applicationId + $"/attachments/application?serverRelativeUrl={files[0].serverrelativeurl}");
                response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            });

            await LogoutAndCleanupTestUser(strId);
        }
    }

}