using System;

namespace Gov.Lclb.Cllb.Interfaces;

public class SharePointDocumentLibraryResponse
{
    public SharePointDocumentLibraryResponseContent d { get; set; }
}

public class SharePointDocumentLibraryResponseContent
{
    public string Id { get; set; }
}

public class SharePointFileSystemItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Documenttype { get; set; }
    public int Size { get; set; }
    public string Serverrelativeurl { get; set; }
    public DateTime Timecreated { get; set; }
    public DateTime Timelastmodified { get; set; }
}

public class SharePointFileDetailsList
{
    public string Name { get; set; }
    public string TimeLastModified { get; set; }
    public string TimeCreated { get; set; }
    public string Length { get; set; }
    public string DocumentType { get; set; }
    public string ServerRelativeUrl { get; set; }
}
