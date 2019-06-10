using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarlaSpiceSync.models;

namespace SpdSync.models
{
    public class ApplicationScreeningResponse
    {
        public string RecordIdentifier { get; set; }
        public string Result { get; set; }
        public List<Associate> Associates { get; set; }
    }
}
