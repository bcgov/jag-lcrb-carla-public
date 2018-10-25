using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.SpdSync;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using SpdSync.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Gov.Lclb.Cllb.Interfaces.SharePointFileManager;

namespace SpdSync
{
    public class WorkerUpdater
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly string SharePointDocumentTitle = "spd_worker";
        private readonly string SharePointFolderName = "SPD Worker Files";
        private IConfiguration Configuration { get; }

        private IDynamicsClient _dynamics;
        private SharePointFileManager _sharePointFileManager;

        public WorkerUpdater(IConfiguration Configuration, SharePointFileManager sharePointFileManager)
        {
            this.Configuration = Configuration;
            _dynamics = SpdUtils.SetupDynamics(Configuration);
            _sharePointFileManager = sharePointFileManager;
        }

        /// <summary>
        /// Hangfire job to send an export to SPD.
        /// </summary>
        public async Task SendSharepointCheckerJob(PerformContext hangfireContext)
        {
            //hangfireContext.WriteLine("Starting Sharepoint Checker Job.");

            // If folder does not exist create folder.
            var folderExists = await _sharePointFileManager.DocumentLibraryExists(SharePointDocumentTitle);
            if (!folderExists)
            {
                //hangfireContext.WriteLine("No directory found. Creating directory.");
                await _sharePointFileManager.CreateDocumentLibrary(SharePointDocumentTitle);
                await _sharePointFileManager.CreateFolder(SharePointDocumentTitle, SharePointFolderName);
                //hangfireContext.WriteLine("End of Sharepoint Checker Job.");
                return;
            }
            List<FileSystemItem> fileSystemItemVMList = await getFileDetailsListInFolder();

            // Look for files with unprocessed name
            var unprocessedFiles = fileSystemItemVMList.Where(f => !f.name.StartsWith("processed_")).ToList();
            unprocessedFiles.ForEach(async file =>
            {

                // Download file
                byte[] fileContents = await _sharePointFileManager.DownloadFile(file.serverrelativeurl);

                // Update worker
                string data = System.Text.Encoding.Default.GetString(fileContents);
                List<WorkerResponse> parsedData = WorkerResponseParser.ParseWorkerResponse(data);
                parsedData.ForEach(async spdResponse =>
                {
                    await UpdateWorker(spdResponse, spdResponse.RecordIdentifier);
                });

                // Reupload with updated name
                string newFileName = "processed_" + file.name;
                try
                {
                    await _sharePointFileManager.AddFile(SharePointDocumentTitle, SharePointFolderName, newFileName, new MemoryStream(fileContents), null);
                }
                catch (SharePointRestException ex)
                {
                    //_logger.LogError("Error uploading file to SharePoint");
                    //_logger.LogError(ex.Response.Content);
                    //_logger.LogError(ex.Message);
                }

                // Delete file
                await _sharePointFileManager.DeleteFile(file.serverrelativeurl);
            });



            // Else end job
            //hangfireContext.WriteLine("End of Sharepoint Checker Job.");
        }

        private async Task<List<FileSystemItem>> getFileDetailsListInFolder()
        {
            List<FileSystemItem> fileSystemItemVMList = new List<FileSystemItem>();

            // Get the file details list in folder
            List<FileDetailsList> fileDetailsList = null;
            try
            {
                fileDetailsList = await _sharePointFileManager.GetFileDetailsListInFolder(SharePointDocumentTitle, SharePointFolderName, "");
            }
            catch (SharePointRestException spre)
            {
                //_logger.LogError("Error getting SharePoint File List");
                //_logger.LogError("Request URI:");
                //_logger.LogError(spre.Request.RequestUri.ToString());
                //_logger.LogError("Response:");
                //_logger.LogError(spre.Response.Content);
                throw new Exception("Unable to get Sharepoint File List.");
            }

            if (fileDetailsList != null)
            {
                foreach (FileDetailsList fileDetails in fileDetailsList)
                {
                    FileSystemItem fileSystemItemVM = new FileSystemItem()
                    {
                        // remove the document type text from file name
                        name = fileDetails.Name,
                        // convert size from bytes (original) to KB
                        size = int.Parse(fileDetails.Length),
                        serverrelativeurl = fileDetails.ServerRelativeUrl,
                        timelastmodified = DateTime.Parse(fileDetails.TimeLastModified),
                        documenttype = fileDetails.DocumentType
                    };

                    fileSystemItemVMList.Add(fileSystemItemVM);
                }
            }

            return fileSystemItemVMList;
        }

        public async Task UpdateWorker(WorkerResponse spdResponse, string id)
        {
            var filter = "adoxio_workerjobnumber eq " + id;
            List<string> expand = new List<string> { "adoxio_WorkerId" };
            MicrosoftDynamicsCRMadoxioPersonalhistorysummary response = null;
            try
            {
               response =  _dynamics.Personalhistorysummaries.Get(filter: filter).Value.FirstOrDefault();
            }
            catch (OdataerrorException odee)
            {
                var x = odee.Request.Content;
                //_logger.LogError("Error updating contact");
                //_logger.LogError("Request:");
                //_logger.LogError(odee.Request.Content);
                //_logger.LogError("Response:");
                //_logger.LogError(odee.Response.Content);
            }
            MicrosoftDynamicsCRMadoxioWorker patchWorker = new MicrosoftDynamicsCRMadoxioWorker
            {
                SecurityStatus = spdResponse.Result,
                SecurityCompletedOn = spdResponse.DateProcessed

            };

            if (patchWorker != null)
            {
                try
                {
                    await _dynamics.Workers.UpdateAsync(response._adoxioWorkeridValue, patchWorker);
                }
                catch (OdataerrorException odee)
                {
                    //_logger.LogError("Error updating contact");
                    //_logger.LogError("Request:");
                    //_logger.LogError(odee.Request.Content);
                    //_logger.LogError("Response:");
                    //_logger.LogError(odee.Response.Content);
                }
            }
        }
    }

    public class FileSystemItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string documenttype { get; set; }
        public int size { get; set; }
        public string serverrelativeurl { get; set; }
        public DateTime timecreated { get; set; }
        public DateTime timelastmodified { get; set; }
    }
}
