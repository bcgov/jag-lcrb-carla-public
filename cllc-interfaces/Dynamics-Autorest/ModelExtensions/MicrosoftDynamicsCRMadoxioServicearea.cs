namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMadoxioServicearea
    {
        [JsonProperty(PropertyName = "adoxio_ApplicationId@odata.bind")]
        public string ApplicationOdataBind { get; set; }
    }
}