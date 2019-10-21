namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMadoxioAlias
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_ContactId@odata.bind")]
        public string ContactIdODataBind { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_WorkerId@odata.bind")]
        public string WorkerIdODataBind { get; set; }

    }
}
