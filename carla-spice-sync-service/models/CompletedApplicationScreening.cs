using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class CompletedApplicationScreening
    {
        public string RecordIdentifier { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public SpiceApplicationStatus Result { get; set; }
        public List<Associate> Associates { get; set; }
    }
}
