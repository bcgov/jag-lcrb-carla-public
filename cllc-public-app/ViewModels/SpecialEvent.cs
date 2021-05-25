using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public enum LicensedSEPLocationValue
    {
        Yes = 845280000,
        No = 845280001,
        NotSure = 845280002,
    }

    public enum ChargingForLiquorReasons {
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

        public FundRaisingPurposes? FundRaisingPurpose { get; set; }
        public string SepCityId { get; set; }
        public string SpecialEventPostalCode { get; set; }
        public int? Statecode { get; set; }
        public bool? BeerGarden { get; set; }
        public bool? TastingEvent { get; set; }
        public int? TotalServings { get; set; }
        public int? InvoiceTrigger { get; set; }
        public string SpecialEventProvince { get; set; }
        public string MajorSignificanceRationale { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SEPPublicOrPrivate? PrivateOrPublic { get; set; }
        public string SpecialEventDescripton { get; set; }
        public int? Capacity { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ChargingForLiquorReasons? ChargingForLiquorReason { get; set; }
        public bool? DrinksIncluded { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DonatedOrConsular? DonateOrConsular { get; set; }
        public bool? IsAnnualEvent { get; set; }
        public string SpecialEventPermitNumber { get; set; }
        public string SpecialEventCity { get; set; }
        public string SpecialEventStreet2 { get; set; }
        public int? Statuscode { get; set; }

        public bool? IsMajorSignigicance { get; set; }
        public bool? IsGstRegisteredOrg { get; set; }
        public string MajorSignigicanceRationale { get; set; }
        public string HostOrganizationName { get; set; }
        public string HostOrganizationAddress { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public HostOrgCatergory? HostOrganizationCategory { get; set; }
        public string SpecialEventStreet1 { get; set; }
        public int? MaximumNumberOfGuests { get; set; }
        public string NonProfitName { get; set; }
        public System.DateTimeOffset? DateSubmitted { get; set; }
        public int? PoliceApproval { get; set; }
        public bool? IsManufacturingExclusivity { get; set; }
        public string HowProceedsWillBeUsedDescription { get; set; }

        public LicensedSEPLocationValue? IsLocationLicensed { get; set; }
        // list of SEP locations
        public List<SepEventLocation> EventLocations { get; set; }

        public SepCity SepCity { get; set; }
    }
}
