namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMpicklistAttributeMetadata
    {

        /// <summary>
        /// </summary>
                 
        [JsonProperty(PropertyName = "LogicalName")]
        public string LogicalName{ get; set; }

        [JsonProperty(PropertyName = "MetadataId")]
        public string MetadataId { get; set; }

        [JsonProperty(PropertyName = "OptionSet@odata.context")]
        public string OptionSetODataContext { get; set; }

        [JsonProperty(PropertyName = "OptionSet")]
        public MicrosoftDynamicsCRMoptionSet OptionSet { get; set; }

        [JsonProperty(PropertyName = "GlobalOptionSet")]
        public MicrosoftDynamicsCRMoptionSet GlobalOptionSet { get; set; }

    }
}
