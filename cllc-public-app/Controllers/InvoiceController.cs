using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class InvoiceController : Controller
    {
        private readonly IConfiguration Configuration;        
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ILogger _logger;        

		public InvoiceController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            Configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
            this._dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(AdoxioLegalEntityController));                    
        }

        /// <summary>
        /// Get all Invoices
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet()]
		public IActionResult GetInvoices()
        {
			if (TestUtility.InUnitTestMode())
            {
				List<ViewModels.Invoice> result = new List<Invoice>();
                IEnumerable<MicrosoftDynamicsCRMinvoice> invoices = null;

                invoices = _dynamicsClient.Invoices.Get().Value;

                foreach (var invoice in invoices)
                {
                    result.Add(invoice.ToViewModel());
                }

                return Json(result);
            }
            return new NotFoundResult();
        }


        /// <summary>
        /// Get a specific invoice
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(string id)
        {
			if (TestUtility.InUnitTestMode())
            {
                ViewModels.Invoice result = null;
                // query the Dynamics system to get the invoice record.
                if (string.IsNullOrEmpty(id))
                {
                    return new NotFoundResult();
                }
                else
                {
    				// get the current user.
                    string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
                    UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

    				Guid adoxio_legalentityid = new Guid(id);
                    MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(adoxio_legalentityid);                
                    if (invoice == null)
                    {
                        return new NotFoundResult();
                    }      
                    
                    // setup the related account.
                    if (invoice._accountidValue != null)
                    {
                        Guid accountId = Guid.Parse(invoice._accountidValue);
                        invoice.CustomeridAccount = await _dynamicsClient.GetAccountById(accountId);
                    }

                    result = invoice.ToViewModel();                
                }

                return Json(result);
			}
            return new NotFoundResult();
        }


        /// <summary>
        /// Create a invoice
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateInvoice([FromBody] ViewModels.Invoice item)
        {
			if (TestUtility.InUnitTestMode())
            {
                // create a new invoice.
                MicrosoftDynamicsCRMinvoice invoice = new MicrosoftDynamicsCRMinvoice();

    			// get the current user.
                string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
                UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
                // check that the session is setup correctly.
                userSettings.Validate();
                // copy received values to Dynamics LegalEntity
                invoice.CopyValues(item);
                try
                {
                    invoice = await _dynamicsClient.Invoices.CreateAsync(invoice);
                }
                catch (OdataerrorException odee)
                {
                    _logger.LogError("Error creating invoice");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                    throw new Exception("Unable to create invoice");
                }

                // setup navigation properties.
                MicrosoftDynamicsCRMinvoice patchEntity = new MicrosoftDynamicsCRMinvoice();
                Guid accountId = Guid.Parse(userSettings.AccountId);
                var userAccount = await _dynamicsClient.GetAccountById(accountId);
                patchEntity.CustomerIdAccountODataBind = _dynamicsClient.GetEntityURI("accounts", accountId.ToString());

                // patch the record.
                try
                {
                    await _dynamicsClient.Invoices.UpdateAsync(invoice.Invoiceid, patchEntity);
                    // setup the view model.
                    invoice.CustomeridAccount = userAccount;
                }
                catch (OdataerrorException odee)
                {
                    _logger.LogError("Error patching invoice");
                    _logger.LogError(odee.Request.RequestUri.ToString());
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                }

                return Json(invoice.ToViewModel());
			}
            return new NotFoundResult();
        }

        /// <summary>
        /// Update a invoice
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice([FromBody] ViewModels.Invoice item, string id)
        {
			if (TestUtility.InUnitTestMode())
            {
                if (id != item.id)
                {
                    return BadRequest();
                }

                // get the invoice.
                Guid adoxio_legalentityid = new Guid(id);

                MicrosoftDynamicsCRMinvoice invoice = await _dynamicsClient.GetInvoiceById(adoxio_legalentityid);
                if (invoice == null)
                {
                    return new NotFoundResult();
                }

                // we are doing a patch, so wipe out the record.
                invoice = new MicrosoftDynamicsCRMinvoice();

                // copy values over from the data provided
                invoice.CopyValues(item);

                await _dynamicsClient.Invoices.UpdateAsync(adoxio_legalentityid.ToString(), invoice);
                return Json(invoice.ToViewModel());
			}
            return new NotFoundResult();
        }

        /// <summary>
        /// Delete a invoice.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteInvoice(string id)
        {
			if (TestUtility.InUnitTestMode())
            {
                // get the invoice.
                Guid adoxio_legalentityid = new Guid(id);
                MicrosoftDynamicsCRMinvoice legalEntity = await _dynamicsClient.GetInvoiceById(adoxio_legalentityid);
                if (legalEntity == null)
                {
                    return new NotFoundResult();
                }
                try
                {
                    await _dynamicsClient.Invoices.DeleteAsync(adoxio_legalentityid.ToString());
                    return NoContent(); // 204
                }
                catch (OdataerrorException odee)
                {
                    _logger.LogError("Error deleteing invoice");
                    _logger.LogError(odee.Request.RequestUri.ToString());
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                }                
			}
            return new NotFoundResult();
        }
    }
}
