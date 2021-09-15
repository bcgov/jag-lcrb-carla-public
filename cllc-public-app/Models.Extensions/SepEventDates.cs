using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Models;

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
    }
}
