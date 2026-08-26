extern alias DV;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Public.Utils;
using DvArea = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialeventlicencedarea;
using DvAreaSetting = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialeventlicencedarea_adoxio_setting;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class SepServiceAreExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>

        public static ViewModels.SepServiceArea ToViewModel(this DvArea area)
        {
            if (area == null) return null;
            return new ViewModels.SepServiceArea
            {
                Id = area.adoxio_specialeventlicencedareaId?.ToString(),
                LocationId = area.adoxio_SpecialEventLocationId?.Id.ToString(),
                SpecialEventId = area.adoxio_SpecialEventId?.Id.ToString(),
                MinorPresent = area.adoxio_MinorPresent,
                LicencedAreaMaxNumberOfGuests = area.adoxio_LicencedAreaMaxNumberofGuests,
                NumberOfMinors = area.adoxio_LicencedAreaNumberofMinors,
                LicencedAreaNumberOfMinors = area.adoxio_LicencedAreaNumberofMinors,
                Setting = (ViewModels.ServiceAreaSetting?)(int?)area.adoxio_setting,
                EventName = area.adoxio_EventName,
                LicencedAreaDescription = area.adoxio_LicencedAreaDescription,
            };
        }

        public static void CopyValues(this DvArea to, ViewModels.SepServiceArea from)
        {
            if (from == null) return;
            to.adoxio_MinorPresent = from.MinorPresent;
            to.adoxio_LicencedAreaMaxNumberofGuests = from.LicencedAreaMaxNumberOfGuests;
            to.adoxio_LicencedAreaNumberofMinors = from.NumberOfMinors;
            to.adoxio_setting = (DvAreaSetting?)(int?)from.Setting;
            to.adoxio_EventName = StringUtility.Truncate(from.EventName, 255);
            to.adoxio_LicencedAreaDescription = from.LicencedAreaDescription;
        }

    }
}

