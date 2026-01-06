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

namespace Gov.Lclb.Cllb.Services.FileManager;

/// <summary>
/// File Manager Service.
/// </summary>
[Authorize]
public class FileManagerService : FileManager.FileManagerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileManagerService> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public FileManagerService(
        IConfiguration configuration,
        ILogger<FileManagerService> logger,
        ILoggerFactory loggerFactory
    )
    {
        _configuration = configuration;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Returns a JWT token if the provided secret matches the configured secret.
    /// This token is required when making calls to the SharePoint service.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
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
            result.ErrorDetail = "Bad Request";
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Creates a folder in SharePoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<CreateFolderReply> CreateFolder(CreateFolderRequest request, ServerCallContext context)
    {
        var result = new CreateFolderReply();

        if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
        {
            result.ResultStatus = ResultStatus.Success;
            return Task.FromResult(result);
        }

        var logFolder = WordSanitizer.Sanitize(request.FolderName);

        var listTitle = GetSharePointFolderInternalName(request.EntityName);

        var _sharePointFileManager = new SharePointFileManager(_configuration, _loggerFactory);

        CreateDocumentLibraryIfMissing(listTitle, GetSharePointFolderInternalName(request.EntityName));

        bool folderExists = false;
        try
        {
            folderExists = _sharePointFileManager.FolderExists(listTitle, request.FolderName).GetAwaiter().GetResult();
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

        if (!folderExists)
        {
            try
            {
                _sharePointFileManager
                    .CreateFolder(GetSharePointFolderInternalName(request.EntityName), request.FolderName)
                    .GetAwaiter()
                    .GetResult();
                folderExists = _sharePointFileManager
                    .FolderExists(listTitle, request.FolderName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (SharePointRestException ex)
            {
                Log.Error(ex, result.ErrorDetail);
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in creating folder {logFolder}";
                folderExists = false;
            }
            catch (Exception e)
            {
                Log.Error(e, result.ErrorDetail);
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in creating folder {logFolder}";
                folderExists = false;
            }
        }

        if (folderExists)
        {
            result.ResultStatus = ResultStatus.Success;
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Determines if a file exists in SharePoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns>`true` if the file exists; otherwise, `false`.</returns>
    public override Task<FileExistsReply> FileExists(FileExistsRequest request, ServerCallContext context)
    {
        var result = new FileExistsReply();

        if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
        {
            result.ResultStatus = FileExistStatus.Exist;
            return Task.FromResult(result);
        }

        var _sharePointFileManager = new SharePointFileManager(_configuration, _loggerFactory);

        List<SharePointFileDetailsList> fileDetailsList = null;
        try
        {
            fileDetailsList = _sharePointFileManager
                .GetFileDetailsListInFolder(
                    GetSharePointFolderInternalName(request.EntityName),
                    request.FolderName,
                    request.DocumentType
                )
                .GetAwaiter()
                .GetResult();
            if (fileDetailsList != null)
            {
                var hasFile = fileDetailsList.Any(file => file.ServerRelativeUrl == request.ServerRelativeUrl);

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
            result.ErrorDetail = "Error determining if file exists";
        }
        catch (Exception e)
        {
            result.ResultStatus = FileExistStatus.Error;
            result.ErrorDetail = "Error determining if file exists";
            Log.Error(e, result.ErrorDetail);
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Deletes a file from SharePoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<DeleteFileReply> DeleteFile(DeleteFileRequest request, ServerCallContext context)
    {
        var result = new DeleteFileReply();

        if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
        {
            result.ResultStatus = ResultStatus.Success;
            return Task.FromResult(result);
        }

        var logUrl = WordSanitizer.Sanitize(request.ServerRelativeUrl);

        var _sharePointFileManager = new SharePointFileManager(_configuration, _loggerFactory);

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
            result.ErrorDetail = $"ERROR in deleting file {logUrl}";
            Log.Error(ex, result.ErrorDetail);
        }
        catch (Exception e)
        {
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail = $"ERROR in deleting file {logUrl}";
            Log.Error(e, result.ErrorDetail);
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Downloads a file from SharePoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
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
        var _sharePointFileManager = new SharePointFileManager(_configuration, _loggerFactory);

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
            result.ErrorDetail = $"ERROR in downloading file {logUrl}";
            Log.Error(ex, result.ErrorDetail);
        }
        catch (Exception e)
        {
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail = $"ERROR in downloading file {logUrl}";
            Log.Error(e, result.ErrorDetail);
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Uploads a file to the specified folder.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<UploadFileReply> UploadFile(UploadFileRequest request, ServerCallContext context)
    {
        var result = new UploadFileReply();

        var logFileName = WordSanitizer.Sanitize(request.FileName);
        var logFolderName = WordSanitizer.Sanitize(request.FolderName);

        try
        {
            if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
            {
                result.ResultStatus = ResultStatus.Success;
                result.FileName = request.FileName;
                return Task.FromResult(result);
            }

            var _sharePointFileManager = new SharePointFileManager(_configuration, _loggerFactory);

            CreateDocumentLibraryIfMissing(
                GetSharePointFolderInternalName(request.EntityName),
                GetSharePointFolderInternalName(request.EntityName)
            );

            var fileName = _sharePointFileManager
                .AddFile(
                    GetSharePointFolderInternalName(request.EntityName),
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
            result.ErrorDetail = $"ERROR in uploading file {logFileName} to folder {logFolderName}";
            Log.Error(ex, result.ErrorDetail);
        }
        catch (Exception e)
        {
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail = $"ERROR in uploading file {logFileName} to folder {logFolderName}";
            Log.Error(e, result.ErrorDetail);
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Returns the list of files in the specified folder.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
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
        List<SharePointFileDetailsList> fileDetailsList = null;
        var _sharePointFileManager = new SharePointFileManager(_configuration, _loggerFactory);
        try
        {
            fileDetailsList = _sharePointFileManager
                .GetFileDetailsListInFolder(
                    GetSharePointFolderInternalName(request.EntityName),
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
            result.ErrorDetail = "Error getting SharePoint File List";
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Returns the truncated filename as it would appear in SharePoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
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
            var _sharePointFileManager = new SharePointFileManager(_configuration, _loggerFactory);

            // Ask SharePoint whether this filename would be truncated upon upload
            var listTitle = GetSharePointFolderInternalName(request.EntityName);
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
            result.ErrorDetail = $"ERROR in getting truncated filename {logFileName} for folder {logFolderName}";
            Log.Error(ex, result.ErrorDetail);
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Creates the document library if it does not already exist.
    /// </summary>
    /// <param name="listTitle"></param>
    /// <param name="documentTemplateUrl"></param>
    private void CreateDocumentLibraryIfMissing(string listTitle, string documentTemplateUrl = null)
    {
        var _sharePointFileManager = new SharePointFileManager(_configuration, _loggerFactory);
        var exists = _sharePointFileManager.DocumentLibraryExists(listTitle).GetAwaiter().GetResult();
        if (!exists)
        {
            _sharePointFileManager.CreateDocumentLibrary(listTitle, documentTemplateUrl).GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// Maps a generic entity name to its SharePoint document library `name`.
    /// Example: "application" -> "adoxio_application".
    /// </summary>
    /// <remarks>
    /// There are a handful of places in the existing portal code that pass generic names like "application" when making
    /// file related calls. These could probably be replaced with the direct sharepoint constant
    /// (e.g., SharePointConstants.ApplicationFolderInternalName), but for safety with the existing code, this function
    /// has been left to do the mapping as it did originally.
    /// </remarks>
    /// <param name="entityName"></param>
    /// <returns></returns>
    private string GetSharePointFolderInternalName(string entityName)
    {
        switch (entityName.ToLower())
        {
            case "account":
                return SharePointConstants.AccountFolderInternalName;
            case "application":
                return SharePointConstants.ApplicationFolderInternalName;
            case "contact":
                return SharePointConstants.ContactFolderInternalName;
            case "worker":
                return SharePointConstants.WorkerFolderInternalName;
            case "event":
                return SharePointConstants.EventFolderInternalName;
            case "federal_report":
                return SharePointConstants.FederalReportFolderInternalName;
            case "licence":
                return SharePointConstants.LicenceFolderDisplayName;
            case "special_event":
                return SharePointConstants.SpecialEventFolderInternalName;
            default:
                return entityName;
        }
    }
}
