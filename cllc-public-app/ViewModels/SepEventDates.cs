using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SepEventDates
    {
        public string Id { get; set; }
        public string SpecialEventId { get; set; }
        public string LocationId { get; set; }
        public System.DateTimeOffset? EventStart { get; set; }
        public System.DateTimeOffset? EventEnd { get; set; }
        public System.DateTimeOffset? ServiceStart { get; set; }
        public System.DateTimeOffset? ServiceEnd { get; set; }
        public int? Statuscode { get; set; }
        public int? Statecode { get; set; }
    }
}
