
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;


namespace SharePoint.Tests
{
    public class SharePoint 
    {

        IConfiguration Configuration;

        SharePointFileManager sharePointFileManager;

        string serverAppIdUri;

        /// <summary>
        /// Setup the test
        /// </summary>        
        public SharePoint()
        {
            Configuration = new ConfigurationBuilder()
                // The following line is the only reason we have a project reference for cllc-public-app.
                // If you were to use this code on a different project simply add user secrets as appropriate to match the environment / secret variables below.
                .AddUserSecrets<Startup>() // Add secrets from the cllc-public-app
                .AddEnvironmentVariables()
                .Build();

            serverAppIdUri = Configuration["SHAREPOINT_SERVER_APPID_URI"];

            sharePointFileManager = new SharePointFileManager(Configuration);

        }


        [Fact]
        public async void UploadRemoveFilesTest()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string name = "test-name" + rnd.Next() +".txt";
            string testFolder = "test-folder" + rnd.Next();
            string listTitle = "Shared Documents";
            string url = serverAppIdUri + "/cannabisdev/Shared Documents/" + testFolder + "/" + name;
            
            string contentType = "text/plain";
            
            string testData = "This is just a test.";

            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            await sharePointFileManager.CreateFolder(listTitle, testFolder);

            await sharePointFileManager.UploadFile(name, "Shared Documents",testFolder, fileData, contentType);

            // now delete it.

            await sharePointFileManager.DeleteFile("Shared Documents", testFolder, name);

            // cleanup the test folder.
            await sharePointFileManager.DeleteFolder("Shared Documents", testFolder);
        }

        [Fact]
        public async void FolderNameTest()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string name = "test-name" + rnd.Next() + ".txt";
            string testFolder = "O'Test " + rnd.Next();
            string listTitle = "Shared Documents";
            string url = serverAppIdUri + "/cannabisdev/Shared Documents/" + testFolder + "/" + name;

            string contentType = "text/plain";

            string testData = "This is just a test.";

            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            await sharePointFileManager.CreateFolder(listTitle, testFolder);

            await sharePointFileManager.UploadFile(name, "Shared Documents", testFolder, fileData, contentType);

            // now delete it.

            await sharePointFileManager.DeleteFile("Shared Documents", testFolder, name);

            // cleanup the test folder.
            await sharePointFileManager.DeleteFolder("Shared Documents", testFolder);
        }

        [Fact]
        public async void AddRemoveListFilesTest()
        {
            // set file and folder settings

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string documentType = "Document Type";
            string fileName = documentType + "__" + "test-file-name" + rnd.Next() + ".txt";
            string folderName = "test-folder-name" + rnd.Next();
            string path = "/";
            if (! string.IsNullOrEmpty (sharePointFileManager.WebName) )
            {
                path += $"{sharePointFileManager.WebName}/";
            }
            path += SharePointFileManager.DefaultDocumentListTitle + "/" + folderName + "/" + fileName;
            string contentType = "text/plain";
            string testData = "This is just a test.";
            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            // add file to SP

            await sharePointFileManager.AddFile(folderName, fileName, fileData, contentType);

            // get file details list in SP folder

            List<Gov.Lclb.Cllb.Interfaces.SharePointFileManager.FileDetailsList> fileDetailsList = await sharePointFileManager.GetFileDetailsListInFolder(SharePointFileManager.DefaultDocumentListTitle, folderName, documentType);
            //only one file should be returned
            Assert.Single(fileDetailsList);
            // validate that file name uploaded and listed are the same
            string serverRelativeUrl = null;
            foreach (var fileDetails in fileDetailsList)
            {
                Assert.Equal(fileName, fileDetails.Name);
                serverRelativeUrl = fileDetails.ServerRelativeUrl;
            }
            
            // verify that we can download the same file.

            byte[] data = await sharePointFileManager.DownloadFile(path);
            string stringData = System.Text.Encoding.ASCII.GetString(data);
            Assert.Equal(stringData, testData);

            // delete file from SP

            await sharePointFileManager.DeleteFile(SharePointFileManager.DefaultDocumentUrlTitle, folderName, fileName);

            // delete folder from SP

            await sharePointFileManager.DeleteFolder(SharePointFileManager.DefaultDocumentUrlTitle, folderName);
        }


        [Fact]
        public async void LongFilenameTest()
        {
            // set file and folder settings

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string documentType = "Document Type";

            int maxLen = 255 - 4; // Windows allows for a maximum of 255 characters for a given file; subtract 4 for the extension.

            string fileName = documentType + "__" + "test-file-name"; 
            maxLen -= fileName.Length;
            for (int i = 0; i < maxLen; i++)
            {
                string r = rnd.Next().ToString();
                fileName += r[0];
            }                

            fileName += ".txt";

            string folderName = "test-folder-name" + rnd.Next();
            string path = "/";
            if (!string.IsNullOrEmpty(sharePointFileManager.WebName))
            {
                path += $"{sharePointFileManager.WebName}/";
            }
            
            string contentType = "text/plain";
            string testData = "This is just a test.";
            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            // add file to SP

            fileName = await sharePointFileManager.AddFile(folderName, fileName, fileData, contentType);

            path += SharePointFileManager.DefaultDocumentListTitle + "/" + folderName + "/" + fileName;

            // get file details list in SP folder

            List<Gov.Lclb.Cllb.Interfaces.SharePointFileManager.FileDetailsList> fileDetailsList = await sharePointFileManager.GetFileDetailsListInFolder(SharePointFileManager.DefaultDocumentListTitle, folderName, documentType);
            //only one file should be returned
            Assert.Single(fileDetailsList);
            // validate that file name uploaded and listed are the same
            string serverRelativeUrl = null;
            foreach (var fileDetails in fileDetailsList)
            {
                Assert.Equal(fileName, fileDetails.Name);
                serverRelativeUrl = fileDetails.ServerRelativeUrl;
            }

            // verify that we can download the same file.

            byte[] data = await sharePointFileManager.DownloadFile(path);
            string stringData = System.Text.Encoding.ASCII.GetString(data);
            Assert.Equal(stringData, testData);

            // delete file from SP

            await sharePointFileManager.DeleteFile(SharePointFileManager.DefaultDocumentUrlTitle, folderName, fileName);

            // delete folder from SP

            await sharePointFileManager.DeleteFolder(SharePointFileManager.DefaultDocumentUrlTitle, folderName);
        }


        [Fact]
        public async void FileNameWithApostropheTest()
        {
            // set file and folder settings

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string documentType = "Document Type";
            string fileName = documentType + "__" + "test-'-name" + rnd.Next() + ".txt";
            string folderName = "test-folder-name" + rnd.Next();
            string path = "/" + sharePointFileManager.WebName + "/" + SharePointFileManager.DefaultDocumentListTitle + "/" + folderName + "/" + fileName;
            string url = serverAppIdUri + sharePointFileManager.WebName + "/" + SharePointFileManager.DefaultDocumentListTitle + "/" + folderName + "/" + fileName;
            string contentType = "text/plain";
            string testData = "This is just a test.";
            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            // add file to SP

            await sharePointFileManager.AddFile(folderName, fileName, fileData, contentType);

            // get file details list in SP folder

            List<Gov.Lclb.Cllb.Interfaces.SharePointFileManager.FileDetailsList> fileDetailsList = await sharePointFileManager.GetFileDetailsListInFolder(SharePointFileManager.DefaultDocumentListTitle, folderName, documentType);
            //only one file should be returned
            Assert.Single(fileDetailsList);
            // validate that file name uploaded and listed are the same
            foreach (Gov.Lclb.Cllb.Interfaces.SharePointFileManager.FileDetailsList fileDetails in fileDetailsList)
            {
                Assert.Equal(fileName, fileDetails.Name);
            }

            // verify that we can download the same file.

            byte[] data = await sharePointFileManager.DownloadFile(path);
            string stringData = System.Text.Encoding.ASCII.GetString(data);
            Assert.Equal(stringData, testData);

            // delete file from SP

            await sharePointFileManager.DeleteFile(SharePointFileManager.DefaultDocumentListTitle, folderName, fileName);

            // delete folder from SP

            await sharePointFileManager.DeleteFolder(SharePointFileManager.DefaultDocumentListTitle, folderName);
        }


        /// <summary>
        /// Test Create Folder
        /// </summary>
        [Fact]
        public async void InvalidFolderDoesNotExist()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string documentLocation = "Account";
            string folderName = "Test Folder" + rnd.Next() + "---" + rnd.Next();

            
            bool exists = await sharePointFileManager.FolderExists(documentLocation, folderName);

            Assert.False(exists);

        }

        /// <summary>
        /// Test Create Folder
        /// </summary>
        [Fact]
        public async void CreateFolderTest()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string folderName = "Test-Folder-" + rnd.Next();

            await sharePointFileManager.CreateFolder(SharePointFileManager.DefaultDocumentUrlTitle, folderName);


            bool exists = await sharePointFileManager.FolderExists(SharePointFileManager.DefaultDocumentUrlTitle, folderName);

            Assert.True(exists);


            await sharePointFileManager.DeleteFolder(SharePointFileManager.DefaultDocumentUrlTitle, folderName);

            exists = await sharePointFileManager.FolderExists(SharePointFileManager.DefaultDocumentUrlTitle, folderName);

            Assert.False(exists);
        }

        [Fact]
        public async void GetFilesInEmptyFolderTest()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string folderName = "Test Folder" + rnd.Next();
            string documentType = "Corporate Information";
            await sharePointFileManager.CreateFolder(SharePointFileManager.DefaultDocumentListTitle, folderName);
            
            var files = await sharePointFileManager.GetFileDetailsListInFolder(SharePointFileManager.DefaultDocumentListTitle, folderName, documentType);
            Assert.True(files != null);
            Assert.True(files.Count == 0);
            await sharePointFileManager.DeleteFolder(SharePointFileManager.DefaultDocumentListTitle, folderName);            
        }

        [Fact]
        public async void GetFilesInPopulatedFolderTest()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string folderName = "Test Folder" + rnd.Next();
            string documentType = "Corporate Information";
            await sharePointFileManager.CreateFolder(SharePointFileManager.DefaultDocumentListTitle, folderName);

            string fileName = documentType + "__" + "test-file-name" + rnd.Next() + ".txt";
            string contentType = "text/plain";

            string testData = "This is just a test.";
            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            // add file to SP

            await sharePointFileManager.AddFile(folderName, fileName, fileData, contentType);

            // get file details list in SP folder

            List<Gov.Lclb.Cllb.Interfaces.SharePointFileManager.FileDetailsList> fileDetailsList = await sharePointFileManager.GetFileDetailsListInFolder(SharePointFileManager.DefaultDocumentListTitle, folderName, documentType);
            //only one file should be returned
            Assert.Single(fileDetailsList);
            // validate that file name uploaded and listed are the same
            foreach (var fileDetails in fileDetailsList)
            {
                Assert.Equal(fileName, fileDetails.Name);
            }                        

            // delete file from SP

            await sharePointFileManager.DeleteFile(SharePointFileManager.DefaultDocumentListTitle, folderName, fileName);


            await sharePointFileManager.DeleteFolder(SharePointFileManager.DefaultDocumentListTitle, folderName);
        }

    }
}
