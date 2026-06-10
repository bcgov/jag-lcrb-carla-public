extern alias DV;
using System;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
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
                IsTemporaryExtensionArea = serviceArea.AdoxioTemporaryextensionarea.HasValue
                    ? serviceArea.AdoxioTemporaryextensionarea.Value
                    : false,
                EndorsementId = serviceArea._adoxioEndorsementValue?.ToString(),
            };
        }
    }
}
