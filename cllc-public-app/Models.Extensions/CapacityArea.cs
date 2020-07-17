using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models.Extensions
{
    public static class CapacityAreaExtensions
    {
        public static ViewModels.CapacityArea ToViewModel(this MicrosoftDynamicsCRMadoxioServicearea serviceArea)
        {
            return new CapacityArea()
            {
                AreaNumber = (int)serviceArea.AdoxioAreanumber,
                AreaCategory = serviceArea.AdoxioAreacategory,
                AreaLocation = serviceArea.AdoxioArealocation,
                IsIndoor = (bool)serviceArea.AdoxioIsindoor,
                IsOutdoor = (bool)serviceArea.AdoxioIsoutdoor,
                IsPatio = (bool)serviceArea.AdoxioIspatio,
                Capacity = serviceArea.AdoxioCapacity.HasValue ? serviceArea.AdoxioCapacity : 0
            };
        }
    }
}
