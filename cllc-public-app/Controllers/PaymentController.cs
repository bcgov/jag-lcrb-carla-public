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

		public PaymentController(Interfaces.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, 
		                         IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
			Configuration = configuration;
            this._system = context;
            this._httpContextAccessor = httpContextAccessor;
            this._dynamicsClient = dynamicsClient;
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
		public async Task<JsonResult> GetPaymentUrl(string id)
		{
			_logger.LogError("Called GetPaymentUrl(" + id + ")");
			return Json("{ url: 'https://google.ca' }");
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
		public async Task<JsonResult> UpdatePaymentStatus(string id)
        {
			_logger.LogError("Called UpdatePaymentStatus(" + id + ")");
			return Json("{ url: 'https://google.ca' }");
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
		public async Task<JsonResult> VerifyPaymentStatus(string id)
        {
			_logger.LogError("Called VerifyPaymentStatus(" + id + ")");
			return Json("{ url: 'https://google.ca' }");
        }

    }
}
