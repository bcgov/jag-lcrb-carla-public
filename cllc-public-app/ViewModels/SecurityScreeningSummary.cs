using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SecurityScreeningSummary
    {
        SecurityScreeningCategorySummary[] Cannabis { get; set; }

        SecurityScreeningCategorySummary[] Liquor { get; set; }
    }
}
