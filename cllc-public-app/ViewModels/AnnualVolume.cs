using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class AnnualVolume
    {
        public string? Id { get; set; }
        public string? ApplicationId { get; set; }
        public string? LicenceId { get; set; }
        public string CalendarYear { get; set; }
        public int? VolumeProduced { get; set; }
        public int? VolumeDestroyed { get; set; }
    }
}
