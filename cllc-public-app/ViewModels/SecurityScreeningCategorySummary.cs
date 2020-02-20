using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SecurityScreeningCategorySummary
    {
        public List<SecurityScreeningStatusItem> OutstandingItems { get; set; }
        public List<SecurityScreeningStatusItem> CompletedItems { get; set; }
    }
}
