using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Logging;
using Gov.Lclb.Cllb.Interfaces.Models;
using Newtonsoft.Json;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Controllers
{
	[Route("api/[controller]")]
	public class PaymentController : Controller
    {
		private static Random random = new Random();

		private readonly IConfiguration Configuration;
        private readonly Interfaces.Microsoft.Dynamics.CRM.System _system;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IDynamicsClient _dynamicsClient;
		private readonly BCEPWrapper _bcep;
        
		public PaymentController(Interfaces.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, 
		                         IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, 
		                         IDynamicsClient dynamicsClient, BCEPWrapper bcep)
        {
			Configuration = configuration;
            this._system = context;
            this._httpContextAccessor = httpContextAccessor;
            this._dynamicsClient = dynamicsClient;
			this._bcep = bcep;
			_logger = loggerFactory.CreateLogger(typeof(PaymentController));
        }

		public static string RandomOrderNum(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

		public static string GetOrderNumForApplication(MicrosoftDynamicsCRMadoxioApplication application)
		{
			string ordernum = "04";
			foreach (char ch in application.AdoxioApplicationid)
			{
				if (0 <= "0123456789".IndexOf(ch))
					ordernum += ch;
				if (10 <= ordernum.Length)
					return ordernum;
			}
			while (10 > ordernum.Length)
				ordernum += "0";
			return ordernum;
			//return "04" + RandomOrderNum(8);
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
			adoxioApplication2.AdoxioInvoicetrigger = (int?)ViewModels.GeneralYesNo.Yes;
            _dynamicsClient.Applications.Update(id, adoxioApplication2);
			adoxioApplication2 = await GetDynamicsApplication(id);

			// load the invoice for this application
			string invoiceId = adoxioApplication2._adoxioInvoiceValue;
			int retries = 0;
			while (retries < 10 && (invoiceId == null || invoiceId.Length == 0))
			{
				// pause and try again - in case Dynamics is slow ...
				retries++;
				_logger.LogError("No invoice found, retry = " + retries);
				System.Threading.Thread.Sleep(1000);
				invoiceId = adoxioApplication2._adoxioInvoiceValue;
			}
			_logger.LogError("Created invoice for application = " + invoiceId);

			MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(Guid.Parse(invoiceId));
			//var ordernum = GetOrderNumForApplication(adoxioApplication2); 
			var ordernum = invoice.AdoxioTransactionid;
			var orderamt = invoice.Totalamount;

			Dictionary<string, string> redirectUrl;
			redirectUrl = new Dictionary<string, string>();
			redirectUrl["url"] = await _bcep.GeneratePaymentRedirectUrl(ordernum, id, String.Format("{0:0.00}", orderamt));

			_logger.LogError(">>>>>" + redirectUrl["url"]);

			return Json(redirectUrl);
		}

		/// <summary>
        /// Update a payment response from Bamboora (payment success or failed)
        /// This is called when the re-direct is received back from Bamboora.
		/// This will also update the invoice payment status, and, if the payment is successful,
		/// it will push the Application into Submitted status
        /// </summary>
        /// <param name="id">GUID of the Application to pay</param>
        /// <returns></returns>
        [HttpGet("update/{id}")]
		public async Task<IActionResult> UpdatePaymentStatus(string id)
        {
			MicrosoftDynamicsCRMadoxioApplication result = await GetDynamicsApplication(id);
            if (result == null)
            {
                return NotFound();
            }

            Dictionary<string, string> redirectUrl;
            redirectUrl = new Dictionary<string, string>();
            redirectUrl["url"] = "https://google.ca?id=" + id;

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
			MicrosoftDynamicsCRMadoxioApplication result = await GetDynamicsApplication(id);
            if (result == null)
            {
                return NotFound();
            }

			string ordernum = GetOrderNumForApplication(result);

            var response = await _bcep.ProcessPaymentResponse(ordernum, id);

			foreach (var key in response.Keys)
			{
				_logger.LogError(">>>>>" + key + ":" + response[key]);
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
    }
}
