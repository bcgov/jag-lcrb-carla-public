using Microsoft.AppServices;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
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
    class SharePointFileManager
    {
        private ApiData apiData;

        public SharePointFileManager(string serverAppIdUri, string webname, string aadTenantId, string clientId, string certFileName, string certPassword)
        {
            string endpoint = serverAppIdUri + webname + "/_api";
            this.apiData = new ApiData(new Uri(endpoint));
             
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

        public async Task<FileSystemItem> GetFile(string name)
        {
            DataServiceQuery<MS.FileServices.FileSystemItem> query = (DataServiceQuery<MS.FileServices.FileSystemItem>)
                from file in apiData.Files
                where file.Name == name
                select file;

            TaskFactory<IEnumerable<FileSystemItem>> taskFactory = new TaskFactory<IEnumerable<FileSystemItem>>();
            IEnumerable<FileSystemItem> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));
            return result.FirstOrDefault();           
        }

        public async Task UploadFile(FileSystemItem fileSystemItem, FileStream fileData, string contentType, string slug)
        {
            // upload a file.            
            apiData.AddToFiles( fileSystemItem );
            apiData.SetSaveStream(fileSystemItem, fileData, true, contentType, slug);
            TaskFactory taskFactory = new TaskFactory();
            await taskFactory.FromAsync(apiData.BeginSaveChanges(null, null), iar => apiData.EndSaveChanges(iar));
        }
        
    }
}
