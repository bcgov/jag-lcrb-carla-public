using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpdSync.models
{
    public class WorkerScreeningRequest
    {
        public string RecordIdentifier { get; set; }
        public string Name { get; set; }

        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string SecondName { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public string SelfDisclosure { get; set; }

        public string Gender { get; set; }
        public string Birthplace { get; set; }
        public string BCIdCardNumber { get; set; }
        public string DriversLicence { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        
        public Address Address { get; set; }
        public List<Alias> Aliases { get; set; }

        public List<Address> PreviousAddresses { get; set; }

    }
}
