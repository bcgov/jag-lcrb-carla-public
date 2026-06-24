using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class StatsResultResponse
    {
        [JsonProperty(PropertyName = "value")]
        public List<Dictionary<string, string>> Value { get; set; }
    }
}
