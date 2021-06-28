using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
   
    public class SpecialEventSummary
    {
        public string SpecialEventId { get; set; } // server side primary key
        public DateTimeOffset? EventStartDate { get; set; }
        public string EventName { get; set; }
        public string InvoiceId { get; set; }
        public string EventType {get; set;}
        public bool? IsInvoicePaid { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EventStatus? EventStatus { get; set; }


        [JsonConverter(typeof(StringEnumConverter))]
        public ApproverStatus? PoliceApproval { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ApproverStatus? LcrbApproval { get; set; }
        public Contact? LcrbApprovalBy {get; set; }
        
        public int? MaximumNumberOfGuests { get; set; }
        public DateTimeOffset? DateSubmitted { get; set; }

        public ViewModels.Account PoliceAccount { get; set; }
        public Contact PoliceDecisionBy { get; set; }
        public int? PoliceDecision { get; set; }
        public DateTimeOffset? DateOfPoliceDecision { get; set; }

        public string? DenialReason {get; set;}
        public string? CancelReason {get; set;}
    }
}
