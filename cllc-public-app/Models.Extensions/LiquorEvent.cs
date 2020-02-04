using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class LiquorEventExtensions
    {
        // Converts a dynamics entity into a view model
        public static ViewModels.LiquorEvent ToViewModel(this MicrosoftDynamicsCRMadoxioEvent item)
        {
            ViewModels.LiquorEvent result = null;
            if (item != null)
            {
                result = new ViewModels.LiquorEvent();
                if (item.AdoxioEventid != null)
                {
                    result.Id = item.AdoxioEventid;
                }
                result.Name = item.AdoxioName;
                result.StartDate = item.AdoxioStartdate;
                result.EndDate = item.AdoxioEnddate;
                result.VenueDescription = item.AdoxioVenuedescription;
                result.AdditionalLocationInformation = item.AdoxioAdditionallocationinformation;
                result.FoodService = (FoodService?)item.AdoxioFoodservice;
                result.FoodServiceDescription = item.AdoxioFoodservicedescription;
                result.Entertainment = (Entertainment?)item.AdoxioEntertainment;
                result.EntertainmentDescription = item.AdoxioEntertainmentdescription;
                result.ContactPhone = item.AdoxioContactphone;
                result.ExternalId = item.AdoxioExternalid;
                result.ContactName = item.AdoxioContactname;
                result.ContactEmail = item.AdoxioContactemail;
                result.EventNumber = item.AdoxioEventnumber;
                result.ClientHostname = item.AdoxioClienthostname;
                result.EventType = (EventType?)item.AdoxioEventtype;
                result.EventTypeDescription = item.AdoxioEventtypedescription;
                result.ImportSequenceNumber = item.Importsequencenumber;
                result.SpecificLocation = (SpecificLocation?)item.AdoxioSpecificlocation;
                result.EventClass = (EventClass?)item.AdoxioClass;
                result.MaxAttendance = item.AdoxioMaxattendance;
                result.CommunityApproval = item.AdoxioCommunityapproval;
                result.NotifyEventInspector = item.AdoxioNotifyeventinspector;
                result.LicenceId = item._adoxioLicenceValue;
                result.AccountId = item._adoxioAccountValue;
                result.Street1 = item.AdoxioStreet1;
                result.Street2 = item.AdoxioStreet2;
                result.City = item.AdoxioCity;
                result.Province = item.AdoxioProvince;
                result.PostalCode = item.AdoxioPostalcode;
            }

            return result;
        }


        // Converts a view model into a dynamics entity
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioEvent to, ViewModels.LiquorEvent from)
        {
            to.AdoxioEventid = from.Id;
            to.AdoxioName = from.Name;
            to.AdoxioStartdate = from.StartDate;
            to.AdoxioEnddate = from.EndDate;
            to.AdoxioVenuedescription = from.VenueDescription;
            to.AdoxioAdditionallocationinformation = from.AdditionalLocationInformation;
            to.AdoxioFoodservice = (int?)from.FoodService;
            to.AdoxioFoodservicedescription = from.FoodServiceDescription;
            to.AdoxioEntertainment = (int?)from.Entertainment;
            to.AdoxioEntertainmentdescription = from.EntertainmentDescription;
            to.AdoxioContactphone = from.ContactPhone;
            to.AdoxioContactname = from.ContactName;
            to.AdoxioExternalid = from.ExternalId;
            to.AdoxioContactemail = from.ContactEmail;
            to.AdoxioEventnumber = from.EventNumber;
            to.AdoxioClienthostname = from.ClientHostname;
            to.AdoxioEventtype = (int?)from.EventType;
            to.AdoxioEventtypedescription = from.EventTypeDescription;
            to.Importsequencenumber = from.ImportSequenceNumber;
            to.AdoxioSpecificlocation = (int?)from.SpecificLocation;
            to.AdoxioClass = (int?)from.EventClass;
            to.AdoxioMaxattendance = from.MaxAttendance;
            to.AdoxioCommunityapproval = from.CommunityApproval;
            to.AdoxioNotifyeventinspector = from.NotifyEventInspector;
            to.AdoxioStreet1 = from.Street1;
            to.AdoxioStreet2 = from.Street2;
            to.AdoxioCity = from.City;
            to.AdoxioProvince = from.Province;
            to.AdoxioPostalcode = from.PostalCode;
        }
    }
}
