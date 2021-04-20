using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    class MicrosoftDynamicsCRMadoxioLdborderMetadata
    {
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty(PropertyName = "adoxio_monthstart")]
        public System.DateTimeOffset? AdoxioMonthstart { get; set; }

        /// <summary>
        /// </summary>
        ///
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty(PropertyName = "adoxio_monthend")]
        public System.DateTimeOffset? AdoxioMonthend { get; set; }

    }

    [MetadataType(typeof(MicrosoftDynamicsCRMadoxioLdborderMetadata))]
    public partial class MicrosoftDynamicsCRMadoxioLdborder
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_LicenceId@odata.bind")]
        public string LicenceIdODataBind { get; set; }

    }
}
