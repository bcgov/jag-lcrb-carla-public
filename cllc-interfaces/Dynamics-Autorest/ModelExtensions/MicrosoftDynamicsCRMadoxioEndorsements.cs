namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMadoxioEndorsement
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_ApplicationType")]
        public MicrosoftDynamicsCRMadoxioApplicationtype AdoxioApplicationType { get; set; }
    }
}
