namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMadoxioTiedhouseconnection
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_AccountId@odata.bind")]
        public string AccountODataBind { get; set; }
        [JsonProperty(PropertyName = "adoxio_Application@odata.bind")]
        public string ApplicationOdataBind { get; set; }
        [JsonProperty(PropertyName = "adoxio_SupersededBy@odata.bind")]
        public string SupersededByOdataBind { get; set; }
        
    }
}
