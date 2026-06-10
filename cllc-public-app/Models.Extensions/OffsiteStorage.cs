extern alias DV;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using adoxio_offsitestorage = DV::Gov.Lclb.Cllb.Interfaces.adoxio_offsitestorage;

namespace Gov.Lclb.Cllb.Public.Models
{
    public static class OffsiteStorageExtensions
    {
        public static OffsiteStorage ToViewModel(this adoxio_offsitestorage storage)
        {
            return new OffsiteStorage
            {
                Id = storage.adoxio_offsitestorageId?.ToString(),
                Name = storage.adoxio_name,
                Street1 = storage.adoxio_Street1,
                City = storage.adoxio_City,
                PostalCode = storage.adoxio_PostalCode,
                Status = storage.statuscode != null ? (OffsiteStorageStatus)(int)storage.statuscode : null,
                DateAdded = storage.adoxio_DateAdded.HasValue ? new DateTimeOffset(storage.adoxio_DateAdded.Value, TimeSpan.Zero) : null,
                DateRemoved = storage.adoxio_DateRemoved.HasValue ? new DateTimeOffset(storage.adoxio_DateRemoved.Value, TimeSpan.Zero) : null
            };
        }

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