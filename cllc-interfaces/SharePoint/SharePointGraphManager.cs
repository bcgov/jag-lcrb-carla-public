using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gov.Lclb.Cllb.Interfaces;

/// <summary>
/// SharePoint file manager using Microsoft Graph API instead of SharePoint REST API.
/// Provides modern API approach with consistent patterns across Microsoft 365 services.
/// </summary>
public class SharePointGraphManager : ISharePointFileManager
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

    public string SiteUrl { get; set; }
    public string SiteId { get; set; }
    public string GraphApiEndpoint { get; set; } = "https://graph.microsoft.com/v1.0/";

    public SharePointGraphManager(IConfiguration Configuration)
    {
        // Create the HttpClient for Graph API calls
        _Client = new HttpClient();
        _Client.Timeout = TimeSpan.FromSeconds(HttpClientTimeoutSeconds);

        // SharePoint Online configuration
        string sharePointOdataUri = Configuration["SHAREPOINT_ODATA_URI"];
        string sharePointAadTenantId = Configuration["SHAREPOINT_AAD_TENANTID"];
        string sharePointClientId = Configuration["SHAREPOINT_CLIENT_ID"];
        string sharePointClientSecret = Configuration["SHAREPOINT_CLIENT_SECRET"];
        string sharePointCertificatePath = Configuration["SHAREPOINT_CERTIFICATE_PATH"];
        string sharePointCertificatePassword = Configuration["SHAREPOINT_CERTIFICATE_PASSWORD"];

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

        SiteUrl = sharePointOdataUri;

        // Configure OAuth authentication using MSAL with client credentials flow
        string authority = $"https://login.microsoftonline.com/{sharePointAadTenantId}";

        var appBuilder = ConfidentialClientApplicationBuilder
            .Create(sharePointClientId)
            .WithAuthority(new Uri(authority));

        // Support both certificate and client secret authentication
        if (!string.IsNullOrEmpty(sharePointCertificatePath))
        {
            // Certificate-based authentication
            if (!File.Exists(sharePointCertificatePath))
            {
                throw new FileNotFoundException(
                    $"Certificate file not found at path: {sharePointCertificatePath}"
                );
            }

            // Load certificate with private key flags
            // Use EphemeralKeySet for better cross-platform compatibility and to avoid permission issues
            X509Certificate2 certificate;
            X509KeyStorageFlags keyStorageFlags =
                X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable;

            if (!string.IsNullOrEmpty(sharePointCertificatePassword))
            {
                certificate = new X509Certificate2(
                    sharePointCertificatePath,
                    sharePointCertificatePassword,
                    keyStorageFlags
                );
            }
            else
            {
                certificate = new X509Certificate2(
                    sharePointCertificatePath,
                    (string)null,
                    keyStorageFlags
                );
            }

            // Verify the certificate has a private key
            if (!certificate.HasPrivateKey)
            {
                throw new InvalidOperationException(
                    $"The certificate at '{sharePointCertificatePath}' does not contain a private key. "
                        + "Please ensure you're using a .pfx/.p12 file that was exported with the private key included. "
                        + ".cer files only contain the public key and won't work for authentication."
                );
            }

            appBuilder = appBuilder.WithCertificate(certificate);
        }
        else if (!string.IsNullOrEmpty(sharePointClientSecret))
        {
            // Client secret authentication (existing method)
            appBuilder = appBuilder.WithClientSecret(sharePointClientSecret);
        }
        else
        {
            throw new ArgumentException(
                "Either SHAREPOINT_CLIENT_SECRET or SHAREPOINT_CERTIFICATE_PATH configuration is required"
            );
        }

        confidentialClientApp = appBuilder.Build();

        // Standard headers for Graph API access
        _Client.DefaultRequestHeaders.Add("Accept", "application/json");

        // Site ID will be resolved on first API call
    }

    /// <summary>
    /// Ensures a valid access token is available for Microsoft Graph API.
    /// Uses MSAL's built-in token caching.
    /// </summary>
    private async Task EnsureValidAccessTokenAsync()
    {
        int retryDelayMs = InitialRetryDelayMs;

        Exception lastException = null;

        for (int attempt = 0; attempt < MaxAuthRetries; attempt++)
        {
            try
            {
                authenticationResult = await confidentialClientApp
                    .AcquireTokenForClient(new[] { GraphScope })
                    .ExecuteAsync();

                string newAuthHeader = $"Bearer {authenticationResult.AccessToken}";

                _Client.DefaultRequestHeaders.Remove("Authorization");
                _Client.DefaultRequestHeaders.Add("Authorization", newAuthHeader);

                return;
            }
            catch (MsalServiceException ex) when (attempt < MaxAuthRetries - 1)
            {
                lastException = ex;
                await Task.Delay(retryDelayMs);
                retryDelayMs *= 2;
            }
            catch (MsalServiceException ex)
            {
                throw new Exception(
                    $"Failed to acquire Graph API access token after {MaxAuthRetries} attempts: {ex.Message}",
                    ex
                );
            }
            catch (MsalClientException ex)
            {
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
    /// Resolves the SharePoint site ID from the site URL using Graph API.
    /// Site ID is cached after first resolution.
    /// </summary>
    private async Task EnsureSiteIdResolvedAsync()
    {
        if (!string.IsNullOrEmpty(SiteId))
        {
            return; // Already resolved
        }

        await EnsureValidAccessTokenAsync();

        // Parse site URL to get hostname and site path
        Uri siteUri = new Uri(SiteUrl);
        string hostname = siteUri.Host;
        string sitePath = siteUri.AbsolutePath;

        // Graph API endpoint to get site by URL
        string requestUrl = $"{GraphApiEndpoint}sites/{hostname}:{sitePath}";

        var response = await _Client.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();
        var siteInfo = JsonConvert.DeserializeObject<JObject>(responseContent);

        SiteId = siteInfo["id"]?.ToString();

        if (string.IsNullOrEmpty(SiteId))
        {
            throw new Exception($"Failed to resolve site ID for {SiteUrl}");
        }
    }

    /// <summary>
    /// Health check method to verify Microsoft Graph connectivity and authentication.
    /// </summary>
    public async Task<bool> CheckHealthAsync()
    {
        try
        {
            await EnsureValidAccessTokenAsync();
            await EnsureSiteIdResolvedAsync();

            // Make a simple API call to verify access
            string requestUrl = $"{GraphApiEndpoint}sites/{SiteId}";
            var response = await _Client.GetAsync(requestUrl);

            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Get list (document library) by title.
    /// </summary>
    private async Task<string> GetListIdByTitleAsync(string listTitle)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists?$filter=displayName eq '{EscapeApostrophe(listTitle)}'";
        var response = await _Client.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();
        var lists = JsonConvert.DeserializeObject<JObject>(responseContent);
        var list = lists["value"]?.FirstOrDefault();

        return list?["id"]?.ToString();
    }

    /// <summary>
    /// Create a document library (list).
    /// Note: documentTemplateUrlTitle parameter is maintained for API compatibility with REST version,
    /// but Graph API uses a standard "documentLibrary" template and doesn't support custom templates.
    /// </summary>
    public async Task<object> CreateDocumentLibrary(
        string listTitle,
        string documentTemplateUrlTitle = null
    )
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        // For compatibility with original API, use documentTemplateUrlTitle as display name if provided
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
    /// Check if document library exists.
    /// </summary>
    public async Task<bool> DocumentLibraryExists(string listTitle)
    {
        var listId = await GetListIdByTitleAsync(listTitle);
        return !string.IsNullOrEmpty(listId);
    }

    /// <summary>
    /// Get document library metadata.
    /// </summary>
    public async Task<object> GetDocumentLibrary(string listTitle)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            return null;
        }

        string requestUrl = $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}";
        var response = await _Client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject(responseContent);
    }

    /// <summary>
    /// Update document library properties.
    /// </summary>
    public async Task<object> UpdateDocumentLibrary(string listTitle)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            throw new Exception($"Document library '{listTitle}' not found");
        }

        var requestBody = new { displayName = listTitle };

        string requestUrl = $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}";
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
        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject(responseContent);
    }

    /// <summary>
    /// Create a folder in a document library.
    /// </summary>
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
    /// Get folder by name.
    /// </summary>
    public async Task<object> GetFolder(string listTitle, string folderName)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            return null;
        }

        // Use Drive API to get folder (consistent with file operations)
        string encodedFolderName = Uri.EscapeDataString(folderName);
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedFolderName}";
        var response = await _Client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject(responseContent);
    }

    /// <summary>
    /// Check if folder exists.
    /// </summary>
    public async Task<bool> FolderExists(string listTitle, string folderName)
    {
        var folder = await GetFolder(listTitle, folderName);
        return folder != null;
    }

    /// <summary>
    /// Upload a file to SharePoint using Microsoft Graph API.
    /// For files under 4MB, uses simple upload. For larger files, uses resumable upload session.
    /// </summary>
    public async Task<string> UploadFile(
        string fileName,
        string listTitle,
        string folderName,
        byte[] data,
        string contentType
    )
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            throw new Exception($"Document library '{listTitle}' not found");
        }

        // Fix folder and file names to match SharePoint requirements
        folderName = FixFoldername(folderName);
        fileName = GetTruncatedFileName(fileName, listTitle, folderName);

        // Ensure folder exists
        bool folderExists = await FolderExists(listTitle, folderName);
        if (!folderExists)
        {
            await CreateFolder(listTitle, folderName);
        }

        if (data.Length < UploadSessionThreshold)
        {
            // Simple upload for small files
            return await SimpleUpload(listId, folderName, fileName, data, contentType);
        }
        else
        {
            // Resumable upload session for large files
            return await LargeFileUpload(listId, folderName, fileName, data, contentType);
        }
    }

    /// <summary>
    /// Simple upload for files less than 4MB.
    /// </summary>
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

        var content = new ByteArrayContent(data);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        var response = await _Client.PutAsync(requestUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception(
                $"Failed to upload file '{fileName}': {response.StatusCode} - {errorContent}"
            );
        }

        return fileName;
    }

    /// <summary>
    /// Large file upload using resumable upload session for files >= 4MB.
    /// </summary>
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

            var chunkResponse = await _Client.PutAsync(uploadUrl, chunkContent);

            if (
                !chunkResponse.IsSuccessStatusCode
                && chunkResponse.StatusCode != HttpStatusCode.Accepted
            )
            {
                string errorContent = await chunkResponse.Content.ReadAsStringAsync();
                throw new Exception(
                    $"Failed to upload chunk at position {uploadedBytes}: {chunkResponse.StatusCode} - {errorContent}"
                );
            }

            uploadedBytes += bytesToUpload;
        }

        return fileName;
    }

    /// <summary>
    /// Download a file from SharePoint.
    /// NOTE: This method expects the full path including library name as first segment after site path.
    /// For better reliability, extract listTitle from the path and use list-specific drive API.
    /// </summary>
    public async Task<byte[]> DownloadFile(string serverRelativeUrl)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        // Parse to extract file path relative to site
        string filePath = ParseServerRelativeUrl(serverRelativeUrl);

        // Extract library name (first segment) and remaining path
        int firstSlash = filePath.IndexOf('/');
        if (firstSlash <= 0)
        {
            throw new ArgumentException(
                $"Invalid path format: {serverRelativeUrl}. Expected format: /library/folder/file.pdf"
            );
        }

        string listTitle = filePath.Substring(0, firstSlash);
        string pathInLibrary = filePath.Substring(firstSlash + 1);

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            throw new Exception($"Document library '{listTitle}' not found");
        }

        string encodedPath = string.Join(
            "/",
            pathInLibrary.Split('/').Select(Uri.EscapeDataString)
        );
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedPath}:/content";

        var response = await _Client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception(
                $"Failed to download file '{serverRelativeUrl}': {response.StatusCode} - {errorContent}"
            );
        }

        return await response.Content.ReadAsByteArrayAsync();
    }

    /// <summary>
    /// Parse server relative URL to extract file path relative to the site.
    /// Handles URLs like "/sites/sitename/documentlibrary/folder/file.pdf"
    /// </summary>
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
    /// NOTE: This method expects the full path including library name as first segment after site path.
    /// </summary>
    public async Task<bool> DeleteFile(string serverRelativeUrl)
    {
        await EnsureValidAccessTokenAsync();
        await EnsureSiteIdResolvedAsync();

        // Parse to extract file path relative to site
        string filePath = ParseServerRelativeUrl(serverRelativeUrl);

        // Extract library name (first segment) and remaining path
        int firstSlash = filePath.IndexOf('/');
        if (firstSlash <= 0)
        {
            throw new ArgumentException(
                $"Invalid path format: {serverRelativeUrl}. Expected format: /library/folder/file.pdf"
            );
        }

        string listTitle = filePath.Substring(0, firstSlash);
        string pathInLibrary = filePath.Substring(firstSlash + 1);

        var listId = await GetListIdByTitleAsync(listTitle);
        if (string.IsNullOrEmpty(listId))
        {
            throw new Exception($"Document library '{listTitle}' not found");
        }

        string encodedPath = string.Join(
            "/",
            pathInLibrary.Split('/').Select(Uri.EscapeDataString)
        );
        string requestUrl =
            $"{GraphApiEndpoint}sites/{SiteId}/lists/{listId}/drive/root:/{encodedPath}";

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
            $"Failed to delete file '{serverRelativeUrl}': {response.StatusCode} - {errorContent}"
        );
    }

    /// <summary>
    /// Delete a file by list title, folder name, and file name.
    /// </summary>
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
    /// </summary>
    public async Task<List<SharePointFileDetailsList>> GetFileDetailsListInFolder(
        string listTitle,
        string folderName,
        string documentType
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

        // Handle pagination
        while (!string.IsNullOrEmpty(requestUrl))
        {
            var response = await _Client.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // Folder doesn't exist, return empty list
                    return new List<SharePointFileDetailsList>();
                }

                string errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception(
                    $"Failed to get folder contents: {response.StatusCode} - {errorContent}"
                );
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<JObject>(responseContent);
            var files = items["value"]?.Where(i => i["file"] != null).ToList();

            if (files != null)
            {
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

                    var fileDetails = new SharePointFileDetailsList
                    {
                        Name = fileName,
                        ServerRelativeUrl = file["webUrl"]?.ToString(),
                        Length = file["size"]?.ToString(),
                        TimeCreated = file["createdDateTime"]?.ToString(),
                        TimeLastModified = file["lastModifiedDateTime"]?.ToString(),
                        DocumentType = null
                    };

                    // Only set DocumentType if it matches the filter
                    if (fileDoctypeEnd > -1 && fileDocType == documentType)
                    {
                        fileDetails.DocumentType = documentType;
                    }

                    fileDetailsList.Add(fileDetails);
                }
            }

            // Check for next page
            requestUrl = items["@odata.nextLink"]?.ToString();
        }

        // Filter by document type at the end, like REST version
        if (documentType != null)
        {
            fileDetailsList = fileDetailsList.Where(f => f.DocumentType == documentType).ToList();
        }

        return fileDetailsList;
    }

    /// <summary>
    /// Delete a folder.
    /// </summary>
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
    /// Removes invalid characters from folder names, for SharePoint compatibility.
    /// </summary>
    public string FixFoldername(string foldername)
    {
        return RemoveInvalidCharacters(foldername);
    }

    /// <summary>
    /// Removes invalid characters and truncates filename to max length for SharePoint compatibility.
    /// </summary>
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
    /// SharePoint has limits on filename length and total URL length.
    /// This method returns the input filename or a truncated version if needed.
    /// </summary>
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
    /// Escape single quotes for OData filter queries.
    /// </summary>
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
    /// Generate upload request URI string (for compatibility with REST version).
    /// </summary>
    public string GenerateUploadRequestUriString(string folderServerRelativeUrl, string fileName)
    {
        string encodedFolder = string.Join(
            "/",
            folderServerRelativeUrl.Split('/').Select(Uri.EscapeDataString)
        );

        string encodedFileName = Uri.EscapeDataString(fileName);

        return $"{GraphApiEndpoint}sites/{SiteId}/drive/root:/{encodedFolder}/{encodedFileName}:/content";
    }

    /// <summary>
    /// Dispose of resources.
    /// </summary>
    public void Dispose()
    {
        _Client?.Dispose();
    }
}
