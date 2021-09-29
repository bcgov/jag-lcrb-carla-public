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
                Id = location.AdoxioSpecialeventlocationid,
                SpecialEventId = location._adoxioSpecialeventidValue,
                LocationDescription = location.AdoxioLocationdescription,
                EventLocationCity = location.AdoxioEventlocationcity,
                EventLocationPostalCode = location.AdoxioEventlocationpostalcode,
                EventLocationStreet1 = location.AdoxioEventlocationstreet1,
                EventLocationStreet2 = location.AdoxioEventlocationstreet2,
                EventLocationProvince = location.AdoxioEventlocationprovince,
                MaximumNumberOfGuests = location.AdoxioMaximumnumberofguestslocation,
                LocationName = location.AdoxioLocationname,
                PermitNumber = location.AdoxioPermitnumber,
                NumberOfMinors = location.AdoxioNumberofminors
            };

            if (location.AdoxioSpecialeventlocationLicencedareas != null)
            {
                 result.ServiceAreas = location.AdoxioSpecialeventlocationLicencedareas
                 .Select(area => area.ToViewModel())
                 .ToList();
            }

            if (location.AdoxioSpecialeventlocationSchedule != null)
            { 
                result.EventDates = location.AdoxioSpecialeventlocationSchedule
                   .Select(sched => sched.ToViewModel())
                 .ToList();
            }
            return result;
        }

        public static void CopyValues(this MicrosoftDynamicsCRMadoxioSpecialeventlocation to, ViewModels.SepEventLocation from)
        {
            if (from == null)
            {
                return;
            }
            to.AdoxioEventlocationcity = from.EventLocationCity;
            to.AdoxioEventlocationpostalcode = from.EventLocationPostalCode.ToUpper();
            to.AdoxioEventlocationstreet1 = from.EventLocationStreet1;
            to.AdoxioEventlocationstreet2 = from.EventLocationStreet2;
            to.AdoxioLocationdescription = from.LocationDescription;
            to.AdoxioMaximumnumberofguestslocation = from.MaximumNumberOfGuests;
            to.AdoxioNumberofminors = from.NumberOfMinors;
            to.AdoxioLocationname = from.LocationName;
            to.AdoxioPermitnumber = from.PermitNumber;
        }
    }
}

