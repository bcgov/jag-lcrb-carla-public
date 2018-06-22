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
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class SharePointFileManager
    {
        public const string DefaultDocumentListTitle = "Account";

        //public const string DefaultDocumentList = "Account";

        private string AuthorizationHeader;

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
            authenticationResult = task.Result;
            

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

        public async Task<List<FileSystemItem>> GetFilesInFolder(string listTitle, string folderName)
        {
            List<FileSystemItem> result = new List<FileSystemItem>();

            // first get a reference to the containing list.
            DataServiceQuery<SP.List> query = (DataServiceQuery<SP.List>)
                from list in apiData.Lists
                where list.Title == listTitle
                select list;
            TaskFactory<IEnumerable<SP.List>> taskFactory = new TaskFactory<IEnumerable<SP.List>>();
            IEnumerable<SP.List> listResults = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));
            SP.List listResult = listResults.FirstOrDefault();

            SP.ListItem folderListItem = listResult.Items.Where(x => x.DisplayName == folderName).FirstOrDefault();


            var items = folderListItem.Folder.Files;
            foreach (var item in items)
            {
                FileSystemItem fsi = new MS.FileServices.File();
                fsi.Id = item.UniqueId.ToString();
                fsi.Name = item.Name;

                result.Add (fsi);
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
            SP.ListItem folder = null;
            // first get a reference to the containing list.
            var query = (DataServiceQuery<SP.List>)
                from list in apiData.Lists.Expand(l => l.Items)
                where list.Title == listTitle
                select list;
            TaskFactory<IEnumerable<SP.List>> taskFactory = new TaskFactory<IEnumerable<SP.List>>();
            IEnumerable<SP.List> listResults = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));
            SP.List listResult = listResults.FirstOrDefault();

            if (listResult != null)
            {
                apiData.UpdateObject(listResult);
                folder = new SP.ListItem();
                folder.DisplayName = folderName;
                folder.Folder = new SP.Folder
                {
                    Name = folderName,
                    ServerRelativeUrl = $"/${listTitle}/${folderName}"
                };
                listResult.Items.Add(folder);
                apiData.AddObject("AccountItem", folder);
                
                // save to SharePoint.
                TaskFactory saveTaskFactory = new TaskFactory();
                var res = await saveTaskFactory.FromAsync(apiData.BeginSaveChanges(null, null), iar => apiData.EndSaveChanges(iar));
            }            

            return folder;
        }

        public async Task DeleteFolder(string listTitle, string folderName)
        {
            // first get a reference to the containing list.
            DataServiceQuery<SP.List> query = (DataServiceQuery<SP.List>)
                from list in apiData.Lists
                where list.Title == listTitle
                select list;
            TaskFactory<IEnumerable<SP.List>> taskFactory = new TaskFactory<IEnumerable<SP.List>>();
            IEnumerable<SP.List> listResults = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));
            SP.List listResult = listResults.FirstOrDefault();

            SP.ListItem folderListItem = listResult.Items.Where(x => x.DisplayName == folderName).FirstOrDefault();
            if (folderListItem != null)
            {
                listResult.Items.Remove(folderListItem);
            }

            TaskFactory saveTaskFactory = new TaskFactory();
            await saveTaskFactory.FromAsync(listData.BeginSaveChanges(null, null), iar => listData.EndSaveChanges(iar));


        }

        public async Task<bool> FolderExists(string listTitle, string folderName)
        {
            bool result = false;
            // first get a reference to the containing list.
            var query = (DataServiceQuery<SP.List>)(
                        from list in apiData.Lists.Expand(l => l.Items)
                          where list.Title == listTitle
                          select list);

            var taskFactory = new TaskFactory<IEnumerable<SP.List>>();
            IEnumerable<SP.List> listResults = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));
            var listResult = listResults.FirstOrDefault();
            if (listResult != null)
            {
                result = listResult.Items.Any(i => i.Folder.Name == folderName);
            }

            return result;
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
			// TODO this currently fails with:
			// The property 'DisableGridEditing' does not exist on type 'SP.List'. Make sure to only use property names that are defined by the type.
			// start by ensuring that the folder exists.
            //bool folderExists = await this.FolderExists(DefaultDocumentListTitle, folderName);
            //if (! folderExists)
            //{
            //  var folder =  await this.CreateFolder(DefaultDocumentListTitle, folderName);                
            //}

            // now add the file to the folder.
            string path = $"/{this.WebName}/{DefaultDocumentListTitle}/{folderName}/{fileName}";
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
            var webRequest = System.Net.WebRequest.Create(this.ServerAppIdUri + "/_api/Web/getfilebyserverrelativeurl('"+ url +"')/$value");            
            HttpWebRequest request = (HttpWebRequest)webRequest;
            request.PreAuthenticate = true;
            request.Headers.Add ("Authorization", authenticationResult.CreateAuthorizationHeader());
            request.Accept = "*";
                        
            // we need to add authentication to a HTTP Client to fetch the file.
            using (
                MemoryStream ms = new MemoryStream())
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
