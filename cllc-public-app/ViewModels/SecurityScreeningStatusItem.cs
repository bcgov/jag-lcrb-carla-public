using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Models;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class SecurityScreeningStatusItem
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        // 2021-03-31 ASR: changed the birthdate field to DateTime (from DateTimeOffset) to ignore the timezone on the birthdate field in Dynamics.
        // Accordingly the security screening Angular page has been modified to display this date in GMT to match the value set here.
        public DateTime? Birthdate { get; set; }
        public DateTimeOffset? DateSubmitted { get; set; }
        public string PhsLink { get; set; }
        public string CasLink { get; set; }

        [JsonIgnore] //Exclude from json serialization
        public string ContactId { get; set; }
        [JsonIgnore] //Exclude from json serialization
        public MicrosoftDynamicsCRMcontact Contact { get; set; }
        [JsonIgnore] //Exclude from json serialization
        public bool IsComplete { get; set; }
    }
}
