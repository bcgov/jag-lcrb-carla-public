using System;
using System.Collections.Generic;

namespace SpdSync.models
{
    public class LegalEntity
    {
        public string EntityId { get; set; }
        public string Name { get; set; }
        public bool IsIndividual { get; set; }
        public Contact Contact { get; set; }
        public Account Account { get; set; }
        public List<Alias> Aliases { get; set; }
        public List<Address> PreviousAddresses { get; set; }
    }
}
