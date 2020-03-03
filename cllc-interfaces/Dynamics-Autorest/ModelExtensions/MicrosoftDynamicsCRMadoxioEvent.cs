using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    class MicrosoftDynamicsCRMadoxioeventMetadata
    {
        [JsonProperty(PropertyName = "adoxio_startdate")]
        [JsonConverter(typeof(SimpleDateTimeConverter))]

        public System.DateTimeOffset? AdoxioStartdate { get; set; }

        [JsonProperty(PropertyName = "adoxio_enddate")]
        [JsonConverter(typeof(SimpleDateTimeConverter))]

        public System.DateTimeOffset? AdoxioEnddate { get; set; }
    }
    
    [MetadataType(typeof(MicrosoftDynamicsCRMadoxioeventMetadata))]
    public partial class MicrosoftDynamicsCRMadoxioEvent
    {

        [JsonProperty(PropertyName = "adoxio_Licence@odata.bind")]
        public string LicenceODataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_Account@odata.bind")]
        public string AccountODataBind { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_contactphonenumber")]
        public string AdoxioContactphonenumber { get; set; }
    }
}
