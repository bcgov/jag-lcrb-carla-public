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

namespace Gov.Lclb.Cllb.Public.Controllers
{
	[Route("api/[controller]")]
	public class PaymentController : Controller
    {
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
			ViewModels.AdoxioApplication result = await GetDynamicsApplication(id);
			if (result == null)
			{
				return NotFound();
			}

			Dictionary<string, string> redirectUrl;
			redirectUrl = new Dictionary<string, string>();
			redirectUrl["url"] = await _bcep.GeneratePaymentRedirectUrl(id, "7500.00");

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
			ViewModels.AdoxioApplication result = await GetDynamicsApplication(id);
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
			ViewModels.AdoxioApplication result = await GetDynamicsApplication(id);
            if (result == null)
            {
                return NotFound();
            }

            Dictionary<string, string> redirectUrl;
            redirectUrl = new Dictionary<string, string>();
            redirectUrl["url"] = "https://google.ca?id=" + id;

            return Json(redirectUrl);
        }

		private async Task<ViewModels.AdoxioApplication> GetDynamicsApplication(string id)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            _logger.LogError("Application id = " + id);
            _logger.LogError("User id = " + userSettings.AccountId);

            ViewModels.AdoxioApplication result = null;
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
                result = await dynamicsApplication.ToViewModel(_dynamicsClient);
            }

            return result;
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
