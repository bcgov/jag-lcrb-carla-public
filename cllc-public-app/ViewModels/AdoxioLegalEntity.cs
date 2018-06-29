using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;



namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum Adoxio_applicanttypecodes
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
        Estate = 845280013,
        [Display(Name = "Local Government")]
        LocalGovernment = 845280014,
        University = 845280016
    }
    public enum Adoxio_accounttypecodes
    {
        Applicant = 845280000,
        Licensee = 2,
        Shareholder = 845280001,
        [Display(Name = "Police Jurisdiction")]
        PoliceJurisdiction = 0,
        Municipality = 1
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

    public class AdoxioLegalEntity
    {
        // string form of the guid.
        public string id { get; set; } //adoxio_legalentityid (primary key)
        public string accountId { get; set; } //_adoxio_account_value

        public string parentLegalEntityId { get; set; }
        public string name { get; set; } //adoxio_name (text)
        public bool? isindividual { get; set; } //adoxio_isindividual (option set)
        public bool? sameasapplyingperson { get; set; } //adoxio_sameasapplyingperson (option set)
        [JsonConverter(typeof(StringEnumConverter))]
        public Adoxio_applicanttypecodes legalentitytype { get; set; } //adoxio_legalentitytype (option set)
        public string otherlegalentitytype { get; set; } //adoxio_otherlegalentitytype (text)
        public string firstname { get; set; } //adoxio_firstname (text)
        public string middlename { get; set; } //adoxio_middlename (text)
        public string lastname { get; set; } //adoxio_lastname (text)
        [JsonConverter(typeof(StringEnumConverter))]
        public PositionOptions position; //adoxio_position (option set)
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

        //adoxio_contact (lookup contact)
        //adoxio_correspondingpersonalhistorysummary (lookup personal history summary)
        //adoxio_dateemailsent (date time)
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
    }
}
