using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SepEventLocation
    {
        public int Id { get; set; } // Client side local db id
        public string LocationId { get; set; }
        public string SpecialEventId { get; set; }
        public string LocationDescription { get; set; }
        public string EventLocationCity { get; set; }
        public string EventLocationPostalCode { get; set; }
        public string EventLocationStreet1 { get; set; }
        public string EventLocationStreet2 { get; set; }
        public string EventLocationProvince { get; set; }
        public string MaximumNumberOfGuests { get; set; }
        public string LocationName { get; set; }
        public string PermitNumber { get; set; }
         public List<SepServiceArea> ServiceAreas { get; set; }
        public List<SepEventDates> EventDates { get; set; }
        public int? Statecode { get; set; }

    }
}
