extern alias DV;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using adoxio_servicearea = DV::Gov.Lclb.Cllb.Interfaces.adoxio_servicearea;

namespace Gov.Lclb.Cllb.Public.Models
{
    public static class CapacityAreaExtensions
    {
        public static CapacityArea ToViewModel(this adoxio_servicearea area)
        {
            return new CapacityArea
            {
                Id = area.adoxio_serviceareaId?.ToString(),
                AreaNumber = area.adoxio_areanumber ?? 0,
                AreaLocation = area.adoxio_arealocation,
                AreaCategory = (int?)area.adoxio_areacategory,
                IsIndoor = area.adoxio_isindoor ?? false,
                IsOutdoor = area.adoxio_isoutdoor ?? false,
                IsPatio = area.adoxio_ispatio ?? false,
                Capacity = area.adoxio_capacity,
                IsTemporaryExtensionArea = area.adoxio_TemporaryExtensionArea ?? false,
                EndorsementId = area.adoxio_Endorsement?.Id.ToString()
            };
        }
    }
}
