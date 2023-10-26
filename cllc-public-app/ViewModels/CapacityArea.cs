using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum AdoxioAreaCategories
    {
        Service = 845280000,
        OutdoorArea = 845280001,
        Capacity = 845280002
    }

    public class CapacityArea
    {
        // string form of the guid.
        public string Id { get; set; }
        public int AreaNumber { get; set; }
        public string AreaLocation { get; set; }
        public int? AreaCategory { get; set; }
        public bool IsIndoor { get; set; }
        public bool IsOutdoor { get; set; }
        public bool IsPatio { get; set; }
        public int? Capacity { get; set; }
        public bool IsTemporaryExtensionArea { get; set; }
    }
}
