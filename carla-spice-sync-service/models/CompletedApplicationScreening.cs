using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class CompletedApplicationScreening
    {
        public string RecordIdentifier { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApplicationSecurityStatus Result { get; set; }
        public List<Associate> Associates { get; set; }
    }
}
