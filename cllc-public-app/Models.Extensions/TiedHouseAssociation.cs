using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class TiedhouseAssociationExtensions
    {


        /// <summary>
        /// Copy values from View Model to Dynamics model
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioTiedhouseassociation to, ViewModels.TiedHouseAssociation from)
        {
            to.AdoxioTiedhouseassociationid = from.Id;
            to.AdoxioBusinessname = from.Businessname;
            to.AdoxioCity = from.City;
            to.AdoxioCountry = from.Country;
            to.AdoxioName = from.Name;
            to.AdoxioPostalcode = from.Postalcode;
            to.AdoxioProvince = from.Province;
            to.AdoxioStreet1 = from.Street1;
            to.AdoxioStreet2 = from.Street2;
        }


        /// <summary>
        /// Convert a TiedHouseAssociation to a ViewModel
        /// </summary>        
        public static ViewModels.TiedHouseAssociation ToViewModel(this MicrosoftDynamicsCRMadoxioTiedhouseassociation association)
        {
            ViewModels.TiedHouseAssociation result = null;
            if (association != null)
            {
                result = new ViewModels.TiedHouseAssociation();
                if (association.AdoxioTiedhouseassociationid != null)
                {
                    result.Id = association.AdoxioTiedhouseassociationid.ToString();
                }

                result.Id = association.AdoxioTiedhouseassociationid;
                result.Businessname = association.AdoxioBusinessname;
                result.City = association.AdoxioCity;
                result.Country = association.AdoxioCountry;
                result.Name = association.AdoxioName;
                result.Postalcode = association.AdoxioPostalcode;
                result.Province = association.AdoxioProvince;
                result.Street1 = association.AdoxioStreet1;
                result.Street2 = association.AdoxioStreet2;

                var tiedHouse = association.AdoxioTiedhouseassociationAdoxioTiedhouseconnectionTiedHouse.FirstOrDefault();
                if(tiedHouse != null){
                    result.TiedHouse = tiedHouse.ToViewModel();
                }

            }
            return result;
        }

    }

}
