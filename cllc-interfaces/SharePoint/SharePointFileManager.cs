using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Interfaces;

/// <summary>
/// Factory class that creates either a REST or Graph API SharePoint client based on configuration.
/// Allows gradual migration from REST to Graph API without breaking existing code.
/// Set SHAREPOINT_USE_GRAPH_API=true in configuration to use the modern Graph API.
/// </summary>
public class SharePointFileManager : ISharePointFileManager
{
    private readonly ISharePointFileManager _implementation;

    public SharePointFileManager(IConfiguration configuration)
    {
        // Check if Graph API should be used (defaults to false for backward compatibility)
        string useGraphApi = configuration["SHAREPOINT_USE_GRAPH_API"];

        bool shouldUseGraph =
            !string.IsNullOrEmpty(useGraphApi)
            && (
                useGraphApi.Equals("true", StringComparison.OrdinalIgnoreCase)
                || useGraphApi.Equals("1")
            );

        if (shouldUseGraph)
        {
            _implementation = new SharePointGraphManager(configuration);
        }
        else
        {
            _implementation = new SharePointRestFileManager(configuration);
        }
    }

    // Delegate all interface methods to the chosen implementation

    public Task<bool> CheckHealthAsync() => _implementation.CheckHealthAsync();

    public Task<List<SharePointFileDetailsList>> GetFileDetailsListInFolder(
        string listTitle,
        string folderName,
        string documentType
    ) => _implementation.GetFileDetailsListInFolder(listTitle, folderName, documentType);

    public Task CreateFolder(string listTitle, string folderName) =>
        _implementation.CreateFolder(listTitle, folderName);

    public Task<object> CreateDocumentLibrary(
        string listTitle,
        string documentTemplateUrlTitle = null
    ) => _implementation.CreateDocumentLibrary(listTitle, documentTemplateUrlTitle);

    public Task<object> UpdateDocumentLibrary(string listTitle) =>
        _implementation.UpdateDocumentLibrary(listTitle);

    public Task<bool> DeleteFolder(string listTitle, string folderName) =>
        _implementation.DeleteFolder(listTitle, folderName);

    public Task<bool> FolderExists(string listTitle, string folderName) =>
        _implementation.FolderExists(listTitle, folderName);

    public Task<bool> DocumentLibraryExists(string listTitle) =>
        _implementation.DocumentLibraryExists(listTitle);

    public Task<object> GetFolder(string listTitle, string folderName) =>
        _implementation.GetFolder(listTitle, folderName);

    public Task<object> GetDocumentLibrary(string listTitle) =>
        _implementation.GetDocumentLibrary(listTitle);

    public Task<string> AddFile(
        string folderName,
        string fileName,
        Stream fileData,
        string contentType
    ) => _implementation.AddFile(folderName, fileName, fileData, contentType);

    public Task<string> AddFile(
        string documentLibrary,
        string folderName,
        string fileName,
        Stream fileData,
        string contentType
    ) => _implementation.AddFile(documentLibrary, folderName, fileName, fileData, contentType);

    public Task<string> AddFile(
        string folderName,
        string fileName,
        byte[] fileData,
        string contentType
    ) => _implementation.AddFile(folderName, fileName, fileData, contentType);

    public Task<string> AddFile(
        string documentLibrary,
        string folderName,
        string fileName,
        byte[] fileData,
        string contentType
    ) => _implementation.AddFile(documentLibrary, folderName, fileName, fileData, contentType);

    public Task<string> UploadFile(
        string fileName,
        string listTitle,
        string folderName,
        Stream fileData,
        string contentType
    ) => _implementation.UploadFile(fileName, listTitle, folderName, fileData, contentType);

    public Task<string> UploadFile(
        string fileName,
        string listTitle,
        string folderName,
        byte[] data,
        string contentType
    ) => _implementation.UploadFile(fileName, listTitle, folderName, data, contentType);

    public Task<byte[]> DownloadFile(string url) => _implementation.DownloadFile(url);

    public Task<bool> DeleteFile(string listTitle, string folderName, string fileName) =>
        _implementation.DeleteFile(listTitle, folderName, fileName);

    public Task<bool> DeleteFile(string serverRelativeUrl) =>
        _implementation.DeleteFile(serverRelativeUrl);

    public Task<bool> RenameFile(string oldServerRelativeUrl, string newServerRelativeUrl) =>
        _implementation.RenameFile(oldServerRelativeUrl, newServerRelativeUrl);

    public string RemoveInvalidCharacters(string filename) =>
        _implementation.RemoveInvalidCharacters(filename);

    public string FixFoldername(string foldername) => _implementation.FixFoldername(foldername);

    public string FixFilename(string filename, int maxLength = 128) =>
        _implementation.FixFilename(filename, maxLength);

    public string GetTruncatedFileName(string fileName, string listTitle, string folderName) =>
        _implementation.GetTruncatedFileName(fileName, listTitle, folderName);

    public string EscapeApostrophe(string filename) => _implementation.EscapeApostrophe(filename);

    public string GetServerRelativeURL(string listTitle, string folderName) =>
        _implementation.GetServerRelativeURL(listTitle, folderName);

    public string GenerateUploadRequestUriString(string folderServerRelativeUrl, string fileName) =>
        _implementation.GenerateUploadRequestUriString(folderServerRelativeUrl, fileName);
}
