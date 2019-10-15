
namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MicrosoftDynamicsCRMadoxioLicenseechangelog
    {
        [JsonProperty(PropertyName = "adoxio_LegalEntity@odata.bind")]
        public string LegalEntityOdataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_Application@odata.bind")]
        public string ApplicationOdataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_ParentLinceseeChangeLogId@odata.bind")]
        public string ParentLinceseeChangeLogOdataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_ParentLegalEntityId@odata.bind")]
        public string ParentLegalEntityOdataBind { get; set; }


    }
}
