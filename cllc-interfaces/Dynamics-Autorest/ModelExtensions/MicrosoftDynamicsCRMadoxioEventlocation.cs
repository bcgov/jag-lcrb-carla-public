using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    public partial class MicrosoftDynamicsCRMadoxioEventlocation
    {

        [JsonProperty(PropertyName = "adoxio_EventId@odata.bind")]
        public string EventODataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_ServiceAreaId@odata.bind")]
        public string ServiceAreaODataBind { get; set; }
    }
}
