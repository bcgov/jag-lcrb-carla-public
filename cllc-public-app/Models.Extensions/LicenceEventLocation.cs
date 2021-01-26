using Gov.Lclb.Cllb.Interfaces.Models;
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
        public static LicenceEventLocation ToViewModel(this MicrosoftDynamicsCRMadoxioEventlocation item)
        {
            LicenceEventLocation result = null;
            if (item != null)
            {
                result = new LicenceEventLocation();
                if (item.AdoxioEventlocationid != null)
                {
                    result.Id = item.AdoxioEventlocationid;
                }
                result.EventId = item._adoxioEventidValue;
                result.Name = item.AdoxioName;
                result.Attendance = item.AdoxioAttendance;
                result.ServiceAreaId = item._adoxioServiceareaidValue;
            }
            return result;
        }


        // Converts a view model into a dynamics entity
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioEventlocation to, LicenceEventLocation from)
        {
            to.AdoxioEventlocationid = from.Id;
            to.AdoxioName = from.Name;
            to.AdoxioAttendance = from.Attendance;
        }
    }
}
