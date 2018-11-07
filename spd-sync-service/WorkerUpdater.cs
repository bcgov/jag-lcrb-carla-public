using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.SpdSync;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        public ILogger _logger { get; }

        private static readonly HttpClient Client = new HttpClient();
        private readonly string SharePointDocumentTitle = "spd_worker";
        private readonly string SharePointFolderName = "SPD Worker Files";
        private SharePointFileManager _sharePointFileManager;
        private IConfiguration Configuration { get; }
        private IDynamicsClient _dynamics;

        public WorkerUpdater(IConfiguration Configuration, ILoggerFactory loggerFactory, SharePointFileManager sharePointFileManager)
        {
            this.Configuration = Configuration;
            _logger = loggerFactory.CreateLogger(typeof(WorkerUpdater));            
            _dynamics = SpdUtils.SetupDynamics(Configuration);
            _sharePointFileManager = sharePointFileManager;
        }

        /// <summary>
        /// Hangfire job to send an export to SPD.
        /// </summary>
        public async Task ReceiveImportJob(PerformContext hangfireContext)
        {
            hangfireContext.WriteLine("Starting Sharepoint Checker Job.");
            _logger.LogError("Starting Sharepoint Checker Job.");

            // If folder does not exist create folder.
            var folderExists = await _sharePointFileManager.DocumentLibraryExists(SharePointDocumentTitle);
            if (!folderExists)
            {
                hangfireContext.WriteLine("No directory found. Creating directory.");
                _logger.LogError("No directory found. Creating directory.");
                await _sharePointFileManager.CreateDocumentLibrary(SharePointDocumentTitle);
                await _sharePointFileManager.CreateFolder(SharePointDocumentTitle, SharePointFolderName);
                hangfireContext.WriteLine("End of Sharepoint Checker Job.");
                _logger.LogError("End of Sharepoint Checker Job.");
            }
            else
            {
                List<FileSystemItem> fileSystemItemVMList = await getFileDetailsListInFolder(hangfireContext);

                // Look for files with unprocessed name
                hangfireContext.WriteLine("Looking for unprocessed files.");
                _logger.LogError("Looking for unprocessed files.");
                var unprocessedFiles = fileSystemItemVMList.Where(f => !f.name.StartsWith("processed_")).ToList();
                foreach (var file in unprocessedFiles)
                {
                    // Skip if file is not .csv
                    if(Path.GetExtension(file.name) != ".csv")
                    {
                        continue;
                    }

                    // Download file
                    hangfireContext.WriteLine("File found. Downloading file.");
                    _logger.LogError("File found. Downloading file.");
                    byte[] fileContents = await _sharePointFileManager.DownloadFile(file.serverrelativeurl);

                    // Update worker
                    hangfireContext.WriteLine("Updating worker.");
                    _logger.LogError("Updating worker.");
                    string data = System.Text.Encoding.Default.GetString(fileContents);
                    List<WorkerResponse> parsedData = WorkerResponseParser.ParseWorkerResponse(data, _logger);

                    foreach (var spdResponse in parsedData)
                    {
                        try
                        {
                            await UpdateSecurityClearance(hangfireContext, spdResponse, spdResponse.RecordIdentifier);
                        }
                        catch (SharePointRestException spre)
                        {
                            hangfireContext.WriteLine("Unable to update security clearance status due to SharePoint.");
                            hangfireContext.WriteLine("Request:");
                            hangfireContext.WriteLine(spre.Request.Content);
                            hangfireContext.WriteLine("Response:");
                            hangfireContext.WriteLine(spre.Response.Content);
                            
                            _logger.LogError("Unable to update security clearance status due to SharePoint.");
                            _logger.LogError("Request:");
                            _logger.LogError(spre.Request.Content);
                            _logger.LogError("Response:");
                            _logger.LogError(spre.Response.Content);
                            continue;
                        }
                        catch (Exception e)
                        {
                            hangfireContext.WriteLine("Unable to update security clearance status.");
                            hangfireContext.WriteLine("Message:");
                            hangfireContext.WriteLine(e.Message);

                            _logger.LogError("Unable to update security clearance status.");
                            _logger.LogError("Message:");
                            _logger.LogError(e.Message);
                            continue;
                        }
                    }

                    // Rename file
                    hangfireContext.WriteLine("Finished processing file.");
                    _logger.LogError("Finished processing file.");
                    string newserverrelativeurl = "";
                    int index = file.serverrelativeurl.LastIndexOf("/");
                    if (index > 0)
                    {
                        newserverrelativeurl = file.serverrelativeurl.Substring(0, index);
                        newserverrelativeurl += "/" + "processed_" + file.name;
                    }

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

                        _logger.LogError("Unable to rename file.");
                        _logger.LogError("Message:");
                        _logger.LogError(spre.Message);
                        throw spre;
                    }
                }
            }
            hangfireContext.WriteLine("End of Sharepoint Checker Job.");
            _logger.LogError("End of Sharepoint Checker Job.");
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

                _logger.LogError("Unable to get Sharepoint File List.");
                _logger.LogError("Request:");
                _logger.LogError(spre.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(spre.Request.Content);
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

                _logger.LogError("Unable to get personal history summary.");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Request.Content);
                throw odee;
            }

            MicrosoftDynamicsCRMadoxioPersonalhistorysummary patchPHS = new MicrosoftDynamicsCRMadoxioPersonalhistorysummary
            {
                AdoxioSecuritystatus = (int)Enum.Parse(typeof(SecurityStatusPicklist), spdResponse.Result, true),
                AdoxioCompletedon = spdResponse.DateProcessed
            };

            if (patchPHS != null)
            {
                try
                {
                    await _dynamics.Personalhistorysummaries.UpdateAsync(response.AdoxioPersonalhistorysummaryid, patchPHS);
                }
                catch (OdataerrorException odee)
                {
                    hangfireContext.WriteLine("Unable to patch personal history summary.");
                    hangfireContext.WriteLine("Request:");
                    hangfireContext.WriteLine(odee.Request.Content);
                    hangfireContext.WriteLine("Response:");
                    hangfireContext.WriteLine(odee.Response.Content);

                    _logger.LogError("Unable to patch personal history summary.");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Request.Content);
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
