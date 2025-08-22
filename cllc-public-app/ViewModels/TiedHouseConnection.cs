using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum MarketerYesNo
    {
        Yes = 845280000,
        No = 845280001
    }

    public enum TiedHouseConnectionType
    {
        Marketer = 845280006
    }

    public enum TiedHouseStatusCode
    {
        New = 1,
        Ready = 845280000,
        Existing = 845280001,
        Inactive = 2
    }

    public enum TiedHouseCategoryType
    {
        Liquor = 845280000,
        Cannabis = 845280001
    }

    public class TiedHouseConnection
    {
        public string id { get; set; } //adoxio_tiedhouseconnectionId (primary key)
        public int? CorpConnectionFederalProducer { get; set; } //adoxio_corpconnectionfederalproducer (PicklistType)
        public string CorpConnectionFederalProducerDetails { get; set; } //adoxio_corpconnectionfederalproducerdetails (MemoType)
        public int? FamilyMemberFederalProducer { get; set; } //adoxio_familymemberfederalproducer (PicklistType)
        public string FamilyMemberFederalProducerDetails { get; set; } //adoxio_familymemberfederalproducerdetails (MemoType)
        public int? FederalProducerConnectionToCorp { get; set; } //adoxio_federalproducerconnectiontocorp (PicklistType)
        public string FederalProducerConnectionToCorpDetails { get; set; } //adoxio_federalproducerconnectiontocorpdetails (MemoType)
        public int? IsConnection { get; set; } //adoxio_IsConnection (PicklistType)
        public string Name { get; set; } //adoxio_name (StringType)
        public string OwnershipType { get; set; } //adoxio_OwnershipType (PicklistType)
        public int? PartnersConnectionFederalProducer { get; set; } //adoxio_partnersconnectionfederalproducer (PicklistType)
        public string PartnersConnectionFederalProducerDetails { get; set; } //adoxio_partnersconnectionfederalproducerdetails (MemoType)
        public int? PercentageofOwnership { get; set; } //adoxio_PercentageofOwnership (IntegerType)
        public int? Share20PlusConnectionProducer { get; set; } //adoxio_share20plusconnectionproducer (PicklistType)
        public string Share20PlusConnectionProducerDetails { get; set; } //adoxio_share20plusconnectionproducerdetails (MemoType)
        public int? Share20PlusFamilyConnectionProducer { get; set; } //adoxio_share20plusfamilyconnectionproducer (PicklistType)
        public string Share20PlusFamilyConnectionProducerDetail { get; set; } //adoxio_share20plusfamilyconnectionproducerdetail (MemoType)
        public string ShareType { get; set; } //adoxio_ShareType (StringType)
        public int? SocietyConnectionFederalProducer { get; set; } //adoxio_societyconnectionfederalproducer (PicklistType)
        public string SocietyConnectionFederalProducerDetails { get; set; } //adoxio_societyconnectionfederalproducerdetails (MemoType)
        public int? LiquorFinancialInterest { get; set; }
        public string LiquorFinancialInterestDetails { get; set; }

        // TODO: Is this field still used? If not, remove it.
        public string TiedHouse { get; set; } //adoxio_TiedHouse (LookupType)

        // TODO: Is this field still used? If not, remove it.
        public string TiedHouseName { get; set; } //adoxio_TiedHouseName (StringType)
        public string ApplicationId { get; set; }
        public string AccountId { get; set; }

        public int LIQTiedHouseType { get; set; }

        public DateTimeOffset? DateOfBirth { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public int? RelationshipToLicence { get; set; }

        public List<RelatedLicence> AssociatedLiquorLicense { get; set; }

        public string LegalEntityName { get; set; }

        public string RelationshipToLicense { get; set; }

        public string OtherDescription { get; set; }

        public int? BusinessType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TiedHouseConnectionType? ConnectionType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MarketerYesNo? CrsConnectionToMarketer { get; set; }

        public string CrsConnectionToMarketerDetails { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MarketerYesNo? MarketerConnectionToCrs { get; set; }

        public string MarketerConnectionToCrsDetails { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "iNConnectionToFederalProducer")]
        public MarketerYesNo? INConnectionToFederalProducer { get; set; }

        [JsonProperty(PropertyName = "iNConnectionToFederalProducerDetails")]
        public string INConnectionToFederalProducerDetails { get; set; }
        public string OtherRelationship { get; set; }

        public string SupersededById { get; set; }

        public int? StatusCode { get; set; }
        public bool? MarkedForRemoval { get; set; }

        public int? CategoryType { get; set; }

        public DateTimeOffset? DeclarationDate { get; set; }
        public int? SelfDeclared { get; set; }

        public bool isConnectionToProducersComplete(AdoxioApplicantTypeCodes? legalentitytype)
        {
            var isComplete = false;
            switch (legalentitytype)
            {
                case AdoxioApplicantTypeCodes.PublicCorporation:
                    isComplete =
                        (
                            CorpConnectionFederalProducer == 0
                            || (
                                CorpConnectionFederalProducer == 1
                                && !String.IsNullOrEmpty(CorpConnectionFederalProducerDetails)
                            )
                        )
                        && (
                            FederalProducerConnectionToCorp == 0
                            || (
                                FederalProducerConnectionToCorp == 1
                                && !String.IsNullOrEmpty(FederalProducerConnectionToCorpDetails)
                            )
                        )
                        && (
                            Share20PlusConnectionProducer == 0
                            || (
                                Share20PlusConnectionProducer == 1
                                && !String.IsNullOrEmpty(Share20PlusConnectionProducerDetails)
                            )
                        )
                        && (
                            Share20PlusFamilyConnectionProducer == 0
                            || (
                                Share20PlusFamilyConnectionProducer == 1
                                && !String.IsNullOrEmpty(Share20PlusFamilyConnectionProducerDetail)
                            )
                        );
                    break;
                case AdoxioApplicantTypeCodes.PrivateCorporation:
                case AdoxioApplicantTypeCodes.UnlimitedLiabilityCorporation:
                case AdoxioApplicantTypeCodes.LimitedLiabilityCorporation:
                    isComplete =
                        (
                            CorpConnectionFederalProducer == 0
                            || (
                                CorpConnectionFederalProducer == 1
                                && !String.IsNullOrEmpty(CorpConnectionFederalProducerDetails)
                            )
                        )
                        && (
                            FederalProducerConnectionToCorp == 0
                            || (
                                FederalProducerConnectionToCorp == 1
                                && !String.IsNullOrEmpty(FederalProducerConnectionToCorpDetails)
                            )
                        );
                    break;
                case AdoxioApplicantTypeCodes.Society:
                    isComplete = (
                        SocietyConnectionFederalProducer == 0
                        || (
                            SocietyConnectionFederalProducer == 1
                            && !String.IsNullOrEmpty(SocietyConnectionFederalProducerDetails)
                        )
                    );
                    break;
                case AdoxioApplicantTypeCodes.SoleProprietorship:
                    isComplete = true;
                    break;
                case AdoxioApplicantTypeCodes.GeneralPartnership:
                case AdoxioApplicantTypeCodes.LimitedLiabilityPartnership:
                case AdoxioApplicantTypeCodes.LimitedPartnership:
                    isComplete = (
                        PartnersConnectionFederalProducer == 0
                        || (
                            PartnersConnectionFederalProducer == 1
                            && !String.IsNullOrEmpty(PartnersConnectionFederalProducerDetails)
                        )
                    );
                    break;
                default:
                    isComplete = false;
                    break;
            }

            return isComplete;
        }
    }
}
