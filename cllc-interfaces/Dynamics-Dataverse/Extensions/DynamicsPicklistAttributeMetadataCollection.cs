using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Interfaces;

public class DynamicsPicklistAttributeMetadataCollection
{
    [JsonProperty(PropertyName = "@odata.context")]
    public string ODataContext { get; set; }

    [JsonProperty(PropertyName = "value")]
    public List<DynamicsPicklistAttributeMetadata> Value { get; set; }
}
