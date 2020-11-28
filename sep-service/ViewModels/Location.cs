
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SepService.ViewModels
{
    public enum LocationSetting
    {
        Indoor = 845280000,
        Outdoor  = 845280001,
        Both = 845280002
    }

    public class Location
    {
        public Address Address { get; set; }
        public EventDate EventDate { get; set; }
        public string LocationDescription { get; set; }
        public string LocationName { get; set; }
        public int MaxGuests { get; set; }
        public bool MinorPresent { get; set; }
        public int? NumberMinors { get; set; }

        [Required]
        [EnumDataType(typeof(LocationSetting))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LocationSetting Setting { get; set;}
        

    }
}
