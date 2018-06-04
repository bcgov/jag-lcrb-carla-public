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
        public const string DefaultDocumentListTitle = "Shared%20Documents";
        private LCLBCannabisDEVDataContext listData;
        private ApiData apiData;
        private AuthenticationResult authenticationResult;
        public string ServerAppIdUri { get; set; }
        public string WebName { get; set; }
        public SharePointFileManager(string serverAppIdUri, string webname, string aadTenantId, string clientId, string certFileName, string certPassword)
        {
            this.ServerAppIdUri = serverAppIdUri;
            this.WebName = webname;
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

        public async Task<List<FileSystemItem>> GetFilesInFolder(string name)
        {
            List<FileSystemItem> result = new List<FileSystemItem>();

            DataServiceQuery<Folder> query = (DataServiceQuery<Folder>)
                from folder in apiData.Folders
                where folder.Name == name
                select folder;

            TaskFactory<IEnumerable<Folder>> taskFactory = new TaskFactory<IEnumerable<Folder>>();
            IEnumerable<Folder> folderResults = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));
            Folder folderResult = folderResults.FirstOrDefault();
            // get the files in the folder.
            if (folderResult != null)
            {
                result = folderResult.Children.ToList();
            }

            return result;
        }

        /// <summary>
        /// Create Folder
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<SP.ListItem> CreateFolder(string listTitle, string folderName)
        {
            // first get a reference to the containing list.
            DataServiceQuery<SP.List> query = (DataServiceQuery<SP.List>)
                from list in apiData.Lists
                where list.Title == listTitle
                select list;
            TaskFactory<IEnumerable<SP.List>> taskFactory = new TaskFactory<IEnumerable<SP.List>>();
            IEnumerable<SP.List> listResults = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));
            SP.List listResult = listResults.FirstOrDefault();
            apiData.UpdateObject(listResult);
            SP.ListItem folder = new SP.ListItem();
            folder.DisplayName = folderName;
            folder.Folder = new SP.Folder
            {
                Name = folderName
            };

            listResult.Items.Add(folder);
            // save to SharePoint.
            TaskFactory saveTaskFactory = new TaskFactory();
            await saveTaskFactory.FromAsync(listData.BeginSaveChanges(null, null), iar => listData.EndSaveChanges(iar));

            return folder;
        }

        /// <summary>
        /// Get Folder
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Folder> GetFolder(string name)
        {            
            DataServiceQuery<Folder> query = (DataServiceQuery<Folder>)
                from folder in apiData.Folders
                where folder.Name == name
                select folder;

            TaskFactory<IEnumerable<Folder>> taskFactory = new TaskFactory<IEnumerable<Folder>>();
            IEnumerable<Folder> folderResults = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));
            Folder result = folderResults.FirstOrDefault();
            // get the files in the folder.
            
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
            // start by ensuring that the folder exists.
            Folder folder = await this.GetFolder(folderName);
            if (folder == null)
            {
                await this.CreateFolder(SharePointFileManager.DefaultDocumentListTitle, folderName);
                folder = await this.GetFolder(folderName);
            }

            // now add the file to the folder.
            string path = "/" + this.WebName + "/" + fileName;

            await this.UploadFile(fileName, path, fileData, contentType);

        }

        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="fileData"></param>
        /// <param name="contentType"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task UploadFile(string name, string path, Stream fileData, string contentType)
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

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task DeleteFile(string url)
        {
            var file = await this.GetFile(url);
            

            apiData.DeleteObject(file);
            TaskFactory taskFactory = new TaskFactory();
            await taskFactory.FromAsync(apiData.BeginSaveChanges(null, null), iar => apiData.EndSaveChanges(iar));            
        }        
    }
}
