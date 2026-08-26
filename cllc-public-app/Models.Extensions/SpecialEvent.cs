extern alias DV;
using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces;
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

