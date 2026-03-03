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

/// <summary>
/// Methods and business logic for managing files in SharePoint.
/// </summary>
[Authorize]
public partial class FileManagerService : FileManager.FileManagerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileManagerService> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public FileManagerService(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<FileManagerService>();
        _logger.LogDebug(
            "[FileManagerService] Constructor - ILoggerFactory is {LoggerFactoryStatus}",
            loggerFactory == null ? "NULL" : "available"
        );
    }

    public override Task<CreateFolderReply> CreateFolder(CreateFolderRequest request, ServerCallContext context)
    {
        _logger.LogDebug(
            "[FileManagerService] CreateFolder - START - EntityName: {EntityName}, FolderName: {FolderName}",
            request.EntityName,
            request.FolderName
        );
        var result = new CreateFolderReply();

        if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
        {
            _logger.LogDebug("[FileManagerService] CreateFolder - SharePoint integration disabled");
            result.ResultStatus = ResultStatus.Success;
            return Task.FromResult(result);
        }

        var logFolder = request.FolderName;

        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        var listTitle = SharePointConstants.GetDocumentListTitle(request.EntityName);
        var urlTitle = SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName);
        _logger.LogDebug(
            "[FileManagerService] CreateFolder - ListTitle: {ListTitle}, UrlTitle: {UrlTitle}",
            listTitle,
            urlTitle
        );

        CreateDocumentLibraryIfMissing(listTitle, urlTitle);

        _logger.LogDebug("[FileManagerService] CreateFolder - Checking if folder exists");
        var folderExists = false;
        try
        {
            var folder = _sharePointFileManager.GetFolder(urlTitle, request.FolderName).GetAwaiter().GetResult();
            if (folder != null)
            {
                _logger.LogDebug("[FileManagerService] CreateFolder - Folder already exists");
                folderExists = true;
            }
        }
        catch (SharePointRestException ex)
        {
            _logger.LogError(
                ex,
                "[FileManagerService] CreateFolder - SharePointRestException creating sharepoint folder - Status: {StatusCode}, Request: {RequestUri}, Response: {Response}",
                ex.Response?.StatusCode,
                ex.Request?.RequestUri,
                ex.Response?.Content
            );
            folderExists = false;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[FileManagerService] CreateFolder - Generic Exception creating sharepoint folder");
            folderExists = false;
        }

        if (folderExists)
        {
            _logger.LogDebug("[FileManagerService] CreateFolder - Folder exists, returning success");
            result.ResultStatus = ResultStatus.Success;
        }
        else
        {
            _logger.LogDebug("[FileManagerService] CreateFolder - Folder does not exist, attempting to create");
            try
            {
                _sharePointFileManager.CreateFolder(urlTitle, request.FolderName).GetAwaiter().GetResult();
                var folder = _sharePointFileManager.GetFolder(urlTitle, request.FolderName).GetAwaiter().GetResult();
                if (folder != null)
                {
                    result.ResultStatus = ResultStatus.Success;
                    _logger.LogInformation(
                        "[FileManagerService] CreateFolder - successfully created folder {FolderName} in {ListTitle}",
                        logFolder,
                        listTitle
                    );
                }
            }
            catch (SharePointRestException ex) when (ex.Response?.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                // Forbidden could mean folder already exists (race condition)
                // Verify if folder actually exists
                Log.Warning(
                    ex,
                    $"[FileManagerService] CreateFolder - Received Forbidden when creating folder {logFolder}, verifying existence - Status: {ex.Response?.StatusCode}, Request: {ex.Request?.RequestUri}"
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
                        _logger.LogInformation(
                            "[FileManagerService] CreateFolder - folder {FolderName} verified to exist after Forbidden error in {ListTitle}",
                            logFolder,
                            listTitle
                        );
                    }
                    else
                    {
                        // Folder doesn't exist and we can't create it
                        result.ResultStatus = ResultStatus.Fail;
                        result.ErrorDetail =
                            $"[FileManagerService] CreateFolder - ERROR: Access denied creating folder {logFolder}";
                        _logger.LogError(
                            ex,
                            "{ErrorDetail} - Status: {StatusCode}, Request: {RequestUri}, Response: {Response}",
                            result.ErrorDetail,
                            ex.Response?.StatusCode,
                            ex.Request?.RequestUri,
                            ex.Response?.Content
                        );
                    }
                }
                catch (Exception verifyEx)
                {
                    // Verification failed - log original error
                    result.ResultStatus = ResultStatus.Fail;
                    result.ErrorDetail = $"[FileManagerService] CreateFolder - ERROR in creating folder {logFolder}";
                    _logger.LogError(
                        ex,
                        "{ErrorDetail} - Status: {StatusCode}, Request: {RequestUri}, Response: {Response}",
                        result.ErrorDetail,
                        ex.Response?.StatusCode,
                        ex.Request?.RequestUri,
                        ex.Response?.Content
                    );
                }
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"[FileManagerService] CreateFolder - ERROR in creating folder {logFolder}";
                _logger.LogError(
                    ex,
                    "{ErrorDetail} - Status: {StatusCode}, Request: {RequestUri}, Response: {Response}",
                    result.ErrorDetail,
                    ex.Response?.StatusCode,
                    ex.Request?.RequestUri,
                    ex.Response?.Content
                );
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"[FileManagerService] CreateFolder - ERROR in creating folder {logFolder}";
                _logger.LogError(e, "{ErrorDetail}", result.ErrorDetail);
            }
        }

        _logger.LogDebug("[FileManagerService] CreateFolder - END - ResultStatus: {ResultStatus}", result.ResultStatus);
        return Task.FromResult(result);
    }

    public override Task<FileExistsReply> FileExists(FileExistsRequest request, ServerCallContext context)
    {
        _logger.LogDebug(
            "[FileManagerService] FileExists - START - EntityName: {EntityName}, FolderName: {FolderName}, ServerRelativeUrl: {ServerRelativeUrl}",
            request.EntityName,
            request.FolderName,
            request.ServerRelativeUrl
        );
        var result = new FileExistsReply();

        if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
        {
            _logger.LogDebug("[FileManagerService] FileExists - SharePoint integration disabled");
            result.ResultStatus = FileExistStatus.Exist;
            return Task.FromResult(result);
        }

        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        _logger.LogDebug("[FileManagerService] FileExists - Getting file details list");
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
                    _logger.LogDebug(
                        "[FileManagerService] FileExists - file exists at {ServerRelativeUrl}",
                        request.ServerRelativeUrl
                    );
                }
                else
                {
                    result.ResultStatus = FileExistStatus.NotExist;
                    _logger.LogDebug(
                        "[FileManagerService] FileExists - file does not exist at {ServerRelativeUrl}",
                        request.ServerRelativeUrl
                    );
                }
            }
        }
        catch (SharePointRestException spre)
        {
            result.ResultStatus = FileExistStatus.Error;
            result.ErrorDetail = "FileExists - Error determining if file exists";
            _logger.LogError(
                spre,
                "{ErrorDetail} - Status: {StatusCode}, Request: {RequestUri}, Response: {Response}",
                result.ErrorDetail,
                spre.Response?.StatusCode,
                spre.Request?.RequestUri,
                spre.Response?.Content
            );
        }
        catch (Exception e)
        {
            result.ResultStatus = FileExistStatus.Error;
            result.ErrorDetail = "[FileManagerService] FileExists - Error determining if file exists";
            _logger.LogError(e, "{ErrorDetail}", result.ErrorDetail);
        }

        _logger.LogDebug("[FileManagerService] FileExists - END - ResultStatus: {ResultStatus}", result.ResultStatus);
        return Task.FromResult(result);
    }

    private void CreateDocumentLibraryIfMissing(string listTitle, string documentTemplateUrl = null)
    {
        _logger.LogDebug(
            "[FileManagerService] CreateDocumentLibraryIfMissing - START - ListTitle: {ListTitle}",
            listTitle
        );
        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);
        var exists = _sharePointFileManager.DocumentLibraryExists(listTitle).GetAwaiter().GetResult();
        _logger.LogDebug("[FileManagerService] CreateDocumentLibraryIfMissing - Library exists: {Exists}", exists);
        if (!exists)
        {
            _logger.LogDebug("[FileManagerService] CreateDocumentLibraryIfMissing - Creating document library");
            _sharePointFileManager.CreateDocumentLibrary(listTitle, documentTemplateUrl).GetAwaiter().GetResult();
            _logger.LogInformation(
                "[FileManagerService] CreateDocumentLibraryIfMissing - Document library {ListTitle} created",
                listTitle
            );
        }
        _logger.LogDebug("[FileManagerService] CreateDocumentLibraryIfMissing - END");
    }

    public override Task<DeleteFileReply> DeleteFile(DeleteFileRequest request, ServerCallContext context)
    {
        _logger.LogDebug(
            "[FileManagerService] DeleteFile - START - ServerRelativeUrl: {ServerRelativeUrl}",
            request.ServerRelativeUrl
        );
        var result = new DeleteFileReply();

        if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
        {
            _logger.LogDebug("[FileManagerService] DeleteFile - SharePoint integration disabled");
            result.ResultStatus = ResultStatus.Success;
            return Task.FromResult(result);
        }

        var logUrl = request.ServerRelativeUrl;

        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        _logger.LogDebug("[FileManagerService] DeleteFile - Calling SharePoint delete");
        try
        {
            var success = _sharePointFileManager.DeleteFile(request.ServerRelativeUrl).GetAwaiter().GetResult();

            if (success)
            {
                result.ResultStatus = ResultStatus.Success;
                _logger.LogInformation("[FileManagerService] DeleteFile - successfully deleted file at {Url}", logUrl);
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
            _logger.LogError(
                ex,
                "{ErrorDetail} - Status: {StatusCode}, Request: {RequestUri}, Response: {Response}",
                result.ErrorDetail,
                ex.Response?.StatusCode,
                ex.Request?.RequestUri,
                ex.Response?.Content
            );
        }
        catch (Exception e)
        {
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail = $"[FileManagerService] DeleteFile - ERROR in deleting file {logUrl}";
            _logger.LogError(e, "{ErrorDetail}", result.ErrorDetail);
        }

        _logger.LogDebug("[FileManagerService] DeleteFile - END - ResultStatus: {ResultStatus}", result.ResultStatus);
        return Task.FromResult(result);
    }

    public override Task<DownloadFileReply> DownloadFile(DownloadFileRequest request, ServerCallContext context)
    {
        _logger.LogDebug(
            "[FileManagerService] DownloadFile - START - ServerRelativeUrl: {ServerRelativeUrl}",
            request.ServerRelativeUrl
        );
        var result = new DownloadFileReply();

        if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
        {
            _logger.LogDebug("[FileManagerService] DownloadFile - SharePoint integration disabled");
            result.ResultStatus = ResultStatus.Success;
            result.Data = ByteString.CopyFrom(new byte[0]);
            return Task.FromResult(result);
        }

        var logUrl = request.ServerRelativeUrl;
        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        _logger.LogDebug("[FileManagerService] DownloadFile - Calling SharePoint download");
        try
        {
            var data = _sharePointFileManager.DownloadFile(request.ServerRelativeUrl).GetAwaiter().GetResult();

            if (data != null)
            {
                result.ResultStatus = ResultStatus.Success;
                result.Data = ByteString.CopyFrom(data);
                _logger.LogInformation(
                    "[FileManagerService] DownloadFile - successfully downloaded file from {Url}, size: {Size} bytes",
                    logUrl,
                    data.Length
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
            _logger.LogError(
                ex,
                "{ErrorDetail} - Status: {StatusCode}, Request: {RequestUri}, Response: {Response}",
                result.ErrorDetail,
                ex.Response?.StatusCode,
                ex.Request?.RequestUri,
                ex.Response?.Content
            );
        }
        catch (Exception e)
        {
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail = $"[FileManagerService] DownloadFile - ERROR in downloading file {logUrl}";
            _logger.LogError(e, "{ErrorDetail}", result.ErrorDetail);
        }

        _logger.LogDebug("[FileManagerService] DownloadFile - END - ResultStatus: {ResultStatus}", result.ResultStatus);
        return Task.FromResult(result);
    }

    public override Task<UploadFileReply> UploadFile(UploadFileRequest request, ServerCallContext context)
    {
        _logger.LogDebug(
            "[FileManagerService] UploadFile - START - EntityName: {EntityName}, FolderName: {FolderName}, FileName: {FileName}",
            request.EntityName,
            request.FolderName,
            request.FileName
        );
        var result = new UploadFileReply();

        if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
        {
            _logger.LogDebug("[FileManagerService] UploadFile - SharePoint integration disabled");
            result.ResultStatus = ResultStatus.Success;
            result.FileName = request.FileName;
            return Task.FromResult(result);
        }

        var logFileName = request.FileName;
        var logFolderName = request.FolderName;

        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        var listTitle = SharePointConstants.GetDocumentListTitle(request.EntityName);
        var documentTemplateUrlPart = SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName);

        _logger.LogInformation(
            "[FileManagerService] UploadFile - Uploading file {FileName} to entity {EntityName}, folder {FolderName}",
            logFileName,
            request.EntityName,
            logFolderName
        );

        CreateDocumentLibraryIfMissing(listTitle, documentTemplateUrlPart);

        try
        {
            // Create intermediate folders by calling EnsureFolderPath if the path contains multiple segments
            if (!string.IsNullOrEmpty(request.FolderName) && request.FolderName.Contains("/"))
            {
                _logger.LogDebug(
                    "[FileManagerService] UploadFile - Multi-level folder path detected, ensuring folder structure"
                );

                var ensureRequest = new EnsureFolderPathRequest { EntityName = request.EntityName };

                // Split the folder path and create segments
                var pathSegments = request.FolderName.Split('/', StringSplitOptions.RemoveEmptyEntries);
                _logger.LogDebug("[FileManagerService] UploadFile - Split into {Count} segments", pathSegments.Length);

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

                _logger.LogDebug("[FileManagerService] UploadFile - Folder structure ensured successfully");
            }
            else if (!string.IsNullOrEmpty(request.FolderName))
            {
                _logger.LogDebug(
                    "[FileManagerService] UploadFile - Single-level folder path, AddFile will handle creation if needed"
                );
            }
            else
            {
                _logger.LogDebug(
                    "[FileManagerService] UploadFile - No folder specified, uploading to root of document library"
                );
            }

            // Upload the file (AddFile will handle single-level folder creation if needed)
            _logger.LogDebug("[FileManagerService] UploadFile - Calling AddFile to upload file");

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

            _logger.LogInformation(
                "[FileManagerService] UploadFile - successfully uploaded file {FileName} to entity {EntityName}, folder {FolderName}",
                fileName,
                request.EntityName,
                logFolderName
            );
        }
        catch (SharePointRestException ex)
        {
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail = $"UploadFile - ERROR in uploading file {logFileName} to folder {logFolderName}";
            _logger.LogError(
                ex,
                "{ErrorDetail} - Status: {StatusCode}, Request: {RequestUri}, Response: {Response}",
                result.ErrorDetail,
                ex.Response?.StatusCode,
                ex.Request?.RequestUri,
                ex.Response?.Content
            );
        }
        catch (Exception e)
        {
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail =
                $"[FileManagerService] UploadFile - ERROR in uploading file {logFileName} to folder {logFolderName}";
            _logger.LogError(e, "{ErrorDetail}", result.ErrorDetail);
        }

        _logger.LogDebug("[FileManagerService] UploadFile - END - ResultStatus: {ResultStatus}", result.ResultStatus);
        return Task.FromResult(result);
    }

    public override Task<FolderFilesReply> FolderFiles(FolderFilesRequest request, ServerCallContext context)
    {
        _logger.LogDebug(
            "[FileManagerService] FolderFiles - START - EntityName: {EntityName}, FolderName: {FolderName}, DocumentType: {DocumentType}",
            request.EntityName,
            request.FolderName,
            request.DocumentType
        );
        var result = new FolderFilesReply();

        if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
        {
            _logger.LogDebug("[FileManagerService] FolderFiles - SharePoint integration disabled");
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
        _logger.LogDebug("[FileManagerService] FolderFiles - Getting file details list from SharePoint");
        var documentTemplateUrlPart = SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName);
        _logger.LogDebug(
            "[FileManagerService] FolderFiles - DocumentTemplateUrlPart: {DocumentTemplateUrlPart}",
            documentTemplateUrlPart
        );
        List<SharePointFileDetailsList> fileDetailsList = null;
        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);
        _logger.LogDebug("[FileManagerService] FolderFiles - SharePointFileManager created");
        try
        {
            _logger.LogDebug("[FileManagerService] FolderFiles - Calling GetFileDetailsListInFolder");
            fileDetailsList = _sharePointFileManager
                .GetFileDetailsListInFolder(documentTemplateUrlPart, request.FolderName, request.DocumentType)
                .GetAwaiter()
                .GetResult();
            _logger.LogDebug(
                "[FileManagerService] FolderFiles - GetFileDetailsListInFolder returned {Count} items",
                fileDetailsList?.Count ?? 0
            );
            if (fileDetailsList != null)
            {
                _logger.LogDebug(
                    "[FileManagerService] FolderFiles - Processing {Count} file(s)",
                    fileDetailsList.Count
                );
                // gRPC ensures that the collection has space to accept new data; no need to call a constructor
                int processedCount = 0;
                foreach (var item in fileDetailsList)
                {
                    _logger.LogDebug(
                        "[FileManagerService] FolderFiles - Processing file #{Index}: Name={Name}, DocumentType={DocumentType}, ServerRelativeUrl={ServerRelativeUrl}",
                        processedCount + 1,
                        item.Name,
                        item.DocumentType,
                        item.ServerRelativeUrl
                    );
                    // Sharepoint API responds with dates in UTC format
                    var utcFormat = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
                    DateTime parsedCreateDate,
                        parsedLastModified;
                    bool createDateParsed = DateTime.TryParse(
                        item.TimeCreated,
                        CultureInfo.InvariantCulture,
                        utcFormat,
                        out parsedCreateDate
                    );
                    bool modifiedDateParsed = DateTime.TryParse(
                        item.TimeLastModified,
                        CultureInfo.InvariantCulture,
                        utcFormat,
                        out parsedLastModified
                    );
                    _logger.LogDebug(
                        "[FileManagerService] FolderFiles - Date parsing: TimeCreated={TimeCreated} (parsed: {CreateDateParsed}), TimeLastModified={TimeLastModified} (parsed: {ModifiedDateParsed})",
                        item.TimeCreated,
                        createDateParsed,
                        item.TimeLastModified,
                        modifiedDateParsed
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

                    _logger.LogDebug(
                        "[FileManagerService] FolderFiles - Created FileSystemItem: Size={Size} bytes",
                        newItem.Size
                    );
                    result.Files.Add(newItem);
                    processedCount++;
                }

                result.ResultStatus = ResultStatus.Success;
                _logger.LogInformation(
                    "[FileManagerService] FolderFiles - successfully retrieved {Count} files from folder {FolderName} in entity {EntityName}",
                    result.Files.Count,
                    request.FolderName,
                    request.EntityName
                );
            }
            else
            {
                _logger.LogDebug("[FileManagerService] FolderFiles - fileDetailsList is null, no files found");
                result.ResultStatus = ResultStatus.Success;
            }
        }
        catch (SharePointRestException spre)
        {
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail = "[FileManagerService] FolderFiles - Error getting SharePoint File List";
            _logger.LogError(
                spre,
                "{ErrorDetail} - Status: {StatusCode}, Request: {RequestUri}, Response: {Response}",
                result.ErrorDetail,
                spre.Response?.StatusCode,
                spre.Request?.RequestUri,
                spre.Response?.Content
            );
        }
        catch (Exception ex)
        {
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail = "[FileManagerService] FolderFiles - Unexpected error getting SharePoint File List";
            _logger.LogError(ex, "{ErrorDetail}", result.ErrorDetail);
        }

        _logger.LogDebug("[FileManagerService] FolderFiles - END - ResultStatus: {ResultStatus}", result.ResultStatus);
        return Task.FromResult(result);
    }

    [AllowAnonymous]
    public override Task<TokenReply> GetToken(TokenRequest request, ServerCallContext context)
    {
        _logger.LogDebug("[FileManagerService] GetToken - START");
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
            _logger.LogInformation("[FileManagerService] GetToken - successfully generated authentication token");
        }
        else
        {
            _logger.LogWarning("[FileManagerService] GetToken - Invalid secret provided");
            result.ErrorDetail = "[FileManagerService] GetToken - Invalid Secret";
        }

        _logger.LogDebug("[FileManagerService] GetToken - END - ResultStatus: {ResultStatus}", result.ResultStatus);
        return Task.FromResult(result);
    }

    public override Task<TruncatedFilenameReply> GetTruncatedFilename(
        TruncatedFilenameRequest request,
        ServerCallContext context
    )
    {
        _logger.LogDebug(
            "[FileManagerService] GetTruncatedFilename - START - EntityName: {EntityName}, FileName: {FileName}, FolderName: {FolderName}",
            request.EntityName,
            request.FileName,
            request.FolderName
        );
        var result = new TruncatedFilenameReply();

        if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
        {
            _logger.LogDebug("[FileManagerService] GetTruncatedFilename - SharePoint integration disabled");
            result.ResultStatus = ResultStatus.Success;
            result.FileName = request.FileName;
            return Task.FromResult(result);
        }

        var logFileName = request.FileName;
        var logFolderName = request.FolderName;

        try
        {
            var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

            // Ask SharePoint whether this filename would be truncated upon upload
            var urlTitle = SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName);
            var maybeTruncated = _sharePointFileManager.GetTruncatedFileName(
                request.FileName,
                urlTitle,
                request.FolderName
            );
            result.FileName = maybeTruncated;
            result.ResultStatus = ResultStatus.Success;
            _logger.LogInformation(
                "[FileManagerService] GetTruncatedFilename - successfully computed filename {TruncatedFileName} for {FileName}",
                maybeTruncated,
                logFileName
            );
        }
        catch (SharePointRestException ex)
        {
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail =
                $"[FileManagerService] GetTruncatedFilename - ERROR in getting truncated filename {logFileName} for folder {logFolderName}";
            _logger.LogError(
                ex,
                "{ErrorDetail} - Status: {StatusCode}, Request: {RequestUri}, Response: {Response}",
                result.ErrorDetail,
                ex.Response?.StatusCode,
                ex.Request?.RequestUri,
                ex.Response?.Content
            );
        }

        _logger.LogDebug(
            "[FileManagerService] GetTruncatedFilename - END - ResultStatus: {ResultStatus}",
            result.ResultStatus
        );
        return Task.FromResult(result);
    }
}
