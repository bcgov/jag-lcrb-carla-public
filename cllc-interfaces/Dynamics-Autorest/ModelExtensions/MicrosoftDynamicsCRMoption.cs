namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMoption
    {

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMOptionMetadata class.
        /// </summary>
        public MicrosoftDynamicsCRMoption()
        {
            CustomInit();
        }       

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Value")]
        public int? Value { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Label")]
        public MicrosoftDynamicsCRMlocalizedLabel Label { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Description")]
        public MicrosoftDynamicsCRMlocalizedLabel Description { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Color")]
        public string Color { get; set; }

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
