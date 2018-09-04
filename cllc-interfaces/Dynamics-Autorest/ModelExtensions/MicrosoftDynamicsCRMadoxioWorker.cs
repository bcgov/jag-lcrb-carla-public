namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MicrosoftDynamicsCRMadoxioWorker
    {
        
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_adoxio_contactid_value@odata.bind")]
        public string ContactIdAccountODataBind { get; set; }

    }
}
