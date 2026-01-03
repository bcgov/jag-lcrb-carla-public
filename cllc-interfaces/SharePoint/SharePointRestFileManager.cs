using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gov.Lclb.Cllb.Interfaces;

public class SharePointRestFileManager : ISharePointFileManager
{
    private AuthenticationResult authenticationResult;
    private IConfidentialClientApplication confidentialClientApp;
    private string oauthScope;
    private HttpClient _Client;

    public string OdataUri { get; set; }
    public string WebName { get; set; }
    public string ApiEndpoint { get; set; }

    public SharePointRestFileManager(IConfiguration Configuration)
    {
        // Create the HttpClient for REST API calls
        _Client = new HttpClient();
        // Set timeout to 100 seconds to handle large file uploads/downloads
        _Client.Timeout = TimeSpan.FromSeconds(100);

        // SharePoint Online configuration
        string sharePointOdataUri = Configuration["SHAREPOINT_ODATA_URI"];
        string sharePointWebname = Configuration["SHAREPOINT_WEBNAME"];
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

        OdataUri = sharePointOdataUri;

        WebName = sharePointWebname;

        // Ensure the webname has a slash
        if (!string.IsNullOrEmpty(WebName) && WebName[0] != '/')
        {
            WebName = "/" + WebName;
        }

        ApiEndpoint = sharePointOdataUri;
        // Ensure there is a trailing slash
        if (!ApiEndpoint.EndsWith("/"))
        {
            ApiEndpoint += "/";
        }
        ApiEndpoint += "_api/";

        // Configure OAuth authentication using MSAL with client credentials flow
        string authority = $"https://login.microsoftonline.com/{sharePointAadTenantId}";

        confidentialClientApp = ConfidentialClientApplicationBuilder
            .Create(sharePointClientId)
            .WithClientSecret(sharePointClientSecret)
            .WithAuthority(new Uri(authority))
            .Build();

        // Get the SharePoint site URL and extract the tenant
        Uri siteUri = new Uri(sharePointOdataUri);
        oauthScope = $"{siteUri.Scheme}://{siteUri.Host}/.default";

        // Standard headers for API access
        _Client.DefaultRequestHeaders.Add("Accept", "application/json;odata=nometadata");
        _Client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        _Client.DefaultRequestHeaders.Add("OData-Version", "4.0");

        // Token will be acquired on first API call via EnsureValidAccessTokenAsync
    }

    /// <summary>
    /// Ensures a valid access token is available.
    /// Uses MSAL's built-in token caching - will return cached token if still valid,
    /// or automatically refresh if expired.
    /// Implements retry logic with exponential backoff for transient failures.
    /// </summary>
    private async Task EnsureValidAccessTokenAsync()
    {
        // Retry logic with exponential backoff for transient failures
        int maxRetries = 3;
        int retryDelayMs = 1000; // Start with 1 second
        Exception lastException = null;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                // MSAL handles token caching automatically
                // If the token is still valid, it returns from cache
                // If expired, it automatically refreshes
                authenticationResult = await confidentialClientApp
                    .AcquireTokenForClient(new[] { oauthScope })
                    .ExecuteAsync();

                Console.WriteLine("1-----------------------------------------");
                Console.WriteLine(authenticationResult.AccessToken);
                Console.WriteLine("1-----------------------------------------");

                // Update the Authorization header with the current (possibly refreshed) token
                string newAuthHeader = $"Bearer {authenticationResult.AccessToken}";

                // Remove old Authorization header if present
                _Client.DefaultRequestHeaders.Remove("Authorization");

                // Add the updated Authorization header
                _Client.DefaultRequestHeaders.Add("Authorization", newAuthHeader);

                // Success - exit retry loop
                return;
            }
            catch (MsalServiceException ex) when (attempt < maxRetries - 1)
            {
                // Transient error - retry with exponential backoff
                lastException = ex;
                await Task.Delay(retryDelayMs);
                retryDelayMs *= 2; // Exponential backoff
            }
            catch (MsalServiceException ex)
            {
                // Final retry failed
                throw new Exception(
                    $"Failed to acquire access token after {maxRetries} attempts: {ex.Message}",
                    ex
                );
            }
            catch (MsalClientException ex)
            {
                // Client configuration error - don't retry
                throw new Exception(
                    $"Client configuration error when acquiring token: {ex.Message}",
                    ex
                );
            }
        }

        // Should not reach here, but just in case
        if (lastException != null)
        {
            throw new Exception(
                $"Failed to acquire access token after {maxRetries} attempts",
                lastException
            );
        }
    }

    /// <summary>
    /// Health check method to verify SharePoint connectivity and authentication.
    /// Acquires an OAuth token and makes a simple API call to verify access.
    /// </summary>
    /// <returns>True if SharePoint is accessible, false otherwise.</returns>
    public async Task<bool> CheckHealthAsync()
    {
        try
        {
            // Ensure we can acquire a valid access token
            await EnsureValidAccessTokenAsync();

            // Make a simple API call to verify we can access SharePoint
            // Get web properties - this is a lightweight, read-only operation
            HttpRequestMessage healthCheckRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(ApiEndpoint + "web?$select=Title"),
                Headers = { { "Accept", "application/json" } }
            };

            var response = await _Client.SendAsync(healthCheckRequest);

            // Return true if we got a successful response
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            // Any exception means SharePoint is not healthy
            return false;
        }
    }

    /// <summary>
    /// Get file details list from SharePoint filtered by folder name and document type
    /// </summary>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <param name="documentType"></param>
    /// <returns></returns>
    public async Task<List<SharePointFileDetailsList>> GetFileDetailsListInFolder(
        string listTitle,
        string folderName,
        string documentType
    )
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        folderName = FixFoldername(folderName);

        string serverRelativeUrl = "";
        // ensure the webname has a slash.
        if (!string.IsNullOrEmpty(WebName))
        {
            serverRelativeUrl += $"{WebName}/";
        }

        serverRelativeUrl += Uri.EscapeDataString(listTitle);
        if (!string.IsNullOrEmpty(folderName))
        {
            serverRelativeUrl += "/" + Uri.EscapeDataString(folderName);
        }

        string _responseContent = null;
        HttpRequestMessage _httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(
                ApiEndpoint
                    + "web/getFolderByServerRelativeUrl('"
                    + EscapeApostrophe(serverRelativeUrl)
                    + "')/files"
            ),
            Headers = { { "Accept", "application/json" } }
        };

        // make the request.
        var _httpResponse = await _Client.SendAsync(_httpRequest);
        HttpStatusCode _statusCode = _httpResponse.StatusCode;

        if ((int)_statusCode != 200)
        {
            var ex = new SharePointRestException(
                string.Format("Operation returned an invalid status code C '{0}'", _statusCode)
            );
            _responseContent = await _httpResponse
                .Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            ex.Request = new HttpRequestMessageWrapper(_httpRequest, null);
            ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);

            _httpRequest.Dispose();
            if (_httpResponse != null)
            {
                _httpResponse.Dispose();
            }
            throw ex;
        }
        else
        {
            _responseContent = await _httpResponse.Content.ReadAsStringAsync();
        }

        // parse the response
        JObject responseObject = null;
        try
        {
            responseObject = JObject.Parse(_responseContent);
        }
        catch (JsonReaderException jre)
        {
            throw jre;
        }
        // get JSON response objects into a list
        List<JToken> responseResults = responseObject["value"].Children().ToList();

        // create file details list to add from response
        List<SharePointFileDetailsList> fileDetailsList = new List<SharePointFileDetailsList>();
        // create .NET objects
        foreach (JToken responseResult in responseResults)
        {
            // JToken.ToObject is a helper method that uses JsonSerializer internally
            SharePointFileDetailsList searchResult =
                responseResult.ToObject<SharePointFileDetailsList>();
            //filter by parameter documentType
            int fileDoctypeEnd = searchResult.Name.IndexOf("__");
            if (fileDoctypeEnd > -1)
            {
                string fileDoctype = searchResult.Name.Substring(0, fileDoctypeEnd);
                if (fileDoctype == documentType)
                {
                    searchResult.DocumentType = documentType;
                }
            }
            fileDetailsList.Add(searchResult);
        }

        if (documentType != null)
        {
            fileDetailsList = fileDetailsList.Where(f => f.DocumentType == documentType).ToList();
        }
        return fileDetailsList;
    }

    /// <summary>
    /// Create Folder
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task CreateFolder(string listTitle, string folderName)
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        folderName = FixFoldername(folderName);

        string relativeUrl = EscapeApostrophe($"/{listTitle}/{folderName}");

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(ApiEndpoint + $"web/folders/add('{relativeUrl}')"),
            Headers = { { "Accept", "application/json" } }
        };

        //string jsonString = "{ '__metadata': { 'type': 'SP.Folder' }, 'ServerRelativeUrl': '" + relativeUrl + "'}";

        StringContent strContent = new StringContent("", Encoding.UTF8);
        strContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            "application/json;odata=nometadata"
        );

        endpointRequest.Content = strContent;

        // make the request.

        var response = await _Client.SendAsync(endpointRequest);
        HttpStatusCode _statusCode = response.StatusCode;

        // check to see if the folder creation worked.
        if (!(_statusCode == HttpStatusCode.OK || _statusCode == HttpStatusCode.Created))
        {
            string _responseContent;
            var ex = new SharePointRestException(
                string.Format("Operation returned an invalid status code D '{0}'", _statusCode)
            );
            _responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            ex.Request = new HttpRequestMessageWrapper(endpointRequest, null);
            ex.Response = new HttpResponseMessageWrapper(response, _responseContent);

            endpointRequest.Dispose();
            if (response != null)
            {
                response.Dispose();
            }
            throw ex;
        }
        else
        {
            string jsonString = await response.Content.ReadAsStringAsync();
        }
    }

    /// <summary>
    /// Create Folder
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<Object> CreateDocumentLibrary(
        string listTitle,
        string documentTemplateUrlTitle = null
    )
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        HttpRequestMessage endpointRequest = new HttpRequestMessage(
            HttpMethod.Post,
            ApiEndpoint + "web/Lists"
        );

        if (string.IsNullOrEmpty(documentTemplateUrlTitle))
        {
            documentTemplateUrlTitle = listTitle;
        }
        var library = CreateNewDocumentLibraryRequest(documentTemplateUrlTitle);

        string jsonString = JsonConvert.SerializeObject(library);
        StringContent strContent = new StringContent(jsonString, Encoding.UTF8);
        strContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            "application/json;odata=nometadata"
        );
        endpointRequest.Content = strContent;
        // fix for bad request
        endpointRequest.Headers.Add("odata-version", "4.0");

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);
        HttpStatusCode _statusCode = response.StatusCode;

        if (_statusCode != HttpStatusCode.Created)
        {
            Console.WriteLine($"{System.Text.Json.JsonSerializer.Serialize(response)}");
            Console.WriteLine($"{_statusCode}");

            var ex = new SharePointRestException(
                string.Format("Operation returned an invalid status code E '{0}'", _statusCode)
            );
            string _responseContent = await response
                .Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            ex.Request = new HttpRequestMessageWrapper(endpointRequest, null);
            ex.Response = new HttpResponseMessageWrapper(response, _responseContent);

            endpointRequest.Dispose();
            if (response != null)
            {
                response.Dispose();
            }
            throw ex;
        }
        else
        {
            jsonString = await response.Content.ReadAsStringAsync();
            var ob =
                Newtonsoft.Json.JsonConvert.DeserializeObject<SharePointDocumentLibraryResponse>(
                    jsonString
                );

            if (listTitle != documentTemplateUrlTitle)
            {
                // update list title
                endpointRequest = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"{ApiEndpoint}web/lists(guid'{ob.d.Id}')"
                );
                var type = new { type = "SP.List" };
                var request = new { __metadata = type, Title = listTitle };
                jsonString = JsonConvert.SerializeObject(request);
                strContent = new StringContent(jsonString, Encoding.UTF8);
                strContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                    "application/json;odata=nometadata"
                );
                endpointRequest.Headers.Add("IF-MATCH", "*");
                endpointRequest.Headers.Add("X-HTTP-Method", "MERGE");
                endpointRequest.Content = strContent;
                response = await _Client.SendAsync(endpointRequest);
                jsonString = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }
        }

        return library;
    }

    public async Task<Object> UpdateDocumentLibrary(string listTitle)
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        HttpRequestMessage endpointRequest = new HttpRequestMessage(
            HttpMethod.Put,
            $"{ApiEndpoint}web/Lists"
        );

        var library = CreateNewDocumentLibraryRequest(listTitle);

        string jsonString = JsonConvert.SerializeObject(library);
        StringContent strContent = new StringContent(jsonString, Encoding.UTF8);
        strContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            "application/json;odata=nometadata"
        );
        endpointRequest.Content = strContent;

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);
        HttpStatusCode _statusCode = response.StatusCode;

        if (_statusCode != HttpStatusCode.Created)
        {
            string _responseContent = null;
            var ex = new SharePointRestException(
                string.Format("Operation returned an invalid status code F '{0}'", _statusCode)
            );
            _responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            ex.Request = new HttpRequestMessageWrapper(endpointRequest, null);
            ex.Response = new HttpResponseMessageWrapper(response, _responseContent);

            endpointRequest.Dispose();
            if (response != null)
            {
                response.Dispose();
            }
            throw ex;
        }
        else
        {
            jsonString = await response.Content.ReadAsStringAsync();
        }

        return library;
    }

    public async Task<bool> DeleteFolder(string listTitle, string folderName)
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        folderName = FixFoldername(folderName);

        bool result = false;
        // Delete is very similar to a GET.
        string serverRelativeUrl = "/";
        if (!string.IsNullOrEmpty(WebName))
        {
            serverRelativeUrl += $"{WebName}/";
        }

        serverRelativeUrl += $"{listTitle}/{folderName}";

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(
                ApiEndpoint
                    + "web/getFolderByServerRelativeUrl('"
                    + EscapeApostrophe(serverRelativeUrl)
                    + "')"
            ),
            Headers = { { "Accept", "application/json" } }
        };

        // We want to delete this folder.
        endpointRequest.Headers.Add("IF-MATCH", "*");
        endpointRequest.Headers.Add("X-HTTP-Method", "DELETE");

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            result = true;
        }
        else
        {
            string _responseContent = null;
            var ex = new SharePointRestException(
                string.Format(
                    "Operation returned an invalid status code G '{0}'",
                    response.StatusCode
                )
            );
            _responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            ex.Request = new HttpRequestMessageWrapper(endpointRequest, null);
            ex.Response = new HttpResponseMessageWrapper(response, _responseContent);

            endpointRequest.Dispose();
            if (response != null)
            {
                response.Dispose();
            }
            throw ex;
        }

        return result;
    }

    public async Task<bool> FolderExists(string listTitle, string folderName)
    {
        Object folder = await GetFolder(listTitle, folderName);

        return (folder != null);
    }

    public async Task<bool> DocumentLibraryExists(string listTitle)
    {
        Object lisbrary = await GetDocumentLibrary(listTitle);

        return (lisbrary != null);
    }

    public async Task<Object> GetFolder(string listTitle, string folderName)
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        folderName = FixFoldername(folderName);

        Object result = null;
        string serverRelativeUrl = "/";
        if (!string.IsNullOrEmpty(WebName))
        {
            serverRelativeUrl += $"{WebName}/";
        }

        serverRelativeUrl += $"{listTitle}/{folderName}";

        HttpRequestMessage endpointRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(
                ApiEndpoint
                    + "web/getFolderByServerRelativeUrl('"
                    + EscapeApostrophe(serverRelativeUrl)
                    + "')"
            ),
            Headers = { { "Accept", "application/json" } }
        };

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);
        string jsonString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            result = JsonConvert.DeserializeObject(jsonString);
        }

        return result;
    }

    public async Task<Object> GetDocumentLibrary(string listTitle)
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        Object result = null;
        string title = Uri.EscapeDataString(listTitle);
        string query = $"web/lists/GetByTitle('{title}')";

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(ApiEndpoint + query),
            Headers = { { "Accept", "application/json" } }
        };

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);
        string jsonString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            result = JsonConvert.DeserializeObject(jsonString);
        }

        return result;
    }

    public async Task<string> AddFile(
        String folderName,
        String fileName,
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

    public async Task<string> AddFile(
        String documentLibrary,
        String folderName,
        String fileName,
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

        // now add the file to the folder.

        fileName = await UploadFile(fileName, documentLibrary, folderName, fileData, contentType);

        return fileName;
    }

    public async Task<string> AddFile(
        String folderName,
        String fileName,
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

    public async Task<string> AddFile(
        String documentLibrary,
        String folderName,
        String fileName,
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

        // now add the file to the folder.

        fileName = await UploadFile(fileName, documentLibrary, folderName, fileData, contentType);

        return fileName;
    }

    /// <summary>
    /// Upload a file
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <param name="fileData"></param>
    /// <param name="contentType"></param>
    /// <returns>Uploaded Filename, or Null if not successful.</returns>
    public async Task<string> UploadFile(
        string fileName,
        string listTitle,
        string folderName,
        Stream fileData,
        string contentType
    )
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        // convert the stream into a byte array.
        MemoryStream ms = new MemoryStream();
        fileData.CopyTo(ms);
        byte[] data = ms.ToArray();
        return await UploadFile(fileName, listTitle, folderName, data, contentType);
    }

    /// <summary>
    /// Upload a file
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="listTitle"></param>
    /// <param name="folderName"></param>
    /// <param name="fileData"></param>
    /// <param name="contentType"></param>
    /// <returns>Uploaded Filename, or Null if not successful.</returns>
    public async Task<string> UploadFile(
        string fileName,
        string listTitle,
        string folderName,
        byte[] data,
        string contentType
    )
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        folderName = FixFoldername(folderName);
        fileName = GetTruncatedFileName(fileName, listTitle, folderName);

        string serverRelativeUrl = GetServerRelativeURL(listTitle, folderName);
        string requestUriString = GenerateUploadRequestUriString(serverRelativeUrl, fileName);

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestUriString),
            Headers = { { "Accept", "application/json" } }
        };

        ByteArrayContent byteArrayContent = new ByteArrayContent(data);
        byteArrayContent.Headers.Add(@"content-length", data.Length.ToString());
        endpointRequest.Content = byteArrayContent;

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            string _responseContent = null;
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(response));
            var ex = new SharePointRestException(
                string.Format(
                    "Operation returned an invalid status code G '{0}'",
                    response.StatusCode
                )
            );
            _responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            ex.Request = new HttpRequestMessageWrapper(endpointRequest, null);
            ex.Response = new HttpResponseMessageWrapper(response, _responseContent);

            endpointRequest.Dispose();
            if (response != null)
            {
                response.Dispose();
            }
            throw ex;
        }

        return fileName;
    }

    /// <summary>
    /// Download a file
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<byte[]> DownloadFile(string url)
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        byte[] result = null;

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(
                ApiEndpoint
                    + "web/GetFileByServerRelativeUrl('"
                    + EscapeApostrophe(url)
                    + "')/$value"
            ),
        };

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);

        using (MemoryStream ms = new MemoryStream())
        {
            await response.Content.CopyToAsync(ms);
            result = ms.ToArray();
        }

        return result;
    }

    /// <summary>
    /// Delete a file
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<bool> DeleteFile(string listTitle, string folderName, string fileName)
    {
        bool result = false;
        // Delete is very similar to a GET.
        string serverRelativeUrl = "";
        if (!string.IsNullOrEmpty(WebName))
        {
            serverRelativeUrl += $"{WebName}/";
        }

        folderName = FixFoldername(folderName);

        serverRelativeUrl += $"/{listTitle}/{folderName}/{fileName}";

        result = await DeleteFile(serverRelativeUrl);

        return result;
    }

    public async Task<bool> DeleteFile(string serverRelativeUrl)
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        bool result = false;
        // Delete is very similar to a GET.

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(
                ApiEndpoint
                    + "web/GetFileByServerRelativeUrl('"
                    + EscapeApostrophe(serverRelativeUrl)
                    + "')"
            ),
            Headers = { { "Accept", "application/json" } }
        };

        // We want to delete this file.
        endpointRequest.Headers.Add("IF-MATCH", "*");
        endpointRequest.Headers.Add("X-HTTP-Method", "DELETE");

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            result = true;
        }
        else
        {
            string _responseContent = null;
            var ex = new SharePointRestException(
                string.Format(
                    "Operation returned an invalid status code A '{0}'",
                    response.StatusCode
                )
            );
            _responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            ex.Request = new HttpRequestMessageWrapper(endpointRequest, null);
            ex.Response = new HttpResponseMessageWrapper(response, _responseContent);

            endpointRequest.Dispose();
            if (response != null)
            {
                response.Dispose();
            }
            throw ex;
        }

        return result;
    }

    /// <summary>
    /// Rename a file.  Note that this only works for files with relatively short names due to the max URL length.  It may be possible to allow that to work by using @variables in the URL.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<bool> RenameFile(string oldServerRelativeUrl, string newServerRelativeUrl)
    {
        // Ensure we have a valid access token (auto-refresh if needed)
        await EnsureValidAccessTokenAsync();

        string url =
            $"{ApiEndpoint}web/GetFileByServerRelativeUrl('{EscapeApostrophe(oldServerRelativeUrl)}')/moveto(newurl='{EscapeApostrophe(newServerRelativeUrl)}', flags=1)";
        HttpRequestMessage endpointRequest = new HttpRequestMessage(HttpMethod.Post, url);

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            string _responseContent = null;
            var ex = new SharePointRestException(
                string.Format(
                    "Operation returned an invalid status code B '{0}'",
                    response.StatusCode
                )
            );
            _responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            ex.Request = new HttpRequestMessageWrapper(endpointRequest, null);
            ex.Response = new HttpResponseMessageWrapper(response, _responseContent);
            endpointRequest.Dispose();
            if (response != null)
            {
                response.Dispose();
            }
            throw ex;
        }

        return true;
    }

    /// <summary>
    /// Removes invalid characters from the filename string, for SharePoint compatibility.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns>The filename.</returns>
    public string RemoveInvalidCharacters(string filename)
    {
        var osInvalidChars = new string(Path.GetInvalidFileNameChars());

        osInvalidChars += "~#%&*()[]{}"; // add additional characters that do not work with SharePoint

        string invalidChars = Regex.Escape(osInvalidChars);

        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        // Get the validated file name string
        string result = Regex.Replace(filename, invalidRegStr, "_");

        return result;
    }

    /// <summary>
    /// Removes invalid characters from folder names for SharePoint compatibility.
    /// </summary>
    /// <param name="foldername"></param>
    /// <returns>The foldername.</returns>
    public string FixFoldername(string foldername)
    {
        string result = RemoveInvalidCharacters(foldername);

        return result;
    }

    /// <summary>
    /// Removes invalid characters from folder names, for SharePoint compatibility.
    /// Truncates the filename if it exceeds the maxLength, ensuring it stays within SharePoint limits.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="maxLength"></param>
    /// <returns>The filename.</returns>
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
    /// SharePoint is very particular about the file name length and the total characters in the URL to access a file.
    /// This method returns the input file name or a truncated version of the file name if it is over the max number of characters.
    /// </summary>
    /// <param name="fileName">The file name to check; e.g. "abcdefg1111222233334444.pdf"</param>
    /// <param name="listTitle">The list title</param>
    /// <param name="folderName">The    folder name where the file would be uploaded</param>
    /// <returns>The (potentially truncated) file name; e.g. "abcd.pdf"</returns>
    public string GetTruncatedFileName(string fileName, string listTitle, string folderName)
    {
        // SharePoint requires that filenames are less than 128 characters.
        int maxLength = 128;
        fileName = FixFilename(fileName, maxLength);

        folderName = FixFoldername(folderName);

        // SharePoint also imposes a limit on the whole URL
        string serverRelativeUrl = GetServerRelativeURL(listTitle, folderName);
        string requestUriString = GenerateUploadRequestUriString(serverRelativeUrl, fileName);
        if (requestUriString.Length > SharePointConstants.MaxUrlLength)
        {
            int delta = requestUriString.Length - SharePointConstants.MaxUrlLength;
            maxLength -= delta;
            fileName = FixFilename(fileName, maxLength);
        }

        return fileName;
    }

    /// <summary>
    /// Escape the apostrophe character. Since we use it to enclose the filename it must be escaped.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns>Filename, with apropstophes escaped.</returns>
    public string EscapeApostrophe(string filename)
    {
        string result = null;
        if (!string.IsNullOrEmpty(filename))
        {
            result = filename.Replace("'", "''");
        }
        return result;
    }

    public string GetServerRelativeURL(string listTitle, string folderName)
    {
        folderName = FixFoldername(folderName);
        string serverRelativeUrl = "";
        if (!string.IsNullOrEmpty(WebName))
        {
            serverRelativeUrl += $"{WebName}/";
        }

        serverRelativeUrl +=
            Uri.EscapeDataString(listTitle) + "/" + Uri.EscapeDataString(folderName);

        return serverRelativeUrl;
    }

    public string GenerateUploadRequestUriString(string folderServerRelativeUrl, string fileName)
    {
        string requestUriString =
            ApiEndpoint
            + "web/getFolderByServerRelativeUrl('"
            + EscapeApostrophe(folderServerRelativeUrl)
            + "')/Files/add(url='"
            + EscapeApostrophe(fileName)
            + "',overwrite=true)";
        return requestUriString;
    }

    public object CreateNewDocumentLibraryRequest(string listName)
    {
        var type = new { type = "SP.List" };
        var request = new
        {
            __metadata = type,
            BaseTemplate = 101,
            Title = listName
        };
        return request;
    }
}
