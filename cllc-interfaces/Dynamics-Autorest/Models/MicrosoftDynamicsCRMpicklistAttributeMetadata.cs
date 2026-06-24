using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    public class MicrosoftDynamicsCRMpicklistAttributeMetadata
    {
        [JsonProperty(PropertyName = "OptionSet")]
        public object OptionSet { get; set; }
    }

    public class MicrosoftDynamicsCRMpicklistAttributeMetadataCollection
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string ODataContext { get; set; }

        [JsonProperty(PropertyName = "value")]
        public List<MicrosoftDynamicsCRMpicklistAttributeMetadata> Value { get; set; }
    }
}
