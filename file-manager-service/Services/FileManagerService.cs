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
                case "adoxio_application":
                    listTitle = SharePointFileManager.ApplicationDocumentListTitle;
                    break;
                case "contact":
                    listTitle = SharePointFileManager.ContactDocumentListTitle;
                    break;
                case "worker":
                case "adoxio_worker":
                    listTitle = SharePointFileManager.WorkerDocumentListTitle;
                    break;
                case "event":
                case "adoxio_event":
                    listTitle = SharePointFileManager.EventDocumentListTitle;
                    break;
                case "federal_report":
                case "adoxio_federalreportexport":
                    listTitle = SharePointFileManager.FederalReportListTitle;
                    break;
                case "licence":
                case "adoxio_licences":
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
                case "adoxio_application":
                    listTitle = "adoxio_application";
                    break;
                case "contact":
                    listTitle = SharePointFileManager.ContactDocumentListTitle;
                    break;
                case "worker":
                case "adoxio_worker":
                    listTitle = "adoxio_worker";
                    break;
                case "event":
                case "adoxio_event":
                    listTitle = SharePointFileManager.EventDocumentListTitle;
                    break;
                case "federal_report":
                case "adoxio_federalreportexport":
                    listTitle = SharePointFileManager.FederalReportListTitle;
                    break;
                case "licence":
                case "adoxio_licences":
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

            var listTitle = GetDocumentListTitle(request.EntityName);
            CreateDocumentLibraryIfMissing(listTitle, GetDocumentTemplateUrlPart(request.EntityName));

            try
            {
                // Create intermediate folders by calling EnsureFolderPath if the path contains multiple segments
                if (!string.IsNullOrEmpty(request.FolderName) && request.FolderName.Contains("/"))
                {
                    var ensureRequest = new EnsureFolderPathRequest { EntityName = request.EntityName };

                    // Split the folder path and create segments
                    var pathSegments = request.FolderName.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var segment in pathSegments)
                    {
                        ensureRequest.FolderPath.Add(new FolderSegment { FolderName = segment });
                    }

                    // Call EnsureFolderPath to create all intermediate folders
                    var ensureResult = EnsureFolderPath(ensureRequest, context).GetAwaiter().GetResult();
                    if (ensureResult.ResultStatus != ResultStatus.Success)
                    {
                        throw new Exception($"Failed to ensure folder path: {ensureResult.ErrorDetail}");
                    }
                }

                // Upload the file (AddFile will handle single-level folder creation if needed)
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

        public override Task<EnsureFolderPathReply> EnsureFolderPath(
            EnsureFolderPathRequest request,
            ServerCallContext context
        )
        {
            var result = new EnsureFolderPathReply();

            if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
            {
                result.ResultStatus = ResultStatus.Success;

                // Build path from folderPath segments
                if (request.FolderPath != null && request.FolderPath.Count > 0)
                {
                    var pathSegments = string.Join(
                        "/",
                        request.FolderPath.Select(s =>
                        {
                            if (!string.IsNullOrEmpty(s.FolderName))
                            {
                                return s.FolderName;
                            }
                            else
                            {
                                // Convert GUID to uppercase and remove dashes
                                var guidSegment = s.FolderGuidSegment ?? "";
                                if (!string.IsNullOrEmpty(guidSegment))
                                {
                                    guidSegment = guidSegment.Replace("-", "").ToUpper();
                                }
                                return $"{s.FolderNameSegment}_{guidSegment}";
                            }
                        })
                    );
                    result.ServerRelativeUrl = $"/{request.EntityName}/{pathSegments}";
                }
                else
                {
                    result.ServerRelativeUrl = $"/{request.EntityName}";
                }

                return Task.FromResult(result);
            }

            try
            {
                var _sharePointFileManager = new SharePointFileManager(_configuration);
                var listTitle = GetDocumentListTitle(request.EntityName);
                var documentTemplateUrlPart = GetDocumentTemplateUrlPart(request.EntityName);

                Log.Information(
                    $"EnsureFolderPath: Building folder path with {request.FolderPath?.Count ?? 0} segments"
                );

                // Create document library if missing
                CreateDocumentLibraryIfMissing(listTitle, documentTemplateUrlPart);

                string currentPath = "";

                // Build and create nested folder structure
                if (request.FolderPath != null && request.FolderPath.Count > 0)
                {
                    for (int i = 0; i < request.FolderPath.Count; i++)
                    {
                        var segment = request.FolderPath[i];

                        Log.Information(
                            $"EnsureFolderPath: Segment {i} - FolderName: '{segment.FolderName}', FolderNameSegment: '{segment.FolderNameSegment}', FolderGuidSegment: '{segment.FolderGuidSegment}'"
                        );

                        // Build folder name - either use complete folderName or combine name and guid segments
                        string folderName;
                        if (!string.IsNullOrEmpty(segment.FolderName))
                        {
                            // Use the complete folder name provided and sanitize it
                            folderName = segment.FolderName;
                            Log.Information($"EnsureFolderPath: Using complete folderName: '{folderName}'");
                        }
                        else
                        {
                            // Sanitize individual segments before combining them
                            var sanitizedNameSegment = segment.FolderNameSegment ?? "";
                            var sanitizedGuidSegment = segment.FolderGuidSegment ?? "";

                            // Convert GUID to uppercase and remove dashes (consistent with other file operations)
                            if (!string.IsNullOrEmpty(sanitizedGuidSegment))
                            {
                                sanitizedGuidSegment = sanitizedGuidSegment.Replace("-", "").ToUpper();
                            }

                            // Build folder name by combining sanitized name segment and guid segment
                            folderName = $"{sanitizedNameSegment}_{sanitizedGuidSegment}";
                            Log.Information($"EnsureFolderPath: Built from segments: '{folderName}'");
                        }

                        // Apply SharePoint filename sanitization and truncation BEFORE building cumulative path
                        var originalFolderName = folderName;
                        folderName = _sharePointFileManager.RemoveInvalidCharacters(folderName);
                        Log.Information(
                            $"EnsureFolderPath: After RemoveInvalidCharacters: '{originalFolderName}' -> '{folderName}'"
                        );

                        // Build the cumulative path with the sanitized folder name
                        if (i == 0)
                        {
                            currentPath = folderName;
                        }
                        else
                        {
                            currentPath += "/" + folderName;
                        }

                        Log.Information($"EnsureFolderPath: Current cumulative path: '{currentPath}'");

                        var logSegmentPath = WordSanitizer.Sanitize(currentPath);
                        Log.Information($"EnsureFolderPath: Processing folder segment {i + 1}: {logSegmentPath}");

                        // Check if this level exists, create if not
                        var folderExists = false;
                        try
                        {
                            var folder = _sharePointFileManager
                                .GetFolder(documentTemplateUrlPart, currentPath)
                                .GetAwaiter()
                                .GetResult();
                            if (folder != null)
                            {
                                folderExists = true;
                                Log.Information($"EnsureFolderPath: Folder exists: {logSegmentPath}");
                            }
                        }
                        catch (SharePointRestException)
                        {
                            folderExists = false;
                        }
                        catch (Exception)
                        {
                            folderExists = false;
                        }

                        if (!folderExists)
                        {
                            Log.Information($"EnsureFolderPath: Creating folder: {logSegmentPath}");
                            _sharePointFileManager.CreateFolder(documentTemplateUrlPart, currentPath).GetAwaiter().GetResult();
                        }
                    }
                }

                Log.Information($"EnsureFolderPath: Final currentPath value: '{currentPath}'");

                // Build full server relative URL
                var serverRelativeUrl = string.IsNullOrEmpty(currentPath)
                    ? _sharePointFileManager.GetServerRelativeURL(documentTemplateUrlPart, "")
                    : _sharePointFileManager.GetServerRelativeURL(documentTemplateUrlPart, currentPath);

                // Remove trailing slash if present and path is not empty
                if (!string.IsNullOrEmpty(currentPath) && serverRelativeUrl.EndsWith("/"))
                {
                    serverRelativeUrl = serverRelativeUrl.TrimEnd('/');
                }

                // Add leading slash if not already present
                if (!string.IsNullOrEmpty(serverRelativeUrl) && !serverRelativeUrl.StartsWith("/"))
                {
                    serverRelativeUrl = "/" + serverRelativeUrl;
                }

                result.ServerRelativeUrl = serverRelativeUrl;
                result.ResultStatus = ResultStatus.Success;

                Log.Information($"EnsureFolderPath: Final path: {WordSanitizer.Sanitize(serverRelativeUrl)}");
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = "EnsureFolderPath - ERROR in ensuring folder path";
                Log.Error(ex, result.ErrorDetail);
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = "EnsureFolderPath - ERROR in ensuring folder path";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        public override Task<FindFolderReply> FindFolder(FindFolderRequest request, ServerCallContext context)
        {
            var result = new FindFolderReply();

            if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
            {
                result.ResultStatus = ResultStatus.Success;
                // Return a dummy folder for testing when SharePoint is disabled
                result.Folders.Add(
                    new FolderInfo
                    {
                        Name = $"TestFolder_{request.SearchString}",
                        ServerRelativeUrl = $"/{request.EntityName}/TestFolder_{request.SearchString}"
                    }
                );
                return Task.FromResult(result);
            }

            var logSearchString = request.SearchString;
            if (!string.IsNullOrEmpty(request.SearchGuid))
            {
                logSearchString = request.SearchGuid.Replace("-", "").ToUpper();
            }
            logSearchString = WordSanitizer.Sanitize(logSearchString);

            try
            {
                var _sharePointFileManager = new SharePointFileManager(_configuration);
                var listTitle = GetDocumentListTitle(request.EntityName);

                Log.Information(
                    $"FindFolder: Searching for folders in document library '{listTitle}' containing '{logSearchString}'"
                );

                // Use server-side filtering for better performance with large folder counts
                // Pass either searchString (case-sensitive) or searchGuid (normalized to uppercase, no dashes)
                var folders = _sharePointFileManager
                    .SearchFoldersInDocumentLibrary(listTitle, request.SearchString, request.SearchGuid)
                    .GetAwaiter()
                    .GetResult();

                if (folders != null && folders.Count > 0)
                {
                    Log.Information($"FindFolder: Found {folders.Count} matching folders");

                    foreach (var folder in folders)
                    {
                        result.Folders.Add(
                            new FolderInfo { Name = folder.Name, ServerRelativeUrl = folder.ServerRelativeUrl }
                        );
                    }

                    result.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    Log.Information($"FindFolder: No folders found matching '{logSearchString}'");
                    result.ResultStatus = ResultStatus.Success; // Not an error, just no matches
                }
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"FindFolder - ERROR searching for folders containing {logSearchString}";
                Log.Error(ex, result.ErrorDetail);
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"FindFolder - ERROR searching for folders containing {logSearchString}";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }
    }
}
