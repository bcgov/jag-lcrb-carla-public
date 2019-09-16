using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class CompletedWorkerScreening
    {
        public string SpdJobId { get; set; }
        public string RecordIdentifier { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SpiceApplicationStatus Result { get; set; }
        public Worker Worker { get; set; }
    }
}
