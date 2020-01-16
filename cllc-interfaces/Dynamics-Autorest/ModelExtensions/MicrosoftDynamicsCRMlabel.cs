namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMlabel
    {

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMOptionMetadata class.
        /// </summary>
        public MicrosoftDynamicsCRMlabel()
        {
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Label")]
        public string Label { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "LanguageCode")]
        public string LanguageCode { get; set; }
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IsManaged")]
        public bool? IsManaged { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "MetadataId")]
        public string MetadataId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "HasChanged")]
        public bool? HasChanged { get; set; }

    }

}
