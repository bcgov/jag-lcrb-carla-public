using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    
    class MicrosoftDynamicsCRMaccountMetadata
    {
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty(PropertyName = "adoxio_dateoperationsceased")]
        public System.DateTimeOffset? AdoxioDateoperationsceased { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty(PropertyName = "adoxio_dateinvolvementceased")]
        public System.DateTimeOffset? AdoxioDateinvolvementceased { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty(PropertyName = "adoxio_datesignordismissed")]
        public System.DateTimeOffset? AdoxioDatesignordismissed { get; set; }
    }
    
    [MetadataType(typeof(MicrosoftDynamicsCRMaccountMetadata))]

    public partial class MicrosoftDynamicsCRMaccount
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_account_adoxio_legalentity_Account@odata.bind")]
        public string AdoxioAccountAdoxioLegalentityAccountODataBind { get; set; }


    }
}
