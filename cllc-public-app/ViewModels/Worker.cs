using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum Gender {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum StatusCode{
        [EnumMember(Value = "Application Incomplete")]
        NotSubmitted = 845280002,
        [EnumMember(Value = "Pending Review")]
        PendingReview  = 845280003,
        Active  = 1,
        Withdrawn  = 845280004,
        Rejected = 845280005,
        Revoked = 845280000,
        Expired  = 845280001,
    }

    public class Worker
    {
        public string id { get; set; }
        public bool? isldbworker { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public DateTimeOffset? dateofbirth { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Gender gender { get; set; }
        public string birthplace { get; set; }
        public string driverslicencenumber { get; set; }
        public string bcidcardnumber { get; set; }
        public string phonenumber { get; set; }
        public string email { get; set; }
        public bool? selfdisclosure { get; set; }
        public bool? triggerphs { get; set; }
        public Contact contact { get; set; }
        public bool? paymentReceived { get; set; }
        public DateTimeOffset? paymentRecievedDate { get; set; }
        public DateTimeOffset? modifiedOn { get; set; }
        public string workerId { get; set; }
        public DateTimeOffset? fromdate { get; set; }   
        
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusCode Status { get; set; }
    }
}
