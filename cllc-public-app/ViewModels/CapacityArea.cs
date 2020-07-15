using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class CapacityArea
    {
        public int AreaNumber { get; set;  }
        public string AreaLocation { get; set; }
        public bool IsIndoor { get; set; }
        public bool IsOutdoor { get; set; }
        public bool IsPatio { get; set; }
        public int? Capacity { get; set; }
    }
}
