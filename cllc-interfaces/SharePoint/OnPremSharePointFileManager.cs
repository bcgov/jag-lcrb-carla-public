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
/// Methods for calling On-Premise SharePoint REST APIs.
/// </summary>
public partial class OnPremSharePointFileManager : ISharePointFileManager
{
    private const int MAX_TOTAL_LENGTH = 260; // default maximum URL length.
    private const int MAX_SEGMENT_LENGTH = 128; // default maximum segment length.

    private AuthenticationResult authenticationResult;

    public string OdataUri { get; set; }
    public string ServerAppIdUri { get; set; }
    public string WebName { get; set; }
    public string ApiEndpoint { get; set; }
    public string NativeBaseUri { get; set; }
    string Authorization { get; set; }
    private HttpClient _Client;
    private string Digest;
    private CookieContainer _CookieContainer;
    private HttpClientHandler _HttpClientHandler;

    public OnPremSharePointFileManager(IConfiguration Configuration)
    {
        // create the HttpClient that is used for our direct REST calls.
        _CookieContainer = new CookieContainer();

        // SharePoint configuration settings.
        string sharePointServerAppIdUri = Configuration["SHAREPOINT_SERVER_APPID_URI"];
        string sharePointOdataUri = Configuration["SHAREPOINT_ODATA_URI"];
        string sharePointWebname = Configuration["SHAREPOINT_WEBNAME"];
        string sharePointNativeBaseURI = Configuration["SHAREPOINT_NATIVE_BASE_URI"];

        string bypassSharePointCertValidation = Configuration["BYPASS_STS_CERT_VALIDATION"]; // Bypass SharePoint certificate validation (true/false)

        if (
            !string.IsNullOrEmpty(bypassSharePointCertValidation)
            && bypassSharePointCertValidation.ToLower() == "true"
        )
        {
            _HttpClientHandler = new HttpClientHandler()
            {
                UseCookies = true,
                AllowAutoRedirect = false,
                CookieContainer = _CookieContainer,
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (
                    httpRequestMessage,
                    cert,
                    cetChain,
                    policyErrors
                ) =>
                {
                    // Ignore all certificate validation errors.
                    return true;
                },
            };
        }
        else
        {
            _HttpClientHandler = new HttpClientHandler()
            {
                UseCookies = true,
                AllowAutoRedirect = false,
                CookieContainer = _CookieContainer,
            };
        }

        _Client = new HttpClient(_HttpClientHandler);
        _Client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

        // ADFS using fed auth

        string sharePointStsTokenUri = Configuration["SHAREPOINT_STS_TOKEN_URI"]; // Full URI to the STS service we will use to get the initial token.
        string sharePointRelyingPartyIdentifier = Configuration[
            "SHAREPOINT_RELYING_PARTY_IDENTIFIER"
        ]; // use Fiddler to grab this from an interactive session.  Will normally start with urn:
        string sharePointUsername = Configuration["SHAREPOINT_USERNAME"]; // Service account username.  Be sure to add this user to the SharePoint instance.
        string sharePointPassword = Configuration["SHAREPOINT_PASSWORD"]; // Service account password

        // SharePoint Online
        string sharePointAadTenantId = Configuration["SHAREPOINT_AAD_TENANTID"];
        string sharePointClientId = Configuration["SHAREPOINT_CLIENT_ID"];
        string sharePointCertFileName = Configuration["SHAREPOINT_CERTIFICATE_FILENAME"];
        string sharePointCertPassword = Configuration["SHAREPOINT_CERTIFICATE_PASSWORD"];

        // Basic Auth (SSG API Gateway)
        string ssgUsername = Configuration["SSG_USERNAME"]; // BASIC authentication username
        string ssgPassword = Configuration["SSG_PASSWORD"]; // BASIC authentication password

        // sometimes SharePoint could be using a different username / password.
        string sharePointSsgUsername = Configuration["SHAREPOINT_SSG_USERNAME"];
        string sharePointSsgPassword = Configuration["SHAREPOINT_SSG_PASSWORD"];

        if (string.IsNullOrEmpty(sharePointSsgUsername))
        {
            sharePointSsgUsername = ssgUsername;
        }

        if (string.IsNullOrEmpty(sharePointSsgPassword))
        {
            sharePointSsgPassword = ssgPassword;
        }

        OdataUri = sharePointOdataUri;
        ServerAppIdUri = sharePointServerAppIdUri;
        NativeBaseUri = sharePointNativeBaseURI;
        WebName = sharePointWebname;

        // ensure the webname has a slash.
        if (!string.IsNullOrEmpty(WebName) && WebName[0] != '/')
        {
            WebName = "/" + WebName;
        }

        ApiEndpoint = sharePointOdataUri;
        // ensure there is a trailing slash.
        if (!ApiEndpoint.EndsWith("/"))
        {
            ApiEndpoint += "/";
        }
        ApiEndpoint += "_api/";

        // Scenario #1 - ADFS (2016) using FedAuth
        if (
            !string.IsNullOrEmpty(sharePointRelyingPartyIdentifier)
            && !string.IsNullOrEmpty(sharePointUsername)
            && !string.IsNullOrEmpty(sharePointPassword)
            && !string.IsNullOrEmpty(sharePointStsTokenUri)
        )
        {
            Authorization = null;
            var samlST = Authentication
                .GetStsSamlToken(
                    sharePointRelyingPartyIdentifier,
                    sharePointUsername,
                    sharePointPassword,
                    sharePointStsTokenUri,
                    _Client
                )
                .GetAwaiter()
                .GetResult();
            //FedAuthValue =
            Authentication
                .GetFedAuth(
                    sharePointOdataUri,
                    samlST,
                    sharePointRelyingPartyIdentifier,
                    _Client,
                    _CookieContainer
                )
                .GetAwaiter()
                .GetResult();
        }
        // Scenario #2 - SharePoint Online (Cloud) using a Client Certificate
        else if (
            !string.IsNullOrEmpty(sharePointAadTenantId)
            && !string.IsNullOrEmpty(sharePointCertFileName)
            && !string.IsNullOrEmpty(sharePointCertPassword)
            && !string.IsNullOrEmpty(sharePointClientId)
        )
        {
            // add authentication.
            var authenticationContext = new AuthenticationContext(
                "https://login.windows.net/" + sharePointAadTenantId
            );

            // Create the Client cert.
            X509Certificate2 cert = new X509Certificate2(
                sharePointCertFileName,
                sharePointCertPassword
            );
            ClientAssertionCertificate clientAssertionCertificate = new ClientAssertionCertificate(
                sharePointClientId,
                cert
            );

            //ClientCredential clientCredential = new ClientCredential(clientId, clientKey);
            var task = authenticationContext.AcquireTokenAsync(
                sharePointServerAppIdUri,
                clientAssertionCertificate
            );
            task.Wait();
            authenticationResult = task.Result;
            Authorization = authenticationResult.CreateAuthorizationHeader();
        }
        else
        // Scenario #3 - Using an API Gateway with Basic Authentication.  The API Gateway will handle other authentication and have different credentials, which may be NTLM
        {
            // authenticate using the SSG.
            string credentials = Convert.ToBase64String(
                ASCIIEncoding.ASCII.GetBytes(sharePointSsgUsername + ":" + sharePointSsgPassword)
            );
            Authorization = "Basic " + credentials;
        }

        // Authorization header is used for Cloud or Basic API Gateway access
        if (!string.IsNullOrEmpty(Authorization))
        {
            _Client.DefaultRequestHeaders.Add("Authorization", Authorization);
        }

        // Add a Digest header.  Needed for certain API operations
        Digest = GetDigest(_Client).GetAwaiter().GetResult();
        if (Digest != null)
        {
            _Client.DefaultRequestHeaders.Add("X-RequestDigest", Digest);
        }

        // Standard headers for API access
        _Client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        _Client.DefaultRequestHeaders.Add("OData-Version", "4.0");
    }

    public bool IsValid()
    {
        bool result = false;
        if (!string.IsNullOrEmpty(OdataUri))
        {
            result = true;
        }
        return result;
    }

    /// <summary>
    /// Escape the apostrophe character.  Since we use it to enclose the filename it must be escaped.
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
        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            return null;
        }

        folderName = SharePointUtils.FixFoldername(folderName, listTitle);

        string serverRelativeUrl = "";
        // ensure the webname has a slash.
        if (!string.IsNullOrEmpty(WebName))
        {
            serverRelativeUrl += $"{WebName}/";
        }

        serverRelativeUrl += Uri.EscapeUriString(listTitle);
        if (!string.IsNullOrEmpty(folderName))
        {
            serverRelativeUrl += "/" + Uri.EscapeUriString(folderName);
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
            Headers = { { "Accept", "application/json" } },
        };

        // make the request.
        var _httpResponse = await _Client.SendAsync(_httpRequest);
        HttpStatusCode _statusCode = _httpResponse.StatusCode;

        if ((int)_statusCode != 200)
        {
            var ex = new SharePointRestException(
                string.Format("Operation returned an invalid status code '{0}'", _statusCode)
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
        Console.WriteLine(
            $"OnPremSharePointFileManager - GetFileDetailsListInFolder - returning {fileDetailsList.Count} files from folder '{folderName}' in '{listTitle}'"
        );
        return fileDetailsList;
    }

    /// <summary>
    /// Create Folder
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task CreateFolder(string listTitle, string folderName)
    {
        Console.WriteLine(
            $"OnPremSharePointFileManager - CreateFolder - called with listTitle='{listTitle}', folderName='{folderName}'"
        );
        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            return;
        }

        folderName = SharePointUtils.FixFoldername(folderName, listTitle);

        string relativeUrl = SharePointUtils.EscapeApostrophe($"/{listTitle}/{folderName}");

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(ApiEndpoint + $"web/folders/add('{relativeUrl}')"),
            Headers = { { "Accept", "application/json" } },
        };

        //string jsonString = "{ '__metadata': { 'type': 'SP.Folder' }, 'ServerRelativeUrl': '" + relativeUrl + "'}";

        StringContent strContent = new StringContent("", Encoding.UTF8);
        strContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            "application/json;odata=verbose"
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
                string.Format("Operation returned an invalid status code '{0}'", _statusCode)
            );
            _responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Enhanced logging for debugging folder creation failures
            string originalRelativeUrl = $"/{listTitle}/{folderName}";
            Console.WriteLine(
                $"OnPremSharePointFileManager - CreateFolder - failed - Status: {_statusCode}, FolderName: '{folderName}', EscapedRelativeUrl: '{relativeUrl}', OriginalRelativeUrl: '{originalRelativeUrl}'"
            );
            Console.WriteLine(
                $"OnPremSharePointFileManager - CreateFolder - Response: {_responseContent}"
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
        else
        {
            string jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(
                $"OnPremSharePointFileManager - CreateFolder - successfully created folder '{folderName}' in '{listTitle}'"
            );
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
        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            return null;
        }

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
            "application/json;odata=verbose"
        );
        endpointRequest.Content = strContent;
        // fix for bad request
        endpointRequest.Headers.Add("odata-version", "3.0");

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);
        HttpStatusCode _statusCode = response.StatusCode;

        if (_statusCode != HttpStatusCode.Created)
        {
            string _responseContent = null;
            var ex = new SharePointRestException(
                string.Format("Operation returned an invalid status code '{0}'", _statusCode)
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
            var ob = Newtonsoft.Json.JsonConvert.DeserializeObject<DocumentLibraryResponse>(
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
                    "application/json;odata=verbose"
                );
                endpointRequest.Headers.Add("IF-MATCH", "*");
                endpointRequest.Headers.Add("X-HTTP-Method", "MERGE");
                endpointRequest.Content = strContent;
                response = await _Client.SendAsync(endpointRequest);
                jsonString = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }
            Console.WriteLine(
                $"OnPremSharePointFileManager - CreateDocumentLibrary - successfully created document library '{listTitle}' (template: '{documentTemplateUrlTitle}')"
            );
        }

        return library;
    }

    public async Task<Object> UpdateDocumentLibrary(string listTitle)
    {
        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            return null;
        }

        HttpRequestMessage endpointRequest = new HttpRequestMessage(
            HttpMethod.Put,
            $"{ApiEndpoint}web/Lists"
        );

        var library = CreateNewDocumentLibraryRequest(listTitle);

        string jsonString = JsonConvert.SerializeObject(library);
        StringContent strContent = new StringContent(jsonString, Encoding.UTF8);
        strContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            "application/json;odata=verbose"
        );
        endpointRequest.Content = strContent;

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);
        HttpStatusCode _statusCode = response.StatusCode;

        if (_statusCode != HttpStatusCode.Created)
        {
            string _responseContent = null;
            var ex = new SharePointRestException(
                string.Format("Operation returned an invalid status code '{0}'", _statusCode)
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
            Console.WriteLine(
                $"OnPremSharePointFileManager - UpdateDocumentLibrary - successfully updated document library '{listTitle}'"
            );
        }

        return library;
    }

    private object CreateNewDocumentLibraryRequest(string listName)
    {
        var type = new { type = "SP.List" };
        var request = new
        {
            __metadata = type,
            BaseTemplate = 101,
            Title = listName,
        };
        return request;
    }

    public async Task<bool> DeleteFolder(string listTitle, string folderName)
    {
        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            return false;
        }

        folderName = SharePointUtils.FixFoldername(folderName, listTitle);

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
            Headers = { { "Accept", "application/json" } },
        };

        // We want to delete this folder.
        endpointRequest.Headers.Add("IF-MATCH", "*");
        endpointRequest.Headers.Add("X-HTTP-Method", "DELETE");

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            result = true;
            Console.WriteLine(
                $"OnPremSharePointFileManager - DeleteFolder - successfully deleted folder '{folderName}' from '{listTitle}'"
            );
        }
        else
        {
            string _responseContent = null;
            var ex = new SharePointRestException(
                string.Format(
                    "Operation returned an invalid status code '{0}'",
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

    /// <summary>
    /// Check if a folder contains any files or subfolders
    /// </summary>
    /// <param name="listTitle">The document library title</param>
    /// <param name="folderName">The folder name to check</param>
    /// <returns>True if the folder contains items, false otherwise</returns>
    private async Task<bool> FolderHasItems(string listTitle, string folderName)
    {
        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            return false;
        }

        string serverRelativeUrl = "/";
        if (!string.IsNullOrEmpty(WebName))
        {
            serverRelativeUrl += $"{WebName}/";
        }

        serverRelativeUrl += $"{listTitle}/{folderName}";

        try
        {
            // Check for files
            HttpRequestMessage filesRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(
                    ApiEndpoint
                        + "web/getFolderByServerRelativeUrl('"
                        + EscapeApostrophe(serverRelativeUrl)
                        + "')/files?$top=1"
                ),
                Headers = { { "Accept", "application/json" } },
            };

            var filesResponse = await _Client.SendAsync(filesRequest);
            if (filesResponse.StatusCode == HttpStatusCode.OK)
            {
                string jsonString = await filesResponse.Content.ReadAsStringAsync();
                JObject responseObject = JObject.Parse(jsonString);
                var files = responseObject["value"].Children().ToList();
                if (files.Count > 0)
                {
                    return true;
                }
            }

            // Check for subfolders
            HttpRequestMessage foldersRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(
                    ApiEndpoint
                        + "web/getFolderByServerRelativeUrl('"
                        + EscapeApostrophe(serverRelativeUrl)
                        + "')/folders?$top=1"
                ),
                Headers = { { "Accept", "application/json" } },
            };

            var foldersResponse = await _Client.SendAsync(foldersRequest);
            if (foldersResponse.StatusCode == HttpStatusCode.OK)
            {
                string jsonString = await foldersResponse.Content.ReadAsStringAsync();
                JObject responseObject = JObject.Parse(jsonString);
                var folders = responseObject["value"].Children().ToList();
                if (folders.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
        catch
        {
            // If there's any error checking, assume no items
            return false;
        }
    }

    /// <summary>
    /// Search for a folder by GUID. If multiple folders contain the GUID, disambiguate using folderName
    /// (case-sensitive first, then case-insensitive, then by checking folder contents).
    /// Returns the actual folder name from SharePoint if found, otherwise null.
    /// </summary>
    /// <param name="urlTitle">The document library url title (ex: "adoxio_specialevent")</param>
    /// <param name="folderName">The expected folder name (for disambiguation when multiple GUID matches)</param>
    /// <param name="guid">The GUID to search for (required)</param>
    /// <returns>The actual folder name from SharePoint, or null if not found or cannot be disambiguated</returns>
    public async Task<string> EnhancedFolderExists(string urlTitle, string folderName, string guid)
    {
        Console.WriteLine(
            $"EnhancedFolderExists - called with urlTitle='{urlTitle}', folderName='{folderName}', guid='{guid}'"
        );

        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            Console.WriteLine("EnhancedFolderExists - SharePoint is not valid, returning null");
            return null;
        }

        // Normalize the GUID: remove dashes and convert to uppercase
        string normalizedGuid = guid?.Replace("-", "").ToUpper();
        Console.WriteLine($"EnhancedFolderExists - normalizedGuid='{normalizedGuid}'");

        if (string.IsNullOrEmpty(normalizedGuid))
        {
            Console.WriteLine(
                "EnhancedFolderExists - normalizedGuid is null or empty, returning null"
            );
            return null;
        }

        // Search for folders containing the normalized GUID
        var folders = await SearchFoldersInDocumentLibrary(
            urlTitle,
            searchString: null,
            searchGuid: normalizedGuid
        );
        Console.WriteLine(
            $"EnhancedFolderExists - SearchFoldersInDocumentLibrary returned {folders?.Count ?? 0} folders"
        );

        if (folders == null || folders.Count == 0)
        {
            // No folders found
            Console.WriteLine("EnhancedFolderExists - No folders found, returning null");
            return null;
        }
        else if (folders.Count == 1)
        {
            // Exactly one folder found - return its name
            Console.WriteLine(
                $"EnhancedFolderExists - Found exactly one folder: '{folders[0].Name}'"
            );
            return folders[0].Name;
        }
        else
        {
            // Multiple folders found - need to disambiguate using folderName
            Console.WriteLine(
                $"EnhancedFolderExists - Multiple folders found ({folders.Count}), disambiguating..."
            );

            // First try: case-sensitive comparison
            var caseSensitiveMatches = folders
                .Where(f => f.Name.Contains(folderName, StringComparison.Ordinal))
                .ToList();
            Console.WriteLine(
                $"EnhancedFolderExists - Case-sensitive matches: {caseSensitiveMatches.Count}"
            );

            if (caseSensitiveMatches.Count == 1)
            {
                // Found exactly one case-sensitive match
                Console.WriteLine(
                    $"EnhancedFolderExists - Found one case-sensitive match: '{caseSensitiveMatches[0].Name}'"
                );
                return caseSensitiveMatches[0].Name;
            }
            else if (caseSensitiveMatches.Count == 0)
            {
                // No case-sensitive matches - try case-insensitive comparison
                Console.WriteLine(
                    $"EnhancedFolderExists - No case-sensitive matches, trying case-insensitive..."
                );
                var caseInsensitiveMatches = folders
                    .Where(f => f.Name.Contains(folderName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                Console.WriteLine(
                    $"EnhancedFolderExists - Case-insensitive matches: {caseInsensitiveMatches.Count}"
                );

                if (caseInsensitiveMatches.Count == 1)
                {
                    // Found exactly one case-insensitive match
                    Console.WriteLine(
                        $"EnhancedFolderExists - Found one case-insensitive match: '{caseInsensitiveMatches[0].Name}'"
                    );
                    return caseInsensitiveMatches[0].Name;
                }
                else if (caseInsensitiveMatches.Count == 0)
                {
                    // No matches at all
                    Console.WriteLine(
                        "EnhancedFolderExists - No case-insensitive matches, returning null"
                    );
                    return null;
                }
                else
                {
                    // Multiple case-insensitive matches - check which folder has items
                    Console.WriteLine(
                        $"EnhancedFolderExists - Multiple case-insensitive matches ({caseInsensitiveMatches.Count}), checking for folder contents..."
                    );

                    var foldersWithItems = new List<FolderItem>();
                    foreach (var folder in caseInsensitiveMatches)
                    {
                        bool hasItems = await FolderHasItems(urlTitle, folder.Name);
                        Console.WriteLine(
                            $"EnhancedFolderExists - Folder '{folder.Name}' hasItems: {hasItems}"
                        );
                        if (hasItems)
                        {
                            foldersWithItems.Add(folder);
                        }
                    }

                    if (foldersWithItems.Count == 1)
                    {
                        // Exactly one folder has items - return it
                        Console.WriteLine(
                            $"EnhancedFolderExists - Found exactly one folder with items: '{foldersWithItems[0].Name}'"
                        );
                        return foldersWithItems[0].Name;
                    }
                    else if (foldersWithItems.Count == 0)
                    {
                        // No folders have items - cannot determine which one to use
                        Console.WriteLine(
                            "EnhancedFolderExists - No folders contain items, returning null"
                        );
                        return null;
                    }
                    else
                    {
                        // Multiple folders have items - cannot determine which one to use
                        Console.WriteLine(
                            $"EnhancedFolderExists - Multiple folders ({foldersWithItems.Count}) contain items, returning null"
                        );
                        return null;
                    }
                }
            }
            else
            {
                // Multiple case-sensitive matches (shouldn't be possible in SharePoint, but handle gracefully)
                Console.WriteLine(
                    $"EnhancedFolderExists - Multiple case-sensitive matches ({caseSensitiveMatches.Count}) - unexpected, returning first: '{caseSensitiveMatches[0].Name}'"
                );
                return caseSensitiveMatches[0].Name;
            }
        }
    }

    public async Task<bool> DocumentLibraryExists(string listTitle)
    {
        Object lisbrary = await GetDocumentLibrary(listTitle);

        return (lisbrary != null);
    }

    /// <summary>
    /// Attempt to fetch an existing folder by name.
    /// If a GUID is provided, use it to enhance the search by matching on GUID (more reliable).
    /// If multiple folders match the GUID, disambiguate using the folderName (case-sensitive first, then
    /// case-insensitive, then by checking folder contents).
    /// </summary>
    /// <param name="urlTitle">The internal url name (ex: "adoxio_specialevent")</param>
    /// <param name="folderName"></param>
    /// <param name="guid"></param>
    /// <returns></returns>
    public async Task<Object> GetFolder(string urlTitle, string folderName, string guid = null)
    {
        Console.WriteLine(
            $"GetFolder - called with urlTitle='{urlTitle}', folderName='{folderName}', guid='{guid}'"
        );

        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            Console.WriteLine("GetFolder - SharePoint is not valid, returning null");
            return null;
        }
        var fixedFolderName = SharePointUtils.FixFoldername(folderName, urlTitle);
        Console.WriteLine($"GetFolder - After FixFoldername: '{fixedFolderName}'");

        // If GUID is provided, use EnhancedFolderExists to find the folder by GUID and resolve the actual folder name
        if (!string.IsNullOrEmpty(guid))
        {
            Console.WriteLine(
                $"GetFolder - GUID provided, calling FindFolderOne to search by GUID"
            );

            List<FolderItem> matchingFolders = await FindFolderOne(urlTitle, guid);

            if (matchingFolders != null && matchingFolders.Count > 0)
            {
                // Found the folder with enhanced matching - use the actual name from SharePoint
                Console.WriteLine(
                    $"GetFolder - FindFolderOne returned actual folder name: '{matchingFolders[0].Name}' (was '{fixedFolderName}')"
                );
                fixedFolderName = matchingFolders[0].Name;
            }
        }

        Object result = null;
        string serverRelativeUrl = "/";
        if (!string.IsNullOrEmpty(WebName))
        {
            serverRelativeUrl += $"{WebName}/";
        }

        serverRelativeUrl += $"{urlTitle}/{fixedFolderName}";
        Console.WriteLine($"GetFolder - Constructed serverRelativeUrl: '{serverRelativeUrl}'");

        HttpRequestMessage endpointRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(
                ApiEndpoint
                    + "web/getFolderByServerRelativeUrl('"
                    + EscapeApostrophe(serverRelativeUrl)
                    + "')?$select=Name,ServerRelativeUrl,UniqueId,ItemCount"
            ),
            Headers = { { "Accept", "application/json" } },
        };

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);
        string jsonString = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"GetFolder - Response StatusCode: {response.StatusCode}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"GetFolder - Folder found successfully");
            result = JsonConvert.DeserializeObject(jsonString);
            Console.WriteLine(
                $"GetFolder - completed successfully for folder '{fixedFolderName}' in '{urlTitle}'"
            );
        }
        else
        {
            Console.WriteLine($"GetFolder - Folder not found or error occurred");
        }

        return result;
    }

    public async Task<Object> GetDocumentLibrary(string listTitle)
    {
        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            return null;
        }

        Object result = null;
        string title = Uri.EscapeUriString(listTitle);
        string query = $"web/lists/GetByTitle('{title}')";

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(ApiEndpoint + query),
            Headers = { { "Accept", "application/json" } },
        };

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);
        string jsonString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            result = JsonConvert.DeserializeObject(jsonString);
            Console.WriteLine(
                $"OnPremSharePointFileManager - GetDocumentLibrary - successfully retrieved document library '{listTitle}'"
            );
        }

        return result;
    }

    public async Task<List<FolderItem>> GetFoldersInDocumentLibrary(string listTitle)
    {
        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            return null;
        }

        List<FolderItem> folderList = new List<FolderItem>();
        string title = Uri.EscapeUriString(listTitle);
        // Get folders from the rootFolder to exclude system folders
        string query = $"web/lists/GetByTitle('{title}')/rootFolder/folders";

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(ApiEndpoint + query),
            Headers = { { "Accept", "application/json" } },
        };

        // make the request.
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

                    // Filter out system folders (Forms, etc.)
                    if (!folderItem.Name.Equals("Forms", StringComparison.OrdinalIgnoreCase))
                    {
                        folderList.Add(folderItem);
                    }
                }
            }
            catch (JsonReaderException jre)
            {
                throw jre;
            }
        }

        Console.WriteLine(
            $"OnPremSharePointFileManager - GetFoldersInDocumentLibrary - returning {folderList.Count} folders from '{listTitle}'"
        );
        return folderList;
    }

    /// <summary>
    /// Search for folders in a document library by name (server-side filtering for performance)
    /// </summary>
    /// <param name="listTitle">The document library title</param>
    /// <param name="searchString">The string to search for in folder names (case-sensitive)</param>
    /// <param name="searchGuid">Optional GUID to search for (will be normalized: dashes removed, uppercase)</param>
    /// <returns>List of matching folders</returns>
    public async Task<List<FolderItem>> SearchFoldersInDocumentLibrary(
        string listTitle,
        string searchString = null,
        string searchGuid = null
    )
    {
        string internalTitle = SharePointConstants.GetDocumentTemplateUrlPart(listTitle);

        Console.WriteLine(
            $"SearchFoldersInDocumentLibrary - called with listTitle='{listTitle}', internalTitle='{internalTitle}' searchString='{searchString}', searchGuid='{searchGuid}'"
        );

        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            Console.WriteLine(
                "SearchFoldersInDocumentLibrary - SharePoint is not valid, returning null"
            );
            return null;
        }

        // Normalize searchGuid if provided: remove dashes and convert to uppercase
        if (!string.IsNullOrEmpty(searchGuid))
        {
            searchString = searchGuid.Replace("-", "").ToUpper();
            Console.WriteLine(
                $"SearchFoldersInDocumentLibrary - Normalized searchGuid to searchString: '{searchString}'"
            );
        }

        if (string.IsNullOrEmpty(searchString))
        {
            // Return empty list if no search criteria provided
            Console.WriteLine(
                "SearchFoldersInDocumentLibrary - searchString is null or empty, returning empty list"
            );
            return new List<FolderItem>();
        }

        List<FolderItem> folderList = new List<FolderItem>();
        string title = Uri.EscapeUriString(internalTitle);

        // Use OData filter to search on server side for better performance with large folder counts
        // substringof is used for SharePoint compatibility (older OData syntax)
        // Case-sensitive search by default (GUIDs are uppercase in folder names)
        string escapedSearch = searchString.Replace("'", "''"); // Escape single quotes for OData
        string filter = $"$filter=substringof('{escapedSearch}',Name) and Name ne 'Forms'";
        // string query = $"web/lists/GetByTitle('{title}')/rootFolder/folders?{filter}";
        string query = $"web/GetList('/{title}')/rootFolder/folders?{filter}";
        Console.WriteLine($"SearchFoldersInDocumentLibrary - Constructed query: '{query}'");

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(ApiEndpoint + query),
            Headers = { { "Accept", "application/json" } },
        };

        // make the request.
        Console.WriteLine(
            $"SearchFoldersInDocumentLibrary - Making API request to: {endpointRequest.RequestUri}"
        );
        var response = await _Client.SendAsync(endpointRequest);
        string jsonString = await response.Content.ReadAsStringAsync();
        Console.WriteLine(
            $"SearchFoldersInDocumentLibrary - Response StatusCode: {response.StatusCode}"
        );

        if (response.StatusCode == HttpStatusCode.OK)
        {
            try
            {
                JObject responseObject = JObject.Parse(jsonString);
                List<JToken> responseResults = responseObject["value"].Children().ToList();
                Console.WriteLine(
                    $"SearchFoldersInDocumentLibrary - Found {responseResults.Count} folders matching search criteria"
                );

                foreach (JToken responseResult in responseResults)
                {
                    FolderItem folderItem = responseResult.ToObject<FolderItem>();
                    Console.WriteLine(
                        $"SearchFoldersInDocumentLibrary - Adding folder: '{folderItem.Name}'"
                    );
                    folderList.Add(folderItem);
                }
            }
            catch (JsonReaderException jre)
            {
                Console.WriteLine(
                    $"SearchFoldersInDocumentLibrary - JSON parsing error: {jre.Message}"
                );
                throw jre;
            }
        }
        else
        {
            Console.WriteLine($"SearchFoldersInDocumentLibrary - Error response: {jsonString}");
            throw new SharePointRestException(
                string.Format(
                    "Operation returned an invalid status code '{0}'",
                    response.StatusCode
                )
            );
        }

        Console.WriteLine($"SearchFoldersInDocumentLibrary - Returning {folderList.Count} folders");
        return folderList;
    }

    public async Task<string> AddFile(
        String folderName,
        String fileName,
        Stream fileData,
        string contentType
    )
    {
        return await this.AddFile(
            SharePointConstants.AccountFolderDisplayName,
            folderName,
            fileName,
            fileData,
            contentType
        );
    }

    public async Task<string> AddFile(
        String listTitle,
        String folderName,
        String fileName,
        Stream fileData,
        string contentType
    )
    {
        folderName = SharePointUtils.FixFoldername(folderName, listTitle);
        bool folderExists = await this.FolderExists(listTitle, folderName);
        if (!folderExists)
        {
            await this.CreateFolder(listTitle, folderName);
        }

        // now add the file to the folder.

        fileName = await this.UploadFile(fileName, listTitle, folderName, fileData, contentType);
        Console.WriteLine(
            $"OnPremSharePointFileManager - AddFile - successfully added file '{fileName}' to folder '{folderName}' in '{listTitle}'"
        );

        return fileName;
    }

    public async Task<string> AddFile(
        String folderName,
        String fileName,
        byte[] fileData,
        string contentType
    )
    {
        return await this.AddFile(
            SharePointConstants.AccountFolderDisplayName,
            folderName,
            fileName,
            fileData,
            contentType
        );
    }

    public async Task<string> AddFile(
        String listTitle,
        String folderName,
        String fileName,
        byte[] fileData,
        string contentType
    )
    {
        folderName = SharePointUtils.FixFoldername(folderName, listTitle);
        bool folderExists = await this.FolderExists(listTitle, folderName);
        if (!folderExists)
        {
            await this.CreateFolder(listTitle, folderName);
        }

        // now add the file to the folder.

        fileName = await this.UploadFile(fileName, listTitle, folderName, fileData, contentType);
        Console.WriteLine(
            $"OnPremSharePointFileManager - AddFile - successfully added file '{fileName}' to folder '{folderName}' in '{listTitle}'"
        );

        return fileName;
    }

    public string GetServerRelativeURL(string listTitle, string folderName)
    {
        folderName = SharePointUtils.FixFoldername(folderName, listTitle);
        string serverRelativeUrl = "";
        if (!string.IsNullOrEmpty(WebName))
        {
            serverRelativeUrl += $"{WebName}/";
        }

        serverRelativeUrl += Uri.EscapeUriString(listTitle) + "/" + Uri.EscapeUriString(folderName);

        return serverRelativeUrl;
    }

    private string GenerateUploadRequestUriString(string folderServerRelativeUrl, string fileName)
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
        string result = null;
        if (IsValid())
        {
            // convert the stream into a byte array.
            MemoryStream ms = new MemoryStream();
            fileData.CopyTo(ms);
            byte[] data = ms.ToArray();
            return await UploadFile(fileName, listTitle, folderName, data, contentType);
        }
        return result;
    }

    /// <summary>
    /// SharePoint is very particular about the file name length and the total characters in the URL to access a file.
    /// This method returns the input file name or a truncated version of the file name if it is over the max number of characters.
    /// </summary>
    /// <param name="fileName">The file name to check; e.g. "abcdefg1111222233334444.pdf"</param>
    /// <param name="listTitle">The list title</param>
    /// <param name="folderName">The folder name where the file would be uploaded</param>
    /// <returns>The (potentially truncated) file name; e.g. "abcd.pdf"</returns>
    public string GetTruncatedFileName(string fileName, string listTitle, string folderName)
    {
        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            return fileName;
        }

        // SharePoint requires that filenames are less than 128 characters.
        int maxLength = MAX_SEGMENT_LENGTH;
        fileName = SharePointUtils.FixFilename(fileName, maxLength);

        folderName = SharePointUtils.FixFoldername(folderName, listTitle);

        // SharePoint also imposes a limit on the whole URL
        string serverRelativeUrl = GetServerRelativeURL(listTitle, folderName);
        string requestUriString = GenerateUploadRequestUriString(serverRelativeUrl, fileName);
        if (requestUriString.Length > MAX_TOTAL_LENGTH)
        {
            int delta = requestUriString.Length - MAX_TOTAL_LENGTH;
            maxLength -= delta;

            // Ensure maxLength doesn't become too small (minimum 10 characters to allow for some filename + extension)
            if (maxLength < 10)
            {
                maxLength = 10;
            }

            fileName = SharePointUtils.FixFilename(fileName, maxLength);
        }

        return fileName;
    }

    /// <summary>
    /// Extract the UniqueId (GUID) from a folder object returned by SharePoint API
    /// </summary>
    private string GetFolderUniqueId(object folderObject)
    {
        try
        {
            var folderJson = folderObject as JObject;
            if (folderJson != null && folderJson["UniqueId"] != null)
            {
                return folderJson["UniqueId"].ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetFolderUniqueId - Error extracting UniqueId: {ex.Message}");
        }
        return null;
    }

    /// <summary>
    /// Upload a file using folder ID instead of path (avoids URL length issues)
    /// </summary>
    private async Task<string> UploadFileByFolderId(
        string fileName,
        string folderId,
        byte[] data,
        string contentType
    )
    {
        string result = null;

        string requestUriString =
            ApiEndpoint
            + "web/GetFolderById('"
            + folderId
            + "')/Files/add(url='"
            + EscapeApostrophe(fileName)
            + "',overwrite=true)";

        Console.WriteLine(
            $"UploadFileByFolderId - Using folder ID '{folderId}', URL length: {requestUriString.Length}"
        );

        HttpRequestMessage endpointRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestUriString),
            Headers = { { "Accept", "application/json" } },
        };

        ByteArrayContent byteArrayContent = new ByteArrayContent(data);
        byteArrayContent.Headers.Add(@"content-length", data.Length.ToString());
        endpointRequest.Content = byteArrayContent;

        var response = await _Client.SendAsync(endpointRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            result = fileName;
            Console.WriteLine(
                $"OnPremSharePointFileManager - UploadFileByFolderId - successfully uploaded file '{fileName}' using folder ID"
            );
        }
        else
        {
            string _responseContent = null;
            var ex = new SharePointRestException(
                string.Format(
                    "Operation returned an invalid status code '{0}'",
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
        string result = null;
        if (IsValid())
        {
            folderName = SharePointUtils.FixFoldername(folderName, listTitle);
            fileName = GetTruncatedFileName(fileName, listTitle, folderName);

            string serverRelativeUrl = GetServerRelativeURL(listTitle, folderName);
            string requestUriString = GenerateUploadRequestUriString(serverRelativeUrl, fileName);

            // If URL is too long, try using folder ID instead
            if (requestUriString.Length > MAX_TOTAL_LENGTH)
            {
                Console.WriteLine(
                    $"UploadFile - URL too long ({requestUriString.Length} chars), attempting upload by folder ID"
                );

                try
                {
                    // Get the folder object which contains the UniqueId
                    var folder = await GetFolder(listTitle, folderName);
                    if (folder != null)
                    {
                        string folderId = GetFolderUniqueId(folder);
                        if (!string.IsNullOrEmpty(folderId))
                        {
                            Console.WriteLine(
                                $"UploadFile - Found folder ID: {folderId}, using ID-based upload"
                            );
                            return await UploadFileByFolderId(
                                fileName,
                                folderId,
                                data,
                                contentType
                            );
                        }
                        else
                        {
                            Console.WriteLine(
                                "UploadFile - Could not extract folder ID, falling back to path-based upload"
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"UploadFile - Error getting folder ID: {ex.Message}, falling back to path-based upload"
                    );
                }
            }

            // Standard path-based upload
            HttpRequestMessage endpointRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUriString),
                Headers = { { "Accept", "application/json" } },
            };

            ByteArrayContent byteArrayContent = new ByteArrayContent(data);
            byteArrayContent.Headers.Add(@"content-length", data.Length.ToString());
            endpointRequest.Content = byteArrayContent;

            // make the request.
            var response = await _Client.SendAsync(endpointRequest);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = fileName;
                Console.WriteLine(
                    $"OnPremSharePointFileManager - UploadFile - successfully uploaded file '{fileName}' to '{listTitle}/{folderName}'"
                );
            }
            else
            {
                string _responseContent = null;
                var ex = new SharePointRestException(
                    string.Format(
                        "Operation returned an invalid status code '{0}'",
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
        }
        return result;
    }

    /// <summary>
    /// Download a file
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<byte[]> DownloadFile(string url)
    {
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
        HttpStatusCode statusCode = response.StatusCode;

        if (statusCode != HttpStatusCode.OK)
        {
            string responseContent = await response
                .Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var ex = new SharePointRestException(
                string.Format("Operation returned an invalid status code '{0}'", statusCode)
            );
            ex.Request = new HttpRequestMessageWrapper(endpointRequest, null);
            ex.Response = new HttpResponseMessageWrapper(response, responseContent);

            endpointRequest.Dispose();
            if (response != null)
            {
                response.Dispose();
            }
            throw ex;
        }

        using (MemoryStream ms = new MemoryStream())
        {
            await response.Content.CopyToAsync(ms);
            result = ms.ToArray();
        }

        Console.WriteLine(
            $"OnPremSharePointFileManager - DownloadFile - successfully downloaded file from '{url}', size: {result.Length} bytes"
        );
        return result;
    }

    public async Task<string> GetDigest(HttpClient client)
    {
        // return early if SharePoint is disabled.
        if (!IsValid())
        {
            return null;
        }

        string result = null;

        HttpRequestMessage endpointRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(ApiEndpoint + "contextinfo"),
            Headers = { { "Accept", "application/json;odata=verbose" } },
        };

        // make the request.
        var response = await client.SendAsync(endpointRequest);
        string jsonString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK && jsonString.Length > 1)
        {
            if (jsonString[0] == '{')
            {
                JToken t = JToken.Parse(jsonString);
                result = t["d"]["GetContextWebInformation"]["FormDigestValue"].ToString();
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(jsonString);
                var digests = doc.GetElementsByTagName("d:FormDigestValue");
                if (digests.Count > 0)
                {
                    result = digests[0].InnerText;
                }
            }
        }

        if (!string.IsNullOrEmpty(result))
        {
            Console.WriteLine(
                $"OnPremSharePointFileManager - GetDigest - successfully retrieved digest token"
            );
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

        folderName = SharePointUtils.FixFoldername(folderName, listTitle);

        serverRelativeUrl += $"/{listTitle}/{folderName}/{fileName}";

        result = await DeleteFile(serverRelativeUrl);

        return result;
    }

    public async Task<bool> DeleteFile(string serverRelativeUrl)
    {
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
            Headers = { { "Accept", "application/json" } },
        };

        // We want to delete this file.
        endpointRequest.Headers.Add("IF-MATCH", "*");
        endpointRequest.Headers.Add("X-HTTP-Method", "DELETE");

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            result = true;
            Console.WriteLine(
                $"OnPremSharePointFileManager - DeleteFile - successfully deleted file at '{serverRelativeUrl}'"
            );
        }
        else
        {
            string _responseContent = null;
            var ex = new SharePointRestException(
                string.Format(
                    "Operation returned an invalid status code '{0}'",
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
        bool result = false;
        string url =
            $"{ApiEndpoint}web/GetFileByServerRelativeUrl('{EscapeApostrophe(oldServerRelativeUrl)}')/moveto(newurl='{EscapeApostrophe(newServerRelativeUrl)}', flags=1)";
        HttpRequestMessage endpointRequest = new HttpRequestMessage(HttpMethod.Post, url);

        // make the request.
        var response = await _Client.SendAsync(endpointRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            result = true;
            Console.WriteLine(
                $"OnPremSharePointFileManager - RenameFile - successfully renamed file from '{oldServerRelativeUrl}' to '{newServerRelativeUrl}'"
            );
        }
        else
        {
            string _responseContent = null;
            var ex = new SharePointRestException(
                string.Format(
                    "Operation returned an invalid status code '{0}'",
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
}
