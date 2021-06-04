using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{


    public partial class MicrosoftDynamicsCRMadoxioSpecialevent
    {
       [JsonProperty(PropertyName = "adoxio_AccountId@odata.bind")]
         public string AccountODataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_ContactId@odata.bind")]
         public string ContactODataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_SpecialEventCityDistrictId@odata.bind")]
        public string SepCityODataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_PoliceRepresentativeId@odata.bind")]
        public string PoliceRepresentativeIdODataBind { get; set; }
    }
}
