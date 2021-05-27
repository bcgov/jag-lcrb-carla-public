using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Models;

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
        public static ViewModels.SepDrinksSalesForecast ToViewModel(this MicrosoftDynamicsCRMadoxioSepdrinksalesforecast forecast)
        {
            ViewModels.SepDrinksSalesForecast result = null;
            if (forecast != null)
            {
                result = new ViewModels.SepDrinksSalesForecast()
                {
                    Id = forecast.AdoxioSepdrinksalesforecastid,
                    EstimatedRevenue = forecast.AdoxioEstimatedrevenue,
                    IsCharging = forecast.AdoxioIscharging,
                    Name = forecast.AdoxioName,
                    EstimatedServings = forecast.AdoxioEstimatedservings,
                    PricePerServing = forecast.AdoxioPriceperserving,
                    EstimatedCost = forecast.AdoxioEstimatedcost,
                    DrinkTypeId = forecast._adoxioTypeValue
                };
            }
            return result;
        }

        public static void CopyValues(this MicrosoftDynamicsCRMadoxioSepdrinksalesforecast to, ViewModels.SepDrinksSalesForecast from)
        {
            to.AdoxioEstimatedrevenue = from.EstimatedRevenue;
            to.AdoxioIscharging = from.IsCharging;
            to.AdoxioName = from.Name;
            to.AdoxioEstimatedservings = from.EstimatedServings;
            to.AdoxioPriceperserving = from.PricePerServing;
            to.AdoxioEstimatedcost = from.EstimatedCost;
        }
    }
}

