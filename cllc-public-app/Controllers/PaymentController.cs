using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;


namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private static Random random = new Random();

        private readonly IBCEPService _bcep;

        private readonly IGeocoderService _geocoderClient;

        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly Serilog.ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public PaymentController(IConfiguration configuration,
                                 IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory,
                                 IDynamicsClient dynamicsClient, IBCEPService bcep, IGeocoderService geocoderClient)
        {
            _configuration = configuration;
            _bcep = bcep;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _geocoderClient = geocoderClient;
            _logger = Log.Logger;
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
            _logger.Debug("Called GetPaymentUrl(" + id + ")");

            // get the application and confirm access (call parse to ensure we are getting a valid id)
            Guid applicationId = Guid.Parse(id);
            MicrosoftDynamicsCRMadoxioApplication application = await GetDynamicsApplication(id);
            if (application == null)
            {
                return NotFound();
            }
            if (application.AdoxioInvoice?.Statuscode == (int?)Adoxio_invoicestatuses.Paid)
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
            catch (HttpOperationException httpOperationException)
            {
                _logger.Error(httpOperationException, "Error updating application");
                // fail 
                throw (httpOperationException);
            }
            application = await GetDynamicsApplication(id);

            // now load the invoice for this application to get the pricing
            string invoiceId = application._adoxioInvoiceValue;
            int retries = 0;
            while (retries < 10 && (invoiceId == null || invoiceId.Length == 0))
            {
                // should happen immediately, but ...
                // pause and try again - in case Dynamics is slow ...
                retries++;
                _logger.Error($"No application {id} invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(1000);
                application = await GetDynamicsApplication(id);
                invoiceId = application._adoxioInvoiceValue;
            }

            if (!string.IsNullOrEmpty(invoiceId))
            {


                _logger.Debug("Created invoice for application = " + invoiceId);

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

                MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(invoiceId);
                // dynamics creates a unique transaction id per invoice, used as the "order number" for payment
                var ordernum = invoice.AdoxioTransactionid;
                // dynamics determines the amount based on the licence type of the application
                var orderamt = invoice.Totalamount;

                Dictionary<string, string> redirectUrl;
                redirectUrl = new Dictionary<string, string>();

                bool isAlternateAccount = application.IsLiquor(_dynamicsClient); // set to true for Liquor.

                redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, id, String.Format("{0:0.00}", orderamt), isAlternateAccount);

                _logger.Debug(">>>>>" + redirectUrl["url"]);

                return new JsonResult(redirectUrl);
            }
            else
            {
                _logger.Error("GetPaymentUrl failed - Unable to get invoice for application {id}");
                return NotFound();
            }
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
            _logger.Debug("Called GetLicencePaymentUrl(" + id + ")");

            // get the application and confirm access (call parse to ensure we are getting a valid id)
            Guid applicationId = Guid.Parse(id);
            MicrosoftDynamicsCRMadoxioApplication application = await GetDynamicsApplication(id);

            if (application == null)
            {
                return NotFound();
            }

            if (application.AdoxioLicenceFeeInvoice?.Statuscode == (int?)Adoxio_invoicestatuses.Paid)
            {
                if (application.AdoxioLicencefeeinvoicepaid == false)
                {
                    try
                    {
                        MicrosoftDynamicsCRMadoxioApplication fixApplication = new MicrosoftDynamicsCRMadoxioApplication()
                        {
                            AdoxioLicencefeeinvoicepaid = true
                        };
                        _dynamicsClient.Applications.Update(id, fixApplication);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating application");
                        // fail 
                        throw (httpOperationException);
                    }
                }
                return NotFound("Payment already made");
            }

            if (!string.IsNullOrEmpty(application._adoxioLicencefeeinvoiceValue))
            {

                MicrosoftDynamicsCRMinvoice invoice2 = await _dynamicsClient.GetInvoiceById(Guid.Parse(application._adoxioLicencefeeinvoiceValue));
                if (invoice2 != null && invoice2.Statecode == (int)Adoxio_invoicestates.Cancelled)
                {
                    // set the application invoice trigger to create an invoice                    
                    MicrosoftDynamicsCRMadoxioApplication adoxioApplication2 = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        // this is the money - setting this flag to "Y" triggers a dynamics workflow that creates an invoice
                        AdoxioLicencefeeinvoicetrigger = (int?)ViewModels.GeneralYesNo.Yes
                    };

                    try
                    {
                        _dynamicsClient.Applications.Update(id, adoxioApplication2);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating application");
                        // fail 
                        throw (httpOperationException);
                    }
                    application = await GetDynamicsApplication(id);
                }
            }
            string invoiceId = application._adoxioLicencefeeinvoiceValue;



            int retries = 0;
            while (retries < 10 && string.IsNullOrEmpty(invoiceId))
            {
                // should happen immediately, but ...
                // pause and try again - in case Dynamics is slow ...
                retries++;
                _logger.Debug("No invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(1000);
                application = await GetDynamicsApplication(id);
                invoiceId = application._adoxioInvoiceValue;
            }

            if (!string.IsNullOrEmpty(invoiceId))
            {
                _logger.Debug("Created invoice for application = " + invoiceId);

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

                bool isAlternateAccount = application.IsLiquor(_dynamicsClient); // set to true for Liquor.

                Dictionary<string, string> redirectUrl;
                redirectUrl = new Dictionary<string, string>();

                var redirectPath = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/licence-fee-payment-confirmation";
                redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, id, String.Format("{0:0.00}", orderamt), isAlternateAccount, redirectPath);

                _logger.Debug(">>>>>" + redirectUrl["url"]);

                return new JsonResult(redirectUrl);
            }
            else
            {
                _logger.Error($"No invoice found for application {id}");
                return NotFound();
            }
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
            MicrosoftDynamicsCRMadoxioApplication application = await GetDynamicsApplication(id);
            if (application == null)
            {
                return NotFound();
            }

            // load the invoice for this application
            string invoiceId = application._adoxioInvoiceValue;
            Guid invoiceGuid = Guid.Parse(invoiceId);
            _logger.Debug("Found invoice for application = " + invoiceId);
            MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(invoiceGuid);
            var ordernum = invoice.AdoxioTransactionid;
            var orderamt = invoice.Totalamount;

            bool isAlternateAccount = application.IsLiquor(_dynamicsClient); // determine if it is for liquor

            var response = await _bcep.ProcessPaymentResponse(ordernum, id, isAlternateAccount);

            if (response.ContainsKey ("error"))
            {
                // handle error.
                _logger.Error($"PAYMENT VERIFICATION ERROR - {response["message"]} for application {id}");
                return StatusCode(500,response); // client will retry.
            }

            response["invoice"] = invoice.Invoicenumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            /* 
			 * - if the invoice status is not "New", skip
             * - we will update the Invoice status to "Complete" (if paid) or "Cancelled" (if payment was rejected)
             * - if payment is successful, we will also set the Application "Payment Received" to "Y" and "Method" to "Credit Card"
             */

            if (invoice.Statecode == (int?)Adoxio_invoicestates.New || invoice.Statecode == null)
            {
                _logger.Debug("Processing invoice with status New");

                // if payment was successful:
                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");

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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating invoice");
                        // fail 
                        throw (httpOperationException);
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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating application");
                        // fail 
                        throw (httpOperationException);
                    }

                    _logger.Information($"Payment approved.  Application ID: {id} Invoice: {invoice.Invoicenumber} Liquor: {isAlternateAccount}");

                }
                // if payment failed:
                else
                {
                    _logger.Debug("Transaction NOT approved");

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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating invoice");
                        // fail 
                        throw (httpOperationException);
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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating application");
                        // fail 
                        throw (httpOperationException);
                    }

                    _logger.Information($"Payment not approved.  Application ID: {id} Invoice: {invoice.Invoicenumber} Liquor: {isAlternateAccount}");

                }
            }
            else
            {
                // that can happen if we are re-validating a completed invoice (paid or cancelled)
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            return new JsonResult(response);
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
            MicrosoftDynamicsCRMadoxioApplication application = await GetDynamicsApplication(id);
            if (application == null)
            {
                return NotFound();
            }

            // load the invoice for this application
            string invoiceId = application._adoxioLicencefeeinvoiceValue;
            _logger.Debug("Found invoice for application = " + invoiceId);
            MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(Guid.Parse(invoiceId));
            var ordernum = invoice.AdoxioTransactionid;
            var orderamt = invoice.Totalamount;

            bool isAlternateAccount = application.IsLiquor(_dynamicsClient); // set to true for Liquor.

            var response = await _bcep.ProcessPaymentResponse(ordernum, id, isAlternateAccount);

            if (response.ContainsKey("error"))
            {
                // handle error.
                _logger.Error($"PAYMENT VERIFICATION ERROR - {response["message"]} for application {id}");
                return StatusCode(503); // client will retry.
            }

            response["invoice"] = invoice.Invoicenumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            /* 
			 * - if the invoice status is not "New", skip
             * - we will update the Invoice status to "Complete" (if paid) or "Cancelled" (if payment was rejected)
             * - if payment is successful, we will also set the Application "Payment Received" to "Y" and "Method" to "Credit Card"
             */

            if (invoice.Statecode == (int?)Adoxio_invoicestates.New || invoice.Statecode == null)
            {
                _logger.Debug("Processing invoice with status New");


                // if payment was successful:
                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");

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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating invoice");
                        // fail 
                        throw (httpOperationException);
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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating application");
                        // fail 
                        throw (httpOperationException);
                    }

                    // trigger geocoding
                    if (!string.IsNullOrEmpty(_configuration["FEATURE_MAPS"]))
                    {
                        await _geocoderClient.GeocodeEstablishment(application._adoxioLicenceestablishmentValue, _logger);
                    }

                    _logger.Information($"Licence Fee Transaction approved.  Application ID: {id} Invoice: {invoice.Invoicenumber} Liquor: {isAlternateAccount}");

                }
                // if payment failed:
                else
                {
                    _logger.Debug("Transaction NOT approved");

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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating invoice");

                        // fail 
                        throw (httpOperationException);
                    }


                    // set the Application invoice status back to No
                    MicrosoftDynamicsCRMadoxioApplication patchApplication = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioLicencefeeinvoicetrigger = (int?)ViewModels.GeneralYesNo.No
                    };
                    try
                    {
                        _dynamicsClient.Applications.Update(id, patchApplication);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating application");
                        // fail 
                        throw (httpOperationException);
                    }
                    _logger.Information($"Licence Fee Transaction NOT approved.  Application ID: {id} Invoice: {invoice.Invoicenumber} Liquor: {isAlternateAccount}");
                }



            }
            else
            {
                // that can happen if we are re-validating a completed invoice (paid or cancelled)
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            return new JsonResult(response);
        }

        private async Task<MicrosoftDynamicsCRMadoxioApplication> GetDynamicsApplication(string id)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            _logger.Debug("Application id = " + id);
            _logger.Debug("User id = " + userSettings.AccountId);
            var expand = new List<string> { "adoxio_LicenceFeeInvoice", "adoxio_Invoice", "adoxio_Establishment" };

            MicrosoftDynamicsCRMadoxioApplication dynamicsApplication = await _dynamicsClient.GetApplicationByIdWithChildren(Guid.Parse(id));

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

            _logger.Debug("Worker id = " + id);
            _logger.Debug("User Contact id = " + userSettings.ContactId);

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
            _logger.Debug($"Called GetWorkerPaymentUrl({workerId})");

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
            catch (HttpOperationException httpOperationException)
            {
                _logger.Error(httpOperationException, "Error updating worker");
                // fail 
                throw (httpOperationException);
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
                _logger.Debug("No invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(1000);
                patchWorker = await GetDynamicsWorker(workerId, false);
                invoiceId = patchWorker._adoxioInvoiceValue;
            }
            _logger.Debug("Created invoice for worker = " + invoiceId);

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

            bool isAlternateAccount = false;  // always false for a worker.

            Dictionary<string, string> redirectUrl;
            redirectUrl = new Dictionary<string, string>();
            var redirectPath = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/worker-qualification/payment-confirmation";
            redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, workerId, String.Format("{0:0.00}", orderamt), isAlternateAccount, redirectPath);

            _logger.Debug(">>>>>" + redirectUrl["url"]);

            return new JsonResult(redirectUrl);
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
            _logger.Debug("Found invoice for application = " + invoiceId);
            MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(Guid.Parse(invoiceId));
            var ordernum = invoice.AdoxioTransactionid;
            var orderamt = invoice.Totalamount;

            bool isAlternateAccount = false;  // in this case it is always false, as the worker process uses the Cannabis account.

            var response = await _bcep.ProcessPaymentResponse(ordernum, workerId, isAlternateAccount);

            if (response.ContainsKey("error"))
            {
                // handle error.
                _logger.Error($"PAYMENT VERIFICATION ERROR - {response["message"]} for worker {workerId}");
                return StatusCode(503); // client will retry.
            }

            response["invoice"] = invoice.Invoicenumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            /* 
			 * - if the invoice status is not "New", skip
             * - we will update the Invoice status to "Complete" (if paid) or "Cancelled" (if payment was rejected)
             * - if payment is successful, we will also set the Application "Payment Received" to "Y" and "Method" to "Credit Card"
             */

            if (invoice.Statecode == (int?)Adoxio_invoicestates.New || invoice.Statecode == null)
            {
                _logger.Debug("Processing invoice with status New");

                // if payment was successful:
                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");

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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating invoice");
                        // fail 
                        throw (httpOperationException);
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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error("Error updating worker");
                        _logger.Error("Request:");
                        _logger.Error(httpOperationException.Request.Content);
                        _logger.Error("Response:");
                        _logger.Error(httpOperationException.Response.Content);
                        // fail 
                        throw (httpOperationException);
                    }

                }
                // if payment failed:
                else
                {
                    _logger.Debug("Transaction NOT approved");

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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating invoice");
                        // fail 
                        throw (httpOperationException);
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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error updating worker");
                        // fail 
                        throw (httpOperationException);
                    }
                }
            }
            else
            {
                // that can happen if we are re-validating a completed invoice (paid or cancelled)
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            return new JsonResult(response);
        }

    }
}
