extern alias DV;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using DvInvoice = DV::Gov.Lclb.Cllb.Interfaces.Invoice;

namespace Gov.Lclb.Cllb.Public.Models
{
    public enum Adoxio_invoicestates
    {
        New = 0,
        Paid = 2,
        Cancelled = 3
    }

    public enum Adoxio_invoicestatuses
    {
        New = 1,
        Paid = 100001,
        Cancelled = 100003
    }

    public enum Adoxio_paymentmethods
    {
        CC = 3
    }

    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class InvoiceExtensions
    {
        public static ViewModels.Invoice ToViewModel(this DvInvoice dvInvoice)
        {
            var result = new ViewModels.Invoice();
            if (dvInvoice.InvoiceId != null)
                result.id = dvInvoice.InvoiceId.ToString();
            result.name = dvInvoice.Name;
            result.invoicenumber = dvInvoice.InvoiceNumber;
            result.statecode = (int?)dvInvoice.StateCode;
            result.statuscode = (int?)dvInvoice.StatusCode;
            if (dvInvoice.TotalTax != null)
                result.totaltax = dvInvoice.TotalTax.Value;
            if (dvInvoice.TotalAmount != null)
                result.totalamount = dvInvoice.TotalAmount.Value;
            result.transactionId = dvInvoice.adoxio_TransactionID;
            result.returnedTransactionId = dvInvoice.adoxio_returnedtransactionid;
            result.description = dvInvoice.Description;
            if (dvInvoice.DueDate.HasValue)
                result.duedate = DateTime.SpecifyKind(dvInvoice.DueDate.Value, DateTimeKind.Local);
            return result;
        }

    }
}
