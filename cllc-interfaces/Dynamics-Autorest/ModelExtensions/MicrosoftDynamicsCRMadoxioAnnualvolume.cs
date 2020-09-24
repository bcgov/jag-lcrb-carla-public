using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    public partial class MicrosoftDynamicsCRMadoxioAnnualvolume
    {
        [JsonProperty(PropertyName = "adoxio_Application@odata.bind")]
        public string AdoxioApplicationODataBind { get; set; }
        [JsonProperty(PropertyName = "adoxio_Licence@odata.bind")]
        public string AdoxioLicenceODataBind { get; set; }
    }

}
