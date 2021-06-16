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
                    LocationId = serviceArea._adoxioSpecialeventlocationidValue,
                    SpecialEventId = serviceArea._adoxioSpecialeventidValue,
                    MinorPresent = serviceArea.AdoxioMinorpresent,
                    LicencedAreaMaxNumberOfGuests = serviceArea.AdoxioLicencedareamaxnumberofguests,
                    NumberOfMinors = serviceArea.AdoxioLicencedareanumberofminors,
                    LicencedAreaNumberOfMinors = serviceArea.AdoxioLicencedareanumberofminors,
                    Setting = (ViewModels.ServiceAreaSetting?)serviceArea.AdoxioSetting,
                    StatusCode = serviceArea.Statecode,
                    StateCode = serviceArea.Statecode,
                    EventName = serviceArea.AdoxioEventname,
                    LicencedAreaDescription = serviceArea.AdoxioLicencedareadescription,
                };

            }
            return result;
        }

        public static void CopyValues(this MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea to, ViewModels.SepServiceArea from)
        {
            to.AdoxioMinorpresent = from.MinorPresent;
            to.AdoxioLicencedareamaxnumberofguests = from.LicencedAreaMaxNumberOfGuests;
            to.AdoxioLicencedareanumberofminors = from.NumberOfMinors;
            to.AdoxioSetting = (int?)from.Setting;
            to.Statecode = from.StatusCode;
            to.Statecode = from.StateCode;
            to.AdoxioEventname = from.EventName;
            to.AdoxioLicencedareadescription = from.LicencedAreaDescription;
        }

    }
}

