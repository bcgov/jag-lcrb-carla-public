
namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMevent
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_AccountId@odata.bind")]
        public string AccountODataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_ContactId@odata.bind")]
        public string ContactODataBind { get; set; }

    }
}
