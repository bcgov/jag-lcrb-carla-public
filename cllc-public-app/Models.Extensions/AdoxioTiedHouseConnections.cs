using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

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
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioTiedhouseconnection to, TiedHouseConnection from)
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
            to.AdoxioSocietyconnectionfederalproducer = from.SocietyConnectionFederalProducer;
            to.AdoxioSocietyconnectionfederalproducerdetails = from.SocietyConnectionFederalProducerDetails;

            to.AdoxioConnectiontype = (int?)from.ConnectionType;
            to.AdoxioCrsconnectiontomarketer = (int?)from.CrsConnectionToMarketer;
            to.AdoxioCrsconnectiontomarketerdetails = from.CrsConnectionToMarketerDetails;
            to.AdoxioMarketerconnectiontocrs = (int?)from.MarketerConnectionToCrs;
            to.AdoxioMarketerconnectiontocrsdetails = from.MarketerConnectionToCrsDetails;
            to.AdoxioInconnectiontofederalproducer = (int?)from.INConnectionToFederalProducer;
            to.AdoxioInconnectiontofederalproducerdetails = from.INConnectionToFederalProducerDetails;
            to.AdoxioLiquorfinancialinterest = @from.LiquorFinancialInterest;
            to.AdoxioLiquorfinancialinterestdetails = from.LiquorFinancialInterestDetails;
            to.AdoxioFirstName = from.FirstName;
            to.AdoxioMiddlename = from.LastName;
            to.AdoxioDateOfBirth = from.DateOfBirth;
            to.AdoxioLastname = from.LastName;
            to.AdoxioOtherRelationship = from.OtherRelationship;
            to.AdoxioRelationshipType = from.RelationshipToLicence;
            to.AdoxioLegalEntityReview = from.IsLegalEntity == true ? 1: 0;
            to.SupersededByOdataBind = !String.IsNullOrEmpty(from.SupersededById) ? $"/adoxio_tiedhouseconnections({from.SupersededById})": null;
        }


        /// <summary>
        /// Convert a Dynamics Legal Entity to a ViewModel
        /// </summary>        
        public static TiedHouseConnection ToViewModel(this MicrosoftDynamicsCRMadoxioTiedhouseconnection tiedHouse)
        {
            TiedHouseConnection result = null;
            if (tiedHouse != null)
            {
                result = new TiedHouseConnection();
                if (tiedHouse.AdoxioTiedhouseconnectionid != null)
                {
                    result.id = tiedHouse.AdoxioTiedhouseconnectionid;
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
                result.LiquorFinancialInterest = tiedHouse.AdoxioLiquorfinancialinterest;
                result.LiquorFinancialInterestDetails = tiedHouse.AdoxioLiquorfinancialinterestdetails;

                result.ConnectionType = (TiedHouseConnectionType?)tiedHouse.AdoxioConnectiontype;
                result.CrsConnectionToMarketer = (MarketerYesNo?)tiedHouse.AdoxioCrsconnectiontomarketer;
                result.CrsConnectionToMarketerDetails = tiedHouse.AdoxioCrsconnectiontomarketerdetails;
                result.MarketerConnectionToCrs = (MarketerYesNo?)tiedHouse.AdoxioMarketerconnectiontocrs;
                result.MarketerConnectionToCrsDetails = tiedHouse.AdoxioMarketerconnectiontocrsdetails;
                result.INConnectionToFederalProducer = (MarketerYesNo?)tiedHouse.AdoxioInconnectiontofederalproducer;
                result.INConnectionToFederalProducerDetails = tiedHouse.AdoxioInconnectiontofederalproducerdetails;
                result.FirstName = tiedHouse.AdoxioFirstName;
                result.MiddleName = tiedHouse.AdoxioMiddlename;
                result.LastName = tiedHouse.AdoxioLastname;
                result.RelationshipToLicence = tiedHouse.AdoxioRelationshipType;
                result.DateOfBirth = tiedHouse.AdoxioDateOfBirth;
                result.IsLegalEntity = tiedHouse.AdoxioLegalEntityReview == 1;
                result.ApplicationId = tiedHouse._adoxioApplicationValue;
                result.SupersededById = tiedHouse._adoxio_supersededbyValue;
                result.StatusCode = tiedHouse.Statuscode;

            }
            return result;
        }

    }

}
