using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SepService.ViewModels
{
    public class Location
    {
        public Address Address { get; set; }
        public EventDate EventDate { get; set; }
        public string LocationDescription { get; set; }
        public string LocationName { get; set; }
        public int MaxGuests { get; set; }
    }
}
