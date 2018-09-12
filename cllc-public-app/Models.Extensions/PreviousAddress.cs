using Gov.Lclb.Cllb.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class PreviousAddressExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.PreviousAddress ToViewModel(this MicrosoftDynamicsCRMadoxioPreviousaddress address)
        {
            ViewModels.PreviousAddress result = null;
            if (address != null)
            {
                result = new ViewModels.PreviousAddress();
                if (address.AdoxioPreviousaddressid != null)
                {
                    result.id = address.AdoxioPreviousaddressid;
                }

                result.name = address.AdoxioName;
                result.streetaddress = address.AdoxioStreetaddress;
                result.city = address.AdoxioCity;
                result.provstate = address.AdoxioProvstate;
                result.country = address.AdoxioCountry;
                result.postalcode = address.AdoxioPostalcode;
                result.fromdate = address.AdoxioFromdate;
                result.todate = address.AdoxioTodate;
            }
            return result;
        }


        public static void CopyValues(this MicrosoftDynamicsCRMadoxioPreviousaddress to, ViewModels.PreviousAddress from)
        {
            to.AdoxioName = from.name;
            to.AdoxioStreetaddress = from.streetaddress;
            to.AdoxioCity = from.city;
            to.AdoxioProvstate = from.provstate;
            to.AdoxioCountry = from.country;
            to.AdoxioPostalcode = from.postalcode;
            to.AdoxioFromdate = from.fromdate;
            to.AdoxioTodate = from.todate;
        }
    }
}
