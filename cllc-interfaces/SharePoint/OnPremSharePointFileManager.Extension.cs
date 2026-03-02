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
        Console.WriteLine(
            $"findFolderOne - called with entityName='{entityName}', folderGuidSegment='{folderGuidSegment}'"
        );

        List<FolderItem> allResults = new List<FolderItem>();

        // Try searching for uppercase version
        var uppercaseResults = await SearchForGuidVariant(entityName, folderGuidSegment.ToUpper());

        if (uppercaseResults != null && uppercaseResults.Count > 0)
        {
            allResults.AddRange(uppercaseResults);
            Console.WriteLine(
                $"findFolderOne - Found {uppercaseResults.Count} folders with uppercase GUID"
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
            Console.WriteLine(
                $"findFolderOne - Found {lowercaseResults.Count} folders with lowercase GUID (added unique ones)"
            );
        }

        Console.WriteLine($"findFolderOne - Found {allResults.Count} total folders");

        // If 0 or 1 results, return as-is
        if (allResults.Count <= 1)
        {
            Console.WriteLine($"findFolderOne - Returning {allResults.Count} folders");
            return allResults;
        }

        // Multiple matches - disambiguate by checking child file counts first
        Console.WriteLine(
            $"findFolderOne - Multiple matches ({allResults.Count}), checking child file counts..."
        );

        var fileCounts = new Dictionary<FolderItem, int>();
        int maxFileCount = 0;

        foreach (var folder in allResults)
        {
            var childFiles = await GetChildFiles(folder.ServerRelativeUrl);
            int fileCount = childFiles?.Count ?? 0;
            fileCounts[folder] = fileCount;
            Console.WriteLine(
                $"findFolderOne - Folder '{folder.Name}' has {fileCount} child files"
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
            Console.WriteLine(
                $"findFolderOne - Returning folder with most child files ({maxFileCount}): '{foldersWithMaxFileCount[0].Name}'"
            );
            return new List<FolderItem> { foldersWithMaxFileCount[0] };
        }

        // Multiple folders tied on file count - check folder counts
        Console.WriteLine(
            $"findFolderOne - {foldersWithMaxFileCount.Count} folders tied with {maxFileCount} child files, checking folder counts..."
        );

        var folderCounts = new Dictionary<FolderItem, int>();
        int maxFolderCount = 0;

        foreach (var folder in foldersWithMaxFileCount)
        {
            var subFolders = await GetChildFolders(folder.ServerRelativeUrl);
            int folderCount = subFolders?.Count ?? 0;
            folderCounts[folder] = folderCount;
            Console.WriteLine(
                $"findFolderOne - Folder '{folder.Name}' has {folderCount} child folders"
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
        Console.WriteLine(
            $"findFolderOne - Returning folder with most child folders ({maxFolderCount}): '{foldersWithMaxFolderCount[0].Name}'"
        );
        return new List<FolderItem> { foldersWithMaxFolderCount[0] };
    }

    public async Task<List<FolderItem>> FindFolderTwo(
        string parentRelativePath,
        string rawFolderName
    )
    {
        Console.WriteLine(
            $"FindFolderTwo - called with parentRelativePath='{parentRelativePath}', rawFolderName='{rawFolderName}'"
        );

        // Sanitize the raw folder name
        string sanitizedFolderName = SharePointUtils.RemoveInvalidCharacters(rawFolderName);
        Console.WriteLine($"FindFolderTwo - Sanitized folder name: '{sanitizedFolderName}'");

        // Construct the full path
        string fullPath = parentRelativePath.TrimEnd('/') + "/" + sanitizedFolderName;
        Console.WriteLine($"FindFolderTwo - Full path to check: '{fullPath}'");

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

                Console.WriteLine(
                    $"FindFolderTwo - Found folder: '{folderItem.Name}' at '{folderItem.ServerRelativeUrl}'"
                );
                return new List<FolderItem> { folderItem };
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"FindFolderTwo - JSON parsing error: {ex.Message}");
                return new List<FolderItem>();
            }
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine($"FindFolderTwo - Folder not found at path: '{fullPath}'");
            return new List<FolderItem>();
        }
        else
        {
            Console.WriteLine(
                $"FindFolderTwo - Error StatusCode: {response.StatusCode}, Response: {jsonString}"
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
        Console.WriteLine(
            $"FindFolderThree - called with parentRelativePath='{parentRelativePath}', rawFolderNameSegment='{rawFolderNameSegment}', rawFolderGuidSegment='{rawFolderGuidSegment}'"
        );

        // Get all child folders under the parent
        var childFolders = await GetChildFolders(parentRelativePath);
        Console.WriteLine(
            $"FindFolderThree - Found {childFolders.Count} child folders under parent"
        );

        if (childFolders == null || childFolders.Count == 0)
        {
            Console.WriteLine("FindFolderThree - No child folders found, returning empty list");
            return new List<FolderItem>();
        }

        // Normalize the GUID for comparison (remove dashes, uppercase)
        string normalizedGuid = rawFolderGuidSegment?.Replace("-", "").ToUpper();
        Console.WriteLine($"FindFolderThree - Normalized GUID: '{normalizedGuid}'");

        // Filter folders that contain the GUID
        var matchingFolders = childFolders
            .Where(f =>
                !string.IsNullOrEmpty(normalizedGuid)
                && f.Name.Contains(normalizedGuid, StringComparison.OrdinalIgnoreCase)
            )
            .ToList();
        Console.WriteLine($"FindFolderThree - Found {matchingFolders.Count} folders matching GUID");

        if (matchingFolders.Count == 0)
        {
            Console.WriteLine("FindFolderThree - No folders match the GUID, returning empty list");
            return new List<FolderItem>();
        }
        else if (matchingFolders.Count == 1)
        {
            Console.WriteLine(
                $"FindFolderThree - Found exactly one match: '{matchingFolders[0].Name}'"
            );
            return new List<FolderItem> { matchingFolders[0] };
        }
        else
        {
            // Multiple matches - disambiguate by checking which has the most child files
            Console.WriteLine(
                $"FindFolderThree - Multiple matches ({matchingFolders.Count}), checking child file counts..."
            );

            var fileCounts = new Dictionary<FolderItem, int>();
            int maxFileCount = 0;

            foreach (var folder in matchingFolders)
            {
                var childFiles = await GetChildFiles(folder.ServerRelativeUrl);
                int fileCount = childFiles?.Count ?? 0;
                fileCounts[folder] = fileCount;
                Console.WriteLine(
                    $"FindFolderThree - Folder '{folder.Name}' has {fileCount} child files"
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
            Console.WriteLine(
                $"FindFolderThree - Returning folder with most child files ({maxFileCount}): '{foldersWithMaxFileCount[0].Name}'"
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

            Console.WriteLine(
                $"OnPremSharePointFileManager - CreateFolderOne - failed - Status: {_statusCode}, relativeUrl: '{relativeUrl}', EscapedRelativeUrl: '{relativeUrl}'"
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
        Console.WriteLine(
            $"OnPremSharePointFileManager - CreateRootFolder - successfully created folder '{relativeUrl}'"
        );
        Console.WriteLine(
            $"OnPremSharePointFileManager - CreateRootFolder - jsonString: {jsonString}"
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
        Console.WriteLine(
            $"UploadFile2 - called with serverRelativeUrl='{serverRelativeUrl}', fileName='{fileName}', contentType='{contentType}'"
        );

        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            Console.WriteLine("UploadFile2 - SharePoint is not valid, returning null");
            return null;
        }

        if (string.IsNullOrEmpty(serverRelativeUrl))
        {
            Console.WriteLine("UploadFile2 - serverRelativeUrl is null or empty, returning null");
            return null;
        }

        if (string.IsNullOrEmpty(fileName))
        {
            Console.WriteLine("UploadFile2 - fileName is null or empty, returning null");
            return null;
        }

        if (fileData == null || fileData.Length == 0)
        {
            Console.WriteLine("UploadFile2 - fileData is null or empty, returning null");
            return null;
        }

        // Build the upload request URI
        string escapedServerRelativeUrl = EscapeApostrophe(serverRelativeUrl);
        string escapedFileName = EscapeApostrophe(fileName);
        string requestUriString =
            ApiEndpoint
            + $"web/getFolderByServerRelativeUrl('{escapedServerRelativeUrl}')/files/add(url='{escapedFileName}',overwrite=true)";

        Console.WriteLine($"UploadFile2 - Request URI length: {requestUriString.Length}");

        // If URL is too long, try using folder ID instead
        int maxUrlLength = 400; // Conservative limit for SharePoint request URLs
        if (requestUriString.Length > maxUrlLength)
        {
            Console.WriteLine(
                $"UploadFile2 - URL too long ({requestUriString.Length} chars), attempting upload by folder ID"
            );

            try
            {
                string folderId = await GetFolderUniqueId(serverRelativeUrl);
                if (!string.IsNullOrEmpty(folderId))
                {
                    Console.WriteLine(
                        $"UploadFile2 - Found folder ID: {folderId}, using ID-based upload"
                    );
                    return await UploadFileByFolderId2(fileName, folderId, fileData, contentType);
                }
                else
                {
                    Console.WriteLine(
                        "UploadFile2 - Could not get folder ID, falling back to path-based upload"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"UploadFile2 - Error getting folder ID: {ex.Message}, falling back to path-based upload"
                );
            }
        }

        // Standard path-based upload
        Console.WriteLine($"UploadFile2 - Using path-based upload");

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
            Console.WriteLine(
                $"UploadFile2 - Successfully uploaded file '{fileName}' to '{serverRelativeUrl}'"
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

            Console.WriteLine(
                $"UploadFile2 - Failed to upload file '{fileName}' to '{serverRelativeUrl}'. Status: {response.StatusCode}, Response: {jsonString}"
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

        Console.WriteLine($"SearchForGuidVariant - Query: '{query}'");

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
                    Console.WriteLine($"SearchForGuidVariant - Found folder: '{folderItem.Name}'");
                }
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"SearchForGuidVariant - JSON parsing error: {ex.Message}");
                throw;
            }
        }
        else
        {
            Console.WriteLine(
                $"SearchForGuidVariant - Error StatusCode: {response.StatusCode}, Response: {jsonString}"
            );
        }

        return folderList;
    }

    /// <summary>
    /// Get all child folders in a folder by its server relative URL
    /// </summary>
    /// <param name="serverRelativeUrl">The server relative URL of the parent folder</param>
    /// <returns>List of child folders</returns>
    private async Task<List<FolderItem>> GetChildFolders(string serverRelativeUrl)
    {
        Console.WriteLine($"GetChildFolders - called with serverRelativeUrl='{serverRelativeUrl}'");

        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            Console.WriteLine("GetChildFolders - SharePoint is not valid, returning empty list");
            return new List<FolderItem>();
        }

        if (string.IsNullOrEmpty(serverRelativeUrl))
        {
            Console.WriteLine(
                "GetChildFolders - serverRelativeUrl is null or empty, returning empty list"
            );
            return new List<FolderItem>();
        }

        List<FolderItem> folderList = new List<FolderItem>();

        string query =
            $"web/getFolderByServerRelativeUrl('{EscapeApostrophe(serverRelativeUrl)}')/folders";
        Console.WriteLine($"GetChildFolders - Query: '{query}'");

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

                Console.WriteLine($"GetChildFolders - Found {folderList.Count} child folders");
            }
            catch (JsonReaderException jre)
            {
                Console.WriteLine($"GetChildFolders - JSON parsing error: {jre.Message}");
                throw;
            }
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine($"GetChildFolders - Folder not found at: '{serverRelativeUrl}'");
            return new List<FolderItem>();
        }
        else
        {
            Console.WriteLine(
                $"GetChildFolders - Error: StatusCode={response.StatusCode}, Response={jsonString}"
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
        Console.WriteLine($"GetChildFiles - called with serverRelativeUrl='{serverRelativeUrl}'");

        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            Console.WriteLine("GetChildFiles - SharePoint is not valid, returning empty list");
            return new List<FileItem>();
        }

        if (string.IsNullOrEmpty(serverRelativeUrl))
        {
            Console.WriteLine(
                "GetChildFiles - serverRelativeUrl is null or empty, returning empty list"
            );
            return new List<FileItem>();
        }

        List<FileItem> fileList = new List<FileItem>();

        string query =
            $"web/getFolderByServerRelativeUrl('{EscapeApostrophe(serverRelativeUrl)}')/files";
        Console.WriteLine($"GetChildFiles - Query: '{query}'");

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

                Console.WriteLine($"GetChildFiles - Found {fileList.Count} child files");
            }
            catch (JsonReaderException jre)
            {
                Console.WriteLine($"GetChildFiles - JSON parsing error: {jre.Message}");
                throw;
            }
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine($"GetChildFiles - Folder not found at: '{serverRelativeUrl}'");
            return new List<FileItem>();
        }
        else
        {
            Console.WriteLine(
                $"GetChildFiles - Error: StatusCode={response.StatusCode}, Response={jsonString}"
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
        Console.WriteLine(
            $"GetFolderUniqueId - called with serverRelativeUrl='{serverRelativeUrl}'"
        );

        if (!IsValid())
        {
            Console.WriteLine("GetFolderUniqueId - SharePoint is not valid, returning null");
            return null;
        }

        if (string.IsNullOrEmpty(serverRelativeUrl))
        {
            Console.WriteLine(
                "GetFolderUniqueId - serverRelativeUrl is null or empty, returning null"
            );
            return null;
        }

        try
        {
            string query =
                $"web/getFolderByServerRelativeUrl('{EscapeApostrophe(serverRelativeUrl)}')?$select=UniqueId";
            Console.WriteLine($"GetFolderUniqueId - Query: '{query}'");

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
                    Console.WriteLine($"GetFolderUniqueId - Found UniqueId: '{uniqueId}'");
                    return uniqueId;
                }
            }
            else
            {
                Console.WriteLine(
                    $"GetFolderUniqueId - Error StatusCode: {response.StatusCode}, Response: {jsonString}"
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetFolderUniqueId - Exception: {ex.Message}");
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
        Console.WriteLine(
            $"UploadFileByFolderId2 - called with folderId='{folderId}', fileName='{fileName}'"
        );

        string requestUriString =
            ApiEndpoint
            + "web/GetFolderById('"
            + folderId
            + "')/Files/add(url='"
            + EscapeApostrophe(fileName)
            + "',overwrite=true)";

        Console.WriteLine(
            $"UploadFileByFolderId2 - Using folder ID, URL length: {requestUriString.Length}"
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
            Console.WriteLine(
                $"UploadFileByFolderId2 - Successfully uploaded file '{fileName}' using folder ID"
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

            Console.WriteLine(
                $"UploadFileByFolderId2 - Failed to upload file '{fileName}'. Status: {response.StatusCode}, Response: {jsonString}"
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
