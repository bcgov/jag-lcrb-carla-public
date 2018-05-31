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
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class SharePointFileManager
    {
        private LCLBCannabisDEVDataContext listData;
        private ApiData apiData;
        private AuthenticationResult authenticationResult;

        public SharePointFileManager(string serverAppIdUri, string webname, string aadTenantId, string clientId, string certFileName, string certPassword)
        {
            string listDataEndpoint = serverAppIdUri + webname + "/_vti_bin/listdata.svc/";
            string apiEndpoint = serverAppIdUri + webname + "/_api/";

            this.listData = new LCLBCannabisDEVDataContext(new Uri(listDataEndpoint));
            this.apiData = new ApiData(new Uri(apiEndpoint));
             
            // add authentication.
            var authenticationContext = new AuthenticationContext(
               "https://login.windows.net/" + aadTenantId);

            // Create the Client cert.
            X509Certificate2 cert = new X509Certificate2(certFileName, certPassword);
            ClientAssertionCertificate clientAssertionCertificate = new ClientAssertionCertificate(clientId, cert);

            //ClientCredential clientCredential = new ClientCredential(clientId, clientKey);
            var task = authenticationContext.AcquireTokenAsync(serverAppIdUri, clientAssertionCertificate);
            task.Wait();
            AuthenticationResult authenticationResult = task.Result;

            apiData.BuildingRequest += (sender, eventArgs) => eventArgs.Headers.Add(
            "Authorization", authenticationResult.CreateAuthorizationHeader());

            listData.BuildingRequest += (sender, eventArgs) => eventArgs.Headers.Add(
            "Authorization", authenticationResult.CreateAuthorizationHeader());
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

        public async Task UploadFile(string name, string path, Stream fileData, string contentType, string slug)
        {
            // upload a file.   
            
            DocumentsItem documentsItem = new DocumentsItem()
            {
                ContentType = contentType,
                Name = name,
                Title = name,
                Path = path
            };

            listData.AddToDocuments(documentsItem);            
            DataServiceRequestArgs dsra = new DataServiceRequestArgs()
            {
                ContentType = contentType,                
                Slug = path
            };


            listData.SetSaveStream(documentsItem, fileData, false, dsra);
            TaskFactory taskFactory = new TaskFactory();
            await taskFactory.FromAsync(listData.BeginSaveChanges(null, null), iar => listData.EndSaveChanges(iar));

            

        }

        public async Task<byte[]> DownloadFile(string url)
        {
            var file = await this.GetFile(url);
            byte[] result = null;

            var request = System.Net.HttpWebRequest.Create(file.Url);
            request.Headers.Add ("Authorization", authenticationResult.CreateAuthorizationHeader());

            // we need to add authentication to a HTTP Client to fetch the file.
            using (MemoryStream ms = new MemoryStream())
            {
                request.GetResponse().GetResponseStream().CopyTo(ms);
                result = ms.ToArray();
            }
            
            return result;
        }

        public async Task DeleteFile(string url)
        {
            var file = await this.GetFile(url);
            

            apiData.DeleteObject(file);
            TaskFactory taskFactory = new TaskFactory();
            await taskFactory.FromAsync(apiData.BeginSaveChanges(null, null), iar => apiData.EndSaveChanges(iar));            
        }        
    }
}
