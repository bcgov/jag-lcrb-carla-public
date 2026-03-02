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
    }
}
