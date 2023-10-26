using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    public static class CapacityAreaExtensions
    {
        public static CapacityArea ToViewModel(this MicrosoftDynamicsCRMadoxioServicearea serviceArea)
        {
            return new CapacityArea
            {
                Id = serviceArea.AdoxioServiceareaid,
                // we can not cast to int when  the value is null. 
                AreaNumber = serviceArea.AdoxioAreanumber == null ? 0 : (int)serviceArea.AdoxioAreanumber,
                AreaCategory = serviceArea.AdoxioAreacategory,
                AreaLocation = serviceArea.AdoxioArealocation,
                IsIndoor = serviceArea.AdoxioIsindoor == true,
                IsOutdoor = serviceArea.AdoxioIsoutdoor == true,
                IsPatio = serviceArea.AdoxioIspatio == true,
                Capacity = serviceArea.AdoxioCapacity == null ? 0 : serviceArea.AdoxioCapacity,
                IsTemporaryExtensionArea= serviceArea.AdoxioTemporaryextensionarea.HasValue? serviceArea.AdoxioTemporaryextensionarea.Value:false,
            };
        }
    }
}
