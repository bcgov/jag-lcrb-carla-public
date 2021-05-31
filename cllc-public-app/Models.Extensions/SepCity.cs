using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class SepCityExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>
        public static ViewModels.SepCity ToViewModel(this MicrosoftDynamicsCRMadoxioSepcity sepCity)
        {
            ViewModels.SepCity result = null;
            if (sepCity != null)
            {
                result = new ViewModels.SepCity()
                {
                    Id = sepCity.AdoxioSepcityid,
                    Name = sepCity.AdoxioName,
                    IsPreview = sepCity.AdoxioIspreview,
                    PoliceJurisdictionName = sepCity.AdoxioPoliceJurisdictionId?.AdoxioName,
                    LGINName = sepCity.AdoxioLGINId?.AdoxioName
                };

            }
            return result;
        }


    }
}

