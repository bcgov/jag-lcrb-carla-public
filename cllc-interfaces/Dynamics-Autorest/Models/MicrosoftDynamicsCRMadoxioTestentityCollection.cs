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
    /// Collection of adoxio_testentity
    /// </summary>
    /// <remarks>
    /// Microsoft.Dynamics.CRM.adoxio_testentityCollection
    /// </remarks>
    public partial class MicrosoftDynamicsCRMadoxioTestentityCollection
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioTestentityCollection class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioTestentityCollection()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioTestentityCollection class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioTestentityCollection(IList<MicrosoftDynamicsCRMadoxioTestentity> value = default(IList<MicrosoftDynamicsCRMadoxioTestentity>))
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
        public IList<MicrosoftDynamicsCRMadoxioTestentity> Value { get; set; }

    }
}
