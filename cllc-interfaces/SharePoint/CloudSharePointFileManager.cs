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
/// Methods for calling Cloud SharePoint using Microsoft Graph API.
/// </summary>
public partial class CloudSharePointFileManager : ISharePointFileManager
{
    private const int MAX_TOTAL_LENGTH = 260; // default maximum URL length.
    private const int MAX_SEGMENT_LENGTH = 128; // default maximum segment length.
    private const int UploadSessionThreshold = 4 * 1024 * 1024; // 4MB
    private const int ChunkSize = 320 * 1024 * 10; // 3.2MB
    private const int HttpClientTimeoutSeconds = 100;
    private const int MaxAuthRetries = 3;
    private const int InitialRetryDelayMs = 1000;
    private const string DocumentLibraryTemplate = "documentLibrary"; // Standard template for document libraries

    private AuthenticationResult authenticationResult;
    private IConfidentialClientApplication confidentialClientApp;
    private const string GraphScope = "https://graph.microsoft.com/.default";
    private HttpClient _Client;

    private readonly ILogger<CloudSharePointFileManager> _logger;

    public string SiteUrl { get; set; }
    public string SiteId { get; set; }
    public string GraphApiEndpoint { get; set; } = "https://graph.microsoft.com/v1.0/";

    /// <summary>
    /// WebName property for interface compatibility. Always returns null for cloud implementation.
    /// Cloud SharePoint uses site URLs directly without the /sites/ prefix convention.
    /// </summary>
    public string WebName { get; } = null;

    public CloudSharePointFileManager(IConfiguration Configuration, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CloudSharePointFileManager>();
        _logger.LogDebug("[CloudSharePointFileManager] Constructor - called");

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
        _logger.LogDebug("[CloudSharePointFileManager] EnsureValidAccessTokenAsync - called");
        int retryDelayMs = InitialRetryDelayMs;

        Exception lastException = null;

        for (int attempt = 0; attempt < MaxAuthRetries; attempt++)
        {
            try
            {
                _logger.LogDebug(
                    "[CloudSharePointFileManager] EnsureValidAccessTokenAsync - Acquiring token (attempt {Attempt}/{MaxAttempts})",
                    attempt + 1,
                    MaxAuthRetries
                );

                authenticationResult = await confidentialClientApp
                    .AcquireTokenForClient(new[] { GraphScope })
                    .ExecuteAsync();

                string newAuthHeader = $"Bearer {authenticationResult.AccessToken}";

                _logger.LogDebug(
                    "[CloudSharePointFileManager] EnsureValidAccessTokenAsync - Access Token {AccessToken}",
                    authenticationResult.AccessToken
                );

                _Client.DefaultRequestHeaders.Remove("Authorization");
                _Client.DefaultRequestHeaders.Add("Authorization", newAuthHeader);

                _logger.LogDebug(
                    "[CloudSharePointFileManager] EnsureValidAccessTokenAsync - Successfully acquired token"
                );
                return;
            }
            catch (MsalServiceException ex) when (attempt < MaxAuthRetries - 1)
            {
                _logger.LogWarning(
                    ex,
                    "[CloudSharePointFileManager] EnsureValidAccessTokenAsync - Failed to acquire token on attempt {Attempt}/{MaxAttempts}, retrying in {DelayMs}ms",
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
                    "[CloudSharePointFileManager] EnsureValidAccessTokenAsync - Failed to acquire token after {MaxAttempts} attempts",
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
                    "[CloudSharePointFileManager] EnsureValidAccessTokenAsync - Client configuration error when acquiring SharePointGraph API token"
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
        _logger.LogDebug(
            "[CloudSharePointFileManager] EnsureSiteIdResolvedAsync - SiteId={SiteId}",
            SiteId
        );
        if (!string.IsNullOrEmpty(SiteId))
        {
            // SiteId already resolved
            _logger.LogDebug(
                "[CloudSharePointFileManager] EnsureSiteIdResolvedAsync - Site ID already resolved: {SiteId}",
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
            "[CloudSharePointFileManager] EnsureSiteIdResolvedAsync - Resolving site ID for {SiteUrl}, request URL: {RequestUrl}",
            SiteUrl,
            requestUrl
        );

        var response = await _Client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "[CloudSharePointFileManager] EnsureSiteIdResolvedAsync - Failed to resolve site ID: {StatusCode} - {Error}",
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
                "[CloudSharePointFileManager] EnsureSiteIdResolvedAsync - Site ID resolution returned empty ID for {SiteUrl}",
                SiteUrl
            );
            throw new Exception($"Failed to resolve site ID for {SiteUrl}");
        }

        _logger.LogDebug(
            "[CloudSharePointFileManager] EnsureSiteIdResolvedAsync - Successfully resolved site ID: {SiteId} for {SiteUrl}",
            SiteId,
            SiteUrl
        );
    }

    /// <summary>
    /// Get list (document library) by `name` or `displayName`.
    /// Matches first by the list's internal `name` field, and then by `displayName` if not found.
    /// </summary>
    /// <remarks>
    /// All of the LCRB sharepoint documents typically reside underneath a top-level folder corresponding to which
    /// class of document it is (eg. Application, Contact, Account, etc). Each of these top-level folders is
    /// represented as a SharePoint document library (list) with a `displayName` field like "Application" and and
    /// internal `name` field like "adoxio_application".
    /// </remarks>
    /// <example>
    /// GetDocumentLibraryIdByNameAsync("adoxio_application"); // returns the ID of the "Application" document library.
    /// GetDocumentLibraryIdByNameAsync("Application"); // returns the ID of the "Application" document library.
    /// </example>
    private async Task<string> GetDocumentLibraryIdByNameAsync(string listTitle)
    {
        _logger.LogDebug(
            "[CloudSharePointFileManager] GetDocumentLibraryIdByNameAsync - listTitle={ListTitle}",
            listTitle
        );
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        _logger.LogDebug(
            "[CloudSharePointFileManager] GetDocumentLibraryIdByNameAsync - Looking up list ID for: {ListTitle}",
            listTitle
        );

        string requestUrl = $"{GraphApiEndpoint}sites/{SiteId}/lists?$select=id,name,displayName";

        _logger.LogDebug(
            "[CloudSharePointFileManager] GetDocumentLibraryIdByNameAsync - Request URL: {RequestUrl}",
            requestUrl
        );

        var response = await _Client.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();
        var listsResponse = JsonConvert.DeserializeObject<JObject>(responseContent);
        var allLists = listsResponse["value"]?.ToObject<List<JObject>>();

        if (allLists == null || !allLists.Any())
        {
            _logger.LogWarning(
                "[CloudSharePointFileManager] GetDocumentLibraryIdByNameAsync - No lists found in site"
            );
            return null;
        }

        // Find the list whose name matches the listTitle
        var matchingList = allLists.FirstOrDefault(list =>
        {
            string name = list["name"]?.ToString();
            return string.Equals(name, listTitle, StringComparison.OrdinalIgnoreCase);
        });

        // If not found by name, try matching by displayName
        if (matchingList == null)
        {
            matchingList = allLists.FirstOrDefault(list =>
            {
                string displayName = list["displayName"]?.ToString();
                return string.Equals(displayName, listTitle, StringComparison.OrdinalIgnoreCase);
            });
        }

        string listId = matchingList?["id"]?.ToString();

        if (string.IsNullOrEmpty(listId))
        {
            _logger.LogWarning(
                "[CloudSharePointFileManager] GetDocumentLibraryIdByNameAsync - List not found with name or displayName: {ListTitle}",
                listTitle
            );
        }
        else
        {
            string name = matchingList["name"]?.ToString();
            string displayName = matchingList["displayName"]?.ToString();
            _logger.LogDebug(
                "[CloudSharePointFileManager] GetDocumentLibraryIdByNameAsync - Found list ID {ListId} for {ListTitle} (name: {Name}, displayName: {DisplayName})",
                listId,
                listTitle,
                name,
                displayName
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
        _logger.LogInformation(
            "[CloudSharePointFileManager] CreateDocumentLibrary - listTitle={ListTitle}, documentTemplateUrlTitle={DocumentTemplateUrlTitle}",
            listTitle,
            documentTemplateUrlTitle
        );
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        if (string.IsNullOrEmpty(documentTemplateUrlTitle))
        {
            documentTemplateUrlTitle = listTitle;
        }

        var requestBody = new
        {
            displayName = documentTemplateUrlTitle,
            list = new { template = DocumentLibraryTemplate },
        };

        string requestUrl = $"{GraphApiEndpoint}sites/{SiteId}/lists";

        _logger.LogDebug(
            "[CloudSharePointFileManager] CreateDocumentLibrary - Request URL: {requestUrl}",
            requestUrl
        );

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
        _logger.LogDebug(
            "[CloudSharePointFileManager] DocumentLibraryExists - listTitle={ListTitle}",
            listTitle
        );
        var listId = await GetDocumentLibraryIdByNameAsync(listTitle);

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
        _logger.LogInformation(
            "[CloudSharePointFileManager] CreateFolder - listTitle={ListTitle}, folderName={FolderName}",
            listTitle,
            folderName
        );
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetDocumentLibraryIdByNameAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            throw new Exception($"Document library '{listTitle}' not found");
        }

        // Use Drive API to create folder (consistent with file upload/delete operations)
        string encodedFolderName = Uri.EscapeDataString(folderName);
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}";

        _logger.LogDebug(
            "[CloudSharePointFileManager] CreateFolder - Request URL: {RequestUrl}",
            requestUrl
        );

        var requestBody = new { folder = new { }, name = folderName };
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        // PATCH is used to create/update in Drive API
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUrl)
        {
            Content = content,
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
        _logger.LogDebug(
            "[CloudSharePointFileManager] FolderExists - listTitle={ListTitle}, folderName={FolderName}",
            listTitle,
            folderName
        );
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetDocumentLibraryIdByNameAsync(listTitle);

        if (string.IsNullOrEmpty(listId))
        {
            return false;
        }

        string encodedFolderName = Uri.EscapeDataString(folderName);
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}";

        _logger.LogDebug(
            "[CloudSharePointFileManager] FolderExists - Request URL: {RequestUrl}",
            requestUrl
        );

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
        _logger.LogInformation(
            "[CloudSharePointFileManager] UploadFile - fileName={FileName}, listTitle={ListTitle}, folderName={FolderName}, dataSize={DataSize}, contentType={ContentType}",
            fileName,
            listTitle,
            folderName,
            data.Length,
            contentType
        );
        _logger.LogDebug(
            "[CloudSharePointFileManager] UploadFile - Uploading file: {FileName} to {ListTitle}/{FolderName}, size: {Size} bytes",
            fileName,
            listTitle,
            folderName,
            data.Length
        );

        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetDocumentLibraryIdByNameAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            _logger.LogError(
                "[CloudSharePointFileManager] UploadFile - Document library not found: {ListTitle}",
                listTitle
            );
            throw new Exception($"Document library '{listTitle}' not found");
        }

        // Fix folder and file names to match SharePoint requirements
        folderName = SharePointUtils.FixFoldername(folderName, null, false);
        fileName = GetTruncatedFileName(fileName, listTitle, folderName);

        // Ensure folder exists
        bool folderExists = await FolderExists(listTitle, folderName);
        if (!folderExists)
        {
            _logger.LogDebug(
                "[CloudSharePointFileManager] UploadFile - Creating folder: {FolderName}",
                folderName
            );
            await CreateFolder(listTitle, folderName);
        }

        if (data.Length < UploadSessionThreshold)
        {
            // Simple upload for small files
            _logger.LogDebug(
                "[CloudSharePointFileManager] UploadFile - Using simple upload for file: {FileName}",
                fileName
            );
            return await SimpleUpload(listId, folderName, fileName, data, contentType);
        }
        else
        {
            // Resumable upload session for large files
            _logger.LogDebug(
                "[CloudSharePointFileManager] UploadFile - Using chunked upload for large file: {FileName}",
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
        _logger.LogDebug(
            "[CloudSharePointFileManager] SimpleUpload - listId={ListId}, folderName={FolderName}, fileName={FileName}, dataSize={DataSize}",
            listId,
            folderName,
            fileName,
            data.Length
        );
        string encodedFolderName = Uri.EscapeDataString(folderName);
        string encodedFileName = Uri.EscapeDataString(fileName);

        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}/{encodedFileName}:/content";

        _logger.LogDebug(
            "[CloudSharePointFileManager] SimpleUpload - Request URL: {RequestUrl}",
            requestUrl
        );

        var content = new ByteArrayContent(data);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        var response = await _Client.PutAsync(requestUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "[CloudSharePointFileManager] SimpleUpload - Simple upload failed: {StatusCode} - {Error}",
                response.StatusCode,
                errorContent
            );
            throw new Exception(
                $"Failed to upload file '{fileName}': {response.StatusCode} - {errorContent}"
            );
        }

        _logger.LogDebug(
            "[CloudSharePointFileManager] SimpleUpload - Successfully uploaded file: {FileName}",
            fileName
        );
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
        _logger.LogDebug(
            "[CloudSharePointFileManager] LargeFileUpload - listId={ListId}, folderName={FolderName}, fileName={FileName}, dataSize={DataSize}",
            listId,
            folderName,
            fileName,
            data.Length
        );
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
            "[CloudSharePointFileManager] LargeFileUpload - Starting chunked upload: {TotalBytes} bytes in {ChunkSize} byte chunks",
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
                "[CloudSharePointFileManager] LargeFileUpload - Uploading chunk: {UploadedBytes}/{TotalBytes} bytes ({Percentage:F1}%)",
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
                    "[CloudSharePointFileManager] LargeFileUpload - Chunk upload failed at position {Position}: {StatusCode} - {Error}",
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
            "[CloudSharePointFileManager] LargeFileUpload - Successfully completed chunked upload: {FileName}",
            fileName
        );
        return fileName;
    }

    /// <summary>
    /// Download a file from SharePoint.
    /// </summary>
    /// <example>
    /// serverRelativeUrl: "/sites/lcrb-cllceDEV/account/TEST-FOLDER/NOTICE__MY-Notice.pdf"
    /// </example>
    /// <param name="serverRelativeUrl"></param>
    /// <returns></returns>
    public async Task<byte[]> DownloadFile(string serverRelativeUrl)
    {
        _logger.LogInformation(
            "[CloudSharePointFileManager] DownloadFile - serverRelativeUrl={ServerRelativeUrl}",
            serverRelativeUrl
        );
        _logger.LogDebug(
            "[CloudSharePointFileManager] DownloadFile - Downloading file: {ServerRelativeUrl}",
            serverRelativeUrl
        );

        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        // Parse to extract file path relative to site
        string filePath = ParseServerRelativeUrl(serverRelativeUrl);

        // Extract library name (first segment) and remaining path
        int firstSlash = filePath.IndexOf('/');
        if (firstSlash <= 0)
        {
            _logger.LogError(
                "[CloudSharePointFileManager] DownloadFile - Invalid path format: {ServerRelativeUrl}",
                serverRelativeUrl
            );
            throw new ArgumentException(
                $"Invalid path format: {serverRelativeUrl}. Expected format: /library/folder/file.pdf"
            );
        }

        string listTitle = filePath.Substring(0, firstSlash);
        string pathInLibrary = filePath.Substring(firstSlash + 1);

        var listId = await GetDocumentLibraryIdByNameAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            _logger.LogError(
                "[CloudSharePointFileManager] DownloadFile - Document library not found: {ListTitle}",
                listTitle
            );
            throw new Exception($"Document library '{listTitle}' not found");
        }

        // Decode the path first in case it's already URL-encoded, then re-encode each segment
        string decodedPath = Uri.UnescapeDataString(pathInLibrary);
        string encodedPath = string.Join("/", decodedPath.Split('/').Select(Uri.EscapeDataString));
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedPath}:/content";

        _logger.LogDebug(
            "[CloudSharePointFileManager] DownloadFile - Request URL: {RequestUrl}",
            requestUrl
        );

        var response = await _Client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "[CloudSharePointFileManager] DownloadFile - Download failed: {StatusCode} - {Error}",
                response.StatusCode,
                errorContent
            );
            throw new Exception(
                $"Failed to download file '{serverRelativeUrl}': {response.StatusCode} - {errorContent}"
            );
        }

        byte[] fileData = await response.Content.ReadAsByteArrayAsync();
        _logger.LogDebug(
            "[CloudSharePointFileManager] DownloadFile - Successfully downloaded file: {ServerRelativeUrl}, size: {Size} bytes",
            serverRelativeUrl,
            fileData.Length
        );
        return fileData;
    }

    /// <summary>
    /// Parse server relative URL to extract file path relative to the site.
    /// </summary>
    /// <example>
    /// var result = ParseServerRelativeUrl("/sites/sitename/documentlibrary/folder/file.pdf")
    /// // result = "documentlibrary/folder/file.pdf"
    /// </example>
    /// <param name="serverRelativeUrl">The server relative URL of the file. Ex: "/sites/sitename/documentlibrary/folder/file.pdf"</param>
    /// <returns>The file path relative to the site. Ex: "documentlibrary/folder/file.pdf"</returns>
    private string ParseServerRelativeUrl(string serverRelativeUrl)
    {
        _logger.LogDebug(
            "[CloudSharePointFileManager] ParseServerRelativeUrl - serverRelativeUrl={ServerRelativeUrl}",
            serverRelativeUrl
        );
        if (string.IsNullOrEmpty(serverRelativeUrl))
        {
            throw new ArgumentException("Server relative URL cannot be null or empty");
        }

        string path = serverRelativeUrl.TrimStart('/');

        // If SiteUrl contains a site path, remove it from the server relative URL
        Uri siteUri = new Uri(SiteUrl);
        string sitePath = siteUri.AbsolutePath.TrimStart('/').TrimEnd('/'); // ex: "sites/sitename"

        if (!string.IsNullOrEmpty(sitePath) && path.StartsWith(sitePath + "/"))
        {
            path = path.Substring(sitePath.Length + 1);
        }

        return path; // ex: "documentlibrary/folder/file.pdf"
    }

    /// <summary>
    /// Delete a file from SharePoint.
    /// NOTE: This method expects the full path including library name as first segment after site path
    /// </summary>
    /// <param name="serverRelativeUrl"></param>
    /// <returns></returns>
    public async Task<bool> DeleteFile(string serverRelativeUrl)
    {
        _logger.LogInformation(
            "[CloudSharePointFileManager] DeleteFile - serverRelativeUrl={ServerRelativeUrl}",
            serverRelativeUrl
        );
        _logger.LogDebug(
            "[CloudSharePointFileManager] DeleteFile - Deleting file: {ServerRelativeUrl}",
            serverRelativeUrl
        );

        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        // Parse to extract file path relative to site
        string filePath = ParseServerRelativeUrl(serverRelativeUrl);

        // Extract library name (first segment) and remaining path
        int firstSlash = filePath.IndexOf('/');
        if (firstSlash <= 0)
        {
            _logger.LogError(
                "[CloudSharePointFileManager] DeleteFile - Invalid path format: {ServerRelativeUrl}",
                serverRelativeUrl
            );
            throw new ArgumentException(
                $"Invalid path format: {serverRelativeUrl}. Expected format: /library/folder/file.pdf"
            );
        }

        string listTitle = filePath.Substring(0, firstSlash);
        string pathInLibrary = filePath.Substring(firstSlash + 1);

        var listId = await GetDocumentLibraryIdByNameAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            _logger.LogError(
                "[CloudSharePointFileManager] DeleteFile - Document library not found: {ListTitle}",
                listTitle
            );
            throw new Exception($"Document library '{listTitle}' not found");
        }

        string encodedPath = string.Join(
            "/",
            pathInLibrary.Split('/').Select(Uri.EscapeDataString)
        );
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedPath}";

        _logger.LogDebug(
            "[CloudSharePointFileManager] DeleteFile - Request URL: {RequestUrl}",
            requestUrl
        );

        var response = await _Client.DeleteAsync(requestUrl);

        if (response.StatusCode == HttpStatusCode.NoContent || response.IsSuccessStatusCode)
        {
            _logger.LogDebug(
                "[CloudSharePointFileManager] DeleteFile - Successfully deleted file: {ServerRelativeUrl}",
                serverRelativeUrl
            );
            return true;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug(
                "[CloudSharePointFileManager] DeleteFile - File not found (already deleted): {ServerRelativeUrl}",
                serverRelativeUrl
            );
            return true;
        }

        string errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogError(
            "[CloudSharePointFileManager] DeleteFile - Delete failed: {StatusCode} - {Error}",
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
        _logger.LogInformation(
            "[CloudSharePointFileManager] DeleteFile - listTitle={ListTitle}, folderName={FolderName}, fileName={FileName}",
            listTitle,
            folderName,
            fileName
        );
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetDocumentLibraryIdByNameAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            throw new Exception($"Document library '{listTitle}' not found");
        }

        folderName = SharePointUtils.FixFoldername(folderName, null, false);
        string encodedFolderName = Uri.EscapeDataString(folderName);
        string encodedFileName = Uri.EscapeDataString(fileName);

        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}/{encodedFileName}";

        _logger.LogDebug(
            "[CloudSharePointFileManager] DeleteFile - Request URL: {RequestUrl}",
            requestUrl
        );

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
        _logger.LogInformation(
            "[CloudSharePointFileManager] RenameFile - oldServerRelativeUrl={OldServerRelativeUrl}, newServerRelativeUrl={NewServerRelativeUrl}",
            oldServerRelativeUrl,
            newServerRelativeUrl
        );
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

        var listId = await GetDocumentLibraryIdByNameAsync(listTitle);
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

        _logger.LogDebug(
            "[CloudSharePointFileManager] RenameFile - Request URL: {RequestUrl}",
            requestUrl
        );

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
                    : $"/drive/root/{newParentPath}",
            },
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUrl)
        {
            Content = content,
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
    /// </summary>
    public async Task<List<SharePointFileDetailsList>> GetFileDetailsListInFolder(
        string listTitle, // EntityName
        string folderName, // FolderName
        string documentType // DocumentYype
    )
    {
        _logger.LogInformation(
            "[CloudSharePointFileManager] GetFileDetailsListInFolder - listTitle={ListTitle}, folderName={FolderName}, documentType={DocumentType}",
            listTitle,
            folderName,
            documentType
        );
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetDocumentLibraryIdByNameAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            return new List<SharePointFileDetailsList>();
        }

        string encodedFolderName = Uri.EscapeDataString(folderName);
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}:/children";

        _logger.LogDebug(
            "[CloudSharePointFileManager] GetFileDetailsListInFolder - Initial request URL: {RequestUrl}",
            requestUrl
        );

        var fileDetailsList = new List<SharePointFileDetailsList>();
        int pageCount = 0;

        _logger.LogDebug(
            "[CloudSharePointFileManager] GetFileDetailsListInFolder - Getting file list from {ListTitle}/{FolderName}, filtering by documentType: {DocumentType}",
            listTitle,
            folderName,
            documentType ?? "(none)"
        );

        // Handle pagination
        while (!string.IsNullOrEmpty(requestUrl))
        {
            pageCount++;
            _logger.LogDebug(
                "[CloudSharePointFileManager] GetFileDetailsListInFolder - Fetching page {PageCount} from: {RequestUrl}",
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
                        "[CloudSharePointFileManager] GetFileDetailsListInFolder - Folder not found: {ListTitle}/{FolderName}, returning empty list",
                        listTitle,
                        folderName
                    );
                    // Folder doesn't exist, return empty list
                    return new List<SharePointFileDetailsList>();
                }

                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "[CloudSharePointFileManager] GetFileDetailsListInFolder - Failed to get folder contents: {StatusCode} - {Error}",
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
                    DocumentType = fileDocType,
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
            "[CloudSharePointFileManager] GetFileDetailsListInFolder - Fetched {PageCount} page(s), found {FileCount} files before filtering",
            pageCount,
            fileDetailsList.Count
        );

        // Filter by document type at the end
        if (documentType != null)
        {
            int beforeFilter = fileDetailsList.Count;
            fileDetailsList = fileDetailsList.Where(f => f.DocumentType == documentType).ToList();
            _logger.LogDebug(
                "[CloudSharePointFileManager] GetFileDetailsListInFolder - Filtered by documentType '{DocumentType}': {BeforeCount} -> {AfterCount} files",
                documentType,
                beforeFilter,
                fileDetailsList.Count
            );
        }

        _logger.LogDebug(
            "[CloudSharePointFileManager] GetFileDetailsListInFolder - Returning {FileCount} files from {ListTitle}/{FolderName}",
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
        _logger.LogInformation(
            "[CloudSharePointFileManager] DeleteFolder - listTitle={ListTitle}, folderName={FolderName}",
            listTitle,
            folderName
        );
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetDocumentLibraryIdByNameAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            return false;
        }

        string encodedFolderName = Uri.EscapeDataString(folderName);
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}";

        _logger.LogDebug(
            "[CloudSharePointFileManager] DeleteFolder - Request URL: {RequestUrl}",
            requestUrl
        );

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
        _logger.LogDebug(
            "[CloudSharePointFileManager] AddFile (Stream, default library) - folderName={FolderName}, fileName={FileName}, contentType={ContentType}",
            folderName,
            fileName,
            contentType
        );
        return await AddFile(
            SharePointConstants.AccountFolderDisplayName,
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
        _logger.LogDebug(
            "[CloudSharePointFileManager] AddFile (Stream) - documentLibrary={DocumentLibrary}, folderName={FolderName}, fileName={FileName}, contentType={ContentType}",
            documentLibrary,
            folderName,
            fileName,
            contentType
        );
        folderName = SharePointUtils.FixFoldername(folderName, null, false);
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
        _logger.LogDebug(
            "[CloudSharePointFileManager] AddFile (byte[], default library) - folderName={FolderName}, fileName={FileName}, dataSize={DataSize}, contentType={ContentType}",
            folderName,
            fileName,
            fileData.Length,
            contentType
        );
        return await AddFile(
            SharePointConstants.AccountFolderDisplayName,
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
        _logger.LogDebug(
            "[CloudSharePointFileManager] AddFile (byte[]) - documentLibrary={DocumentLibrary}, folderName={FolderName}, fileName={FileName}, dataSize={DataSize}, contentType={ContentType}",
            documentLibrary,
            folderName,
            fileName,
            fileData.Length,
            contentType
        );
        folderName = SharePointUtils.FixFoldername(folderName, null, false);
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
        _logger.LogDebug(
            "[CloudSharePointFileManager] UploadFile (Stream) - fileName={FileName}, listTitle={ListTitle}, folderName={FolderName}, contentType={ContentType}",
            fileName,
            listTitle,
            folderName,
            contentType
        );
        using (var ms = new MemoryStream())
        {
            await fileData.CopyToAsync(ms);
            byte[] data = ms.ToArray();
            return await UploadFile(fileName, listTitle, folderName, data, contentType);
        }
    }

    /// <summary>
    /// Truncates filename if needed to comply with SharePoint limits.
    /// </summary>
    /// <remarks>
    /// SharePoint has limits on filename length and total URL length.
    /// This method returns the input filename or a truncated version if needed.
    /// </remarks>
    /// /// <param name="fileName"></param>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public string GetTruncatedFileName(string fileName, string listTitle, string folderName)
    {
        _logger.LogDebug(
            "[CloudSharePointFileManager] GetTruncatedFileName - fileName={FileName}, listTitle={ListTitle}, folderName={FolderName}",
            fileName,
            listTitle,
            folderName
        );
        int maxLength = MAX_SEGMENT_LENGTH;
        fileName = SharePointUtils.FixFilename(fileName, maxLength);
        folderName = SharePointUtils.FixFoldername(folderName, null, false);

        string serverRelativeUrl = GetServerRelativeURL(listTitle, folderName);
        string fullPath = $"{serverRelativeUrl}/{fileName}";

        if (fullPath.Length > MAX_TOTAL_LENGTH)
        {
            int delta = fullPath.Length - MAX_TOTAL_LENGTH;
            maxLength -= delta;
            fileName = SharePointUtils.FixFilename(fileName, maxLength);
        }

        return fileName;
    }

    /// <summary>
    /// Get server relative URL for a folder.
    /// </summary>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public string GetServerRelativeURL(string listTitle, string folderName)
    {
        _logger.LogDebug(
            "[CloudSharePointFileManager] GetServerRelativeURL - listTitle={ListTitle}, folderName={FolderName}",
            listTitle,
            folderName
        );
        folderName = SharePointUtils.FixFoldername(folderName, null, false);

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
    /// Get all folders in a document library that were modified after a specific date.
    /// Uses Microsoft Graph API to retrieve folders from the library's root.
    /// </summary>
    /// <param name="listTitle">The document library title</param>
    /// <param name="afterDate">Only return folders modified after this date</param>
    /// <returns>List of folder items</returns>
    public async Task<List<FolderItem>> GetFoldersInDocumentLibraryAfterDate(
        string listTitle,
        DateTime afterDate
    )
    {
        _logger.LogDebug(
            "[CloudSharePointFileManager] GetFoldersInDocumentLibraryAfterDate - listTitle={ListTitle}, afterDate={AfterDate}",
            listTitle,
            afterDate
        );

        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetDocumentLibraryIdByNameAsync(listTitle);

        if (string.IsNullOrEmpty(listId))
        {
            _logger.LogWarning(
                "[CloudSharePointFileManager] GetFoldersInDocumentLibraryAfterDate - Document library not found: {ListTitle}",
                listTitle
            );
            return new List<FolderItem>();
        }

        List<FolderItem> folderList = new List<FolderItem>();

        try
        {
            // Use SharePoint list items endpoint with indexed field filtering only
            // Filter by Modified date only (likely indexed), then filter for folders client-side
            string filter = $"fields/Modified ge '{afterDate:yyyy-MM-ddTHH:mm:ss}Z'";
            string requestUrl =
                $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/items?$expand=fields&$filter={Uri.EscapeDataString(filter)}&$top=5000";

            _logger.LogDebug(
                "[CloudSharePointFileManager] GetFoldersInDocumentLibraryAfterDate - Request URL: {RequestUrl}",
                requestUrl
            );

            string nextLink = requestUrl;
            int pageCount = 0;
            int totalItems = 0;

            // Paginate through all results
            while (!string.IsNullOrEmpty(nextLink))
            {
                pageCount++;
                _logger.LogDebug(
                    "[CloudSharePointFileManager] GetFoldersInDocumentLibraryAfterDate - Fetching page {PageCount}",
                    pageCount
                );

                var request = new HttpRequestMessage(HttpMethod.Get, nextLink);

                var response = await _Client.SendAsync(request);
                string jsonString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "[CloudSharePointFileManager] GetFoldersInDocumentLibraryAfterDate - Error: {StatusCode}, Response: {Response}",
                        response.StatusCode,
                        jsonString
                    );
                    return folderList;
                }

                var responseObject = JObject.Parse(jsonString);
                var items = responseObject["value"]?.ToObject<List<JObject>>();

                if (items != null)
                {
                    totalItems += items.Count;

                    foreach (var item in items)
                    {
                        var fields = item["fields"];
                        if (fields != null)
                        {
                            // Client-side filter: only process folders (FSObjType = 1)
                            var fsObjType = fields["FSObjType"]?.ToObject<int?>();
                            if (fsObjType == 1)
                            {
                                var folderName = fields["FileLeafRef"]?.ToString();
                                var serverRelativeUrl = fields["FileRef"]?.ToString();
                                var modified = fields["Modified"]?.ToObject<DateTime?>();
                                var created = fields["Created"]?.ToObject<DateTime?>();

                                // Filter out system folders (Forms, etc.)
                                if (
                                    !string.IsNullOrEmpty(folderName)
                                    && !folderName.Equals(
                                        "Forms",
                                        StringComparison.OrdinalIgnoreCase
                                    )
                                )
                                {
                                    var folderItem = new FolderItem
                                    {
                                        Name = folderName,
                                        ServerRelativeUrl = serverRelativeUrl,
                                        TimeCreated = created ?? DateTime.MinValue,
                                        TimeLastModified = modified ?? DateTime.MinValue,
                                    };

                                    folderList.Add(folderItem);
                                }
                            }
                        }
                    }
                }

                // Check for next page
                nextLink = responseObject["@odata.nextLink"]?.ToString();
            }

            _logger.LogInformation(
                "[CloudSharePointFileManager] GetFoldersInDocumentLibraryAfterDate - returning {FolderCount} folders from '{ListTitle}' (processed {TotalItems} items across {PageCount} pages)",
                folderList.Count,
                listTitle,
                totalItems,
                pageCount
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[CloudSharePointFileManager] GetFoldersInDocumentLibraryAfterDate - Exception occurred"
            );
            throw;
        }

        return folderList;
    }

    /// <summary>
    /// Get all child folders in a folder by its server relative URL.
    /// Uses Microsoft Graph API to retrieve child folders.
    /// </summary>
    /// <param name="serverRelativeUrl">The server relative URL or web URL of the parent folder</param>
    /// <returns>List of child folders</returns>
    public async Task<List<FolderItem>> GetChildFolders(string serverRelativeUrl)
    {
        _logger.LogDebug(
            "[CloudSharePointFileManager] GetChildFolders - serverRelativeUrl={ServerRelativeUrl}",
            serverRelativeUrl
        );

        if (string.IsNullOrEmpty(serverRelativeUrl))
        {
            _logger.LogWarning(
                "[CloudSharePointFileManager] GetChildFolders - serverRelativeUrl is null or empty, returning empty list"
            );
            return new List<FolderItem>();
        }

        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        List<FolderItem> folderList = new List<FolderItem>();

        try
        {
            // For Graph API, we need to use the folder's drive item path or ID
            // The serverRelativeUrl might be a web URL, so we need to extract the path
            string folderPath = ExtractFolderPathFromUrl(serverRelativeUrl);

            // Use Graph API to get children of the folder, filtering by folders only
            string requestUrl =
                $"{GraphApiEndpoint}sites/{SiteId}/drive/root:/{Uri.EscapeDataString(folderPath)}:/children?$filter=folder ne null";

            _logger.LogDebug(
                "[CloudSharePointFileManager] GetChildFolders - Request URL: {RequestUrl}",
                requestUrl
            );

            var response = await _Client.GetAsync(requestUrl);
            string jsonString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation(
                        "[CloudSharePointFileManager] GetChildFolders - Folder not found at: {ServerRelativeUrl}",
                        serverRelativeUrl
                    );
                    return folderList;
                }

                _logger.LogError(
                    "[CloudSharePointFileManager] GetChildFolders - Error: {StatusCode}, Response: {Response}",
                    response.StatusCode,
                    jsonString
                );
                return folderList;
            }

            var responseObject = JObject.Parse(jsonString);
            var items = responseObject["value"]?.ToObject<List<JObject>>();

            if (items != null)
            {
                foreach (var item in items)
                {
                    // Only process items that have a folder facet
                    if (item["folder"] != null)
                    {
                        var folderItem = new FolderItem
                        {
                            Name = item["name"]?.ToString(),
                            ServerRelativeUrl = item["webUrl"]?.ToString(),
                            TimeCreated =
                                item["createdDateTime"]?.ToObject<DateTime?>() ?? DateTime.MinValue,
                            TimeLastModified =
                                item["lastModifiedDateTime"]?.ToObject<DateTime?>()
                                ?? DateTime.MinValue,
                        };

                        // Filter out system folders (Forms, etc.)
                        if (
                            !string.IsNullOrEmpty(folderItem.Name)
                            && !folderItem.Name.Equals("Forms", StringComparison.OrdinalIgnoreCase)
                        )
                        {
                            folderList.Add(folderItem);
                        }
                    }
                }
            }

            _logger.LogDebug(
                "[CloudSharePointFileManager] GetChildFolders - Found {Count} child folders",
                folderList.Count
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[CloudSharePointFileManager] GetChildFolders - Exception occurred"
            );
            throw;
        }

        return folderList;
    }

    /// <summary>
    /// Extract the folder path from a server relative URL or web URL
    /// </summary>
    private string ExtractFolderPathFromUrl(string url)
    {
        // If it's a full web URL, extract the path after the site URL
        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            Uri uri = new Uri(url);
            string path = uri.AbsolutePath;

            // Remove the site path from the beginning
            Uri siteUri = new Uri(SiteUrl);
            string sitePath = siteUri.AbsolutePath.TrimEnd('/');

            if (path.StartsWith(sitePath, StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(sitePath.Length).TrimStart('/');
            }

            return path;
        }

        // Otherwise, it's already a relative path, just trim leading slash
        return url.TrimStart('/');
    }

    /// <summary>
    /// Dispose of resources.
    /// </summary>
    public void Dispose()
    {
        _logger.LogDebug("[CloudSharePointFileManager] Dispose - called");
        _Client?.Dispose();
    }
}
