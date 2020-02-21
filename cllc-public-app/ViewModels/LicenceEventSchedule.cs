using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class LicenceEventSchedule
    {
        // string form of the guid.
        public string Id { get; set; }
        public string EventId { get; set; }
        public DateTimeOffset? EventStartDateTime { get; set; }
        public DateTimeOffset? EventEndDateTime { get; set; }
        public DateTimeOffset? ServiceStartDateTime { get; set; }
        public DateTimeOffset? ServiceEndDateTime { get; set; }
    }
}
