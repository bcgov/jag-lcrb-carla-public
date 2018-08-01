using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum AdoxioApplicantTypeCodes
    {
        [Display(Name = "Private Corporation")]
        PrivateCorporation = 845280000,
        [Display(Name = "Public Corporation")]
        PublicCorporation = 845280003,
        [Display(Name = "Unlimited Liability Corporation")]
        UnlimitedLiabilityCorporation = 845280007,
        [Display(Name = "Limited Liability Corporation")]
        LimitedLiabilityCorporation = 845280006,
        [Display(Name = "General Partnership")]
        GeneralPartnership = 845280001,
        [Display(Name = "Limited Partnership")]
        LimitedPartnership = 845280008,
        [Display(Name = "Limited Liability Partnership")]
        LimitedLiabilityPartnership = 845280009,
        Society = 845280004,
        [Display(Name = "Sole Proprietor")]
        SoleProprietor = 845280002,
        [Display(Name = "Indigenous Nation")]
        IndigenousNation = 845280010,
        [Display(Name = "Co-op")]
        Coop = 845280011,
        Trust = 845280012,
        Estate = 845280013, [Display(Name = "Local Government")]
        LocalGovernment = 845280014,
        University = 845280016,
        Partnership = 845280005,

    }
    public enum AdoxioAccountTypeCodes
    {
        Applicant = 845280000,
        Licensee = 2,
        Shareholder = 845280001,
        Partner = 845280002,
        [Display(Name = "Police Jurisdiction")]
        PoliceJurisdiction = 0,
        Municipality = 1
    }

    public enum AdoxioPartnerType
    {
        General = 845280000,
        Limited = 845280001,
    }

    public enum PositionOptions
    {
        Partner,
        Shareholder,
        Trustee,
        Director,
        Officer,
        Owner
    }

    public enum GeneralYesNo
    {
        No = 0,
        Yes = 1
    }

    public class BusinessProfileCompletion
    {
        public bool isCorporateDetailsComplete { get; set; }
        public bool isConnectionsToProducersComplete { get; set; }
        public bool isOrganizationStructureComplete { get; set; }
        public bool isDirectorsAndOfficersComplete { get; set; }
        public bool isKeyPersonnelComplete { get; set; }
        public bool isShareholdersComplete { get; set; }
        public bool isFinanceIntegrityComplete { get; set; }
        public bool isSecurityAssessmentComplete { get; set; }
    }
    public class AdoxioLegalEntity
    {
        // string form of the guid.
        public string id { get; set; } //adoxio_legalentityid (primary key)
        public string accountId { get; set; } //_adoxio_account_value
        public string shareholderAccountId { get; set; } //_adoxio_shareholderaccountid_value

        public string parentLegalEntityId { get; set; }
        public string name { get; set; } //adoxio_name (text)

        public bool isApplicant { get; set; } // adoxio_isapplicant
        public bool? isindividual { get; set; } //adoxio_isindividual (option set)
        public bool? sameasapplyingperson { get; set; } //adoxio_sameasapplyingperson (option set)

        [JsonConverter(typeof(StringEnumConverter))]
        public AdoxioApplicantTypeCodes? legalentitytype { get; set; } //adoxio_legalentitytype (option set)

        [JsonConverter(typeof(StringEnumConverter))]
        public AdoxioPartnerType? partnerType { get; set; } //adoxio_legalentitytype (option set)
        public string otherlegalentitytype { get; set; } //adoxio_otherlegalentitytype (text)
        public string firstname { get; set; } //adoxio_firstname (text)
        public string middlename { get; set; } //adoxio_middlename (text)
        public string lastname { get; set; } //adoxio_lastname (text)
        //[JsonConverter(typeof(StringEnumConverter))]
        //public PositionOptions position; //adoxio_position (option set)
        public bool? isOfficer { get; set; }
        public bool? isDirector { get; set; }
        public bool? isSeniorManagement { get; set; }
        public bool? isShareholder { get; set; }
        public bool? isPartner { get; set; }
        public bool? isOwner { get; set; }

        public DateTimeOffset? dateofbirth { get; set; } //adoxio_dateofbirth (date time)
        public decimal? interestpercentage { get; set; } //adoxio_interestpercentage (decimal number)
        public int? commonvotingshares { get; set; } //adoxio_commonvotingshares (whole number)
        public int? preferredvotingshares { get; set; } //adoxio_preferredvotingshares (whole number)
        public int? commonnonvotingshares { get; set; } //adoxio_commonnonvotingshares (whole number)
        public int? preferrednonvotingshares { get; set; } //adoxio_preferrednonvotingshares (whole number)
        public Account account { get; set; } //adoxio_account (lookup account)
        List<AdoxioLegalEntity> relatedentities { get; set; }
        public string email { get; set; } //adoxio_email
        public DateTimeOffset? dateofappointment { get; set; } //adoxio_dateofappointment (date time)
        public DateTimeOffset? securityAssessmentEmailSentOn { get; set; } //adoxio_dateemailsent (date time)

        //adoxio_contact (lookup contact)
        //adoxio_correspondingpersonalhistorysummary (lookup personal history summary)
        //adoxio_dateofsharesissued (date time)
        //adoxio_incorporationdate (date time)
        //adoxio_instructionsoninsertform ???
        //adoxio_isapplicant (two options yes/no)
        //adoxio_isdirector (two options yes/no)
        //adoxio_isofficer (two options yes/no)
        //adoxio_isowner (two options yes/no)
        //adoxio_ispartner (two options yes/no)
        //adoxio_isseniormanagement (two options yes/no)
        //adoxio_isshareholder (two options yes/no)
        //adoxio_istrustee (two options yes/no)
        //adoxio_legalentityowned (lookup legal entity)
        //adoxio_partnertype (option set)
        //adoxio_relatedapplication (lookup application)
        //adoxio_relatedlicence (lookup licence)
        //adoxio_sameastheapplyingperson (two options yes/no)
        //adoxio_shareholderaccountid (lookup account)
        //adoxio_sharepointanchor (text)
        //adoxio_totalshares (whole number)
        public bool isShareholderComplete(AdoxioApplicantTypeCodes? businessType, bool shareholderFilesExists, bool shareholdersExist, bool partnersExist)
        {
            var isComplete = false;
            switch (businessType)
            {
                case AdoxioApplicantTypeCodes.PrivateCorporation:
                    isComplete =
                        (
                            isindividual == true &&
                            !String.IsNullOrEmpty(firstname) &&
                            !String.IsNullOrEmpty(lastname) &&
                            !String.IsNullOrEmpty(email) &&
                            commonvotingshares != null
                        ) || (
                            isindividual != true &&
                            legalentitytype != null &&
                            !String.IsNullOrEmpty(name) &&
                            commonvotingshares != null
                        );
                    isComplete = isComplete && shareholderFilesExists;
                    isComplete = isComplete && shareholdersExist;
                    break;
                case AdoxioApplicantTypeCodes.PublicCorporation:
                    isComplete = shareholderFilesExists;
                    break;
                case AdoxioApplicantTypeCodes.Society:
                case AdoxioApplicantTypeCodes.SoleProprietor:
                    isComplete = true;
                    break;
                case AdoxioApplicantTypeCodes.UnlimitedLiabilityCorporation:
                case AdoxioApplicantTypeCodes.LimitedLiabilityCorporation:
                    isComplete =
                        (
                            isindividual == true &&
                            !String.IsNullOrEmpty(firstname) &&
                            !String.IsNullOrEmpty(lastname) &&
                            !String.IsNullOrEmpty(email) &&
                            commonvotingshares != null //&&
                                                       // dateIssued != null
                        ) || (
                            isindividual != true &&
                            legalentitytype != null &&
                            !String.IsNullOrEmpty(name) &&
                            commonvotingshares != null &&
                            commonnonvotingshares != null //&& 
                                                          // dateIssued != null
                        );
                    isComplete = isComplete && shareholderFilesExists;
                    isComplete = isComplete && shareholdersExist;
                    break;
                case AdoxioApplicantTypeCodes.GeneralPartnership:
                case AdoxioApplicantTypeCodes.LimitedLiabilityPartnership:
                case AdoxioApplicantTypeCodes.LimitedPartnership:
                    isComplete =
                        (
                            isindividual == true &&
                            !String.IsNullOrEmpty(firstname) &&
                            !String.IsNullOrEmpty(lastname) &&
                            !String.IsNullOrEmpty(email)
                        ) || (
                            isindividual != true &&
                            legalentitytype != null &&
                            !String.IsNullOrEmpty(name)
                        );
                    isComplete = isComplete && partnersExist;
                    break;
                default:
                    isComplete = false;
                    break;
            }

            return isComplete;

        }
        public bool isDirectorOfficerComplete(AdoxioApplicantTypeCodes? businessType, bool directorsExist)
        {
            var isComplete = false;
            switch (businessType)
            {
                case AdoxioApplicantTypeCodes.PrivateCorporation:
                case AdoxioApplicantTypeCodes.PublicCorporation:
                case AdoxioApplicantTypeCodes.UnlimitedLiabilityCorporation:
                case AdoxioApplicantTypeCodes.LimitedLiabilityCorporation:
                    isComplete =
                        (
                            isindividual == true &&
                            !String.IsNullOrEmpty(firstname) &&
                            !String.IsNullOrEmpty(lastname) &&
                            !String.IsNullOrEmpty(email) &&
                            (isDirector == true || isOfficer == true)//&& 
                                                                     // dateIssued != null
                        );
                    isComplete = isComplete && directorsExist;
                    break;
                case AdoxioApplicantTypeCodes.Society:
                    isComplete =
                        (
                            isindividual == true &&
                            !String.IsNullOrEmpty(firstname) &&
                            !String.IsNullOrEmpty(lastname) &&
                            !String.IsNullOrEmpty(email) &&
                            (isDirector == true || isOfficer == true || isSeniorManagement == true)//&& 
                                                                                                   // dateIssued != null
                        );
                    isComplete = isComplete && directorsExist;
                    break;
                case AdoxioApplicantTypeCodes.SoleProprietor:
                    isComplete =
                        (
                            isindividual == true &&
                            !String.IsNullOrEmpty(firstname) &&
                            !String.IsNullOrEmpty(lastname) &&
                            !String.IsNullOrEmpty(email) // && 
                                                         // dateIssued != null
                        );
                    isComplete = isComplete && directorsExist;
                    break;
                case AdoxioApplicantTypeCodes.GeneralPartnership:
                case AdoxioApplicantTypeCodes.LimitedLiabilityPartnership:
                case AdoxioApplicantTypeCodes.LimitedPartnership:
                    isComplete = true;
                    break;
                default:
                    isComplete = false;
                    break;
            }
            return isComplete;

        }
    }

}