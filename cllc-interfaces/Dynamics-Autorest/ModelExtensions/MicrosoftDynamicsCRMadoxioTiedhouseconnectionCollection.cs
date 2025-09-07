using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    public partial class MicrosoftDynamicsCRMadoxioTiedhouseconnectionCollection
    {
        [JsonProperty(PropertyName = "@odata.nextLink")]
        public string OdataNextLink { get; set; }

        [JsonProperty(PropertyName = "@odata.count")]
        public string Count { get; set; }
    }
}
