extern alias DV;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Public.Utils;
using DvForecast = DV::Gov.Lclb.Cllb.Interfaces.adoxio_sepdrinksalesforecast;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class SepDrinksSalesForecastExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>

        public static ViewModels.SepDrinksSalesForecast ToViewModel(this DvForecast forecast)
        {
            if (forecast == null) return null;
            return new ViewModels.SepDrinksSalesForecast
            {
                Id = forecast.adoxio_sepdrinksalesforecastId?.ToString(),
                EstimatedRevenue = forecast.adoxio_EstimatedRevenue,
                IsCharging = forecast.adoxio_IsCharging,
                Name = forecast.adoxio_name,
                EstimatedServings = forecast.adoxio_EstimatedServings,
                PricePerServing = forecast.adoxio_PricePerServing,
                EstimatedCost = forecast.adoxio_EstimatedCost,
                DrinkTypeId = forecast.adoxio_Type?.Id.ToString(),
            };
        }
    }
}

