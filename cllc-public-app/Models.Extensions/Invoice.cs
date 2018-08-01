using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

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
        public static ViewModels.Invoice ToViewModel(this MicrosoftDynamicsCRMinvoice dynamicsInvoice)
        {
            Invoice result = new Invoice();            
            
            if (dynamicsInvoice.Invoiceid != null)
            {
                result.id = dynamicsInvoice.Invoiceid;
            }
            result.name = dynamicsInvoice.Name;
            result.invoicenumber = dynamicsInvoice.Invoicenumber;
			result.statecode = dynamicsInvoice.Statecode;
			result.statuscode = dynamicsInvoice.Statuscode;
            if (dynamicsInvoice.Totaltax != null)
            {
                result.totaltax = (double)dynamicsInvoice.Totaltax;
            }
            if (dynamicsInvoice.Totalamount != null)
            {
                result.totalamount = (double) dynamicsInvoice.Totalamount;
            }
            
            if (dynamicsInvoice.CustomeridAccount != null)
            {
                result.customer = dynamicsInvoice.CustomeridAccount.ToViewModel();
            }

			if (dynamicsInvoice.AdoxioTransactionid != null)
            {
                result.transactionId = dynamicsInvoice.AdoxioTransactionid;
            }
			if (dynamicsInvoice.AdoxioReturnedtransactionid != null)
            {
				result.returnedTransactionId = dynamicsInvoice.AdoxioReturnedtransactionid;
            }

            return result;
        }

        public static void CopyValues(this MicrosoftDynamicsCRMinvoice to, ViewModels.Invoice from)
        {
			to.Invoiceid = from.id;
			to.Name = from.name;
            to.Invoicenumber = from.invoicenumber;
			to.Statecode = from.statecode;
			to.Statuscode = from.statuscode;
            to.Totaltax = from.totaltax;
            to.Totalamount = from.totalamount;
			to.AdoxioTransactionid = from.transactionId;
			to.AdoxioReturnedtransactionid = from.returnedTransactionId;
        }

    }
}
