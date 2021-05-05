using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class SepDrinkTypeExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>
        public static ViewModels.SepDrinkType ToViewModel(this MicrosoftDynamicsCRMadoxioSepdrinktype drinkType)
        {
            ViewModels.SepDrinkType result = null;
            if (drinkType != null)
            {
                result = new ViewModels.SepDrinkType
                {
                    Id = drinkType.AdoxioSepdrinktypeid,
                    UnitSize = drinkType.AdoxioUnitsize,
                    BulkSize = drinkType.AdoxioBulksize,
                    BulkMultiplier = drinkType.AdoxioBulkmultiplier,
                    CostPerServing = drinkType.AdoxioCostperserving,
                    IsHomeMade = drinkType.AdoxioIshomemade,
                    Group = (ViewModels.DrinkTypeGroup?)drinkType.AdoxioGroup,
                    StorageMethod = (ViewModels.StorageMethod?)drinkType.AdoxioStoragemethod,
                    ServingMethod = (ViewModels.ServingMethod?)drinkType.AdoxioServingmethod,
                    ServingSizeMl = drinkType.AdoxioServingsizeml,
                    StoraheSizeMl = drinkType.AdoxioStoragesizeml,
                };

            }
            return result;
        }
    }
}

