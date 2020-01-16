namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class MicrosoftDynamicsCRMpicklistAttributeMetadataCollection
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "@odata.context")]
        public string ODataContext{ get; set; }

        [JsonProperty(PropertyName = "value")]
        public List<MicrosoftDynamicsCRMpicklistAttributeMetadata> Value { get; set; }

    }
}
