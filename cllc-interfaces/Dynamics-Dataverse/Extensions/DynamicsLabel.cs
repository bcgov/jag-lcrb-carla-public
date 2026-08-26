using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Interfaces;

public class DynamicsLabel
{
    [JsonProperty(PropertyName = "Label")]
    public string Label { get; set; }

    [JsonProperty(PropertyName = "LanguageCode")]
    public string LanguageCode { get; set; }

    [JsonProperty(PropertyName = "IsManaged")]
    public bool? IsManaged { get; set; }

    [JsonProperty(PropertyName = "MetadataId")]
    public string MetadataId { get; set; }

    [JsonProperty(PropertyName = "HasChanged")]
    public bool? HasChanged { get; set; }
}
