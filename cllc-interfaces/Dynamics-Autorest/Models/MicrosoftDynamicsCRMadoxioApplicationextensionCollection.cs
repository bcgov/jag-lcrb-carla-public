// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Collection of adoxio_applicationextension
    /// </summary>
    /// <remarks>
    /// Microsoft.Dynamics.CRM.adoxio_applicationextensionCollection
    /// </remarks>
    public partial class MicrosoftDynamicsCRMadoxioApplicationextensionCollection
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioApplicationextensionCollection class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioApplicationextensionCollection()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioApplicationextensionCollection class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioApplicationextensionCollection(IList<MicrosoftDynamicsCRMadoxioApplicationextension> value = default(IList<MicrosoftDynamicsCRMadoxioApplicationextension>))
        {
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public IList<MicrosoftDynamicsCRMadoxioApplicationextension> Value { get; set; }

    }
}
