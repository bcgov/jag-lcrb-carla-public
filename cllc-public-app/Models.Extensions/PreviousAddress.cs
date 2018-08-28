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

                result.adoxio_name = address.AdoxioName;
                result.adoxio_streetaddress = address.AdoxioStreetaddress;
                result.adoxio_city = address.AdoxioCity;
                result.adoxio_provstate = address.AdoxioProvstate;
                result.adoxio_country = address.AdoxioCountry;
                result.adoxio_postalcode = address.AdoxioPostalcode;
                result.adoxio_fromdate = (DateTime) address.AdoxioFromdate;
                result.adoxio_todate = (DateTime) address.AdoxioTodate;
            }
            return result;
        }


        public static void CopyValues(this MicrosoftDynamicsCRMadoxioPreviousaddress to, ViewModels.PreviousAddress from)
        {
            to.AdoxioName = from.adoxio_name;
            to.AdoxioStreetaddress = from.adoxio_streetaddress;
            to.AdoxioCity = from.adoxio_city;
            to.AdoxioProvstate = from.adoxio_provstate;
            to.AdoxioCountry = from.adoxio_country;
            to.AdoxioPostalcode = from.adoxio_postalcode;
            to.AdoxioFromdate = from.adoxio_fromdate;
            to.AdoxioTodate = from.adoxio_todate;
        }
    }
}
