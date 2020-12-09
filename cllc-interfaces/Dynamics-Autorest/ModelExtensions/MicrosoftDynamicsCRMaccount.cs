using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{

    class MicrosoftDynamicsCRMaccountMetadata
    {
        [JsonProperty(PropertyName = "adoxio_dateoperationsceased")]
        [JsonConverter(typeof(SimpleDateTimeConverter))]
        public System.DateTimeOffset? AdoxioDateoperationsceased { get; set; }

        [JsonProperty(PropertyName = "adoxio_dateinvolvementceased")]
        [JsonConverter(typeof(SimpleDateTimeConverter))]
        public System.DateTimeOffset? AdoxioDateinvolvementceased { get; set; }

        // 2020-12-08 ASR - Removed extra annotations for "adoxio_datesignordismissed" to fix data serialization error
    }

    [MetadataType(typeof(MicrosoftDynamicsCRMaccountMetadata))]

    public partial class MicrosoftDynamicsCRMaccount
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_account_adoxio_legalentity_Account@odata.bind")]
        public string AdoxioAccountAdoxioLegalentityAccountODataBind { get; set; }

        [JsonProperty(PropertyName = "primarycontactid@odata.bind")]
        public string PrimaryContactIdODataBind { get; set; }

        [JsonProperty(PropertyName = "contact_customer_accounts@odata.bind")]
        public string ContactCustomerAccountsODataBind { get; set; }
        
    }
}
