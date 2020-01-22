using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using grpc = global::Grpc.Core;

namespace Gov.Lclb.Cllb.Services.FileManager {
  /// <summary>
  /// The files service definition.
  /// </summary>
  public static partial class FileManager{
        public const string DefaultDocumentListTitle = "Account";
        public const string AccountDocumentUrlTitle = "account";
        public const string ApplicationDocumentListTitle = "Application";
        public const string ApplicationDocumentUrlTitle = "adoxio_application";
        public const string ContactDocumentListTitle = "contact";
        public const string WorkertDocumentListTitle = "Worker Qualification";
        public const string WorkerDocumentUrlTitle = "adoxio_worker";

        public static void CreateFolderIfNotExist (this FileManagerClient _fileManagerClient, ILogger _logger, string entityName, string folderName)
        {
            try
            {
                
                var createFolderRequest = new CreateFolderRequest()
                {
                    EntityName = entityName,
                    FolderName = folderName
                };

                var createFolderResult = _fileManagerClient.CreateFolder(createFolderRequest);

                if (createFolderResult.ResultStatus == ResultStatus.Fail)
                {
                    _logger.LogError($"Error creating folder for entity {entityName} and folder {folderName}. Error is {createFolderResult.ErrorDetail}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error creating folder for account {entityName} and folder {folderName}");
            }
        }


        public static List<Public.ViewModels.FileSystemItem> GetFileDetailsListInFolder(this FileManagerClient _fileManagerClient, ILogger _logger, string entityName, string folderName)
        {            
                List<Public.ViewModels.FileSystemItem> fileSystemItemVMList = new List<Public.ViewModels.FileSystemItem>();


                try
                {
                    // call the web service
                    var request = new FolderFilesRequest()
                    {
                        DocumentType = null,
                        EntityId = null,
                        EntityName = entityName,
                        FolderName = folderName
                    };

                    var result = _fileManagerClient.FolderFiles(request);

                    if (result.ResultStatus == ResultStatus.Success)
                    {
                        // convert the results to the view model.
                        foreach (var fileDetails in result.Files)
                        {
                        Public.ViewModels.FileSystemItem fileSystemItemVM = new Public.ViewModels.FileSystemItem()
                            {
                                // remove the document type text from file name
                                name = fileDetails.Name.Substring(fileDetails.Name.IndexOf("__") + 2),
                                // convert size from bytes (original) to KB
                                size = fileDetails.Size,
                                serverrelativeurl = fileDetails.ServerRelativeUrl,
                                //timelastmodified = fileDetails.TimeLastModified.ToDateTime(),
                                documenttype = fileDetails.DocumentType
                            };

                            fileSystemItemVMList.Add(fileSystemItemVM);
                        }

                    }
                    else
                    {
                        _logger.LogError($"ERROR in getting folder files for entity {entityName}");
                    }



                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting SharePoint File List");
                }

                return fileSystemItemVMList;
                
        }
    }
}
