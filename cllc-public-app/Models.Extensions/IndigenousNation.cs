using Gov.Lclb.Cllb.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static ViewModels.IndigenousNation ToViewModel(this MicrosoftDynamicsCRMadoxioLocalgovindigenousnation item)
        {
            ViewModels.IndigenousNation result = null;
            if (item != null)
            {
                result = new ViewModels.IndigenousNation();
                if (item.AdoxioLocalgovindigenousnationid != null)
                {
                    result.id = item.AdoxioLocalgovindigenousnationid;
                }

                result.name = item.AdoxioName;
            }
            return result;
        }
    }
}
