
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SepService.ViewModels
{
    public enum LocationSetting
    {
        Indoor = 845280000,
        Outdoor  = 845280001,
        Both = 845280002
    }

    public class Location
    {
        public Address Address { get; set; }
        public List<EventDate> EventDates { get; set; }
        public string LocationDescription { get; set; }
        public string LocationName { get; set; }
        public int MaximumGuests { get; set; }

        public LicencedArea LicencedArea { get; set; }

 
        

    }
}
