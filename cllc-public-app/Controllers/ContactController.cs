extern alias DV;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Extensions;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Serilog;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using User = Gov.Lclb.Cllb.Public.Models.User;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DvContact = DV::Gov.Lclb.Cllb.Interfaces.Contact;
using adoxio_alias = DV::Gov.Lclb.Cllb.Interfaces.adoxio_alias;
using adoxio_worker = DV::Gov.Lclb.Cllb.Interfaces.adoxio_worker;
using adoxio_generalyesno = DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno;
using contact_customertypecode = DV::Gov.Lclb.Cllb.Interfaces.contact_customertypecode;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly IDataverseClient _dataverse;
        private readonly string _encryptionKey;
        private readonly IWebHostEnvironment _env;
        private readonly FileManagerClient _fileManagerClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public ContactController(IConfiguration configuration, IDataverseClient dataverse,
            IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IWebHostEnvironment env,
            FileManagerClient fileManagerClient)
        {
            _configuration = configuration;
            _dataverse = dataverse;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(ContactController));
            _env = env;
            _encryptionKey = _configuration["ENCRYPTION_KEY"];
            _fileManagerClient = fileManagerClient;
        }



        /// <summary>
        ///     Get a specific legal entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContact(string id)
        {
            ViewModels.Contact result = null;

            if (!string.IsNullOrEmpty(id))
            {
                // query Dataverse to get the contact record.
                var contact = await _dataverse.GetContactByIdAsync(id);

                if (contact != null)
                    result = contact.ToViewModel();
                else
                    return new NotFoundResult();
            }
            else
            {
                return BadRequest();
            }

            return new JsonResult(result);
        }


        /// <summary>
        ///     Update a contact
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact([FromBody] ViewModels.Contact item, string id)
        {
            if (id != null && item.id != null && id != item.id) return BadRequest();
            var accessGranted = false;

            // Allow access if the current user is the contact.
            if (DynamicsExtensions.CurrentUserIsContact(id, _httpContextAccessor))
            {
                accessGranted = true;
            }
            else
            {
                var contact = await _dataverse.GetContactByIdAsync(id);

                // get the related account and determine if the current user is allowed access
                if (contact?.ParentCustomerId != null)
                {
                    var accountId = contact.ParentCustomerId.Id;
                    accessGranted = await DynamicsExtensions.CurrentUserHasAccessToAccountAsync(
                        accountId, _httpContextAccessor, _dataverse);
                }
            }

            if (!accessGranted)
            {
                _logger.LogError(LoggingEvents.BadRequest, $"Current user has NO access to the contact record. Aborting update to contact {id} ");
                return NotFound();
            }

            var patchContact = new DvContact();
            patchContact.ContactId = new Guid(id);
            patchContact.CopyValues(item);
            try
            {
                await _dataverse.UpdateContactAsync(patchContact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact");
            }

            var result = await _dataverse.GetContactByIdAsync(id);
            return new JsonResult(result.ToViewModel());
        }


        /// <summary>
        ///     Update a contact using PHS or CASS token
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("security-screening/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateContactByToken([FromBody] ViewModels.Contact item, string token)
        {
            if (token == null || item == null) return BadRequest();

            // get the contact
            var contactId = EncryptionUtility.DecryptStringHex(token, _encryptionKey);
            var contact = await _dataverse.GetContactByIdAsync(contactId);
            if (contact == null) return new NotFoundResult();

            var patchContact = new DvContact();
            patchContact.ContactId = new Guid(contactId);
            patchContact.CopyValues(item);
            try
            {
                await _dataverse.UpdateContactAsync(patchContact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact");
            }

            foreach (var alias in item.Aliases) CreateAlias(alias, contactId);

            contact = await _dataverse.GetContactByIdAsync(contactId);
            return new JsonResult(contact.ToViewModel());
        }

        private async Task<IActionResult> CreateAlias(ViewModels.Alias item, string contactId)
        {
            if (item == null || string.IsNullOrEmpty(contactId)) return BadRequest();

            var alias = new adoxio_alias();
            alias.CopyValues(item, contactId);
            try
            {
                await _dataverse.CreateAliasAsync(alias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating alias");
                throw;
            }

            return new JsonResult(alias.ToViewModel());
        }

        /// <summary>
        ///     Create a contact
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] ViewModels.Contact item)
        {
            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            // first check to see that a contact exists.
            var contactSiteminderGuid = userSettings.SiteMinderGuid;
            if (contactSiteminderGuid == null || contactSiteminderGuid.Length == 0)
            {
                _logger.LogDebug(LoggingEvents.Error, "No Contact Siteminder Guid exernal id");
                throw new Exception("Error. No ContactSiteminderGuid exernal id");
            }

            // see if the contact exists.
            try
            {
                var externalId = DynamicsExtensions.GetServiceCardID(contactSiteminderGuid);
                var userContact = await _dataverse.GetContactByExternalIdAsync(externalId);
                if (userContact != null) throw new Exception("Contact already Exists");
            }
            catch (Exception ex) when (ex.Message != "Contact already Exists")
            {
                _logger.LogError(ex, "Error getting contact by Siteminder Guid.");
                throw new Exception("Error getting contact by Siteminder Guid");
            }

            // create a new contact.
            var contact = new DvContact();
            contact.CopyValues(item);

            if (userSettings.IsNewUserRegistration)
                // get additional information from the service card headers.
                contact.CopyHeaderValues(_httpContextAccessor);

            contact.adoxio_ExternalID = DynamicsExtensions.GetServiceCardID(contactSiteminderGuid);
            Guid contactId;
            try
            {
                contactId = await _dataverse.CreateContactAsync(contact);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating contact.");
                throw;
            }

            var createdContact = await _dataverse.GetContactByIdAsync(contactId.ToString());

            // if we have not yet authenticated, then this is the new record for the user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.ContactId = contactId.ToString();

                // we can now authenticate.
                if (userSettings.AuthenticatedUser == null)
                {
                    var user = new User();
                    user.Active = true;
                    user.ContactId = contactId;
                    user.UserType = userSettings.UserType;
                    user.SmUserId = userSettings.UserId;
                    userSettings.AuthenticatedUser = user;
                }

                userSettings.IsNewUserRegistration = false;

                var userSettingsString = JsonConvert.SerializeObject(userSettings);
                _logger.LogDebug("userSettingsString --> " + userSettingsString);

                // add the user to the session.
                _httpContextAccessor.HttpContext.Session.SetString("UserSettings", userSettingsString);
                _logger.LogDebug("user added to session. ");
            }
            else
            {
                _logger.LogDebug(LoggingEvents.Error, "Invalid user registration.");
                throw new Exception("Invalid user registration.");
            }

            return new JsonResult(createdContact.ToViewModel());
        }

        /// <summary>
        ///     Create a contact (worker registration flow)
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("worker")]
        public async Task<IActionResult> CreateWorkerContact([FromBody] ViewModels.Contact item)
        {
            // get the current user.
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            // first check to see that we have the correct inputs.
            var contactSiteminderGuid = userSettings.SiteMinderGuid;
            if (contactSiteminderGuid == null || contactSiteminderGuid.Length == 0)
            {
                _logger.LogDebug(LoggingEvents.Error, "No Contact Siteminder Guid exernal id");
                throw new Exception("Error. No ContactSiteminderGuid exernal id");
            }

            // see if the contact exists.
            try
            {
                var externalId = DynamicsExtensions.GetServiceCardID(contactSiteminderGuid);
                var userContact = await _dataverse.GetContactByExternalIdAsync(externalId);
                if (userContact != null) throw new Exception("Contact already Exists");
            }
            catch (Exception ex) when (ex.Message != "Contact already Exists")
            {
                _logger.LogError(ex, "Error getting contact by Siteminder Guid.");
                throw new Exception("Error getting contact by Siteminder Guid");
            }

            // create a new contact and worker.
            var contact = new DvContact();
            var worker = new adoxio_worker
            {
                adoxio_FirstName = item.firstname,
                adoxio_MiddleName = item.middlename,
                adoxio_LastName = item.lastname,
                adoxio_IsManual = adoxio_generalyesno.No
            };

            contact.CopyValues(item);
            // set the type to Retail Worker.
            contact.CustomerTypeCode = (contact_customertypecode)845280000;

            if (userSettings.NewWorker != null)
            {
                // get additional information from the service card headers.
                contact.CopyContactUserSettings(userSettings.NewContact);
                worker.CopyValues(userSettings.NewWorker);
            }

            //Default the country to Canada
            if (string.IsNullOrEmpty(contact.Address1_Country)) contact.Address1_Country = "Canada";
            if (string.IsNullOrEmpty(contact.Address2_Country)) contact.Address2_Country = "Canada";

            contact.adoxio_ExternalID = DynamicsExtensions.GetServiceCardID(contactSiteminderGuid);

            Guid contactId;
            try
            {
                // Create contact first, then link worker to it
                contactId = await _dataverse.CreateContactAsync(contact);
                worker.adoxio_ContactId = new EntityReference(DvContact.EntityLogicalName, contactId);
                var workerId = await _dataverse.CreateWorkerAsync(worker);
                worker.Id = workerId;
                await CreateSharepointDynamicsLink(worker, workerId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating worker contact");
                _logger.LogError(ex.Message);
                throw;
            }

            contact = await _dataverse.GetContactByIdAsync(contactId.ToString());

            // if we have not yet authenticated, then this is the new record for the user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.ContactId = contactId.ToString();

                // we can now authenticate.
                if (userSettings.AuthenticatedUser == null)
                {
                    var user = new User();
                    user.Active = true;
                    user.ContactId = contactId;
                    user.UserType = userSettings.UserType;
                    user.SmUserId = userSettings.UserId;
                    userSettings.AuthenticatedUser = user;
                }

                userSettings.IsNewUserRegistration = false;

                var userSettingsString = JsonConvert.SerializeObject(userSettings);
                _logger.LogDebug("userSettingsString --> " + userSettingsString);

                // add the user to the session.
                _httpContextAccessor.HttpContext.Session.SetString("UserSettings", userSettingsString);
                _logger.LogDebug("user added to session. ");
            }
            else
            {
                _logger.LogDebug(LoggingEvents.Error, "Invalid user registration.");
                throw new Exception("Invalid user registration.");
            }

            return new JsonResult(contact.ToViewModel());
        }


        private async Task CreateSharepointDynamicsLink(adoxio_worker worker, string workerId)
        {
            var workerIdCleaned = workerId.ToUpper().Replace("-", "");
            var workerName = worker.adoxio_name ?? string.Empty;
            var folderName = $"{workerName}_{workerIdCleaned}";

            _fileManagerClient.CreateFolderIfNotExist(_logger, SharePointConstants.WorkerFolderInternalName, folderName);
            await _dataverse.CreateWorkerSharePointDocLocAsync(workerId, folderName);
        }


        [HttpGet("cass-link/{contactId}")]
        public JsonResult GetCASLinkForContactGuid(string contactId)
        {
            string casLink = null;
            try
            {
                casLink = GetCASSLink(contactId, _configuration, _encryptionKey);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting cannabis screening link");
                _logger.LogError("Details:");
                _logger.LogError(ex.Message);
            }

            return new JsonResult(casLink);
        }

        public static string GetCASSLink(string contactId, IConfiguration _configuration, string _encryptionKey)
        {
            var result = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/cannabis-associate-screening/";
            result += HttpUtility.UrlEncode(EncryptionUtility.EncryptStringHex(contactId, _encryptionKey));
            return result;
        }

        [HttpGet("phs-link/{contactId}")]
        public JsonResult GetPhsLinkForContactGuid(string contactId)
        {
            string phsLink = null;
            try
            {
                phsLink = DynamicsExtensions.GetPhsLink(contactId, _configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting personal history link");
            }

            return new JsonResult(phsLink);
        }

        public static string GetPhsLink(string contactId, IConfiguration _configuration, string encryptionKey)
        {
            var result = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/personal-history-summary/";
            result += HttpUtility.UrlEncode(EncryptionUtility.EncryptStringHex(contactId, encryptionKey));
            return result;
        }

        [HttpGet("phs/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetContactByToken(string code)
        {
            var id = EncryptionUtility.DecryptStringHex(code, _encryptionKey);
            if (!string.IsNullOrEmpty(id))
            {
                // query Dataverse to get the contact record.
                var contact = await _dataverse.GetContactByIdAsync(id);

                if (contact != null)
                {
                    var result = new PHSContact
                    {
                        Id = contact.ContactId?.ToString(),
                        token = code,
                        shortName = contact.FirstName.First() + " " + contact.LastName,
                        isComplete = (int?)contact.adoxio_PHSComplete == (int)ViewModels.YesNoOptions.Yes
                    };
                    return new JsonResult(result);
                }

                return new NotFoundResult();
            }

            return BadRequest();
        }

        [HttpGet("cass/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCASSContactByToken(string code)
        {
            var id = EncryptionUtility.DecryptStringHex(code, _encryptionKey);
            if (!string.IsNullOrEmpty(id))
            {
                DvContact userContact = null;
                try
                {
                    var userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
                    userContact = await _dataverse.GetContactByIdAsync(userSettings.ContactId);
                }
                catch (ArgumentNullException)
                {
                    // anonymous
                }

                // query Dataverse to get the contact record.
                var contact = await _dataverse.GetContactByIdAsync(id);

                if (userContact == null)
                    return new JsonResult(new CASSPublicContact
                    {
                        Id = contact.ContactId?.ToString(),
                        token = code,
                        shortName = contact.FirstName.First() + " " + contact.LastName,
                        IsWrongUser = false
                    });

                if (contact != null
                    && userContact.FirstName != null &&
                    contact.FirstName.StartsWith(userContact.FirstName.Substring(0, 1), true,
                        CultureInfo.CurrentCulture)
                    && userContact.LastName != null && userContact.LastName.ToLower() == contact.LastName.ToLower()
                    && userContact.BirthDate != null && userContact.BirthDate.Value.Date.ToShortDateString() ==
                    contact.BirthDate.Value.Date.ToShortDateString()
                )
                    return new JsonResult(new CASSPrivateContact
                    {
                        Id = contact.ContactId?.ToString(),
                        token = code,
                        shortName = contact.FirstName + " " + contact.LastName,
                        dateOfBirth = contact.adoxio_DateofBirthShortDateString,
                        gender = ((ViewModels.Gender?)(int?)contact.adoxio_GenderCode).ToString(),
                        streetAddress = contact.Address1_Line1,
                        city = contact.Address1_City,
                        province = contact.Address1_StateOrProvince,
                        postalCode = contact.Address1_PostalCode,
                        country = contact.Address1_Country
                    });
                return new JsonResult(new CASSPublicContact
                {
                    Id = contact.ContactId?.ToString(),
                    token = code,
                    shortName = contact.FirstName.First() + " " + contact.LastName,
                    IsWrongUser = true
                });
            }

            return BadRequest();
        }
    }

    public class ScreeningContact
    {
        public string Id { get; set; }
        public string token { get; set; }
        public string shortName { get; set; }
    }

    public class PHSContact : ScreeningContact
    {
        public bool isComplete { get; set; }
    }

    public class CASSPublicContact : ScreeningContact
    {
        public bool IsWrongUser;
    }

    public class CASSPrivateContact : CASSPublicContact
    {
        public string dateOfBirth { get; set; }
        public string gender { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string email { get; set; }
    }
}
