using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.Extensions.Logging;
using grpc = Grpc.Core;

namespace Gov.Lclb.Cllb.Services.FileManager
{
    /// <summary>
    /// The files service definition.
    /// </summary>
    public static partial class FileManager
    {
        public static void CreateFolderIfNotExist(
            this FileManagerClient _fileManagerClient,
            ILogger _logger,
            string entityName,
            string folderName
        )
        {
            string logFolderName = WordSanitizer.Sanitize(folderName);
            _logger.LogDebug(
                "[FileManagerClient] CreateFolderIfNotExist - called with entityName={EntityName}, folderName={LogFolderName}",
                entityName,
                logFolderName
            );

            try
            {
                var createFolderRequest = new CreateFolderRequest { EntityName = entityName, FolderName = folderName };

                var createFolderResult = _fileManagerClient.CreateFolder(createFolderRequest);

                if (createFolderResult.ResultStatus == ResultStatus.Fail)
                {
                    _logger.LogError(
                        "[FileManagerClient] CreateFolderIfNotExist - Error creating folder for entity {EntityName} and folder {LogFolderName}. Error is {ErrorDetail}",
                        entityName,
                        logFolderName,
                        createFolderResult.ErrorDetail
                    );
                }
                else
                {
                    _logger.LogDebug(
                        "[FileManagerClient] CreateFolderIfNotExist - Successfully created or verified folder for entity {EntityName} and folder {LogFolderName}",
                        entityName,
                        logFolderName
                    );
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "[FileManagerClient] CreateFolderIfNotExist - Exception creating folder for entity {EntityName} and folder {LogFolderName}",
                    entityName,
                    logFolderName
                );
            }
        }

        public static List<Public.ViewModels.FileSystemItem> GetFileDetailsListInFolder(
            this FileManagerClient _fileManagerClient,
            ILogger _logger,
            string entityName,
            string entityId,
            string folderName
        )
        {
            string logFolderName = WordSanitizer.Sanitize(folderName);
            _logger.LogDebug(
                "[FileManagerClient] GetFileDetailsListInFolder - called with entityName={EntityName}, entityId={EntityId}, folderName={LogFolderName}",
                entityName,
                entityId,
                logFolderName
            );

            List<Public.ViewModels.FileSystemItem> fileSystemItemVMList = new List<Public.ViewModels.FileSystemItem>();

            try
            {
                // call the web service
                var request = new FolderFilesRequest
                {
                    DocumentType = "",
                    EntityId = entityId,
                    EntityName = entityName,
                    FolderName = folderName,
                };

                var result = _fileManagerClient.FolderFiles(request);

                if (result.ResultStatus == ResultStatus.Success)
                {
                    _logger.LogDebug(
                        "[FileManagerClient] GetFileDetailsListInFolder - Successfully retrieved {FileCount} files from folder {LogFolderName} for entity {EntityName}",
                        result.Files.Count,
                        logFolderName,
                        entityName
                    );

                    // convert the results to the view model.
                    foreach (var fileDetails in result.Files)
                    {
                        Public.ViewModels.FileSystemItem fileSystemItemVM = new Public.ViewModels.FileSystemItem
                        {
                            // remove the document type text from file name (ex: NOTICE__filename.pdf --> filename.pdf)
                            name = fileDetails.Name.Substring(fileDetails.Name.IndexOf("__") + 2),
                            // convert size from bytes (original) to KB
                            size = fileDetails.Size,
                            serverrelativeurl = fileDetails.ServerRelativeUrl,
                            //timelastmodified = fileDetails.TimeLastModified.ToDateTime(),
                            documenttype = fileDetails.DocumentType,
                        };

                        fileSystemItemVMList.Add(fileSystemItemVM);
                    }
                }
                else
                {
                    _logger.LogError(
                        "[FileManagerClient] GetFileDetailsListInFolder - Error getting folder files for entity {EntityName}, folder {LogFolderName}. Error: {ErrorDetail}",
                        entityName,
                        logFolderName,
                        result.ErrorDetail
                    );
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "[FileManagerClient] GetFileDetailsListInFolder - Exception getting SharePoint File List for entity {EntityName}, folder {LogFolderName}",
                    entityName,
                    logFolderName
                );
            }

            _logger.LogDebug(
                "[FileManagerClient] GetFileDetailsListInFolder - Returning {ItemCount} files",
                fileSystemItemVMList.Count
            );
            return fileSystemItemVMList;
        }

        public static void UploadPdfIfChanged(
            this FileManagerClient _fileManagerClient,
            ILogger _logger,
            string entityName,
            string entityId,
            string folderName,
            string documentType,
            byte[] data,
            string hash
        )
        {
            Contract.Requires(_fileManagerClient != null);
            Contract.Requires(documentType != null);

            var logFolderName = WordSanitizer.Sanitize(folderName);
            _logger.LogDebug(
                "[FileManagerClient] UploadPdfIfChanged - called with entityName={EntityName}, entityId={EntityId}, folderName={LogFolderName}, documentType={DocumentType}, hash={Hash}, dataSize={DataSize}",
                entityName,
                entityId,
                logFolderName,
                documentType,
                hash,
                data?.Length ?? 0
            );

            // SharePoint can truncate file names that are too long. Make sure we account for that.
            var fileName = FileSystemItemExtensions.CombineNameDocumentType($"{hash}.pdf", documentType);
            fileName = _fileManagerClient.GetTruncatedFilename(_logger, entityName, folderName, fileName);

            var notChanged = _fileManagerClient.FileExists(
                _logger,
                entityName,
                entityId,
                folderName,
                documentType,
                fileName
            );
            if (notChanged)
            {
                // Do not save full file names to the logs (for privacy)
                var logFileName = WordSanitizer.Sanitize(fileName);
                _logger.LogInformation(
                    "[FileManagerClient] UploadPdfIfChanged - PDF file {LogFileName} in folder {LogFolderName} hasn't changed. Will NOT UPLOAD again.",
                    logFileName,
                    logFolderName
                );

                // Abort early if PDF hasn't changed...
                return;
            }

            _logger.LogDebug("[FileManagerClient] UploadPdfIfChanged - File changed, proceeding with upload");

            _fileManagerClient.UploadPdf(_logger, entityName, entityId, folderName, fileName, data);
        }

        public static void UploadPdf(
            this FileManagerClient _fileManagerClient,
            ILogger _logger,
            string entityName,
            string entityId,
            string folderName,
            string filename,
            byte[] data
        )
        {
            Contract.Requires(_fileManagerClient != null);

            // Do not save full file names to the logs (for privacy)
            var logFolderName = WordSanitizer.Sanitize(folderName);
            var logFileName = WordSanitizer.Sanitize(filename);

            _logger.LogDebug(
                "[FileManagerClient] UploadPdf - called with entityName={EntityName}, entityId={EntityId}, folderName={LogFolderName}, filename={LogFileName}, dataSize={DataSize}",
                entityName,
                entityId,
                logFolderName,
                logFileName,
                data?.Length ?? 0
            );

            // call the web service
            var uploadRequest = new UploadFileRequest
            {
                ContentType = "application/pdf",
                Data = ByteString.CopyFrom(data),
                EntityName = entityName,
                FileName = filename,
                FolderName = folderName,
            };

            var uploadResult = _fileManagerClient.UploadFile(uploadRequest);

            if (uploadResult.ResultStatus == ResultStatus.Success)
            {
                _logger.LogInformation(
                    "[FileManagerClient] UploadPdf - Successfully uploaded PDF file {LogFileName} to folder {LogFolderName} for entity {EntityName}",
                    logFileName,
                    logFolderName,
                    entityName
                );
            }
            else
            {
                _logger.LogError(
                    "[FileManagerClient] UploadPdf - Error uploading PDF file {LogFileName} to folder {LogFolderName}. Error: {ErrorDetail}",
                    logFileName,
                    logFolderName,
                    uploadResult.ErrorDetail
                );
                throw new Exception(
                    $"[FileManagerClient] UploadPdf - ERROR in uploading PDF file {logFileName} to folder {logFolderName}. Error: {uploadResult.ErrorDetail}"
                );
            }
        }

        public static bool FileExists(
            this FileManagerClient _fileManagerClient,
            ILogger _logger,
            string entityName,
            string entityId,
            string folderName,
            string documentType,
            string fileName
        )
        {
            Contract.Requires(_fileManagerClient != null);

            var logFolderName = WordSanitizer.Sanitize(folderName);
            var logFileName = WordSanitizer.Sanitize(fileName);

            _logger.LogDebug(
                "[FileManagerClient] FileExists - called with entityName={EntityName}, entityId={EntityId}, folderName={LogFolderName}, documentType={DocumentType}, fileName={LogFileName}",
                entityName,
                entityId,
                logFolderName,
                documentType,
                logFileName
            );

            var exists = false;
            try
            {
                // call the web service
                var request = new FolderFilesRequest
                {
                    DocumentType = documentType,
                    EntityId = entityId,
                    EntityName = entityName,
                    FolderName = folderName,
                };

                var result = _fileManagerClient.FolderFiles(request);

                if (result.ResultStatus == ResultStatus.Success)
                {
                    exists = result.Files.Any(f => f.Name == fileName);
                    _logger.LogDebug(
                        "[FileManagerClient] FileExists - File {LogFileName} exists: {Exists} in folder {LogFolderName}",
                        logFileName,
                        exists,
                        logFolderName
                    );
                }
                else
                {
                    _logger.LogError(
                        "[FileManagerClient] FileExists - Error getting folder files for entity {EntityName}, folder {LogFolderName}. Error: {ErrorDetail}",
                        entityName,
                        logFolderName,
                        result.ErrorDetail
                    );
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "[FileManagerClient] FileExists - Exception getting SharePoint File List for entity {EntityName}, folder {LogFolderName}",
                    entityName,
                    logFolderName
                );
            }

            _logger.LogDebug("[FileManagerClient] FileExists - Returning exists={Exists}", exists);
            return exists;
        }

        public static string GetTruncatedFilename(
            this FileManagerClient _fileManagerClient,
            ILogger _logger,
            string entityName,
            string folderName,
            string fileName
        )
        {
            Contract.Requires(_fileManagerClient != null);

            // Do not save full file names to the logs (for privacy)
            var logFileName = WordSanitizer.Sanitize(fileName);
            var logFolderName = WordSanitizer.Sanitize(folderName);

            _logger.LogDebug(
                "[FileManagerClient] GetTruncatedFilename - called with entityName={EntityName}, folderName={LogFolderName}, fileName={LogFileName}",
                entityName,
                logFolderName,
                logFileName
            );

            var truncated = fileName;

            try
            {
                // call the web service
                var request = new TruncatedFilenameRequest
                {
                    EntityName = entityName,
                    FolderName = folderName,
                    FileName = fileName,
                };

                // Get the (potentially) truncated filename from SharePoint
                var result = _fileManagerClient.GetTruncatedFilename(request);

                if (result.ResultStatus == ResultStatus.Success)
                {
                    truncated = result.FileName;
                    var logTruncatedFileName = WordSanitizer.Sanitize(truncated);
                    if (truncated != fileName)
                    {
                        _logger.LogDebug(
                            "[FileManagerClient] GetTruncatedFilename - Filename was truncated from {LogFileName} to {LogTruncatedFileName}",
                            logFileName,
                            logTruncatedFileName
                        );
                    }
                    else
                    {
                        _logger.LogDebug(
                            "[FileManagerClient] GetTruncatedFilename - Filename {LogFileName} does not need truncation",
                            logFileName
                        );
                    }
                }
                else
                {
                    _logger.LogError(
                        "[FileManagerClient] GetTruncatedFilename - Error getting truncated filename {LogFileName} for folder {LogFolderName}. Error: {ErrorDetail}",
                        logFileName,
                        logFolderName,
                        result.ErrorDetail
                    );
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "[FileManagerClient] GetTruncatedFilename - Exception getting truncated filename {LogFileName} for folder {LogFolderName}",
                    logFileName,
                    logFolderName
                );
            }

            _logger.LogDebug("[FileManagerClient] GetTruncatedFilename - Returning truncated filename");
            return truncated;
        }
    }
}
