extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class IndigenousNationExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.IndigenousNation ToViewModel(this adoxio_localgovindigenousnation item)
        {
            if (item == null) return null;
            return new ViewModels.IndigenousNation
            {
                Id = item.adoxio_localgovindigenousnationId?.ToString(),
                Name = item.adoxio_name
            };
        }
    }
}
