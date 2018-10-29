using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class WorkerController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly PdfClient _pdfClient;

        public WorkerController(IConfiguration configuration, IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, PdfClient pdfClient)
        {
            Configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(WorkerController));
            _pdfClient = pdfClient;
        }

        /// <summary>
        /// Get a  workers  associated with the contactId
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        [HttpGet("contact/{contactId}")]
        public IActionResult GetWorkers(string contactId)
        {
            List<ViewModels.Worker> results = new List<ViewModels.Worker>();
            if (!string.IsNullOrEmpty(contactId))
            {
                Guid id = Guid.Parse(contactId);
                // query the Dynamics system to get the contact record.
                string filter = $"_adoxio_contactid_value eq {contactId}";
                var fields = new List<string> { "adoxio_ContactId" };
                List<MicrosoftDynamicsCRMadoxioWorker> workers = _dynamicsClient.Workers.Get(filter: filter, expand: fields).Value.ToList();
                if (workers != null)
                {
                    workers.ForEach(w =>
                    {
                        results.Add(w.ToViewModel());
                    });
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            else
            {
                return BadRequest();
            }
            return Json(results);
        }


        /// <summary>
        /// Get a specific worker
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetWorker(string id)
        {
            ViewModels.Worker result = null;
            if (!string.IsNullOrEmpty(id))
            {
                Guid workerId = Guid.Parse(id);
                // query the Dynamics system to get the contact record.
                string filter = $"adoxio_workerid eq {id}";
                var fields = new List<string> { "adoxio_ContactId" };
                MicrosoftDynamicsCRMadoxioWorker worker = _dynamicsClient.Workers.Get(filter: filter, expand: fields).Value.FirstOrDefault();
                if (worker != null)
                {
                    result = worker.ToViewModel();
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            else
            {
                return BadRequest();
            }
            return Json(result);
        }

        /// <summary>
        /// Update a worker
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorker([FromBody] ViewModels.Worker item, string id)
        {
            if (id != null && item.id != null && id != item.id)
            {
                return BadRequest();
            }

            // get the contact
            Guid workerId = Guid.Parse(id);

            MicrosoftDynamicsCRMadoxioWorker worker = await _dynamicsClient.GetWorkerById(Guid.Parse(item.id));
            if (worker == null)
            {
                return new NotFoundResult();
            }
            if(worker.Statuscode != (int)ViewModels.StatusCode.NotSubmitted)
            {
                return BadRequest("Applications with this status cannot be updated");
            }
            MicrosoftDynamicsCRMadoxioWorker patchWorker = new MicrosoftDynamicsCRMadoxioWorker();
            patchWorker.CopyValues(item);
            try
            {
                await _dynamicsClient.Workers.UpdateAsync(worker.AdoxioWorkerid.ToString(), patchWorker);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating contact");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
            }
            worker = await _dynamicsClient.GetWorkerById(workerId);
            return Json(worker.ToViewModel());
        }

        /// <summary>
        /// Create a worker    
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateWorker([FromBody] ViewModels.Worker item)
        {
            // get UserSettings from the session
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // create a new worker.
            MicrosoftDynamicsCRMadoxioWorker worker = new MicrosoftDynamicsCRMadoxioWorker();
            worker.CopyValues(item);
            if (item?.contact?.id == null)
            {
                return BadRequest();
            }
            try
            {
                worker = await _dynamicsClient.Workers.CreateAsync(worker);
                var patchWorker = new MicrosoftDynamicsCRMadoxioWorker();
                patchWorker.ContactIdAccountODataBind = _dynamicsClient.GetEntityURI("contacts", item.contact.id);
                await _dynamicsClient.Workers.UpdateAsync(worker.AdoxioWorkerid.ToString(), patchWorker);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error updating contact");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
            }
            return Json(worker);
        }


        /// <summary>
        /// Delete a Worker.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteWorker(string id)
        {
            MicrosoftDynamicsCRMadoxioWorker worker = await _dynamicsClient.GetWorkerById(Guid.Parse(id));
            if (worker == null)
            {
                return new NotFoundResult();
            }
            await _dynamicsClient.Workers.DeleteAsync(id);
            return NoContent(); // 204
        }

        /// GET a licence as PDF.
        [HttpGet("{workerId}/pdf")]
        public async Task<IActionResult> GetLicencePDF(string workerId)
        {

            var expand = new List<string> {
               "adoxio_ContactId",
               "adoxio_workerregistration_personalhistorysummary"
           };

            MicrosoftDynamicsCRMadoxioWorker adoxioWorker = _dynamicsClient.Workers.GetByKey(workerId, expand: expand);
            if (adoxioWorker == null)
            {
                throw new Exception("Error getting worker.");
            }

            var dateOfBirthParam = "";
            if (adoxioWorker.AdoxioDateofbirth.HasValue)
            {
                DateTime dateOfBirth = adoxioWorker.AdoxioDateofbirth.Value.DateTime;
                dateOfBirthParam = dateOfBirth.ToString("dd/MM/yyyy");
            }

            var effectiveDateParam = "";
            var securityClearance = adoxioWorker.AdoxioWorkerregistrationPersonalhistorysummary.FirstOrDefault();
            if (securityClearance != null && securityClearance.AdoxioCompletedon.HasValue)
            {
                DateTime effectiveDate = securityClearance.AdoxioCompletedon.Value.DateTime;
                effectiveDateParam = effectiveDate.ToString("dd/MM/yyyy");
            }

            var expiryDateParam = "";
            if (securityClearance != null && securityClearance.AdoxioExpirydate.HasValue)
            {
                DateTime expiryDate = securityClearance.AdoxioExpirydate.Value.DateTime;
                expiryDateParam = expiryDate.ToString("dd/MM/yyyy");
            }

            var parameters = new Dictionary<string, string>
            {
                { "title", "Worker_Qualification" },
                { "currentDate", DateTime.Now.ToLongDateString() },
                { "firstName", adoxioWorker.AdoxioFirstname },
                { "middleName", adoxioWorker.AdoxioMiddlename },
                { "lastName", adoxioWorker.AdoxioLastname },
                { "dateOfBirth", dateOfBirthParam },
                { "address", adoxioWorker.AdoxioContactId.Address1Line1 },
                { "city", adoxioWorker.AdoxioContactId.Address1City },
                { "province", adoxioWorker.AdoxioContactId.Address1Stateorprovince},
                { "postalCode", adoxioWorker.AdoxioContactId.Address1Postalcode},
                { "effectiveDate", effectiveDateParam },
                { "expiryDate", expiryDateParam },
                { "border", "{ \"top\": \"40px\", \"right\": \"40px\", \"bottom\": \"0px\", \"left\": \"40px\" }" }
            };

            try
            {
                byte[] data = await _pdfClient.GetPdf(parameters, "worker_qualification_letter");
                return File(data, "application/pdf");
            }
            catch
            {
                string basePath = string.IsNullOrEmpty(Configuration["BASE_PATH"]) ? "" : Configuration["BASE_PATH"];
                basePath += "/worker-qualification/dashboard";
                return Redirect(basePath);
            }
        }
    }
}
