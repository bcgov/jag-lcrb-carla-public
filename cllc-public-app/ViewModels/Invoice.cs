﻿using System;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class Invoice
    {
        public string id { get; set; }
        public string name { get; set; }
        public string invoicenumber { get; set; }
        public decimal totaltax { get; set; }
        public decimal totalamount { get; set; }
        public Account customer { get; set; }
        public string transactionId { get; set; }
        public string returnedTransactionId { get; set; }
        public int? statecode { get; set; }
        public int? statuscode { get; set; }
        public string description { get; set; }
        public DateTime? duedate { get; set; }
    }
}
