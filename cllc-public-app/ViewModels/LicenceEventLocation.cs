using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class LicenceEventLocation
    {
        // string form of the guid.
        public string Id { get; set; }
        public string EventId { get; set; }
        public string Name { get; set; }
        public int? Attendance { get; set; }
        public string LocationId { get; set; }
    }
}
