using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    partial class MicrosoftDynamicsCRMadoxioSepdrinksalesforecast
    {
         [JsonProperty(PropertyName = "adoxio_SpecialEvent@odata.bind")]
        public string SpecialEventODataBind { get; set; }

         [JsonProperty(PropertyName = "adoxio_Type@odata.bind")]
        public string DrinkTypeODataBind { get; set; }

    }
}
