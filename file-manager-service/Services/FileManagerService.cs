using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
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

            var _sharePointFileManager = new SharePointFileManager(_configuration);

            var listTitle = _sharePointFileManager.GetDocumentListTitle(request.EntityName);
            var urlTitle = _sharePointFileManager.GetDocumentTemplateUrlPart(request.EntityName);

            CreateDocumentLibraryIfMissing(listTitle, urlTitle);

            var folderExists = false;
            try
            {
                var folder = _sharePointFileManager.GetFolder(urlTitle, request.FolderName).GetAwaiter().GetResult();
                if (folder != null)
                {
                    folderExists = true;
                }
            }
            catch (SharePointRestException ex)
            {
                Log.Error(
                    ex,
                    $"SharePointRestException creating sharepoint folder - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}, Response: {ex.Response?.Content}"
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
                    _sharePointFileManager.CreateFolder(urlTitle, request.FolderName).GetAwaiter().GetResult();
                    var folder = _sharePointFileManager
                        .GetFolder(urlTitle, request.FolderName)
                        .GetAwaiter()
                        .GetResult();
                    if (folder != null)
                    {
                        result.ResultStatus = ResultStatus.Success;
                        Console.WriteLine(
                            $"FileManagerService - CreateFolder - successfully created folder '{logFolder}' in '{listTitle}'"
                        );
                    }
                }
                catch (SharePointRestException ex) when (ex.Response?.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    // Forbidden could mean folder already exists (race condition)
                    // Verify if folder actually exists
                    Log.Warning(
                        ex,
                        $"CreateFolder - Received Forbidden when creating folder {logFolder}, verifying existence - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}"
                    );
                    try
                    {
                        var folder = _sharePointFileManager
                            .GetFolder(urlTitle, request.FolderName)
                            .GetAwaiter()
                            .GetResult();
                        if (folder != null)
                        {
                            result.ResultStatus = ResultStatus.Success;
                            Console.WriteLine(
                                $"FileManagerService - CreateFolder - folder '{logFolder}' verified to exist after Forbidden error in '{listTitle}'"
                            );
                        }
                        else
                        {
                            // Folder doesn't exist and we can't create it
                            result.ResultStatus = ResultStatus.Fail;
                            result.ErrorDetail = $"CreateFolder - ERROR: Access denied creating folder {logFolder}";
                            Log.Error(
                                ex,
                                $"{result.ErrorDetail} - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}, Response: {ex.Response?.Content}"
                            );
                        }
                    }
                    catch (Exception verifyEx)
                    {
                        // Verification failed - log original error
                        result.ResultStatus = ResultStatus.Fail;
                        result.ErrorDetail = $"CreateFolder - ERROR in creating folder {logFolder}";
                        Log.Error(
                            ex,
                            $"{result.ErrorDetail} - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}, Response: {ex.Response?.Content}"
                        );
                    }
                }
                catch (SharePointRestException ex)
                {
                    result.ResultStatus = ResultStatus.Fail;
                    result.ErrorDetail = $"CreateFolder - ERROR in creating folder {logFolder}";
                    Log.Error(
                        ex,
                        $"{result.ErrorDetail} - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}, Response: {ex.Response?.Content}"
                    );
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
                        _sharePointFileManager.GetDocumentTemplateUrlPart(request.EntityName),
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
                        Console.WriteLine(
                            $"FileManagerService - FileExists - file exists at '{WordSanitizer.Sanitize(request.ServerRelativeUrl)}'"
                        );
                    }
                    else
                    {
                        result.ResultStatus = FileExistStatus.NotExist;
                        Console.WriteLine(
                            $"FileManagerService - FileExists - file does not exist at '{WordSanitizer.Sanitize(request.ServerRelativeUrl)}'"
                        );
                    }
                }
            }
            catch (SharePointRestException spre)
            {
                result.ResultStatus = FileExistStatus.Error;
                result.ErrorDetail = "FileExists - Error determining if file exists";
                Log.Error(
                    spre,
                    $"{result.ErrorDetail} - Status: {spre.Response?.StatusCode}, Request: {spre.Request?.RequestUri}, Response: {spre.Response?.Content}"
                );
            }
            catch (Exception e)
            {
                result.ResultStatus = FileExistStatus.Error;
                result.ErrorDetail = "FileExists - Error determining if file exists";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
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
                    Console.WriteLine($"FileManagerService - DeleteFile - successfully deleted file at '{logUrl}'");
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
                Log.Error(
                    ex,
                    $"{result.ErrorDetail} - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}, Response: {ex.Response?.Content}"
                );
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
                    Console.WriteLine(
                        $"FileManagerService - DownloadFile - successfully downloaded file from '{logUrl}', size: {data.Length} bytes"
                    );
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
                Log.Error(
                    ex,
                    $"{result.ErrorDetail} - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}, Response: {ex.Response?.Content}"
                );
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

            var listTitle = _sharePointFileManager.GetDocumentListTitle(request.EntityName);
            var documentTemplateUrlPart = _sharePointFileManager.GetDocumentTemplateUrlPart(request.EntityName);

            Console.WriteLine(
                $"UploadFile: Uploading file '{logFileName}' to entity {request.EntityName}, folder '{logFolderName}'"
            );

            CreateDocumentLibraryIfMissing(listTitle, documentTemplateUrlPart);

            try
            {
                // Create intermediate folders by calling EnsureFolderPath if the path contains multiple segments
                if (!string.IsNullOrEmpty(request.FolderName) && request.FolderName.Contains("/"))
                {
                    Console.WriteLine($"UploadFile: Multi-level folder path detected, ensuring folder structure");

                    var ensureRequest = new EnsureFolderPathRequest { EntityName = request.EntityName };

                    // Split the folder path and create segments
                    var pathSegments = request.FolderName.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    Console.WriteLine($"UploadFile: Split into {pathSegments.Length} segments");

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

                    Console.WriteLine($"UploadFile: Folder structure ensured successfully");
                }
                else if (!string.IsNullOrEmpty(request.FolderName))
                {
                    Console.WriteLine($"UploadFile: Single-level folder path, AddFile will handle creation if needed");
                }
                else
                {
                    Console.WriteLine($"UploadFile: No folder specified, uploading to root of document library");
                }

                // Upload the file (AddFile will handle single-level folder creation if needed)
                Console.WriteLine($"UploadFile: Calling AddFile to upload file");

                var fileName = _sharePointFileManager
                    .AddFile(
                        documentTemplateUrlPart,
                        request.FolderName,
                        request.FileName,
                        request.Data.ToByteArray(),
                        request.ContentType
                    )
                    .GetAwaiter()
                    .GetResult();

                result.FileName = fileName;
                result.ResultStatus = ResultStatus.Success;

                Console.WriteLine(
                    $"FileManagerService - UploadFile - successfully uploaded file '{fileName}' to entity '{request.EntityName}', folder '{logFolderName}'"
                );
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"UploadFile - ERROR in uploading file {logFileName} to folder {logFolderName}";
                Log.Error(
                    ex,
                    $"{result.ErrorDetail} - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}, Response: {ex.Response?.Content}"
                );
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
                        _sharePointFileManager.GetDocumentTemplateUrlPart(request.EntityName),
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
                    Console.WriteLine(
                        $"FileManagerService - FolderFiles - successfully retrieved {result.Files.Count} files from folder '{WordSanitizer.Sanitize(request.FolderName)}' in entity '{request.EntityName}'"
                    );
                }
            }
            catch (SharePointRestException spre)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = "FolderFiles - Error getting SharePoint File List";
                Log.Error(
                    spre,
                    $"{result.ErrorDetail} - Status: {spre.Response?.StatusCode}, Request: {spre.Request?.RequestUri}, Response: {spre.Response?.Content}"
                );
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
                Console.WriteLine($"FileManagerService - GetToken - successfully generated authentication token");
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
                var urlTitle = _sharePointFileManager.GetDocumentTemplateUrlPart(request.EntityName);
                var maybeTruncated = _sharePointFileManager.GetTruncatedFileName(
                    request.FileName,
                    urlTitle,
                    request.FolderName
                );
                result.FileName = maybeTruncated;
                result.ResultStatus = ResultStatus.Success;
                Console.WriteLine(
                    $"FileManagerService - GetTruncatedFilename - successfully computed filename '{maybeTruncated}' for '{logFileName}'"
                );
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail =
                    $"GetTruncatedFilename - ERROR in getting truncated filename {logFileName} for folder {logFolderName}";
                Log.Error(
                    ex,
                    $"{result.ErrorDetail} - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}, Response: {ex.Response?.Content}"
                );
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
                var listTitle = _sharePointFileManager.GetDocumentListTitle(request.EntityName);
                var documentTemplateUrlPart = _sharePointFileManager.GetDocumentTemplateUrlPart(request.EntityName);

                Console.WriteLine(
                    $"EnsureFolderPath: Building folder path with {request.FolderPath?.Count ?? 0} segments"
                );

                // Create document library if missing
                CreateDocumentLibraryIfMissing(listTitle, documentTemplateUrlPart);

                // Validate folder path segments before processing
                if (request.FolderPath != null && request.FolderPath.Count > 0)
                {
                    for (int i = 0; i < request.FolderPath.Count; i++)
                    {
                        var segment = request.FolderPath[i];

                        // If FolderName is not provided, both FolderNameSegment and FolderGuidSegment are required
                        if (string.IsNullOrEmpty(segment.FolderName?.Trim()))
                        {
                            if (
                                string.IsNullOrEmpty(segment.FolderNameSegment?.Trim())
                                || segment.FolderNameSegment?.Trim() == "-" // Legacy edge case where a records "name" may be initialized to a "-".
                                || string.IsNullOrEmpty(segment.FolderGuidSegment?.Trim())
                            )
                            {
                                Log.Warning(
                                    $"EnsureFolderPath: Invalid request - Segment {i} is missing FolderName and does not have both FolderNameSegment and FolderGuidSegment. "
                                        + $"FolderNameSegment: '{segment.FolderNameSegment ?? "null"}', FolderGuidSegment: '{segment.FolderGuidSegment ?? "null"}'"
                                );

                                // Return empty response gracefully
                                result.ResultStatus = ResultStatus.Fail;
                                result.ErrorDetail =
                                    $"Invalid folder path: Segment {i + 1} must have either FolderName or both FolderNameSegment and FolderGuidSegment";
                                return Task.FromResult(result);
                            }
                        }
                    }
                }

                string currentPath = "";

                // Build and create nested folder structure
                if (request.FolderPath != null && request.FolderPath.Count > 0)
                {
                    for (int i = 0; i < request.FolderPath.Count; i++)
                    {
                        var segment = request.FolderPath[i];

                        Console.WriteLine(
                            $"EnsureFolderPath: Segment {i} - FolderName: '{segment.FolderName}', FolderNameSegment: '{segment.FolderNameSegment}', FolderGuidSegment: '{segment.FolderGuidSegment}'"
                        );

                        // Build folder name - either use complete folderName or combine name and guid segments
                        string folderName;
                        if (!string.IsNullOrEmpty(segment.FolderName))
                        {
                            // Use the complete folder name provided and sanitize it
                            folderName = segment.FolderName;
                            Console.WriteLine($"EnsureFolderPath: Using complete folderName: '{folderName}'");
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
                            Console.WriteLine($"EnsureFolderPath: Built from segments: '{folderName}'");
                        }

                        // Apply SharePoint filename sanitization and truncation BEFORE building cumulative path
                        var originalFolderName = folderName;
                        folderName = _sharePointFileManager.RemoveInvalidCharacters(folderName, request.EntityName);
                        Console.WriteLine(
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

                        Console.WriteLine($"EnsureFolderPath: Current cumulative path: '{currentPath}'");

                        var logSegmentPath = WordSanitizer.Sanitize(currentPath);
                        Console.WriteLine($"EnsureFolderPath: Processing folder segment {i + 1}: {logSegmentPath}");

                        // Check if this level exists, create if not
                        var folderExists = false;
                        try
                        {
                            var folder = _sharePointFileManager
                                .GetFolder(documentTemplateUrlPart, currentPath, segment.FolderGuidSegment)
                                .GetAwaiter()
                                .GetResult();
                            if (folder != null)
                            {
                                folderExists = true;
                                Console.WriteLine($"EnsureFolderPath: Folder exists: {logSegmentPath}");

                                // Extract actual folder name from GetFolder response for first segment only
                                // (GUID-based matching only works at the document library root level)
                                if (i == 0)
                                {
                                    try
                                    {
                                        var folderJson = folder as Newtonsoft.Json.Linq.JObject;
                                        Console.WriteLine(
                                            $"EnsureFolderPath: GetFolder response for segment {i}: {folderJson}"
                                        );
                                        if (folderJson != null && folderJson["Name"] != null)
                                        {
                                            var actualFolderName = folderJson["Name"].ToString();
                                            if (
                                                !string.IsNullOrEmpty(actualFolderName)
                                                && actualFolderName != currentPath
                                            )
                                            {
                                                Console.WriteLine(
                                                    $"EnsureFolderPath: SharePoint returned actual folder name: '{actualFolderName}' (provided name: '{currentPath}')"
                                                );
                                                currentPath = actualFolderName;
                                                logSegmentPath = WordSanitizer.Sanitize(currentPath);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(
                                            $"EnsureFolderPath: Error extracting folder name from result: {ex.Message}. Continuing with provided name: '{currentPath}'"
                                        );
                                    }
                                }
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
                            Console.WriteLine($"EnsureFolderPath: Creating folder: {logSegmentPath}");
                            try
                            {
                                _sharePointFileManager
                                    .CreateFolder(documentTemplateUrlPart, currentPath)
                                    .GetAwaiter()
                                    .GetResult();
                            }
                            catch (SharePointRestException createEx) when (createEx.Response?.StatusCode == System.Net.HttpStatusCode.Forbidden)
                            {
                                // Forbidden could mean folder already exists (race condition)
                                // Verify if folder actually exists now
                                Console.WriteLine($"EnsureFolderPath: CreateFolder returned Forbidden, verifying folder existence for: {logSegmentPath}");
                                try
                                {
                                    var verifyFolder = _sharePointFileManager
                                        .GetFolder(documentTemplateUrlPart, currentPath)
                                        .GetAwaiter()
                                        .GetResult();
                                    if (verifyFolder != null)
                                    {
                                        Console.WriteLine($"EnsureFolderPath: Folder verified to exist after Forbidden error: {logSegmentPath}");
                                        // Folder exists, continue normally
                                    }
                                    else
                                    {
                                        // Folder doesn't exist and we can't create it - rethrow
                                        Console.WriteLine($"EnsureFolderPath: Folder does not exist and cannot be created: {logSegmentPath}");
                                        throw;
                                    }
                                }
                                catch (SharePointRestException)
                                {
                                    // Verification failed - rethrow original error
                                    throw createEx;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"EnsureFolderPath: Final currentPath value: '{currentPath}'");

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

                Console.WriteLine(
                    $"FileManagerService - EnsureFolderPath - successfully ensured folder path for entity '{request.EntityName}', final path: {WordSanitizer.Sanitize(serverRelativeUrl)}"
                );
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = "EnsureFolderPath - ERROR in ensuring folder path";
                Log.Error(
                    ex,
                    $"{result.ErrorDetail} - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}, Response: {ex.Response?.Content}"
                );
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
                var listTitle = _sharePointFileManager.GetDocumentListTitle(request.EntityName);

                Console.WriteLine(
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
                    Console.WriteLine($"FindFolder: Found {folders.Count} matching folders");

                    foreach (var folder in folders)
                    {
                        result.Folders.Add(
                            new FolderInfo { Name = folder.Name, ServerRelativeUrl = folder.ServerRelativeUrl }
                        );
                    }

                    result.ResultStatus = ResultStatus.Success;
                    Console.WriteLine(
                        $"FileManagerService - FindFolder - successfully found {folders.Count} folders matching '{logSearchString}' in entity '{request.EntityName}'"
                    );
                }
                else
                {
                    Console.WriteLine($"FindFolder: No folders found matching '{logSearchString}'");
                    result.ResultStatus = ResultStatus.Success; // Not an error, just no matches
                    Console.WriteLine(
                        $"FileManagerService - FindFolder - no folders found matching '{logSearchString}' in entity '{request.EntityName}'"
                    );
                }
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"FindFolder - ERROR searching for folders containing {logSearchString}";
                Log.Error(
                    ex,
                    $"{result.ErrorDetail} - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}, Response: {ex.Response?.Content}"
                );
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"FindFolder - ERROR searching for folders containing {logSearchString}";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        public override Task<UploadFileReply> UploadFileWithFolderPath(
            UploadFileWithFolderPathRequest request,
            ServerCallContext context
        )
        {
            var result = new UploadFileReply();

            if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
            {
                result.ResultStatus = ResultStatus.Success;
                result.FileName = request.FileName;
                return Task.FromResult(result);
            }

            var logFileName = WordSanitizer.Sanitize(request.FileName);

            try
            {
                var _sharePointFileManager = new SharePointFileManager(_configuration);
                var listTitle = _sharePointFileManager.GetDocumentListTitle(request.EntityName);
                var documentTemplateUrlPart = _sharePointFileManager.GetDocumentTemplateUrlPart(request.EntityName);

                Console.WriteLine(
                    $"UploadFileWithFolderPath: Uploading file '{logFileName}' with {request.FolderPath?.Count ?? 0} folder segments"
                );

                // Create document library if missing
                CreateDocumentLibraryIfMissing(listTitle, documentTemplateUrlPart);

                // Validate folder path segments before processing
                if (request.FolderPath != null && request.FolderPath.Count > 0)
                {
                    for (int i = 0; i < request.FolderPath.Count; i++)
                    {
                        var segment = request.FolderPath[i];

                        // If FolderName is not provided, both FolderNameSegment and FolderGuidSegment are required
                        if (string.IsNullOrEmpty(segment.FolderName?.Trim()))
                        {
                            if (
                                string.IsNullOrEmpty(segment.FolderNameSegment?.Trim())
                                || segment.FolderNameSegment?.Trim() == "-" // Legacy edge case where a records "name" may be initialized to a "-".
                                || string.IsNullOrEmpty(segment.FolderGuidSegment?.Trim())
                            )
                            {
                                Log.Warning(
                                    $"UploadFileWithFolderPath: Invalid request - Segment {i} is missing FolderName and does not have both FolderNameSegment and FolderGuidSegment. "
                                        + $"FolderNameSegment: '{segment.FolderNameSegment ?? "null"}', FolderGuidSegment: '{segment.FolderGuidSegment ?? "null"}'"
                                );

                                result.ResultStatus = ResultStatus.Fail;
                                result.ErrorDetail =
                                    $"Invalid folder path: Segment {i + 1} must have either FolderName or both FolderNameSegment and FolderGuidSegment";
                                return Task.FromResult(result);
                            }
                        }
                    }
                }

                string currentPath = "";

                // Build and create nested folder structure
                if (request.FolderPath != null && request.FolderPath.Count > 0)
                {
                    for (int i = 0; i < request.FolderPath.Count; i++)
                    {
                        var segment = request.FolderPath[i];

                        Console.WriteLine(
                            $"UploadFileWithFolderPath: Segment {i} - FolderName: '{segment.FolderName}', FolderNameSegment: '{segment.FolderNameSegment}', FolderGuidSegment: '{segment.FolderGuidSegment}'"
                        );

                        // Build folder name - either use complete folderName or combine name and guid segments
                        string folderName;
                        if (!string.IsNullOrEmpty(segment.FolderName))
                        {
                            folderName = segment.FolderName;
                            Console.WriteLine($"UploadFileWithFolderPath: Using complete folderName: '{folderName}'");
                        }
                        else
                        {
                            var sanitizedNameSegment = segment.FolderNameSegment ?? "";
                            var sanitizedGuidSegment = segment.FolderGuidSegment ?? "";

                            // Convert GUID to uppercase and remove dashes
                            if (!string.IsNullOrEmpty(sanitizedGuidSegment))
                            {
                                sanitizedGuidSegment = sanitizedGuidSegment.Replace("-", "").ToUpper();
                            }

                            folderName = $"{sanitizedNameSegment}_{sanitizedGuidSegment}";
                            Console.WriteLine($"UploadFileWithFolderPath: Built from segments: '{folderName}'");
                        }

                        // Apply SharePoint filename sanitization
                        var originalFolderName = folderName;
                        folderName = _sharePointFileManager.RemoveInvalidCharacters(folderName, request.EntityName);
                        Console.WriteLine(
                            $"UploadFileWithFolderPath: After RemoveInvalidCharacters: '{originalFolderName}' -> '{folderName}'"
                        );

                        // Build cumulative path
                        if (i == 0)
                        {
                            currentPath = folderName;
                        }
                        else
                        {
                            currentPath += "/" + folderName;
                        }

                        Console.WriteLine($"UploadFileWithFolderPath: Current cumulative path: '{currentPath}'");

                        var logSegmentPath = WordSanitizer.Sanitize(currentPath);

                        // Check if this level exists, create if not
                        var folderExists = false;
                        try
                        {
                            var folder = _sharePointFileManager
                                .GetFolder(documentTemplateUrlPart, currentPath, segment.FolderGuidSegment)
                                .GetAwaiter()
                                .GetResult();
                            if (folder != null)
                            {
                                folderExists = true;
                                Console.WriteLine($"UploadFileWithFolderPath: Folder exists: {logSegmentPath}");

                                // Extract actual folder name from GetFolder response for first segment only
                                // (GUID-based matching only works at the document library root level)
                                if (i == 0)
                                {
                                    try
                                    {
                                        var folderJson = folder as Newtonsoft.Json.Linq.JObject;
                                        if (folderJson != null && folderJson["Name"] != null)
                                        {
                                            var actualFolderName = folderJson["Name"].ToString();
                                            if (
                                                !string.IsNullOrEmpty(actualFolderName)
                                                && actualFolderName != currentPath
                                            )
                                            {
                                                Console.WriteLine(
                                                    $"UploadFileWithFolderPath: SharePoint returned actual folder name: '{actualFolderName}' (was '{currentPath}')"
                                                );
                                                currentPath = actualFolderName;
                                                logSegmentPath = WordSanitizer.Sanitize(currentPath);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(
                                            $"UploadFileWithFolderPath: Error extracting folder name from result: {ex.Message}"
                                        );
                                    }
                                }
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
                            Console.WriteLine($"UploadFileWithFolderPath: Creating folder: {logSegmentPath}");
                            _sharePointFileManager
                                .CreateFolder(documentTemplateUrlPart, currentPath)
                                .GetAwaiter()
                                .GetResult();
                        }
                    }
                }

                Console.WriteLine($"UploadFileWithFolderPath: Final folder path: '{currentPath}'");

                // Upload the file to the final folder path
                var fileName = _sharePointFileManager
                    .AddFile(
                        documentTemplateUrlPart,
                        currentPath,
                        request.FileName,
                        request.Data.ToByteArray(),
                        request.ContentType
                    )
                    .GetAwaiter()
                    .GetResult();

                // Get the server relative URL for the folder
                var folderServerRelativeUrl = string.IsNullOrEmpty(currentPath)
                    ? $"/{documentTemplateUrlPart}"
                    : $"/{documentTemplateUrlPart}/{currentPath}";

                result.FileName = fileName;
                result.ServerRelativeUrl = folderServerRelativeUrl;
                result.ResultStatus = ResultStatus.Success;

                Console.WriteLine(
                    $"FileManagerService - UploadFileWithFolderPath - successfully uploaded file '{fileName}' to entity '{request.EntityName}', path: '{folderServerRelativeUrl}'"
                );
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"UploadFileWithFolderPath - ERROR uploading file {logFileName}";
                Log.Error(
                    ex,
                    $"{result.ErrorDetail} - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}, Response: {ex.Response?.Content}"
                );
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"UploadFileWithFolderPath - ERROR uploading file {logFileName}";
                Log.Error(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }
    }
}
