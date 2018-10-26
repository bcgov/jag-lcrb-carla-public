using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.SpdSync;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using SpdSync.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.Print("SendSharepointCheckerJob");
            hangfireContext.WriteLine("Starting Sharepoint Checker Job.");

            // If folder does not exist create folder.
            var folderExists = await _sharePointFileManager.DocumentLibraryExists(SharePointDocumentTitle);
            if (!folderExists)
            {
                hangfireContext.WriteLine("No directory found. Creating directory.");
                await _sharePointFileManager.CreateDocumentLibrary(SharePointDocumentTitle);
                await _sharePointFileManager.CreateFolder(SharePointDocumentTitle, SharePointFolderName);
                hangfireContext.WriteLine("End of Sharepoint Checker Job.");
                return;
            }

            List<FileSystemItem> fileSystemItemVMList = await getFileDetailsListInFolder(hangfireContext);
           
            // Look for files with unprocessed name
            hangfireContext.WriteLine("Looking for unprocessed files.");
            var unprocessedFiles = fileSystemItemVMList.Where(f => !f.name.StartsWith("processed_")).ToList();
            foreach (var file in unprocessedFiles)
            {
                // Download file
                hangfireContext.WriteLine("File found. Downloading file.");
                byte[] fileContents = await _sharePointFileManager.DownloadFile(file.serverrelativeurl);

                // Update worker
                hangfireContext.WriteLine("Updating worker.");
                string data = System.Text.Encoding.Default.GetString(fileContents);
                List<WorkerResponse> parsedData = WorkerResponseParser.ParseWorkerResponse(data);

                foreach (var spdResponse in parsedData)
                {
                    try
                    {
                        await UpdateSecurityClearance(hangfireContext, spdResponse, spdResponse.RecordIdentifier);
                    }
                    catch (SharePointRestException spre)
                    {
                        hangfireContext.WriteLine("Unable to get Sharepoint File List.");
                        hangfireContext.WriteLine("Request:");
                        hangfireContext.WriteLine(spre.Request.Content);
                        hangfireContext.WriteLine("Response:");
                        hangfireContext.WriteLine(spre.Response.Content);
                        throw spre;
                    }
                }

                // Rename file
                hangfireContext.WriteLine("Finished processing file.");
                string newserverrelativeurl = "";
                int index = file.serverrelativeurl.LastIndexOf("/");
                if (index > 0)
                    newserverrelativeurl = file.serverrelativeurl.Substring(0, index);
                newserverrelativeurl += "/" + "processed_" + file.name;

                try
                {
                    await _sharePointFileManager.RenameFile(file.serverrelativeurl, newserverrelativeurl);
                }
                catch (SharePointRestException spre)
                {
                    hangfireContext.WriteLine("Unable to rename file.");
                    hangfireContext.WriteLine("Request:");
                    hangfireContext.WriteLine(spre.Request.Content);
                    hangfireContext.WriteLine("Response:");
                    hangfireContext.WriteLine(spre.Response.Content);
                    throw spre;
                }
            }

            hangfireContext.WriteLine("End of Sharepoint Checker Job.");
        }

        private async Task<List<FileSystemItem>> getFileDetailsListInFolder(PerformContext hangfireContext)
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
                hangfireContext.WriteLine("Unable to get Sharepoint File List.");
                hangfireContext.WriteLine("Request:");
                hangfireContext.WriteLine(spre.Request.Content);
                hangfireContext.WriteLine("Response:");
                hangfireContext.WriteLine(spre.Response.Content);
                throw spre;
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

        public async Task UpdateSecurityClearance(PerformContext hangfireContext, WorkerResponse spdResponse, string id)
        {
            var filter = "adoxio_workerjobnumber eq '" + id + "'";
            List<string> expand = new List<string> { "adoxio_WorkerId" };
            MicrosoftDynamicsCRMadoxioPersonalhistorysummary response = null;
            try
            {
               response =  _dynamics.Personalhistorysummaries.Get(filter: filter).Value.FirstOrDefault();
            }
            catch (OdataerrorException odee)
            {
                hangfireContext.WriteLine("Unable to get personal history summary.");
                hangfireContext.WriteLine("Request:");
                hangfireContext.WriteLine(odee.Request.Content);
                hangfireContext.WriteLine("Response:");
                hangfireContext.WriteLine(odee.Response.Content);
                throw odee;
            }

            MicrosoftDynamicsCRMadoxioWorker patchWorker = new MicrosoftDynamicsCRMadoxioWorker
            {
                SecurityStatus = (int) Enum.Parse(typeof(SecurityStatusPicklist), spdResponse.Result, true),
                SecurityCompletedOn = spdResponse.DateProcessed
            };
            MicrosoftDynamicsCRMadoxioPersonalhistorysummary patchPHS = new MicrosoftDynamicsCRMadoxioPersonalhistorysummary
            {
                AdoxioSecuritystatus = (int)Enum.Parse(typeof(SecurityStatusPicklist), spdResponse.Result, true),
                AdoxioCompletedon = spdResponse.DateProcessed
            };

            if (patchWorker != null)
            {
                try
                {
                    await _dynamics.Workers.UpdateAsync(response._adoxioWorkeridValue, patchWorker);
                    await _dynamics.Personalhistorysummaries.UpdateAsync(response.AdoxioPersonalhistorysummaryid, patchPHS);
                }
                catch (OdataerrorException odee)
                {
                    hangfireContext.WriteLine("Unable to patch worker or personal history summary.");
                    hangfireContext.WriteLine("Request:");
                    hangfireContext.WriteLine(odee.Request.Content);
                    hangfireContext.WriteLine("Response:");
                    hangfireContext.WriteLine(odee.Response.Content);
                    throw odee;
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
