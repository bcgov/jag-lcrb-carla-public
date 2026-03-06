using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces;

/// <summary>
/// This class is only for backwards compatibility with existing legacy code so that the ISharePointFileManager
/// interface can be used without modification.
/// </summary>
public partial class CloudSharePointFileManager : ISharePointFileManager
{
    public async Task<List<FolderItem>> GetFoldersInDocumentLibraryAfterDate(
        string listTitle,
        DateTime afterDate
    )
    {
        throw new NotImplementedException();
    }

    public async Task<List<FolderItem>> FindFolderOne(string entityName, string folderGuidSegment)
    {
        throw new NotImplementedException();
    }

    public async Task<List<FolderItem>> FindFolderTwo(
        string parentRelativePath,
        string rawFolderName
    )
    {
        throw new NotImplementedException();
    }

    public async Task<List<FolderItem>> FindFolderThree(
        string parentRelativePath,
        string rawFolderNameSegment,
        string rawFolderGuidSegment
    )
    {
        throw new NotImplementedException();
    }

    public async Task<FolderItem> CreateFolder2(string relativeUrl)
    {
        throw new NotImplementedException();
    }

    public async Task<string> UploadFile2(
        string serverRelativeUrl,
        string fileName,
        byte[] fileData,
        string contentType
    )
    {
        throw new NotImplementedException();
    }
}
