using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class SepEventLocationExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>
        public static ViewModels.SepEventLocation ToViewModel(this MicrosoftDynamicsCRMadoxioSpecialeventlocation location)
        {
            ViewModels.SepEventLocation result = null;
            if (location == null)
            {
                return result;
            }

            result = new ViewModels.SepEventLocation
            {
                ServiceAreas = new List<ViewModels.SepServiceArea>(),
            };
            result.LocationId = location.AdoxioSpecialeventlocationid;
            result.SpecialEventId = location._adoxioSpecialeventidValue;
            result.LocationDescription = location.AdoxioLocationdescription;
            result.EventLocationCity = location.AdoxioEventlocationcity;
            result.EventLocationPostalCode = location.AdoxioEventlocationpostalcode;
            result.EventLocationStreet1 = location.AdoxioEventlocationstreet1;
            result.EventLocationStreet2 = location.AdoxioEventlocationstreet2;
            result.EventLocationProvince = location.AdoxioLocationdescription;
            result.MaximumNumberOfGuests = location.AdoxioMaximumnumberofguests;
            result.LocationName = location.AdoxioLocationname;
            result.PermitNumber = location.AdoxioPermitnumber;

            // if (location.AdoxioSpecialeventlocationLicencedareas?.Count > 0)
            // {
            //     result.ServiceAreas = location.AdoxioSpecialeventlocationLicencedareas
            //     .Select(area => area.ToViewModel())
            //     .ToList();
            // }

            // if (location.AdoxioSpecialeventlocationSchedule?.Count > 0)
            // {
            //     result.EventDates = location.AdoxioSpecialeventlocationSchedule
            //     .Select(sched => sched.ToViewModel())
            //     .ToList();
            // }
            return result;
        }

        public static void CopyValues(this MicrosoftDynamicsCRMadoxioSpecialeventlocation to, ViewModels.SepEventLocation from)
        {
            if (from == null)
            {
                return;
            }

            to.AdoxioSpecialeventlocationid = from.LocationId;
            to._adoxioSpecialeventidValue = from.SpecialEventId;
            to.AdoxioLocationdescription = from.LocationDescription;
            to.AdoxioEventlocationcity = from.EventLocationCity;
            to.AdoxioEventlocationpostalcode = from.EventLocationPostalCode;
            to.AdoxioEventlocationstreet1 = from.EventLocationStreet1;
            to.AdoxioEventlocationstreet2 = from.EventLocationStreet2;
            to.AdoxioLocationdescription = from.EventLocationProvince;
            to.AdoxioMaximumnumberofguests = from.MaximumNumberOfGuests;
            to.AdoxioLocationname = from.LocationName;
            to.AdoxioPermitnumber = from.PermitNumber;
            // to.AdoxioMaxnumberofguests = from.MaxNumberOfGuests;
        }
    }
}

