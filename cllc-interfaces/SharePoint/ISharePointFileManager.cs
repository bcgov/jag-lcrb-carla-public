using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    /// <summary>
    /// Minimal interface for SharePoint file management operations currently used in the application.
    /// Supports both on-premises and cloud SharePointManagers.
    /// </summary>
    public interface ISharePointFileManager
    {
        string WebName { get; }

        Task<bool> FolderExists(string listTitle, string folderName);

        Task<bool> DocumentLibraryExists(string listTitle);

        Task<Object> CreateDocumentLibrary(
            string listTitle,
            string documentTemplateUrlTitle = null
        );

        Task CreateFolder(string listTitle, string folderName);

        Task<string> UploadFile(
            string fileName,
            string listTitle,
            string folderName,
            byte[] data,
            string contentType
        );
        Task<string> UploadFile(
            string fileName,
            string listTitle,
            string folderName,
            Stream fileData,
            string contentType
        );

        Task<string> AddFile(
            string folderName,
            string fileName,
            byte[] fileData,
            string contentType
        );

        Task<string> AddFile(
            string documentLibrary,
            string folderName,
            string fileName,
            byte[] fileData,
            string contentType
        );

        Task<string> AddFile(
            string folderName,
            string fileName,
            Stream fileData,
            string contentType
        );

        Task<string> AddFile(
            string documentLibrary,
            string folderName,
            string fileName,
            Stream fileData,
            string contentType
        );

        Task<byte[]> DownloadFile(string url);

        Task<bool> DeleteFile(string serverRelativeUrl);

        Task<bool> DeleteFile(string listTitle, string folderName, string fileName);

        Task<List<SharePointFileDetailsList>> GetFileDetailsListInFolder(
            string listTitle,
            string folderName,
            string documentType
        );

        string GetServerRelativeURL(string listTitle, string folderName);

        string GetTruncatedFileName(string fileName, string listTitle, string folderName);

        // ================================================================================
        // New methods added for enhanced folder searching and creation capabilities,
        // as well as file uploading with server relative URL.
        // These were added to support the temporary period when Cloud Dynamics had to
        // interface with On-Prem SharePoint, by routing through the File Manager Service.
        // ================================================================================

        Task<List<FolderItem>> FindFolderOne(string entityName, string folderGuidSegment);

        Task<List<FolderItem>> FindFolderTwo(string parentRelativePath, string rawFolderName);

        Task<List<FolderItem>> FindFolderThree(
            string parentRelativePath,
            string rawFolderNameSegment,
            string rawFolderGuidSegment
        );

        Task<FolderItem> CreateFolder2(string relativeUrl);

        Task<string> UploadFile2(
            string serverRelativeUrl,
            string fileName,
            byte[] fileData,
            string contentType
        );

        /// <summary>
        /// Get all folders in a document library that were modified after a specific date
        /// </summary>
        /// <param name="listTitle">The document library title</param>
        /// <param name="afterDate">Only return folders modified after this date</param>
        /// <returns>List of folder items</returns>
        Task<List<FolderItem>> GetFoldersInDocumentLibraryAfterDate(
            string listTitle,
            DateTime afterDate
        );

        /// <summary>
        /// Get all child folders in a folder by its server relative URL
        /// </summary>
        /// <param name="serverRelativeUrl">The server relative URL of the parent folder</param>
        /// <returns>List of child folders</returns>
        Task<List<FolderItem>> GetChildFolders(string serverRelativeUrl);
    }
}
