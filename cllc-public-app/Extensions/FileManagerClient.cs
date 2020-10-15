using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using grpc = global::Grpc.Core;
using Google.Protobuf;
using System.Diagnostics.Contracts;

namespace Gov.Lclb.Cllb.Services.FileManager
{
    /// <summary>
    /// The files service definition.
    /// </summary>
    public static partial class FileManager
    {
        public const string DefaultDocumentListTitle = "Account";
        public const string AccountDocumentUrlTitle = "account";
        public const string ApplicationDocumentListTitle = "Application";
        public const string ApplicationDocumentUrlTitle = "adoxio_application";
        public const string ContactDocumentListTitle = "contact";
        public const string WorkerDocumentListTitle = "Worker Qualification";
        public const string WorkerDocumentUrlTitle = "adoxio_worker";

        public static void CreateFolderIfNotExist(this FileManagerClient _fileManagerClient, ILogger _logger, string entityName, string folderName)
        {
            string logFolderName = WordSanitizer.Sanitize(folderName);
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
                    _logger.LogError($"Error creating folder for entity {entityName} and folder {logFolderName}. Error is {createFolderResult.ErrorDetail}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error creating folder for account {entityName} and folder {logFolderName}");
            }
        }


        public static List<Public.ViewModels.FileSystemItem> GetFileDetailsListInFolder(this FileManagerClient _fileManagerClient, ILogger _logger, string entityName, string entityId, string folderName)
        {
            List<Public.ViewModels.FileSystemItem> fileSystemItemVMList = new List<Public.ViewModels.FileSystemItem>();


            try
            {
                // call the web service
                var request = new FolderFilesRequest()
                {
                    DocumentType = "",
                    EntityId = entityId,
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

        public static void UploadHashedPdf(this FileManagerClient _fileManagerClient, ILogger _logger, string entityName, string entityId, string folderName, string documentType, byte[] data)
        {
            Contract.Requires(_fileManagerClient != null);
            Contract.Requires(documentType != null);

            var hash = HashUtility.GetSHA256(data);
            var filename = FileSystemItemExtensions.CombineNameDocumentType($"{hash}.pdf", documentType);

            // Abort early if PDF hasn't changed...
            if (_fileManagerClient.FileExistsByHash(_logger, entityName, entityId, folderName, documentType, hash))
            {
                return;
            }

            // call the web service
            var uploadRequest = new UploadFileRequest
            {
                ContentType = "application/pdf",
                Data = ByteString.CopyFrom(data),
                EntityName = entityName,
                FileName = filename,
                FolderName = folderName
            };

            var uploadResult = _fileManagerClient.UploadFile(uploadRequest);

            // Do not save full file names to the logs (for privacy)
            var logFolderName = WordSanitizer.Sanitize(folderName);
            var logFileName = WordSanitizer.Sanitize(filename);

            if (uploadResult.ResultStatus == ResultStatus.Success)
            {
                _logger.LogInformation($"SUCCESS in uploading PDF file {logFileName} to folder {logFolderName}");
            }
            else
            {
                _logger.LogError($"ERROR in uploading PDF file {logFileName} to folder {logFolderName}");
                throw new Exception($"ERROR in uploading PDF file {logFileName} to folder {logFolderName}");
            }
        }

        public static bool FileExistsByHash(this FileManagerClient _fileManagerClient, ILogger _logger, string entityName, string entityId, string folderName, string documentType, string hash)
        {
            Contract.Requires(_fileManagerClient != null);

            var filename = FileSystemItemExtensions.CombineNameDocumentType($"{hash}.pdf", documentType);
            var exists = false;
            try
            {
                // call the web service
                var request = new FolderFilesRequest()
                {
                    DocumentType = documentType,
                    EntityId = entityId,
                    EntityName = entityName,
                    FolderName = folderName
                };

                var result = _fileManagerClient.FolderFiles(request);

                if (result.ResultStatus == ResultStatus.Success)
                {
                    exists = result.Files.Any(f => FileSystemItemExtensions.GetDocumentName(f.Name) == filename);
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
            return exists;
        }
    }
}
