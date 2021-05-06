using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public class SpecialEvent
    {
        public string Id { get; set; }
        public bool? AdmissionFee { get; set; }
        public System.DateTimeOffset? EventStartDate { get; set; }
        public string EventName { get; set; }
        public string SpecialEventPostalCode { get; set; }
        public int? Statecode { get; set; }
        public bool? BeerGarden { get; set; }
        public bool? TastingEvent { get; set; }
        public string SpecialEventProvince { get; set; }
        public int? TypeOfEvent { get; set; }
        public string SpecialEventDescripton { get; set; }
        public int? Capacity { get; set; }
        public bool? DrinksIncluded { get; set; }
        public string SpecialEventPermitNumber { get; set; }
        public string SpecialEventCity { get; set; }
        public string SpecialEventStreet2 { get; set; }
        public System.DateTimeOffset? EventEndDate { get; set; }
        public int? Statuscode { get; set; }
        public string SpecialEventStreet1 { get; set; }
        public int? MaximumNumberOfGuests { get; set; }
        public List<SepEventLocation> EventLocations { get; set; }
    }
}
