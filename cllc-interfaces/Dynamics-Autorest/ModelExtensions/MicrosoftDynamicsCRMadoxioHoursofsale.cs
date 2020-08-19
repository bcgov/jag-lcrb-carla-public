using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    public partial class MicrosoftDynamicsCRMadoxioHoursofservice
    {

        [JsonProperty(PropertyName = "adoxio_Application@odata.bind")]
        public string ApplicationODataBind { get; set; }
        
    }
}
