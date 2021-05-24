using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum ServiceAreaSetting{
        Indoors = 845280000,
        Outdoors = 845280001,
        BothOutdoorsAndIndoors = 845280002
    }
    public class SepServiceArea
    {
        public string Id { get; set; }
        public string LocationId { get; set; }
        public string SpecialEventId { get; set; }
        public int LocalId { get; set; } // Client side local db id
        public bool? MinorPresent { get; set; }
        public int? LicencedAreaMaxNumberOfGuests { get; set; }
        public int? MaximumNumberOfGuests { get; set; }
        public bool? IsBothOutdoorIndoor { get; set; }
        public bool? IsIndoors { get; set; }
        public int? NumberOfMinors { get; set; }
        public int? LicencedAreaNumberOfMinors { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceAreaSetting? Setting { get; set; }
        public int? StatusCode { get; set; }
        public int? StateCode { get; set; }
        public string EventName { get; set; }
        public bool? IsOutdoors { get; set; }
        public string LicencedAreaDescription { get; set; }

    }
}
