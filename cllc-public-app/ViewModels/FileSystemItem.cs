using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class FileSystemItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string documenttype { get; set; }
        public int size { get; set; }
        public string serverrelativeurl { get; set; }
        public DateTime timecreated { get; set; }
        public DateTime timelastmodified { get; set; }
    }
}
