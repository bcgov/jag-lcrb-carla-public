using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces;

/// <summary>
/// Interface defining SharePoint file management operations.
/// Implemented by both REST and Graph API clients.
/// </summary>
public interface ISharePointFileManager
{
    Task<bool> CheckHealthAsync();
    Task<List<SharePointFileDetailsList>> GetFileDetailsListInFolder(
        string listTitle,
        string folderName,
        string documentType
    );
    Task CreateFolder(string listTitle, string folderName);
    Task<object> CreateDocumentLibrary(string listTitle, string documentTemplateUrlTitle = null);
    Task<object> UpdateDocumentLibrary(string listTitle);
    Task<bool> DeleteFolder(string listTitle, string folderName);
    Task<bool> FolderExists(string listTitle, string folderName);
    Task<bool> DocumentLibraryExists(string listTitle);
    Task<object> GetFolder(string listTitle, string folderName);
    Task<object> GetDocumentLibrary(string listTitle);
    Task<string> AddFile(string folderName, string fileName, Stream fileData, string contentType);
    Task<string> AddFile(
        string documentLibrary,
        string folderName,
        string fileName,
        Stream fileData,
        string contentType
    );
    Task<string> AddFile(string folderName, string fileName, byte[] fileData, string contentType);
    Task<string> AddFile(
        string documentLibrary,
        string folderName,
        string fileName,
        byte[] fileData,
        string contentType
    );
    Task<string> UploadFile(
        string fileName,
        string listTitle,
        string folderName,
        Stream fileData,
        string contentType
    );
    Task<string> UploadFile(
        string fileName,
        string listTitle,
        string folderName,
        byte[] data,
        string contentType
    );
    Task<byte[]> DownloadFile(string url);
    Task<bool> DeleteFile(string listTitle, string folderName, string fileName);
    Task<bool> DeleteFile(string serverRelativeUrl);
    Task<bool> RenameFile(string oldServerRelativeUrl, string newServerRelativeUrl);
    string RemoveInvalidCharacters(string filename);
    string FixFoldername(string foldername);
    string FixFilename(string filename, int maxLength = 128);
    string GetTruncatedFileName(string fileName, string listTitle, string folderName);
    string EscapeApostrophe(string filename);
    string GetServerRelativeURL(string listTitle, string folderName);
    string GenerateUploadRequestUriString(string folderServerRelativeUrl, string fileName);
}
