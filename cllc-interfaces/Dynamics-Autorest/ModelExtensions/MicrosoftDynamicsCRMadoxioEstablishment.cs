namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MicrosoftDynamicsCRMadoxioEstablishment
    {
        
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_Licencee@odata.bind")]
        public string AdoxioLicenceeODataBind { get; set; }
        
    }
}
