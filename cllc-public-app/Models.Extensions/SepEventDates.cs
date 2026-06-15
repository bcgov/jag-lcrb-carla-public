extern alias DV;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Models;
using DvSchedule = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialeventschedule;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class SepEventDatesExtensions
    {
        /// <summary>
        /// Convert a given Special Event Schedule to a ViewModel
        /// </summary>
        public static ViewModels.SepEventDates ToViewModel(this MicrosoftDynamicsCRMadoxioSpecialeventschedule eventDates)
        {
            ViewModels.SepEventDates result = null;
            if (eventDates != null)
            {
                result = new ViewModels.SepEventDates
                {
                    Id = eventDates.AdoxioSpecialeventscheduleid,
                    SpecialEventId = eventDates._adoxioSpecialeventidValue,
                    LocationId = eventDates._adoxioSpecialeventlocationidValue,
                    EventDate = eventDates.AdoxioEventdate,
                    EventStart = eventDates.AdoxioEventstart,
                    EventEnd = eventDates.AdoxioEventend,
                    ServiceStart = eventDates.AdoxioServicestart,
                    ServiceEnd = eventDates.AdoxioServiceend,
                    LiquorServiceHoursExtensionReason = eventDates.AdoxioLiquorservicehoursextensionreason,
                    DisturbancePreventionMeasuresDetails = eventDates.AdoxioDisturbancepreventionmeasuresdetails
                };
            }
            return result;
        }

        public static void CopyValues(this MicrosoftDynamicsCRMadoxioSpecialeventschedule to, ViewModels.SepEventDates from)
        {
            to.AdoxioEventstart = from.EventStart;
            to.AdoxioEventend = from.EventEnd;
            to.AdoxioServicestart = from.ServiceStart;
            to.AdoxioServiceend = from.ServiceEnd;
            to.AdoxioDisturbancepreventionmeasuresdetails = from.DisturbancePreventionMeasuresDetails;
            to.AdoxioLiquorservicehoursextensionreason = from.LiquorServiceHoursExtensionReason;
        }

        public static ViewModels.SepEventDates ToViewModel(this DvSchedule sched)
        {
            if (sched == null) return null;
            return new ViewModels.SepEventDates
            {
                Id = sched.adoxio_specialeventscheduleId?.ToString(),
                SpecialEventId = sched.adoxio_SpecialEventId?.Id.ToString(),
                LocationId = sched.adoxio_SpecialEventLocationId?.Id.ToString(),
                EventDate = sched.adoxio_EventDate,
                EventStart = sched.adoxio_EventStart,
                EventEnd = sched.adoxio_EventEnd,
                ServiceStart = sched.adoxio_ServiceStart,
                ServiceEnd = sched.adoxio_ServiceEnd,
                LiquorServiceHoursExtensionReason = sched.adoxio_LiquorServiceHoursExtensionReason,
                DisturbancePreventionMeasuresDetails = sched.adoxio_DisturbancePreventionMeasuresDetails,
            };
        }

        public static void CopyValues(this DvSchedule to, ViewModels.SepEventDates from)
        {
            if (from == null) return;
            to.adoxio_EventStart = from.EventStart;
            to.adoxio_EventEnd = from.EventEnd;
            to.adoxio_ServiceStart = from.ServiceStart;
            to.adoxio_ServiceEnd = from.ServiceEnd;
            to.adoxio_DisturbancePreventionMeasuresDetails = from.DisturbancePreventionMeasuresDetails;
            to.adoxio_LiquorServiceHoursExtensionReason = from.LiquorServiceHoursExtensionReason;
        }
    }
}
