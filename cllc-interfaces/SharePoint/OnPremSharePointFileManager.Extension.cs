using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gov.Lclb.Cllb.Interfaces;

/// <summary>
/// Extension methods for OnPremSharePointFileManager to support new folder finding and file uploading logic.
/// </summary>
/// <remarks>
/// These were added to support the temporary period when Cloud Dynamics had to interface with On-Prem SharePoint, by
/// routing through the File Manager Service.
/// </remarks>
public partial class OnPremSharePointFileManager : ISharePointFileManager
{
    public async Task<List<FolderItem>> FindFolderOne(string entityName, string folderGuidSegment)
    {
        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderOne - called with entityName={EntityName}, folderGuidSegment={FolderGuidSegment}",
            entityName,
            folderGuidSegment
        );

        List<FolderItem> allResults = new List<FolderItem>();

        // Try searching for uppercase version
        var uppercaseResults = await SearchForGuidVariant(entityName, folderGuidSegment.ToUpper());

        if (uppercaseResults != null && uppercaseResults.Count > 0)
        {
            allResults.AddRange(uppercaseResults);
            _logger.LogDebug(
                "[OnPremSharePointFileManager] FindFolderOne - Found {Count} folders with uppercase GUID",
                uppercaseResults.Count
            );
        }

        // Try searching for lowercase version
        var lowercaseResults = await SearchForGuidVariant(entityName, folderGuidSegment.ToLower());
        if (lowercaseResults != null && lowercaseResults.Count > 0)
        {
            // Add only folders that aren't already in the results (avoid duplicates)
            foreach (var folder in lowercaseResults)
            {
                if (
                    !allResults.Any(f =>
                        f.ServerRelativeUrl.Equals(
                            folder.ServerRelativeUrl,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                )
                {
                    allResults.Add(folder);
                }
            }
            _logger.LogDebug(
                "[OnPremSharePointFileManager] FindFolderOne - Found {Count} folders with lowercase GUID (added unique ones)",
                lowercaseResults.Count
            );
        }

        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderOne - Found {TotalCount} total folders",
            allResults.Count
        );

        // If 0 or 1 results, return as-is
        if (allResults.Count <= 1)
        {
            _logger.LogDebug(
                "[OnPremSharePointFileManager] FindFolderOne - Returning {Count} folders",
                allResults.Count
            );
            return allResults;
        }

        // Multiple matches - disambiguate by checking child file counts first
        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderOne - Multiple matches ({Count}), checking child file counts...",
            allResults.Count
        );

        var fileCounts = new Dictionary<FolderItem, int>();
        int maxFileCount = 0;

        foreach (var folder in allResults)
        {
            var childFiles = await GetChildFiles(folder.ServerRelativeUrl);
            int fileCount = childFiles?.Count ?? 0;
            fileCounts[folder] = fileCount;
            _logger.LogDebug(
                "[OnPremSharePointFileManager] FindFolderOne - Folder {FolderName} has {FileCount} child files",
                folder.Name,
                fileCount
            );

            if (fileCount > maxFileCount)
            {
                maxFileCount = fileCount;
            }
        }

        // Get folders with the maximum file count
        var foldersWithMaxFileCount = allResults.Where(f => fileCounts[f] == maxFileCount).ToList();

        // If only one has the max file count, return it
        if (foldersWithMaxFileCount.Count == 1)
        {
            _logger.LogDebug(
                "[OnPremSharePointFileManager] FindFolderOne - Returning folder with most child files ({MaxFileCount}): {FolderName}",
                maxFileCount,
                foldersWithMaxFileCount[0].Name
            );
            return new List<FolderItem> { foldersWithMaxFileCount[0] };
        }

        // Multiple folders tied on file count - check folder counts
        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderOne - {Count} folders tied with {MaxFileCount} child files, checking folder counts...",
            foldersWithMaxFileCount.Count,
            maxFileCount
        );

        var folderCounts = new Dictionary<FolderItem, int>();
        int maxFolderCount = 0;

        foreach (var folder in foldersWithMaxFileCount)
        {
            var subFolders = await GetChildFolders(folder.ServerRelativeUrl);
            int folderCount = subFolders?.Count ?? 0;
            folderCounts[folder] = folderCount;
            _logger.LogDebug(
                "[OnPremSharePointFileManager] FindFolderOne - Folder {FolderName} has {FolderCount} child folders",
                folder.Name,
                folderCount
            );

            if (folderCount > maxFolderCount)
            {
                maxFolderCount = folderCount;
            }
        }

        // Get folders with the maximum folder count
        var foldersWithMaxFolderCount = foldersWithMaxFileCount
            .Where(f => folderCounts[f] == maxFolderCount)
            .ToList();

        // Return the one with the most folders (or the first one if tied)
        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderOne - Returning folder with most child folders ({MaxFolderCount}): {FolderName}",
            maxFolderCount,
            foldersWithMaxFolderCount[0].Name
        );
        return new List<FolderItem> { foldersWithMaxFolderCount[0] };
    }

    public async Task<List<FolderItem>> FindFolderTwo(
        string parentRelativePath,
        string rawFolderName
    )
    {
        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderTwo - called with parentRelativePath={ParentRelativePath}, rawFolderName={RawFolderName}",
            parentRelativePath,
            rawFolderName
        );

        // Sanitize the raw folder name
        string sanitizedFolderName = SharePointUtils.RemoveInvalidCharacters(rawFolderName);
        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderTwo - Sanitized folder name: {SanitizedFolderName}",
            sanitizedFolderName
        );

        // Construct the full path
        string fullPath = parentRelativePath.TrimEnd('/') + "/" + sanitizedFolderName;
        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderTwo - Full path to check: {FullPath}",
            fullPath
        );

        // Query SharePoint to see if the folder exists
        HttpRequestMessage request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(
                ApiEndpoint
                    + "web/getFolderByServerRelativeUrl('"
                    + EscapeApostrophe(fullPath)
                    + "')?$select=Name,ServerRelativeUrl,UniqueId,ItemCount"
            ),
            Headers = { { "Accept", "application/json" } },
        };

        var response = await _Client.SendAsync(request);
        string jsonString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            try
            {
                // Parse the folder response
                var folderJson = JObject.Parse(jsonString);
                var folderItem = new FolderItem
                {
                    Name = folderJson["Name"]?.ToString(),
                    ServerRelativeUrl = folderJson["ServerRelativeUrl"]?.ToString(),
                };

                _logger.LogDebug(
                    "[OnPremSharePointFileManager] FindFolderTwo - Found folder: {FolderName} at {ServerRelativeUrl}",
                    folderItem.Name,
                    folderItem.ServerRelativeUrl
                );
                return new List<FolderItem> { folderItem };
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(
                    ex,
                    "[OnPremSharePointFileManager] FindFolderTwo - JSON parsing error"
                );
                return new List<FolderItem>();
            }
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation(
                "[OnPremSharePointFileManager] FindFolderTwo - Folder not found at path: {FullPath}",
                fullPath
            );
            return new List<FolderItem>();
        }
        else
        {
            _logger.LogError(
                "[OnPremSharePointFileManager] FindFolderTwo - Error StatusCode: {StatusCode}, Response: {Response}",
                response.StatusCode,
                jsonString
            );
            return new List<FolderItem>();
        }
    }

    public async Task<List<FolderItem>> FindFolderThree(
        string parentRelativePath,
        string rawFolderNameSegment,
        string rawFolderGuidSegment
    )
    {
        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderThree - called with parentRelativePath={ParentRelativePath}, rawFolderNameSegment={RawFolderNameSegment}, rawFolderGuidSegment={RawFolderGuidSegment}",
            parentRelativePath,
            rawFolderNameSegment,
            rawFolderGuidSegment
        );

        // Get all child folders under the parent
        var childFolders = await GetChildFolders(parentRelativePath);
        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderThree - Found {Count} child folders under parent",
            childFolders.Count
        );

        if (childFolders == null || childFolders.Count == 0)
        {
            _logger.LogInformation(
                "[OnPremSharePointFileManager] FindFolderThree - No child folders found, returning empty list"
            );
            return new List<FolderItem>();
        }

        // Normalize the GUID for comparison (remove dashes, uppercase)
        string normalizedGuid = rawFolderGuidSegment?.Replace("-", "").ToUpper();
        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderThree - Normalized GUID: {NormalizedGuid}",
            normalizedGuid
        );

        // Filter folders that contain the GUID
        var matchingFolders = childFolders
            .Where(f =>
                !string.IsNullOrEmpty(normalizedGuid)
                && f.Name.Contains(normalizedGuid, StringComparison.OrdinalIgnoreCase)
            )
            .ToList();
        _logger.LogDebug(
            "[OnPremSharePointFileManager] FindFolderThree - Found {Count} folders matching GUID",
            matchingFolders.Count
        );

        if (matchingFolders.Count == 0)
        {
            _logger.LogInformation(
                "[OnPremSharePointFileManager] FindFolderThree - No folders match the GUID, returning empty list"
            );
            return new List<FolderItem>();
        }
        else if (matchingFolders.Count == 1)
        {
            _logger.LogDebug(
                "[OnPremSharePointFileManager] FindFolderThree - Found exactly one match: {FolderName}",
                matchingFolders[0].Name
            );
            return new List<FolderItem> { matchingFolders[0] };
        }
        else
        {
            // Multiple matches - disambiguate by checking which has the most child files
            _logger.LogDebug(
                "[OnPremSharePointFileManager] FindFolderThree - Multiple matches ({Count}), checking child file counts...",
                matchingFolders.Count
            );

            var fileCounts = new Dictionary<FolderItem, int>();
            int maxFileCount = 0;

            foreach (var folder in matchingFolders)
            {
                var childFiles = await GetChildFiles(folder.ServerRelativeUrl);
                int fileCount = childFiles?.Count ?? 0;
                fileCounts[folder] = fileCount;
                _logger.LogDebug(
                    "[OnPremSharePointFileManager] FindFolderThree - Folder {FolderName} has {FileCount} child files",
                    folder.Name,
                    fileCount
                );

                if (fileCount > maxFileCount)
                {
                    maxFileCount = fileCount;
                }
            }

            // Get folders with the maximum file count
            var foldersWithMaxFileCount = matchingFolders
                .Where(f => fileCounts[f] == maxFileCount)
                .ToList();

            // Return the one with the most files (or the first one if tied)
            _logger.LogDebug(
                "[OnPremSharePointFileManager] FindFolderThree - Returning folder with most child files ({MaxFileCount}): {FolderName}",
                maxFileCount,
                foldersWithMaxFileCount[0].Name
            );
            return new List<FolderItem> { foldersWithMaxFileCount[0] };
        }
    }

    public async Task<FolderItem> CreateFolder2(string relativeUrl)
    {
        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(ApiEndpoint + $"web/folders/add('{relativeUrl}')"),
            Headers = { { "Accept", "application/json" } },
        };

        StringContent strContent = new StringContent("", Encoding.UTF8);
        strContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            "application/json;odata=verbose"
        );

        endpointRequest.Content = strContent;

        var response = await _Client.SendAsync(endpointRequest);

        HttpStatusCode _statusCode = response.StatusCode;

        // check to see if the folder creation worked.
        if (!(_statusCode == HttpStatusCode.OK || _statusCode == HttpStatusCode.Created))
        {
            string _responseContent;

            var ex = new SharePointRestException(
                string.Format("Operation returned an invalid status code '{0}'", _statusCode)
            );

            _responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.LogError(
                "[OnPremSharePointFileManager] CreateFolder2 - failed - Status: {StatusCode}, relativeUrl: {RelativeUrl}, EscapedRelativeUrl: {EscapedRelativeUrl}",
                _statusCode,
                relativeUrl,
                relativeUrl
            );

            ex.Request = new HttpRequestMessageWrapper(endpointRequest, null);
            ex.Response = new HttpResponseMessageWrapper(response, _responseContent);

            endpointRequest.Dispose();

            if (response != null)
            {
                response.Dispose();
            }

            throw ex;
        }

        string jsonString = await response.Content.ReadAsStringAsync();
        _logger.LogInformation(
            "[OnPremSharePointFileManager] CreateFolder2 - successfully created folder {RelativeUrl}",
            relativeUrl
        );
        _logger.LogDebug(
            "[OnPremSharePointFileManager] CreateFolder2 - jsonString: {JsonString}",
            jsonString
        );
        FolderItem folderItem = JsonConvert.DeserializeObject<FolderItem>(jsonString);
        return folderItem;
    }

    /// <summary>
    /// Upload a file to a specified server relative URL
    /// </summary>
    /// <param name="serverRelativeUrl">The server relative URL of the folder where the file should be uploaded</param>
    /// <param name="fileName">The name of the file to upload</param>
    /// <param name="fileData">The file data as a byte array</param>
    /// <param name="contentType">The content type of the file</param>
    /// <returns>The uploaded file name</returns>
    public async Task<string> UploadFile2(
        string serverRelativeUrl,
        string fileName,
        byte[] fileData,
        string contentType
    )
    {
        _logger.LogDebug(
            "[OnPremSharePointFileManager] UploadFile2 - called with serverRelativeUrl={ServerRelativeUrl}, fileName={FileName}, contentType={ContentType}",
            serverRelativeUrl,
            fileName,
            contentType
        );

        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            _logger.LogWarning(
                "[OnPremSharePointFileManager] UploadFile2 - SharePoint is not valid, returning null"
            );
            return null;
        }

        if (string.IsNullOrEmpty(serverRelativeUrl))
        {
            _logger.LogWarning(
                "[OnPremSharePointFileManager] UploadFile2 - serverRelativeUrl is null or empty, returning null"
            );
            return null;
        }

        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogWarning(
                "[OnPremSharePointFileManager] UploadFile2 - fileName is null or empty, returning null"
            );
            return null;
        }

        if (fileData == null || fileData.Length == 0)
        {
            _logger.LogWarning(
                "[OnPremSharePointFileManager] UploadFile2 - fileData is null or empty, returning null"
            );
            return null;
        }

        // Build the upload request URI
        string escapedServerRelativeUrl = EscapeApostrophe(serverRelativeUrl);
        string escapedFileName = EscapeApostrophe(fileName);
        string requestUriString =
            ApiEndpoint
            + $"web/getFolderByServerRelativeUrl('{escapedServerRelativeUrl}')/files/add(url='{escapedFileName}',overwrite=true)";

        _logger.LogDebug(
            "[OnPremSharePointFileManager] UploadFile2 - Request URI length: {UriLength}",
            requestUriString.Length
        );

        // If URL is too long, try using folder ID instead
        int maxUrlLength = 400; // Conservative limit for SharePoint request URLs
        if (requestUriString.Length > maxUrlLength)
        {
            _logger.LogWarning(
                "[OnPremSharePointFileManager] UploadFile2 - URL too long ({UriLength} chars), attempting upload by folder ID",
                requestUriString.Length
            );

            try
            {
                string folderId = await GetFolderUniqueId(serverRelativeUrl);
                if (!string.IsNullOrEmpty(folderId))
                {
                    _logger.LogDebug(
                        "[OnPremSharePointFileManager] UploadFile2 - Found folder ID: {FolderId}, using ID-based upload",
                        folderId
                    );
                    return await UploadFileByFolderId2(fileName, folderId, fileData, contentType);
                }
                else
                {
                    _logger.LogWarning(
                        "[OnPremSharePointFileManager] UploadFile2 - Could not get folder ID, falling back to path-based upload"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[OnPremSharePointFileManager] UploadFile2 - Error getting folder ID, falling back to path-based upload"
                );
            }
        }

        // Standard path-based upload
        _logger.LogDebug("[OnPremSharePointFileManager] UploadFile2 - Using path-based upload");

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestUriString),
            Headers = { { "Accept", "application/json" } },
        };

        ByteArrayContent byteArrayContent = new ByteArrayContent(fileData);
        byteArrayContent.Headers.Add(@"content-length", fileData.Length.ToString());
        endpointRequest.Content = byteArrayContent;

        // Make the request
        var response = await _Client.SendAsync(endpointRequest);
        string jsonString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            _logger.LogInformation(
                "[OnPremSharePointFileManager] UploadFile2 - Successfully uploaded file {FileName} to {ServerRelativeUrl}",
                fileName,
                serverRelativeUrl
            );
            return fileName;
        }
        else
        {
            var ex = new SharePointRestException(
                string.Format(
                    "UploadFile2 - Operation returned an invalid status code '{0}'",
                    response.StatusCode
                )
            );

            _logger.LogError(
                "[OnPremSharePointFileManager] UploadFile2 - Failed to upload file {FileName} to {ServerRelativeUrl}. Status: {StatusCode}, Response: {Response}",
                fileName,
                serverRelativeUrl,
                response.StatusCode,
                jsonString
            );

            ex.Request = new HttpRequestMessageWrapper(endpointRequest, null);
            ex.Response = new HttpResponseMessageWrapper(response, jsonString);

            endpointRequest.Dispose();
            if (response != null)
            {
                response.Dispose();
            }

            throw ex;
        }
    }

    /// <summary>
    /// Search for a specific GUID variant (uppercase or lowercase) in folder names
    /// </summary>
    private async Task<List<FolderItem>> SearchForGuidVariant(string entityName, string guidVariant)
    {
        List<FolderItem> folderList = new List<FolderItem>();

        // Escape single quotes for OData query
        string escapedGuid = guidVariant.Replace("'", "''");

        // Build OData filter query
        string filter = $"$filter=substringof('{escapedGuid}',Name) and Name ne 'Forms'";
        string query =
            $"web/GetList('/{SharePointConstants.GetDocumentTemplateUrlPart(entityName)}')/rootFolder/folders?{filter}";

        _logger.LogDebug(
            "[OnPremSharePointFileManager] SearchForGuidVariant - Query: {Query}",
            query
        );

        HttpRequestMessage request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(ApiEndpoint + query),
            Headers = { { "Accept", "application/json" } },
        };

        // Send request
        var response = await _Client.SendAsync(request);

        // Read response
        string jsonString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            try
            {
                JObject responseObject = JObject.Parse(jsonString);
                List<JToken> responseResults = responseObject["value"].Children().ToList();

                foreach (JToken responseResult in responseResults)
                {
                    FolderItem folderItem = responseResult.ToObject<FolderItem>();
                    folderList.Add(folderItem);
                    _logger.LogDebug(
                        "[OnPremSharePointFileManager] SearchForGuidVariant - Found folder: {FolderName}",
                        folderItem.Name
                    );
                }
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(
                    ex,
                    "[OnPremSharePointFileManager] SearchForGuidVariant - JSON parsing error"
                );
                throw;
            }
        }
        else
        {
            _logger.LogError(
                "[OnPremSharePointFileManager] SearchForGuidVariant - Error StatusCode: {StatusCode}, Response: {Response}",
                response.StatusCode,
                jsonString
            );
        }

        return folderList;
    }

    /// <summary>
    /// Get all child folders in a folder by its server relative URL
    /// </summary>
    /// <param name="serverRelativeUrl">The server relative URL of the parent folder</param>
    /// <returns>List of child folders</returns>
    public async Task<List<FolderItem>> GetChildFolders(string serverRelativeUrl)
    {
        _logger.LogDebug(
            "[OnPremSharePointFileManager] GetChildFolders - called with serverRelativeUrl={ServerRelativeUrl}",
            serverRelativeUrl
        );

        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            _logger.LogWarning(
                "[OnPremSharePointFileManager] GetChildFolders - SharePoint is not valid, returning empty list"
            );
            return new List<FolderItem>();
        }

        if (string.IsNullOrEmpty(serverRelativeUrl))
        {
            _logger.LogWarning(
                "[OnPremSharePointFileManager] GetChildFolders - serverRelativeUrl is null or empty, returning empty list"
            );
            return new List<FolderItem>();
        }

        List<FolderItem> folderList = new List<FolderItem>();

        string query =
            $"web/getFolderByServerRelativeUrl('{EscapeApostrophe(serverRelativeUrl)}')/folders";
        _logger.LogDebug("[OnPremSharePointFileManager] GetChildFolders - Query: {Query}", query);

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(ApiEndpoint + query),
            Headers = { { "Accept", "application/json" } },
        };

        var response = await _Client.SendAsync(endpointRequest);
        string jsonString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            try
            {
                JObject responseObject = JObject.Parse(jsonString);
                List<JToken> responseResults = responseObject["value"].Children().ToList();

                foreach (JToken responseResult in responseResults)
                {
                    FolderItem folderItem = responseResult.ToObject<FolderItem>();

                    // Filter out system folders
                    if (!folderItem.Name.Equals("Forms", StringComparison.OrdinalIgnoreCase))
                    {
                        folderList.Add(folderItem);
                    }
                }

                _logger.LogDebug(
                    "[OnPremSharePointFileManager] GetChildFolders - Found {Count} child folders",
                    folderList.Count
                );
            }
            catch (JsonReaderException jre)
            {
                _logger.LogError(
                    jre,
                    "[OnPremSharePointFileManager] GetChildFolders - JSON parsing error"
                );
                throw;
            }
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation(
                "[OnPremSharePointFileManager] GetChildFolders - Folder not found at: {ServerRelativeUrl}",
                serverRelativeUrl
            );
            return new List<FolderItem>();
        }
        else
        {
            _logger.LogError(
                "[OnPremSharePointFileManager] GetChildFolders - Error: StatusCode={StatusCode}, Response={Response}",
                response.StatusCode,
                jsonString
            );
        }

        return folderList;
    }

    /// <summary>
    /// Get all child files in a folder by its server relative URL
    /// </summary>
    /// <param name="serverRelativeUrl">The server relative URL of the parent folder</param>
    /// <returns>List of child files</returns>
    private async Task<List<FileItem>> GetChildFiles(string serverRelativeUrl)
    {
        _logger.LogDebug(
            "[OnPremSharePointFileManager] GetChildFiles - called with serverRelativeUrl={ServerRelativeUrl}",
            serverRelativeUrl
        );

        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            _logger.LogWarning(
                "[OnPremSharePointFileManager] GetChildFiles - SharePoint is not valid, returning empty list"
            );
            return new List<FileItem>();
        }

        if (string.IsNullOrEmpty(serverRelativeUrl))
        {
            _logger.LogWarning(
                "[OnPremSharePointFileManager] GetChildFiles - serverRelativeUrl is null or empty, returning empty list"
            );
            return new List<FileItem>();
        }

        List<FileItem> fileList = new List<FileItem>();

        string query =
            $"web/getFolderByServerRelativeUrl('{EscapeApostrophe(serverRelativeUrl)}')/files";
        _logger.LogDebug("[OnPremSharePointFileManager] GetChildFiles - Query: {Query}", query);

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(ApiEndpoint + query),
            Headers = { { "Accept", "application/json" } },
        };

        var response = await _Client.SendAsync(endpointRequest);
        string jsonString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            try
            {
                JObject responseObject = JObject.Parse(jsonString);
                List<JToken> responseResults = responseObject["value"].Children().ToList();

                foreach (JToken responseResult in responseResults)
                {
                    FileItem fileItem = responseResult.ToObject<FileItem>();
                    fileList.Add(fileItem);
                }

                _logger.LogDebug(
                    "[OnPremSharePointFileManager] GetChildFiles - Found {Count} child files",
                    fileList.Count
                );
            }
            catch (JsonReaderException jre)
            {
                _logger.LogError(
                    jre,
                    "[OnPremSharePointFileManager] GetChildFiles - JSON parsing error"
                );
                throw;
            }
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation(
                "[OnPremSharePointFileManager] GetChildFiles - Folder not found at: {ServerRelativeUrl}",
                serverRelativeUrl
            );
            return new List<FileItem>();
        }
        else
        {
            _logger.LogError(
                "[OnPremSharePointFileManager] GetChildFiles - Error: StatusCode={StatusCode}, Response={Response}",
                response.StatusCode,
                jsonString
            );
        }

        return fileList;
    }

    /// <summary>
    /// Get the UniqueId (GUID) of a folder by its server relative URL
    /// </summary>
    /// <param name="serverRelativeUrl">The server relative URL of the folder</param>
    /// <returns>The folder's UniqueId as a string, or null if not found</returns>
    private async Task<string> GetFolderUniqueId(string serverRelativeUrl)
    {
        _logger.LogDebug(
            "[OnPremSharePointFileManager] GetFolderUniqueId - called with serverRelativeUrl={ServerRelativeUrl}",
            serverRelativeUrl
        );

        if (!IsValid())
        {
            _logger.LogWarning(
                "[OnPremSharePointFileManager] GetFolderUniqueId - SharePoint is not valid, returning null"
            );
            return null;
        }

        if (string.IsNullOrEmpty(serverRelativeUrl))
        {
            _logger.LogWarning(
                "[OnPremSharePointFileManager] GetFolderUniqueId - serverRelativeUrl is null or empty, returning null"
            );
            return null;
        }

        try
        {
            string query =
                $"web/getFolderByServerRelativeUrl('{EscapeApostrophe(serverRelativeUrl)}')?$select=UniqueId";
            _logger.LogDebug(
                "[OnPremSharePointFileManager] GetFolderUniqueId - Query: {Query}",
                query
            );

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(ApiEndpoint + query),
                Headers = { { "Accept", "application/json" } },
            };

            var response = await _Client.SendAsync(request);
            string jsonString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var folderJson = JObject.Parse(jsonString);
                if (folderJson["UniqueId"] != null)
                {
                    string uniqueId = folderJson["UniqueId"].ToString();
                    _logger.LogDebug(
                        "[OnPremSharePointFileManager] GetFolderUniqueId - Found UniqueId: {UniqueId}",
                        uniqueId
                    );
                    return uniqueId;
                }
            }
            else
            {
                _logger.LogError(
                    "[OnPremSharePointFileManager] GetFolderUniqueId - Error StatusCode: {StatusCode}, Response: {Response}",
                    response.StatusCode,
                    jsonString
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[OnPremSharePointFileManager] GetFolderUniqueId - Exception");
        }

        return null;
    }

    /// <summary>
    /// Upload a file using folder ID instead of path (avoids URL length issues)
    /// </summary>
    private async Task<string> UploadFileByFolderId2(
        string fileName,
        string folderId,
        byte[] fileData,
        string contentType
    )
    {
        _logger.LogDebug(
            "[OnPremSharePointFileManager] UploadFileByFolderId2 - called with folderId={FolderId}, fileName={FileName}",
            folderId,
            fileName
        );

        string requestUriString =
            ApiEndpoint
            + "web/GetFolderById('"
            + folderId
            + "')/Files/add(url='"
            + EscapeApostrophe(fileName)
            + "',overwrite=true)";

        _logger.LogDebug(
            "[OnPremSharePointFileManager] UploadFileByFolderId2 - Using folder ID, URL length: {UriLength}",
            requestUriString.Length
        );

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestUriString),
            Headers = { { "Accept", "application/json" } },
        };

        ByteArrayContent byteArrayContent = new ByteArrayContent(fileData);
        byteArrayContent.Headers.Add(@"content-length", fileData.Length.ToString());
        endpointRequest.Content = byteArrayContent;

        var response = await _Client.SendAsync(endpointRequest);
        string jsonString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            _logger.LogInformation(
                "[OnPremSharePointFileManager] UploadFileByFolderId2 - Successfully uploaded file {FileName} using folder ID",
                fileName
            );
            return fileName;
        }
        else
        {
            var ex = new SharePointRestException(
                string.Format(
                    "UploadFileByFolderId2 - Operation returned an invalid status code '{0}'",
                    response.StatusCode
                )
            );

            _logger.LogError(
                "[OnPremSharePointFileManager] UploadFileByFolderId2 - Failed to upload file {FileName}. Status: {StatusCode}, Response: {Response}",
                fileName,
                response.StatusCode,
                jsonString
            );

            ex.Request = new HttpRequestMessageWrapper(endpointRequest, null);
            ex.Response = new HttpResponseMessageWrapper(response, jsonString);

            endpointRequest.Dispose();
            if (response != null)
            {
                response.Dispose();
            }

            throw ex;
        }
    }
}
