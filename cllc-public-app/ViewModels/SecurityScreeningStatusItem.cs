using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SecurityScreeningStatusItem
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset? DateSubmitted { get; set; }
        public string ScreeningLink { get; set; }
    }
}
