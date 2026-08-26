extern alias DV;
using System.Collections.Generic;
using System.Linq;
using DvCity = DV::Gov.Lclb.Cllb.Interfaces.adoxio_sepcity;

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
        public static ViewModels.SepCity ToViewModel(this DvCity city)
        {
            if (city == null) return null;
            return new ViewModels.SepCity
            {
                Id = city.adoxio_sepcityId?.ToString(),
                Name = city.adoxio_name,
                IsPreview = city.adoxio_IsPreview,
                PoliceJurisdictionName = city.adoxio_PoliceJurisdictionId?.Name,
                LGINName = city.adoxio_LGINId?.Name,
            };
        }
    }
}

