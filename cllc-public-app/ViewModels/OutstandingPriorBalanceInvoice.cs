using System;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class OutstandingParioBalanceInvoice
    {
        public OutstandingParioBalanceInvoice() {
            invoice = new Invoice();
        }
        public string applicationId { get; set; }
        public Invoice invoice { get; set; }      
    }
}
