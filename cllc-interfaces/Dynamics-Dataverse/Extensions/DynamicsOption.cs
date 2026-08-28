using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Interfaces;

public class DynamicsOption
{
    [JsonProperty(PropertyName = "Value")]
    public int? Value { get; set; }

    [JsonProperty(PropertyName = "Label")]
    public DynamicsLocalizedLabel Label { get; set; }

    [JsonProperty(PropertyName = "Description")]
    public DynamicsLocalizedLabel Description { get; set; }

    [JsonProperty(PropertyName = "Color")]
    public string Color { get; set; }

    [JsonProperty(PropertyName = "IsManaged")]
    public bool? IsManaged { get; set; }

    [JsonProperty(PropertyName = "MetadataId")]
    public string MetadataId { get; set; }

    [JsonProperty(PropertyName = "HasChanged")]
    public bool? HasChanged { get; set; }
}
