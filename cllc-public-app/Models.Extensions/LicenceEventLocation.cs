extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class LicenceEventLocationExtensions
    {
        // Converts a dynamics entity into a view model

        public static LicenceEventLocation ToViewModel(this adoxio_eventlocation item)
        {
            if (item == null) return null;
            return new LicenceEventLocation
            {
                Id = item.adoxio_eventlocationId?.ToString(),
                EventId = item.adoxio_EventId?.Id.ToString(),
                Name = item.adoxio_name,
                Attendance = item.adoxio_Attendance,
                ServiceAreaId = item.adoxio_ServiceAreaId?.Id.ToString(),
            };
        }

        public static void CopyValues(this adoxio_eventlocation to, LicenceEventLocation from)
        {
            to.adoxio_name = from.Name;
            to.adoxio_Attendance = from.Attendance;
        }
    }
}
