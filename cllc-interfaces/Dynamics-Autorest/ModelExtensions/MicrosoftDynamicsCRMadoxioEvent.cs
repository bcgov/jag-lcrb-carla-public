using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    class MicrosoftDynamicsCRMadoxioeventMetadata
    {
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty(PropertyName = "adoxio_startdate")]
        public System.DateTimeOffset? AdoxioStartdate { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty(PropertyName = "adoxio_enddate")]
        public System.DateTimeOffset? AdoxioEnddate { get; set; }
    }
    
    [MetadataType(typeof(MicrosoftDynamicsCRMadoxioeventMetadata))]
    public partial class MicrosoftDynamicsCRMadoxioEvent
    {

        [JsonProperty(PropertyName = "adoxio_Licence@odata.bind")]
        public string LicenceODataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_Account@odata.bind")]
        public string AccountODataBind { get; set; }
    }
}
