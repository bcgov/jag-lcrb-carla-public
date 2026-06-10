extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
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

        /// <summary>
        /// Converts a MicrosoftDynamicsCRMadoxioEvent entity into a LicenceEvent view model.
        /// </summary>
        /// <param name="item">The dynamics entity to convert.</param>
        /// <param name="dynamicsClient">The dynamics client.</param>
        /// <returns>The converted LicenceEvent view model.</returns>
        public static LicenceEvent ToViewModel(this MicrosoftDynamicsCRMadoxioEvent item, IDynamicsClient dynamicsClient)
        {
            LicenceEvent result = null;
            if (item != null)
            {
                result = new LicenceEvent();
                if (item.AdoxioEventid != null)
                {
                    result.Id = item.AdoxioEventid;
                }
                result.Status = (LicenceEventStatus?)item.Statuscode;
                result.Name = item.AdoxioName;
                result.StartDate = item.AdoxioEventstartdate;
                result.EndDate = item.AdoxioEventenddate;
                result.VenueDescription = item.AdoxioVenuenamedescription;
                result.AdditionalLocationInformation = item.AdoxioAdditionallocationinfo;
                result.FoodService = (FoodService?)item.AdoxioFoodservice;
                result.FoodServiceDescription = item.AdoxioFoodservicedescription;
                result.Entertainment = (Entertainment?)item.AdoxioEntertainment;
                result.EntertainmentDescription = item.AdoxioEntertainmentdescription;
                result.ContactPhone = item.AdoxioContactphonenumber;
                result.ExternalId = item.AdoxioExternalid;
                result.ContactName = item.AdoxioContactname;
                result.ContactEmail = item.AdoxioContactemail;
                result.EventNumber = item.AdoxioEventnumber;
                result.ClientHostname = item.AdoxioClienthostname;
                result.EventType = (EventType?)item.AdoxioEventtype;
                result.EventTypeDescription = item.AdoxioEventdescription;
                result.ImportSequenceNumber = item.Importsequencenumber;
                result.SpecificLocation = (SpecificLocation?)item.AdoxioSpecificlocation;
                result.EventClass = (EventClass?)item.AdoxioClass;
                result.MaxAttendance = item.AdoxioMaxattendance;
                result.MaxStaffAttendance = item.AdoxioMaxstaffattendance;
                result.MinorsAttending = item.AdoxioAttendanceminors;
                result.CommunityApproval = item.AdoxioCommunityapproval;
                result.NotifyEventInspector = item.AdoxioNotifyeventinspector;
                result.LicenceId = item._adoxioLicenceValue;
                result.AccountId = item._adoxioAccountValue;
                result.Street1 = item.AdoxioStreet1;
                result.Street2 = item.AdoxioStreet2;
                result.City = item.AdoxioCity;
                result.Province = item.AdoxioProvince;
                result.PostalCode = item.AdoxioPostalcode;
                result.ModifiedOn = item.Modifiedon;
                result.Schedules = new List<LicenceEventSchedule>();
                // Security Plan
                result.SecurityPlanRequested = item.AdoxioRequestsafetysecurityplan;
                result.EventLiquorLayout = item.AdoxioEventliquorlayout;
                result.DailyEventAttendees = item.AdoxioNumberdailyeventattendees;
                result.DailyMinorAttendees = item.AdoxioNumberdailyminorattendees;
                result.OccupantLoad = item.AdoxioEventoccupantload;
                result.OccupantLoadAvailable = item.AdoxioIseventloadavailable;
                result.OccupantLoadServiceArea = item.AdoxioEventoccupantloadservicesarea;
                result.OccupantLoadServiceAreaAvailable = item.AdoxioIsservicearealoadavailable;
                result.ServiceAreaControlledDetails = item.AdoxioEventliquorcontainment;
                result.StaffingManagers = item.AdoxioEventstaffingmanagers;
                result.StaffingBartenders = item.AdoxioEventstaffingbartenders;
                result.StaffingServers = item.AdoxioEventstaffingservers;
                result.SecurityPersonnel = item.AdoxioSecuritycompanysummary;
                result.SecurityPersonnelThroughCompany = item.AdoxioSecuritypersonnelnumberhired;
                result.SecurityCompanyName = item.AdoxioSecuritycompanyname;
                result.SecurityCompanyAddress = item.AdoxioSecuritycompanystreet;
                result.SecurityCompanyCity = item.AdoxioSecuritycompanycity;
                result.SecurityCompanyPostalCode = item.AdoxioSecuritycompanypostal;
                result.SecurityCompanyContactPerson = item.AdoxioSecuritycompanycontactname;
                result.SecurityCompanyPhoneNumber = item.AdoxioSecuritycompanycontactphone;
                result.SecurityCompanyEmail = item.AdoxioSecuritycompanycontactemail;
                result.SecurityPoliceOfficerSummary = item.AdoxioPoliceofficersummary;
                result.SafeAndResponsibleMinorsNotAttending = item.AdoxioIsminorsattending;
                result.SafeAndResponsibleLiquorAreaControlled = item.AdoxioIsliquorareacontrolled;
                result.SafeAndResponsibleLiquorAreaControlledDescription = item.AdoxioLiquorareacontrolleddetails;
                result.SafeAndResponsibleMandatoryID = item.AdoxioIstwopiecesidrequired;
                result.SafeAndResponsibleSignsAdvisingMinors = item.AdoxioIssignsadvisingminors;
                result.SafeAndResponsibleMinorsOther = item.AdoxioIsotherminorssafety;
                result.SafeAndResponsibleMinorsOtherDescription = item.AdoxioIsotherminorssafetydetails;
                result.SafeAndResponsibleSignsAdvisingRemoval = item.AdoxioIssignsintoxicatedpersons;
                result.SafeAndResponsibleSignsAdvisingTwoDrink = item.AdoxioIssignstwodrinkmax;
                result.SafeAndResponsibleOverConsumptionOther = item.AdoxioIsotherconsumptionsafety;
                result.SafeAndResponsibleOverConsumptionOtherDescription = item.AdoxioIsotherconsumptionsafetydetails;
                result.SafeAndResponsibleReadAppendix2 = item.AdoxioIsdisturbanceappendix2;
                result.SafeAndResponsibleDisturbancesOther = item.AdoxioIsotherdisturbance;
                result.SafeAndResponsibleDisturbancesOtherDescription = item.AdoxioIsotherdisturbancedetails;
                result.SafeAndResponsibleAdditionalSafetyMeasures = item.AdoxioAdditionalsafetydetails;
                result.SafeAndResponsibleServiceAreaSupervision = item.AdoxioServiceareaentrancesupervisiondetails;
                result.DeclarationIsAccurate = item.AdoxioIsdeclarationaccurate;
                result.SecurityPlanSubmitted = item.AdoxioSafetysecurityplanchangessubmitted;
                result.SEPLicensee = item.AdoxioSeplicensee;
                result.SEPLicenceNumber = item.AdoxioSeplicencenumber;
                result.SEPContactName = item.AdoxioSepcontactname;
                result.SEPContactPhoneNumber = item.AdoxioSepcontactphonenumber;
                //market events
                result.IsNoPreventingSaleofLiquor = item.AdoxioIsnopreventingsaleofliquor;
                result.IsMarketManagedorCarried = item.AdoxioIsmarketmanagedorcarried;
                result.IsMarketOnlyVendors = item.AdoxioIsmarketonlyvendors;
                result.IsNoImportedGoods = item.AdoxioIsnoimportedgoods;
                result.IsMarketHostsSixVendors = item.AdoxioIsmarkethostssixvendors;
                result.IsMarketMaxAmountorDuration = item.AdoxioIsmarketmaxamountorduration;
                result.MKTOrganizerContactName = item.AdoxioMktorganizercontactname;
                result.MKTOrganizerContactPhone = item.AdoxioMktorganizercontactphone;
                result.RegistrationNumber = item.AdoxioRegistrationnumber;
                result.BusinessNumber = item.AdoxioBusinessnumber;
                result.MarketName = item.AdoxioMarketname;
                result.MarketWebsite = item.AdoxioMarketwebsite;
                result.MarketDuration = (MarketDuration?)item.AdoxioMarketduration;
                result.IsAllStaffServingitRight = item.AdoxioIsallstaffservingitright;
                result.IsSalesAreaAvailandDefined = item.AdoxioIssalesareaavailanddefined;
                result.IsSampleSizeCompliant = item.AdoxioIssamplesizecompliant;
                result.EventCategory = (EventCategory?)item.AdoxioEventcategory;
                result.MarketEventType = (MarketEventType?)item.AdoxioMarketeventtype;

                // temporary use area (TUA) events
                result.EventName = item.AdoxioEventname;
                result.TuaEventType = (TuaEventType?)item.AdoxioTuaeventtype;
                result.IsClosedToPublic = item.AdoxioIsclosedtopublic;
                result.IsWedding = item.AdoxioIswedding;
                result.IsNetworkingParty = item.AdoxioIsnetworkingparty;
                result.IsConcert = item.AdoxioIsconcert;
                result.IsBanquet = item.AdoxioIsbanquet;
                result.IsAmplifiedSound = item.AdoxioIsamplifiedsound;
                result.IsDancing = item.AdoxioIsdancing;
                result.IsReception = item.AdoxioIsreception;
                result.IsLiveEntertainment = item.AdoxioIsliveentertainment;
                result.IsGambling = item.AdoxioIsgambling;
                result.IsNoneOfTheAbove = item.AdoxioIsnoneoftheabove;
                result.IsAgreement1 = item.AdoxioIsagreement1;
                result.IsAgreement2 = item.AdoxioIsagreement2;
                result.EventLocations = new List<LicenceEventLocation>();
            }

            MicrosoftDynamicsCRMadoxioEventscheduleCollection eventSchedules = dynamicsClient.GetEventSchedulesByEventId(result.Id);
            foreach (var schedule in eventSchedules.Value)
            {
                result.Schedules.Add(schedule.ToViewModel());
            }

            // TUA event locations
            MicrosoftDynamicsCRMadoxioEventlocationCollection eventLocations = dynamicsClient.GetEventLocationsByEventId(result.Id);
            foreach (var loc in eventLocations?.Value)
            {
                result.EventLocations.Add(loc.ToViewModel());
            }

            return result;
        }


        /// <summary>
        /// Copies values from a LicenceEvent view model to a MicrosoftDynamicsCRMadoxioEvent entity.
        /// </summary>
        /// <param name="to">The dynamics entity to copy values to.</param>
        /// <param name="from">The view model to copy values from.</param>
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioEvent to, LicenceEvent from)
        {
            to.AdoxioEventid = from.Id;
            to.AdoxioName = from.Name;
            to.Statuscode = (int?)from.Status;
            if (from.StartDate.HasValue)
            {
                DateTimeOffset oldStart = (DateTimeOffset)from.StartDate;
                to.AdoxioEventstartdate = oldStart;
                /*DateTimeOffset startDate = new DateTimeOffset(oldStart.Year, oldStart.Month, oldStart.Day, 0, 0, 0, new TimeSpan(0, 0, 0));
                to.AdoxioStartdate = startDate;*/
            }
            if (from.EndDate.HasValue)
            {
                DateTimeOffset oldEnd = (DateTimeOffset)from.EndDate;
                to.AdoxioEventenddate = oldEnd;
                /*DateTimeOffset endDate = new DateTimeOffset(oldEnd.Year, oldEnd.Month, oldEnd.Day, 0, 0, 0, TimeZone.CurrentTimeZone);
                to.AdoxioEnddate = endDate;*/
            }
            to.AdoxioVenuenamedescription = from.VenueDescription;
            to.AdoxioAdditionallocationinfo = from.AdditionalLocationInformation;
            to.AdoxioFoodservice = (int?)from.FoodService;
            to.AdoxioFoodservicedescription = from.FoodServiceDescription;
            to.AdoxioEntertainment = (int?)from.Entertainment;
            to.AdoxioEntertainmentdescription = from.EntertainmentDescription;
            to.AdoxioContactphonenumber = from.ContactPhone;
            to.AdoxioContactname = from.ContactName;
            to.AdoxioExternalid = from.ExternalId;
            to.AdoxioContactemail = from.ContactEmail;
            to.AdoxioEventnumber = from.EventNumber;
            to.AdoxioClienthostname = from.ClientHostname;
            to.AdoxioEventtype = (int?)from.EventType;
            to.AdoxioEventdescription = from.EventTypeDescription;
            to.Importsequencenumber = from.ImportSequenceNumber;
            to.AdoxioSpecificlocation = (int?)from.SpecificLocation;
            to.AdoxioClass = (int?)from.EventClass;
            to.AdoxioMaxattendance = from.MaxAttendance;
            to.AdoxioMaxstaffattendance = from.MaxStaffAttendance;
            to.AdoxioAttendanceminors = from.MinorsAttending;
            to.AdoxioCommunityapproval = from.CommunityApproval;
            to.AdoxioNotifyeventinspector = from.NotifyEventInspector;
            to.AdoxioStreet1 = from.Street1;
            to.AdoxioStreet2 = from.Street2;
            to.AdoxioCity = from.City;
            to.AdoxioProvince = from.Province;
            to.AdoxioPostalcode = from.PostalCode;

            // Security Plan
            to.AdoxioRequestsafetysecurityplan = from.SecurityPlanRequested;
            to.AdoxioEventliquorlayout = from.EventLiquorLayout;
            to.AdoxioNumberdailyeventattendees = from.DailyEventAttendees;
            to.AdoxioNumberdailyminorattendees = from.DailyMinorAttendees;
            to.AdoxioEventoccupantload = from.OccupantLoad;
            to.AdoxioIseventloadavailable = from.OccupantLoadAvailable;
            to.AdoxioEventoccupantloadservicesarea = from.OccupantLoadServiceArea;
            to.AdoxioIsservicearealoadavailable = from.OccupantLoadServiceAreaAvailable;
            to.AdoxioEventliquorcontainment = from.ServiceAreaControlledDetails;
            to.AdoxioEventstaffingmanagers = from.StaffingManagers;
            to.AdoxioEventstaffingbartenders = from.StaffingBartenders;
            to.AdoxioEventstaffingservers = from.StaffingServers;
            to.AdoxioSecuritycompanysummary = from.SecurityPersonnel;
            to.AdoxioSecuritypersonnelnumberhired = from.SecurityPersonnelThroughCompany;
            to.AdoxioSecuritycompanyname = from.SecurityCompanyName;
            to.AdoxioSecuritycompanystreet = from.SecurityCompanyAddress;
            to.AdoxioSecuritycompanycity = from.SecurityCompanyCity;
            to.AdoxioSecuritycompanypostal = from.SecurityCompanyPostalCode;
            to.AdoxioSecuritycompanycontactname = from.SecurityCompanyContactPerson;
            to.AdoxioSecuritycompanycontactphone = from.SecurityCompanyPhoneNumber;
            to.AdoxioSecuritycompanycontactemail = from.SecurityCompanyEmail;
            to.AdoxioPoliceofficersummary = from.SecurityPoliceOfficerSummary;
            to.AdoxioIsminorsattending = from.SafeAndResponsibleMinorsNotAttending;
            to.AdoxioIsliquorareacontrolled = from.SafeAndResponsibleLiquorAreaControlled;
            to.AdoxioLiquorareacontrolleddetails = from.SafeAndResponsibleLiquorAreaControlledDescription;
            to.AdoxioIstwopiecesidrequired = from.SafeAndResponsibleMandatoryID;
            to.AdoxioIssignsadvisingminors = from.SafeAndResponsibleSignsAdvisingMinors;
            to.AdoxioIsotherminorssafety = from.SafeAndResponsibleMinorsOther;
            to.AdoxioIsotherminorssafetydetails = from.SafeAndResponsibleMinorsOtherDescription;
            to.AdoxioIssignsintoxicatedpersons = from.SafeAndResponsibleSignsAdvisingRemoval;
            to.AdoxioIssignstwodrinkmax = from.SafeAndResponsibleSignsAdvisingTwoDrink;
            to.AdoxioIsotherconsumptionsafety = from.SafeAndResponsibleOverConsumptionOther;
            to.AdoxioIsotherconsumptionsafetydetails = from.SafeAndResponsibleOverConsumptionOtherDescription;
            to.AdoxioIsdisturbanceappendix2 = from.SafeAndResponsibleReadAppendix2;
            to.AdoxioIsotherdisturbance = from.SafeAndResponsibleDisturbancesOther;
            to.AdoxioIsotherdisturbancedetails = from.SafeAndResponsibleDisturbancesOtherDescription;
            to.AdoxioAdditionalsafetydetails = from.SafeAndResponsibleAdditionalSafetyMeasures;
            to.AdoxioServiceareaentrancesupervisiondetails = from.SafeAndResponsibleServiceAreaSupervision;
            to.AdoxioIsdeclarationaccurate = from.DeclarationIsAccurate;

            to.AdoxioSepcontactphonenumber = from.SEPContactPhoneNumber;
            to.AdoxioSepcontactname = from.SEPContactName;
            to.AdoxioSeplicencenumber = from.SEPLicenceNumber;
            to.AdoxioSeplicensee = from.SEPLicensee;

            to.AdoxioSafetysecurityplanchangessubmitted = from.SecurityPlanSubmitted;

            // market events
            to.AdoxioIsnopreventingsaleofliquor = from.IsNoPreventingSaleofLiquor;
            to.AdoxioIsmarketmanagedorcarried = from.IsMarketManagedorCarried;
            to.AdoxioIsmarketonlyvendors = from.IsMarketOnlyVendors;
            to.AdoxioIsnoimportedgoods = from.IsNoImportedGoods;
            to.AdoxioIsmarkethostssixvendors = from.IsMarketHostsSixVendors;
            to.AdoxioIsmarketmaxamountorduration = from.IsMarketMaxAmountorDuration;
            to.AdoxioMktorganizercontactname = from.MKTOrganizerContactName;
            to.AdoxioMktorganizercontactphone = from.MKTOrganizerContactPhone;
            to.AdoxioRegistrationnumber = from.RegistrationNumber;
            to.AdoxioMarketname = from.MarketName;
            to.AdoxioMarketwebsite = from.MarketWebsite;
            to.AdoxioMarketduration = (int?)from.MarketDuration;
            to.AdoxioIsallstaffservingitright = from.IsAllStaffServingitRight;
            to.AdoxioIssalesareaavailanddefined = from.IsSalesAreaAvailandDefined;
            to.AdoxioIssamplesizecompliant = from.IsSampleSizeCompliant;
            to.AdoxioEventcategory = (int?)from.EventCategory;
            to.AdoxioMarketeventtype = (int?)from.MarketEventType;

            // TUA events
            to.AdoxioEventname = from.EventName;
            to.AdoxioTuaeventtype = (int?)from.TuaEventType;
            to.AdoxioIsclosedtopublic = from.IsClosedToPublic;
            to.AdoxioIswedding = from.IsWedding;
            to.AdoxioIsnetworkingparty = from.IsNetworkingParty;
            to.AdoxioIsconcert = from.IsConcert;
            to.AdoxioIsbanquet = from.IsBanquet;
            to.AdoxioIsamplifiedsound = from.IsAmplifiedSound;
            to.AdoxioIsdancing = from.IsDancing;
            to.AdoxioIsreception = from.IsReception;
            to.AdoxioIsliveentertainment = from.IsLiveEntertainment;
            to.AdoxioIsgambling = from.IsGambling;
            to.AdoxioIsnoneoftheabove = from.IsNoneOfTheAbove;
            to.AdoxioIsagreement1 = from.IsAgreement1;
            to.AdoxioIsagreement2 = from.IsAgreement2;
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
