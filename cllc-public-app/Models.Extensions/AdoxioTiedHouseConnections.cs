using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class AdoxioTiedhouseconnectionsExtensions
    {


        /// <summary>
        /// Copy values from a Dynamics legal entity to another one
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioTiedhouseconnection to, ViewModels.TiedHouseConnection from)
        {
            to.AdoxioTiedhouseconnectionid = from.id;
            to.AdoxioCorpconnectionfederalproducer = from.CorpConnectionFederalProducer;
            to.AdoxioCorpconnectionfederalproducerdetails = from.CorpConnectionFederalProducerDetails;
            to.AdoxioFamilymemberfederalproducer = from.FamilyMemberFederalProducer;
            to.AdoxioFamilymemberfederalproducerdetails = from.FamilyMemberFederalProducerDetails;
            to.AdoxioFederalproducerconnectiontocorp = from.FederalProducerConnectionToCorp;
            to.AdoxioFederalproducerconnectiontocorpdetails = from.FederalProducerConnectionToCorpDetails;
            to.AdoxioIsconnection = from.IsConnection;
            to.AdoxioPartnersconnectionfederalproducer = from.PartnersConnectionFederalProducer;
            to.AdoxioPartnersconnectionfederalproducerdetails = from.PartnersConnectionFederalProducerDetails;
            to.AdoxioPercentageofownership = from.PercentageofOwnership;
            to.AdoxioShare20plusconnectionproducer = from.Share20PlusConnectionProducer;
            to.AdoxioShare20plusconnectionproducerdetails = from.Share20PlusConnectionProducerDetails;
            to.AdoxioShare20plusfamilyconnectionproducer = from.Share20PlusFamilyConnectionProducer;
            to.AdoxioShare20plusfamilyconnectionproducerdetail = from.Share20PlusFamilyConnectionProducerDetail;
            to.AdoxioSharetype = from.ShareType;
            to.AdoxioSocietyconnectionfederalproducer = from.SocietyConnectionFederalProducer;
            to.AdoxioSocietyconnectionfederalproducerdetails = from.SocietyConnectionFederalProducerDetails;

        }


        /// <summary>
        /// Convert a Dynamics Legal Entity to a ViewModel
        /// </summary>        
        public static ViewModels.TiedHouseConnection ToViewModel(this MicrosoftDynamicsCRMadoxioTiedhouseconnection tiedHouse)
        {
            ViewModels.TiedHouseConnection result = null;
            if (tiedHouse != null)
            {
                result = new ViewModels.TiedHouseConnection();
                if (tiedHouse.AdoxioTiedhouseconnectionid != null)
                {
                    result.id = tiedHouse.AdoxioTiedhouseconnectionid.ToString();
                }

                result.CorpConnectionFederalProducer = tiedHouse.AdoxioCorpconnectionfederalproducer;
                result.CorpConnectionFederalProducerDetails = tiedHouse.AdoxioCorpconnectionfederalproducerdetails;
                result.FamilyMemberFederalProducer = tiedHouse.AdoxioFamilymemberfederalproducer;
                result.FamilyMemberFederalProducerDetails = tiedHouse.AdoxioFamilymemberfederalproducerdetails;
                result.FederalProducerConnectionToCorp = tiedHouse.AdoxioFederalproducerconnectiontocorp;
                result.FederalProducerConnectionToCorpDetails = tiedHouse.AdoxioFederalproducerconnectiontocorpdetails;
                result.IsConnection = tiedHouse.AdoxioIsconnection;
                result.PartnersConnectionFederalProducer = tiedHouse.AdoxioPartnersconnectionfederalproducer;
                result.PartnersConnectionFederalProducerDetails = tiedHouse.AdoxioPartnersconnectionfederalproducerdetails;
                result.PercentageofOwnership = tiedHouse.AdoxioPercentageofownership;
                result.Share20PlusConnectionProducer = tiedHouse.AdoxioShare20plusconnectionproducer;
                result.Share20PlusConnectionProducerDetails = tiedHouse.AdoxioShare20plusconnectionproducerdetails;
                result.Share20PlusFamilyConnectionProducer = tiedHouse.AdoxioShare20plusfamilyconnectionproducer;
                result.Share20PlusFamilyConnectionProducerDetail = tiedHouse.AdoxioShare20plusfamilyconnectionproducerdetail;
                result.ShareType = tiedHouse.AdoxioSharetype;
                result.SocietyConnectionFederalProducer = tiedHouse.AdoxioSocietyconnectionfederalproducer;
                result.SocietyConnectionFederalProducerDetails = tiedHouse.AdoxioSocietyconnectionfederalproducerdetails;

            }
            return result;
        }

    }

}
