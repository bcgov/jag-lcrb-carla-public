using System.Text.Json.Serialization;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class CompletedWorkerScreening
    {
        public string SpdJobId { get; set; }
        public string RecordIdentifier { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WorkerSecurityStatus ScreeningResult { get; set; }
        public Worker Worker { get; set; }
    }
}
