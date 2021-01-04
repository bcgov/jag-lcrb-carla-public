using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    class MicrosoftDynamicsCRMadoxioOnestopmessageitemMetadata
    {
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty(PropertyName = "adoxio_datesent")]
        public System.DateTimeOffset? AdoxioDatesent { get; set; }  // Edm.Date type
    }

    [MetadataType(typeof(MicrosoftDynamicsCRMadoxioOnestopmessageitemMetadata))]
    public partial class MicrosoftDynamicsCRMadoxioOnestopmessageitem
    {
        
    }

    
}
