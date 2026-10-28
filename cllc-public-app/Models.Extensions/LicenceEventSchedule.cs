extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.ViewModels;
using Gov.Lclb.Cllb.Public.Utils;
using System;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class LicenceEventScheduleExtensions
    {
        // Converts a dynamics entity into a view model

        public static LicenceEventSchedule ToViewModel(this adoxio_eventschedule item)
        {
            if (item == null) return null;
            return new LicenceEventSchedule
            {
                Id = item.adoxio_eventscheduleId?.ToString(),
                EventId = item.adoxio_EventId?.Id.ToString(),
                EventStartDateTime = item.adoxio_EventStartDateTime.HasValue ? (DateTimeOffset?)item.adoxio_EventStartDateTime.Value : null,
                EventEndDateTime = item.adoxio_EventEndDateTime.HasValue ? (DateTimeOffset?)item.adoxio_EventEndDateTime.Value : null,
                ServiceStartDateTime = item.adoxio_ServiceStartDateTime.HasValue ? (DateTimeOffset?)item.adoxio_ServiceStartDateTime.Value : null,
                ServiceEndDateTime = item.adoxio_ServiceEndDateTime.HasValue ? (DateTimeOffset?)item.adoxio_ServiceEndDateTime.Value : null,
            };
        }

        public static void CopyValues(this adoxio_eventschedule to, LicenceEventSchedule from)
        {
            to.adoxio_EventStartDateTime = from.EventStartDateTime?.UtcDateTime;
            to.adoxio_EventEndDateTime = from.EventEndDateTime?.UtcDateTime;
            to.adoxio_ServiceStartDateTime = from.ServiceStartDateTime?.UtcDateTime;
            to.adoxio_ServiceEndDateTime = from.ServiceEndDateTime?.UtcDateTime;
        }
    }
}
