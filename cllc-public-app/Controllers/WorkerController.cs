extern alias DV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using adoxio_worker = DV::Gov.Lclb.Cllb.Interfaces.adoxio_worker;
using adoxio_worker_statuscode = DV::Gov.Lclb.Cllb.Interfaces.adoxio_worker_statuscode;
using adoxio_generalyesno = DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno;
using DvContact = DV::Gov.Lclb.Cllb.Interfaces.Contact;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDataverseClient _dataverse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IPdfService _pdfClient;
        private readonly FileManagerClient _fileManagerClient;

        public WorkerController(IConfiguration configuration, IDataverseClient dataverse, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IPdfService pdfClient, FileManagerClient fileClient)
        {
            _configuration = configuration;
            _dataverse = dataverse;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(WorkerController));
            _pdfClient = pdfClient;
            _fileManagerClient = fileClient;
        }

        /// <summary>
        /// Get workers associated with the contactId
        /// </summary>
        [HttpGet("contact/{contactId}")]
        public async Task<IActionResult> GetWorkers(string contactId)
        {
            if (!CurrentUserHasAccessToContactWorkerApplicationOwnedBy(contactId))
                return NotFound("No access to contact");

            if (string.IsNullOrEmpty(contactId))
                return BadRequest();

            var results = new List<ViewModels.Worker>();
            var workers = await _dataverse.GetWorkersByContactIdAsync(contactId);
            foreach (var w in workers)
                results.Add(w.ToViewModel());

            var sharedContact = await _dataverse.GetContactByIdAsync(contactId);

            if (results.Count == 0)
            {
                if (sharedContact != null)
                {
                    var worker = new ViewModels.Worker
                    {
                        firstname = sharedContact.FirstName,
                        middlename = sharedContact.MiddleName,
                        lastname = sharedContact.LastName,
                        contact = new ViewModels.Contact { id = contactId }
                    };
                    worker = await this.CreateWorkerRecord(worker);
                    worker.contact = sharedContact.ToViewModel();
                    results.Add(worker);
                }
            }
            else if (sharedContact != null)
            {
                foreach (var r in results)
                    r.contact = sharedContact.ToViewModel();
            }

            return new JsonResult(results);
        }

        /// <summary>
        /// Get a specific worker
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorker(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var worker = await _dataverse.GetWorkerByIdAsync(id);
            if (worker == null)
                return NotFound();

            if (!CurrentUserHasAccessToContactWorkerApplicationOwnedBy(worker.adoxio_ContactId?.Id.ToString()))
                return NotFound("No access to worker");

            var workerVm = worker.ToViewModel();
            if (worker.adoxio_ContactId != null)
            {
                var contact = await _dataverse.GetContactByIdAsync(worker.adoxio_ContactId.Id.ToString());
                if (contact != null)
                    workerVm.contact = contact.ToViewModel();
            }
            return new JsonResult(workerVm);
        }

        /// <summary>
        /// Update a worker
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorker([FromBody] ViewModels.Worker item, string id)
        {
            if (id != null && item.id != null && id != item.id)
                return BadRequest();

            var worker = await _dataverse.GetWorkerByIdAsync(id);
            if (worker == null)
                return new NotFoundResult();

            if (!CurrentUserHasAccessToContactWorkerApplicationOwnedBy(worker.adoxio_ContactId?.Id.ToString()))
                return NotFound("No access to worker");

            if (worker.statuscode != adoxio_worker_statuscode.NotSubmitted)
                return BadRequest("Applications with this status cannot be updated");

            var patchWorker = new adoxio_worker();
            patchWorker.Id = Guid.Parse(id);
            patchWorker.CopyValues(item);
            await _dataverse.UpdateWorkerAsync(patchWorker);

            var updated = await _dataverse.GetWorkerByIdAsync(id);
            var updatedVm = updated.ToViewModel();
            if (updated.adoxio_ContactId != null)
            {
                var contact = await _dataverse.GetContactByIdAsync(updated.adoxio_ContactId.Id.ToString());
                if (contact != null)
                    updatedVm.contact = contact.ToViewModel();
            }
            return new JsonResult(updatedVm);
        }

        /// <summary>
        /// Create a worker
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateWorker([FromBody] ViewModels.Worker item)
        {
            if (item?.contact?.id == null)
                return BadRequest();

            var worker = await CreateWorkerRecord(item);
            return new JsonResult(worker);
        }

        private async Task<ViewModels.Worker> CreateWorkerRecord(ViewModels.Worker item)
        {
            if (item?.contact?.id == null)
                throw new ArgumentNullException(nameof(item.contact.id));

            var worker = new adoxio_worker();
            worker.adoxio_IsManual = adoxio_generalyesno.No;
            worker.CopyValues(item);
            worker.adoxio_ContactId = new EntityReference(DvContact.EntityLogicalName, Guid.Parse(item.contact.id));

            try
            {
                var workerId = await _dataverse.CreateWorkerAsync(worker);
                worker.Id = workerId;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating worker.");
                throw;
            }

            var workerVm = worker.ToViewModel();
            workerVm.contact = item.contact;
            return workerVm;
        }

        /// <summary>
        /// Delete a Worker. Using HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteWorker(string id)
        {
            var worker = await _dataverse.GetWorkerByIdAsync(id);
            if (worker == null)
                return new NotFoundResult();

            if (!CurrentUserHasAccessToContactWorkerApplicationOwnedBy(worker.adoxio_ContactId?.Id.ToString()))
                return NotFound("No access to worker");

            try
            {
                await _dataverse.DeleteWorkerAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting worker.");
            }

            return NoContent();
        }

        /// GET a worker qualification letter as PDF.
        [HttpGet("{workerId}/pdf")]
        public async Task<IActionResult> GetLicencePDF(string workerId)
        {
            var adoxioWorker = await _dataverse.GetWorkerByIdAsync(workerId);
            if (adoxioWorker == null)
            {
                _logger.LogError($"Unable to send Worker Qualification Letter for worker {workerId} - unable to get worker record");
                throw new Exception("Error getting worker.");
            }

            if (!CurrentUserHasAccessToContactWorkerApplicationOwnedBy(adoxioWorker.adoxio_ContactId?.Id.ToString()))
            {
                _logger.LogError($"Unable to send Worker Qualification Letter for worker {workerId} - current user does not have access to worker");
                return NotFound("No access to worker");
            }

            var contactId = adoxioWorker.adoxio_ContactId?.Id.ToString();
            var contact = contactId != null ? await _dataverse.GetContactByIdAsync(contactId) : null;

            try
            {
                var dateOfBirthParam = "";
                if (adoxioWorker.adoxio_DateofBirth.HasValue)
                    dateOfBirthParam = adoxioWorker.adoxio_DateofBirth.Value.ToString("dd/MM/yyyy");

                var effectiveDateParam = "";
                if (adoxioWorker.adoxio_SecurityCompletedOn.HasValue)
                    effectiveDateParam = adoxioWorker.adoxio_SecurityCompletedOn.Value.ToString("dd/MM/yyyy");

                var expiryDateParam = "";
                if (adoxioWorker.adoxio_ExpiryDate.HasValue)
                    expiryDateParam = adoxioWorker.adoxio_ExpiryDate.Value.ToString("dd/MM/yyyy");

                var parameters = new Dictionary<string, string>
                {
                    { "title", "Worker_Qualification" },
                    { "currentDate", DateTime.Now.ToLongDateString() },
                    { "firstName", adoxioWorker.adoxio_FirstName },
                    { "middleName", adoxioWorker.adoxio_MiddleName },
                    { "lastName", adoxioWorker.adoxio_LastName },
                    { "dateOfBirth", dateOfBirthParam },
                    { "address", contact?.Address1_Line1 ?? "" },
                    { "city", contact?.Address1_City ?? "" },
                    { "province", contact?.Address1_StateOrProvince ?? "" },
                    { "postalCode", contact?.Address1_PostalCode ?? "" },
                    { "effectiveDate", effectiveDateParam },
                    { "expiryDate", expiryDateParam },
                    { "border", "{ \"top\": \"40px\", \"right\": \"40px\", \"bottom\": \"0px\", \"left\": \"40px\" }" }
                };

                byte[] data = await _pdfClient.GetPdf(parameters, "worker_qualification_letter");

                // Save copy of generated PDF for auditing purposes
                try
                {
                    var hash = await _pdfClient.GetPdfHash(parameters, "worker_qualification_letter");
                    var entityId = adoxioWorker.Id.ToString();
                    var docLocs = await _dataverse.GetSharePointDocLocsByObjectIdAsync(entityId);
                    var folderName = docLocs.FirstOrDefault(d => !string.IsNullOrEmpty(d.RelativeUrl))?.RelativeUrl
                        ?? $"{adoxioWorker.adoxio_name}_{entityId.Replace("-", "")}";
                    _fileManagerClient.UploadPdfIfChanged(_logger, "worker", entityId, folderName, "WorkerQualification", data, hash);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error uploading PDF");
                }

                _logger.LogInformation($"Sending Worker Qualification Letter for worker {workerId}");
                return File(data, "application/pdf", "WorkerQualificationLetter.pdf");
            }
            catch (Exception e)
            {
                string basePath = string.IsNullOrEmpty(_configuration["BASE_PATH"]) ? "" : _configuration["BASE_PATH"];
                basePath += "/worker-qualification/dashboard";
                _logger.LogError(e, $"Unable to send Worker Qualification Letter for worker {workerId}");
                return Redirect(basePath);
            }
        }

        private bool CurrentUserHasAccessToWorkerApplicationOwnedBy(string accountId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
                return userSettings.AccountId == accountId;
            return false;
        }

        private bool CurrentUserHasAccessToContactWorkerApplicationOwnedBy(string contactid)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            if (userSettings.ContactId != null && userSettings.ContactId.Length > 0)
                return userSettings.ContactId == contactid;
            return false;
        }
    }
}
