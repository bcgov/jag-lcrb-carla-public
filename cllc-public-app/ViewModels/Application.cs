using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum AdoxioApplicationStatusCodes
    {
        Active = 1,
        Cancelled = 2,
        [EnumMember(Value = "In progress")]
        InProgress = 845280000,
        Intake = 845280001,
        Incomplete = 845280002,
        [EnumMember(Value = "Under Review")]
        UnderReview = 845280003,
        Approved = 845280004,
        [EnumMember(Value = "Pending for LG/FN/Police Feedback")]
        PendingForLGFNPFeedback = 845280006,
        Refused = 845280005,
        [EnumMember(Value = "Pending for Licence Fee")]
        PendingForLicenceFee = 845280007,
        [EnumMember(Value = "Pending Final Inspection")]
        PendingFinalInspection = 845280008,
        [EnumMember(Value = "Submitted")]
        Submitted = 845280013,        
        Terminated = 845280009,
        [EnumMember(Value = "Terminated and Refunded")]
        TerminatedAndRefunded = 845280010,

        Processed = 845280011,
        [EnumMember(Value = "Reviewing Inspection Results")]
        ReviewingInspectionResults = 845280012,

        [EnumMember(Value = "Application Assessment")]
        ApplicationAssessment = 845280014
    }

    public enum AdoxioFinalDecisionCodes
    {
        [EnumMember(Value = null)]
        Unknown = 0,
        Approved = 845280000,
        Denied = 845280001
    }

    public enum ServiceHours
    {
        [EnumMember(Value = "00:00")]
        sh0000 = 845280000,

        [EnumMember(Value = "00:15")]
        sh0015 = 845280024,

        [EnumMember(Value = "00:30")]
        sh0030 = 845280025,

        [EnumMember(Value = "00:45")]
        sh0045 = 845280026,

        [EnumMember(Value = "01:00")]
        sh0100 = 845280001,

        [EnumMember(Value = "01:15")]
        sh0115 = 845280027,

        [EnumMember(Value = "01:30")]
        sh0130 = 845280028,

        [EnumMember(Value = "01:45")]
        sh0145 = 845280029,

        [EnumMember(Value = "02:00")]
        sh0200 = 845280002,

        [EnumMember(Value = "02:15")]
        sh0215 = 845280030,

        [EnumMember(Value = "02:30")]
        sh0230 = 845280031,

        [EnumMember(Value = "02:45")]
        sh0245 = 845280032,

        [EnumMember(Value = "03:00")]
        sh0300 = 845280003,

        [EnumMember(Value = "03:15")]
        sh0315 = 845280033,

        [EnumMember(Value = "03:30")]
        sh0330 = 845280034,

        [EnumMember(Value = "03:45")]
        sh0345 = 845280035,

        [EnumMember(Value = "04:00")]
        sh0400 = 845280004,

        [EnumMember(Value = "04:15")]
        sh0415 = 845280036,

        [EnumMember(Value = "04:30")]
        sh0430 = 845280037,

        [EnumMember(Value = "04:45")]
        sh0445 = 845280038,

        [EnumMember(Value = "05:00")]
        sh0500 = 845280005,

        [EnumMember(Value = "05:15")]
        sh0515 = 845280039,

        [EnumMember(Value = "05:30")]
        sh0530 = 845280040,

        [EnumMember(Value = "05:45")]
        sh0545 = 845280041,

        [EnumMember(Value = "06:00")]
        sh0600 = 845280006,

        [EnumMember(Value = "06:15")]
        sh0615 = 845280042,

        [EnumMember(Value = "06:30")]
        sh0630 = 845280043,

        [EnumMember(Value = "06:45")]
        sh0645 = 845280044,

        [EnumMember(Value = "07:00")]
        sh0700 = 845280007,

        [EnumMember(Value = "07:15")]
        sh0715 = 845280045,

        [EnumMember(Value = "07:30")]
        sh0730 = 845280046,

        [EnumMember(Value = "07:45")]
        sh0745 = 845280047,

        [EnumMember(Value = "08:00")]
        sh0800 = 845280008,

        [EnumMember(Value = "08:15")]
        sh0815 = 845280048,

        [EnumMember(Value = "08:30")]
        sh0830 = 845280049,

        [EnumMember(Value = "08:45")]
        sh0845 = 845280050,

        [EnumMember(Value = "09:00")]
        sh0900 = 845280009,

        [EnumMember(Value = "09:15")]
        sh0915 = 845280051,

        [EnumMember(Value = "09:30")]
        sh0930 = 845280052,

        [EnumMember(Value = "09:45")]
        sh0945 = 845280053,

        [EnumMember(Value = "10:00")]
        sh1000 = 845280010,

        [EnumMember(Value = "10:15")]
        sh1015 = 845280054,

        [EnumMember(Value = "10:30")]
        sh1030 = 845280055,

        [EnumMember(Value = "10:45")]
        sh1045 = 845280056,

        [EnumMember(Value = "11:00")]
        sh1100 = 845280011,

        [EnumMember(Value = "11:15")]
        sh1115 = 845280057,

        [EnumMember(Value = "11:30")]
        sh1130 = 845280058,

        [EnumMember(Value = "11:45")]
        sh1145 = 845280059,

        [EnumMember(Value = "12:00")]
        sh1200 = 845280012,

        [EnumMember(Value = "12:15")]
        sh1215 = 845280060,

        [EnumMember(Value = "12:30")]
        sh1230 = 845280061,

        [EnumMember(Value = "12:45")]
        sh1245 = 845280062,

        [EnumMember(Value = "13:00")]
        sh1300 = 845280013,

        [EnumMember(Value = "13:15")]
        sh1315 = 845280063,

        [EnumMember(Value = "13:30")]
        sh1330 = 845280064,

        [EnumMember(Value = "13:45")]
        sh1345 = 845280065,

        [EnumMember(Value = "14:00")]
        sh1400 = 845280014,

        [EnumMember(Value = "14:15")]
        sh1415 = 845280066,

        [EnumMember(Value = "14:30")]
        sh1430 = 845280067,

        [EnumMember(Value = "14:45")]
        sh1445 = 845280068,

        [EnumMember(Value = "15:00")]
        sh1500 = 845280015,

        [EnumMember(Value = "15:15")]
        sh1515 = 845280069,

        [EnumMember(Value = "15:30")]
        sh1530 = 845280070,

        [EnumMember(Value = "15:45")]
        sh1545 = 845280071,

        [EnumMember(Value = "16:00")]
        sh1600 = 845280016,

        [EnumMember(Value = "16:15")]
        sh1615 = 845280072,

        [EnumMember(Value = "16:30")]
        sh1630 = 845280073,

        [EnumMember(Value = "16:45")]
        sh1645 = 845280074,

        [EnumMember(Value = "17:00")]
        sh1700 = 845280017,

        [EnumMember(Value = "17:15")]
        sh1715 = 845280075,

        [EnumMember(Value = "17:30")]
        sh1730 = 845280076,

        [EnumMember(Value = "17:45")]
        sh1745 = 845280077,

        [EnumMember(Value = "18:00")]
        sh1800 = 845280018,

        [EnumMember(Value = "18:15")]
        sh1815 = 845280078,

        [EnumMember(Value = "18:30")]
        sh1830 = 845280079,

        [EnumMember(Value = "18:45")]
        sh1845 = 845280080,

        [EnumMember(Value = "19:00")]
        sh1900 = 845280019,

        [EnumMember(Value = "19:15")]
        sh1915 = 845280081,

        [EnumMember(Value = "19:30")]
        sh1930 = 845280082,

        [EnumMember(Value = "19:45")]
        sh1945 = 845280083,

        [EnumMember(Value = "20:00")]
        sh2000 = 845280020,

        [EnumMember(Value = "20:15")]
        sh2015 = 845280084,

        [EnumMember(Value = "20:30")]
        sh2030 = 845280085,

        [EnumMember(Value = "20:45")]
        sh2045 = 845280086,

        [EnumMember(Value = "21:00")]
        sh2100 = 845280021,

        [EnumMember(Value = "21:15")]
        sh2115 = 845280087,

        [EnumMember(Value = "21:30")]
        sh2130 = 845280088,

        [EnumMember(Value = "21:45")]
        sh2145 = 845280089,

        [EnumMember(Value = "22:00")]
        sh2200 = 845280022,

        [EnumMember(Value = "22:15")]
        sh2215 = 845280090,

        [EnumMember(Value = "22:30")]
        sh2230 = 845280091,

        [EnumMember(Value = "22:45")]
        sh2245 = 845280092,

        [EnumMember(Value = "23:00")]
        sh2300 = 845280023,

        [EnumMember(Value = "23:15")]
        sh2315 = 845280093,

        [EnumMember(Value = "23:30")]
        sh2330 = 845280094,

        [EnumMember(Value = "23:45")]
        sh2345 = 845280095,
        [EnumMember(Value = "Closed")]
        closed = 845280096
    }
    public enum ValueNotChanged
    {
        Yes = 845280000,
        No = 845280001
    }
    public enum Zoning
    {
        Allows = 845280000,
        DoesNotAllow = 845280001
    }

    public enum LGDecision
    {
        Approved = 845280000,
        OptOut = 845280001,
        Rejected = 845280002,
        Pending = 845280003
    }

    public enum PatioLocatedAbove
    {
        Grass = 845280000,
        Earth = 845280001,
        Gravel = 845280002,
        [EnumMember(Value = "Finished Flooring")]
        FinishedFlooring = 845280003,
        [EnumMember(Value = "Cement Sidewalk")]
        CementSidewalk = 845280004,
        Other = 845280005
    }

    public enum LicenceSubCategory
    {
        Winery = 845280000,
        Brewery = 845280001,
        Distillery = 845280002,
        [EnumMember(Value = "Co-Packer")]
        CoPacker = 845280003
    }

    public enum YesNoNotApplicable
    {
        Yes = 845280000,
        No = 845280001,
        [EnumMember(Value = "N/A")]
        NotApplicable = 845280002
    }

    public class Application
    {
        public string Id { get; set; } //adoxio_applicationid

        public string ParentApplicationId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AdoxioApplicationStatusCodes ApplicationStatus { get; set; } //statuscode

        [JsonConverter(typeof(StringEnumConverter))]
        public AdoxioApplicantTypeCodes ApplicantType { get; set; } //adoxio_applicanttype

        [JsonConverter(typeof(StringEnumConverter))]
        public AdoxioFinalDecisionCodes AppChecklistFinalDecision { get; set; } //adoxioFinaldecision

        public int? PreviousApplication { get; set; }
        public string PreviousApplicationDetails { get; set; }
        public int? RuralAgencyStoreAppointment { get; set; }
        public int? LiquorIndustryConnections { get; set; }
        public string LiquorIndustryConnectionsDetails { get; set; }
        public int? OtherBusinesses { get; set; }
        public string OtherBusinessesDetails { get; set; }


        public GeneralYesNo? InvoiceTrigger { get; set; } //adoxio_invoicetrigger
        public Account Applicant { get; set; }
        public License AssignedLicence { get; set; }
        public string AdditionalPropertyInformation { get; set; } //adoxio_additionalpropertyinformation
        public string InvoiceId;
        public string SecondaryInvoiceId;
        public string ApplyingPerson { get; set; } //_adoxio_applyingperson_value
        public bool? AuthorizedToSubmit { get; set; } //adoxio_authorizedtosubmit        

        public DateTimeOffset? CreatedOn { get; set; }
        public string ContactPersonEmail { get; set; } //adoxio_email
        public string ContactPersonFirstName { get; set; } //adoxio_contactpersonfirstname
        public string ContactPersonLastName { get; set; } //adoxio_contactpersonlastname
        public string ContactPersonPhone { get; set; } //adoxio_contactpersonphone
        public string ContactPersonRole { get; set; } //adoxio_role

        public string EstablishmentAddress { get; set; } //adoxio_establishmentaddress
        public string EstablishmentName { get; set; } //adoxio_establishmentpropsedname
        public string EstablishmentAddressCity { get; set; } //adoxio_establishmentaddresscity
        public string EstablishmentAddressPostalCode { get; set; } //adoxio_establishmentaddresspostalcode
        public string EstablishmentAddressStreet { get; set; } //adoxio_establishmentaddressstreet
        public string EstablishmentEmail { get; set; }
        public string EstablishmentParcelId { get; set; } //adoxio_establishmentparcelid
        public string EstablishmentPhone { get; set; }
        public string Pin { get; set; } //adoxio_pin

        public bool IsLocationChangeInProgress { get; set; }
        public bool IsPaid { get; set; }
        public bool IsSubmitted { get; set; }

        public string JobNumber { get; set; } //adoxio_jobnumber

        public string LicenseType { get; set; } //_adoxio_licencetype_value		
        public Invoice LicenceFeeInvoice { get; set; }
        public bool LicenceFeeInvoicePaid { get; set; }

        public DateTimeOffset? ModifiedOn { get; set; }

        public string Name { get; set; } //adoxio_name

        public DateTimeOffset? PaymentReceivedDate { get; set; }
        public bool PrevPaymentFailed { get; set; }

        public GeneralYesNo RegisteredEstablishment { get; set; } //adoxio_registeredestablishment
        public string ServiceHoursId { get; set; }

        public bool? ServicehHoursStandardHours { get; set; }
        public bool? SignatureAgreement { get; set; } //adoxio_signatureagreement  

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursSundayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursSundayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursMondayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursMondayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursTuesdayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursTuesdayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursWednesdayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursWednesdayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursThursdayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursThursdayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursFridayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursFridayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursSaturdayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursSaturdayClose { get; set; }

        public ApplicationType ApplicationType { get; set; }

        public TiedHouseConnection TiedHouse { get; set; }
        public PoliceJurisdiction PoliceJurisdiction { get; set; }
        public IndigenousNation IndigenousNation { get; set; }
        public ServiceHours ServiceHours { get; set; }


        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalCriminalOffenceCheck { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalUnreportedSaleOfBusiness { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalBusinessType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalTiedhouse { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? TiedhouseFederalInterest { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalOrgLeadership { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? Renewalkeypersonnel { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalShareholders { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalOutstandingFines { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalBranding { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalSignage { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalEstablishmentAddress { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalValidInterest { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalZoning { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalFloorPlan { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalSiteMap { get; set; }

        public string IndigenousNationId { get; set; }
        public string PoliceJurisdictionId { get; set; }
        public string FederalProducerNames { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalDUI { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueNotChanged? RenewalThirdParty { get; set; }


        public bool? IsMonth01 {get; set;}
        public bool? IsMonth02 {get; set;}
        public bool? IsMonth03 {get; set;}
        public bool? IsMonth04 {get; set;}
        public bool? IsMonth05 {get; set;}
        public bool? IsMonth06 {get; set;}
        public bool? IsMonth07 {get; set;}
        public bool? IsMonth08 {get; set;}
        public bool? IsMonth09 {get; set;}
        public bool? IsMonth10 {get; set;}
        public bool? IsMonth11 {get; set;}
        public bool? IsMonth12 {get; set;}

        public DateTimeOffset? Establishmentopeningdate { get; set; }
        public bool? IsReadyValidInterest { get; set; }
        public bool? IsReadyWorkers { get; set; }
        public bool? IsReadyNameBranding { get; set; }
        public bool? IsReadyDisplays { get; set; }
        public bool? IsReadyIntruderAlarm { get; set; }
        public bool? IsReadyFireAlarm { get; set; }
        public bool? IsReadyLockedCases { get; set; }
        public bool? IsReadyLockedStorage { get; set; }
        public bool? IsReadyPerimeter { get; set; }
        public bool? IsReadyRetailArea { get; set; }
        public bool? IsReadyStorage { get; set; }
        public bool? IsReadyExtranceExit { get; set; }
        public bool? IsReadySurveillanceNotice { get; set; }
        public bool? IsReadyProductNotVisibleOutside { get; set; }
        public bool? IsLocatedInGroceryStore { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public GeneralYesNo? IsApplicationComplete { get; set; }


        public string LgInName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "lGApprovalDecision")]
        public LGDecision? LGApprovalDecision { get; set; }

        [JsonProperty(PropertyName = "lGDecisionComments")]
        public string LGDecisionComments { get; set; }

        [JsonProperty(PropertyName = "lGNameOfOfficial")]
        public string LGNameOfOfficial { get; set; }

        [JsonProperty(PropertyName = "lGTitlePosition")]
        public string LGTitlePosition { get; set; }

        [JsonProperty(PropertyName = "lGContactPhone")]
        public string LGContactPhone { get; set; }

        [JsonProperty(PropertyName = "lGContactEmail")]
        public string LGContactEmail { get; set; }

        [JsonProperty(PropertyName = "lGDecisionSubmissionDate")]
        public DateTimeOffset? LGDecisionSubmissionDate { get; set; }

        public bool? LgNoObjection { get; set; }

        public DateTimeOffset? DateApplicantSentToLG { get; set; }


        public bool ResolutionDocsUploaded { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Zoning? LgZoning { get; set; }

        public List<CapacityArea> ServiceAreas { get; set; }
        public List<CapacityArea> OutsideAreas { get; set; }
        public List<CapacityArea> CapacityArea { get; set; }

        // Manufactuer

        public bool? IsPackaging { get; set; }
        public bool? IsPermittedInZoning { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public YesNoNotApplicable? MfgBrewPubOnSite { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public YesNoNotApplicable? MfgPipedInProduct { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public YesNoNotApplicable? MfgUsesNeutralGrainSpirits { get; set; }

        public int? ZoningStatus { get; set; }
        public bool? IsOwnerBusiness { get; set; }
        public bool? HasValidInterest { get; set; }
        public bool? IsHasPatio { get; set; }
        public bool? WillHaveValidInterest { get; set; }
        //public YesNoNotApplicable? InGroceryStore { get; set; }


        // these are just optional int - not picklist references.
        public int? MfgAcresOfFruit { get; set; }
        public int? MfgAcresOfGrapes { get; set; }
        public int? MfgAcresOfHoney { get; set; }
        public bool? MfgMeetsProductionMinimum { get; set; }
        public bool? MfgStepBlending { get; set; }
        public bool? MfgStepCrushing { get; set; }
        public bool? MfgStepFiltering { get; set; }
        public bool? MfgStepSecFermOrCarb { get; set; }





        // Manufactuer Structural Change - Patio
        public string PatioCompDescription { get; set; }
        public string PatioLocationDescription { get; set; }
        public string PatioAccessDescription { get; set; }
        public bool? PatioIsLiquorCarried { get; set; }
        public string PatioLiquorCarriedDescription { get; set; }
        public string PatioAccessControlDescription { get; set; }

        public int? LocatedAboveDescription { get; set; }
        public int? PatioServiceBar { get; set; }

        public string LicenceSubCategory { get; set; }

        public string PidList { get; set; }

        public bool IsAlr { get; set; }
        public bool HasCoolerAccess { get; set; }

        public string LocatedAboveOther { get; set; }

        public string FirstNameOld { get; set; }
        public string FirstNameNew { get; set; }
        public string LastNameOld { get; set; }
        public string LastNameNew { get; set; }
        public bool? CsInternalTransferOfShares { get; set; }
        public bool? CsExternalTransferOfShares { get; set; }
        public bool? CsChangeOfDirectorsOrOfficers { get; set; }
        public bool? CsNameChangeLicenseeCorporation { get; set; }
        public bool? CsNameChangeLicenseePartnership { get; set; }
        public bool? CsNameChangeLicenseeSociety { get; set; }
        public bool? CsNameChangeLicenseePerson { get; set; }
        public bool? CsAdditionalReceiverOrExecutor { get; set; }
        public bool? PrimaryInvoicePaid { get; set; }
        public bool? SecondaryInvoicePaid { get; set; }
        public bool? IsOnINLand { get; set; }

        public string TermConditionId { get; set; }
        public string TermConditionOriginalText { get; set; }

        public DateTimeOffset? TempDateFrom { get; set; }
        public DateTimeOffset? TempDateTo { get; set; }


        // LOCATION ELIGIBILITY

        // Note that many of these fields are "Yes / No" lookups.  As the fields are used in Dynamic Forms 
        // they are stored as integer lookup values rather than converted to boolean.

        public int? IsRlrsLocatedInRuralCommunityAlone { get; set; }

        public int? IsRlrsLocatedAtTouristDestinationAlone { get; set; }

        public string RlrsResortCommunityDescription { get; set; }

        public int? HasYearRoundAllWeatherRoadAccess { get; set; }

        public int? DoesGeneralStoreOperateSeasonally { get; set; }

        public int? SurroundingResidentsOfRlrs { get; set; }

        public int? IsRlrsAtLeast10kmFromAnotherStore { get; set; }

        public int? IsApplicantOwnerOfStore { get; set; }

        public string LegalAndBeneficialOwnersOfStore { get; set; }

        public int? IsApplicantFranchiseOrAffiliated { get; set; }

        public string FranchiseOrAffiliatedBusiness { get; set; }
        
        public int? HasSufficientRangeOfProducts { get; set; }

        public int? HasOtherProducts { get; set; }

        public int? HasAdditionalServices { get; set; }

        public DateTimeOffset? StoreOpenDate { get; set; }

        public int? ConfirmLiquorSalesIsNotPrimaryBusiness { get; set; }

        public string RelatedLicenceNumber { get; set; }
        public int? PicnicReadAndAccept { get; set; }
        public int? PicnicConfirmZoning { get; set; }
        public int? PicnicConfirmLGFNCapacity { get; set; }

        public int? ManufacturerProductionAmountForPrevYear { get; set; }
        public int? ManufacturerProductionAmountUnit { get; set; }

    }
}
