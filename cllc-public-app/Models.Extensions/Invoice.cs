using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
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
            return result;
        }

        public static void CopyValues(this MicrosoftDynamicsCRMinvoice to, ViewModels.Invoice from)
        {
            to.Name = from.name;
            to.Invoicenumber = from.invoicenumber;
            to.Totaltax = from.totaltax;
            to.Totalamount = from.totalamount;
        }


    }
}
