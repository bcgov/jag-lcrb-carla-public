using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SecurityScreeningSummary
    {
        public SecurityScreeningCategorySummary Cannabis { get; set; }

        public SecurityScreeningCategorySummary Liquor { get; set; }
    }
}
