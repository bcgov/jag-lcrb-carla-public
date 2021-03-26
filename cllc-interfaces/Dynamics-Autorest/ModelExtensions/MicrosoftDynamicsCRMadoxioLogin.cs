using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    partial class MicrosoftDynamicsCRMadoxioLogin
    {
        [JsonProperty(PropertyName = "adoxio_Contact@odata.bind")]
        public string ContactODataBind { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_RelatedAccount@odata.bind")]
        public string RelatedAccountODataBind { get; set; }
    }
}
