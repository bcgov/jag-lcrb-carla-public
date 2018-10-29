using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private static Random random = new Random();

        private readonly BCEPWrapper _bcep;

        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentController(IConfiguration configuration,
                                 IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory,
                                 IDynamicsClient dynamicsClient, BCEPWrapper bcep)
        {
            Configuration = configuration;
            _bcep = bcep;
            _dynamicsClient = dynamicsClient;            
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(PaymentController));
        }

        /// <summary>
        /// GET a payment re-direct url for an Application
        /// This will register an (unpaid) invoice against the application and generate an invoice number,
        /// which will be used to match payments
        /// </summary>
        /// <param name="id">GUID of the Application to pay</param>
        /// <returns></returns>
        [HttpGet("submit/{id}")]
        public async Task<IActionResult> GetPaymentUrl(string id)
        {
            _logger.LogInformation("Called GetPaymentUrl(" + id + ")");

            // get the application and confirm access (call parse to ensure we are getting a valid id)
            Guid applicationId = Guid.Parse(id);
            MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await GetDynamicsApplication(id, false);
            if (adoxioApplication == null)
            {
                return NotFound();
            }
            if (adoxioApplication.AdoxioInvoice?.Statuscode == (int?)Adoxio_invoicestatuses.Paid)
            {
                return NotFound("Payment already made");
            }

            // set the application invoice trigger to create an invoice
            // no need to copy the whole record over as we are doing a Patch for a single field.
            MicrosoftDynamicsCRMadoxioApplication patchApplication = new MicrosoftDynamicsCRMadoxioApplication()
            {
                // this is the money - setting this flag to "Y" triggers a dynamics workflow that creates an invoice
                AdoxioInvoicetrigger = (int?)ViewModels.GeneralYesNo.Yes
            };

            try
            {
                _dynamicsClient.Applications.Update(id, patchApplication);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating application");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail 
                throw (odee);
            }
            patchApplication = await GetDynamicsApplication(id, false);

            // now load the invoice for this application to get the pricing
            string invoiceId = patchApplication._adoxioInvoiceValue;
            int retries = 0;
            while (retries < 10 && (invoiceId == null || invoiceId.Length == 0))
            {
                // should happen immediately, but ...
                // pause and try again - in case Dynamics is slow ...
                retries++;
                _logger.LogError("No invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(1000);
                patchApplication = await GetDynamicsApplication(id, false);
                invoiceId = patchApplication._adoxioInvoiceValue;
            }
            _logger.LogInformation("Created invoice for application = " + invoiceId);

            /*
             * When the applicant submits their Application, we will set the application "Application Invoice Trigger" to "Y" - this will trigger a workflow that will create the Invoice
             *  - we will then re-query the Application to get the Invoice number,
             *  - and then query the Invoice to get the amount
             *  - the Invoice will also contain a Transaction Id (starting at 0500000000)
             *  - the Invoice status will be New
             * Notes:
             *  - If there is already an invoice with Status New, don't need to create a new Invoice
             *  - If there is already an invoice with Status Complete, it is an error (can't pay twice)
             *  - We will deal with the history later (i.e. there can be multiple "Cancelled" Invoices - we need to keep them for reconciliation but we don't need them for MVP
             */

            MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(Guid.Parse(invoiceId));
            // dynamics creates a unique transaction id per invoice, used as the "order number" for payment
            var ordernum = invoice.AdoxioTransactionid;
            // dynamics determines the amount based on the licence type of the application
            var orderamt = invoice.Totalamount;

            Dictionary<string, string> redirectUrl;
            redirectUrl = new Dictionary<string, string>();
            redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, id, String.Format("{0:0.00}", orderamt));

            _logger.LogInformation(">>>>>" + redirectUrl["url"]);

            return Json(redirectUrl);
        }

        /// <summary>
        /// GET a payment re-direct url for an Application Licence Fee
        /// This will register an (unpaid) invoice against the application licence and generate an invoice number,
        /// which will be used to match payments
        /// </summary>
        /// <param name="id">GUID of the Application to pay licence fee</param>
        /// <returns></returns>
        [HttpGet("submit/licence-fee/{id}")]
        public async Task<IActionResult> GetLicencePaymentUrl(string id)
        {
            _logger.LogInformation("Called GetLicencePaymentUrl(" + id + ")");

            // get the application and confirm access (call parse to ensure we are getting a valid id)
            Guid applicationId = Guid.Parse(id);
            MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await GetDynamicsApplication(id, true);

            if (adoxioApplication == null)
            {
                return NotFound();
            }

            if (adoxioApplication.AdoxioLicenceFeeInvoice?.Statuscode == (int?)Adoxio_invoicestatuses.Paid)
            {
                return NotFound("Payment already made");
            }

            if (!string.IsNullOrEmpty(adoxioApplication._adoxioLicencefeeinvoiceValue))
            {

                MicrosoftDynamicsCRMinvoice invoice2 = await _dynamicsClient.GetInvoiceById(Guid.Parse(adoxioApplication._adoxioLicencefeeinvoiceValue));
                if (invoice2 != null && invoice2.Statecode == (int)Adoxio_invoicestates.Cancelled)
                {
                    // set the application invoice trigger to create an invoice                    
                    MicrosoftDynamicsCRMadoxioApplication adoxioApplication2 = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        // this is the money - setting this flag to "Y" triggers a dynamics workflow that creates an invoice
                        AdoxioLicenceFeeInvoiceTrigger = (int?)ViewModels.GeneralYesNo.Yes
                    };

                    try
                    {
                        _dynamicsClient.Applications.Update(id, adoxioApplication2);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating application");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }
                    adoxioApplication = await GetDynamicsApplication(id, false);
                }
            }
            string invoiceId = adoxioApplication._adoxioLicencefeeinvoiceValue;



            int retries = 0;
            while (retries < 10 && string.IsNullOrEmpty(invoiceId))
            {
                // should happen immediately, but ...
                // pause and try again - in case Dynamics is slow ...
                retries++;
                _logger.LogError("No invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(1000);
                adoxioApplication = await GetDynamicsApplication(id, false);
                invoiceId = adoxioApplication._adoxioInvoiceValue;
            }
            _logger.LogError("Created invoice for application = " + invoiceId);

            /*
             * When the applicant submits their Application, we will set the application "Application Invoice Trigger" to "Y" - this will trigger a workflow that will create the Invoice
             *  - we will then re-query the Application to get the Invoice number,
             *  - and then query the Invoice to get the amount
             *  - the Invoice will also contain a Transaction Id (starting at 0500000000)
             *  - the Invoice status will be New
             * Notes:
             *  - If there is already an invoice with Status New, don't need to create a new Invoice
             *  - If there is already an invoice with Status Complete, it is an error (can't pay twice)
             *  - We will deal with the history later (i.e. there can be multiple "Cancelled" Invoices - we need to keep them for reconciliation but we don't need them for MVP
             */

            MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(Guid.Parse(invoiceId));
            // dynamics creates a unique transaction id per invoice, used as the "order number" for payment
            var ordernum = invoice.AdoxioTransactionid;
            // dynamics determines the amount based on the licence type of the application
            var orderamt = invoice.Totalamount;

            Dictionary<string, string> redirectUrl;
            redirectUrl = new Dictionary<string, string>();

            var redirectPath = Configuration["BASE_URI"] + Configuration["BASE_PATH"] + "/licence-fee-payment-confirmation";
            redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, id, String.Format("{0:0.00}", orderamt), redirectPath);

            _logger.LogError(">>>>>" + redirectUrl["url"]);

            return Json(redirectUrl);
        }

        /// <summary>
        /// Update a payment response from Bamboora (payment success or failed)
        /// This can be called if no response is received from Bamboora - it will query the server directly
        /// based on the Application's Invoice number
        /// This will also update the invoice payment status, and, if the payment is successful,
        /// it will push the Application into Submitted status
        /// </summary>
        /// <param name="id">GUID of the Application to pay</param>
        /// <returns></returns>
        [HttpGet("verify/{id}")]
        public async Task<IActionResult> VerifyPaymentStatus(string id)
        {
            MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await GetDynamicsApplication(id, false);
            if (adoxioApplication == null)
            {
                return NotFound();
            }

            // load the invoice for this application
            string invoiceId = adoxioApplication._adoxioInvoiceValue;
            Guid invoiceGuid = Guid.Parse(invoiceId);
            _logger.LogInformation("Found invoice for application = " + invoiceId);
            MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(invoiceGuid);
            var ordernum = invoice.AdoxioTransactionid;
            var orderamt = invoice.Totalamount;

            var response = await _bcep.ProcessPaymentResponse(ordernum, id);
            response["invoice"] = invoice.Invoicenumber;

            foreach (var key in response.Keys)
            {
                _logger.LogInformation(">>>>>" + key + ":" + response[key]);
            }

            /* 
			 * - if the invoice status is not "New", skip
             * - we will update the Invoice status to "Complete" (if paid) or "Cancelled" (if payment was rejected)
             * - if payment is successful, we will also set the Application "Payment Received" to "Y" and "Method" to "Credit Card"
             */

            if (invoice.Statecode == (int?)Adoxio_invoicestates.New || invoice.Statecode == null)
            {
                _logger.LogInformation("Processing invoice with status New");

                // if payment was successful:
                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.LogError("Transaction approved");

                    MicrosoftDynamicsCRMinvoice invoice2 = new MicrosoftDynamicsCRMinvoice()
                    {
                        Statecode = (int?)Adoxio_invoicestates.Paid,
                        Statuscode = (int?)Adoxio_invoicestatuses.Paid,
                        AdoxioReturnedtransactionid = response["trnId"]
                    };

                    // set invoice status to Complete
                    try
                    {
                        _dynamicsClient.Invoices.Update(invoice.Invoiceid, invoice2);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating invoice");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }

                    MicrosoftDynamicsCRMadoxioApplication adoxioApplication2 = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        // set the Application payment status
                        AdoxioPaymentrecieved = (bool?)true,
                        AdoxioPaymentmethod = (int?)Adoxio_paymentmethods.CC,
                        AdoxioAppchecklistpaymentreceived = (int?)ViewModels.GeneralYesNo.Yes
                    };
                    try
                    {
                        _dynamicsClient.Applications.Update(id, adoxioApplication2);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating application");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }
                }
                // if payment failed:
                else
                {
                    _logger.LogError("Transaction NOT approved");

                    // set invoice status to Cancelled
                    MicrosoftDynamicsCRMinvoice invoice2 = new MicrosoftDynamicsCRMinvoice()
                    {
                        Statecode = (int?)Adoxio_invoicestates.Cancelled,
                        Statuscode = (int?)Adoxio_invoicestatuses.Cancelled
                    };
                    try
                    {
                        _dynamicsClient.Invoices.Update(invoice.Invoiceid, invoice2);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating invoice");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }
                    // set the Application invoice status back to No
                    MicrosoftDynamicsCRMadoxioApplication adoxioApplication2 = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioInvoicetrigger = (int?)ViewModels.GeneralYesNo.No
                    };
                    try
                    {
                        _dynamicsClient.Applications.Update(id, adoxioApplication2);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating application");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }

                }
            }
            else
            {
                // that can happen if we are re-validating a completed invoice (paid or cancelled)
                _logger.LogError("Invoice status is not New, skipping updates ...");
            }

            return Json(response);
        }
        /// <summary>
        /// Update a payment response from Bambora (payment success or failed)
        /// This can be called if no response is received from Bambora - it will query the server directly
        /// based on the Application's Invoice number
        /// This will also update the invoice payment status, and, if the payment is successful,
        /// it will push the Application into Submitted status
        /// </summary>
        /// <param name="id">GUID of the Application to pay</param>
        /// <returns></returns>
        [HttpGet("verify/licence-fee/{id}")]
        public async Task<IActionResult> VerifyLicenceFeePaymentStatus(string id)
        {
            MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await GetDynamicsApplication(id, false);
            if (adoxioApplication == null)
            {
                return NotFound();
            }

            // load the invoice for this application
            string invoiceId = adoxioApplication._adoxioLicencefeeinvoiceValue;
            _logger.LogError("Found invoice for application = " + invoiceId);
            MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(Guid.Parse(invoiceId));
            var ordernum = invoice.AdoxioTransactionid;
            var orderamt = invoice.Totalamount;

            var response = await _bcep.ProcessPaymentResponse(ordernum, id);
            response["invoice"] = invoice.Invoicenumber;

            foreach (var key in response.Keys)
            {
                _logger.LogInformation(">>>>>" + key + ":" + response[key]);
            }

            /* 
			 * - if the invoice status is not "New", skip
             * - we will update the Invoice status to "Complete" (if paid) or "Cancelled" (if payment was rejected)
             * - if payment is successful, we will also set the Application "Payment Received" to "Y" and "Method" to "Credit Card"
             */

            if (invoice.Statecode == (int?)Adoxio_invoicestates.New || invoice.Statecode == null)
            {
                _logger.LogInformation("Processing invoice with status New");


                // if payment was successful:
                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.LogError("Transaction approved");

                    MicrosoftDynamicsCRMinvoice patchInvoice = new MicrosoftDynamicsCRMinvoice()
                    {
                        // set invoice status to Complete
                        Statecode = (int?)Adoxio_invoicestates.Paid,
                        Statuscode = (int?)Adoxio_invoicestatuses.Paid,
                        AdoxioReturnedtransactionid = response["trnId"]
                    };
                    try
                    {
                        _dynamicsClient.Invoices.Update(invoice.Invoiceid, patchInvoice);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating invoice");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }

                    // set the Application payment status
                    MicrosoftDynamicsCRMadoxioApplication patchApplication = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioLicencefeeinvoicepaid = true
                    };
                    try
                    {
                        _dynamicsClient.Applications.Update(id, patchApplication);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating application");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }


                }
                // if payment failed:
                else
                {
                    _logger.LogError("Transaction NOT approved");

                    MicrosoftDynamicsCRMinvoice patchInvoice = new MicrosoftDynamicsCRMinvoice()
                    {
                        // set invoice status to Cancelled
                        Statecode = (int?)Adoxio_invoicestates.Cancelled,
                        Statuscode = (int?)Adoxio_invoicestatuses.Cancelled
                    };
                    try
                    {
                        _dynamicsClient.Invoices.Update(invoice.Invoiceid, patchInvoice);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating invoice");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }


                    // set the Application invoice status back to No
                    MicrosoftDynamicsCRMadoxioApplication patchApplication = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioLicenceFeeInvoiceTrigger = (int?)ViewModels.GeneralYesNo.No
                    };
                    try
                    {
                        _dynamicsClient.Applications.Update(id, patchApplication);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating application");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }
                }
            }
            else
            {
                // that can happen if we are re-validating a completed invoice (paid or cancelled)
                _logger.LogError("Invoice status is not New, skipping updates ...");
            }

            return Json(response);
        }

        private async Task<MicrosoftDynamicsCRMadoxioApplication> GetDynamicsApplication(string id, bool getInvoice)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            _logger.LogError("Application id = " + id);
            _logger.LogError("User id = " + userSettings.AccountId);
            var expand = new List<string> { "adoxio_LicenceFeeInvoice", "adoxio_Invoice" };
            MicrosoftDynamicsCRMadoxioApplication dynamicsApplication = null;
            if (getInvoice)
            {
                dynamicsApplication = await _dynamicsClient.Applications.GetByKeyAsync(Guid.Parse(id).ToString(), expand: expand);
            }
            else
            {
                dynamicsApplication = await _dynamicsClient.Applications.GetByKeyAsync(Guid.Parse(id).ToString());
            }

            if (dynamicsApplication == null)
            {
                return null;
            }
            else
            {
                if (!CurrentUserHasAccessToApplicationOwnedBy(dynamicsApplication._adoxioApplicantValue))
                {
                    return null;
                }
            }

            return dynamicsApplication;
        }
        private async Task<MicrosoftDynamicsCRMadoxioWorker> GetDynamicsWorker(string id, bool getInvoice)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            _logger.LogError("Worker id = " + id);
            _logger.LogError("User Contact id = " + userSettings.ContactId);

            MicrosoftDynamicsCRMadoxioWorker worker = null;
            if (getInvoice)
            {
                List<string> expand = new List<string> { "adoxio_Invoice" };
                worker = await _dynamicsClient.Workers.GetByKeyAsync(Guid.Parse(id).ToString(), expand: expand);
            }
            else
            {
                worker = await _dynamicsClient.Workers.GetByKeyAsync(Guid.Parse(id).ToString());
            }
            if (worker == null)
            {
                return null;
            }
            else
            {
                if (worker._adoxioContactidValue != userSettings.ContactId)
                {
                    return null;
                }
            }

            return worker;
        }

        /// <summary>
        /// Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
        private bool CurrentUserHasAccessToApplicationOwnedBy(string accountId)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // For now, check if the account id matches the user's account.
            // TODO there may be some account relationships in the future
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return userSettings.AccountId == accountId;
            }

            // if current user doesn't have an account they are probably not logged in
            return false;
        }

        // specific for unit testing and development
        [HttpGet("verify/{id}/APPROVE")]
        public async Task<IActionResult> VerifyPaymentStatusAPPROVE(string id)
        {
            if (TestUtility.InUnitTestMode())
            {
                _bcep.setHashKeyForUnitTesting("APPROVE");
                return await VerifyPaymentStatus(id);
            }
            return NotFound();
        }

        // specific for unit testing and development
        [HttpGet("verify/{id}/DECLINE")]
        public async Task<IActionResult> VerifyPaymentStatusDECLINE(string id)
        {
            if (TestUtility.InUnitTestMode())
            {
                _bcep.setHashKeyForUnitTesting("DECLINE");
                return await VerifyPaymentStatus(id);
            }
            return NotFound();
        }

        /// <summary>
        /// GET a payment re-direct url for an Application
        /// This will register an (unpaid) invoice against the application and generate an invoice number,
        /// which will be used to match payments
        /// </summary>
        /// <param name="workerId">GUID of the Application to pay</param>
        /// <returns></returns>
        [HttpGet("submit/worker/{workerId}")]
        public async Task<IActionResult> GetWorkerPaymentUrl(string workerId)
        {
            _logger.LogInformation($"Called GetWorkerPaymentUrl({workerId})");

            // get the application and confirm access (call parse to ensure we are getting a valid id)
            Guid workerGuid = Guid.Parse(workerId);
            MicrosoftDynamicsCRMadoxioWorker worker = await GetDynamicsWorker(workerId, true);
            if (worker == null)
            {
                return NotFound();
            }
            if (worker.AdoxioInvoice?.Statuscode == (int?)Adoxio_invoicestatuses.Paid)
            {
                return NotFound("Payment already made");
            }
            // set the application invoice trigger to create an invoice            
            MicrosoftDynamicsCRMadoxioWorker patchWorker = new MicrosoftDynamicsCRMadoxioWorker()
            {
                // this is the money - setting this flag to "Y" triggers a dynamics workflow that creates an invoice
                AdoxioInvoicetrigger = (int?)ViewModels.GeneralYesNo.Yes
            };

            try
            {
                _dynamicsClient.Workers.Update(workerId, patchWorker);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating worker");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail 
                throw (odee);
            }
            // we set the getInvoice parameter to false here as there is a chance the Invoice is not created yet - so we may not be able to expand.
            patchWorker = await GetDynamicsWorker(workerId, false);

            // now load the invoice for this application to get the pricing
            string invoiceId = patchWorker._adoxioInvoiceValue;
            int retries = 0;
            while (retries < 10 && (invoiceId == null || invoiceId.Length == 0))
            {
                // should happen immediately, but ...
                // pause and try again - in case Dynamics is slow ...
                retries++;
                _logger.LogError("No invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(1000);
                patchWorker = await GetDynamicsWorker(workerId, false);
                invoiceId = patchWorker._adoxioInvoiceValue;
            }
            _logger.LogError("Created invoice for worker = " + invoiceId);

            /*
             * When the applicant submits their Application, we will set the application "Application Invoice Trigger" to "Y" - this will trigger a workflow that will create the Invoice
             *  - we will then re-query the Application to get the Invoice number,
             *  - and then query the Invoice to get the amount
             *  - the Invoice will also contain a Transaction Id (starting at 0500000000)
             *  - the Invoice status will be New
             * Notes:
             *  - If there is already an invoice with Status New, don't need to create a new Invoice
             *  - If there is already an invoice with Status Complete, it is an error (can't pay twice)
             *  - We will deal with the history later (i.e. there can be multiple "Cancelled" Invoices - we need to keep them for reconciliation but we don't need them for MVP
             */

            MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(Guid.Parse(invoiceId));
            // dynamics creates a unique transaction id per invoice, used as the "order number" for payment
            var ordernum = invoice.AdoxioTransactionid;
            // dynamics determines the amount based on the licence type of the application
            var orderamt = invoice.Totalamount;

            Dictionary<string, string> redirectUrl;
            redirectUrl = new Dictionary<string, string>();
            var redirectPath = Configuration["BASE_URI"] + Configuration["BASE_PATH"] + "/worker-qualification/payment-confirmation";
            redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, workerId, String.Format("{0:0.00}", orderamt), redirectPath);

            _logger.LogInformation(">>>>>" + redirectUrl["url"]);

            return Json(redirectUrl);
        }


        /// <summary>
        /// Update a payment response from Bamboora (payment success or failed)
        /// This can be called if no response is received from Bamboora - it will query the server directly
        /// based on the Application's Invoice number
        /// This will also update the invoice payment status, and, if the payment is successful,
        /// it will push the Application into Submitted status
        /// </summary>
        /// <param name="workerId">GUID of the Application to pay</param>
        /// <returns></returns>
        [HttpGet("verify/worker/{workerId}")]
        public async Task<IActionResult> VerifyWorkerPaymentStatus(string workerId)
        {
            MicrosoftDynamicsCRMadoxioWorker worker = await GetDynamicsWorker(workerId, true);
            if (worker == null)
            {
                return NotFound();
            }

            // load the invoice for this application
            string invoiceId = worker._adoxioInvoiceValue;
            _logger.LogError("Found invoice for application = " + invoiceId);
            MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(Guid.Parse(invoiceId));
            var ordernum = invoice.AdoxioTransactionid;
            var orderamt = invoice.Totalamount;

            var response = await _bcep.ProcessPaymentResponse(ordernum, workerId);
            response["invoice"] = invoice.Invoicenumber;

            foreach (var key in response.Keys)
            {
                _logger.LogInformation(">>>>>" + key + ":" + response[key]);
            }

            /* 
			 * - if the invoice status is not "New", skip
             * - we will update the Invoice status to "Complete" (if paid) or "Cancelled" (if payment was rejected)
             * - if payment is successful, we will also set the Application "Payment Received" to "Y" and "Method" to "Credit Card"
             */

            if (invoice.Statecode == (int?)Adoxio_invoicestates.New || invoice.Statecode == null)
            {
                _logger.LogInformation("Processing invoice with status New");

                // if payment was successful:
                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.LogError("Transaction approved");

                    MicrosoftDynamicsCRMinvoice invoice2 = new MicrosoftDynamicsCRMinvoice()
                    {
                        // set invoice status to Complete
                        Statecode = (int?)Adoxio_invoicestates.Paid,
                        Statuscode = (int?)Adoxio_invoicestatuses.Paid,
                        AdoxioReturnedtransactionid = response["trnId"]
                    };
                    try
                    {
                        _dynamicsClient.Invoices.Update(invoice.Invoiceid, invoice2);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating invoice");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }
                    MicrosoftDynamicsCRMadoxioWorker patchWorker = new MicrosoftDynamicsCRMadoxioWorker()
                    {
                        // set the Application payment status
                        AdoxioPaymentreceived = 1,
                        AdoxioPaymentreceiveddate = DateTime.UtcNow
                    };
                    try
                    {
                        _dynamicsClient.Workers.Update(workerId, patchWorker);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating worker");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }

                }
                // if payment failed:
                else
                {
                    _logger.LogError("Transaction NOT approved");

                    // set invoice status to Cancelled
                    MicrosoftDynamicsCRMinvoice patchInvoice = new MicrosoftDynamicsCRMinvoice()
                    {
                        Statecode = (int?)Adoxio_invoicestates.Cancelled,
                        Statuscode = (int?)Adoxio_invoicestatuses.Cancelled
                    };
                    try
                    {
                        _dynamicsClient.Invoices.Update(invoice.Invoiceid, patchInvoice);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating invoice");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }


                    // set the Application invoice status back to No
                    MicrosoftDynamicsCRMadoxioWorker patchWorker = new MicrosoftDynamicsCRMadoxioWorker()
                    {
                        // set the Application payment status
                        AdoxioInvoicetrigger = (int?)ViewModels.GeneralYesNo.No
                    };

                    try
                    {
                        _dynamicsClient.Workers.Update(workerId, patchWorker);
                    }
                    catch (OdataerrorException odee)
                    {
                        _logger.LogError("Error updating worker");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        // fail 
                        throw (odee);
                    }
                }
            }
            else
            {
                // that can happen if we are re-validating a completed invoice (paid or cancelled)
                _logger.LogError("Invoice status is not New, skipping updates ...");
            }

            return Json(response);
        }

    }
}
