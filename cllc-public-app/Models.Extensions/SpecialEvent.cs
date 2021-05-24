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
                result.BeerGarden = specialEvent.AdoxioBeergarden;
                result.Capacity = specialEvent.AdoxioCapacity;
                result.DateSubmitted = specialEvent.AdoxioDatesubmitted;
                result.DrinksIncluded = specialEvent.AdoxioDrinksincluded;
                result.EventEndDate = specialEvent.AdoxioEventenddate;
                result.EventName = specialEvent.AdoxioEventname;
                result.EventStartDate = specialEvent.AdoxioEventstartdate;
                result.MaximumNumberOfGuests = specialEvent.AdoxioMaxnumofguests;
                result.PoliceApproval = specialEvent.AdoxioPoliceapproval;
                result.IsAnnualEvent = specialEvent.AdoxioIsannualevent;
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
                result.PrivateOrPublic = (ViewModels.SEPPublicOrPrivate?)specialEvent.AdoxioTypeofevent;
                result.IsMajorSignigicance = specialEvent.AdoxioIsmajorsignificance;
                result.MajorSignigicanceRationale = specialEvent.AdoxioMajorsignificancerationale;
                result.HostOrganizationName = specialEvent.AdoxioHostorganisationname;
                result.HostOrganizationCategory = specialEvent.AdoxioHostorganisationcategory;
                result.HostOrganizationAddress = specialEvent.AdoxioHostorganisationaddress;

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
            to.AdoxioEventstartdate = from.EventStartDate;
            to.AdoxioEventname = from.EventName;
            to.AdoxioSpecialeventpostalcode = from.SpecialEventPostalCode;
            to.Statecode = from.Statecode;
            to.AdoxioBeergarden = from.BeerGarden;
            to.AdoxioTastingevent = from.TastingEvent;
            to.AdoxioSpecialeventprovince = from.SpecialEventProvince;
            to.AdoxioPrivateorpublic = (int?)from.PrivateOrPublic;
            to.AdoxioSpecialeventdescripton = from.SpecialEventDescripton;
            to.AdoxioCapacity = from.Capacity;
            to.AdoxioIsannualevent  = from.IsAnnualEvent;
            to.AdoxioDrinksincluded = from.DrinksIncluded;
            to.AdoxioSpecialeventpermitnumber = from.SpecialEventPermitNumber;
            to.AdoxioSpecialeventcity = from.SpecialEventCity;
            to.AdoxioSpecialeventstreet2 = from.SpecialEventStreet2;
            to.AdoxioEventenddate = from.EventEndDate;
            to.Statuscode = from.Statuscode; // Event Status: Draft, Submitted, Pending Review, etc.
            to.AdoxioIsmajorsignificance =  from.IsMajorSignigicance;
            to.AdoxioMajorsignificancerationale =  from.MajorSignificanceRationale;
            to.AdoxioSpecialeventstreet1 = from.SpecialEventStreet1;
            to.AdoxioMaxnumofguests = from.MaximumNumberOfGuests;
            to.AdoxioDatesubmitted = from.DateSubmitted;
            to.AdoxioPoliceapproval = from.PoliceApproval;
        }

    }
}

