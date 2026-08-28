using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Interfaces;

public class DynamicsOptionSet
{
    [JsonProperty(PropertyName = "Name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "OptionSetType")]
    public string OptionSetType { get; set; }

    [JsonProperty(PropertyName = "Description")]
    public DynamicsLocalizedLabel Description { get; set; }

    [JsonProperty(PropertyName = "DisplayName")]
    public DynamicsLocalizedLabel DisplayName { get; set; }

    [JsonProperty(PropertyName = "Color")]
    public string Color { get; set; }

    [JsonProperty(PropertyName = "IsManaged")]
    public bool? IsManaged { get; set; }

    [JsonProperty(PropertyName = "MetadataId")]
    public string MetadataId { get; set; }

    [JsonProperty(PropertyName = "HasChanged")]
    public bool? HasChanged { get; set; }

    [JsonProperty(PropertyName = "IsCustomOptionSet")]
    public bool? IsCustomOptionSet { get; set; }

    [JsonProperty(PropertyName = "IsGlobal")]
    public bool? IsGlobal { get; set; }

    [JsonProperty(PropertyName = "IsCustomizable")]
    public object IsCustomizable { get; set; }

    [JsonProperty(PropertyName = "Options")]
    public List<DynamicsOption> Options { get; set; }
}
