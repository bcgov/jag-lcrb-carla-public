using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SpdSync.models
{
    public class WorkerScreeningRequest
    {
        public enum Adoxio_GenderCode
        {
            Male = 1,
            Female = 2,
            Other = 3
        }
        public enum General_YesNo
        {
            Yes = 1,
            No = 0
        }
        public string RecordIdentifier { get; set; }
        public string Name { get; set; }

        public DateTimeOffset? BirthDate { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public General_YesNo SelfDisclosure { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Adoxio_GenderCode Gender { get; set; }
        public string Birthplace { get; set; }
        public string BCIdCardNumber { get; set; }
        public string DriversLicence { get; set; }
        public Contact Contact { get; set; }
        
        public Address Address { get; set; }
        public List<Alias> Aliases { get; set; }

        public List<Address> PreviousAddresses { get; set; }
    }
}
