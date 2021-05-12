using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    partial class MicrosoftDynamicsCRMadoxioSpecialeventlocation
    {
         [JsonProperty(PropertyName = "adoxio_SpecialEventId@odata.bind")]
        public string AdoxioSpecialEventODataBind { get; set; }

    }
}
