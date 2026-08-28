using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Interfaces;

public class DynamicsPicklistAttributeMetadata
{
    [JsonProperty(PropertyName = "LogicalName")]
    public string LogicalName { get; set; }

    [JsonProperty(PropertyName = "MetadataId")]
    public string MetadataId { get; set; }

    [JsonProperty(PropertyName = "OptionSet@odata.context")]
    public string OptionSetODataContext { get; set; }

    [JsonProperty(PropertyName = "OptionSet")]
    public DynamicsOptionSet OptionSet { get; set; }

    [JsonProperty(PropertyName = "GlobalOptionSet")]
    public DynamicsOptionSet GlobalOptionSet { get; set; }
}
