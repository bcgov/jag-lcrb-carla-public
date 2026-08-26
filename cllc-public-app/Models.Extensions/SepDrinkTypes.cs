extern alias DV;
using System.Collections.Generic;
using System.Linq;
using DvDrinkType = DV::Gov.Lclb.Cllb.Interfaces.adoxio_sepdrinktype;

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
        public static ViewModels.SepDrinkType ToViewModel(this DvDrinkType drinkType)
        {
            if (drinkType == null) return null;
            return new ViewModels.SepDrinkType
            {
                Id = drinkType.adoxio_sepdrinktypeId?.ToString(),
                Name = drinkType.adoxio_name,
                UnitSize = drinkType.adoxio_UnitSize,
                BulkSize = drinkType.adoxio_BulkSize,
                BulkMultiplier = drinkType.adoxio_BulkMultiplier,
                CostPerServing = drinkType.adoxio_CostPerServing,
                PricePerServing = drinkType.adoxio_MaxPrice,
                IsHomeMade = drinkType.adoxio_IsHomeMade,
                Group = (ViewModels.DrinkTypeGroup?)(int?)drinkType.adoxio_Group,
                StorageMethod = (ViewModels.StorageMethod?)(int?)drinkType.adoxio_StorageMethod,
                ServingMethod = (ViewModels.ServingMethod?)(int?)drinkType.adoxio_ServingMethod,
                ServingSizeMl = drinkType.adoxio_ServingSizeML,
                StorageSizeMl = drinkType.adoxio_StorageSizeML,
            };
        }
    }
}



