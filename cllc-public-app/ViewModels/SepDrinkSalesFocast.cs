using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SepDrinksSalesForecast
    {
        public string Id { get; set; }
        public decimal? EstimatedRevenue { get; set; }
        public bool? IsCharging { get; set; }
        public string Name { get; set; }
        public int? EstimatedServings { get; set; }
        public decimal? PricePerServing { get; set; }
        public decimal? EstimatedCost { get; set; }
        public string DrinkTypeId { get; set; }

    }
}
