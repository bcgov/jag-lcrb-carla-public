using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    partial class MicrosoftDynamicsCRMadoxioSpecialeventschedule
    {
        [JsonProperty(PropertyName = "adoxio_SpecialEventId@odata.bind")]
        public string AdoxioSpecialEventODataBind { get; set; }

        
        [JsonProperty(PropertyName = "adoxio_SpecialEventLocationId@odata.bind")]
        public string AdoxioSpecialEventLocationODataBind { get; set; }

        
        [JsonProperty(PropertyName = "adoxio_ServiceAreaId@odata.bind")]
        public string AdoxioServiceAreaODataBind { get; set; }
    }
}
