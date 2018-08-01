using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.Public.ViewModels {
    public class TiedHouseConnection {
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
        public string TiedHouse { get; set; } //adoxio_TiedHouse (LookupType)
        public string TiedHouseName { get; set; } //adoxio_TiedHouseName (StringType)

        public bool isConnectionToProducersComplete (AdoxioApplicantTypeCodes? legalentitytype) {
            var isComplete = false;
            switch (legalentitytype) {
                case AdoxioApplicantTypeCodes.PublicCorporation:
                    isComplete =
                        (CorpConnectionFederalProducer == 0 ||
                            (CorpConnectionFederalProducer == 1 && !String.IsNullOrEmpty (CorpConnectionFederalProducerDetails))) &&
                        (FederalProducerConnectionToCorp == 0 ||
                            (FederalProducerConnectionToCorp == 1 && !String.IsNullOrEmpty (FederalProducerConnectionToCorpDetails))) &&
                        (Share20PlusConnectionProducer == 0 ||
                            (Share20PlusConnectionProducer == 1 && !String.IsNullOrEmpty (Share20PlusConnectionProducerDetails))) &&
                        (Share20PlusFamilyConnectionProducer == 0 ||
                            (Share20PlusFamilyConnectionProducer == 1 && !String.IsNullOrEmpty (Share20PlusFamilyConnectionProducerDetail)));
                    break;
                case AdoxioApplicantTypeCodes.PrivateCorporation:
                case AdoxioApplicantTypeCodes.UnlimitedLiabilityCorporation:
                case AdoxioApplicantTypeCodes.LimitedLiabilityCorporation:
                    isComplete =
                        (CorpConnectionFederalProducer == 0 ||
                            (CorpConnectionFederalProducer == 1 && !String.IsNullOrEmpty (CorpConnectionFederalProducerDetails))) &&
                        (FederalProducerConnectionToCorp == 0 ||
                            (FederalProducerConnectionToCorp == 1 && !String.IsNullOrEmpty (FederalProducerConnectionToCorpDetails)));
                    break;
                case AdoxioApplicantTypeCodes.Society:
                    isComplete =
                        (SocietyConnectionFederalProducer == 0 ||
                            (SocietyConnectionFederalProducer == 1 && !String.IsNullOrEmpty (SocietyConnectionFederalProducerDetails)));
                    break;
                case AdoxioApplicantTypeCodes.SoleProprietor:
                    isComplete = true;
                    break;
                case AdoxioApplicantTypeCodes.GeneralPartnership:
                case AdoxioApplicantTypeCodes.LimitedLiabilityPartnership:
                case AdoxioApplicantTypeCodes.LimitedPartnership:
                    isComplete =
                        (PartnersConnectionFederalProducer == 0 ||
                            (PartnersConnectionFederalProducer == 1 && !String.IsNullOrEmpty (PartnersConnectionFederalProducerDetails)));
                    break;
                default:
                    isComplete = false;
                    break;
            }

            return isComplete;

        }
    }
}