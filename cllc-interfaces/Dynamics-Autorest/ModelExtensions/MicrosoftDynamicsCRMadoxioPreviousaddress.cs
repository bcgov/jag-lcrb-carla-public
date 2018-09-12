namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MicrosoftDynamicsCRMadoxioPreviousaddress
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_ContactId@odata.bind")]
        public string ContactIdODataBind { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_WorkerId@odata.bind")]
        public string WorkerIdODataBind { get; set; }

    }
}
