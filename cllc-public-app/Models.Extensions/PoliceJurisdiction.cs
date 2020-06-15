using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class PoliceDurisdictionExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.PoliceJurisdiction ToViewModel(this MicrosoftDynamicsCRMadoxioPolicejurisdiction item)
        {
            ViewModels.PoliceJurisdiction result = null;
            if (item != null)
            {
                result = new ViewModels.PoliceJurisdiction();
                if (item.AdoxioPolicejurisdictionid != null)
                {
                    result.id = item.AdoxioPolicejurisdictionid;
                }

                result.name = item.AdoxioName;
            }
            return result;
        }
    }
}
