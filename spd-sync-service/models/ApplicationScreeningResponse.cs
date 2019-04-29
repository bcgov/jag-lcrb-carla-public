using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpdSync.models
{
    public class ApplicationScreeningResponse
    {
        public string RecordIdentifier { get; set; }
        public string Name { get; set; }
        public string Result { get; set; }
        public DateTimeOffset DateProcessed { get; set; }
    }
}
