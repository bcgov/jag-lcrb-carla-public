using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class LicenceEventScheduleExtensions
    {
        // Converts a dynamics entity into a view model
        public static ViewModels.LicenceEventSchedule ToViewModel(this MicrosoftDynamicsCRMadoxioEventschedule item)
        {
            ViewModels.LicenceEvent result = null;
            if (item != null)
            {
                result = new ViewModels.LicenceEventSchedule();
                if (item.AdoxioEventscheduleid != null)
                {
                    result.Id = item.AdoxioEventscheduleid;
                }
                result.EventId = schedule.AdoxioEventid,
                result.EventStartDateTime = schedule.AdoxioEventstartdatetime,
                result.EventEndDateTime = schedule.AdoxioEventenddatetime,
                result.ServiceStartDateTime = schedule.AdoxioServicestartdatetime,
                result.ServiceStartDateTime = schedule.AdoxioServicestartdatetime
            }
            return result;
        }


        // Converts a view model into a dynamics entity
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioEventschedule to, ViewModels.LicenceEventSchedule from)
        {
            to.AdoxioEventscheduleid = from.Id;
            to.AdoxioEventid = from.EventId;
            to.AdoxioEventstartdatetime = from.EventStartDateTime;
            to.AdoxioEventenddatetime = from.EventEndDateTime;
            to.AdoxioServicestartdatetime = from.ServiceStartDateTime;
            to.AdoxioServiceenddatetime = from.ServiceEndDateTime;
        }
    }
}
