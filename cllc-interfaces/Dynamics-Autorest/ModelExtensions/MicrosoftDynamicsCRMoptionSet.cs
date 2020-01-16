namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class MicrosoftDynamicsCRMoptionSet
    {

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMOptionMetadata class.
        /// </summary>
        public MicrosoftDynamicsCRMoptionSet()
        {
            CustomInit();
        }
      

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string? Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "OptionSetType")]
        public string OptionSetType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Description")]
        public MicrosoftDynamicsCRMlocalizedLabel Description { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "DisplayName")]
        public MicrosoftDynamicsCRMlocalizedLabel DisplayName { get; set; }

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

        // <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IsCustomOptionSet")]
        public bool? IsCustomOptionSet { get; set; }

        // <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IsGlobal")]
        public bool? IsGlobal { get; set; }

        // <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IsCustomizable")]
        public object IsCustomizable { get; set; }

        // <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Options")]
        public List<MicrosoftDynamicsCRMoption> Options { get; set; }


    }

}
