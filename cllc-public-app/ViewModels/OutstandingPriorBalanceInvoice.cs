using System;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class OutstandingParioBalanceInvoice
    {
        public OutstandingParioBalanceInvoice()
        {
            invoice = new Invoice();
        }
        public string applicationId { get; set; }
        public string licenceNumber { get; set; }
        public Invoice invoice { get; set; }

        /// <summary>
        /// Format "yyyy-MM-dd", so the browser's timezone can't shift the date.
        /// </summary>
        public string dueDate { get; set; }

        public Boolean overdue { get; set; }
    }
}
