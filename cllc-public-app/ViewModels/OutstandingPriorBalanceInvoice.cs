using System;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class OutstandingParioBalanceInvoice
    {
        public OutstandingParioBalanceInvoice() {
            invoice = new Invoice();
        }
        public string applicationId { get; set; }
        public string licenceNumber { get; set; }
        public Invoice invoice { get; set; }

        public Boolean overdue { get; set; }
    }
}
