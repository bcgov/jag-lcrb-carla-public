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
using Org.BouncyCastle.Asn1.Ocsp;
using Serilog;

namespace Gov.Lclb.Cllb.Services.FileManager;

[Authorize]
public partial class FileManagerService : FileManager.FileManagerBase
{
    const int MAX_TOTAL_LENGTH = 160; // Tested against OnPrem SharePoint to determine max path length that can be reliably used
    const int MAX_SEGMENT_LENGTH = 128;

    public override async Task<EnsureFolderPathReply> EnsureFolderPath(
        EnsureFolderPathRequest request,
        ServerCallContext context
    )
    {
        Console.WriteLine(
            $"EnsureFolderPath - START - EntityName: '{request.EntityName}', FolderPath segments: {request.FolderPath?.Count ?? 0}"
        );
        var result = await _ensureFolderPath(request);
        Console.WriteLine(
            $"EnsureFolderPath - END - ResultStatus: {result.ResultStatus}, ServerRelativeUrl: '{result.ServerRelativeUrl}'"
        );
        return result;
    }

    public override async Task<UploadFileReply> UploadFileWithFolderPath(
        UploadFileWithFolderPathRequest request,
        ServerCallContext context
    )
    {
        Console.WriteLine(
            $"UploadFileWithFolderPath - START - EntityName: '{request.EntityName}', FileName: '{request.FileName}', FolderPath segments: {request.FolderPath?.Count ?? 0}"
        );
        var ensureFolderPathRequest = new EnsureFolderPathRequest
        {
            EntityName = request.EntityName,
            FolderPath = { request.FolderPath },
        };

        Console.WriteLine($"UploadFileWithFolderPath - Ensuring folder path exists");
        var result = await _ensureFolderPath(ensureFolderPathRequest);

        Console.WriteLine($"UploadFileWithFolderPath - Folder path ensured at: '{result.ServerRelativeUrl}'");
        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        Console.WriteLine(
            $"UploadFileWithFolderPath - Uploading file '{request.FileName}' to '{result.ServerRelativeUrl}'"
        );
        var fileName = _sharePointFileManager
            .UploadFile2(result.ServerRelativeUrl, request.FileName, request.Data.ToByteArray(), request.ContentType)
            .GetAwaiter()
            .GetResult();

        var response = new UploadFileReply();

        response.FileName = fileName;
        response.ServerRelativeUrl = result.ServerRelativeUrl;
        response.ResultStatus = ResultStatus.Success;

        Console.WriteLine(
            $"UploadFileWithFolderPath - END - Successfully uploaded file '{fileName}' to '{result.ServerRelativeUrl}'"
        );
        return response;
    }

    public async Task<EnsureFolderPathReply> _ensureFolderPath(EnsureFolderPathRequest request)
    {
        Console.WriteLine(
            $"_ensureFolderPath - START - EntityName: '{request.EntityName}', FolderPath segments: {request.FolderPath?.Count ?? 0}"
        );
        var result = new EnsureFolderPathReply();

        if (_configuration["DISABLE_SHAREPOINT_INTEGRATION"] == "true")
        {
            Console.WriteLine($"_ensureFolderPath - SharePoint integration disabled");
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
                                guidSegment = _sanitizeGuid(guidSegment).ToUpper();
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

            Console.WriteLine(
                $"_ensureFolderPath - END (SharePoint disabled) - ServerRelativeUrl: '{result.ServerRelativeUrl}'"
            );
            return result;
        }

        Console.WriteLine($"_ensureFolderPath - Creating SharePointFileManager");
        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        var documentListTitle = SharePointConstants.GetDocumentListTitle(request.EntityName);
        var documentTemplateUrlPart = SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName);

        Console.WriteLine($"EnsureFolderPath: Building folder path with {request.FolderPath?.Count ?? 0} segments");

        // Validate request before processing folder segments
        Console.WriteLine($"_ensureFolderPath - Validating request");
        var validationResult = _validateEnsureFolderPathRequest(request);
        if (validationResult.ResultStatus == ResultStatus.Fail)
        {
            Console.WriteLine($"_ensureFolderPath - Validation FAILED: {validationResult.ErrorDetail}");
            return validationResult;
        }
        Console.WriteLine($"_ensureFolderPath - Validation passed");

        // 0) Create root document library if missing
        Console.WriteLine($"_ensureFolderPath - Ensuring document library '{documentListTitle}' exists");
        CreateDocumentLibraryIfMissing(documentListTitle, documentTemplateUrlPart);

        // 1) Fetch and/or create first folder segment under document library
        Console.WriteLine($"_ensureFolderPath - Ensuring first folder segment");
        var resultOne = await _ensureFolderOne(request);

        if (request.FolderPath.Count == 1)
        {
            result.ResultStatus = ResultStatus.Success;
            result.ServerRelativeUrl = resultOne.ServerRelativeUrl;
            result.ErrorDetail = "";

            Console.WriteLine($"_ensureFolderPath - END (1 segment) - ServerRelativeUrl: '{result.ServerRelativeUrl}'");
            return result;
        }

        // 2) Fetch and/or create second folder segment
        Console.WriteLine($"_ensureFolderPath - Ensuring second folder segment under '{resultOne.ServerRelativeUrl}'");
        var resultTwo = await _ensureFolderTwo(resultOne.ServerRelativeUrl, request);

        if (request.FolderPath.Count == 2)
        {
            result.ResultStatus = ResultStatus.Success;
            result.ServerRelativeUrl = resultTwo.ServerRelativeUrl;
            result.ErrorDetail = "";

            Console.WriteLine(
                $"_ensureFolderPath - END (2 segments) - ServerRelativeUrl: '{result.ServerRelativeUrl}'"
            );
            return result;
        }

        // 3) Fetch and/or create third folder segment
        Console.WriteLine($"_ensureFolderPath - Ensuring third folder segment under '{resultTwo.ServerRelativeUrl}'");
        var resultThree = await _ensureFolderThree(resultTwo.ServerRelativeUrl, request);

        result.ResultStatus = ResultStatus.Success;
        result.ServerRelativeUrl = resultThree.ServerRelativeUrl;
        result.ErrorDetail = "";

        Console.WriteLine($"_ensureFolderPath - END (3 segments) - ServerRelativeUrl: '{result.ServerRelativeUrl}'");
        return result;
    }

    public EnsureFolderPathReply _validateEnsureFolderPathRequest(EnsureFolderPathRequest request)
    {
        Console.WriteLine($"_validateEnsureFolderPathRequest - START");
        var result = new EnsureFolderPathReply();

        // Basic validation - ensure EntityName is provided
        if (string.IsNullOrEmpty(request.EntityName?.Trim()))
        {
            Console.WriteLine("_validateEnsureFolderPathRequest - EntityName is required");
            Log.Warning("EnsureFolderPath: Invalid request - EntityName is required");
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail = "EntityName is required";
            return result;
        }

        // Validate folder path segments if provided
        if (request.FolderPath == null || request.FolderPath.Count == 0)
        {
            Console.WriteLine("_validateEnsureFolderPathRequest - At least 1 FolderPath item is required");
            Log.Warning("EnsureFolderPath: Invalid request - At least 1 FolderPath item is required");
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail = "At least 1 FolderPath item is required";
            return result;
        }

        if (request.FolderPath.Count > 3)
        {
            Console.WriteLine("_validateEnsureFolderPathRequest - At most 3 FolderPath items are allowed");
            Log.Warning("EnsureFolderPath: Invalid request - At most 3 FolderPath items are allowed");
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail = "At most 3 FolderPath items are allowed";
            return result;
        }

        for (int i = 0; i < request.FolderPath.Count; i++)
        {
            var segment = request.FolderPath[i];
            Console.WriteLine($"_validateEnsureFolderPathRequest - Validating segment {i + 1}");
            var segmentValidationResult = _validateFolderSegment(segment, i);
            if (segmentValidationResult.ResultStatus == ResultStatus.Fail)
            {
                Console.WriteLine(
                    $"_validateEnsureFolderPathRequest - Segment {i + 1} validation failed: {segmentValidationResult.ErrorDetail}"
                );
                return segmentValidationResult;
            }
        }

        Console.WriteLine($"_validateEnsureFolderPathRequest - All validations passed");
        result.ResultStatus = ResultStatus.Success;
        return result;
    }

    public EnsureFolderPathReply _validateFolderSegment(FolderSegment segment, int segmentIndex)
    {
        Console.WriteLine($"_validateFolderSegment - Validating segment {segmentIndex + 1}");
        var result = new EnsureFolderPathReply();

        var folderName = segment.FolderName?.Trim();
        var folderNameSegment = segment.FolderNameSegment?.Trim();
        var folderGuidSegment = segment.FolderGuidSegment?.Trim();
        Console.WriteLine(
            $"_validateFolderSegment - FolderName: '{folderName}', FolderNameSegment: '{folderNameSegment}', FolderGuidSegment: '{folderGuidSegment}'"
        );

        if (
            string.IsNullOrEmpty(folderName)
            && string.IsNullOrEmpty(folderNameSegment)
            && string.IsNullOrEmpty(folderGuidSegment)
        )
        {
            Console.WriteLine($"_validateFolderSegment - Segment {segmentIndex + 1} has no valid folder identifier");
            result.ResultStatus = ResultStatus.Fail;
            result.ErrorDetail =
                $"Invalid folder path: Segment {segmentIndex + 1} must have at least one of FolderName or FolderGuidSegment";
            return result;
        }

        if (string.IsNullOrEmpty(folderName))
        {
            if (string.IsNullOrEmpty(folderGuidSegment))
            {
                Console.WriteLine($"_validateFolderSegment - Segment {segmentIndex + 1} missing required data");
                Log.Warning(
                    $"EnsureFolderPath: Invalid request - Segment {segmentIndex} has no FolderName and also no FolderGuidSegment. "
                        + $"FolderNameSegment: '{folderNameSegment ?? "null"}', FolderGuidSegment: '{folderGuidSegment ?? "null"}'"
                );
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail =
                    $"Invalid folder path: Segment {segmentIndex + 1} must have either FolderName or both FolderNameSegment and FolderGuidSegment";
                return result;
            }
        }

        Console.WriteLine($"_validateFolderSegment - Segment {segmentIndex + 1} validation passed");
        result.ResultStatus = ResultStatus.Success;
        return result;
    }

    public string _truncateFolderName(string name, int maxSegmentLength = MAX_SEGMENT_LENGTH)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        if (name.Length <= maxSegmentLength)
        {
            return name;
        }

        Console.WriteLine($"_truncateFolderName - Truncating '{name}' to {maxSegmentLength} characters");
        Log.Warning(
            $"Truncating folder name '{name}' to max length of {maxSegmentLength} characters to comply with SharePoint limits"
        );
        return name.Substring(0, maxSegmentLength);
    }

    public string _sanitizeGuid(string guid)
    {
        return guid.Trim().Replace("-", "").ToUpper();
    }

    public string _buildFolderNameFromSegment(
        string folderNameSegment,
        string folderGuidSegment,
        int maxSegmentLength = MAX_SEGMENT_LENGTH
    )
    {
        var sanitizedFolderGuidSegment = folderGuidSegment;
        if (!string.IsNullOrEmpty(folderGuidSegment))
        {
            sanitizedFolderGuidSegment = _sanitizeGuid(folderGuidSegment);
        }

        var maxFolderNameSegmentLength = maxSegmentLength - (sanitizedFolderGuidSegment.Length + 1); // +1 for the underscore separator

        var truncatedFolderNameSegment = _truncateFolderName(folderNameSegment, maxFolderNameSegmentLength);

        return $"{truncatedFolderNameSegment}_{sanitizedFolderGuidSegment}";
    }

    /// <summary>
    /// Find and/or create a level one folder (under a library).
    /// </summary>
    /// <param name="documentTemplateUrlPart"></param>
    /// <param name="segment"></param>
    /// <param name="createIfNotFound"></param>
    /// <returns></returns>
    public async Task<FolderItem> _ensureFolderOne(EnsureFolderPathRequest request)
    {
        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        var documentTemplateUrlPart = SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName);
        var segment = request.FolderPath[0];

        Console.WriteLine(
            $"_ensureFolderOne - called with documentTemplateUrlPart='{documentTemplateUrlPart}', segment.FolderName='{segment.FolderName}', segment.FolderNameSegment='{segment.FolderNameSegment}', segment.FolderGuidSegment='{segment.FolderGuidSegment}'"
        );

        string rawFolderName = _buildFolderNameFromSegment(segment.FolderNameSegment, segment.FolderGuidSegment);

        Console.WriteLine($"_ensureFolderOne - raw folder name from segment: '{rawFolderName}'");

        // Attempt to find a matching existing folder
        Console.WriteLine($"_ensureFolderOne - Searching for existing folder");
        var findFolderOneResults = await _sharePointFileManager.FindFolderOne(
            request.EntityName,
            segment.FolderGuidSegment
        );

        // No existing folder found, create it
        if (findFolderOneResults == null || findFolderOneResults.Count == 0)
        {
            Console.WriteLine($"_ensureFolderOne - No existing folder found, creating new folder");
            var createdFolder = await createFolderOne(request);

            return createdFolder;
        }

        Console.WriteLine(
            $"_ensureFolderOne - Found existing folder: '{findFolderOneResults.FirstOrDefault()?.ServerRelativeUrl}'"
        );
        return findFolderOneResults.FirstOrDefault();
    }

    /// <summary>
    /// Find and/or create a level two folder (under a level one folder).
    /// </summary>
    /// <param name="documentTemplateUrlPart"></param>
    /// <param name="segment"></param>
    /// <param name="createIfNotFound"></param>
    /// <returns></returns>
    public async Task<FolderItem> _ensureFolderTwo(string parentRelativePath, EnsureFolderPathRequest request)
    {
        if (request.FolderPath.Count < 2)
        {
            return null;
        }

        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        var segment = request.FolderPath[1];
        var folderName = segment.FolderName;
        var folderNameSegment = segment.FolderNameSegment;
        var folderGuidSegment = segment.FolderGuidSegment;

        Console.WriteLine(
            $"_ensureFolderTwo - called with entityName='{request.EntityName}', folderName='{folderName}', folderNameSegment='{folderNameSegment}', folderGuidSegment='{folderGuidSegment}'"
        );

        Console.WriteLine($"_ensureFolderTwo - folder name from segment: '{folderName}'");

        // Attempt to find a matching existing folder
        Console.WriteLine($"_ensureFolderTwo - Searching for existing folder");
        var findFolderTwoResults = await _sharePointFileManager.FindFolderTwo(parentRelativePath, folderName);

        // No existing folder found, create it
        if (findFolderTwoResults == null || findFolderTwoResults.Count == 0)
        {
            Console.WriteLine($"_ensureFolderTwo - No existing folder found, creating new folder");
            var createdFolder = await CreateFolder(request.EntityName, parentRelativePath, folderName);

            return createdFolder;
        }

        Console.WriteLine(
            $"_ensureFolderTwo - Found existing folder: '{findFolderTwoResults.FirstOrDefault()?.ServerRelativeUrl}'"
        );
        return findFolderTwoResults.FirstOrDefault();
    }

    /// <summary>
    /// Find and/or create a level three folder (under a level two folder).
    /// </summary>
    /// <param name="documentTemplateUrlPart"></param>
    /// <param name="segment"></param>
    /// <param name="createIfNotFound"></param>
    /// <returns></returns>
    public async Task<FolderItem> _ensureFolderThree(string parentRelativePath, EnsureFolderPathRequest request)
    {
        if (request.FolderPath.Count < 3)
        {
            return null;
        }

        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        var segment = request.FolderPath[2];
        var folderName = segment.FolderName;
        var folderNameSegment = segment.FolderNameSegment;
        var folderGuidSegment = segment.FolderGuidSegment;

        Console.WriteLine(
            $"_ensureFolderThree - called with entityName='{request.EntityName}', folderName='{folderName}', folderNameSegment='{folderNameSegment}', folderGuidSegment='{folderGuidSegment}'"
        );

        // Truncate folderNameSegment if path would be too long
        // Calculate: parentPath + "/" + folderNameSegment + "_" + folderGuidSegment + "/" + "fileName"
        var maxPathLength = MAX_TOTAL_LENGTH - 30; // Leave 30 characters for file names

        int currentPathLength =
            parentRelativePath.Length
            + (string.IsNullOrEmpty(folderNameSegment) ? 0 : folderNameSegment.Length)
            + (string.IsNullOrEmpty(folderGuidSegment) ? 0 : _sanitizeGuid(folderGuidSegment).Length)
            + 3; // 3 for the two slashes and the underscore

        Console.WriteLine(
            $"_ensureFolderThree - current path length is {currentPathLength} characters (max allowed: {maxPathLength} characters)"
        );

        if (currentPathLength > maxPathLength && !string.IsNullOrEmpty(folderNameSegment))
        {
            int excessLength = currentPathLength - maxPathLength;
            int newFolderNameSegmentLength = folderNameSegment.Length - excessLength;

            if (newFolderNameSegmentLength < 1)
            {
                newFolderNameSegmentLength = 1;
            }

            folderNameSegment = folderNameSegment.Substring(0, newFolderNameSegmentLength);
            Console.WriteLine(
                $"_ensureFolderThree - Truncated folderNameSegment to '{folderNameSegment}' (length: {newFolderNameSegmentLength}) to keep path under {maxPathLength} characters"
            );
        }

        string rawFolderName = _buildFolderNameFromSegment(folderNameSegment, folderGuidSegment);

        Console.WriteLine($"_ensureFolderThree - raw folder name from segment: '{rawFolderName}'");

        // Attempt to find a matching existing folder
        Console.WriteLine($"_ensureFolderThree - Searching for existing folder");
        var findFolderThreeResults = await _sharePointFileManager.FindFolderThree(
            parentRelativePath,
            folderNameSegment,
            folderGuidSegment
        );

        // No existing folder found, create it
        if (findFolderThreeResults == null || findFolderThreeResults.Count == 0)
        {
            Console.WriteLine($"_ensureFolderThree - No existing folder found, creating new folder");
            var createdFolder = await CreateFolder(request.EntityName, parentRelativePath, rawFolderName);

            return createdFolder;
        }

        Console.WriteLine(
            $"_ensureFolderThree - Found existing folder: '{findFolderThreeResults.FirstOrDefault()?.ServerRelativeUrl}'"
        );
        return findFolderThreeResults.FirstOrDefault();
    }

    public async Task<FolderItem> createFolderOne(EnsureFolderPathRequest request)
    {
        Console.WriteLine($"createFolderOne - START");
        var segment = request.FolderPath[0];

        var folderName = segment.FolderName;
        var folderNameSegment = segment.FolderNameSegment;
        var folderGuidSegment = segment.FolderGuidSegment;

        string rawFolderName = _buildFolderNameFromSegment(folderNameSegment, folderGuidSegment);

        Console.WriteLine($"createFolderOne - raw folder name from segment: '{rawFolderName}'");

        var createdFolder = await CreateFolder(
            request.EntityName,
            SharePointConstants.GetDocumentTemplateUrlPart(request.EntityName),
            segment.FolderNameSegment,
            segment.FolderGuidSegment
        );

        Console.WriteLine(
            $"createFolderOne - created folder with ServerRelativeUrl: '{createdFolder.ServerRelativeUrl}'"
        );

        return createdFolder;
    }

    public async Task<FolderItem> CreateFolder(string entityName, string parentRelativePath, string folderName)
    {
        Console.WriteLine($"CreateFolder - Creating folder '{folderName}' at '{parentRelativePath}'");
        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        var sanitizedFolderName = SharePointUtils.RemoveInvalidCharacters(folderName, entityName);
        Console.WriteLine($"CreateFolder - Sanitized folder name: '{sanitizedFolderName}'");

        string relativeUrl = SharePointUtils.EscapeApostrophe($"{parentRelativePath}/{sanitizedFolderName}");
        Console.WriteLine($"CreateFolder - Creating folder at relativeUrl: '{relativeUrl}'");

        var result = await _sharePointFileManager.CreateFolder2(relativeUrl);
        Console.WriteLine($"CreateFolder - Folder created successfully: '{result?.ServerRelativeUrl}'");
        return result;
    }

    public async Task<FolderItem> CreateFolder(
        string entityName,
        string parentRelativePath,
        string folderNameSegment,
        string folderGuidSegment
    )
    {
        Console.WriteLine(
            $"CreateFolder (with segments) - Creating folder with NameSegment: '{folderNameSegment}', GuidSegment: '{folderGuidSegment}' at '{parentRelativePath}'"
        );
        var _sharePointFileManager = SharePointFileManager.Create(_configuration, _loggerFactory);

        var folderName = _buildFolderNameFromSegment(folderNameSegment, folderGuidSegment);
        Console.WriteLine($"CreateFolder (with segments) - Built folder name: '{folderName}'");

        var sanitizedFolderName = SharePointUtils.RemoveInvalidCharacters(folderName, entityName);
        Console.WriteLine($"CreateFolder (with segments) - Sanitized folder name: '{sanitizedFolderName}'");

        string relativeUrl = SharePointUtils.EscapeApostrophe($"{parentRelativePath}/{sanitizedFolderName}");
        Console.WriteLine($"CreateFolder (with segments) - Creating folder at relativeUrl: '{relativeUrl}'");

        var result = await _sharePointFileManager.CreateFolder2(relativeUrl);
        Console.WriteLine($"CreateFolder (with segments) - Folder created successfully: '{result?.ServerRelativeUrl}'");
        return result;
    }
}
