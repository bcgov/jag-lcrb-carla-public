extern alias DV;
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
    }
}
