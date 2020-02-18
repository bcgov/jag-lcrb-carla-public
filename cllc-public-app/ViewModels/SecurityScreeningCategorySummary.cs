using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SecurityScreeningCategorySummary
    {
        SecurityScreeningStatusItem [] OutstandingItems { get; set; }
        SecurityScreeningStatusItem [] CompletedItems { get; set; }
    }
}
