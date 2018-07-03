using Microsoft.AppServices;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.DataService;
using MS.FileServices;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Data.Services.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class SharePointFileManager
    {
        public const string DefaultDocumentListTitle = "Account";

        private string AuthorizationHeader;

        private LCLBCannabisDEVDataContext listData;
        private ApiData apiData;
        private AuthenticationResult authenticationResult;
        public string ServerAppIdUri { get; set; }
        public string WebName { get; set; }
        public string apiEndpoint { get; set; }
        string authorization { get; set; }
        private HttpClient client;

        public SharePointFileManager(string serverAppIdUri, string webname, string aadTenantId, string clientId, string certFileName, string certPassword, string ssgUsername, string ssgPassword)
        {
            this.ServerAppIdUri = serverAppIdUri;
            this.WebName = webname;
            string listDataEndpoint = serverAppIdUri + webname + "/_vti_bin/listdata.svc/";
            apiEndpoint = serverAppIdUri + webname + "/_api/";

            this.listData = new LCLBCannabisDEVDataContext(new Uri(listDataEndpoint));
            this.apiData = new ApiData(new Uri(apiEndpoint));

            

            if (string.IsNullOrEmpty(ssgUsername) || string.IsNullOrEmpty(ssgPassword))
            {

                // add authentication.
                var authenticationContext = new AuthenticationContext(
                   "https://login.windows.net/" + aadTenantId);

                // Create the Client cert.
                X509Certificate2 cert = new X509Certificate2(certFileName, certPassword);
                ClientAssertionCertificate clientAssertionCertificate = new ClientAssertionCertificate(clientId, cert);

                //ClientCredential clientCredential = new ClientCredential(clientId, clientKey);
                var task = authenticationContext.AcquireTokenAsync(serverAppIdUri, clientAssertionCertificate);
                task.Wait();
                authenticationResult = task.Result;
                authorization = authenticationResult.CreateAuthorizationHeader();
            }
            else
            {
                // authenticate using the SSG.                
                string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ssgUsername + ":" + ssgPassword));
                authorization = "Basic " + credentials;
                
            }


            apiData.BuildingRequest += (sender, eventArgs) => eventArgs.Headers.Add(
                "Authorization", authorization);

            listData.BuildingRequest += (sender, eventArgs) => eventArgs.Headers.Add(
            "Authorization", authorization);

            // create the HttpClient that is used for our direct REST calls.
            client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");
            client.DefaultRequestHeaders.Add("Authorization", authorization);

        }

        public async Task<List<FileSystemItem>> GetFiles ()
        {            
            DataServiceQuery<MS.FileServices.FileSystemItem> query = (DataServiceQuery<MS.FileServices.FileSystemItem>)
                from file in apiData.Files
                select file;

            TaskFactory<IEnumerable<FileSystemItem>> taskFactory = new TaskFactory<IEnumerable<FileSystemItem>>();
            IEnumerable<FileSystemItem> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));
            return result.ToList();
        }

        public class FileFolderResults
        {
            public List<FileSystemItem> results { get; set; }
        }


        public class FileFolderData
        {
            public FileFolderResults d { get; set; }
        }

        public class FileDetailsList
        {
            public string Name { get; set; }
            public string TimeLastModified { get; set; }
            public string Length { get; set; }
            public string DocumentType { get; set; }
        }

        /// <summary>
        /// Get file details list from SharePoint filtered by folder name and document type
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="folderName"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        public async Task<List<FileDetailsList>> GetFileDetailsListInFolder(string siteName, string folderName, string documentType)
        {
            string serverRelativeUrl = $"/{WebName}/{siteName}/{folderName}";
            HttpRequestMessage endpointRequest =
                            new HttpRequestMessage(HttpMethod.Post, apiEndpoint + "/web/getfolderbyserverrelativeurl('" + serverRelativeUrl + "')/files");

            // make the request.
            var response = await client.SendAsync(endpointRequest);
            string jsonString = await response.Content.ReadAsStringAsync();
            // parse the response
            JObject responseObject = JObject.Parse(jsonString);
            // get JSON response objects into a list
            List<JToken> responseResults = responseObject["d"]["results"].Children().ToList();
            // create file details list to add from response
            List<FileDetailsList> fileDetailsList = new List<FileDetailsList>();
            // create .NET objects
            foreach (JToken responseResult in responseResults)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                FileDetailsList searchResult = responseResult.ToObject<FileDetailsList>();
                //filter by parameter documentType
                int fileDoctypeStart = searchResult.Name.IndexOf("__") + 2;
                string fileDoctype = searchResult.Name.Substring(fileDoctypeStart);
                if (fileDoctype == documentType)
                {
                    searchResult.DocumentType = documentType;
                    fileDetailsList.Add(searchResult);
                }
            }

            return fileDetailsList;
        }

            /// <summary>
            /// Create Folder
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public async Task<SP.Folder> CreateFolder(string listTitle, string folderName)
        {
            HttpRequestMessage endpointRequest =
                new HttpRequestMessage(HttpMethod.Post, apiEndpoint + "web/folders");
            HttpClient client = new HttpClient();
            
            client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");            
            client.DefaultRequestHeaders.Add("Authorization", authorization);
            SP.Folder folder = new SP.Folder()
            {
                Name = folderName,
                ServerRelativeUrl = $"/{WebName}/{listTitle}/{folderName}"
            };

            string jsonString = JsonConvert.SerializeObject(folder);
            endpointRequest.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // make the request.
            var response = await client.SendAsync(endpointRequest);
           
            jsonString = await response.Content.ReadAsStringAsync();
                       
            return folder;
        }



        public async Task<bool> DeleteFolder(string listTitle, string folderName)
        {
            bool result = false;
            // Delete is very similar to a GET.
            string serverRelativeUrl = $"/{WebName}/" + Uri.EscapeDataString(listTitle) + "/" + Uri.EscapeDataString(folderName);

            HttpRequestMessage endpointRequest =
    new HttpRequestMessage(HttpMethod.Post, apiEndpoint + "web/getFolderByServerRelativeUrl('" + serverRelativeUrl + "')");
            

            // We want to delete this folder.
            endpointRequest.Headers.Add("IF-MATCH", "*");
            endpointRequest.Headers.Add("X-HTTP-Method", "DELETE");

            // make the request.
            var response = await client.SendAsync(endpointRequest);

            if (response.StatusCode == HttpStatusCode.OK)
            {                
                result = true;
            }

            return result;
        }

        public async Task<bool> FolderExists(string listTitle, string folderName)
        {
            SP.Folder folder = await GetFolder(listTitle, folderName);

            return (folder != null);            
        }

        public async Task<SP.Folder> GetFolder(string listTitle, string folderName)
        {
            SP.Folder result = null;
            string serverRelativeUrl = $"/{WebName}/" + Uri.EscapeDataString(listTitle) + "/" + Uri.EscapeDataString(folderName);

            HttpRequestMessage endpointRequest =
    new HttpRequestMessage(HttpMethod.Post, apiEndpoint + "web/getFolderByServerRelativeUrl('" + serverRelativeUrl + "')");
            

            // make the request.
            var response = await client.SendAsync(endpointRequest);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<SP.Folder>(jsonString);
            }

            return result;
        }


        /// <summary>
        /// Get File
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<FileSystemItem> GetFile(string url)
        {
            DataServiceQuery<MS.FileServices.FileSystemItem> query = (DataServiceQuery<MS.FileServices.FileSystemItem>)
                from file in apiData.Files
                where file.Url == url
                select file;

            TaskFactory<IEnumerable<FileSystemItem>> taskFactory = new TaskFactory<IEnumerable<FileSystemItem>>();
            IEnumerable<FileSystemItem> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));
            return result.FirstOrDefault();           
        }

        /// <summary>
        /// Get File By ID
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<FileSystemItem> GetFileById(string id)
        {
            DataServiceQuery<MS.FileServices.FileSystemItem> query = (DataServiceQuery<MS.FileServices.FileSystemItem>)
                from file in apiData.Files
                where file.Id == id
                select file;

            TaskFactory<IEnumerable<FileSystemItem>> taskFactory = new TaskFactory<IEnumerable<FileSystemItem>>();
            IEnumerable<FileSystemItem> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));
            return result.FirstOrDefault();
        }

        public async Task AddFile(String folderName,  String fileName, Stream fileData, string contentType)
        {

            bool folderExists = await this.FolderExists(DefaultDocumentListTitle, folderName);
            if (! folderExists)
            {
              var folder =  await this.CreateFolder(DefaultDocumentListTitle, folderName);                
            }

            // now add the file to the folder.
            
            await this.UploadFile(fileName, DefaultDocumentListTitle, folderName, fileData, contentType);

        }

        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="listTitle"></param>
        /// <param name="folderName"></param>
        /// <param name="fileData"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<bool> UploadFile(string name, string listTitle, string folderName, Stream fileData, string contentType)
        {
            bool result = false;
            // Delete is very similar to a GET.
            string serverRelativeUrl = $"/{WebName}/" + Uri.EscapeDataString(listTitle) + "/" + Uri.EscapeDataString(folderName);

            HttpRequestMessage endpointRequest =
    new HttpRequestMessage(HttpMethod.Post, apiEndpoint + "web/getFolderByServerRelativeUrl('" + serverRelativeUrl + "')/Files/add(url='" 
    + name + "',overwrite=true)");
            // convert the stream into a byte array.
            MemoryStream ms = new MemoryStream();
            fileData.CopyTo(ms);
            Byte[] data = ms.ToArray();

            endpointRequest.Content = new ByteArrayContent(data);

            // make the request.
            var response = await client.SendAsync(endpointRequest);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<byte[]> DownloadFile(string url)
        {
            var file = await this.GetFile(url);
            byte[] result = null;
            var webRequest = System.Net.WebRequest.Create(this.ServerAppIdUri + "/_api/Web/getfilebyserverrelativeurl('"+ url +"')/$value");            
            HttpWebRequest request = (HttpWebRequest)webRequest;
            request.PreAuthenticate = true;
            request.Headers.Add ("Authorization", authenticationResult.CreateAuthorizationHeader());
            request.Accept = "*";
                        
            // we need to add authentication to a HTTP Client to fetch the file.
            using (
                MemoryStream ms = new MemoryStream())
            {
                await request.GetResponse().GetResponseStream().CopyToAsync(ms);
                result = ms.ToArray();
            }
            
            return result;
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFile(string listTitle, string folderName, string fileName)
        {
            bool result = false;
            // Delete is very similar to a GET.
            string serverRelativeUrl = $"/{WebName}/" + Uri.EscapeDataString(listTitle) + "/" + Uri.EscapeDataString(folderName) + "/" + Uri.EscapeDataString(fileName);

            HttpRequestMessage endpointRequest =
    new HttpRequestMessage(HttpMethod.Post, apiEndpoint + "web/GetFileByServerRelativeUrl('" + serverRelativeUrl + "')");

            // We want to delete this file.
            endpointRequest.Headers.Add("IF-MATCH", "*");
            endpointRequest.Headers.Add("X-HTTP-Method", "DELETE");

            // make the request.
            var response = await client.SendAsync(endpointRequest);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = true;
            }

            return result;
        }        
    }
}
