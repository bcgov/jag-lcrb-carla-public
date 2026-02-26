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

namespace Gov.Lclb.Cllb.Interfaces
{
    public class FolderItem
    {
        public string Name { get; set; }
        public string ServerRelativeUrl { get; set; }
    }

    public class FileItem
    {
        public string Name { get; set; }
        public string ServerRelativeUrl { get; set; }
        public long Length { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeLastModified { get; set; }
    }

    public class FolderSegment
    {
        public string FolderNameSegment { get; set; }
        public string FolderGuidSegment { get; set; }
        public string FolderName { get; set; }
    }

    public class EnsureFolderPathRequest
    {
        public string EntityName { get; set; }
        public List<FolderSegment> FolderPath { get; set; }
    }
}
