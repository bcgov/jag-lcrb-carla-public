namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MicrosoftDynamicsCRMsharepointdocumentlocation
    {

        [JsonProperty(PropertyName = "regardingobjectid_adoxio_application@odata.bind")]
        public string RegardingobjectidAdoxioApplicationODataBind { get; set; }

        [JsonProperty(PropertyName = "parentsiteorlocation_sharepointsite@odata.bind")]
        public string ParentSiteODataBind { get; set; }

        [JsonProperty(PropertyName = "regardingobjectid_adoxio_worker@odata.bind")]
        public string RegardingobjectidWorkerApplicationODataBind { get; set; }

        [JsonProperty(PropertyName = "parentsiteorlocation_sharepointdocumentlocation@odata.bind")]        
        public string ParentsiteorlocationSharepointdocumentlocationODataBind { get; set; }

    }
}
