using System;

namespace SpdSync.models
{
    public class WorkerScreeningResponse
    {
        public string RecordIdentifier { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public string Result { get; set; }
        public DateTimeOffset DateProcessed { get; set; }
    }
}
