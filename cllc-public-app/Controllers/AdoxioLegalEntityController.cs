using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Client;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Gov.Lclb.Cllb.Interfaces.Models;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class AdoxioLegalEntityController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Interfaces.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly SharePointFileManager _sharePointFileManager;
        private readonly string _encryptionKey;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ILogger _logger;        

		public AdoxioLegalEntityController(Interfaces.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache, SharePointFileManager sharePointFileManager, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = null; // distributedCache;
            this._sharePointFileManager = sharePointFileManager;
            this._encryptionKey = Configuration["ENCRYPTION_KEY"];
            this._httpContextAccessor = httpContextAccessor;
            this._dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(AdoxioLegalEntityController));                    
        }

        /// <summary>
        /// Get all Dynamics Legal Entities
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsLegalEntities()
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            IEnumerable<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = null;
            String accountfilter = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // set account filter
            accountfilter = "_adoxio_account_value eq " + userSettings.AccountId;
			_logger.LogError("Account filter = " + accountfilter);

            legalEntities = _dynamicsClient.Adoxiolegalentities.Get(filter: accountfilter).Value;

            foreach (var legalEntity in legalEntities)
            {
                result.Add(legalEntity.ToViewModel());
            }

            return Json(result);
        }

        /// <summary>
        /// Get all Dynamics Legal Entities for the current Business Profile Summary
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("business-profile-summary")]
        public async Task<JsonResult> GetBusinessProfileSummary()
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            IEnumerable<Adoxio_legalentity> legalEntities = null;
            String accountfilter = null;
            String bpFilter = null;
            String filter = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // set account filter
            accountfilter = "_adoxio_account_value eq " + userSettings.AccountId;
            bpFilter = "and (adoxio_isapplicant eq true or adoxio_isindividual eq 0)";
            filter = accountfilter + " " + bpFilter;

            legalEntities = await _system.Adoxio_legalentities
                        .AddQueryOption("$filter", filter)
                        .ExecuteAsync();

            foreach (var legalEntity in legalEntities)
            {
                result.Add(legalEntity.ToViewModel());
            }

            return Json(result);
        }

        /// <summary>
        /// Get all Legal Entities where the position matches the parameter received
        /// By default, the account linked to the current user is used
        /// </summary>
        /// <param name="positionType"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("position/{positionType}")]
        public async Task<JsonResult> GetDynamicsLegalEntitiesByPosition(string positionType)
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            IEnumerable<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = null;
            String positionFilter = null;
            String accountfilter = null;
            String filter = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // set account filter
            accountfilter = "_adoxio_account_value eq " + userSettings.AccountId;
            filter = accountfilter;

            try
            {
                if (positionType == null)
                {
					_logger.LogError("Account filter = " + filter);
                    legalEntities = _dynamicsClient.Adoxiolegalentities.Get(filter: accountfilter).Value;

                }
                else
                {
                    positionFilter = Models.Adoxio_LegalEntityExtensions.GetPositionFilter(positionType);
					filter = accountfilter + " and " + positionFilter;                    

                    // Execute query if filter is valid
                    if (filter != null)
                    {
						_logger.LogError("Account filter = " + filter);
                        legalEntities = _dynamicsClient.Adoxiolegalentities.Get(filter: filter).Value;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }


            if (legalEntities != null)
            {
                foreach (var legalEntity in legalEntities)
                {
                    result.Add(legalEntity.ToViewModel());
                }
            }

            return Json(result);
        }

		/// <summary>
        /// Get the special applicant legal entity for the current user
        /// </summary>
        /// <returns></returns>
        [HttpGet("applicant")]
        public async Task<IActionResult> GetApplicantDynamicsLegalEntity()
        {
            ViewModels.AdoxioLegalEntity result = null;

			// get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // query the Dynamics system to get the legal entity record.
            MicrosoftDynamicsCRMadoxioLegalentity legalEntity = null;
            _logger.LogError("Find legal entity for applicant = " + userSettings.AccountId.ToString());

			legalEntity = await _dynamicsClient.GetAdoxioLegalentityByAccountId(Guid.Parse(userSettings.AccountId));
            
            if (legalEntity == null)            
            {
                return new NotFoundResult();
            }
            // fix the account.
            
            result = legalEntity.ToViewModel();
            
            if (result.account == null)
            {
                MicrosoftDynamicsCRMaccount account = await _dynamicsClient.GetAccountById(Guid.Parse(userSettings.AccountId));
                result.account = account.ToViewModel();
            }

            return Json(result);
        }

        /// <summary>
        /// Get a specific legal entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDynamicsLegalEntity(string id)
        {
            ViewModels.AdoxioLegalEntity result = null;
            // query the Dynamics system to get the legal entity record.
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
                MicrosoftDynamicsCRMadoxioLegalentity adoxioLegalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);
                if (adoxioLegalEntity == null)
                {
                    return new NotFoundResult();
                }                
                result = adoxioLegalEntity.ToViewModel();                
            }

            return Json(result);
        }

        // get a list of files associated with this legal entity.
        [HttpGet("{accountId}/attachments/{documentType}")]
        public async Task<IActionResult> GetFiles([FromRoute] string accountId, [FromRoute] string documentType)
        {
            List<ViewModels.FileSystemItem> result = new List<ViewModels.FileSystemItem>();
            // get the LegalEntity.
            Adoxio_legalentity legalEntity = null;

			// get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            if (accountId != null)
            {
                try
                {
                    var accountGUID = new Guid(accountId);
                    var account = await _system.Accounts.ByKey(accountid: accountGUID).GetValueAsync();
                    
                    var accountIdCleaned = account.Accountid.ToString().ToUpper().Replace("-", "");
                    string folderName = $"{account.Name}_{accountIdCleaned}";
                    // Get the folder contents for this Legal Entity.
                    List<MS.FileServices.FileSystemItem> items = await _sharePointFileManager.GetFilesInFolder("Accounts", folderName);
                    items = items.Where(i => i.Name.EndsWith(documentType)).ToList();
                    foreach (MS.FileServices.FileSystemItem item in items)
                    {
                        result.Add(item.ToViewModel());
                    }
                }
                catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                {
					_logger.LogError(dsqe.Message);
                    _logger.LogError(dsqe.StackTrace);
                    return new NotFoundResult();
                }
            }

            return Json(result);
        }

        [HttpPost("{accountId}/attachments")]
        public async Task<IActionResult> UploadFile([FromRoute] string accountId, [FromForm]IFormFile file, [FromForm] string documentType)
        {
            ViewModels.FileSystemItem result = null;
            // get the LegalEntity.
            // Adoxio_legalentity legalEntity = null;

			// get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

			if (accountId != null)
            {
                try
                {
                    var accountGUID = new Guid(accountId);
                    var account = await _system.Accounts.ByKey(accountid: accountGUID).GetValueAsync();

                    string fileName = FileSystemItemExtensions.CombineNameDocumentType(file.FileName, documentType);
                    var accountIdCleaned = account.Accountid.ToString().ToUpper().Replace("-", "");
                    string folderName = $"{account.Name}_{accountIdCleaned}";

                    await _sharePointFileManager.AddFile(folderName, fileName, file.OpenReadStream(), file.ContentType);
                }
                catch (Exception dsqe)
                {
					_logger.LogError(dsqe.Message);
					_logger.LogError(dsqe.StackTrace);
                    return new NotFoundResult();
                }
            }
            return Json(result);
        }

        [HttpGet("{id}/attachments/{fileId}")]
        public async Task<IActionResult> DownloadFile([FromRoute] string id, [FromRoute] string fileId)
        {
            // get the file.
            if (fileId == null)
            {
                return BadRequest();
            }
            else
            {
                _sharePointFileManager.GetFileById(fileId);
            }
            string filename = "";
            byte[] fileContents = new byte[10];
            return new FileContentResult(fileContents, "application/octet-stream")
            {
                FileDownloadName = filename
            };
        }

        /// <summary>
        /// Create a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateDynamicsLegalEntity([FromBody] ViewModels.AdoxioLegalEntity item)
        {

            // create a new legal entity.
            MicrosoftDynamicsCRMadoxioLegalentity adoxioLegalEntity = new MicrosoftDynamicsCRMadoxioLegalentity();


			// get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
			var userAccount = await _dynamicsClient.GetAccountById(Guid.Parse(userSettings.AccountId));

            // copy received values to Dynamics LegalEntity
            adoxioLegalEntity.CopyValues(item);
			adoxioLegalEntity.AdoxioAccountValueODataBind = _dynamicsClient.GetEntityURI("accounts", userAccount.Accountid);

            // TODO take the default for now from the parent account's legal entity record
            // TODO likely will have to re-visit for shareholders that are corporations/organizations
            MicrosoftDynamicsCRMadoxioLegalentity tempLegalEntity = await _dynamicsClient.GetAdoxioLegalentityByAccountId(Guid.Parse(userSettings.AccountId));
            adoxioLegalEntity.AdoxioLegalEntityOwnedODataBind = _dynamicsClient.GetEntityURI("adoxio_legalentities", tempLegalEntity.AdoxioLegalentityid);
            // create the record.

            adoxioLegalEntity = await _dynamicsClient.Adoxiolegalentities.CreateAsync(adoxioLegalEntity);
            
            return Json(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Update a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDynamicsLegalEntity([FromBody] ViewModels.AdoxioLegalEntity item, string id)
        {
            if (id != item.id)
            {
                return BadRequest();
            }

            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);

            MicrosoftDynamicsCRMadoxioLegalentity adoxioLegalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);
            if (adoxioLegalEntity == null)
            {
                return new NotFoundResult();
            }

            // we are doing a patch, so wipe out the record.
            adoxioLegalEntity = new MicrosoftDynamicsCRMadoxioLegalentity();

            // copy values over from the data provided
            adoxioLegalEntity.CopyValues(item);

            await _dynamicsClient.Adoxiolegalentities.UpdateAsync(adoxio_legalentityid.ToString(), adoxioLegalEntity);
            return Json(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Delete a legal entity.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteDynamicsLegalEntity(string id)
        {
            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            MicrosoftDynamicsCRMadoxioLegalentity legalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);
            if (legalEntity == null)
            {
                return new NotFoundResult();
            }

            await _dynamicsClient.Adoxiolegalentities.DeleteAsync(adoxio_legalentityid.ToString());                
            
            return NoContent(); // 204
        }
        /// <summary>
        /// Generate a link to be sent to an email address.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="individualId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private string GetConsentLink(string email, string individualId, string parentId)
        {
            string result = Configuration["BASE_URI"] + Configuration["BASE_PATH"];

            result += "/bcservice?path=/security-consent/" + parentId + "/" + individualId + "?code=";

            // create a newsletter confirmation object.

            ViewModels.SecurityConsentConfirmation securityConsentConfirmation = new ViewModels.SecurityConsentConfirmation()
            {
                email = email,
                parentid = parentId,
                individualid = individualId
            };

            // convert it to a json string.
            string json = JsonConvert.SerializeObject(securityConsentConfirmation);

            // encrypt that using two way encryption.

            result += System.Net.WebUtility.UrlEncode(EncryptionUtility.EncryptString(json, _encryptionKey));

            return result;
        }

        [HttpGet("{id}/verifyconsentcode/{individualid}")]
        public JsonResult VerifyConsentCode(string id, string individualid, string code)
        {
            string result = "Error";
            // validate the code.

            string decrypted = EncryptionUtility.DecryptString(code, _encryptionKey);
            if (decrypted != null)
            {
                // convert the json back to an object.
                ViewModels.SecurityConsentConfirmation consentConfirmation = JsonConvert.DeserializeObject<ViewModels.SecurityConsentConfirmation>(decrypted);
                // check that the keys match.
                if (id.Equals(consentConfirmation.parentid) && individualid.Equals(consentConfirmation.individualid))
                {
                    // update the appropriate dynamics record here.
                    result = "Success";
                }
            }
            return Json(result);
        }


        /// <summary>
        /// send consent requests to the supplied list of legal entities.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost("{id}/sendconsentrequests")]
        public async Task<IActionResult> SendConsentRequests(string id, [FromBody] List<string> recipientIds)
        {
            // start by getting the record for the current legal entity.
			// get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            var userAccount = await _dynamicsClient.GetAccountById(Guid.Parse(userSettings.AccountId));
			MicrosoftDynamicsCRMadoxioLegalentity userLegalentity = await _dynamicsClient.GetAdoxioLegalentityByAccountId(Guid.Parse(userSettings.AccountId));

            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            try
            {
                // TODO verify that this is the current user's legal entity
                Adoxio_legalentity adoxioLegalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid).GetValueAsync();

                // now get each of the supplied ids and send an email to them.

                foreach (string recipientId in recipientIds)
                {
                    Guid recipientIdGuid = new Guid(recipientId);
                    try
                    {
                        // TODO verify that each recipient is part of the current user's user set
                        // TODO switch over to new AuthAPI framework
                        Adoxio_legalentity recipientEntity = await _system.Adoxio_legalentities.ByKey(recipientIdGuid).GetValueAsync();
                        string email = recipientEntity.Adoxio_email;
                        string firstname = recipientEntity.Adoxio_firstname;
                        string lastname = recipientEntity.Adoxio_lastname;

                        string confirmationEmailLink = GetConsentLink(email, recipientId, id);
                        string bclogo = Configuration["BASE_URI"] + Configuration["BASE_PATH"] + "/assets/bc-logo.svg";
						/* send the user an email confirmation. */
						string body =
							  "<img src='" + bclogo + "'/><br><h2>Security Screening and Financial Integrity Checks</h2>"
							+ "<p>"
							+ "Dear " + firstname + " " + lastname + ","
							+ "</p>"
							+ "<p>"
							+ "An application from " + "[TBD Company Name]"
							+ " has been submitted for a non-medical retail cannabis licence in British Columbia. "
							+ "As a " + "[TBD Position]" + " of " + "[TBD Company Name]"
							+ " you are required to authorize a security screening — including criminal and police record checks—"
							+ "and financial integrity checks as part of the application process. "
							+ "</p>"
							+ "<p>"
							+ "Where you reside will determine how you are able to authorize the security screening."
							+ "</p>"
							+ "<p><strong>B.C. Residents</strong></p>"
							+ "<p>"
							+ "Residents of B.C. require a Photo B.C. Services Card to login to the application.  A Services Card "
							+ "verifies your identity, and has enhanced levels of security making the card more secure and helps protect your privacy."
							+ "</p>"
							+ "<p>"
							+ "If you don’t have a B.C. Services Card, or haven’t activated it for online login, visit the B.C. Services Card website to find how to get a card."
							+ "</p>"
							+ "<p>"
							+ "After you receive your verified Photo B.C. Services Card, login through this unique link:"
							+ "</p>"
							+ "<p><a href='" + confirmationEmailLink + "'>" + confirmationEmailLink + "</a></p>"
							+ "<p><strong>Out of Province Residents</strong></p>"
							+ "<p>TBD</p>"
							+ "<p><strong>Residents Outside of Canada</strong></p>"
							+ "<p>TBD</p>"
							+ "<p>If you have any questions about the security authorization, contact helpdesk@lclbc.ca</p>"
							+ "<p>Do not reply to this email address</p>";

                        // send the email.
                        SmtpClient client = new SmtpClient(Configuration["SMTP_HOST"]);

                        // Specify the message content.
                        MailMessage message = new MailMessage("no-reply@gov.bc.ca", email);
                        message.Subject = "BC LCLB Cannabis Licensing Security Consent";
                        message.Body = body;
                        message.IsBodyHtml = true;
                        client.Send(message);

						// save the consent link and the fact that the email has been sent
						recipientEntity.Adoxio_dateemailsent = DateTime.Now;
						//await _dynamicsClient.Adoxiolegalentities.UpdateAsync(recipientEntity.Adoxio_legalentityid, recipientEntity);
                    }
                    catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                    {
						// ignore any not found errors.
						_logger.LogError(dsqe.Message);
						_logger.LogError(dsqe.StackTrace);
                    }

                }

            }
            catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
            {
                return new NotFoundResult();
            }

            return NoContent(); // 204
        }
    }
}
