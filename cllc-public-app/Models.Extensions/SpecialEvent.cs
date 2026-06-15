extern alias DV;
using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.Xrm.Sdk;
using DvSpecialEvent = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent;

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

        // ---------------------------------------------------------------
        // Dataverse SDK overloads (adoxio_specialevent)
        // ---------------------------------------------------------------

        public static ViewModels.SpecialEvent ToViewModel(this DvSpecialEvent se)
        {
            if (se == null) return null;
            return new ViewModels.SpecialEvent
            {
                Id = se.Id == Guid.Empty ? null : se.Id.ToString(),
                AdmissionFee = se.adoxio_AdmissionFee,
                BeerGarden = se.adoxio_BeerGarden,
                Capacity = se.adoxio_capacity,
                NetEstimatedPST = se.adoxio_NetEstimatedPST,
                ChargingForLiquorReason = (ViewModels.ChargingForLiquorReasons?)(int?)se.adoxio_ChargingforLiquorReason,
                DateSubmitted = se.adoxio_DateSubmitted,
                DrinksIncluded = se.adoxio_DrinksIncluded,
                DonatedOrConsular = (ViewModels.DonatedOrConsular?)(int?)se.adoxio_DonatedorConsular,
                EventEndDate = se.adoxio_EventEndDate,
                EventName = se.adoxio_eventname,
                EventStartDate = se.adoxio_EventStartDate,
                FundRaisingPurpose = (ViewModels.FundRaisingPurposes?)(int?)se.adoxio_FundraisingPurpose,
                HostOrganizationAddress = se.adoxio_HostOrganisationAddress,
                HostOrganizationCategory = (ViewModels.HostOrgCatergory?)(int?)se.adoxio_HostOrganisationCategory,
                HostOrganizationName = se.adoxio_HostOrganisationName,
                HowProceedsWillBeUsedDescription = se.adoxio_HowProceedsWillbeUsedDescription,
                IsAnnualEvent = se.adoxio_IsAnnualEvent,
                IsOnPublicProperty = se.adoxio_IsOnPublicProperty,
                IsLocationLicensed = (ViewModels.LicensedSEPLocationValue?)(int?)se.adoxio_IsLocationLicensedOS,
                IsMajorSignificance = se.adoxio_IsMajorSignificance,
                IsGstRegisteredOrg = se.adoxio_IsGSTRegisteredOrg,
                IsManufacturingExclusivity = se.adoxio_IsManufacturingExclusivity,
                IsLocalSignificance = se.adoxio_IsLocalSignificance,
                IsSupportLocalArtsOrSports = se.adoxio_IsSupportLocalArtsorSports,
                IsAgreeTsAndCs = se.adoxio_IsAgreeTsandCs,
                IsPrivateResidence = se.adoxio_IsPrivateResidence,
                ResponsibleBevServiceNumber = se.adoxio_ResponsibleBevServiceNumber,
                ResponsibleBevServiceNumberDoesNotHave = se.adoxio_ResponsibleBevNumberDoesNotHave,
                DateAgreedToTsAndCs = se.adoxio_DateAgreedtoTsandCs,
                DateIssued = se.adoxio_DateIssued,
                MajorSignificanceRationale = se.adoxio_MajorSignificanceRationale,
                NonProfitName = se.adoxio_NonProfitName,
                PoliceApproval = (ViewModels.ApproverStatus?)(int?)se.adoxio_PoliceApproval,
                LcrbApproval = (ViewModels.ApproverStatus?)(int?)se.adoxio_LCRBApproval,
                PrivateOrPublic = (ViewModels.SEPPublicOrPrivate?)(int?)se.adoxio_PrivateorPublic,
                DenialReason = se.adoxio_DenialReason,
                CancelReason = se.adoxio_CancellationReason,
                SpecialEventCity = se.adoxio_SpecialEventCity,
                SpecialEventDescription = se.adoxio_SpecialEventDescripton,
                SpecialEventPermitNumber = se.adoxio_SpecialEventPermitNumber,
                SpecialEventPostalCode = se.adoxio_SpecialEventPostalCode,
                SpecialEventProvince = se.adoxio_SpecialEventProvince,
                SpecialEventStreet1 = se.adoxio_SpecialEventStreet1,
                SpecialEventStreet2 = se.adoxio_SpecialEventStreet2,
                Statecode = (int?)se.statecode,
                EventStatus = (EventStatus?)(int?)se.statuscode,
                TastingEvent = se.adoxio_TastingEvent,
                TotalServings = se.adoxio_TotalServings,
                EventLocations = new List<ViewModels.SepEventLocation>(),
            };
        }

        /// <summary>
        /// Copy ViewModel values onto a Dataverse adoxio_specialevent entity.
        /// Does not set navigation properties (AccountId, ContactId, SepCity) — caller sets those via EntityReference.
        /// </summary>
        public static void CopyValues(this DvSpecialEvent to, ViewModels.SpecialEvent from)
        {
            to.adoxio_AdmissionFee = from.AdmissionFee;
            to.adoxio_BeerGarden = from.BeerGarden;
            to.adoxio_capacity = from.Capacity;
            to.adoxio_IsAgreeTsandCs = from.IsAgreeTsAndCs;
            to.adoxio_IsPrivateResidence = from.IsPrivateResidence;
            to.adoxio_DateAgreedtoTsandCs = from.DateAgreedToTsAndCs?.UtcDateTime;
            to.adoxio_ChargingforLiquorReason = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent_adoxio_chargingforliquorreason?)(int?)from.ChargingForLiquorReason;
            to.adoxio_DateSubmitted = from.DateSubmitted?.UtcDateTime;
            to.adoxio_DrinksIncluded = from.DrinksIncluded;
            to.adoxio_DonatedorConsular = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent_adoxio_donatedorconsular?)(int?)from.DonatedOrConsular;
            to.adoxio_EventEndDate = from.EventEndDate?.UtcDateTime;
            to.adoxio_eventname = from.EventName;
            to.adoxio_EventStartDate = from.EventStartDate?.UtcDateTime;
            to.adoxio_FundraisingPurpose = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent_adoxio_fundraisingpurpose?)(int?)from.FundRaisingPurpose;
            to.adoxio_HowProceedsWillbeUsedDescription = from.HowProceedsWillBeUsedDescription;
            to.adoxio_HostOrganisationAddress = from.HostOrganizationAddress;
            to.adoxio_HostOrganisationCategory = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent_adoxio_hostorganisationcategory?)(int?)from.HostOrganizationCategory;
            to.adoxio_HostOrganisationName = from.HostOrganizationName;
            to.adoxio_InvoiceTrigger = from.InvoiceTrigger;
            to.adoxio_IsAnnualEvent = from.IsAnnualEvent;
            to.adoxio_IsOnPublicProperty = from.IsOnPublicProperty;
            to.adoxio_IsLocationLicensedOS = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent_adoxio_islocationlicensedos?)(int?)from.IsLocationLicensed;
            to.adoxio_IsMajorSignificance = from.IsMajorSignificance;
            to.adoxio_IsGSTRegisteredOrg = from.IsGstRegisteredOrg;
            to.adoxio_IsManufacturingExclusivity = from.IsManufacturingExclusivity;
            to.adoxio_IsLocalSignificance = from.IsLocalSignificance;
            to.adoxio_MajorSignificanceRationale = from.MajorSignificanceRationale;
            to.adoxio_MaxNumofGuests = from.MaximumNumberOfGuests;
            to.adoxio_NonProfitName = from.NonProfitName;
            to.adoxio_PoliceApproval = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent_adoxio_policeapproval?)(int?)from.PoliceApproval;
            to.adoxio_PrivateorPublic = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent_adoxio_privateorpublic?)(int?)from.PrivateOrPublic;
            to.adoxio_DenialReason = from.DenialReason;
            to.adoxio_CancellationReason = from.CancelReason;
            to.adoxio_ResponsibleBevServiceNumber = from.ResponsibleBevServiceNumber;
            to.adoxio_ResponsibleBevNumberDoesNotHave = from.ResponsibleBevServiceNumberDoesNotHave;
            to.adoxio_SpecialEventCity = from.SpecialEventCity;
            to.adoxio_SpecialEventDescripton = from.SpecialEventDescription;
            to.adoxio_SpecialEventPostalCode = from.SpecialEventPostalCode;
            to.adoxio_SpecialEventProvince = from.SpecialEventProvince;
            to.adoxio_SpecialEventStreet1 = from.SpecialEventStreet1;
            to.adoxio_SpecialEventStreet2 = from.SpecialEventStreet2;
            to.adoxio_TastingEvent = from.TastingEvent;
            to.adoxio_TotalServings = from.TotalServings;
            to.adoxio_IsSupportLocalArtsorSports = from.IsSupportLocalArtsOrSports;
        }

        public static ViewModels.SpecialEventSummary ToSummaryViewModel(this DvSpecialEvent se)
        {
            if (se == null) return null;
            var result = new ViewModels.SpecialEventSummary
            {
                SpecialEventId = se.Id == Guid.Empty ? null : se.Id.ToString(),
                EventStartDate = se.adoxio_EventStartDate,
                EventName = se.adoxio_eventname,
                IsInvoicePaid = se.adoxio_IsInvoicePaid,
                MaximumNumberOfGuests = se.adoxio_MaxNumofGuests,
                DateSubmitted = se.adoxio_DateSubmitted,
                PoliceApproval = (ApproverStatus?)(int?)se.adoxio_PoliceApproval,
                LcrbApproval = (ApproverStatus?)(int?)se.adoxio_LCRBApproval,
                DenialReason = se.adoxio_DenialReason,
                CancelReason = se.adoxio_CancellationReason,
                DateOfPoliceDecision = se.adoxio_DatePoliceApproved,
            };
            if (se.adoxio_typeofevent != null)
                result.EventType = (EventType)(int)se.adoxio_typeofevent;
            if (se.statuscode != null)
                result.EventStatus = (EventStatus)(int)se.statuscode;
            return result;
        }
    }
}

