namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class FileSystemItemExtensions
    {
        const string NameDocumentTypeSeparator = "__";

        public static string GetDocumentName (string value)
        {
            string result = "";
            if (value != null)
            {
                int pos = value.IndexOf(NameDocumentTypeSeparator);
                if (pos > -1)
                {  
                    result = value.Substring(pos + 1);
                }                
            }
            return result;
        }

        public static string GetDocumentType (string value)
        {
            string result = "";
            if (value != null)
            {
                int pos = value.IndexOf(NameDocumentTypeSeparator);
                if (pos > -1)
                {
                    result = value.Substring(0, pos);
                }                
            }
            return result;
        }

        public static string CombineNameDocumentType (string name, string documentType)
        {
            // 2018-7-13:  GW changed order of document type and name to fix problems downloading files.
            string result = documentType + NameDocumentTypeSeparator + name;
            return result;
        }

        /// <summary>
        /// Copy values from a Dynamics legal entity to a view model.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this MS.FileServices.FileSystemItem to, ViewModels.FileSystemItem from)
        {
            to.Name = CombineNameDocumentType (from.name, from.documenttype);
            to.Size = from.size;
            to.TimeCreated = from.timecreated;
            to.TimeLastModified = from.timelastmodified;            
        }

        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.FileSystemItem ToViewModel(this MS.FileServices.FileSystemItem fileSystemItem)
        {
            ViewModels.FileSystemItem result = null;
            if (fileSystemItem != null)
            {
                result = new ViewModels.FileSystemItem();
                if (fileSystemItem.Id != null)
                {
                    result.id = fileSystemItem.Id;
                }

                result.name = GetDocumentName(fileSystemItem.Name);
                result.documenttype = GetDocumentType(fileSystemItem.Name);
                result.size = fileSystemItem.Size;
                result.timecreated = fileSystemItem.TimeCreated;
                result.timelastmodified = fileSystemItem.TimeLastModified;

            }            
            return result;
        }                   

        public static MS.FileServices.FileSystemItem ToModel(this ViewModels.FileSystemItem fileSystemItem)
        {
            MS.FileServices.File result = null;
            if (fileSystemItem != null)
            {
                result = new MS.FileServices.File()
                {
                    Id = fileSystemItem.id,
                    Name = CombineNameDocumentType(fileSystemItem.name,fileSystemItem.documenttype),
                    Size = fileSystemItem.size,
                    TimeCreated = fileSystemItem.timecreated,
                    TimeLastModified = fileSystemItem.timelastmodified
                };               
            }
            
            return result;
        }
    }
}
