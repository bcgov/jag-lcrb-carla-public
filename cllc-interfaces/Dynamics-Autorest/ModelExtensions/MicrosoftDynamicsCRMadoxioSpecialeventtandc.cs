using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    public partial class MicrosoftDynamicsCRMadoxioSpecialeventtandc
    {

        [JsonProperty(PropertyName = "adoxio_SpecialEventId@odata.bind")]
        public string SpecialEventODataBind { get; set; }


    }
}
