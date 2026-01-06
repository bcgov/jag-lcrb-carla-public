using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
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
using Microsoft.Rest;
using Newtonsoft.Json;
using Serilog;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using User = Gov.Lclb.Cllb.Public.Models.User;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly IDynamicsClient _dynamicsClient;
        private readonly string _encryptionKey;
        private readonly IWebHostEnvironment _env;
        private readonly FileManagerClient _fileManagerClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public ContactController(IConfiguration configuration, IDynamicsClient dynamicsClient,
            IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IWebHostEnvironment env,
            FileManagerClient fileManagerClient)
        {
            _configuration = configuration;
            _dynamicsClient = dynamicsClient;
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
                var contactId = Guid.Parse(id);
                // query the Dynamics system to get the contact record.
                var contact = await _dynamicsClient.GetContactById(contactId);

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

            // get the contact



            // Allow access if the current user is the contact - for scenarios such as a worker update.
            if (DynamicsExtensions.CurrentUserIsContact(id, _httpContextAccessor))
            {
                accessGranted = true;
            }
            else
            {
                var contact = await _dynamicsClient.GetContactById(id);

                // get the related account and determine if the current user is allowed access
                if (!string.IsNullOrEmpty(contact?._parentcustomeridValue))
                {
                    var accountId = Guid.Parse(contact._parentcustomeridValue);
                    accessGranted =
                        DynamicsExtensions.CurrentUserHasAccessToAccount(accountId, _httpContextAccessor,
                            _dynamicsClient);
                }
            }

            if (!accessGranted)
            {
                _logger.LogError(LoggingEvents.BadRequest, $"Current user has NO access to the contact record. Aborting update to contact {id} ");
                return NotFound();
            }

            var patchContact = new MicrosoftDynamicsCRMcontact();
            patchContact.CopyValues(item);
            try
            {
                await _dynamicsClient.Contacts.UpdateAsync(id, patchContact);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating contact");
            }

            var result = await _dynamicsClient.GetContactById(id);
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
            var contactGuid = Guid.Parse(contactId);

            var contact = await _dynamicsClient.GetContactById(contactGuid);
            if (contact == null) return new NotFoundResult();
            var patchContact = new MicrosoftDynamicsCRMcontact();
            patchContact.CopyValues(item);
            try
            {
                await _dynamicsClient.Contacts.UpdateAsync(contactGuid.ToString(), patchContact);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating contact");
            }

            foreach (var alias in item.Aliases) CreateAlias(alias, contactId);

            contact = await _dynamicsClient.GetContactById(contactGuid);
            return new JsonResult(contact.ToViewModel());
        }

        private async Task<IActionResult> CreateAlias(ViewModels.Alias item, string contactId)
        {
            if (item == null || string.IsNullOrEmpty(contactId)) return BadRequest();


            var alias = new MicrosoftDynamicsCRMadoxioAlias();
            // copy received values to Dynamics Application
            alias.CopyValues(item);
            try
            {
                alias = _dynamicsClient.Aliases.Create(alias);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating application");
                // fail if we can't create.
                throw;
            }


            var patchAlias = new MicrosoftDynamicsCRMadoxioAlias();

            // set contact association
            try
            {
                patchAlias.ContactIdODataBind = _dynamicsClient.GetEntityURI("contacts", contactId);

                await _dynamicsClient.Aliases.UpdateAsync(alias.AdoxioAliasid, patchAlias);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating application");
                // fail if we can't create.
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

            // get the contact record.
            MicrosoftDynamicsCRMcontact userContact = null;

            // see if the contact exists.
            try
            {
                userContact = _dynamicsClient.GetActiveContactByExternalId(contactSiteminderGuid);
                if (userContact != null) throw new Exception("Contact already Exists");
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting contact by Siteminder Guid.");
                throw new HttpOperationException("Error getting contact by Siteminder Guid");
            }

            // create a new contact.
            var contact = new MicrosoftDynamicsCRMcontact();
            contact.CopyValues(item);


            if (userSettings.IsNewUserRegistration)
                // get additional information from the service card headers.
                contact.CopyHeaderValues(_httpContextAccessor);

            contact.AdoxioExternalid = DynamicsExtensions.GetServiceCardID(contactSiteminderGuid);
            try
            {
                contact = await _dynamicsClient.Contacts.CreateAsync(contact);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating contact. ");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown error creating contact.");
            }

            // if we have not yet authenticated, then this is the new record for the user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.ContactId = contact.Contactid;

                // we can now authenticate.
                if (userSettings.AuthenticatedUser == null)
                {
                    var user = new User();
                    user.Active = true;
                    user.ContactId = Guid.Parse(userSettings.ContactId);
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

        /// <summary>
        ///     Create a contact
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

            // get the contact record.
            MicrosoftDynamicsCRMcontact userContact = null;

            // see if the contact exists.
            try
            {
                userContact = _dynamicsClient.GetActiveContactByExternalId(contactSiteminderGuid);
                if (userContact != null) throw new Exception("Contact already Exists");
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting contact by Siteminder Guid.");
                throw new HttpOperationException("Error getting contact by Siteminder Guid");
            }

            // create a new contact.
            var contact = new MicrosoftDynamicsCRMcontact();
            var worker = new MicrosoftDynamicsCRMadoxioWorker
            {
                AdoxioFirstname = item.firstname,
                AdoxioMiddlename = item.middlename,
                AdoxioLastname = item.lastname,
                AdoxioIsmanual = 0 // 0 for false - is a portal user.
            };


            contact.CopyValues(item);
            // set the type to Retail Worker.
            contact.Customertypecode = 845280000;

            if (userSettings.NewWorker != null)
            {
                // get additional information from the service card headers.
                contact.CopyContactUserSettings(userSettings.NewContact);
                worker.CopyValues(userSettings.NewWorker);
            }

            //Default the country to Canada
            if (string.IsNullOrEmpty(contact.Address1Country)) contact.Address1Country = "Canada";
            if (string.IsNullOrEmpty(contact.Address2Country)) contact.Address2Country = "Canada";


            contact.AdoxioExternalid = DynamicsExtensions.GetServiceCardID(contactSiteminderGuid);

            try
            {
                worker.AdoxioContactId = contact;

                worker = await _dynamicsClient.Workers.CreateAsync(worker);
                contact = await _dynamicsClient.GetContactById(Guid.Parse(worker._adoxioContactidValue));
                await CreateSharepointDynamicsLink(worker);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating contact");
                _logger.LogError(httpOperationException.Response.Content);

                //fail
                throw httpOperationException;
            }


            // if we have not yet authenticated, then this is the new record for the user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.ContactId = contact.Contactid;

                // we can now authenticate.
                if (userSettings.AuthenticatedUser == null)
                {
                    var user = new User();
                    user.Active = true;
                    user.ContactId = Guid.Parse(userSettings.ContactId);
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


        private async Task CreateSharepointDynamicsLink(MicrosoftDynamicsCRMadoxioWorker worker)
        {
            // create a SharePointDocumentLocation link
            var folderName = worker.GetDocumentFolderName();
            //var name = worker.AdoxioWorkerid + " Files";

            _fileManagerClient.CreateFolderIfNotExist(_logger, SharePointConstants.WorkerDocumentUrlTitle, folderName);

            _dynamicsClient.CreateEntitySharePointDocumentLocation("worker", worker.AdoxioWorkerid, folderName, folderName);
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
                _logger.LogError("Error getting cannabis associate screening link");
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
                var contactId = Guid.Parse(id);
                // query the Dynamics system to get the contact record.
                var contact = await _dynamicsClient.GetContactById(contactId);

                if (contact != null)
                {
                    var result = new PHSContact
                    {
                        Id = contact.Contactid,
                        token = code,
                        shortName = contact.Firstname.First() + " " + contact.Lastname,
                        isComplete = contact.AdoxioPhscomplete == (int)ViewModels.YesNoOptions.Yes
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
                MicrosoftDynamicsCRMcontact userContact = null;
                try
                {
                    var userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
                    userContact = await _dynamicsClient.GetContactById(userSettings.ContactId);
                }
                catch (ArgumentNullException)
                {
                    // anonymous
                }

                var contactId = Guid.Parse(id);
                // query the Dynamics system to get the contact record.
                var contact = await _dynamicsClient.GetContactById(contactId);

                if (userContact == null)
                    return new JsonResult(new CASSPublicContact
                    {
                        Id = contact.Contactid,
                        token = code,
                        shortName = contact.Firstname.First() + " " + contact.Lastname,
                        IsWrongUser = false
                    });

                if (contact != null
                    && userContact.Firstname != null &&
                    contact.Firstname.StartsWith(userContact.Firstname.Substring(0, 1), true,
                        CultureInfo.CurrentCulture)
                    && userContact.Lastname != null && userContact.Lastname.ToLower() == contact.Lastname.ToLower()
                    && userContact.Birthdate != null && userContact.Birthdate.Value.Date.ToShortDateString() ==
                    contact.Birthdate.Value.Date.ToShortDateString()
                )
                    return new JsonResult(new CASSPrivateContact
                    {
                        Id = contact.Contactid,
                        token = code,
                        shortName = contact.Firstname + " " + contact.Lastname,
                        dateOfBirth = contact.AdoxioDateofbirthshortdatestring,
                        gender = ((ViewModels.Gender?)contact.AdoxioGendercode).ToString(),
                        streetAddress = contact.Address1Line1,
                        city = contact.Address1City,
                        province = contact.Address1Stateorprovince,
                        postalCode = contact.Address1Postalcode,
                        country = contact.Address1Country
                    });
                return new JsonResult(new CASSPublicContact
                {
                    Id = contact.Contactid,
                    token = code,
                    shortName = contact.Firstname.First() + " " + contact.Lastname,
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