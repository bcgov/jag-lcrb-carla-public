using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Interfaces;

public class DynamicsLocalizedLabel
{
    [JsonProperty(PropertyName = "LocalizedLabels")]
    public List<DynamicsLocalizedLabel> LocalizedLabels { get; set; }

    [JsonProperty(PropertyName = "UserLocalizedLabel")]
    public DynamicsLabel UserLocalizedLabel { get; set; }
}
