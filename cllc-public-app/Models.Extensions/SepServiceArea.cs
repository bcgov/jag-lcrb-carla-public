using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Models;

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
        public static ViewModels.SepServiceArea ToViewModel(this MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea serviceArea)
        {
            ViewModels.SepServiceArea result = null;
            if (serviceArea != null)
            {
                result = new ViewModels.SepServiceArea()
                {
                    Id = serviceArea.AdoxioSpecialeventlicencedareaid,
                    SepLocationId = serviceArea._adoxioSpecialeventlocationidValue,
                    SpecialEventId = serviceArea._adoxioSpecialeventidValue,
                    MinorPresent = serviceArea.AdoxioMinorpresent,
                    LicencedAreaMaxNumberOfGuests = serviceArea.AdoxioLicencedareamaxnumberofguests,
                    MaximumNumberOfGuests = serviceArea.AdoxioMaximumnumberofguests,
                    IsBothOutdoorIndoor = serviceArea.AdoxioIsbothoutdoorindoor,
                    IsIndoors = serviceArea.AdoxioIsindoors,
                    NumberOfMinors = serviceArea.AdoxioNumberofminors,
                    LicencedAreaNumberOfMinors = serviceArea.AdoxioLicencedareanumberofminors,
                    Setting = serviceArea.AdoxioSetting,
                    StatusCode = serviceArea.Statecode,
                    StateCode = serviceArea.Statecode,
                    EventName = serviceArea.AdoxioEventname,
                    IsOutdoors = serviceArea.AdoxioIsoutdoors,
                    LicencedAreaDescription = serviceArea.AdoxioLicencedareadescription,
                };

            }
            return result;
        }

        public static void CopyValues(this MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea to, ViewModels.SepServiceArea from)
        {
            to.AdoxioSpecialeventlicencedareaid = from.Id;
            to._adoxioSpecialeventlocationidValue = from.SepLocationId;
            to._adoxioSpecialeventidValue = from.SpecialEventId;
            to.AdoxioMinorpresent = from.MinorPresent;
            to.AdoxioLicencedareamaxnumberofguests = from.LicencedAreaMaxNumberOfGuests;
            to.AdoxioMaximumnumberofguests = from.MaximumNumberOfGuests;
            to.AdoxioIsbothoutdoorindoor = from.IsBothOutdoorIndoor;
            to.AdoxioIsindoors = from.IsIndoors;
            to.AdoxioNumberofminors = from.NumberOfMinors;
            to.AdoxioLicencedareanumberofminors = from.LicencedAreaNumberOfMinors;
            to.AdoxioSetting = from.Setting;
            to.Statecode = from.StatusCode;
            to.Statecode = from.StateCode;
            to.AdoxioEventname = from.EventName;
            to.AdoxioIsoutdoors = from.IsOutdoors;
            to.AdoxioLicencedareadescription = from.LicencedAreaDescription;
        }

    }
}

