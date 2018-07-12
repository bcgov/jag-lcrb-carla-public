using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class Invoice
    {
        public string id { get; set; }   
        public string name { get; set; }
        public string invoicenumber { get; set; }        
        public double totaltax { get; set; }
        public double totalamount { get; set; }
        public Account customer { get; set; }
		public string transactionId { get; set; }
		public string returnedTransactionId { get; set; }
		public int? statecode { get; set; }
		public int? statuscode { get; set; }
    }
}
