using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces;
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
        /// Convert a given SpecialEvent to a ViewModel
        /// </summary>
        public static ViewModels.SpecialEvent ToViewModel(this MicrosoftDynamicsCRMadoxioSpecialevent specialEvent, IDynamicsClient dynamicsClient)
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
                    NetEstimatedPST = specialEvent.AdoxioNetestimatedpst,
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
                    IsSupportLocalArtsOrSports = specialEvent.AdoxioIssupportlocalartsorsports,
                    IsGstRegisteredOrg = specialEvent.AdoxioIsgstregisteredorg,
                    IsManufacturingExclusivity = specialEvent.AdoxioIsmanufacturingexclusivity,
                    IsAgreeTsAndCs = specialEvent.AdoxioIsagreetsandcs,
                    IsPrivateResidence = specialEvent.AdoxioIsprivateresidence,
                    ResponsibleBevServiceNumber = specialEvent.AdoxioResponsiblebevservicenumber,
                    ResponsibleBevServiceNumberDoesNotHave = specialEvent.AdoxioResponsiblebevnumberdoesnothave,
                    DateAgreedToTsAndCs = specialEvent.AdoxioDateagreedtotsandcs,
                    DateIssued = specialEvent.AdoxioDateissued,
                    MajorSignificanceRationale = specialEvent.AdoxioMajorsignificancerationale,
                    NonProfitName = specialEvent.AdoxioNonprofitname,
                    PoliceAccount = specialEvent.AdoxioPoliceAccountId.ToViewModel(),
                    PoliceDecisionBy = specialEvent.AdoxioPoliceRepresentativeId.ToViewModel(),
                    PoliceApproval = (ViewModels.ApproverStatus?)specialEvent.AdoxioPoliceapproval,
                    IsLocationLicensed = (ViewModels.LicensedSEPLocationValue?)specialEvent.AdoxioIslocationlicensedos,
                    LcrbApproval = (ViewModels.ApproverStatus?)specialEvent.AdoxioLcrbapproval,
                    PrivateOrPublic = (ViewModels.SEPPublicOrPrivate?)specialEvent.AdoxioPrivateorpublic,
                    DenialReason = specialEvent.AdoxioDenialreason,
                    CancelReason = specialEvent.AdoxioCancellationreason,
                    SpecialEventCity = specialEvent.AdoxioSpecialeventcity,
                    SpecialEventDescription = specialEvent.AdoxioSpecialeventdescripton,
                    SpecialEventPermitNumber = specialEvent.AdoxioSpecialeventpermitnumber,
                    SpecialEventPostalCode = specialEvent.AdoxioSpecialeventpostalcode,
                    SpecialEventProvince = specialEvent.AdoxioSpecialeventprovince,
                    SpecialEventStreet1 = specialEvent.AdoxioSpecialeventstreet1,
                    SpecialEventStreet2 = specialEvent.AdoxioSpecialeventstreet2,
                    Statecode = specialEvent.Statecode,
                    EventStatus = (EventStatus?)specialEvent.Statuscode, // Event Status: Draft, Submitted, Pending Review, etc.
                    TastingEvent = specialEvent.AdoxioTastingevent,
                    TotalServings = specialEvent.AdoxioTotalservings,
                    SepCity = specialEvent.AdoxioSpecialEventCityDistrictId?.ToViewModel(),
                    Applicant = specialEvent.AdoxioContactId?.ToViewModel(),
                    Invoice = specialEvent.AdoxioInvoice?.ToViewModel(),
                    TotalProceeds = 0,
                    TotalPurchaseCost = 0,
                    TotalRevenue = 0
                };

                var locations = specialEvent?.AdoxioSpecialeventSpecialeventlocations;
                if (locations?.Count > 0)
                {
                    result.EventLocations =
                        locations.Select(eventLocation => eventLocation.ToViewModel())
                        .ToList();
                }

                var forecast = specialEvent?.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent;

                if (forecast?.Count > 0)
                {
                    result.DrinksSalesForecasts = 
                        forecast.Select(drinkSalesForecast => drinkSalesForecast.ToViewModel())
                        .ToList();
                }

                var drinkTypes = dynamicsClient.Sepdrinktypes.Get().Value
                            .ToList();

                string beerTypeId = drinkTypes.Where(drinkType => drinkType.AdoxioName == "Beer/Cider/Cooler")
                                    .Select(drinkType => drinkType.AdoxioSepdrinktypeid)
                                    .FirstOrDefault();

                string wineTypeId = drinkTypes.Where(drinkType => drinkType.AdoxioName == "Wine")
                                    .Select(drinkType => drinkType.AdoxioSepdrinktypeid)
                                    .FirstOrDefault();

                string spiritsTypeId = drinkTypes.Where(drinkType => drinkType.AdoxioName == "Spirits")
                                    .Select(drinkType => drinkType.AdoxioSepdrinktypeid)
                                    .FirstOrDefault();

                if (specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent == null)
                {
                    result.Beer = 0;
                    result.Wine = 0;
                    result.Spirits = 0;
                }
                else
                {
                    result.Beer = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                        .Where(sepDrinkSalesForecast => sepDrinkSalesForecast._adoxioTypeValue == beerTypeId)
                        .Select(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioEstimatedservings)
                        .FirstOrDefault();

                    result.AverageBeerPrice = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                        .Where(sepDrinkSalesForecast => sepDrinkSalesForecast._adoxioTypeValue == beerTypeId)
                        .Select(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioPriceperserving)
                        .FirstOrDefault();

                    result.Wine = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                        .Where(sepDrinkSalesForecast => sepDrinkSalesForecast._adoxioTypeValue == wineTypeId)
                        .Select(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioEstimatedservings)
                        .FirstOrDefault();

                    result.AverageWinePrice = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                        .Where(sepDrinkSalesForecast => sepDrinkSalesForecast._adoxioTypeValue == wineTypeId)
                        .Select(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioPriceperserving)
                        .FirstOrDefault();

                    result.Spirits = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                        .Where(sepDrinkSalesForecast => sepDrinkSalesForecast._adoxioTypeValue == spiritsTypeId)
                        .Select(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioEstimatedservings)
                        .FirstOrDefault();

                    result.AverageSpiritsPrice = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                        .Where(sepDrinkSalesForecast => sepDrinkSalesForecast._adoxioTypeValue == spiritsTypeId)
                        .Select(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioPriceperserving)
                        .FirstOrDefault();

                    result.Beer_free = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                       .Where(sepDrinkSalesForecast => sepDrinkSalesForecast._adoxioTypeValue == beerTypeId &&
                       sepDrinkSalesForecast.AdoxioIscharging==false)
                       .Select(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioEstimatedservings)
                       .FirstOrDefault();
                    result.Wine_free = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                       .Where(sepDrinkSalesForecast => sepDrinkSalesForecast._adoxioTypeValue == wineTypeId &&
                       sepDrinkSalesForecast.AdoxioIscharging == false)
                       .Select(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioEstimatedservings)
                       .FirstOrDefault();
                    result.Spirits_free = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                       .Where(sepDrinkSalesForecast => sepDrinkSalesForecast._adoxioTypeValue == spiritsTypeId &&
                       sepDrinkSalesForecast.AdoxioIscharging == false)
                       .Select(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioEstimatedservings)
                       .FirstOrDefault();

                }

                result.TotalProceeds = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent == null ? 0 : specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                    .Sum(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioEstimatedrevenue.GetValueOrDefault() - sepDrinkSalesForecast.AdoxioEstimatedcost.GetValueOrDefault());

                result.TotalRevenue = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent == null ? 0 : specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                    .Sum(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioEstimatedrevenue.GetValueOrDefault());

                result.TotalPurchaseCost = specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent == null ? 0 : specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                    .Sum(sepDrinkSalesForecast => sepDrinkSalesForecast.AdoxioEstimatedcost.GetValueOrDefault());

                if (specialEvent.AdoxioSpecialeventSpecialeventtsacs != null)
                {
                    result.TermsAndConditions = new List<SepTermAndCondition>();
                    specialEvent.AdoxioSpecialeventSpecialeventtsacs.ToList()
                    .ForEach(term =>
                    {
                        result.TermsAndConditions.Add(new SepTermAndCondition {
                            Id = term.AdoxioSpecialeventtandcid,
                            Content = term.AdoxioTermsandcondition,
                            Originator = term.AdoxioOriginator
                         });
                    });
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
                    InvoiceId = specialEvent._adoxioInvoiceValue,
                    IsInvoicePaid = specialEvent.AdoxioIsinvoicepaid,
                    MaximumNumberOfGuests = specialEvent.AdoxioMaxnumofguests,
                    DateSubmitted = specialEvent.AdoxioDatesubmitted,
                    PoliceAccount = specialEvent.AdoxioPoliceAccountId.ToViewModel(),
                    PoliceDecisionBy = specialEvent.AdoxioPoliceRepresentativeId.ToViewModel(),
                    PoliceApproval = (ApproverStatus?)specialEvent.AdoxioPoliceapproval,
                    //LcrbApprovalBy = specialEvent.AdoxioLCRBRepresentativeId.ToViewModel(),
                    LcrbApproval = (ApproverStatus?)specialEvent.AdoxioLcrbapproval,
                    DenialReason = specialEvent.AdoxioDenialreason,
                    CancelReason = specialEvent.AdoxioCancellationreason,
                    DateOfPoliceDecision = specialEvent.AdoxioDatepoliceapproved
                };

                if (specialEvent.AdoxioTypeofevent != null)
                {
                    result.EventType = (EventType)specialEvent.AdoxioTypeofevent;
                }

                if (specialEvent.Statuscode != null)
                {
                    result.EventStatus = (EventStatus)specialEvent.Statuscode;
                }

                if (specialEvent.AdoxioSpecialeventSpecialeventtsacs != null)
                {
                    result.TermsAndConditions = new List<SepTermAndCondition>();
                    specialEvent.AdoxioSpecialeventSpecialeventtsacs.ToList()
                    .ForEach(term =>
                    {
                        result.TermsAndConditions.Add(new SepTermAndCondition {
                            Id = term.AdoxioSpecialeventtandcid,
                            Content = term.AdoxioTermsandcondition,
                            Originator = term.AdoxioOriginator
                         });
                    });
                }

            }
            return result;
        }

        /// <summary>
        /// Copy the values from a SpecialEvent ViewModel to a SpecialEvent entity.
        ///
        /// Note: The `AdoxioSpecialeventpermitnumber` field should not be included, as this value is generated by
        /// dynamics when the record is created, and should not be supplied by the client.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
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
            to.AdoxioPoliceapproval = (int?)from.PoliceApproval;
            to.AdoxioPrivateorpublic = (int?)from.PrivateOrPublic;
            to.AdoxioDenialreason = from.DenialReason;
            to.AdoxioCancellationreason = from.CancelReason;
            to.AdoxioResponsiblebevservicenumber = from.ResponsibleBevServiceNumber;
            to.AdoxioResponsiblebevnumberdoesnothave = from.ResponsibleBevServiceNumberDoesNotHave;
            to.AdoxioSpecialeventcity = from.SpecialEventCity;
            to.AdoxioSpecialeventdescripton = from.SpecialEventDescription;
            to.AdoxioSpecialeventpostalcode = from.SpecialEventPostalCode;
            to.AdoxioSpecialeventprovince = from.SpecialEventProvince;
            to.AdoxioSpecialeventstreet1 = from.SpecialEventStreet1;
            to.AdoxioSpecialeventstreet2 = from.SpecialEventStreet2;
            to.AdoxioTastingevent = from.TastingEvent;
            to.AdoxioTotalservings = from.TotalServings;
            to.AdoxioIssupportlocalartsorsports = from.IsSupportLocalArtsOrSports;
        }
    }
}

