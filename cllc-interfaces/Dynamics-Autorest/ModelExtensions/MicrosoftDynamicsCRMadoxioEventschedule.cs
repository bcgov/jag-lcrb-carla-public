using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    public partial class MicrosoftDynamicsCRMadoxioEventschedule
    {

        [JsonProperty(PropertyName = "adoxio_EventId@odata.bind")]
        public string EventODataBind { get; set; }
    }
}
