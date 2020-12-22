using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SepService.ViewModels
{
    public class LicencedArea
    {
        public string Description { get; set; }
        public int MaxGuests { get; set; }
        public bool MinorsPresent { get; set; }
        public int NumberOfMinors { get; set; }
        [Required]
        [EnumDataType(typeof(LocationSetting))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LocationSetting Setting { get; set; }
    }
}
