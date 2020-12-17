using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
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
