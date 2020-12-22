namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class MicrosoftDynamicsCRMlocalizedLabel
    {

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMOptionMetadata class.
        /// </summary>
        public MicrosoftDynamicsCRMlocalizedLabel()
        {
            CustomInit();
        }
        
        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        private void CustomInit()
        {
          
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "LocalizedLabels")]
        public List<MicrosoftDynamicsCRMlocalizedLabel> LocalizedLabels { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "UserLocalizedLabel")]
        public MicrosoftDynamicsCRMlabel UserLocalizedLabel { get; set; }

    }

}
