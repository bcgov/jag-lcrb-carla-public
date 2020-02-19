using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SecurityScreeningCategorySummary
    {
        public SecurityScreeningStatusItem[] OutstandingItems { get; set; }
        public SecurityScreeningStatusItem[] CompletedItems { get; set; }
    }
}
