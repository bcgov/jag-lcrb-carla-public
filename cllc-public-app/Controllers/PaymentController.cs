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
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
	public class PaymentController : Controller
	{
		private static Random random = new Random();

		private readonly IConfiguration Configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ILogger _logger;
		private readonly IDynamicsClient _dynamicsClient;
		private readonly BCEPWrapper _bcep;

		public PaymentController(IConfiguration configuration,
								 IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory,
								 IDynamicsClient dynamicsClient, BCEPWrapper bcep)
		{
			Configuration = configuration;			
			this._httpContextAccessor = httpContextAccessor;
			this._dynamicsClient = dynamicsClient;
			this._bcep = bcep;
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
			_logger.LogError("Called GetPaymentUrl(" + id + ")");

			// get the application and confirm access (call parse to ensure we are getting a valid id)
			Guid applicationId = Guid.Parse(id);
			MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await GetDynamicsApplication(id);
			if (adoxioApplication == null)
			{
				return NotFound();
			}

			// set the application invoice trigger to create an invoice
			ViewModels.AdoxioApplication vm = await adoxioApplication.ToViewModel(_dynamicsClient);
			MicrosoftDynamicsCRMadoxioApplication adoxioApplication2 = new MicrosoftDynamicsCRMadoxioApplication();
			adoxioApplication2.CopyValues(vm);
			// this is the money - setting this flag to "Y" triggers a dynamics workflow that creates an invoice
			adoxioApplication2.AdoxioInvoicetrigger = (int?)ViewModels.GeneralYesNo.Yes;
			_dynamicsClient.Applications.Update(id, adoxioApplication2);
			adoxioApplication2 = await GetDynamicsApplication(id);

			// now load the invoice for this application to get the pricing
			string invoiceId = adoxioApplication2._adoxioInvoiceValue;
			int retries = 0;
			while (retries < 10 && (invoiceId == null || invoiceId.Length == 0))
			{
				// should happen immediately, but ...
				// pause and try again - in case Dynamics is slow ...
				retries++;
				_logger.LogError("No invoice found, retry = " + retries);
				System.Threading.Thread.Sleep(1000);
				invoiceId = adoxioApplication2._adoxioInvoiceValue;
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
			redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, id, String.Format("{0:0.00}", orderamt));

			_logger.LogError(">>>>>" + redirectUrl["url"]);

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
			_logger.LogError("Called GetLicencePaymentUrl(" + id + ")");

            // get the application and confirm access (call parse to ensure we are getting a valid id)
            Guid applicationId = Guid.Parse(id);
            MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await GetDynamicsApplication(id);

            if (adoxioApplication == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(adoxioApplication._adoxioLicencefeeinvoiceValue))
            {

                MicrosoftDynamicsCRMinvoice invoice2 = await _dynamicsClient.GetInvoiceById(Guid.Parse(adoxioApplication._adoxioLicencefeeinvoiceValue));
                if (invoice2 != null && invoice2.Statecode == (int)Adoxio_invoicestates.Cancelled)
                {
                    // set the application invoice trigger to create an invoice
                    ViewModels.AdoxioApplication vm = await adoxioApplication.ToViewModel(_dynamicsClient);
                    MicrosoftDynamicsCRMadoxioApplication adoxioApplication2 = new MicrosoftDynamicsCRMadoxioApplication();
                    adoxioApplication2.CopyValues(vm);
                    // this is the money - setting this flag to "Y" triggers a dynamics workflow that creates an invoice
                    adoxioApplication2.AdoxioLicenceFeeInvoiceTrigger = (int?)ViewModels.GeneralYesNo.Yes;
                    _dynamicsClient.Applications.Update(id, adoxioApplication2);
                    adoxioApplication = await GetDynamicsApplication(id);
                }
            }
            string invoiceId = adoxioApplication._adoxioLicencefeeinvoiceValue;
           


			int retries = 0;
			while (retries < 10 && string.IsNullOrEmpty(invoiceId))
			{
                // should happen immediately, but ...
                // pause and try again - in case Dynamics is slow ...
                adoxioApplication = await GetDynamicsApplication(id);
                retries++;
				_logger.LogError("No invoice found, retry = " + retries);
				System.Threading.Thread.Sleep(1000);
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

            var redirectPath = Configuration["BASE_URI"] + Configuration["BASE_PATH"] + "licence-fee-payment-confirmation";
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
            MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await GetDynamicsApplication(id);
            if (adoxioApplication == null)
            {
                return NotFound();
            }

            // load the invoice for this application
            string invoiceId = adoxioApplication._adoxioInvoiceValue;
            _logger.LogError("Found invoice for application = " + invoiceId);
            MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(Guid.Parse(invoiceId));
            var ordernum = invoice.AdoxioTransactionid;
            var orderamt = invoice.Totalamount;

            var response = await _bcep.ProcessPaymentResponse(ordernum, id);
            response["invoice"] = invoice.Invoicenumber;

            foreach (var key in response.Keys)
            {
                _logger.LogError(">>>>>" + key + ":" + response[key]);
            }

            /* 
			 * - if the invoice status is not "New", skip
             * - we will update the Invoice status to "Complete" (if paid) or "Cancelled" (if payment was rejected)
             * - if payment is successful, we will also set the Application "Payment Received" to "Y" and "Method" to "Credit Card"
             */

            if (invoice.Statecode == (int?)Adoxio_invoicestates.New || invoice.Statecode == null)
            {
                _logger.LogError("Processing invoice with status New");

                ViewModels.Invoice vmi = invoice.ToViewModel();
                MicrosoftDynamicsCRMinvoice invoice2 = new MicrosoftDynamicsCRMinvoice();
                invoice2.CopyValues(vmi);

                ViewModels.AdoxioApplication vma = await adoxioApplication.ToViewModel(_dynamicsClient);
                MicrosoftDynamicsCRMadoxioApplication adoxioApplication2 = new MicrosoftDynamicsCRMadoxioApplication();
                adoxioApplication2.CopyValues(vma);

                // if payment was successful:
                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.LogError("Transaction approved");

                    // set invoice status to Complete
                    invoice2.Statecode = (int?)Adoxio_invoicestates.Paid;
                    invoice2.Statuscode = (int?)Adoxio_invoicestatuses.Paid;
                    invoice2.AdoxioReturnedtransactionid = response["trnId"];

                    _dynamicsClient.Invoices.Update(invoice2.Invoiceid, invoice2);

                    // set the Application payment status
                    adoxioApplication2.AdoxioPaymentrecieved = (bool?)true;
                    adoxioApplication2.AdoxioPaymentmethod = (int?)Adoxio_paymentmethods.CC;
                    adoxioApplication2.AdoxioAppchecklistpaymentreceived = (int?)ViewModels.GeneralYesNo.Yes;

                    _dynamicsClient.Applications.Update(id, adoxioApplication2);
                    adoxioApplication2 = await GetDynamicsApplication(id);

                }
                // if payment failed:
                else
                {
                    _logger.LogError("Transaction NOT approved");

                    // set invoice status to Cancelled
                    invoice2.Statecode = (int?)Adoxio_invoicestates.Cancelled;
                    invoice2.Statuscode = (int?)Adoxio_invoicestatuses.Cancelled;

                    _dynamicsClient.Invoices.Update(invoice2.Invoiceid, invoice2);

                    // set the Application invoice status back to No
                    adoxioApplication2.AdoxioInvoicetrigger = (int?)ViewModels.GeneralYesNo.No;
                    // don't clear the invoice, leave the previous "Cancelled" so we can report status
                    //adoxioApplication2._adoxioInvoiceValue = null;
                    //adoxioApplication2.AdoxioInvoice = null;

                    _dynamicsClient.Applications.Update(id, adoxioApplication2);
                    adoxioApplication2 = await GetDynamicsApplication(id);

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
        /// Update a payment response from Bamboora (payment success or failed)
        /// This can be called if no response is received from Bamboora - it will query the server directly
        /// based on the Application's Invoice number
        /// This will also update the invoice payment status, and, if the payment is successful,
        /// it will push the Application into Submitted status
        /// </summary>
        /// <param name="id">GUID of the Application to pay</param>
        /// <returns></returns>
        [HttpGet("verify/licence-fee/{id}")]
        public async Task<IActionResult> VerifyLicenceFeePaymentStatus(string id)
        {
            MicrosoftDynamicsCRMadoxioApplication adoxioApplication = await GetDynamicsApplication(id);
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
                _logger.LogError(">>>>>" + key + ":" + response[key]);
            }

            /* 
			 * - if the invoice status is not "New", skip
             * - we will update the Invoice status to "Complete" (if paid) or "Cancelled" (if payment was rejected)
             * - if payment is successful, we will also set the Application "Payment Received" to "Y" and "Method" to "Credit Card"
             */

            if (invoice.Statecode == (int?)Adoxio_invoicestates.New || invoice.Statecode == null)
            {
                _logger.LogError("Processing invoice with status New");

                ViewModels.Invoice vmi = invoice.ToViewModel();
                MicrosoftDynamicsCRMinvoice invoice2 = new MicrosoftDynamicsCRMinvoice();
                invoice2.CopyValues(vmi);

                ViewModels.AdoxioApplication vma = await adoxioApplication.ToViewModel(_dynamicsClient);
                MicrosoftDynamicsCRMadoxioApplication adoxioApplication2 = new MicrosoftDynamicsCRMadoxioApplication();
                adoxioApplication2.CopyValues(vma);

                // if payment was successful:
                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.LogError("Transaction approved");

                    // set invoice status to Complete
                    invoice2.Statecode = (int?)Adoxio_invoicestates.Paid;
                    invoice2.Statuscode = (int?)Adoxio_invoicestatuses.Paid;
                    invoice2.AdoxioReturnedtransactionid = response["trnId"];

                    _dynamicsClient.Invoices.Update(invoice2.Invoiceid, invoice2);

                    // set the Application payment status
                    adoxioApplication2.AdoxioLicencefeeinvoicepaid = true;

                    _dynamicsClient.Applications.Update(id, adoxioApplication2);
                    adoxioApplication2 = await GetDynamicsApplication(id);

                }
                // if payment failed:
                else
                {
                    _logger.LogError("Transaction NOT approved");

                    // set invoice status to Cancelled
                    invoice2.Statecode = (int?)Adoxio_invoicestates.Cancelled;
                    invoice2.Statuscode = (int?)Adoxio_invoicestatuses.Cancelled;

                    _dynamicsClient.Invoices.Update(invoice2.Invoiceid, invoice2);

                    // set the Application invoice status back to No
                    MicrosoftDynamicsCRMadoxioApplication patchApplication = new MicrosoftDynamicsCRMadoxioApplication();
                    patchApplication.AdoxioLicenceFeeInvoiceTrigger = (int?)ViewModels.GeneralYesNo.No;
                    _dynamicsClient.Applications.Update(id, patchApplication);
                }
            }
            else
            {
                // that can happen if we are re-validating a completed invoice (paid or cancelled)
                _logger.LogError("Invoice status is not New, skipping updates ...");
            }

            return Json(response);
        }

        private async Task<MicrosoftDynamicsCRMadoxioApplication> GetDynamicsApplication(string id)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            _logger.LogError("Application id = " + id);
            _logger.LogError("User id = " + userSettings.AccountId);

            var dynamicsApplication = await _dynamicsClient.GetApplicationById(Guid.Parse(id));
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
        private async Task<MicrosoftDynamicsCRMadoxioWorker> GetDynamicsWorker(string id)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            _logger.LogError("Worker id = " + id);
            _logger.LogError("User Contact id = " + userSettings.ContactId);

            var worker = await _dynamicsClient.GetWorkerById(Guid.Parse(id));
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
            _logger.LogError($"Called GetWorkerPaymentUrl({workerId})");

            // get the application and confirm access (call parse to ensure we are getting a valid id)
            Guid applicationId = Guid.Parse(workerId);
            MicrosoftDynamicsCRMadoxioWorker worker = await GetDynamicsWorker(workerId);
            if (worker == null)
            {
                return NotFound();
            }

            // set the application invoice trigger to create an invoice
            ViewModels.Worker vm = worker.ToViewModel();
            MicrosoftDynamicsCRMadoxioWorker patchWorker = new MicrosoftDynamicsCRMadoxioWorker();
            patchWorker.CopyValues(vm);
            // this is the money - setting this flag to "Y" triggers a dynamics workflow that creates an invoice
            patchWorker.AdoxioInvoicetrigger = (int?)ViewModels.GeneralYesNo.Yes;
            _dynamicsClient.Workers.Update(workerId, patchWorker);
            patchWorker = await GetDynamicsWorker(workerId);

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
                invoiceId = patchWorker._adoxioInvoiceValue;
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
            var redirectPath = Configuration["BASE_URI"] + Configuration["BASE_PATH"] + Configuration["BCEP_CONF_PATH_WORKER"];
            redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, workerId, String.Format("{0:0.00}", orderamt), redirectPath);

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
        /// <param name="workerId">GUID of the Application to pay</param>
        /// <returns></returns>
        [HttpGet("verify/worker/{workerId}")]
        public async Task<IActionResult> VerifyWorkerPaymentStatus(string workerId)
        {
            MicrosoftDynamicsCRMadoxioWorker worker = await GetDynamicsWorker(workerId);
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
                _logger.LogError(">>>>>" + key + ":" + response[key]);
            }

            /* 
			 * - if the invoice status is not "New", skip
             * - we will update the Invoice status to "Complete" (if paid) or "Cancelled" (if payment was rejected)
             * - if payment is successful, we will also set the Application "Payment Received" to "Y" and "Method" to "Credit Card"
             */

            if (invoice.Statecode == (int?)Adoxio_invoicestates.New || invoice.Statecode == null)
            {
                _logger.LogError("Processing invoice with status New");

                ViewModels.Invoice vmi = invoice.ToViewModel();
                MicrosoftDynamicsCRMinvoice invoice2 = new MicrosoftDynamicsCRMinvoice();
                invoice2.CopyValues(vmi);

                ViewModels.Worker workerVM =  worker.ToViewModel();
                var patchWorker = new MicrosoftDynamicsCRMadoxioWorker();
                patchWorker.CopyValues(workerVM);

                // if payment was successful:
                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.LogError("Transaction approved");

                    // set invoice status to Complete
                    invoice2.Statecode = (int?)Adoxio_invoicestates.Paid;
                    invoice2.Statuscode = (int?)Adoxio_invoicestatuses.Paid;
                    invoice2.AdoxioReturnedtransactionid = response["trnId"];

                    _dynamicsClient.Invoices.Update(invoice2.Invoiceid, invoice2);

                    // set the Application payment status
                    patchWorker.AdoxioPaymentreceived = 1;
                    patchWorker.AdoxioPaymentreceiveddate = DateTime.UtcNow;
                    //patchWorker.AdoxioPaymentmethod = (int?)Adoxio_paymentmethods.CC;
                    //patchWorker.AdoxioAppchecklistpaymentreceived = (int?)ViewModels.GeneralYesNo.Yes;

                    _dynamicsClient.Workers.Update(workerId, patchWorker);
                    patchWorker = await GetDynamicsWorker(workerId);

                }
                // if payment failed:
                else
                {
                    _logger.LogError("Transaction NOT approved");

                    // set invoice status to Cancelled
                    invoice2.Statecode = (int?)Adoxio_invoicestates.Cancelled;
                    invoice2.Statuscode = (int?)Adoxio_invoicestatuses.Cancelled;

                    _dynamicsClient.Invoices.Update(invoice2.Invoiceid, invoice2);

                    // set the Application invoice status back to No
                    patchWorker.AdoxioInvoicetrigger = (int?)ViewModels.GeneralYesNo.No;
                    // don't clear the invoice, leave the previous "Cancelled" so we can report status
                    //adoxioApplication2._adoxioInvoiceValue = null;
                    //adoxioApplication2.AdoxioInvoice = null;

                    _dynamicsClient.Workers.Update(workerId, patchWorker);
                    patchWorker = await GetDynamicsWorker(workerId);

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
