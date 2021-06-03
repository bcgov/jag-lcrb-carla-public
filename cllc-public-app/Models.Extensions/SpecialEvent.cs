using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class SpecialEventExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>
        public static ViewModels.SpecialEvent ToViewModel(this MicrosoftDynamicsCRMadoxioSpecialevent specialEvent)
        {
            ViewModels.SpecialEvent result = null;
            if (specialEvent != null)
            {
                result = new ViewModels.SpecialEvent
                {
                    EventLocations = new List<ViewModels.SepEventLocation>(),
                    Id = specialEvent.AdoxioSpecialeventid,
                    AdmissionFee = specialEvent.AdoxioAdmissionfee,
                    BeerGarden = specialEvent.AdoxioBeergarden,
                    Capacity = specialEvent.AdoxioCapacity,
                    ChargingForLiquorReason = (ViewModels.ChargingForLiquorReasons?)specialEvent.AdoxioChargingforliquorreason,
                    DateSubmitted = specialEvent.AdoxioDatesubmitted,
                    DrinksIncluded = specialEvent.AdoxioDrinksincluded,
                    DonatedOrConsular = (ViewModels.DonatedOrConsular?)specialEvent.AdoxioDonatedorconsular,
                    EventEndDate = specialEvent.AdoxioEventenddate,
                    EventName = specialEvent.AdoxioEventname,
                    EventStartDate = specialEvent.AdoxioEventstartdate,
                    FundRaisingPurpose = (ViewModels.FundRaisingPurposes?)specialEvent.AdoxioFundraisingpurpose,
                    HostOrganizationAddress = specialEvent.AdoxioHostorganisationaddress,
                    HostOrganizationCategory = (ViewModels.HostOrgCatergory?)specialEvent.AdoxioHostorganisationcategory,
                    HostOrganizationName = specialEvent.AdoxioHostorganisationname,
                    HowProceedsWillBeUsedDescription = specialEvent.AdoxioHowproceedswillbeuseddescription,
                    IsAnnualEvent = specialEvent.AdoxioIsannualevent,
                    IsOnPublicProperty = specialEvent.AdoxioIsonpublicproperty,
                    IsMajorSignificance = specialEvent.AdoxioIsmajorsignificance,
                    IsLocalSignificance = specialEvent.AdoxioIslocalsignificance,
                    IsGstRegisteredOrg = specialEvent.AdoxioIsgstregisteredorg,
                    IsManufacturingExclusivity = specialEvent.AdoxioIsmanufacturingexclusivity,
                    IsAgreeTsAndCs = specialEvent.AdoxioIsagreetsandcs,
                    IsPrivateResidence = specialEvent.AdoxioIsprivateresidence,
                    ResponsibleBevServiceNumber = specialEvent.AdoxioResponsiblebevservicenumber,
                    ResponsibleBevServiceNumberDoesNotHave = specialEvent.AdoxioResponsiblebevnumberdoesnothave,
                    DateAgreedToTsAndCs = specialEvent.AdoxioDateagreedtotsandcs,
                    MajorSignificanceRationale = specialEvent.AdoxioMajorsignificancerationale,
                    NonProfitName = specialEvent.AdoxioNonprofitname,
                    PoliceAccount = specialEvent.AdoxioPoliceAccountId.ToViewModel(),
                    PoliceDecisionBy = specialEvent.AdoxioPoliceRepresentativeId.ToViewModel(),
                    PoliceApproval = specialEvent.AdoxioPoliceapproval,
                    PrivateOrPublic = (ViewModels.SEPPublicOrPrivate?)specialEvent.AdoxioTypeofevent,
                    SpecialEventCity = specialEvent.AdoxioSpecialeventcity,
                    SpecialEventDescription = specialEvent.AdoxioSpecialeventdescripton,
                    SpecialEventPermitNumber = specialEvent.AdoxioSpecialeventpermitnumber,
                    SpecialEventPostalCode = specialEvent.AdoxioSpecialeventpostalcode,
                    SpecialEventProvince = specialEvent.AdoxioSpecialeventprovince,
                    SpecialEventStreet1 = specialEvent.AdoxioSpecialeventstreet1,
                    SpecialEventStreet2 = specialEvent.AdoxioSpecialeventstreet2,
                    Statecode = specialEvent.Statecode,
                    EventStatus = (ViewModels.EventStatus?)specialEvent.Statuscode, // Event Status: Draft, Submitted, Pending Review, etc.
                    TastingEvent = specialEvent.AdoxioTastingevent,
                    TotalServings = specialEvent.AdoxioTotalservings,
                    SepCity = specialEvent.AdoxioSpecialEventCityDistrictId?.ToViewModel(),
                    Applicant = specialEvent.AdoxioContactId?.ToViewModel()
                };

                var locations = specialEvent?.AdoxioSpecialeventSpecialeventlocations;
                if (locations?.Count > 0)
                {
                    result.EventLocations =
                        locations.Select(specialEvent => specialEvent.ToViewModel())
                        .ToList();
                }

            }
            return result;
        }

        public static ViewModels.SpecialEventSummary ToSummaryViewModel(this MicrosoftDynamicsCRMadoxioSpecialevent specialEvent)
        {
            ViewModels.SpecialEventSummary result = null;
            if (specialEvent != null)
            {
                result = new ViewModels.SpecialEventSummary()
                {
                    SpecialEventId = specialEvent.AdoxioSpecialeventid,
                    EventStartDate = specialEvent.AdoxioEventstartdate,
                    EventName = specialEvent.AdoxioEventname,
                    // TypeOfEvent =
                    EventStatus = (EventStatus?)specialEvent.Statuscode,
                    MaximumNumberOfGuests = specialEvent.AdoxioMaxnumofguests,
                    DateSubmitted = specialEvent.AdoxioDatesubmitted,
                    PoliceAccount = specialEvent.AdoxioPoliceAccountId.ToViewModel(),
                    PoliceDecisionBy = specialEvent.AdoxioPoliceRepresentativeId.ToViewModel(),
                    PoliceDecision = specialEvent.AdoxioPoliceapproval,
                    DateOfPoliceDecision = specialEvent.AdoxioDatepoliceapproved
                };
            }
            return result;
        }

        public static void CopyValues(this MicrosoftDynamicsCRMadoxioSpecialevent to, ViewModels.SpecialEvent from)
        {
            to.AdoxioSpecialeventid = from.Id;
            to.AdoxioAdmissionfee = from.AdmissionFee;
            to.AdoxioBeergarden = from.BeerGarden;
            to.AdoxioCapacity = from.Capacity;
            to.AdoxioIsagreetsandcs = from.IsAgreeTsAndCs;
            to.AdoxioIsprivateresidence = from.IsPrivateResidence;
            to.AdoxioDateagreedtotsandcs = from.DateAgreedToTsAndCs;
            to.AdoxioChargingforliquorreason = (int?)from.ChargingForLiquorReason;
            to.AdoxioDatesubmitted = from.DateSubmitted;
            to.AdoxioDrinksincluded = from.DrinksIncluded;
            to.AdoxioDonatedorconsular = (int?)from.DonatedOrConsular;
            to.AdoxioEventenddate = from.EventEndDate;
            to.AdoxioEventname = from.EventName;
            to.AdoxioEventstartdate = from.EventStartDate;
            to.AdoxioFundraisingpurpose = (int?)from.FundRaisingPurpose;
            to.AdoxioHowproceedswillbeuseddescription = from.HowProceedsWillBeUsedDescription;
            to.AdoxioHostorganisationaddress = from.HostOrganizationAddress;
            to.AdoxioHostorganisationcategory = (int?)from.HostOrganizationCategory;
            to.AdoxioHostorganisationname = from.HostOrganizationName;
            to.AdoxioInvoicetrigger = from.InvoiceTrigger;
            to.AdoxioIsannualevent = from.IsAnnualEvent;
            to.AdoxioIsonpublicproperty = from.IsOnPublicProperty;
            to.AdoxioIslocationlicensedos = (int?)from.IsLocationLicensed;
            to.AdoxioIsmajorsignificance = from.IsMajorSignificance;
            to.AdoxioIsgstregisteredorg = from.IsGstRegisteredOrg;
            to.AdoxioIsmanufacturingexclusivity = from.IsManufacturingExclusivity;
            to.AdoxioIslocalsignificance = from.IsLocalSignificance;
            to.AdoxioMajorsignificancerationale = from.MajorSignificanceRationale;
            to.AdoxioMaxnumofguests = from.MaximumNumberOfGuests;
            to.AdoxioNonprofitname = from.NonProfitName;
            to.AdoxioPoliceapproval = from.PoliceApproval;
            to.AdoxioPrivateorpublic = (int?)from.PrivateOrPublic;
            to.AdoxioResponsiblebevservicenumber = from.ResponsibleBevServiceNumber;
            to.AdoxioResponsiblebevnumberdoesnothave = from.ResponsibleBevServiceNumberDoesNotHave;
            to.AdoxioSpecialeventcity = from.SpecialEventCity;
            to.AdoxioSpecialeventdescripton = from.SpecialEventDescription;
            to.AdoxioSpecialeventpermitnumber = from.SpecialEventPermitNumber;
            to.AdoxioSpecialeventpostalcode = from.SpecialEventPostalCode;
            to.AdoxioSpecialeventprovince = from.SpecialEventProvince;
            to.AdoxioSpecialeventstreet1 = from.SpecialEventStreet1;
            to.AdoxioSpecialeventstreet2 = from.SpecialEventStreet2;
            to.AdoxioTastingevent = from.TastingEvent;
            to.AdoxioTotalservings = from.TotalServings;
        }
    }
}

