extern alias DV;
using System.Collections.Generic;
using System.Linq;
using DvLocation = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialeventlocation;

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

        public static ViewModels.SepEventLocation ToViewModel(this DvLocation location)
        {
            if (location == null) return null;
            return new ViewModels.SepEventLocation
            {
                Id = location.adoxio_specialeventlocationId?.ToString(),
                SpecialEventId = location.adoxio_SpecialEventId?.Id.ToString(),
                LocationDescription = location.adoxio_LocationDescription,
                EventLocationCity = location.adoxio_EventLocationCity,
                EventLocationPostalCode = location.adoxio_EventLocationPostalCode,
                EventLocationStreet1 = location.adoxio_EventLocationStreet1,
                EventLocationStreet2 = location.adoxio_EventLocationStreet2,
                EventLocationProvince = location.adoxio_EventLocationProvince,
                MaximumNumberOfGuests = location.adoxio_MaximumNumberofGuestsLocation,
                LocationName = location.adoxio_locationname,
                PermitNumber = location.adoxio_PermitNumber,
                NumberOfMinors = location.adoxio_NumberofMinors,
                ServiceAreas = new List<ViewModels.SepServiceArea>(),
                EventDates = new List<ViewModels.SepEventDates>(),
            };
        }

        public static void CopyValues(this DvLocation to, ViewModels.SepEventLocation from)
        {
            if (from == null) return;
            to.adoxio_EventLocationCity = from.EventLocationCity;
            to.adoxio_EventLocationPostalCode = from.EventLocationPostalCode?.ToUpper();
            to.adoxio_EventLocationStreet1 = from.EventLocationStreet1;
            to.adoxio_EventLocationStreet2 = from.EventLocationStreet2;
            to.adoxio_LocationDescription = from.LocationDescription;
            to.adoxio_MaximumNumberofGuestsLocation = from.MaximumNumberOfGuests;
            to.adoxio_NumberofMinors = from.NumberOfMinors;
            to.adoxio_locationname = from.LocationName;
            to.adoxio_PermitNumber = from.PermitNumber;
        }
    }
}

