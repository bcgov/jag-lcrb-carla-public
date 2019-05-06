using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SpdSync.models
{
    public class ApplicationScreeningRequest
    {
        public enum Spice_ApplicantType
        {
            Cannabis = 525840001,
            ESS = 525840000
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public Spice_ApplicantType ApplicantType { get; set; }
        public bool UrgentPriority { get; set; }
        public string Name { get; set; }
        public string RecordIdentifier { get; set; }
        public string ApplicantName { get; set; }
        public string BCeIDNumber { get; set; }
        public Account ApplicantAccount { get; set; }
        public Address BusinessAddress { get; set; }
        public Establishment Establishment { get; set; }
        public Contact ContactPerson { get; set; }
        public Contact ApplyingPerson { get; set; }
        public DateTimeOffset DateSent { get; set; }
        public List<LegalEntity> Associates { get; set; }
    }
}
