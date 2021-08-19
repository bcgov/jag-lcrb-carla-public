using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum DrinkTypeGroup
    {
        Beer = 845280000,
        Spirit = 845280002,
        Wine = 845280001,

    }

    public enum ServingMethod
    {
        bottlesCansGlasses = 845280000,
        glasses = 845280001,
        shots = 845280002,
    }

    public enum StorageMethod
    {
        kegs = 845280000,
        bottles = 845280001
    }

    public class SepDrinkType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UnitSize { get; set; }
        public string BulkSize { get; set; }
        public int? BulkMultiplier { get; set; }
        public decimal? CostPerServing { get; set; }
        public decimal? PricePerServing { get; set; }
        public bool? IsHomeMade { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DrinkTypeGroup? Group { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StorageMethod? StorageMethod { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServingMethod? ServingMethod { get; set; }
        public decimal? ServingSizeMl { get; set; }
        public decimal? StoraheSizeMl { get; set; }
    }
}
