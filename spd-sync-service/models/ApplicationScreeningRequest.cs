using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpdSync.models
{
    public class ApplicationScreeningRequest
    {
        public string Name { get; set; }
        public string RecordIdentifier { get; set; }

        public string ApplicantType { get; set; }
        public bool UrgentPriority { get; set; }

        public DateTimeOffset DateSent { get; set; }

        Account Account { get; set; }

        Establishment Establishment { get; set; }

        public List<WorkerScreeningRequest> Associates { get; set; }
}
}
