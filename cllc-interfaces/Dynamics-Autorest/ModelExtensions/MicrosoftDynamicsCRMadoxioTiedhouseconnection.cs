namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMadoxioTiedhouseconnection
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_AccountId@odata.bind")]
        public string AccountODataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_liquorfinancialinterest")]
        public int? AdoxioLiquorFinancialInterest { get; set; }

        [JsonProperty(PropertyName = "adoxio_liquorfinancialinterestdetails")]
        public string AdoxioLiquorFinancialInterestDetails { get; set; }

    }
}
