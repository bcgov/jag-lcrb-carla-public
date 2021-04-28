using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SepServiceArea
    {
        public string Id { get; set; }
        public string SepLocationId { get; set; }
        public string SpecialEventId { get; set; }
        public bool? MinorPresent { get; set; }
        public int? LicencedAreaMaxNumberOfGuests { get; set; }
        public int? MaximumNumberOfGuests { get; set; }
        public bool? IsBothOutdoorIndoor { get; set; }
        public bool? IsIndoors { get; set; }
        public int? NumberOfMinors { get; set; }
        public int? LicencedAreaNumberOfMinors { get; set; }
        public int? Setting { get; set; }
        public int? StatusCode { get; set; }
        public int? StateCode { get; set; }
        public string EventName { get; set; }
        public bool? IsOutdoors { get; set; }
        public string LicencedAreaDescription { get; set; }
        public List<SepEventDates> EventDates { get; set; }

    }
}
