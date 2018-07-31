namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MicrosoftDynamicsCRMsharepointdocumentlocation
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "regardingobjectid_adoxio_application@odata.bind")]
        public string RegardingobjectidAdoxioApplicationODataBind { get; set; }

        [JsonProperty(PropertyName = "parentsiteorlocation_sharepointdocumentlocation@odata.bind")]        
        public string ParentsiteorlocationSharepointdocumentlocationODataBind { get; set; }

    }
}
