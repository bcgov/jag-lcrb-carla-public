using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

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
            //to.adoxio_corpconnectionfederalproducer = from.CorpConnectionFederalProducer;
            //to.adoxio_corpconnectionfederalproducerdetails = from.CorpConnectionFederalProducerDetails;
            //to.adoxio_familymemberfederalproducer = from.FamilyMemberFederalProducer;
            //to.adoxio_familymemberfederalproducerdetails = from.FamilyMemberFederalProducerDetails;
            //to.adoxio_federalproducerconnectiontocorp = from.FederalProducerConnectionToCorp;
            //to.adoxio_federalproducerconnectiontocorpdetails = from.FederalProducerConnectionToCorpDetails;
            //to.adoxio_IsConnection = from.IsConnection;
            //to.adoxio_partnersconnectionfederalproducer = from.PartnersConnectionFederalProducer;
            //to.adoxio_partnersconnectionfederalproducerdetails = from.PartnersConnectionFederalProducerDetails;
            //to.adoxio_PercentageofOwnership = from.PercentageofOwnership;
            //to.adoxio_share20plusconnectionproducer = from.Share20PlusConnectionProducer;
            //to.adoxio_share20plusconnectionproducerdetails = from.Share20PlusConnectionProducerDetails;
            //to.adoxio_share20plusfamilyconnectionproducer = from.Share20PlusFamilyConnectionProducer;
            //to.adoxio_share20plusfamilyconnectionproducerdetail = from.Share20PlusFamilyConnectionProducerDetail;
            //to.adoxio_ShareType = from.ShareType;
            //to.adoxio_societyconnectionfederalproducer = from.SocietyConnectionFederalProducer;
            //to.adoxio_societyconnectionfederalproducerdetails = from.SocietyConnectionFederalProducerDetails;

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

                // result.CorpConnectionFederalProducer = tiedHouse.adoxio_corpconnectionfederalproducer;
                // result.CorpConnectionFederalProducerDetails = tiedHouse.adoxio_corpconnectionfederalproducerdetails;
                // result.FamilyMemberFederalProducer = tiedHouse.adoxio_familymemberfederalproducer;
                // result.FamilyMemberFederalProducerDetails = tiedHouse.adoxio_familymemberfederalproducerdetails;
                // result.FederalProducerConnectionToCorp = tiedHouse.adoxio_federalproducerconnectiontocorp;
                // result.FederalProducerConnectionToCorpDetails = tiedHouse.adoxio_federalproducerconnectiontocorpdetails;
                // result.IsConnection = tiedHouse.adoxio_IsConnection;
                // result.PartnersConnectionFederalProducer = tiedHouse.adoxio_partnersconnectionfederalproducer;
                // result.PartnersConnectionFederalProducerDetails = tiedHouse.adoxio_partnersconnectionfederalproducerdetails;
                // result.PercentageofOwnership = tiedHouse.adoxio_PercentageofOwnership;
                // result.Share20PlusConnectionProducer = tiedHouse.adoxio_share20plusconnectionproducer;
                // result.Share20PlusConnectionProducerDetails = tiedHouse.adoxio_share20plusconnectionproducerdetails;
                // result.Share20PlusFamilyConnectionProducer = tiedHouse.adoxio_share20plusfamilyconnectionproducer;
                // result.Share20PlusFamilyConnectionProducerDetail = tiedHouse.adoxio_share20plusfamilyconnectionproducerdetail;
                // result.ShareType = tiedHouse.adoxio_ShareType;
                // result.SocietyConnectionFederalProducer = tiedHouse.adoxio_societyconnectionfederalproducer;
                // result.SocietyConnectionFederalProducerDetails = tiedHouse.adoxio_societyconnectionfederalproducerdetails;

            }
            return result;
        }

    }

}
