using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gov.Lclb.Cllb.Interfaces;

/// <summary>
/// SharePoint file manager using Microsoft Graph API.
/// </summary>
public class SharePointFileManager
{
    private const int UploadSessionThreshold = 4 * 1024 * 1024; // 4MB
    private const int ChunkSize = 320 * 1024 * 10; // 3.2MB
    private const int HttpClientTimeoutSeconds = 100;
    private const int MaxAuthRetries = 3;
    private const int InitialRetryDelayMs = 1000;

    private AuthenticationResult authenticationResult;
    private IConfidentialClientApplication confidentialClientApp;
    private const string GraphScope = "https://graph.microsoft.com/.default";
    private HttpClient _Client;

    private readonly ILogger<SharePointFileManager> _logger;

    public string SiteUrl { get; set; }
    public string SiteId { get; set; }
    public string GraphApiEndpoint { get; set; } = "https://graph.microsoft.com/v1.0/";

    public SharePointFileManager(IConfiguration Configuration, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<SharePointFileManager>();

        // Create the HttpClient for Graph API calls
        _Client = new HttpClient();
        _Client.Timeout = TimeSpan.FromSeconds(HttpClientTimeoutSeconds);

        // SharePoint Online configuration
        string sharePointOdataUri = Configuration["SHAREPOINT_ODATA_URI"];
        string sharePointAadTenantId = Configuration["SHAREPOINT_AAD_TENANTID"];
        string sharePointClientId = Configuration["SHAREPOINT_CLIENT_ID"];
        string sharePointClientSecret = Configuration["SHAREPOINT_CLIENT_SECRET"];

        if (string.IsNullOrEmpty(sharePointOdataUri))
        {
            throw new ArgumentException("SHAREPOINT_ODATA_URI configuration is required");
        }

        if (string.IsNullOrEmpty(sharePointAadTenantId))
        {
            throw new ArgumentException("SHAREPOINT_AAD_TENANTID configuration is required");
        }

        if (string.IsNullOrEmpty(sharePointClientId))
        {
            throw new ArgumentException("SHAREPOINT_CLIENT_ID configuration is required");
        }

        if (string.IsNullOrEmpty(sharePointClientSecret))
        {
            throw new ArgumentException("SHAREPOINT_CLIENT_SECRET configuration is required");
        }

        SiteUrl = sharePointOdataUri;

        // Configure OAuth authentication using MSAL with client credentials flow
        string authority = $"https://login.microsoftonline.com/{sharePointAadTenantId}";

        var appBuilder = ConfidentialClientApplicationBuilder
            .Create(sharePointClientId)
            .WithAuthority(new Uri(authority));

        // Client secret authentication (existing method)
        appBuilder = appBuilder.WithClientSecret(sharePointClientSecret);

        confidentialClientApp = appBuilder.Build();

        // Standard headers for Graph API access
        _Client.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    /// <summary>
    /// Ensures a valid access token is available for Microsoft Graph API.
    /// Uses MSAL's built-in token caching with retry logic for transient failures.
    /// </summary>
    /// <returns></returns>
    private async Task EnsureValidAccessTokenAsync()
    {
        int retryDelayMs = InitialRetryDelayMs;

        Exception lastException = null;

        for (int attempt = 0; attempt < MaxAuthRetries; attempt++)
        {
            try
            {
                _logger.LogDebug(
                    "EnsureValidAccessTokenAsync - Acquiring token (attempt {Attempt}/{MaxAttempts})",
                    attempt + 1,
                    MaxAuthRetries
                );

                authenticationResult = await confidentialClientApp
                    .AcquireTokenForClient(new[] { GraphScope })
                    .ExecuteAsync();

                string newAuthHeader = $"Bearer {authenticationResult.AccessToken}";

                _Client.DefaultRequestHeaders.Remove("Authorization");
                _Client.DefaultRequestHeaders.Add("Authorization", newAuthHeader);

                _logger.LogDebug("EnsureValidAccessTokenAsync - Successfully acquired token");
                return;
            }
            catch (MsalServiceException ex) when (attempt < MaxAuthRetries - 1)
            {
                _logger.LogWarning(
                    ex,
                    "EnsureValidAccessTokenAsync - Failed to acquire token on attempt {Attempt}/{MaxAttempts}, retrying in {DelayMs}ms",
                    attempt + 1,
                    MaxAuthRetries,
                    retryDelayMs
                );
                lastException = ex;
                await Task.Delay(retryDelayMs);
                retryDelayMs *= 2;
            }
            catch (MsalServiceException ex)
            {
                _logger.LogError(
                    ex,
                    "EnsureValidAccessTokenAsync - Failed to acquire token after {MaxAttempts} attempts",
                    MaxAuthRetries
                );
                throw new Exception(
                    $"Failed to acquire token after {MaxAuthRetries} attempts: {ex.Message}",
                    ex
                );
            }
            catch (MsalClientException ex)
            {
                _logger.LogError(
                    ex,
                    "EnsureValidAccessTokenAsync - Client configuration error when acquiring SharePointGraph API token"
                );
                throw new Exception(
                    $"Client configuration error when acquiring Graph API token: {ex.Message}",
                    ex
                );
            }
        }

        if (lastException != null)
        {
            throw new Exception(
                $"Failed to acquire Graph API access token after {MaxAuthRetries} attempts",
                lastException
            );
        }
    }

    /// <summary>
    /// Ensures the SharePoint Site ID is resolved from the Site URL.
    /// Site ID is cached after first resolution.
    /// </summary>
    /// <returns></returns>
    private async Task EnsureSiteIdResolvedAsync()
    {
        if (!string.IsNullOrEmpty(SiteId))
        {
            // SiteId already resolved
            _logger.LogDebug(
                "EnsureSiteIdResolvedAsync - Site ID already resolved: {SiteId}",
                SiteId
            );
            return;
        }

        await EnsureValidAccessTokenAsync();

        // Parse site URL to get hostname and site path
        Uri siteUri = new Uri(SiteUrl);
        string hostname = siteUri.Host;
        string sitePath = siteUri.AbsolutePath;

        // Graph API endpoint to get site by URL
        string requestUrl = $"{GraphApiEndpoint}sites/{hostname}:{sitePath}";
        _logger.LogDebug(
            "EnsureSiteIdResolvedAsync - Resolving site ID for {SiteUrl}, request URL: {RequestUrl}",
            SiteUrl,
            requestUrl
        );

        var response = await _Client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "EnsureSiteIdResolvedAsync - Failed to resolve site ID: {StatusCode} - {Error}",
                response.StatusCode,
                errorContent
            );
        }

        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();
        var siteInfo = JsonConvert.DeserializeObject<JObject>(responseContent);

        SiteId = siteInfo["id"]?.ToString();

        if (string.IsNullOrEmpty(SiteId))
        {
            _logger.LogError(
                "EnsureSiteIdResolvedAsync - Site ID resolution returned empty ID for {SiteUrl}",
                SiteUrl
            );
            throw new Exception($"Failed to resolve site ID for {SiteUrl}");
        }

        _logger.LogDebug(
            "EnsureSiteIdResolvedAsync - Successfully resolved site ID: {SiteId} for {SiteUrl}",
            SiteId,
            SiteUrl
        );
    }

    /// <summary>
    /// Get list (document library) by title.
    /// </summary>
    private async Task<string> GetListIdByTitleAsync(string listTitle)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        _logger.LogDebug("GetListIdByTitleAsync - Looking up list ID for: {ListTitle}", listTitle);

        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists?$filter=displayName eq '{EscapeApostrophe(listTitle)}'";

        var response = await _Client.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();
        var lists = JsonConvert.DeserializeObject<JObject>(responseContent);
        var list = lists["value"]?.FirstOrDefault();

        string listId = list?["id"]?.ToString();

        if (string.IsNullOrEmpty(listId))
        {
            _logger.LogWarning("GetListIdByTitleAsync - List not found: {ListTitle}", listTitle);
        }
        else
        {
            _logger.LogDebug(
                "GetListIdByTitleAsync - Found list ID {ListId} for {ListTitle}",
                listId,
                listTitle
            );
        }

        return listId;
    }

    /// <summary>
    /// Create a document library.
    /// </summary>
    /// <remarks>
    /// The documentTemplateUrlTitle parameter is maintained for API compatibility with the previous on-prem version,
    /// but Graph API uses a standard "documentLibrary" template and doesn't support custom templates.
    /// </remarks>
    /// <param name="listTitle"></param>
    /// <param name="documentTemplateUrlTitle"></param>
    /// <returns></returns>
    public async Task<object> CreateDocumentLibrary(
        string listTitle,
        string documentTemplateUrlTitle = null
    )
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        if (string.IsNullOrEmpty(documentTemplateUrlTitle))
        {
            documentTemplateUrlTitle = listTitle;
        }

        var requestBody = new
        {
            displayName = documentTemplateUrlTitle,
            list = new { template = "documentLibrary" }
        };

        string requestUrl = $"{GraphApiEndpoint}sites/{SiteId}/lists";
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json"
        );
        var response = await _Client.PostAsync(requestUrl, content);
        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject(responseContent);
    }

    /// <summary>
    /// Checks if a document library exists.
    /// </summary>
    /// <param name="listTitle"></param>
    /// <returns>`true` if the document library exists; otherwise, `false`.</returns>
    public async Task<bool> DocumentLibraryExists(string listTitle)
    {
        var listId = await GetListIdByTitleAsync(listTitle);

        if (string.IsNullOrEmpty(listId))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Create a folder in a document library.
    /// </summary>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public async Task CreateFolder(string listTitle, string folderName)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            throw new Exception($"Document library '{listTitle}' not found");
        }

        // Use Drive API to create folder (consistent with file upload/delete operations)
        string encodedFolderName = Uri.EscapeDataString(folderName);
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}";

        var requestBody = new { folder = new { }, name = folderName };
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        // PATCH is used to create/update in Drive API
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUrl)
        {
            Content = content
        };

        var response = await _Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Check if a folder exists.
    /// </summary>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <returns>`true` if the folder exists; otherwise, `false`.</returns>
    public async Task<bool> FolderExists(string listTitle, string folderName)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetListIdByTitleAsync(listTitle);

        if (string.IsNullOrEmpty(listId))
        {
            return false;
        }

        string encodedFolderName = Uri.EscapeDataString(folderName);
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}";

        var response = await _Client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Upload a file.
    /// For files under 4MB, uses simple upload. For larger files, uses resumable upload session.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <param name="data"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public async Task<string> UploadFile(
        string fileName,
        string listTitle,
        string folderName,
        byte[] data,
        string contentType
    )
    {
        _logger.LogDebug(
            "UploadFile - Uploading file: {FileName} to {ListTitle}/{FolderName}, size: {Size} bytes",
            fileName,
            listTitle,
            folderName,
            data.Length
        );

        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            _logger.LogError("UploadFile - Document library not found: {ListTitle}", listTitle);
            throw new Exception($"Document library '{listTitle}' not found");
        }

        // Fix folder and file names to match SharePoint requirements
        folderName = FixFoldername(folderName);
        fileName = GetTruncatedFileName(fileName, listTitle, folderName);

        // Ensure folder exists
        bool folderExists = await FolderExists(listTitle, folderName);
        if (!folderExists)
        {
            _logger.LogDebug("UploadFile - Creating folder: {FolderName}", folderName);
            await CreateFolder(listTitle, folderName);
        }

        if (data.Length < UploadSessionThreshold)
        {
            // Simple upload for small files
            _logger.LogDebug("UploadFile - Using simple upload for file: {FileName}", fileName);
            return await SimpleUpload(listId, folderName, fileName, data, contentType);
        }
        else
        {
            // Resumable upload session for large files
            _logger.LogDebug(
                "UploadFile - Using chunked upload for large file: {FileName}",
                fileName
            );
            return await LargeFileUpload(listId, folderName, fileName, data, contentType);
        }
    }

    /// <summary>
    /// Simple upload for files less than 4MB.
    /// </summary>
    /// <param name="listId"></param>
    /// <param name="folderName"></param>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    private async Task<string> SimpleUpload(
        string listId,
        string folderName,
        string fileName,
        byte[] data,
        string contentType
    )
    {
        string encodedFolderName = Uri.EscapeDataString(folderName);
        string encodedFileName = Uri.EscapeDataString(fileName);
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}/{encodedFileName}:/content";

        _logger.LogDebug("SimpleUpload - Simple upload PUT request: {RequestUrl}", requestUrl);

        var content = new ByteArrayContent(data);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        var response = await _Client.PutAsync(requestUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "SimpleUpload - Simple upload failed: {StatusCode} - {Error}",
                response.StatusCode,
                errorContent
            );
            throw new Exception(
                $"Failed to upload file '{fileName}': {response.StatusCode} - {errorContent}"
            );
        }

        _logger.LogDebug("SimpleUpload - Successfully uploaded file: {FileName}", fileName);
        return fileName;
    }

    /// <summary>
    /// Large file upload using resumable upload session for files >= 4MB.
    /// </summary>
    /// <param name="listId"></param>
    /// <param name="folderName"></param>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    private async Task<string> LargeFileUpload(
        string listId,
        string folderName,
        string fileName,
        byte[] data,
        string contentType
    )
    {
        string encodedFolderName = Uri.EscapeDataString(folderName);
        string encodedFileName = Uri.EscapeDataString(fileName);

        // Step 1: Create upload session
        string createSessionUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}/{encodedFileName}:/createUploadSession";

        var sessionRequestBody = new { item = new { name = fileName, fileSystemInfo = new { } } };

        var sessionContent = new StringContent(
            JsonConvert.SerializeObject(sessionRequestBody),
            Encoding.UTF8,
            "application/json"
        );

        var sessionResponse = await _Client.PostAsync(createSessionUrl, sessionContent);
        sessionResponse.EnsureSuccessStatusCode();

        string sessionResponseContent = await sessionResponse.Content.ReadAsStringAsync();
        var sessionInfo = JsonConvert.DeserializeObject<JObject>(sessionResponseContent);
        string uploadUrl = sessionInfo["uploadUrl"]?.ToString();

        if (string.IsNullOrEmpty(uploadUrl))
        {
            throw new Exception("Failed to create upload session - no uploadUrl returned");
        }

        // Step 2: Upload file in chunks
        int totalBytes = data.Length;
        int uploadedBytes = 0;

        _logger.LogDebug(
            "LargeFileUpload - Starting chunked upload: {TotalBytes} bytes in {ChunkSize} byte chunks",
            totalBytes,
            ChunkSize
        );

        while (uploadedBytes < totalBytes)
        {
            int bytesToUpload = Math.Min(ChunkSize, totalBytes - uploadedBytes);
            byte[] chunk = new byte[bytesToUpload];
            Array.Copy(data, uploadedBytes, chunk, 0, bytesToUpload);

            var chunkContent = new ByteArrayContent(chunk);
            chunkContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            chunkContent.Headers.ContentLength = bytesToUpload;
            chunkContent.Headers.ContentRange = new ContentRangeHeaderValue(
                uploadedBytes,
                uploadedBytes + bytesToUpload - 1,
                totalBytes
            );

            _logger.LogDebug(
                "LargeFileUpload - Uploading chunk: {UploadedBytes}/{TotalBytes} bytes ({Percentage:F1}%)",
                uploadedBytes + bytesToUpload,
                totalBytes,
                (uploadedBytes + bytesToUpload) * 100.0 / totalBytes
            );

            var chunkResponse = await _Client.PutAsync(uploadUrl, chunkContent);

            if (
                !chunkResponse.IsSuccessStatusCode
                && chunkResponse.StatusCode != HttpStatusCode.Accepted
            )
            {
                string errorContent = await chunkResponse.Content.ReadAsStringAsync();
                _logger.LogError(
                    "LargeFileUpload - Chunk upload failed at position {Position}: {StatusCode} - {Error}",
                    uploadedBytes,
                    chunkResponse.StatusCode,
                    errorContent
                );
                throw new Exception(
                    $"Failed to upload chunk at position {uploadedBytes}: {chunkResponse.StatusCode} - {errorContent}"
                );
            }

            uploadedBytes += bytesToUpload;
        }

        _logger.LogDebug(
            "LargeFileUpload - Successfully completed chunked upload: {FileName}",
            fileName
        );
        return fileName;
    }

    /// <summary>
    /// Download a file from SharePoint.
    /// </summary>
    /// <example>
    /// serverRelativeUrl: "https://bcgov.sharepoint.com/sites/lcrb-cllceDEV/account/TEST-FOLDER/NOTICE__MY-Notice.pdf"
    /// </example>
    /// <param name="serverRelativeUrl"></param>
    /// <returns></returns>
    public async Task<byte[]> DownloadFile(string serverRelativeUrl)
    {
        _logger.LogDebug("DownloadFile - Downloading file: {ServerRelativeUrl}", serverRelativeUrl);

        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        // Parse to extract file path relative to site
        string filePath = ParseServerRelativeUrl(serverRelativeUrl);

        // Extract library name (first segment) and remaining path
        int firstSlash = filePath.IndexOf('/');
        if (firstSlash <= 0)
        {
            _logger.LogError(
                "DownloadFile - Invalid path format: {ServerRelativeUrl}",
                serverRelativeUrl
            );
            throw new ArgumentException(
                $"Invalid path format: {serverRelativeUrl}. Expected format: /library/folder/file.pdf"
            );
        }

        string listTitle = filePath.Substring(0, firstSlash);
        string pathInLibrary = filePath.Substring(firstSlash + 1);

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            _logger.LogError("DownloadFile - Document library not found: {ListTitle}", listTitle);
            throw new Exception($"Document library '{listTitle}' not found");
        }

        string encodedPath = string.Join(
            "/",
            pathInLibrary.Split('/').Select(Uri.EscapeDataString)
        );
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedPath}:/content";

        _logger.LogDebug("DownloadFile - Download GET request: {RequestUrl}", requestUrl);

        var response = await _Client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "DownloadFile - Download failed: {StatusCode} - {Error}",
                response.StatusCode,
                errorContent
            );
            throw new Exception(
                $"Failed to download file '{serverRelativeUrl}': {response.StatusCode} - {errorContent}"
            );
        }

        byte[] fileData = await response.Content.ReadAsByteArrayAsync();
        _logger.LogDebug(
            "DownloadFile - Successfully downloaded file: {ServerRelativeUrl}, size: {Size} bytes",
            serverRelativeUrl,
            fileData.Length
        );
        return fileData;
    }

    /// <summary>
    /// Parse server relative URL to extract file path relative to the site.
    /// Handles URLs like "/sites/sitename/documentlibrary/folder/file.pdf"
    /// </summary>
    /// <param name="serverRelativeUrl"></param>
    /// <returns></returns>
    private string ParseServerRelativeUrl(string serverRelativeUrl)
    {
        if (string.IsNullOrEmpty(serverRelativeUrl))
        {
            throw new ArgumentException("Server relative URL cannot be null or empty");
        }

        string path = serverRelativeUrl.TrimStart('/');

        // If SiteUrl contains a site path, remove it from the server relative URL
        Uri siteUri = new Uri(SiteUrl);
        string sitePath = siteUri.AbsolutePath.TrimStart('/').TrimEnd('/');

        if (!string.IsNullOrEmpty(sitePath) && path.StartsWith(sitePath + "/"))
        {
            path = path.Substring(sitePath.Length + 1);
        }

        return path;
    }

    /// <summary>
    /// Delete a file from SharePoint.
    /// NOTE: This method expects the full path including library name as first segment after site path
    /// </summary>
    /// <param name="serverRelativeUrl"></param>
    /// <returns></returns>
    public async Task<bool> DeleteFile(string serverRelativeUrl)
    {
        _logger.LogDebug("DeleteFile - Deleting file: {ServerRelativeUrl}", serverRelativeUrl);

        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        // Parse to extract file path relative to site
        string filePath = ParseServerRelativeUrl(serverRelativeUrl);

        // Extract library name (first segment) and remaining path
        int firstSlash = filePath.IndexOf('/');
        if (firstSlash <= 0)
        {
            _logger.LogError(
                "DeleteFile - Invalid path format: {ServerRelativeUrl}",
                serverRelativeUrl
            );
            throw new ArgumentException(
                $"Invalid path format: {serverRelativeUrl}. Expected format: /library/folder/file.pdf"
            );
        }

        string listTitle = filePath.Substring(0, firstSlash);
        string pathInLibrary = filePath.Substring(firstSlash + 1);

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            _logger.LogError("DeleteFile - Document library not found: {ListTitle}", listTitle);
            throw new Exception($"Document library '{listTitle}' not found");
        }

        string encodedPath = string.Join(
            "/",
            pathInLibrary.Split('/').Select(Uri.EscapeDataString)
        );
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedPath}";

        _logger.LogDebug("DeleteFile - Delete request: {RequestUrl}", requestUrl);

        var response = await _Client.DeleteAsync(requestUrl);

        if (response.StatusCode == HttpStatusCode.NoContent || response.IsSuccessStatusCode)
        {
            _logger.LogDebug(
                "DeleteFile - Successfully deleted file: {ServerRelativeUrl}",
                serverRelativeUrl
            );
            return true;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug(
                "DeleteFile - File not found (already deleted): {ServerRelativeUrl}",
                serverRelativeUrl
            );
            return true;
        }

        string errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogError(
            "DeleteFile - Delete failed: {StatusCode} - {Error}",
            response.StatusCode,
            errorContent
        );
        throw new Exception(
            $"Failed to delete file '{serverRelativeUrl}': {response.StatusCode} - {errorContent}"
        );
    }

    /// <summary>
    /// Delete a file.
    /// </summary>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<bool> DeleteFile(string listTitle, string folderName, string fileName)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            throw new Exception($"Document library '{listTitle}' not found");
        }

        folderName = FixFoldername(folderName);
        string encodedFolderName = Uri.EscapeDataString(folderName);
        string encodedFileName = Uri.EscapeDataString(fileName);

        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}/{encodedFileName}";

        var response = await _Client.DeleteAsync(requestUrl);

        if (response.StatusCode == HttpStatusCode.NoContent || response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return true;
        }

        string errorContent = await response.Content.ReadAsStringAsync();
        throw new Exception(
            $"Failed to delete file '{fileName}': {response.StatusCode} - {errorContent}"
        );
    }

    /// <summary>
    /// Rename or move a file.
    /// NOTE: Both URLs must reference the same document library.
    /// </summary>
    /// <param name="oldServerRelativeUrl"></param>
    /// <param name="newServerRelativeUrl"></param>
    /// <returns></returns>
    public async Task<bool> RenameFile(string oldServerRelativeUrl, string newServerRelativeUrl)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        // Parse old path
        string oldFilePath = ParseServerRelativeUrl(oldServerRelativeUrl);
        int firstSlash = oldFilePath.IndexOf('/');
        if (firstSlash <= 0)
        {
            throw new ArgumentException($"Invalid path format: {oldServerRelativeUrl}");
        }

        string listTitle = oldFilePath.Substring(0, firstSlash);
        string oldPathInLibrary = oldFilePath.Substring(firstSlash + 1);

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            throw new Exception($"Document library '{listTitle}' not found");
        }

        string encodedOldPath = string.Join(
            "/",
            oldPathInLibrary.Split('/').Select(Uri.EscapeDataString)
        );
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedOldPath}";

        // Parse new path (must be in same library)
        string newFilePath = ParseServerRelativeUrl(newServerRelativeUrl);
        if (!newFilePath.StartsWith(listTitle + "/"))
        {
            throw new ArgumentException(
                $"Cannot move file across libraries. Old: {listTitle}, New path: {newFilePath}"
            );
        }

        string newPathInLibrary = newFilePath.Substring(listTitle.Length + 1);
        int lastSlash = newPathInLibrary.LastIndexOf('/');
        string newParentPath = lastSlash > 0 ? newPathInLibrary.Substring(0, lastSlash) : "";
        string newFileName =
            lastSlash >= 0 ? newPathInLibrary.Substring(lastSlash + 1) : newPathInLibrary;

        var requestBody = new
        {
            name = newFileName,
            parentReference = new
            {
                path = string.IsNullOrEmpty(newParentPath)
                    ? $"/drive/root"
                    : $"/drive/root/{newParentPath}"
            }
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUrl)
        {
            Content = content
        };

        var response = await _Client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception(
                $"Failed to rename file from '{oldServerRelativeUrl}' to '{newServerRelativeUrl}': {response.StatusCode} - {errorContent}"
            );
        }

        return true;
    }

    /// <summary>
    /// Get file details list in a folder with pagination support.
    /// /// </summary>
    public async Task<List<SharePointFileDetailsList>> GetFileDetailsListInFolder(
        string listTitle, // EntityName
        string folderName, // FolderName
        string documentType // DocumentYype
    )
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            return new List<SharePointFileDetailsList>();
        }

        string encodedFolderName = Uri.EscapeDataString(folderName);
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}:/children";

        var fileDetailsList = new List<SharePointFileDetailsList>();
        int pageCount = 0;

        _logger.LogDebug(
            "GetFileDetailsListInFolder - Getting file list from {ListTitle}/{FolderName}, filtering by documentType: {DocumentType}",
            listTitle,
            folderName,
            documentType ?? "(none)"
        );

        // Handle pagination
        while (!string.IsNullOrEmpty(requestUrl))
        {
            pageCount++;
            _logger.LogDebug(
                "GetFileDetailsListInFolder - Fetching page {PageCount} from: {RequestUrl}",
                pageCount,
                requestUrl
            );

            // Get the current page of items
            var response = await _Client.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogDebug(
                        "GetFileDetailsListInFolder - Folder not found: {ListTitle}/{FolderName}, returning empty list",
                        listTitle,
                        folderName
                    );
                    // Folder doesn't exist, return empty list
                    return new List<SharePointFileDetailsList>();
                }

                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "GetFileDetailsListInFolder - Failed to get folder contents: {StatusCode} - {Error}",
                    response.StatusCode,
                    errorContent
                );
                throw new Exception(
                    $"Failed to get folder contents: {response.StatusCode} - {errorContent}"
                );
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<JObject>(responseContent);
            var files = items["value"]?.Where(i => i["file"] != null).ToList();

            if (files == null)
            {
                // No files found, check for next page and continue
                requestUrl = items["@odata.nextLink"]?.ToString();
                continue;
            }

            foreach (var file in files)
            {
                string fileName = file["name"]?.ToString();

                // Parse document type from filename (assuming format: documentType__filename.ext)
                string fileDocType = null;
                int fileDoctypeEnd = fileName?.IndexOf("__") ?? -1;
                if (fileDoctypeEnd > -1)
                {
                    fileDocType = fileName.Substring(0, fileDoctypeEnd);
                }

                // Extract server-relative URL from webUrl (convert full URL to path-only)
                string webUrl = file["webUrl"]?.ToString();
                string serverRelativeUrl = webUrl;
                if (
                    !string.IsNullOrEmpty(webUrl)
                    && Uri.TryCreate(webUrl, UriKind.Absolute, out Uri uri)
                )
                {
                    serverRelativeUrl = uri.AbsolutePath;
                }

                var fileDetails = new SharePointFileDetailsList
                {
                    Name = fileName,
                    ServerRelativeUrl = serverRelativeUrl,
                    Length = file["size"]?.ToString(),
                    TimeCreated = file["createdDateTime"]?.ToString(),
                    TimeLastModified = file["lastModifiedDateTime"]?.ToString(),
                    DocumentType = fileDocType
                };

                if (fileDoctypeEnd > -1 && fileDocType == documentType)
                {
                    // Only set DocumentType if it matches the filter
                    fileDetails.DocumentType = documentType;
                }

                fileDetailsList.Add(fileDetails);
            }

            // Finished processing files, check for next page and continue
            requestUrl = items["@odata.nextLink"]?.ToString();
        }

        _logger.LogDebug(
            "GetFileDetailsListInFolder - Fetched {PageCount} page(s), found {FileCount} files before filtering",
            pageCount,
            fileDetailsList.Count
        );

        // Filter by document type at the end
        if (documentType != null)
        {
            int beforeFilter = fileDetailsList.Count;
            fileDetailsList = fileDetailsList.Where(f => f.DocumentType == documentType).ToList();
            _logger.LogDebug(
                "GetFileDetailsListInFolder - Filtered by documentType '{DocumentType}': {BeforeCount} -> {AfterCount} files",
                documentType,
                beforeFilter,
                fileDetailsList.Count
            );
        }

        _logger.LogDebug(
            "GetFileDetailsListInFolder - Returning {FileCount} files from {ListTitle}/{FolderName}",
            fileDetailsList.Count,
            listTitle,
            folderName
        );
        return fileDetailsList;
    }

    /// <summary>
    /// Delete a folder.
    /// </summary>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public async Task<bool> DeleteFolder(string listTitle, string folderName)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            return false;
        }

        string encodedFolderName = Uri.EscapeDataString(folderName);
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}";

        var response = await _Client.DeleteAsync(requestUrl);

        return response.StatusCode == HttpStatusCode.NoContent || response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Helper method to add file with folder creation (using Stream).
    /// </summary>
    /// <param name="folderName"></param>
    /// <param name="fileName"></param>
    /// <param name="fileData"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public async Task<string> AddFile(
        string folderName,
        string fileName,
        Stream fileData,
        string contentType
    )
    {
        return await AddFile(
            SharePointConstants.DefaultDocumentListTitle,
            folderName,
            fileName,
            fileData,
            contentType
        );
    }

    /// <summary>
    /// Helper method to add file with folder creation (using Stream).
    /// </summary>
    /// <param name="documentLibrary"></param>
    /// <param name="folderName"></param>
    /// <param name="fileName"></param>
    /// <param name="fileData"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public async Task<string> AddFile(
        string documentLibrary,
        string folderName,
        string fileName,
        Stream fileData,
        string contentType
    )
    {
        folderName = FixFoldername(folderName);
        bool folderExists = await FolderExists(documentLibrary, folderName);
        if (!folderExists)
        {
            await CreateFolder(documentLibrary, folderName);
        }

        fileName = await UploadFile(fileName, documentLibrary, folderName, fileData, contentType);
        return fileName;
    }

    /// <summary>
    /// Helper method to add file with folder creation (using byte array).
    /// </summary>
    /// <param name="folderName"></param>
    /// <param name="fileName"></param>
    /// <param name="fileData"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public async Task<string> AddFile(
        string folderName,
        string fileName,
        byte[] fileData,
        string contentType
    )
    {
        return await AddFile(
            SharePointConstants.DefaultDocumentListTitle,
            folderName,
            fileName,
            fileData,
            contentType
        );
    }

    /// <summary>
    /// Helper method to add file with folder creation (using byte array).
    /// </summary>
    /// <param name="documentLibrary"></param>
    /// <param name="folderName"></param>
    /// <param name="fileName"></param>
    /// <param name="fileData"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public async Task<string> AddFile(
        string documentLibrary,
        string folderName,
        string fileName,
        byte[] fileData,
        string contentType
    )
    {
        folderName = FixFoldername(folderName);
        bool folderExists = await FolderExists(documentLibrary, folderName);
        if (!folderExists)
        {
            await CreateFolder(documentLibrary, folderName);
        }

        fileName = await UploadFile(fileName, documentLibrary, folderName, fileData, contentType);
        return fileName;
    }

    /// <summary>
    /// Upload a file using Stream.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <param name="fileData"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public async Task<string> UploadFile(
        string fileName,
        string listTitle,
        string folderName,
        Stream fileData,
        string contentType
    )
    {
        using (var ms = new MemoryStream())
        {
            await fileData.CopyToAsync(ms);
            byte[] data = ms.ToArray();
            return await UploadFile(fileName, listTitle, folderName, data, contentType);
        }
    }

    /// <summary>
    /// Removes invalid characters from the filename string, for SharePoint compatibility.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public string RemoveInvalidCharacters(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            return filename;
        }

        var osInvalidChars = new string(Path.GetInvalidFileNameChars());
        osInvalidChars += "~#%&*()[]{}"; // Additional characters that don't work with SharePoint

        string invalidChars = System.Text.RegularExpressions.Regex.Escape(osInvalidChars);
        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        string result = System.Text.RegularExpressions.Regex.Replace(filename, invalidRegStr, "_");

        return result;
    }

    /// <summary>
    /// Fixes folder name by removing invalid characters, for SharePoint compatibility.
    /// </summary>
    /// <param name="foldername"></param>
    /// <returns></returns>
    public string FixFoldername(string foldername)
    {
        return RemoveInvalidCharacters(foldername);
    }

    /// <summary>
    /// Removes invalid characters and truncates filename to max length, for SharePoint compatibility.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public string FixFilename(string filename, int maxLength = 128)
    {
        string result = RemoveInvalidCharacters(filename);

        if (result.Length >= maxLength)
        {
            string extension = Path.GetExtension(result);
            result = Path.GetFileNameWithoutExtension(result)
                .Substring(0, maxLength - extension.Length);
            result += extension;
        }

        return result;
    }

    /// <summary>
    /// Truncates filename if needed to comply with SharePoint limits.
    /// </summary>
    /// <remarks>
    /// SharePoint has limits on filename length and total URL length.
    /// This method returns the input filename or a truncated version if needed.
    /// </remarks>
    /// <param name="fileName"></param>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public string GetTruncatedFileName(string fileName, string listTitle, string folderName)
    {
        int maxLength = 128;
        fileName = FixFilename(fileName, maxLength);
        folderName = FixFoldername(folderName);

        string serverRelativeUrl = GetServerRelativeURL(listTitle, folderName);
        string fullPath = $"{serverRelativeUrl}/{fileName}";

        if (fullPath.Length > SharePointConstants.MaxUrlLength)
        {
            int delta = fullPath.Length - SharePointConstants.MaxUrlLength;
            maxLength -= delta;
            fileName = FixFilename(fileName, maxLength);
        }

        return fileName;
    }

    /// <summary>
    /// Escape single quotes in a string for OData queries.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string EscapeApostrophe(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Replace("'", "''");
    }

    /// <summary>
    /// Get server relative URL for a folder.
    /// </summary>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public string GetServerRelativeURL(string listTitle, string folderName)
    {
        folderName = FixFoldername(folderName);

        Uri siteUri = new Uri(SiteUrl);
        string sitePath = siteUri.AbsolutePath.TrimStart('/').TrimEnd('/');

        string serverRelativeUrl;
        if (!string.IsNullOrEmpty(sitePath))
        {
            serverRelativeUrl = $"/{sitePath}/";
        }
        else
        {
            serverRelativeUrl = "/";
        }

        serverRelativeUrl +=
            Uri.EscapeDataString(listTitle) + "/" + Uri.EscapeDataString(folderName);

        return serverRelativeUrl;
    }

    /// <summary>
    /// Dispose of resources.
    /// </summary>
    public void Dispose()
    {
        _Client?.Dispose();
    }
}
