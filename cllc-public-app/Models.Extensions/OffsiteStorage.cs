using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class OffsiteStorageExtensions
    {
        // Converts a dynamics entity into a view model
        public static OffsiteStorage ToViewModel(this MicrosoftDynamicsCRMadoxioOffsitestorage item)
        {
            OffsiteStorage result = null;
            if (item != null)
            {
                result = new OffsiteStorage
                {
                    Name = item.AdoxioName,
                    Status = (OffsiteStorageStatus?)item.Statuscode,
                    Street1 = item.AdoxioStreet1,
                    City = item.AdoxioCity,
                    PostalCode = item.AdoxioPostalcode,
                    DateAdded = item.AdoxioDateadded,
                    DateRemoved = item.AdoxioDateremoved
                };

                if (item.AdoxioOffsitestorageid != null)
                {
                    result.Id = item.AdoxioOffsitestorageid;
                }
            }
            return result;
        }

        // Converts a view model into a dynamics entity
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioOffsitestorage to, OffsiteStorage from)
        {
            if (from.Id != null)
            {
                to.AdoxioOffsitestorageid = from.Id;
            }
            to.Statuscode = (int?)from.Status;
            to.AdoxioName = from.Name;
            to.AdoxioStreet1 = from.Street1;
            to.AdoxioCity = from.City;
            to.AdoxioPostalcode = from.PostalCode;
            to.AdoxioDateadded = from.DateAdded;
            to.AdoxioDateremoved = from.DateRemoved;
        }
    }
}