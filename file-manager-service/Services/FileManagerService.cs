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

namespace Gov.Lclb.Cllb.Services.FileManager;

// Default to require authorization
[Authorize]
public partial class FileManagerService : FileManager.FileManagerBase
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

        var logFolder = request.FolderName;

        var _sharePointFileManager = SharePointFileManager.Create(_configuration);

        var listTitle = SharePointConstants.GetDocumentListTitle(request.EntityName);
        var urlTitle = SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName);

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
                var folder = _sharePointFileManager.GetFolder(urlTitle, request.FolderName).GetAwaiter().GetResult();
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

        var _sharePointFileManager = SharePointFileManager.Create(_configuration);

        List<SharePointFileDetailsList> fileDetailsList = null;
        try
        {
            fileDetailsList = _sharePointFileManager
                .GetFileDetailsListInFolder(
                    SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName),
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
                        $"FileManagerService - FileExists - file exists at '{request.ServerRelativeUrl}'"
                    );
                }
                else
                {
                    result.ResultStatus = FileExistStatus.NotExist;
                    Console.WriteLine(
                        $"FileManagerService - FileExists - file does not exist at '{request.ServerRelativeUrl}'"
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
        var _sharePointFileManager = SharePointFileManager.Create(_configuration);
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

        var logUrl = request.ServerRelativeUrl;

        var _sharePointFileManager = SharePointFileManager.Create(_configuration);

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

        var logUrl = request.ServerRelativeUrl;
        var _sharePointFileManager = SharePointFileManager.Create(_configuration);

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

        var logFileName = request.FileName;
        var logFolderName = request.FolderName;

        var _sharePointFileManager = SharePointFileManager.Create(_configuration);

        var listTitle = SharePointConstants.GetDocumentListTitle(request.EntityName);
        var documentTemplateUrlPart = SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName);

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
                    TimeLastModified = Timestamp.FromDateTime(DateTime.UtcNow),
                }
            );
            return Task.FromResult(result);
        }

        // Get the file details list in folder
        List<SharePointFileDetailsList> fileDetailsList = null;
        var _sharePointFileManager = SharePointFileManager.Create(_configuration);
        try
        {
            fileDetailsList = _sharePointFileManager
                .GetFileDetailsListInFolder(
                    SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName),
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
                    DateTime.TryParse(item.TimeCreated, CultureInfo.InvariantCulture, utcFormat, out parsedCreateDate);
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
                        TimeLastModified = Timestamp.FromDateTime(parsedLastModified),
                    };

                    result.Files.Add(newItem);
                }

                result.ResultStatus = ResultStatus.Success;
                Console.WriteLine(
                    $"FileManagerService - FolderFiles - successfully retrieved {result.Files.Count} files from folder '{request.FolderName}' in entity '{request.EntityName}'"
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

        var logFileName = request.FileName;
        var logFolderName = request.FolderName;

        try
        {
            var _sharePointFileManager = SharePointFileManager.Create(_configuration);

            // Ask SharePoint whether this filename would be truncated upon upload
            var urlTitle = SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName);
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
}
