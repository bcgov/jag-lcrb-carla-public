extern alias DV;
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using Gov.Lclb.Cllb.Public.ViewModels;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using adoxio_application_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application;
using adoxio_worker_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_worker;
using adoxio_specialevent_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent;
using adoxio_applicationextension_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicationextension;
using adoxio_generalyesno_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno;
using adoxio_application_paymentmethod = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_paymentmethod;
using adoxio_applicationtype_category = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicationtype_adoxio_category;
using adoxio_application_statuscode_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_statuscode;
using DvInvoice = DV::Gov.Lclb.Cllb.Interfaces.Invoice;
using invoice_statecode = DV::Gov.Lclb.Cllb.Interfaces.invoice_statecode;
using invoice_statuscode = DV::Gov.Lclb.Cllb.Interfaces.invoice_statuscode;


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
        private readonly IDataverseClient _dataverse;
        private readonly Serilog.ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public PaymentController(IConfiguration configuration,
                                 IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory,
                                 IDataverseClient dataverse,
                                 IBCEPService bcep, IGeocoderService geocoderClient)
        {
            _configuration = configuration;
            _bcep = bcep;
            _dataverse = dataverse;
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
            _logger.Debug($"Called GetPaymentUrl({id})");

            // get the application and confirm access (call parse to ensure we are getting a valid id)
            Guid applicationId = Guid.Parse(id);
            adoxio_application_dv application = await GetDynamicsApplication(id);
            if (application == null)
            {
                return NotFound("Application not found");
            }

            // Check if application's primary invoice is already paid
            string existingInvoiceId = application.adoxio_Invoice?.Id.ToString();
            if (existingInvoiceId != null)
            {
                var existingInvoice = await _dataverse.GetInvoiceByIdAsync(existingInvoiceId);
                if (existingInvoice?.StatusCode == invoice_statuscode.Complete)
                {
                    return NotFound("Payment already made");
                }
                else
                {
                    //TODO Reverify Payment Status with BCEP
                    bool invoicePaid = await ReVerifyPaymentStatus(id);
                    if (invoicePaid)
                    {
                        return NotFound("Payment already made");
                    }
                }
            }

            // set the application invoice trigger to create an invoice
            var patchApplication = new adoxio_application_dv { Id = applicationId };
            patchApplication.adoxio_InvoiceTrigger = adoxio_generalyesno_dv.Yes;

            await _dataverse.UpdateApplicationAsync(patchApplication);

            application = await GetDynamicsApplication(id);

            // now load the invoice for this application to get the pricing
            string invoiceId = application.adoxio_Invoice?.Id.ToString();
            int retries = 0;
            while (retries < 10 && string.IsNullOrEmpty(invoiceId))
            {
                retries++;
                _logger.Error($"No application {id} invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(2000);
                application = await GetDynamicsApplication(id);
                invoiceId = application.adoxio_Invoice?.Id.ToString();
            }

            if (!string.IsNullOrEmpty(invoiceId))
            {
                _logger.Debug("Created invoice for application = " + invoiceId);

                DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
                var ordernum = invoice.adoxio_TransactionID;
                var orderamt = invoice.TotalAmount?.Value;

                Dictionary<string, string> redirectUrl = new Dictionary<string, string>();

                PaymentType paymentType = await application.GetPaymentTypeAsync(_dataverse);

                redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, id, String.Format("{0:0.00}", orderamt), paymentType);

                _logger.Debug($"Payment redirect url = {redirectUrl["url"]}");

                return new JsonResult(redirectUrl);
            }

            _logger.Error($"GetPaymentUrl failed - Unable to get invoice for application {id}");
            return NotFound();
        }

        /// <summary>
        /// GET a payment re-direct url for an Application
        /// This will register an (unpaid) invoice against the application and generate an invoice number,
        /// which will be used to match payments
        /// </summary>
        /// <param name="invoiceType">Allowed values: 'primary' and 'secondary'</param>
        /// <param name="id">GUID of the Application to pay</param>
        /// <param name="redirectContext">
        /// Optional context for determining the post-payment redirect URL.
        /// Allowed values: "permanent-change" and "legal-entity".
        /// If not provided, the default value is "permanent-change".
        /// </param>
        /// <returns></returns>
        [HttpGet("payment-uri/{invoiceType}/{id}")]
        public async Task<IActionResult> GetPaymentUrlUpdated(string id, string invoiceType, [FromQuery] string redirectContext = "permanent-change")
        {
            _logger.Debug($"Called GetPaymentUrlUpdated({id})");

            const string primary = "primary";
            const string secondary = "secondary";

            adoxio_application_dv application = await GetDynamicsApplication(id);
            if (application == null)
            {
                return NotFound($"Application not found for id {id}");
            }

            if (invoiceType != primary && invoiceType != secondary)
            {
                return BadRequest($"Invalid invoiceType {invoiceType}");
            }

            if (redirectContext != "permanent-change" && redirectContext != "legal-entity")
            {
                return BadRequest($"Invalid redirectContext {redirectContext}");
            }

            bool invoicePaid = false;

            if (invoiceType == primary)
            {
                _logger.Debug($"Primary invoice for application {id} has been paid");
                invoicePaid = application.adoxio_PrimaryApplicationInvoicePaid == adoxio_generalyesno_dv.Yes;
            }
            else if (invoiceType == secondary)
            {
                _logger.Debug($"Secondary invoice for application {id} has been paid");
                invoicePaid = application.adoxio_SecondaryApplicationInvoicePaid == adoxio_generalyesno_dv.Yes;
            }

            if (invoicePaid)
            {
                return NotFound($"Payment for application {id} already made");
            }
            else
            {
                if (application.adoxio_Invoice?.Id != null)
                {
                    //TODO Reverify Payment Status with BCEP
                    var verifyInvoicePaid = await ReVerifyPaymentStatus(id);
                    if (verifyInvoicePaid)
                    {
                        return NotFound($"Payment for application {id} already made");
                    }
                }
            }

            var patch = new adoxio_application_dv { Id = Guid.Parse(id) };
            patch.adoxio_InvoiceTrigger = adoxio_generalyesno_dv.Yes;

            await _dataverse.UpdateApplicationAsync(patch);

            application = await GetDynamicsApplication(id);
            string invoiceId = application.adoxio_Invoice?.Id.ToString();
            if (invoiceType == secondary)
            {
                invoiceId = application.adoxio_SecondaryApplicationInvoice?.Id.ToString();
            }
            int retries = 0;
            while (retries < 10 && string.IsNullOrEmpty(invoiceId))
            {
                retries++;
                _logger.Warning($"No application {id} invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(2000);
                application = await GetDynamicsApplication(id);
                invoiceId = application.adoxio_Invoice?.Id.ToString();
                if (invoiceType == secondary)
                {
                    invoiceId = application.adoxio_SecondaryApplicationInvoice?.Id.ToString();
                }
            }

            if (string.IsNullOrEmpty(invoiceId))
            {
                _logger.Error($"GetPaymentUrl failed - Unable to get invoice for application {id}");
                return NotFound($"Invoice not found for application {id}");
            }

            _logger.Debug($"Created invoice for application {id}. Invoice {invoiceId}");

            DvInvoice invoice2 = await _dataverse.GetInvoiceByIdAsync(invoiceId);
            var ordernum2 = invoice2.adoxio_TransactionID;
            var orderamt2 = invoice2.TotalAmount?.Value;

            Dictionary<string, string> redirectUrl2 = new Dictionary<string, string>();

            bool isAlternateAccount = (invoiceType == secondary);
            PaymentType paymentType2 = PaymentType.CANNABIS;
            if (isAlternateAccount)
            {
                paymentType2 = PaymentType.LIQUOR;
            }

            string redirectPath;
            if (redirectContext == "permanent-change")
            {
                redirectPath =
                    $"{_configuration["BASE_URI"]}{_configuration["BASE_PATH"]}/permanent-change-to-a-licensee/{id}/{invoiceType}";
            }
            else
            {
                redirectPath =
                    $"{_configuration["BASE_URI"]}{_configuration["BASE_PATH"]}/legal-entity-review-permanent-change-to-a-licensee/{id}/{invoiceType}";
            }

            redirectUrl2["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum2, id, String.Format("{0:0.00}", orderamt2), paymentType2, redirectPath);

            _logger.Debug($"Payment redirect url = {redirectUrl2["url"]}");

            return new JsonResult(redirectUrl2);
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
            _logger.Debug($"Called GetLicencePaymentUrl({id})");

            adoxio_application_dv application = await GetDynamicsApplication(id);

            if (application == null)
            {
                return NotFound("Application not found");
            }

            string licFeeInvoiceId = application.adoxio_LicenceFeeInvoice?.Id.ToString();

            // Check if licence fee invoice is already paid
            if (licFeeInvoiceId != null)
            {
                var lfInvoice = await _dataverse.GetInvoiceByIdAsync(licFeeInvoiceId);
                if (lfInvoice?.StatusCode == invoice_statuscode.Complete)
                {
                    if (application.adoxio_LicenceFeeInvoicePaid == false)
                    {
                        var fixPatch = new adoxio_application_dv { Id = Guid.Parse(id) };
                        fixPatch.adoxio_LicenceFeeInvoicePaid = true;
                        await _dataverse.UpdateApplicationAsync(fixPatch);
                    }
                    return NotFound("Payment already made");
                }
                else
                {
                    //TODO Reverify Payment Status with BCEP
                    bool invoicePaid = await ReVerifyLicenceFeePaymentStatus(id);
                    if (invoicePaid)
                    {
                        return NotFound("Payment already made");
                    }
                }
            }

            if (!string.IsNullOrEmpty(licFeeInvoiceId))
            {
                DvInvoice invoice2 = await _dataverse.GetInvoiceByIdAsync(licFeeInvoiceId);
                if (invoice2 != null && invoice2.StateCode == invoice_statecode.Canceled)
                {
                    var triggerPatch = new adoxio_application_dv { Id = Guid.Parse(id) };
                    triggerPatch.adoxio_LicenceFeeInvoiceTrigger = adoxio_generalyesno_dv.Yes;
                    await _dataverse.UpdateApplicationAsync(triggerPatch);
                    application = await GetDynamicsApplication(id);
                }
            }

            string invoiceId = application.adoxio_LicenceFeeInvoice?.Id.ToString();

            int retries = 0;
            while (retries < 10 && string.IsNullOrEmpty(invoiceId))
            {
                retries++;
                _logger.Debug("No invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(1000);
                application = await GetDynamicsApplication(id);
                invoiceId = application.adoxio_LicenceFeeInvoice?.Id.ToString();
            }

            if (string.IsNullOrEmpty(invoiceId))
            {
                _logger.Error($"No invoice found for application {id}");
                return NotFound();
            }

            _logger.Debug("Created invoice for application = " + invoiceId);

            DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
            var ordernum = invoice.adoxio_TransactionID;
            var orderamt = invoice.TotalAmount?.Value;

            PaymentType paymentType = await application.GetPaymentTypeAsync(_dataverse);

            Dictionary<string, string> redirectUrl = new Dictionary<string, string>();

            var redirectPath = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/licence-fee-payment-confirmation";
            redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, id, String.Format("{0:0.00}", orderamt), paymentType, redirectPath);

            _logger.Debug($"Payment redirect url = {redirectUrl["url"]}");

            return new JsonResult(redirectUrl);
        }

        /// <summary>
        /// GET a payment re-direct url for an Application additional Fee
        /// This will register an (unpaid) invoice against the application licence and generate an invoice number,
        /// which will be used to match payments
        /// </summary>
        /// <param name="id">GUID of the Application to pay licence fee</param>
        /// <returns></returns>
        [HttpGet("submit/outstanding-prior-balance-invoice/{id}")]
        public async Task<IActionResult> GetOutStandingPriorBalanceInvoicePaymentUrl(string id)
        {
            _logger.Debug($"Called GetOutStandingPriorBalanceInvoicePaymentUrl({id})");

            adoxio_application_dv application = await GetDynamicsApplication(id);

            if (application == null)
            {
                return NotFound("Application not found");
            }

            string licFeeInvoiceId = application.adoxio_LicenceFeeInvoice?.Id.ToString();

            if (licFeeInvoiceId != null)
            {
                var lfInvoice = await _dataverse.GetInvoiceByIdAsync(licFeeInvoiceId);
                if (lfInvoice?.StatusCode == invoice_statuscode.Complete)
                {
                    if (application.adoxio_LicenceFeeInvoicePaid == false)
                    {
                        var fixPatch = new adoxio_application_dv { Id = Guid.Parse(id) };
                        fixPatch.adoxio_LicenceFeeInvoicePaid = true;
                        await _dataverse.UpdateApplicationAsync(fixPatch);
                    }
                    return NotFound("Payment already made");
                }
                else
                {
                    //TODO Reverify Payment Status with BCEP
                    bool invoicePaid = await ReVerifyLicenceFeePaymentStatus(id);
                    if (invoicePaid)
                    {
                        return NotFound("Payment already made");
                    }
                }
            }

            if (!string.IsNullOrEmpty(licFeeInvoiceId))
            {
                DvInvoice invoice2 = await _dataverse.GetInvoiceByIdAsync(licFeeInvoiceId);
                if (invoice2 != null && invoice2.StateCode == invoice_statecode.Canceled)
                {
                    var triggerPatch = new adoxio_application_dv { Id = Guid.Parse(id) };
                    triggerPatch.adoxio_LicenceFeeInvoiceTrigger = adoxio_generalyesno_dv.Yes;
                    await _dataverse.UpdateApplicationAsync(triggerPatch);
                    application = await GetDynamicsApplication(id);
                }
            }

            string invoiceId = application.adoxio_LicenceFeeInvoice?.Id.ToString();

            int retries = 0;
            while (retries < 10 && string.IsNullOrEmpty(invoiceId))
            {
                retries++;
                _logger.Debug("No invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(1000);
                application = await GetDynamicsApplication(id);
                invoiceId = application.adoxio_LicenceFeeInvoice?.Id.ToString();
            }

            if (!string.IsNullOrEmpty(invoiceId))
            {
                _logger.Debug("Created invoice for application = " + invoiceId);

                DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
                var ordernum = invoice.adoxio_TransactionID;
                var orderamt = invoice.TotalAmount?.Value;

                PaymentType paymentType = await application.GetPaymentTypeAsync(_dataverse);

                Dictionary<string, string> redirectUrl = new Dictionary<string, string>();

                var redirectPath = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/licence-fee-payment-confirmation";
                redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, id, String.Format("{0:0.00}", orderamt), paymentType, redirectPath);

                _logger.Debug($"Payment redirect url = {redirectUrl["url"]}");

                return new JsonResult(redirectUrl);
            }

            _logger.Error($"No invoice found for application {id}");
            return NotFound();
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
            adoxio_application_dv application = await GetDynamicsApplication(id);
            if (application == null)
            {
                return NotFound("Application not found");
            }

            string invoiceId = application.adoxio_Invoice?.Id.ToString();

            //LCSD-6409 handle invoiceID is null issue:
            int retries = 0;
            while (retries < 10 && string.IsNullOrEmpty(invoiceId))
            {
                retries++;
                _logger.Error($"No application {id} invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(2000);
                application = await GetDynamicsApplication(id);
                invoiceId = application.adoxio_Invoice?.Id.ToString();
            }
            if (invoiceId == null)
            {
                _logger.Error($"No application {id} invoice found after 10 times retries. ");
                return NotFound();
            }

            _logger.Debug("Found invoice for application = " + invoiceId);
            DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
            var ordernum = invoice.adoxio_TransactionID;
            var orderamt = invoice.TotalAmount?.Value;

            PaymentType paymentType = await application.GetPaymentTypeAsync(_dataverse);

            var response = await _bcep.ProcessPaymentResponse(ordernum, id, paymentType);

            if (response.ContainsKey("error"))
            {
                _logger.Error($"PAYMENT VERIFICATION ERROR - {response["message"]} for application {id}");
                return StatusCode(500, response);
            }

            response["invoice"] = invoice.InvoiceNumber;
            response["paymentType"] = paymentType.ToString();

            // LCSD-6224: if Application is for Renewal, send renewLicenseNumber to front-end
            string renewLicenseNumber = "";
            if (application.adoxio_ApplicationTypeId?.Id is Guid appTypeGuid)
            {
                var appType = await _dataverse.GetApplicationTypeByIdAsync(appTypeGuid.ToString());
                if (appType?.adoxio_IsRenewal == true && application.adoxio_AssignedLicence?.Id is Guid licenceGuid)
                {
                    var licence = await _dataverse.GetLicenceByIdAsync(licenceGuid.ToString());
                    if (licence != null)
                    {
                        var licVM = await licence.ToViewModelAsync(_dataverse);
                        renewLicenseNumber = licVM.LicenseNumber ?? "";
                    }
                }
            }
            response["renewLicenseNumber"] = renewLicenseNumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            if (invoice.StateCode == invoice_statecode.Active || invoice.StateCode == null)
            {
                _logger.Debug("Processing invoice with status New");

                var pay_status = response["trnApproved"];
                var messageId = response["messageId"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Paid;
                    patchInvoice.StatusCode = invoice_statuscode.Complete;
                    patchInvoice.adoxio_returnedtransactionid = response["trnId"];
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                    patchApp.adoxio_PaymentRecieved = true;
                    patchApp.adoxio_PaymentMethod = adoxio_application_paymentmethod.CreditCard;
                    patchApp.adoxio_AppChecklistPaymentReceived = adoxio_generalyesno_dv.Yes;
                    await _dataverse.UpdateApplicationAsync(patchApp);

                    _logger.Information($"Payment approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Liquor: {paymentType}");
                }
                else
                {
                    _logger.Debug("Transaction NOT approved");
                    if (messageId != "559" && messageId != "761")
                    {
                        var patchInvoice = new DvInvoice { Id = invoice.Id };
                        patchInvoice.StateCode = invoice_statecode.Canceled;
                        patchInvoice.StatusCode = invoice_statuscode.Canceled;
                        await _dataverse.UpdateInvoiceAsync(patchInvoice);

                        var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                        patchApp.adoxio_InvoiceTrigger = adoxio_generalyesno_dv.No;
                        await _dataverse.UpdateApplicationAsync(patchApp);
                    }

                    _logger.Information($"Payment not approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Liquor: {paymentType}");
                }
            }
            else
            {
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            return new JsonResult(response);
        }

        private async Task<bool> ReVerifyPaymentStatus(string id)
        {
            Boolean toReturn = false;
            adoxio_application_dv application = await GetDynamicsApplication(id);
            if (application == null)
            {
                return toReturn;
            }

            string invoiceId = application.adoxio_Invoice?.Id.ToString();
            if (string.IsNullOrEmpty(invoiceId)) return toReturn;

            _logger.Debug("Found invoice for application = " + invoiceId);
            DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
            var ordernum = invoice.adoxio_TransactionID;

            PaymentType paymentType = await application.GetPaymentTypeAsync(_dataverse);

            var response = await _bcep.ProcessPaymentResponse(ordernum, id, paymentType);

            if (response.ContainsKey("error"))
            {
                _logger.Error($"PAYMENT RE-VERIFICATION ERROR - {response["message"]} for application {id}");
                return toReturn;
            }
            var messageId = response["messageId"];
            if (messageId == "559" || messageId == "761")
            {
                return false;
            }

            response["invoice"] = invoice.InvoiceNumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            if (invoice.StateCode == invoice_statecode.Active || invoice.StateCode == null)
            {
                _logger.Debug("Processing invoice with status New");

                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Paid;
                    patchInvoice.StatusCode = invoice_statuscode.Complete;
                    patchInvoice.adoxio_returnedtransactionid = response["trnId"];
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                    patchApp.adoxio_PaymentRecieved = true;
                    patchApp.adoxio_PaymentMethod = adoxio_application_paymentmethod.CreditCard;
                    patchApp.adoxio_AppChecklistPaymentReceived = adoxio_generalyesno_dv.Yes;
                    await _dataverse.UpdateApplicationAsync(patchApp);

                    toReturn = true;
                    _logger.Information($"Payment approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Liquor: {paymentType}");
                }
                else
                {
                    _logger.Debug("Transaction NOT approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Canceled;
                    patchInvoice.StatusCode = invoice_statuscode.Canceled;
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                    patchApp.adoxio_InvoiceTrigger = adoxio_generalyesno_dv.No;
                    await _dataverse.UpdateApplicationAsync(patchApp);

                    _logger.Information($"Payment not approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Liquor: {paymentType}");
                }
            }
            else
            {
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            return toReturn;
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
            adoxio_application_dv application = await GetDynamicsApplication(id);
            if (application == null)
            {
                return NotFound("Application not found");
            }

            string invoiceId = application.adoxio_LicenceFeeInvoice?.Id.ToString();
            _logger.Debug("Found invoice for application = " + invoiceId);
            DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
            var ordernum = invoice.adoxio_TransactionID;

            PaymentType paymentType = await application.GetPaymentTypeAsync(_dataverse);

            var response = await _bcep.ProcessPaymentResponse(ordernum, id, paymentType);

            if (response.ContainsKey("error"))
            {
                _logger.Error($"PAYMENT VERIFICATION ERROR - {response["message"]} for application {id}");
                return StatusCode(503);
            }

            response["invoice"] = invoice.InvoiceNumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            if (invoice.StateCode == invoice_statecode.Active || invoice.StateCode == null)
            {
                _logger.Debug("Processing invoice with status New");

                var pay_status = response["trnApproved"];
                var messageId = response["messageId"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Paid;
                    patchInvoice.StatusCode = invoice_statuscode.Complete;
                    patchInvoice.adoxio_returnedtransactionid = response["trnId"];
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                    patchApp.adoxio_LicenceFeeInvoicePaid = true;
                    patchApp.adoxio_PaymentRecieved = true;
                    patchApp.adoxio_PaymentMethod = adoxio_application_paymentmethod.CreditCard;
                    await _dataverse.UpdateApplicationAsync(patchApp);

                    if (!string.IsNullOrEmpty(_configuration["FEATURE_MAPS"]))
                    {
                        await _geocoderClient.GeocodeEstablishment(application.adoxio_LicenceEstablishment?.Id.ToString(), _logger);
                    }

                    _logger.Information($"Licence Fee Transaction approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Liquor: {paymentType}");
                }
                else
                {
                    _logger.Debug("Transaction NOT approved");
                    if (messageId != "559" && messageId != "761")
                    {
                        var patchInvoice = new DvInvoice { Id = invoice.Id };
                        patchInvoice.StateCode = invoice_statecode.Canceled;
                        patchInvoice.StatusCode = invoice_statuscode.Canceled;
                        await _dataverse.UpdateInvoiceAsync(patchInvoice);

                        var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                        patchApp.adoxio_LicenceFeeInvoiceTrigger = adoxio_generalyesno_dv.No;
                        await _dataverse.UpdateApplicationAsync(patchApp);
                    }
                    _logger.Information($"Licence Fee Transaction NOT approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Liquor: {paymentType}");
                }
            }
            else
            {
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            response.Add("description", invoice.Description);

            return new JsonResult(response);
        }

        public async Task<bool> ReVerifyLicenceFeePaymentStatus(string id)
        {
            adoxio_application_dv application = await GetDynamicsApplication(id);
            if (application == null)
            {
                return false;
            }

            string invoiceId = application.adoxio_LicenceFeeInvoice?.Id.ToString();
            if (string.IsNullOrEmpty(invoiceId)) return false;

            _logger.Debug("Found invoice for application = " + invoiceId);
            DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
            var ordernum = invoice.adoxio_TransactionID;

            PaymentType paymentType = await application.GetPaymentTypeAsync(_dataverse);

            var response = await _bcep.ProcessPaymentResponse(ordernum, id, paymentType);

            if (response.ContainsKey("error"))
            {
                _logger.Error($"PAYMENT Re-VERIFICATION ERROR - {response["message"]} for application {id}");
                return false;
            }
            var messageId = response["messageId"];
            if (messageId == "559" || messageId == "761")
            {
                return false;
            }

            response["invoice"] = invoice.InvoiceNumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            if (invoice.StateCode == invoice_statecode.Active || invoice.StateCode == null)
            {
                _logger.Debug("Processing invoice with status New");

                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Paid;
                    patchInvoice.StatusCode = invoice_statuscode.Complete;
                    patchInvoice.adoxio_returnedtransactionid = response["trnId"];
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                    patchApp.adoxio_LicenceFeeInvoicePaid = true;
                    await _dataverse.UpdateApplicationAsync(patchApp);

                    if (!string.IsNullOrEmpty(_configuration["FEATURE_MAPS"]))
                    {
                        await _geocoderClient.GeocodeEstablishment(application.adoxio_LicenceEstablishment?.Id.ToString(), _logger);
                    }

                    _logger.Information($"Licence Fee Transaction approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Liquor: {paymentType}");
                    return true;
                }
                else
                {
                    _logger.Debug("Transaction NOT approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Canceled;
                    patchInvoice.StatusCode = invoice_statuscode.Canceled;
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                    patchApp.adoxio_LicenceFeeInvoiceTrigger = adoxio_generalyesno_dv.No;
                    await _dataverse.UpdateApplicationAsync(patchApp);

                    _logger.Information($"Licence Fee Transaction NOT approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Liquor: {paymentType}");
                    return false;
                }
            }
            else
            {
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            return false;
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
        [HttpGet("verify-by-invoice-type/{invoiceType}/{id}")]
        public async Task<IActionResult> VerifyPaymentStatus(string id, string invoiceType)
        {
            const string primary = "primary";
            const string secondary = "secondary";

            if (invoiceType != primary && invoiceType != secondary)
            {
                return BadRequest("Invalid Invoice Type");
            }

            adoxio_application_dv application = await GetDynamicsApplication(id);
            if (application == null)
            {
                return NotFound("Application not found");
            }

            string invoiceId = application.adoxio_Invoice?.Id.ToString();
            if (invoiceType == secondary)
            {
                invoiceId = application.adoxio_SecondaryApplicationInvoice?.Id.ToString();
            }

            Guid invoiceGuid = Guid.Parse(invoiceId);
            _logger.Debug("Found invoice for application = " + invoiceId);
            DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceGuid.ToString());
            string ordernum = invoice.adoxio_TransactionID;

            bool isAlternateAccount = (invoiceType == secondary);

            PaymentType paymentType = PaymentType.CANNABIS;
            if (isAlternateAccount)
            {
                paymentType = PaymentType.LIQUOR;
            }

            var response = await _bcep.ProcessPaymentResponse(ordernum, id, paymentType);

            if (response.ContainsKey("error"))
            {
                _logger.Error($"PAYMENT VERIFICATION ERROR - {response["message"]} for application {id}");
                return StatusCode(500, response);
            }

            response["invoice"] = invoice.InvoiceNumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            if (invoice.StateCode == invoice_statecode.Active || invoice.StateCode == null)
            {
                _logger.Debug("Processing invoice with status New");

                var pay_status = response["trnApproved"];
                var messageId = response["messageId"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Paid;
                    patchInvoice.StatusCode = invoice_statuscode.Complete;
                    patchInvoice.adoxio_returnedtransactionid = response["trnId"];
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                    if (invoiceType == secondary)
                    {
                        patchApp.adoxio_SecondaryApplicationInvoicePaid = adoxio_generalyesno_dv.Yes;
                    }
                    else
                    {
                        patchApp.adoxio_PrimaryApplicationInvoicePaid = adoxio_generalyesno_dv.Yes;
                    }
                    await _dataverse.UpdateApplicationAsync(patchApp);

                    _logger.Information($"Payment approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Liquor: {isAlternateAccount}");
                }
                else
                {
                    _logger.Debug("Transaction NOT approved");
                    if (messageId != "559" && messageId != "761")
                    {
                        var patchInvoice = new DvInvoice { Id = invoice.Id };
                        patchInvoice.StateCode = invoice_statecode.Canceled;
                        patchInvoice.StatusCode = invoice_statuscode.Canceled;
                        await _dataverse.UpdateInvoiceAsync(patchInvoice);

                        var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                        patchApp.adoxio_InvoiceTrigger = adoxio_generalyesno_dv.No;
                        await _dataverse.UpdateApplicationAsync(patchApp);
                    }
                    _logger.Information($"Payment not approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Liquor: {isAlternateAccount}");
                }
            }
            else
            {
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            return new JsonResult(response);
        }

        /// <summary>
        /// Get the payment status for a PCL application for a user with liquor invoices — DV overload.
        /// </summary>
        public static async Task<PaymentResult> GetLiquorPaymentStatus(adoxio_application_dv application, IDataverseClient dataverse, IBCEPService bcep)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));
            if (dataverse == null) throw new ArgumentNullException(nameof(dataverse));
            if (bcep == null) throw new ArgumentNullException(nameof(bcep));

            var invoiceId = application.adoxio_SecondaryApplicationInvoice?.Id.ToString();

            if (string.IsNullOrEmpty(invoiceId)) return null;

            Log.Debug("Found invoice for application = " + invoiceId);
            DvInvoice invoice = await dataverse.GetInvoiceByIdAsync(invoiceId).ConfigureAwait(true);
            string ordernum = invoice.adoxio_TransactionID;
            var totalAmount = invoice.TotalAmount?.Value;

            if (totalAmount <= 0)
            {
                var patchInvoice = new DvInvoice { Id = invoice.Id };
                patchInvoice.StateCode = invoice_statecode.Paid;
                patchInvoice.StatusCode = invoice_statuscode.Complete;
                patchInvoice.adoxio_returnedtransactionid = null;
                await dataverse.UpdateInvoiceAsync(patchInvoice);

                var patchApp = new adoxio_application_dv { Id = application.Id };
                patchApp.adoxio_PrimaryApplicationInvoicePaid = adoxio_generalyesno_dv.Yes;
                await dataverse.UpdateApplicationAsync(patchApp);

                var ext = await dataverse.GetApplicationExtensionByApplicationIdAsync(application.Id.ToString());
                if (ext?.adoxio_relatedleorpclapplication?.Id != null)
                {
                    await UpdateRelatedLeReviewStatus(ext.adoxio_relatedleorpclapplication.Id.ToString(), application.Id.ToString(), dataverse);
                }

                return null;
            }

            var response = await bcep.ProcessPaymentResponse(ordernum, application.Id.ToString(), PaymentType.LIQUOR);

            if (response.ContainsKey("error"))
            {
                throw new Exception("Error in response");
            }

            response["invoice"] = invoice.InvoiceNumber;

            foreach (var key in response.Keys)
            {
                Log.Debug("GetLiquorPaymentStatus - Payment Response: " + key + ":" + response[key]);
            }

            if (invoice.StateCode == invoice_statecode.Active || invoice.StateCode == null)
            {
                Log.Debug("Processing invoice with status New");

                var pay_status = response["trnApproved"];
                var messageId = response["messageId"];

                if (pay_status == "1")
                {
                    Log.Debug("Transaction approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Paid;
                    patchInvoice.StatusCode = invoice_statuscode.Complete;
                    patchInvoice.adoxio_returnedtransactionid = response["trnId"];
                    await dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchApp = new adoxio_application_dv { Id = application.Id };
                    patchApp.adoxio_SecondaryApplicationInvoicePaid = adoxio_generalyesno_dv.Yes;
                    await dataverse.UpdateApplicationAsync(patchApp);

                    var ext = await dataverse.GetApplicationExtensionByApplicationIdAsync(application.Id.ToString());
                    if (ext?.adoxio_relatedleorpclapplication?.Id != null)
                    {
                        await UpdateRelatedLeReviewStatus(ext.adoxio_relatedleorpclapplication.Id.ToString(), application.Id.ToString(), dataverse);
                    }

                    Log.Information($"Liquor Invoice Payment approved.  Application ID: {application.Id} Invoice: {invoice.InvoiceNumber}.");
                }
                else
                {
                    Log.Debug("Transaction NOT approved");
                    if (messageId != "559" && messageId != "761")
                    {
                        var patchInvoice = new DvInvoice { Id = invoice.Id };
                        patchInvoice.StateCode = invoice_statecode.Canceled;
                        patchInvoice.StatusCode = invoice_statuscode.Canceled;
                        await dataverse.UpdateInvoiceAsync(patchInvoice);

                        var patchApp = new adoxio_application_dv { Id = application.Id };
                        patchApp.adoxio_InvoiceTrigger = adoxio_generalyesno_dv.No;
                        await dataverse.UpdateApplicationAsync(patchApp);
                    }

                    Log.Information($"Liquor Invoice Payment not approved.  Application ID: {application.Id} Invoice: {invoice.InvoiceNumber}.");
                }
            }
            else
            {
                Log.Debug("Invoice status is not New, skipping updates ...");
            }

            return new PaymentResult(response);
        }

        /// <summary>
        /// Get the payment status for a PCL application for a user with cannabis invoices — DV overload.
        /// </summary>
        public static async Task<PaymentResult> GetCannabisPaymentStatus(adoxio_application_dv application, IDataverseClient dataverse, IBCEPService bcep)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));
            if (dataverse == null) throw new ArgumentNullException(nameof(dataverse));
            if (bcep == null) throw new ArgumentNullException(nameof(bcep));

            string invoiceId = application.adoxio_Invoice?.Id.ToString();

            if (string.IsNullOrEmpty(invoiceId)) return null;

            Log.Debug("Found invoice for application = " + invoiceId);
            DvInvoice invoice = await dataverse.GetInvoiceByIdAsync(invoiceId).ConfigureAwait(true);
            string ordernum = invoice.adoxio_TransactionID;
            var totalAmount = invoice.TotalAmount?.Value;

            if (totalAmount <= 0)
            {
                var patchInvoice = new DvInvoice { Id = invoice.Id };
                patchInvoice.StateCode = invoice_statecode.Paid;
                patchInvoice.StatusCode = invoice_statuscode.Complete;
                patchInvoice.adoxio_returnedtransactionid = null;
                await dataverse.UpdateInvoiceAsync(patchInvoice);

                var patchApp = new adoxio_application_dv { Id = application.Id };
                patchApp.adoxio_PrimaryApplicationInvoicePaid = adoxio_generalyesno_dv.Yes;
                await dataverse.UpdateApplicationAsync(patchApp);

                var ext = await dataverse.GetApplicationExtensionByApplicationIdAsync(application.Id.ToString());
                if (ext?.adoxio_relatedleorpclapplication?.Id != null)
                {
                    await UpdateRelatedLeReviewStatus(ext.adoxio_relatedleorpclapplication.Id.ToString(), application.Id.ToString(), dataverse);
                }

                return null;
            }

            var response = await bcep.ProcessPaymentResponse(ordernum, application.Id.ToString(), PaymentType.CANNABIS);

            if (response.ContainsKey("error"))
            {
                throw new Exception("Error in response");
            }

            response["invoice"] = invoice.InvoiceNumber;

            foreach (var key in response.Keys)
            {
                Log.Debug("GetCannabisPaymentStatus - Payment Response: " + key + ":" + response[key]);
            }

            if (invoice.StateCode == invoice_statecode.Active || invoice.StateCode == null)
            {
                Log.Debug("Processing invoice with status New");

                var pay_status = response["trnApproved"];
                var messageId = response["messageId"];
                if (pay_status == "1")
                {
                    Log.Debug("Transaction approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Paid;
                    patchInvoice.StatusCode = invoice_statuscode.Complete;
                    patchInvoice.adoxio_returnedtransactionid = response["trnId"];
                    await dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchApp = new adoxio_application_dv { Id = application.Id };
                    patchApp.adoxio_PrimaryApplicationInvoicePaid = adoxio_generalyesno_dv.Yes;
                    await dataverse.UpdateApplicationAsync(patchApp);

                    var ext = await dataverse.GetApplicationExtensionByApplicationIdAsync(application.Id.ToString());
                    if (ext?.adoxio_relatedleorpclapplication?.Id != null)
                    {
                        await UpdateRelatedLeReviewStatus(ext.adoxio_relatedleorpclapplication.Id.ToString(), application.Id.ToString(), dataverse);
                    }

                    Log.Information($"Cannabis Invoice Payment approved.  Application ID: {application.Id} Invoice: {invoice.InvoiceNumber}.");
                }
                else
                {
                    Log.Debug("Transaction NOT approved");
                    if (messageId != "559" && messageId != "761")
                    {
                        var patchInvoice = new DvInvoice { Id = invoice.Id };
                        patchInvoice.StateCode = invoice_statecode.Canceled;
                        patchInvoice.StatusCode = invoice_statuscode.Canceled;
                        await dataverse.UpdateInvoiceAsync(patchInvoice);

                        var patchApp = new adoxio_application_dv { Id = application.Id };
                        patchApp.adoxio_InvoiceTrigger = adoxio_generalyesno_dv.No;
                        await dataverse.UpdateApplicationAsync(patchApp);
                    }

                    Log.Information($"Cannabis Invoice Payment not approved.  Application ID: {application.Id} Invoice: {invoice.InvoiceNumber}.");
                }
            }
            else
            {
                Log.Debug("Invoice status is not New, skipping updates ...");
            }

            return new PaymentResult(response);
        }

        private async Task<adoxio_application_dv> GetDynamicsApplication(string id)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            _logger.Debug($"Application id = {id}");
            _logger.Debug($"User id = {userSettings.AccountId}");

            var app = await _dataverse.GetApplicationByIdWithChildrenAsync(id);

            if (app == null)
            {
                return null;
            }

            if (!CurrentUserHasAccessToApplicationOwnedBy(app.adoxio_Applicant?.Id.ToString()))
            {
                return null;
            }

            return app;
        }

        private async Task<adoxio_worker_dv> GetDynamicsWorker(string workerId, bool getInvoice)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            _logger.Debug($"Worker id = {workerId}");
            _logger.Debug($"User Contact id = {userSettings.ContactId}");

            var worker = await _dataverse.GetWorkerByIdAsync(workerId);

            if (worker == null)
            {
                return null;
            }

            if (worker.adoxio_ContactId?.Id.ToString() != userSettings.ContactId)
            {
                return null;
            }

            return worker;
        }

        private bool CurrentUserHasAccessToApplicationOwnedBy(string accountId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return userSettings.AccountId == accountId;
            }

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

            adoxio_worker_dv worker = await GetDynamicsWorker(workerId, true);
            if (worker == null)
            {
                return NotFound();
            }

            string existingInvoiceId = worker.adoxio_Invoice?.Id.ToString();
            if (existingInvoiceId != null)
            {
                var existingInvoice = await _dataverse.GetInvoiceByIdAsync(existingInvoiceId);
                if (existingInvoice?.StatusCode == invoice_statuscode.Complete)
                {
                    return NotFound("Payment already made");
                }
                else
                {
                    bool invoicePaid = await ReVerifyWorkerPaymentStatus(workerId);
                    if (invoicePaid)
                    {
                        return NotFound("Payment already made");
                    }
                }
            }

            var patchWorker = new adoxio_worker_dv { Id = Guid.Parse(workerId) };
            patchWorker.adoxio_InvoiceTrigger = adoxio_generalyesno_dv.Yes;
            await _dataverse.UpdateWorkerAsync(patchWorker);

            worker = await GetDynamicsWorker(workerId, false);

            string invoiceId = worker.adoxio_Invoice?.Id.ToString();
            int retries = 0;
            while (retries < 10 && string.IsNullOrEmpty(invoiceId))
            {
                retries++;
                _logger.Debug($"No invoice found, retry = {retries}");
                System.Threading.Thread.Sleep(1000);
                worker = await GetDynamicsWorker(workerId, false);
                invoiceId = worker.adoxio_Invoice?.Id.ToString();
            }
            _logger.Debug($"Created invoice for worker = {invoiceId}");

            DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
            var ordernum = invoice.adoxio_TransactionID;
            var orderamt = invoice.TotalAmount?.Value;

            PaymentType paymentType = PaymentType.CANNABIS;

            Dictionary<string, string> redirectUrl = new Dictionary<string, string>();
            var redirectPath = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/worker-qualification/payment-confirmation";
            redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, workerId, String.Format("{0:0.00}", orderamt), paymentType, redirectPath);

            _logger.Debug($"Payment redirect url = {redirectUrl["url"]}");

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
            adoxio_worker_dv worker = await GetDynamicsWorker(workerId, true);
            if (worker == null)
            {
                return NotFound();
            }

            string invoiceId = worker.adoxio_Invoice?.Id.ToString();
            _logger.Debug("Found invoice for application = " + invoiceId);
            DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
            var ordernum = invoice.adoxio_TransactionID;

            PaymentType paymentType = PaymentType.CANNABIS;

            var response = await _bcep.ProcessPaymentResponse(ordernum, workerId, paymentType);

            if (response.ContainsKey("error"))
            {
                _logger.Error($"PAYMENT VERIFICATION ERROR - {response["message"]} for worker {workerId}");
                return StatusCode(503);
            }

            response["invoice"] = invoice.InvoiceNumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            if (invoice.StateCode == invoice_statecode.Active || invoice.StateCode == null)
            {
                _logger.Debug("Processing invoice with status New");

                var pay_status = response["trnApproved"];
                var messageId = response["messageId"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Paid;
                    patchInvoice.StatusCode = invoice_statuscode.Complete;
                    patchInvoice.adoxio_returnedtransactionid = response["trnId"];
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchWorker = new adoxio_worker_dv { Id = Guid.Parse(workerId) };
                    patchWorker.adoxio_PaymentReceived = adoxio_generalyesno_dv.Yes;
                    patchWorker.adoxio_PaymentReceivedDate = DateTime.UtcNow;
                    await _dataverse.UpdateWorkerAsync(patchWorker);
                }
                else
                {
                    _logger.Debug("Transaction NOT approved");
                    if (messageId != "559" && messageId != "761")
                    {
                        var patchInvoice = new DvInvoice { Id = invoice.Id };
                        patchInvoice.StateCode = invoice_statecode.Canceled;
                        patchInvoice.StatusCode = invoice_statuscode.Canceled;
                        await _dataverse.UpdateInvoiceAsync(patchInvoice);

                        var patchWorker = new adoxio_worker_dv { Id = Guid.Parse(workerId) };
                        patchWorker.adoxio_InvoiceTrigger = adoxio_generalyesno_dv.No;
                        await _dataverse.UpdateWorkerAsync(patchWorker);
                    }
                }
            }
            else
            {
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            return new JsonResult(response);
        }

        private async Task<bool> ReVerifyWorkerPaymentStatus(string workerId)
        {
            Boolean toReturn = false;
            adoxio_worker_dv worker = await GetDynamicsWorker(workerId, true);
            if (worker == null)
            {
                return toReturn;
            }

            string invoiceId = worker.adoxio_Invoice?.Id.ToString();
            if (string.IsNullOrEmpty(invoiceId)) return toReturn;

            _logger.Debug("Found invoice for application = " + invoiceId);
            DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
            var ordernum = invoice.adoxio_TransactionID;

            PaymentType paymentType = PaymentType.CANNABIS;

            var response = await _bcep.ProcessPaymentResponse(ordernum, workerId, paymentType);

            if (response.ContainsKey("error"))
            {
                _logger.Error($"PAYMENT VERIFICATION ERROR - {response["message"]} for worker {workerId}");
                return toReturn;
            }
            var messageId = response["messageId"];
            if (messageId == "559" || messageId == "761")
            {
                return false;
            }

            response["invoice"] = invoice.InvoiceNumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            if (invoice.StateCode == invoice_statecode.Active || invoice.StateCode == null)
            {
                _logger.Debug("Processing invoice with status New");

                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");
                    toReturn = true;

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Paid;
                    patchInvoice.StatusCode = invoice_statuscode.Complete;
                    patchInvoice.adoxio_returnedtransactionid = response["trnId"];
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchWorker = new adoxio_worker_dv { Id = Guid.Parse(workerId) };
                    patchWorker.adoxio_PaymentReceived = adoxio_generalyesno_dv.Yes;
                    patchWorker.adoxio_PaymentReceivedDate = DateTime.UtcNow;
                    await _dataverse.UpdateWorkerAsync(patchWorker);
                }
                else
                {
                    _logger.Debug("Transaction NOT approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Canceled;
                    patchInvoice.StatusCode = invoice_statuscode.Canceled;
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchWorker = new adoxio_worker_dv { Id = Guid.Parse(workerId) };
                    patchWorker.adoxio_InvoiceTrigger = adoxio_generalyesno_dv.No;
                    await _dataverse.UpdateWorkerAsync(patchWorker);
                }
            }
            else
            {
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            return toReturn;
        }

        private async Task<adoxio_specialevent_dv> GetSpecialEventDataAsync(string eventId)
        {
            if (string.IsNullOrEmpty(eventId)) return null;
            return await _dataverse.GetSpecialEventByIdAsync(eventId);
        }

        /// <summary>
        /// GET a payment re-direct url for an Application
        /// This will register an (unpaid) invoice against the application and generate an invoice number,
        /// which will be used to match payments
        /// </summary>
        /// <param name="id">GUID of the Application to pay</param>
        /// <returns></returns>
        [HttpGet("submit/sep-application/{id}")]
        public async Task<IActionResult> GetSepPaymentUrl(string id)
        {
            _logger.Debug($"Called GetSepPaymentUrl({id})");

            adoxio_specialevent_dv application = await GetSpecialEventDataAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            if (application.adoxio_IsInvoicePaid == true)
            {
                return NotFound("Payment already made");
            }
            else
            {
                if (application.adoxio_Invoice?.Id != null)
                {
                    //TODO Reverify Payment Status with BCEP
                    bool invoicePaid = await ReVerifySepPaymentStatus(id);
                    if (invoicePaid)
                    {
                        return NotFound("Payment already made");
                    }
                }
            }

            bool generateInvoice = false;
            string existingInvoiceId = application.adoxio_Invoice?.Id.ToString();
            if (string.IsNullOrEmpty(existingInvoiceId))
            {
                generateInvoice = true;
            }
            else
            {
                var existingInvoice = await _dataverse.GetInvoiceByIdAsync(existingInvoiceId);
                if (existingInvoice != null && existingInvoice.StateCode == invoice_statecode.Canceled)
                {
                    generateInvoice = true;
                }
            }

            if (generateInvoice)
            {
                var patchEvent = new adoxio_specialevent_dv { Id = Guid.Parse(id) };
                patchEvent.adoxio_InvoiceTrigger = true;
                await _dataverse.UpdateSpecialEventAsync(patchEvent);
                application = await GetSpecialEventDataAsync(id);
            }

            string invoiceId = application.adoxio_Invoice?.Id.ToString();

            int retries = 0;
            while (retries < 10 && string.IsNullOrEmpty(invoiceId))
            {
                retries++;
                _logger.Debug("No invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(2000);
                application = await GetSpecialEventDataAsync(id);
                invoiceId = application.adoxio_Invoice?.Id.ToString();
            }

            if (!string.IsNullOrEmpty(invoiceId))
            {
                _logger.Debug("Getting payment URL for SEP invoice with id = " + invoiceId);

                DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
                var ordernum = invoice.adoxio_TransactionID;
                var orderamt = invoice.TotalAmount?.Value;

                Dictionary<string, string> redirectUrl = new Dictionary<string, string>();

                PaymentType paymentType = PaymentType.SPECIAL_EVENT;

                var redirectPath = $"{_configuration["BASE_URI"]}{_configuration["BASE_PATH"]}/sep/application-summary/{id}";
                redirectUrl["url"] = _bcep.GeneratePaymentRedirectUrl(ordernum, id, String.Format("{0:0.00}", orderamt), paymentType, redirectPath);

                _logger.Debug($"Payment redirect url = {redirectUrl["url"]}");

                return new JsonResult(redirectUrl);
            }

            _logger.Error($"GetPaymentUrl failed - Invoice not found for application {id}");
            return NotFound();
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
        [HttpGet("verify/sep-application/{id}")]
        public async Task<IActionResult> VerifySepPaymentStatus(string id)
        {
            adoxio_specialevent_dv application = await GetSpecialEventDataAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            string invoiceId = application.adoxio_Invoice?.Id.ToString();

            int retries = 0;
            while (retries < 10 && string.IsNullOrEmpty(invoiceId))
            {
                retries++;
                _logger.Debug("No invoice found, retry = " + retries);
                System.Threading.Thread.Sleep(2000);
                application = await GetSpecialEventDataAsync(id);
                invoiceId = application.adoxio_Invoice?.Id.ToString();
            }
            if (invoiceId == null)
            {
                _logger.Error($"No application {id} invoice found after 10 times retries. ");
                return NotFound();
            }

            _logger.Debug("Found invoice for application = " + invoiceId);
            DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
            var ordernum = invoice.adoxio_TransactionID;

            PaymentType paymentType = PaymentType.SPECIAL_EVENT;

            var response = await _bcep.ProcessPaymentResponse(ordernum, id, paymentType);

            if (response.ContainsKey("error"))
            {
                _logger.Error($"PAYMENT VERIFICATION ERROR - {response["message"]} for SEP application {id}");
                return StatusCode(503);
            }

            response["invoice"] = invoice.InvoiceNumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            if (invoice.StateCode == invoice_statecode.Active || invoice.StateCode == null)
            {
                _logger.Debug("Processing invoice with status New");

                var pay_status = response["trnApproved"];
                var messageId = response["messageId"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Paid;
                    patchInvoice.StatusCode = invoice_statuscode.Complete;
                    patchInvoice.adoxio_returnedtransactionid = response["trnId"];
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchEvent = new adoxio_specialevent_dv { Id = Guid.Parse(id) };
                    patchEvent.adoxio_IsInvoicePaid = true;
                    await _dataverse.UpdateSpecialEventAsync(patchEvent);

                    _logger.Information($"SEP Application Payment Transaction approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} PaymentType: {paymentType}");
                }
                else
                {
                    _logger.Debug("Transaction NOT approved");
                    if (messageId != "559" && messageId != "761")
                    {
                        var patchInvoice = new DvInvoice { Id = invoice.Id };
                        patchInvoice.StateCode = invoice_statecode.Canceled;
                        patchInvoice.StatusCode = invoice_statuscode.Canceled;
                        await _dataverse.UpdateInvoiceAsync(patchInvoice);

                        var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                        patchApp.adoxio_LicenceFeeInvoiceTrigger = adoxio_generalyesno_dv.No;
                        await _dataverse.UpdateApplicationAsync(patchApp);
                    }
                    _logger.Information($"Licence Fee Transaction NOT approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Payment Type: {paymentType}");
                }
            }
            else
            {
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            return new JsonResult(response);
        }

        private async Task<bool> ReVerifySepPaymentStatus(string id)
        {
            bool toReturn = false;
            adoxio_specialevent_dv application = await GetSpecialEventDataAsync(id);
            if (application == null)
            {
                return toReturn;
            }

            string invoiceId = application.adoxio_Invoice?.Id.ToString();
            if (string.IsNullOrEmpty(invoiceId)) return toReturn;

            _logger.Debug("Found invoice for application = " + invoiceId);
            DvInvoice invoice = await _dataverse.GetInvoiceByIdAsync(invoiceId);
            var ordernum = invoice.adoxio_TransactionID;

            PaymentType paymentType = PaymentType.SPECIAL_EVENT;

            var response = await _bcep.ProcessPaymentResponse(ordernum, id, paymentType);

            if (response.ContainsKey("error"))
            {
                _logger.Error($"PAYMENT VERIFICATION ERROR - {response["message"]} for SEP application {id}");
                return toReturn;
            }
            var messageId = response["messageId"];
            if (messageId == "559" || messageId == "761")
            {
                return false;
            }

            response["invoice"] = invoice.InvoiceNumber;

            foreach (var key in response.Keys)
            {
                _logger.Debug(">>>>>" + key + ":" + response[key]);
            }

            if (invoice.StateCode == invoice_statecode.Active || invoice.StateCode == null)
            {
                _logger.Debug("Processing invoice with status New");

                var pay_status = response["trnApproved"];
                if (pay_status == "1")
                {
                    _logger.Debug("Transaction approved");
                    toReturn = true;

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Paid;
                    patchInvoice.StatusCode = invoice_statuscode.Complete;
                    patchInvoice.adoxio_returnedtransactionid = response["trnId"];
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchEvent = new adoxio_specialevent_dv { Id = Guid.Parse(id) };
                    patchEvent.adoxio_IsInvoicePaid = true;
                    await _dataverse.UpdateSpecialEventAsync(patchEvent);

                    _logger.Information($"SEP Application Payment Transaction approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} PaymentType: {paymentType}");
                }
                else
                {
                    _logger.Debug("Transaction NOT approved");

                    var patchInvoice = new DvInvoice { Id = invoice.Id };
                    patchInvoice.StateCode = invoice_statecode.Canceled;
                    patchInvoice.StatusCode = invoice_statuscode.Canceled;
                    await _dataverse.UpdateInvoiceAsync(patchInvoice);

                    var patchApp = new adoxio_application_dv { Id = Guid.Parse(id) };
                    patchApp.adoxio_LicenceFeeInvoiceTrigger = adoxio_generalyesno_dv.No;
                    await _dataverse.UpdateApplicationAsync(patchApp);

                    _logger.Information($"Licence Fee Transaction NOT approved.  Application ID: {id} Invoice: {invoice.InvoiceNumber} Payment Type: {paymentType}");
                }
            }
            else
            {
                _logger.Debug("Invoice status is not New, skipping updates ...");
            }

            return toReturn;
        }

        private async static Task UpdateRelatedLeReviewStatus(string leReviewId, string pclId, IDataverseClient dataverse)
        {
            var pclApplication = await dataverse.GetApplicationByIdAsync(pclId);
            if (pclApplication == null) return;

            if (pclApplication.adoxio_SecondaryApplicationInvoicePaid == adoxio_generalyesno_dv.Yes
                && pclApplication.adoxio_PrimaryApplicationInvoicePaid == adoxio_generalyesno_dv.Yes)
            {
                var patch = new adoxio_application_dv { Id = Guid.Parse(leReviewId) };
                patch.statuscode = (adoxio_application_statuscode_dv)AdoxioApplicationStatusCodes.UnderReview;
                await dataverse.UpdateApplicationAsync(patch);
            }
        }
    }
}
