using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Models;

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
                    EventLocations = new List<ViewModels.SepEventLocation>()
                };
                result.Id = specialEvent.AdoxioSpecialeventid;

                result.AdmissionFee = specialEvent.AdoxioAdmissionfee;
                result.StepsCompleted = specialEvent.Adoxio
                result.BeerGarden = specialEvent.AdoxioBeergarden;
                result.Capacity = specialEvent.AdoxioCapacity;
                result.ChargingForLiquorReason = (ViewModels.ChargingForLiquorReasons?)specialEvent.AdoxioChargingforliquorreason;
                result.DateSubmitted = specialEvent.AdoxioDatesubmitted;
                result.DrinksIncluded = specialEvent.AdoxioDrinksincluded;
                result.DonateOrConsular = (ViewModels.DonatedOrConsular?)specialEvent.AdoxioDonatedorconsular;
                result.EventEndDate = specialEvent.AdoxioEventenddate;
                result.EventName = specialEvent.AdoxioEventname;
                result.EventStartDate = specialEvent.AdoxioEventstartdate;
                result.FundRaisingPurpose = (ViewModels.FundRaisingPurposes?)specialEvent.AdoxioFundraisingpurpose;
                result.HostOrganizationAddress = specialEvent.AdoxioHostorganisationaddress;
                result.HostOrganizationCategory = (ViewModels.HostOrgCatergory?)specialEvent.AdoxioHostorganisationcategory;
                result.HostOrganizationName = specialEvent.AdoxioHostorganisationname;
                result.HowProceedsWillBeUsedDescription = specialEvent.AdoxioHowproceedswillbeuseddescription;
                result.IsAgreeToTnC = specialEvent.AdoxioIsagreetsandcs;
                result.DateAgreeToTnC = specialEvent.AdoxioDateagreedtotsandcs;e
                result.IsAnnualEvent = specialEvent.AdoxioIsannualevent;
                result.IsMajorSignigicance = specialEvent.AdoxioIsmajorsignificance;
                result.IsGstRegisteredOrg = specialEvent.AdoxioIsgstregisteredorg;
                result.IsManufacturingExclusivity  = specialEvent.AdoxioIsmanufacturingexclusivity;
                result.MajorSignigicanceRationale = specialEvent.AdoxioMajorsignificancerationale;
                result.MaximumNumberOfGuests = specialEvent.AdoxioMaxnumofguests;
                result.NonProfitName = specialEvent.AdoxioNonprofitname;
                result.PoliceApproval = specialEvent.AdoxioPoliceapproval;
                result.PrivateOrPublic = (ViewModels.SEPPublicOrPrivate?)specialEvent.AdoxioTypeofevent;
                result.SpecialEventCity = specialEvent.AdoxioSpecialeventcity;
                result.SpecialEventDescripton = specialEvent.AdoxioSpecialeventdescripton;
                result.SpecialEventPermitNumber = specialEvent.AdoxioSpecialeventpermitnumber;
                result.SpecialEventPostalCode = specialEvent.AdoxioSpecialeventpostalcode;
                result.SpecialEventProvince = specialEvent.AdoxioSpecialeventprovince;
                result.SpecialEventStreet1 = specialEvent.AdoxioSpecialeventstreet1;
                result.SpecialEventStreet2 = specialEvent.AdoxioSpecialeventstreet2;
                result.Statecode = specialEvent.Statecode;
                result.Statuscode = specialEvent.Statuscode; // Event Status: Draft, Submitted, Pending Review, etc.
                result.TastingEvent = specialEvent.AdoxioTastingevent;
                result.TotalServings = specialEvent.AdoxioTotalservings;

                if (specialEvent?.AdoxioSpecialeventSpecialeventlocations?.Count > 0)
                {
                    result.EventLocations = specialEvent.AdoxioSpecialeventSpecialeventlocations
                        .Select(specialEvent => specialEvent.ToViewModel())
                        .ToList();
                }
            }
            return result;
        }

        public static void CopyValues(this MicrosoftDynamicsCRMadoxioSpecialevent to, ViewModels.SpecialEvent from)
        {
            to.AdoxioSpecialeventid = from.Id;
            to.AdoxioAdmissionfee = from.AdmissionFee;
            to.AdoxioBeergarden = from.BeerGarden;
            to.AdoxioCapacity = from.Capacity;
            to.AdoxioChargingforliquorreason = (int?)from.ChargingForLiquorReason;
            to.AdoxioDatesubmitted = from.DateSubmitted;
            to.AdoxioDrinksincluded = from.DrinksIncluded;
            to.AdoxioDonatedorconsular = (int?)from.DonateOrConsular;
            to.AdoxioEventenddate = from.EventEndDate;
            to.AdoxioEventname = from.EventName;
            to.AdoxioEventstartdate = from.EventStartDate;
            to.AdoxioFundraisingpurpose = (int?)from.FundRaisingPurpose;
            to.AdoxioHowproceedswillbeuseddescription = from.HowProceedsWillBeUsedDescription;
            to.AdoxioIsagreetsandcs = from.IsAgreeToTnC;
            to.AdoxioDateagreedtotsandcs = from.DateAgreeToTnC;
            to.AdoxioIsannualevent  = from.IsAnnualEvent;
            to.AdoxioIslocationlicensedos = (int?)from.IsLocationLicensed;
            to.AdoxioIsmajorsignificance =  from.IsMajorSignigicance;
            to.AdoxioIsgstregisteredorg = from.IsGstRegisteredOrg;
            to.AdoxioIsmanufacturingexclusivity = from.IsManufacturingExclusivity;
            to.AdoxioMajorsignificancerationale =  from.MajorSignificanceRationale;
            to.AdoxioMaxnumofguests = from.MaximumNumberOfGuests;
            to.AdoxioNonprofitname = from.NonProfitName;
            to.AdoxioPoliceapproval = from.PoliceApproval;
            to.AdoxioPrivateorpublic = (int?)from.PrivateOrPublic;
            to.AdoxioSpecialeventcity = from.SpecialEventCity;
            to.AdoxioSpecialeventdescripton = from.SpecialEventDescripton;
            to.AdoxioSpecialeventpermitnumber = from.SpecialEventPermitNumber;
            to.AdoxioSpecialeventpostalcode = from.SpecialEventPostalCode;
            to.AdoxioSpecialeventprovince = from.SpecialEventProvince;
            to.AdoxioSpecialeventstreet1 = from.SpecialEventStreet1;
            to.AdoxioSpecialeventstreet2 = from.SpecialEventStreet2;
            to.AdoxioTastingevent = from.TastingEvent;
            to.AdoxioTotalservings = from.TotalServings;
            // to.Statecode = from.Statecode;
            to.Statuscode = from.Statuscode; // Event Status: Draft, Submitted, Pending Review, etc.
        }

    }
}

