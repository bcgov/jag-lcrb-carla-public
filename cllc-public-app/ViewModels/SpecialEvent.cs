using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public enum LicensedSEPLocationValue
    {
        Yes = 845280000,
        No = 845280001,
        NotSure = 845280002,
    }

    public enum ChargingForLiquorReasons
    {
        RecoverCost = 845280000,
        RaiseMoney = 845280001,
        LiquorIsFree = 845280002,
        Combination = 845280003,
    }
    public enum SEPPublicOrPrivate
    {
        Members = 845280000,
        Family = 845280001,
        Hobbyist = 845280002,
        Anyone = 845280003,
    }

    public enum DonatedOrConsular
    {
        No = 845280001,
        Yes = 845280000
    }

    public enum EventStatus
    {
        Draft = 845280001,
        Submitted = 845280002,
        Cancelled = 845280004,
        [EnumMember(Value = "Pending Review")]
        PendingReview = 100000000,
        Approved = 1,
        Issued = 845280003,
        Denied = 845280000
    }

    public enum ApproverStatus
    {
        [EnumMember(Value = "Auto-Reviewed")]
        AutoReviewed = 100000000,
        Reviewed = 845280000,
        [EnumMember(Value = "Pending Review")]
        PendingReview = 100000001,
        Approved = 845280000,
        Denied = 845280001,
        Cancelled = 845280002
    }


    public enum HostOrgCatergory
    {
        IncorporatedNonProfitOrganization = 845280000,
        UnincorporatedNonProfitOrganization = 845280001,
        IncorporatedBusinessOrPartnership = 845280002,
        GovernmentOrPublicOrganization = 845280003,
    }
    public enum FundRaisingPurposes
    {
        ReliefOfPoverty = 845280000,
        AdvancementOfEducation = 845280001,
        ReligiousPurposes = 845280002,
        Recreation = 845280003,
        SportsOrAthletics = 845280004,
        AidToTheDisabledAndOrHandicapped = 845280005,
        AdvancementOfCulture = 845280006,
        BenefitToYouthOrSeniorCitizens = 845280007,
        OtherPurposeBeneficialToTheCommunity = 845280007,
    }
    public class SpecialEvent
    {
        public string Id { get; set; }
        public int LocalId { get; set; } //client side local db id
        public bool? AdmissionFee { get; set; }
        public System.DateTimeOffset? EventStartDate { get; set; }
        public System.DateTimeOffset? EventEndDate { get; set; }
        public string EventName { get; set; }

        public bool? IsAgreeToTnC { get; set; }

        public decimal? TotalRevenue { get; set; }
        public decimal? TotalPurchaseCost { get; set; }
        public decimal? TotalProceeds { get; set; }
        public System.DateTimeOffset? DateAgreeToTnC { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FundRaisingPurposes? FundRaisingPurpose { get; set; }
        public string SepCityId { get; set; }  // not used
        public string SpecialEventPostalCode { get; set; }
        public int? Statecode { get; set; }
        public bool? BeerGarden { get; set; }
        public bool? TastingEvent { get; set; }
        public int? TotalServings { get; set; }
        public bool? InvoiceTrigger { get; set; }
        public string SpecialEventProvince { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SEPPublicOrPrivate? PrivateOrPublic { get; set; }
        public string ResponsibleBevServiceNumber { get; set; }
        public bool? ResponsibleBevServiceNumberDoesNotHave { get; set; }
        public string SpecialEventDescription { get; set; }
        public int? Capacity { get; set; }
        public decimal? NetEstimatedPST { get; set; }
        public bool? IsAgreeTsAndCs { get; set; }
        public bool? IsPrivateResidence { get; set; }
        public bool? IsOnPublicProperty { get; set; }
        public bool? IsLocalSignificance { get; set; }
        public DateTimeOffset? DateAgreedToTsAndCs { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ChargingForLiquorReasons? ChargingForLiquorReason { get; set; }
        public bool? DrinksIncluded { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DonatedOrConsular? DonatedOrConsular { get; set; }
        public bool? IsAnnualEvent { get; set; }
        public string SpecialEventPermitNumber { get; set; }
        public string SpecialEventCity { get; set; } // use SepCity instead
        public string SpecialEventStreet2 { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EventStatus? EventStatus { get; set; }

        public bool? IsMajorSignificance { get; set; }
        public bool? IsGstRegisteredOrg { get; set; }
        public string MajorSignificanceRationale { get; set; }
        public string HostOrganizationName { get; set; }
        public string HostOrganizationAddress { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public HostOrgCatergory? HostOrganizationCategory { get; set; }
        public string SpecialEventStreet1 { get; set; }
        public int? MaximumNumberOfGuests
        {
            get
            {
                return EventLocations?.Sum(loc => loc.MaximumNumberOfGuests);
            }
        }
        public string NonProfitName { get; set; }
        public System.DateTimeOffset? DateSubmitted { get; set; }
        public ViewModels.Account PoliceAccount { get; set; }
        public ViewModels.Contact PoliceDecisionBy { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ApproverStatus? PoliceApproval { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ApproverStatus? LcrbApproval { get; set; }

        public ViewModels.Contact LcrbApprovalBy { get; set; }
        public string DenialReason { get; set; }
        public string CancelReason { get; set; }
        public bool? IsManufacturingExclusivity { get; set; }
        public string HowProceedsWillBeUsedDescription { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LicensedSEPLocationValue? IsLocationLicensed { get; set; }
        // list of SEP locations
        public List<SepEventLocation> EventLocations { get; set; }
        public List<SepDrinksSalesForecast> DrinksSalesForecasts { get; set; }

        public SepCity SepCity { get; set; }

        public Contact Applicant { get; set; }
        public Invoice Invoice { get; set; }

        public int? Beer { get; set; }
        public int? Wine { get; set; }
        public int? Spirits { get; set; }

    }

    public class ItemsToDelete
    {
        public List<string> Locations { get; set; } = new List<string>();
        public List<string> EventDates { get; set; } = new List<string>();
        public List<string> ServiceAreas { get; set; } = new List<string>();
    }
}
