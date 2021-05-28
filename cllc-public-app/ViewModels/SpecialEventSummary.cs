using System;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SpecialEventSummary
    {
        public string SpecialEventId { get; set; } // server side primary key
        public DateTimeOffset? EventStartDate { get; set; }
        public string EventName { get; set; }
        public int? EventStatus { get; set; }
        public int? MaximumNumberOfGuests { get; set; }
        public DateTimeOffset? DateSubmitted { get; set; }

        public ViewModels.Account PoliceAccount { get; set; }
        public Contact PoliceDecisionBy { get; set; }
        public int? PoliceDecision { get; set; }
        public DateTimeOffset? DateOfPoliceDecision { get; set; }
    }
}
