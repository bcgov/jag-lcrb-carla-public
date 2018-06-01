using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Test;
using Microsoft.Extensions.Configuration;
using MS.FileServices;
using SP.Data;
using System;
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
                .AddUserSecrets<ApiIntegrationTestBase>() // Add secrets from the cllc-public-app
                .AddEnvironmentVariables()
                .Build();

            serverAppIdUri = Configuration["SHAREPOINT_SERVER_APPID_URI"];
            string webname = Configuration["SHAREPOINT_WEBNAME"];
            string aadTenantId = Configuration["SHAREPOINT_AAD_TENANTID"];
            string clientId = Configuration["SHAREPOINT_CLIENT_ID"];
            string certFileName = Configuration["SHAREPOINT_CERTIFICATE_FILENAME"];
            string certPassword = Configuration["SHAREPOINT_CERTIFICATE_PASSWORD"];

            sharePointFileManager = new SharePointFileManager(serverAppIdUri, webname, aadTenantId, clientId, certFileName, certPassword);

        }

        /// <summary>
        /// Test GetFiles
        /// </summary>
        [Fact]
        public async void GetFilesTest()
        {
            var files = await sharePointFileManager.GetFiles();
            Assert.True(files != null);
        }

        /// <summary>
        /// Test GetFile
        /// </summary>
        [Fact]
        public async void GetFileTest()
        {
            var files = await sharePointFileManager.GetFiles();
            foreach (var file in files)
            {
                var temp = sharePointFileManager.GetFile(file.Url);
                Assert.True(temp != null);
            }

            Assert.True(files != null);
        }


        /// <summary>
        /// Test GetFile
        /// </summary>
        [Fact]
        public async void DownloadFileTest()
        {
            var files = await sharePointFileManager.GetFiles();
            foreach (var file in files)
            {
                var temp = sharePointFileManager.DownloadFile(file.Url);
                Assert.True(temp != null);

            }

            Assert.True(files != null);
        }

        [Fact]
        public async void AddRemoveFilesTest()
        {
            string name = "test-name";            
            string path = "/cannabisdev/Shared%20Documents/" + name;
            string url = serverAppIdUri + "/cannabisdev/Shared Documents/" + name;

            string title = "test-title";
            
            string contentType = "text/plain";
            
            string testData = "This is just a test.";

            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            await sharePointFileManager.UploadFile(name, path, fileData, contentType, path);

            // now delete it.

            await sharePointFileManager.DeleteFile(url);
        }



    }
}
