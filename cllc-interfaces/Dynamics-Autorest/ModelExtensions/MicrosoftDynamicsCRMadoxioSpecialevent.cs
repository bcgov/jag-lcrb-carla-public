using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{


    public partial class MicrosoftDynamicsCRMadoxioSpecialevent
    {

        [JsonProperty(PropertyName = "adoxio_SpecialEventCityDistrictId@odata.bind")]
        public string SepCityODataBind { get; set; }
    }
}
