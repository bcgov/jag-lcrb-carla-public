using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    

    public partial class MicrosoftDynamicsCRMadoxioSpecialevent
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_specialevent_schedule@odata.bind")]
        public string SpecialEventScheduleODataBind { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_specialevent_licencedarea@odata.bind")]
        public string SpecialEventLicencedAreaODataBind { get; set; }
        


    }
}
