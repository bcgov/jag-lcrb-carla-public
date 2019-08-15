using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarlaSpiceSync.models
{
    public class CompletedWorkerScreening
    {
        public string RecordIdentifier { get; set; }
        public string Result { get; set; }
        public Worker Worker { get; set; }
    }
}
