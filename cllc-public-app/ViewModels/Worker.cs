using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class Worker
    {
        public string id { get; set; }
        public bool isldbworker { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public DateTime? dateofbirth { get; set; }
        public string gender { get; set; }
        public string birthplace { get; set; }
        public string driverslicencenumber { get; set; }
        public string bcidcardnumber { get; set; }
        public string phonenumber { get; set; }
        public string email { get; set; }
        public bool selfdisclosure { get; set; }
        public bool triggerphs { get; set; }
        public string contactId { get; set; }
        public bool paymentReceived { get; set; }
        public DateTime? paymentRecievedDate { get; set; }
        public string workerId { get; set; }
    }
}
