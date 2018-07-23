namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MicrosoftDynamicsCRMinvoice
    {
        
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customerid_account@odata.bind")]
        public string CustomerIdAccountODataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_returnedtransactionid")]
        public string AdoxioReturnedtransactionid { get; set; }

    }
}
