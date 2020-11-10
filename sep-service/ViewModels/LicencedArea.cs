using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SepService.ViewModels
{
    public class LicencedArea
    {
        public string Description { get; set; }
        public int MaxGuests { get; set; }
        public bool MinorsPresent { get; set; }
        public int NumberOfMinors { get; set; }
        public string Setting { get; set; }
    }
}
