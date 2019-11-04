namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMadoxioCannabisinventoryreport
    {
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_packagedunitsnumberretailer")]
        public object AdoxioPackagedunitsnumberretailer { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_totalvalueretailer")]
        public object AdoxioTotalvalueretailer { get; set; }
    }
}
