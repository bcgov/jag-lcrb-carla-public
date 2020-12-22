using Gov.Lclb.Cllb.Interfaces.Models;
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
        public static LicenceEventSchedule ToViewModel(this MicrosoftDynamicsCRMadoxioEventschedule item)
        {
            LicenceEventSchedule result = null;
            if (item != null)
            {
                result = new LicenceEventSchedule();
                if (item.AdoxioEventscheduleid != null)
                {
                    result.Id = item.AdoxioEventscheduleid;
                }
                result.EventId = item._adoxioEventidValue;
                result.EventStartDateTime = item.AdoxioEventstartdatetime;
                result.EventEndDateTime = item.AdoxioEventenddatetime;
                result.ServiceStartDateTime = item.AdoxioServicestartdatetime;
                result.ServiceEndDateTime = item.AdoxioServiceenddatetime;
            }
            return result;
        }


        // Converts a view model into a dynamics entity
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioEventschedule to, LicenceEventSchedule from)
        {
            to.AdoxioEventscheduleid = from.Id;
            to.AdoxioEventstartdatetime = from.EventStartDateTime;
            to.AdoxioEventenddatetime = from.EventEndDateTime;
            to.AdoxioServicestartdatetime = from.ServiceStartDateTime;
            to.AdoxioServiceenddatetime = from.ServiceEndDateTime;
        }
    }
}
