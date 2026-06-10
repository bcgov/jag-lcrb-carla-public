extern alias DV;
using System;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;

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

        // ---- Xrm.Sdk adoxio_previousaddress extensions ----

        public static ViewModels.PreviousAddress ToViewModel(this adoxio_previousaddress address)
        {
            if (address == null) return null;
            return new ViewModels.PreviousAddress
            {
                id = address.Id.ToString(),
                name = address.adoxio_name,
                streetaddress = address.adoxio_StreetAddress,
                city = address.adoxio_City,
                provstate = address.adoxio_PROVSTATE,
                country = address.adoxio_Country,
                postalcode = address.adoxio_PostalCode,
                fromdate = address.adoxio_FromDate.HasValue ? (DateTimeOffset?)address.adoxio_FromDate.Value : null,
                todate = address.adoxio_ToDate.HasValue ? (DateTimeOffset?)address.adoxio_ToDate.Value : null,
                contactId = address.adoxio_ContactId?.Id.ToString(),
                workerId = address.adoxio_WorkerId?.Id.ToString(),
            };
        }

        public static void CopyValues(this adoxio_previousaddress to, ViewModels.PreviousAddress from)
        {
            to.adoxio_name = from.name;
            to.adoxio_StreetAddress = from.streetaddress;
            to.adoxio_City = from.city;
            to.adoxio_PROVSTATE = from.provstate;
            to.adoxio_Country = from.country;
            to.adoxio_PostalCode = from.postalcode;
            to.adoxio_FromDate = from.fromdate?.UtcDateTime;
            to.adoxio_ToDate = from.todate?.UtcDateTime;
        }
    }
}
