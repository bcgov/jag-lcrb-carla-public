extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.ViewModels;
using Gov.Lclb.Cllb.Public.Utils;
using System.Collections.Generic;
using System;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class LicenceEventExtensions
    {
        public static EventClass DetermineEventClass(this LicenceEvent item, bool alwaysAuthorization)
        {
            bool isHighRisk = false;

            // Attendance > 500
            int maxAttendance = item.MaxAttendance != null ? (int)item.MaxAttendance : 0;
            int maxStaffAttendance = item.MaxStaffAttendance != null ? (int)item.MaxStaffAttendance : 0;
            if (maxAttendance + maxStaffAttendance >= 500)
            {
                isHighRisk = true;
            }

            // Location is outdoors
            // int? location = item.SpecificLocation;
            if (item.SpecificLocation == SpecificLocation.Outdoors || item.SpecificLocation == SpecificLocation.Both)
            {
                isHighRisk = true;
            }

            // liquor service ends after 2am (but not community event)
            if (item.EventType != EventType.Community)
            {
                item.Schedules?.ForEach(schedule =>
                {
                    if (schedule.ServiceEndDateTime.HasValue)
                    {
                        TimeZoneInfo hwZone;
                        try
                        {
                            hwZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
                        }
                        catch (TimeZoneNotFoundException)
                        {
                            hwZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                        }

                        DateTimeOffset endTime = TimeZoneInfo.ConvertTimeFromUtc(schedule.ServiceEndDateTime.HasValue ? schedule.ServiceEndDateTime.Value.DateTime : DateTime.MaxValue, hwZone);
                        if ((endTime.Hour == 2 && endTime.Minute != 0) || (endTime.Hour > 2 && endTime.Hour < 9))
                        {
                            isHighRisk = true;
                        }
                    }
                });
            }

            // TODO: Should TUA-specific business rules be added here? Right now TUA events get auto-approved

            if (isHighRisk || alwaysAuthorization)
            {
                return EventClass.Authorization;
            }
            return EventClass.Notice;
        }

        public static LicenceEvent ToViewModel(this adoxio_event item, IList<adoxio_eventschedule> schedules, IList<adoxio_eventlocation> locations)
        {
            if (item == null) return null;
            var result = new LicenceEvent
            {
                Id = item.adoxio_eventId?.ToString(),
                Status = item.statuscode.HasValue ? (LicenceEventStatus?)(int)item.statuscode.Value : null,
                Name = item.adoxio_name,
                StartDate = item.adoxio_EventStartDate.HasValue ? (DateTimeOffset?)item.adoxio_EventStartDate.Value : null,
                EndDate = item.adoxio_EventEndDate.HasValue ? (DateTimeOffset?)item.adoxio_EventEndDate.Value : null,
                VenueDescription = item.adoxio_VenueNameDescription,
                AdditionalLocationInformation = item.adoxio_AdditionalLocationInfo,
                FoodService = (FoodService?)(int?)item.adoxio_FoodService,
                FoodServiceDescription = item.adoxio_FoodServiceDescription,
                Entertainment = (Entertainment?)(int?)item.adoxio_Entertainment,
                EntertainmentDescription = item.adoxio_EntertainmentDescription,
                ContactPhone = item.adoxio_ContactPhoneNumber,
                ContactName = item.adoxio_ContactName,
                ExternalId = item.adoxio_ExternalID,
                ContactEmail = item.adoxio_ContactEmail,
                EventNumber = item.adoxio_EventNumber,
                ClientHostname = item.adoxio_ClientHostname,
                EventType = (EventType?)(int?)item.adoxio_EventType,
                EventTypeDescription = item.adoxio_EventDescription,
                ImportSequenceNumber = item.ImportSequenceNumber,
                SpecificLocation = (SpecificLocation?)(int?)item.adoxio_SpecificLocation,
                EventClass = (EventClass?)(int?)item.adoxio_Class,
                MaxAttendance = item.adoxio_MaxAttendance,
                MaxStaffAttendance = item.adoxio_MaxStaffAttendance,
                MinorsAttending = item.adoxio_attendanceminors,
                CommunityApproval = item.adoxio_CommunityApproval,
                NotifyEventInspector = item.adoxio_NotifyEventInspector,
                LicenceId = item.adoxio_Licence?.Id.ToString(),
                AccountId = item.adoxio_Account?.Id.ToString(),
                Street1 = item.adoxio_Street1,
                Street2 = item.adoxio_Street2,
                City = item.adoxio_City,
                Province = item.adoxio_Province,
                PostalCode = item.adoxio_PostalCode,
                ModifiedOn = item.ModifiedOn.HasValue ? (DateTimeOffset?)item.ModifiedOn.Value : null,
                // Security Plan
                SecurityPlanRequested = item.adoxio_requestsafetysecurityplan,
                EventLiquorLayout = item.adoxio_eventliquorlayout,
                DailyEventAttendees = item.adoxio_numberdailyeventattendees,
                DailyMinorAttendees = item.adoxio_numberdailyminorattendees,
                OccupantLoad = item.adoxio_eventoccupantload,
                OccupantLoadAvailable = item.adoxio_iseventloadavailable,
                OccupantLoadServiceArea = item.adoxio_eventoccupantloadservicesarea,
                OccupantLoadServiceAreaAvailable = item.adoxio_isservicearealoadavailable,
                ServiceAreaControlledDetails = item.adoxio_eventliquorcontainment,
                StaffingManagers = item.adoxio_eventstaffingmanagers,
                StaffingBartenders = item.adoxio_eventstaffingbartenders,
                StaffingServers = item.adoxio_eventstaffingservers,
                SecurityPersonnel = item.adoxio_securitycompanysummary,
                SecurityPersonnelThroughCompany = item.adoxio_SecurityPersonnelNumberHired,
                SecurityCompanyName = item.adoxio_securitycompanyname,
                SecurityCompanyAddress = item.adoxio_securitycompanystreet,
                SecurityCompanyCity = item.adoxio_securitycompanycity,
                SecurityCompanyPostalCode = item.adoxio_securitycompanypostal,
                SecurityCompanyContactPerson = item.adoxio_securitycompanycontactname,
                SecurityCompanyPhoneNumber = item.adoxio_securitycompanycontactphone,
                SecurityCompanyEmail = item.adoxio_securitycompanycontactemail,
                SecurityPoliceOfficerSummary = item.adoxio_policeofficersummary,
                SafeAndResponsibleMinorsNotAttending = item.adoxio_isminorsattending,
                SafeAndResponsibleLiquorAreaControlled = item.adoxio_isliquorareacontrolled,
                SafeAndResponsibleLiquorAreaControlledDescription = item.adoxio_liquorareacontrolleddetails,
                SafeAndResponsibleMandatoryID = item.adoxio_istwopiecesidrequired,
                SafeAndResponsibleSignsAdvisingMinors = item.adoxio_issignsadvisingminors,
                SafeAndResponsibleMinorsOther = item.adoxio_isotherminorssafety,
                SafeAndResponsibleMinorsOtherDescription = item.adoxio_isotherminorssafetydetails,
                SafeAndResponsibleSignsAdvisingRemoval = item.adoxio_issignsintoxicatedpersons,
                SafeAndResponsibleSignsAdvisingTwoDrink = item.adoxio_issignstwodrinkmax,
                SafeAndResponsibleOverConsumptionOther = item.adoxio_isotherconsumptionsafety,
                SafeAndResponsibleOverConsumptionOtherDescription = item.adoxio_isotherconsumptionsafetydetails,
                SafeAndResponsibleReadAppendix2 = item.adoxio_isdisturbanceappendix2,
                SafeAndResponsibleDisturbancesOther = item.adoxio_isotherdisturbance,
                SafeAndResponsibleDisturbancesOtherDescription = item.adoxio_isotherdisturbancedetails,
                SafeAndResponsibleAdditionalSafetyMeasures = item.adoxio_additionalsafetydetails,
                SafeAndResponsibleServiceAreaSupervision = item.adoxio_ServiceAreaEntranceSupervisionDetails,
                DeclarationIsAccurate = item.adoxio_isdeclarationaccurate,
                SecurityPlanSubmitted = item.adoxio_safetysecurityplanchangessubmitted,
                SEPLicensee = item.adoxio_SEPLicensee,
                SEPLicenceNumber = item.adoxio_SEPLicenceNumber,
                SEPContactName = item.adoxio_SEPContactName,
                SEPContactPhoneNumber = item.adoxio_SEPContactPhoneNumber,
                // market events
                IsNoPreventingSaleofLiquor = item.adoxio_IsNoPreventingSaleofLiquor,
                IsMarketManagedorCarried = item.adoxio_IsMarketManagedorCarried,
                IsMarketOnlyVendors = item.adoxio_IsMarketOnlyVendors,
                IsNoImportedGoods = item.adoxio_IsNoImportedGoods,
                IsMarketHostsSixVendors = item.adoxio_IsMarketHostsSixVendors,
                IsMarketMaxAmountorDuration = item.adoxio_IsMarketMaxAmountorDuration,
                MKTOrganizerContactName = item.adoxio_MKTOrganizerContactName,
                MKTOrganizerContactPhone = item.adoxio_MKTOrganizerContactPhone,
                RegistrationNumber = item.adoxio_RegistrationNumber,
                BusinessNumber = item.adoxio_BusinessNumber,
                MarketName = item.adoxio_MarketName,
                MarketWebsite = item.adoxio_MarketWebsite,
                MarketDuration = (MarketDuration?)(int?)item.adoxio_MarketDuration,
                IsAllStaffServingitRight = item.adoxio_IsAllStaffServingitRight,
                IsSalesAreaAvailandDefined = item.adoxio_IsSalesAreaAvailandDefined,
                IsSampleSizeCompliant = item.adoxio_IsSampleSizeCompliant,
                EventCategory = (EventCategory?)(int?)item.adoxio_EventCategory,
                MarketEventType = (MarketEventType?)(int?)item.adoxio_MarketEventType,
                // TUA events
                EventName = item.adoxio_Eventname,
                TuaEventType = (TuaEventType?)(int?)item.adoxio_TUAEventType,
                IsClosedToPublic = item.adoxio_IsClosedtoPublic,
                IsWedding = item.adoxio_IsWedding,
                IsNetworkingParty = item.adoxio_IsNetworkingParty,
                IsConcert = item.adoxio_IsConcert,
                IsBanquet = item.adoxio_IsBanquet,
                IsAmplifiedSound = item.adoxio_IsAmplifiedSound,
                IsDancing = item.adoxio_IsDancing,
                IsReception = item.adoxio_IsReception,
                IsLiveEntertainment = item.adoxio_IsLiveEntertainment,
                IsGambling = item.adoxio_IsGambling,
                IsNoneOfTheAbove = item.adoxio_IsNoneoftheAbove,
                IsAgreement1 = item.adoxio_IsAgreement1,
                IsAgreement2 = item.adoxio_IsAgreement2,
                Schedules = new List<LicenceEventSchedule>(),
                EventLocations = new List<LicenceEventLocation>(),
            };

            if (schedules != null)
                foreach (var s in schedules)
                    result.Schedules.Add(s.ToViewModel());

            if (locations != null)
                foreach (var l in locations)
                    result.EventLocations.Add(l.ToViewModel());

            return result;
        }

        public static void CopyValues(this adoxio_event to, LicenceEvent from)
        {
            to.adoxio_name = from.Name;
            to.statuscode = from.Status.HasValue ? (adoxio_event_statuscode?)(int)from.Status.Value : null;
            if (from.StartDate.HasValue)
                to.adoxio_EventStartDate = from.StartDate.Value.UtcDateTime;
            if (from.EndDate.HasValue)
                to.adoxio_EventEndDate = from.EndDate.Value.UtcDateTime;
            to.adoxio_VenueNameDescription = from.VenueDescription;
            to.adoxio_AdditionalLocationInfo = from.AdditionalLocationInformation;
            to.adoxio_FoodService = from.FoodService.HasValue ? (adoxio_event_adoxio_foodservice?)(int)from.FoodService.Value : null;
            to.adoxio_FoodServiceDescription = from.FoodServiceDescription;
            to.adoxio_Entertainment = from.Entertainment.HasValue ? (adoxio_event_adoxio_entertainment?)(int)from.Entertainment.Value : null;
            to.adoxio_EntertainmentDescription = from.EntertainmentDescription;
            to.adoxio_ContactPhoneNumber = from.ContactPhone;
            to.adoxio_ContactName = from.ContactName;
            to.adoxio_ExternalID = from.ExternalId;
            to.adoxio_ContactEmail = from.ContactEmail;
            to.adoxio_EventNumber = from.EventNumber;
            to.adoxio_ClientHostname = from.ClientHostname;
            to.adoxio_EventType = from.EventType.HasValue ? (adoxio_event_adoxio_eventtype?)(int)from.EventType.Value : null;
            to.adoxio_EventDescription = from.EventTypeDescription;
            to.ImportSequenceNumber = from.ImportSequenceNumber;
            to.adoxio_SpecificLocation = from.SpecificLocation.HasValue ? (adoxio_event_adoxio_specificlocation?)(int)from.SpecificLocation.Value : null;
            to.adoxio_Class = from.EventClass.HasValue ? (adoxio_event_adoxio_class?)(int)from.EventClass.Value : null;
            to.adoxio_MaxAttendance = from.MaxAttendance;
            to.adoxio_MaxStaffAttendance = from.MaxStaffAttendance;
            to.adoxio_attendanceminors = from.MinorsAttending;
            to.adoxio_CommunityApproval = from.CommunityApproval;
            to.adoxio_NotifyEventInspector = from.NotifyEventInspector;
            to.adoxio_Street1 = from.Street1;
            to.adoxio_Street2 = from.Street2;
            to.adoxio_City = from.City;
            to.adoxio_Province = from.Province;
            to.adoxio_PostalCode = from.PostalCode;
            // Security Plan
            to.adoxio_requestsafetysecurityplan = from.SecurityPlanRequested;
            to.adoxio_eventliquorlayout = from.EventLiquorLayout;
            to.adoxio_numberdailyeventattendees = from.DailyEventAttendees;
            to.adoxio_numberdailyminorattendees = from.DailyMinorAttendees;
            to.adoxio_eventoccupantload = from.OccupantLoad;
            to.adoxio_iseventloadavailable = from.OccupantLoadAvailable;
            to.adoxio_eventoccupantloadservicesarea = from.OccupantLoadServiceArea;
            to.adoxio_isservicearealoadavailable = from.OccupantLoadServiceAreaAvailable;
            to.adoxio_eventliquorcontainment = from.ServiceAreaControlledDetails;
            to.adoxio_eventstaffingmanagers = from.StaffingManagers;
            to.adoxio_eventstaffingbartenders = from.StaffingBartenders;
            to.adoxio_eventstaffingservers = from.StaffingServers;
            to.adoxio_securitycompanysummary = from.SecurityPersonnel;
            to.adoxio_SecurityPersonnelNumberHired = from.SecurityPersonnelThroughCompany;
            to.adoxio_securitycompanyname = from.SecurityCompanyName;
            to.adoxio_securitycompanystreet = from.SecurityCompanyAddress;
            to.adoxio_securitycompanycity = from.SecurityCompanyCity;
            to.adoxio_securitycompanypostal = from.SecurityCompanyPostalCode;
            to.adoxio_securitycompanycontactname = from.SecurityCompanyContactPerson;
            to.adoxio_securitycompanycontactphone = from.SecurityCompanyPhoneNumber;
            to.adoxio_securitycompanycontactemail = from.SecurityCompanyEmail;
            to.adoxio_policeofficersummary = from.SecurityPoliceOfficerSummary;
            to.adoxio_isminorsattending = from.SafeAndResponsibleMinorsNotAttending;
            to.adoxio_isliquorareacontrolled = from.SafeAndResponsibleLiquorAreaControlled;
            to.adoxio_liquorareacontrolleddetails = from.SafeAndResponsibleLiquorAreaControlledDescription;
            to.adoxio_istwopiecesidrequired = from.SafeAndResponsibleMandatoryID;
            to.adoxio_issignsadvisingminors = from.SafeAndResponsibleSignsAdvisingMinors;
            to.adoxio_isotherminorssafety = from.SafeAndResponsibleMinorsOther;
            to.adoxio_isotherminorssafetydetails = from.SafeAndResponsibleMinorsOtherDescription;
            to.adoxio_issignsintoxicatedpersons = from.SafeAndResponsibleSignsAdvisingRemoval;
            to.adoxio_issignstwodrinkmax = from.SafeAndResponsibleSignsAdvisingTwoDrink;
            to.adoxio_isotherconsumptionsafety = from.SafeAndResponsibleOverConsumptionOther;
            to.adoxio_isotherconsumptionsafetydetails = from.SafeAndResponsibleOverConsumptionOtherDescription;
            to.adoxio_isdisturbanceappendix2 = from.SafeAndResponsibleReadAppendix2;
            to.adoxio_isotherdisturbance = from.SafeAndResponsibleDisturbancesOther;
            to.adoxio_isotherdisturbancedetails = from.SafeAndResponsibleDisturbancesOtherDescription;
            to.adoxio_additionalsafetydetails = from.SafeAndResponsibleAdditionalSafetyMeasures;
            to.adoxio_ServiceAreaEntranceSupervisionDetails = from.SafeAndResponsibleServiceAreaSupervision;
            to.adoxio_isdeclarationaccurate = from.DeclarationIsAccurate;
            to.adoxio_safetysecurityplanchangessubmitted = from.SecurityPlanSubmitted;
            to.adoxio_SEPLicensee = from.SEPLicensee;
            to.adoxio_SEPLicenceNumber = from.SEPLicenceNumber;
            to.adoxio_SEPContactName = from.SEPContactName;
            to.adoxio_SEPContactPhoneNumber = from.SEPContactPhoneNumber;
            // market events
            to.adoxio_IsNoPreventingSaleofLiquor = from.IsNoPreventingSaleofLiquor;
            to.adoxio_IsMarketManagedorCarried = from.IsMarketManagedorCarried;
            to.adoxio_IsMarketOnlyVendors = from.IsMarketOnlyVendors;
            to.adoxio_IsNoImportedGoods = from.IsNoImportedGoods;
            to.adoxio_IsMarketHostsSixVendors = from.IsMarketHostsSixVendors;
            to.adoxio_IsMarketMaxAmountorDuration = from.IsMarketMaxAmountorDuration;
            to.adoxio_MKTOrganizerContactName = from.MKTOrganizerContactName;
            to.adoxio_MKTOrganizerContactPhone = from.MKTOrganizerContactPhone;
            to.adoxio_RegistrationNumber = from.RegistrationNumber;
            to.adoxio_MarketName = from.MarketName;
            to.adoxio_MarketWebsite = from.MarketWebsite;
            to.adoxio_MarketDuration = from.MarketDuration.HasValue ? (adoxio_event_adoxio_marketduration?)(int)from.MarketDuration.Value : null;
            to.adoxio_IsAllStaffServingitRight = from.IsAllStaffServingitRight;
            to.adoxio_IsSalesAreaAvailandDefined = from.IsSalesAreaAvailandDefined;
            to.adoxio_IsSampleSizeCompliant = from.IsSampleSizeCompliant;
            to.adoxio_EventCategory = from.EventCategory.HasValue ? (adoxio_event_adoxio_eventcategory?)(int)from.EventCategory.Value : null;
            to.adoxio_MarketEventType = from.MarketEventType.HasValue ? (adoxio_event_adoxio_marketeventtype?)(int)from.MarketEventType.Value : null;
            // TUA events
            to.adoxio_Eventname = from.EventName;
            to.adoxio_TUAEventType = from.TuaEventType.HasValue ? (adoxio_event_adoxio_tuaeventtype?)(int)from.TuaEventType.Value : null;
            to.adoxio_IsClosedtoPublic = from.IsClosedToPublic;
            to.adoxio_IsWedding = from.IsWedding;
            to.adoxio_IsNetworkingParty = from.IsNetworkingParty;
            to.adoxio_IsConcert = from.IsConcert;
            to.adoxio_IsBanquet = from.IsBanquet;
            to.adoxio_IsAmplifiedSound = from.IsAmplifiedSound;
            to.adoxio_IsDancing = from.IsDancing;
            to.adoxio_IsReception = from.IsReception;
            to.adoxio_IsLiveEntertainment = from.IsLiveEntertainment;
            to.adoxio_IsGambling = from.IsGambling;
            to.adoxio_IsNoneoftheAbove = from.IsNoneOfTheAbove;
            to.adoxio_IsAgreement1 = from.IsAgreement1;
            to.adoxio_IsAgreement2 = from.IsAgreement2;
        }
    }
}
