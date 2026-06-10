extern alias DV;
using System;
using System.Collections.Generic;
using System.Linq;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.Xrm.Sdk;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// Dataverse SDK extension methods for adoxio_tiedhouseconnection.
    /// Replaces the AutoRest-based MicrosoftDynamicsCRMadoxioTiedhouseconnection extensions.
    /// </summary>
    public static class AdoxioTiedHouseConnectionDataverseExtensions
    {
        public static void CopyValues(this adoxio_tiedhouseconnection to, TiedHouseConnection from)
        {
            to.adoxio_corpconnectionfederalproducer = (adoxio_generalyesno?)from.CorpConnectionFederalProducer;
            to.adoxio_corpconnectionfederalproducerdetails = from.CorpConnectionFederalProducerDetails;
            to.adoxio_familymemberfederalproducer = (adoxio_generalyesno?)from.FamilyMemberFederalProducer;
            to.adoxio_familymemberfederalproducerdetails = from.FamilyMemberFederalProducerDetails;
            to.adoxio_federalproducerconnectiontocorp = (adoxio_generalyesno?)from.FederalProducerConnectionToCorp;
            to.adoxio_federalproducerconnectiontocorpdetails = from.FederalProducerConnectionToCorpDetails;
            to.adoxio_IsConnection = (adoxio_generalyesno?)from.IsConnection;
            to.adoxio_partnersconnectionfederalproducer = (adoxio_generalyesno?)from.PartnersConnectionFederalProducer;
            to.adoxio_partnersconnectionfederalproducerdetails = from.PartnersConnectionFederalProducerDetails;
            to.adoxio_PercentageofOwnership = from.PercentageofOwnership;
            to.adoxio_share20plusconnectionproducer = (adoxio_generalyesno?)from.Share20PlusConnectionProducer;
            to.adoxio_share20plusconnectionproducerdetails = from.Share20PlusConnectionProducerDetails;
            to.adoxio_share20plusfamilyconnectionproducer = (adoxio_generalyesno?)from.Share20PlusFamilyConnectionProducer;
            to.adoxio_share20plusfamilyconnectionproducerdetail = from.Share20PlusFamilyConnectionProducerDetail;
            to.adoxio_ShareType = from.ShareType;
            to.adoxio_societyconnectionfederalproducer = (adoxio_generalyesno?)from.SocietyConnectionFederalProducer;
            to.adoxio_societyconnectionfederalproducerdetails = from.SocietyConnectionFederalProducerDetails;
            to.adoxio_ConnectionType = (adoxio_tiedhouseconnection_adoxio_connectiontype?)(int?)from.ConnectionType;
            to.adoxio_CRSConnectiontoMarketer = (adoxio_tiedhouseconnection_adoxio_crsconnectiontomarketer?)(int?)from.CrsConnectionToMarketer;
            to.adoxio_CRSConnectiontoMarketerDetails = from.CrsConnectionToMarketerDetails;
            to.adoxio_MarketerConnectiontoCRS = (adoxio_tiedhouseconnection_adoxio_marketerconnectiontocrs?)(int?)from.MarketerConnectionToCrs;
            to.adoxio_MarketerConnectiontoCRSDetails = from.MarketerConnectionToCrsDetails;
            to.adoxio_INConnectiontoFederalProducer = (adoxio_tiedhouseconnection_adoxio_inconnectiontofederalproducer?)(int?)from.INConnectionToFederalProducer;
            to.adoxio_INConnectiontoFederalProducerDetails = from.INConnectionToFederalProducerDetails;
            to.adoxio_liquorfinancialinterest = (adoxio_generalyesno?)from.LiquorFinancialInterest;
            to.adoxio_liquorfinancialinterestdetails = from.LiquorFinancialInterestDetails;
            to.adoxio_FirstName = from.FirstName;
            to.adoxio_MiddleName = from.MiddleName;
            to.adoxio_DateOfBirth = from.DateOfBirth?.DateTime;
            to.adoxio_LastName = from.LastName;
            to.adoxio_OtherRelationship = from.OtherDescription;
            to.adoxio_RelationshipType = (adoxio_tiedhouse?)(int?)from.RelationshipToLicence;
            to.adoxio_LIQTiedHouseType = (adoxio_tiedhouseconnection_adoxio_liqtiedhousetype?)(int?)from.LIQTiedHouseType;
            to.adoxio_MarkedForRemoval = from.MarkedForRemoval == true ? adoxio_generalyesno.Yes : adoxio_generalyesno.No;
            to.adoxio_LegalEntityName = from.LegalEntityName;
            to.adoxio_BusinessType = (adoxio_applicanttypecodes?)(int?)from.BusinessType;
            to.adoxio_CategoryType = (adoxio_tiedhouseconnection_adoxio_categorytype?)(int?)from.CategoryType;
            to.adoxio_SelfDeclared = (adoxio_generalyesno?)from.SelfDeclared;
            to.adoxio_DeclarationDate = from.DeclarationDate?.DateTime;

            if (!string.IsNullOrEmpty(from.AccountId) && Guid.TryParse(from.AccountId, out var accountGuid))
                to.adoxio_AccountId = new EntityReference("account", accountGuid);

            if (!string.IsNullOrEmpty(from.ApplicationId) && Guid.TryParse(from.ApplicationId, out var appGuid))
                to.adoxio_Application = new EntityReference("adoxio_application", appGuid);

            if (!string.IsNullOrEmpty(from.SupersededById) && Guid.TryParse(from.SupersededById, out var supersededGuid))
                to.adoxio_SupersededBy = new EntityReference("adoxio_tiedhouseconnection", supersededGuid);
        }

        public static TiedHouseConnection ToViewModel(this adoxio_tiedhouseconnection thc, IList<adoxio_licences> licences = null)
        {
            if (thc == null) return null;

            return new TiedHouseConnection
            {
                id = thc.adoxio_tiedhouseconnectionId?.ToString(),
                AssociatedLiquorLicense = licences?.Select(l => new RelatedLicence
                {
                    Id = l.Id.ToString(),
                    Name = l.adoxio_name
                }).ToList(),
                CorpConnectionFederalProducer = (int?)thc.adoxio_corpconnectionfederalproducer,
                CorpConnectionFederalProducerDetails = thc.adoxio_corpconnectionfederalproducerdetails,
                FamilyMemberFederalProducer = (int?)thc.adoxio_familymemberfederalproducer,
                FamilyMemberFederalProducerDetails = thc.adoxio_familymemberfederalproducerdetails,
                FederalProducerConnectionToCorp = (int?)thc.adoxio_federalproducerconnectiontocorp,
                FederalProducerConnectionToCorpDetails = thc.adoxio_federalproducerconnectiontocorpdetails,
                IsConnection = (int?)thc.adoxio_IsConnection,
                PartnersConnectionFederalProducer = (int?)thc.adoxio_partnersconnectionfederalproducer,
                PartnersConnectionFederalProducerDetails = thc.adoxio_partnersconnectionfederalproducerdetails,
                PercentageofOwnership = thc.adoxio_PercentageofOwnership,
                Share20PlusConnectionProducer = (int?)thc.adoxio_share20plusconnectionproducer,
                Share20PlusConnectionProducerDetails = thc.adoxio_share20plusconnectionproducerdetails,
                Share20PlusFamilyConnectionProducer = (int?)thc.adoxio_share20plusfamilyconnectionproducer,
                Share20PlusFamilyConnectionProducerDetail = thc.adoxio_share20plusfamilyconnectionproducerdetail,
                ShareType = thc.adoxio_ShareType,
                SocietyConnectionFederalProducer = (int?)thc.adoxio_societyconnectionfederalproducer,
                SocietyConnectionFederalProducerDetails = thc.adoxio_societyconnectionfederalproducerdetails,
                LiquorFinancialInterest = (int?)thc.adoxio_liquorfinancialinterest,
                LiquorFinancialInterestDetails = thc.adoxio_liquorfinancialinterestdetails,
                ConnectionType = (TiedHouseConnectionType?)(int?)thc.adoxio_ConnectionType,
                CrsConnectionToMarketer = (MarketerYesNo?)(int?)thc.adoxio_CRSConnectiontoMarketer,
                CrsConnectionToMarketerDetails = thc.adoxio_CRSConnectiontoMarketerDetails,
                MarketerConnectionToCrs = (MarketerYesNo?)(int?)thc.adoxio_MarketerConnectiontoCRS,
                MarketerConnectionToCrsDetails = thc.adoxio_MarketerConnectiontoCRSDetails,
                INConnectionToFederalProducer = (MarketerYesNo?)(int?)thc.adoxio_INConnectiontoFederalProducer,
                INConnectionToFederalProducerDetails = thc.adoxio_INConnectiontoFederalProducerDetails,
                FirstName = thc.adoxio_FirstName,
                MiddleName = thc.adoxio_MiddleName,
                LastName = thc.adoxio_LastName,
                RelationshipToLicence = (int?)thc.adoxio_RelationshipType,
                DateOfBirth = thc.adoxio_DateOfBirth.HasValue ? (DateTimeOffset?)thc.adoxio_DateOfBirth.Value : null,
                LIQTiedHouseType = (int?)thc.adoxio_LIQTiedHouseType,
                ApplicationId = thc.adoxio_Application?.Id.ToString(),
                AccountId = thc.adoxio_AccountId?.Id.ToString(),
                SupersededById = thc.adoxio_SupersededBy?.Id.ToString(),
                StatusCode = (int?)thc.statuscode,
                MarkedForRemoval = thc.adoxio_MarkedForRemoval == adoxio_generalyesno.Yes,
                BusinessType = (int?)thc.adoxio_BusinessType,
                LegalEntityName = thc.adoxio_LegalEntityName,
                CategoryType = (int?)thc.adoxio_CategoryType,
                SelfDeclared = (int?)thc.adoxio_SelfDeclared,
                DeclarationDate = thc.adoxio_DeclarationDate.HasValue ? (DateTimeOffset?)thc.adoxio_DeclarationDate.Value : null,
                OtherDescription = thc.adoxio_OtherRelationship
            };
        }
    }
}
