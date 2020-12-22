using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SepService.ViewModels
{

    public enum TandcType
    {
        GlobalCondition = 845280000,
        Condition = 845280001
    }

    public class TermAndCondition
    {
        public string Originator { get; set; }
        public string Text { get; set; }
        [Required]
        [EnumDataType(typeof(TandcType))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("type")]
        public TandcType TandcType { get; set; }
    }
}
