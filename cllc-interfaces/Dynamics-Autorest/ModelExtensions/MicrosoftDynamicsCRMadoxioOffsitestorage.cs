namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMadoxioOffsitestorage
    {
        [JsonProperty(PropertyName = "adoxio_LicenceId@odata.bind")]
        public string LicenceODataBind { get; set; }
    }
}