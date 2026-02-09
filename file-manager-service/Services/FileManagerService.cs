using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Gov.Lclb.Cllb.Interfaces;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Gov.Lclb.Cllb.Services.FileManager
{
    // Default to require authorization
    [Authorize]
    public class FileManagerService : FileManager.FileManagerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileManagerService> _logger;

        public FileManagerService(ILogger<FileManagerService> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public override Task<CreateFolderReply> CreateFolder(CreateFolderRequest request, ServerCallContext context)
        {
            var result = new CreateFolderReply();

            if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
            {
                result.ResultStatus = ResultStatus.Success;
                return Task.FromResult(result);
            }

            var logFolder = WordSanitizer.Sanitize(request.FolderName);

            var listTitle = GetDocumentListTitle(request.EntityName);

            var _sharePointFileManager = new SharePointFileManager(_configuration);

            CreateDocumentLibraryIfMissing(listTitle, GetDocumentTemplateUrlPart(request.EntityName));

            var folderExists = false;
            try
            {
                var folder = _sharePointFileManager.GetFolder(listTitle, request.FolderName).GetAwaiter().GetResult();
                if (folder != null)
                {
                    folderExists = true;
                }
            }
            catch (SharePointRestException ex)
            {
                Log.Error(
                    ex,
                    $"SharePointRestException creating sharepoint folder (status code: {ex.Response.StatusCode})"
                );
                folderExists = false;
            }
            catch (Exception e)
            {
                Log.Error(e, "Generic Exception creating sharepoint folder");
                folderExists = false;
            }

            if (folderExists)
            {
                result.ResultStatus = ResultStatus.Success;
            }
            else
            {
                try
                {
                    _sharePointFileManager
                        .CreateFolder(GetDocumentListTitle(request.EntityName), request.FolderName)
                        .GetAwaiter()
                        .GetResult();
                    var folder = _sharePointFileManager
                        .GetFolder(listTitle, request.FolderName)
                        .GetAwaiter()
                        .GetResult();
                    if (folder != null)
                    {
                        result.ResultStatus = ResultStatus.Success;
                    }
                }
                catch (SharePointRestException ex)
                {
                    result.ResultStatus = ResultStatus.Fail;
                    result.ErrorDetail = $"CreateFolder - ERROR in creating folder {logFolder}";
                    Log.Error(ex, result.ErrorDetail);
                }
                catch (Exception e)
                {
                    result.ResultStatus = ResultStatus.Fail;
                    result.ErrorDetail = $"CreateFolder - ERROR in creating folder {logFolder}";
                    Log.Error(e, result.ErrorDetail);
                }
            }

            return Task.FromResult(result);
        }

        public override Task<FileExistsReply> FileExists(FileExistsRequest request, ServerCallContext context)
        {
            var result = new FileExistsReply();

            if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
            {
                result.ResultStatus = FileExistStatus.Exist;
                return Task.FromResult(result);
            }

            var _sharePointFileManager = new SharePointFileManager(_configuration);

            List<SharePointFileManager.FileDetailsList> fileDetailsList = null;
            try
            {
                fileDetailsList = _sharePointFileManager
                    .GetFileDetailsListInFolder(
                        GetDocumentTemplateUrlPart(request.EntityName),
                        request.FolderName,
                        request.DocumentType
                    )
                    .GetAwaiter()
                    .GetResult();
                if (fileDetailsList != null)
                {
                    var hasFile = fileDetailsList.Any(f => f.ServerRelativeUrl == request.ServerRelativeUrl);

                    if (hasFile)
                    {
                        result.ResultStatus = FileExistStatus.Exist;
                    }
                    else
                    {
                        result.ResultStatus = FileExistStatus.NotExist;
                    }
                }
            }
            catch (SharePointRestException spre)
            {
                Log.Error(spre, "Error determining if file exists");
                result.ResultStatus = result.ResultStatus = FileExistStatus.Error;
                result.ErrorDetail = "FileExists - Error determining if file exists";
            }
            catch (Exception e)
            {
                result.ResultStatus = FileExistStatus.Error;
                result.ErrorDetail = "FileExists - Error determining if file exists";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        private string GetDocumentListTitle(string entityName)
        {
            string listTitle;
            switch (entityName.ToLower())
            {
                case "account":
                    listTitle = SharePointFileManager.DefaultDocumentListTitle;
                    break;
                case "application":
                    listTitle = SharePointFileManager.ApplicationDocumentListTitle;
                    break;
                case "contact":
                    listTitle = SharePointFileManager.ContactDocumentListTitle;
                    break;
                case "worker":
                    listTitle = SharePointFileManager.WorkerDocumentListTitle;
                    break;
                case "event":
                    listTitle = SharePointFileManager.EventDocumentListTitle;
                    break;
                case "federal_report":
                    listTitle = SharePointFileManager.FederalReportListTitle;
                    break;
                case "licence":
                    listTitle = SharePointFileManager.LicenceDocumentListTitle;
                    break;
                default:
                    listTitle = entityName;
                    break;
            }

            return listTitle;
        }

        private string GetDocumentTemplateUrlPart(string entityName)
        {
            var listTitle = "";
            switch (entityName.ToLower())
            {
                case "account":
                    listTitle = SharePointFileManager.DefaultDocumentUrlTitle;
                    break;
                case "application":
                    listTitle = "adoxio_application";
                    break;
                case "contact":
                    listTitle = SharePointFileManager.ContactDocumentListTitle;
                    break;
                case "worker":
                    listTitle = "adoxio_worker";
                    break;
                case "event":
                    listTitle = SharePointFileManager.EventDocumentListTitle;
                    break;
                case "federal_report":
                    listTitle = SharePointFileManager.FederalReportListTitle;
                    break;
                case "licence":
                    listTitle = SharePointFileManager.LicenceDocumentUrlTitle;
                    break;
                default:
                    listTitle = entityName;
                    break;
            }

            return listTitle;
        }

        private void CreateDocumentLibraryIfMissing(string listTitle, string documentTemplateUrl = null)
        {
            var _sharePointFileManager = new SharePointFileManager(_configuration);
            var exists = _sharePointFileManager.DocumentLibraryExists(listTitle).GetAwaiter().GetResult();
            if (!exists)
                _sharePointFileManager.CreateDocumentLibrary(listTitle, documentTemplateUrl).GetAwaiter().GetResult();
        }

        public override Task<DeleteFileReply> DeleteFile(DeleteFileRequest request, ServerCallContext context)
        {
            var result = new DeleteFileReply();

            if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
            {
                result.ResultStatus = ResultStatus.Success;
                return Task.FromResult(result);
            }

            var logUrl = WordSanitizer.Sanitize(request.ServerRelativeUrl);

            var _sharePointFileManager = new SharePointFileManager(_configuration);

            try
            {
                var success = _sharePointFileManager.DeleteFile(request.ServerRelativeUrl).GetAwaiter().GetResult();

                if (success)
                {
                    result.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    result.ResultStatus = ResultStatus.Fail;
                }
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"DeleteFile - ERROR in deleting file {logUrl}";
                Log.Error(ex, result.ErrorDetail);
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"DeleteFile - ERROR in deleting file {logUrl}";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        public override Task<DownloadFileReply> DownloadFile(DownloadFileRequest request, ServerCallContext context)
        {
            var result = new DownloadFileReply();

            if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
            {
                result.ResultStatus = ResultStatus.Success;
                result.Data = ByteString.CopyFrom(new byte[0]);
                return Task.FromResult(result);
            }

            var logUrl = WordSanitizer.Sanitize(request.ServerRelativeUrl);
            var _sharePointFileManager = new SharePointFileManager(_configuration);

            try
            {
                var data = _sharePointFileManager.DownloadFile(request.ServerRelativeUrl).GetAwaiter().GetResult();

                if (data != null)
                {
                    result.ResultStatus = ResultStatus.Success;
                    result.Data = ByteString.CopyFrom(data);
                }
                else
                {
                    result.ResultStatus = ResultStatus.Fail;
                }
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"DownloadFile - ERROR in downloading file {logUrl}";
                Log.Error(ex, result.ErrorDetail);
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"DownloadFile - ERROR in downloading file {logUrl}";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        public override Task<UploadFileReply> UploadFile(UploadFileRequest request, ServerCallContext context)
        {
            var result = new UploadFileReply();

            if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
            {
                result.ResultStatus = ResultStatus.Success;
                result.FileName = request.FileName;
                return Task.FromResult(result);
            }

            var logFileName = WordSanitizer.Sanitize(request.FileName);
            var logFolderName = WordSanitizer.Sanitize(request.FolderName);

            var _sharePointFileManager = new SharePointFileManager(_configuration);

            CreateDocumentLibraryIfMissing(
                GetDocumentListTitle(request.EntityName),
                GetDocumentTemplateUrlPart(request.EntityName)
            );

            try
            {
                var fileName = _sharePointFileManager
                    .AddFile(
                        GetDocumentTemplateUrlPart(request.EntityName),
                        request.FolderName,
                        request.FileName,
                        request.Data.ToByteArray(),
                        request.ContentType
                    )
                    .GetAwaiter()
                    .GetResult();

                result.FileName = fileName;
                result.ResultStatus = ResultStatus.Success;
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"UploadFile - ERROR in uploading file {logFileName} to folder {logFolderName}";
                Log.Error(ex, result.ErrorDetail);
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"UploadFile - ERROR in uploading file {logFileName} to folder {logFolderName}";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        public override Task<FolderFilesReply> FolderFiles(FolderFilesRequest request, ServerCallContext context)
        {
            var result = new FolderFilesReply();

            if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
            {
                result.ResultStatus = ResultStatus.Success;
                result.Files.Add(
                    new FileSystemItem
                    {
                        DocumentType = request.DocumentType,
                        Name = "disable_sharepoint_integration.pdf",
                        ServerRelativeUrl =
                            $"/adoxio_application/{request.FolderName}/{request.DocumentType}__disable_sharepoint_integration.pdf",
                        Size = 10240,
                        TimeCreated = Timestamp.FromDateTime(DateTime.UtcNow),
                        TimeLastModified = Timestamp.FromDateTime(DateTime.UtcNow)
                    }
                );
                return Task.FromResult(result);
            }

            // Get the file details list in folder
            List<SharePointFileManager.FileDetailsList> fileDetailsList = null;
            var _sharePointFileManager = new SharePointFileManager(_configuration);
            try
            {
                fileDetailsList = _sharePointFileManager
                    .GetFileDetailsListInFolder(
                        GetDocumentTemplateUrlPart(request.EntityName),
                        request.FolderName,
                        request.DocumentType
                    )
                    .GetAwaiter()
                    .GetResult();
                if (fileDetailsList != null)
                {
                    // gRPC ensures that the collection has space to accept new data; no need to call a constructor
                    foreach (var item in fileDetailsList)
                    {
                        // Sharepoint API responds with dates in UTC format
                        var utcFormat = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
                        DateTime parsedCreateDate,
                            parsedLastModified;
                        DateTime.TryParse(
                            item.TimeCreated,
                            CultureInfo.InvariantCulture,
                            utcFormat,
                            out parsedCreateDate
                        );
                        DateTime.TryParse(
                            item.TimeLastModified,
                            CultureInfo.InvariantCulture,
                            utcFormat,
                            out parsedLastModified
                        );

                        var newItem = new FileSystemItem
                        {
                            DocumentType = item.DocumentType,
                            Name = item.Name,
                            ServerRelativeUrl = item.ServerRelativeUrl,
                            Size = int.Parse(item.Length),
                            TimeCreated = Timestamp.FromDateTime(parsedCreateDate),
                            TimeLastModified = Timestamp.FromDateTime(parsedLastModified)
                        };

                        result.Files.Add(newItem);
                    }

                    result.ResultStatus = ResultStatus.Success;
                }
            }
            catch (SharePointRestException spre)
            {
                Log.Error(spre, "Error getting SharePoint File List");
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = "FolderFiles - Error getting SharePoint File List";
            }

            return Task.FromResult(result);
        }

        [AllowAnonymous]
        public override Task<TokenReply> GetToken(TokenRequest request, ServerCallContext context)
        {
            var result = new TokenReply();
            result.ResultStatus = ResultStatus.Fail;

            var configuredSecret = _configuration["JWT_TOKEN_KEY"];
            if (configuredSecret.Equals(request.Secret))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuredSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    _configuration["JWT_VALID_ISSUER"],
                    _configuration["JWT_VALID_AUDIENCE"],
                    expires: DateTime.UtcNow.AddYears(5),
                    signingCredentials: creds
                );
                result.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                result.ResultStatus = ResultStatus.Success;
            }
            else
            {
                result.ErrorDetail = "GetToken - Invalid Secret";
            }

            return Task.FromResult(result);
        }

        public override Task<TruncatedFilenameReply> GetTruncatedFilename(
            TruncatedFilenameRequest request,
            ServerCallContext context
        )
        {
            var result = new TruncatedFilenameReply();

            if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
            {
                result.ResultStatus = ResultStatus.Success;
                result.FileName = request.FileName;
                return Task.FromResult(result);
            }

            var logFileName = WordSanitizer.Sanitize(request.FileName);
            var logFolderName = WordSanitizer.Sanitize(request.FolderName);

            try
            {
                var _sharePointFileManager = new SharePointFileManager(_configuration);

                // Ask SharePoint whether this filename would be truncated upon upload
                var listTitle = GetDocumentListTitle(request.EntityName);
                var maybeTruncated = _sharePointFileManager.GetTruncatedFileName(
                    request.FileName,
                    listTitle,
                    request.FolderName
                );
                result.FileName = maybeTruncated;
                result.ResultStatus = ResultStatus.Success;
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail =
                    $"GetTruncatedFilename - ERROR in getting truncated filename {logFileName} for folder {logFolderName}";
                Log.Error(ex, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }
    }
}
