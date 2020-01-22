using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Gov.Lclb.Cllb.Public.Utility;
using System.Net.Mail;
using System.Net;
using Google.Protobuf;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _encryptionKey;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;
        private readonly FileManagerClient _fileManagerClient;

        public ContactController(IConfiguration configuration, IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IWebHostEnvironment env, FileManagerClient fileManagerClient)
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
        /// Get a specific legal entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContact(string id)
        {
            ViewModels.Contact result = null;

            if (!string.IsNullOrEmpty(id))
            {
                Guid contactId = Guid.Parse(id);
                // query the Dynamics system to get the contact record.
                MicrosoftDynamicsCRMcontact contact = await _dynamicsClient.GetContactById(contactId);

                if (contact != null)
                {
                    result = contact.ToViewModel();
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

            return new JsonResult(result);
        }


        /// <summary>
        /// Update a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact([FromBody] ViewModels.Contact item, string id)
        {
            if (id != null && item.id != null && id != item.id)
            {
                return BadRequest();
            }

            // get the contact
            Guid contactId = Guid.Parse(id);

            MicrosoftDynamicsCRMcontact contact = await _dynamicsClient.GetContactById(contactId);
            if (contact == null)
            {
                return new NotFoundResult();
            }
            MicrosoftDynamicsCRMcontact patchContact = new MicrosoftDynamicsCRMcontact();
            patchContact.CopyValues(item);
            try
            {
                await _dynamicsClient.Contacts.UpdateAsync(contactId.ToString(), patchContact);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError("Error updating contact");
                _logger.LogError("Request:");
                _logger.LogError(httpOperationException.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(httpOperationException.Response.Content);
            }

            contact = await _dynamicsClient.GetContactById(contactId);
            return new JsonResult(contact.ToViewModel());
        }

        /// <summary>
        /// Create a contact
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateContact([FromBody] ViewModels.Contact item)
        {

            // get UserSettings from the session
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // first check to see that a contact exists.
            string contactSiteminderGuid = userSettings.SiteMinderGuid;
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
                userContact = _dynamicsClient.GetContactByExternalId(contactSiteminderGuid);
                if (userContact != null)
                {
                    throw new Exception("Contact already Exists");
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(LoggingEvents.Error, "Error getting contact by Siteminder Guid.");
                _logger.LogError("Request:");
                _logger.LogError(httpOperationException.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(httpOperationException.Response.Content);
                throw new HttpOperationException("Error getting contact by Siteminder Guid");
            }

            // create a new contact.
            MicrosoftDynamicsCRMcontact contact = new MicrosoftDynamicsCRMcontact();
            contact.CopyValues(item);


            if (userSettings.IsNewUserRegistration)
            {
                // get additional information from the service card headers.
                contact.CopyHeaderValues(_httpContextAccessor);
            }

            contact.AdoxioExternalid = DynamicsExtensions.GetServiceCardID(contactSiteminderGuid);
            try
            {
                contact = await _dynamicsClient.Contacts.CreateAsync(contact);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, $"Error creating contact. ");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unknown error creating contact.");
            }

            // if we have not yet authenticated, then this is the new record for the user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.ContactId = contact.Contactid.ToString();

                // we can now authenticate.
                if (userSettings.AuthenticatedUser == null)
                {
                    Models.User user = new Models.User();
                    user.Active = true;
                    user.ContactId = Guid.Parse(userSettings.ContactId);
                    user.UserType = userSettings.UserType;
                    user.SmUserId = userSettings.UserId;
                    userSettings.AuthenticatedUser = user;
                }

                userSettings.IsNewUserRegistration = false;

                string userSettingsString = JsonConvert.SerializeObject(userSettings);
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
        /// Create a contact
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("worker")]
        public async Task<IActionResult> CreateWorkerContact([FromBody] ViewModels.Contact item)
        {
            // get UserSettings from the session
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // first check to see that we have the correct inputs.
            string contactSiteminderGuid = userSettings.SiteMinderGuid;
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
                userContact = _dynamicsClient.GetContactByExternalId(contactSiteminderGuid);
                if (userContact != null)
                {
                    throw new Exception("Contact already Exists");
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(LoggingEvents.Error, "Error getting contact by Siteminder Guid.");
                _logger.LogError("Request:");
                _logger.LogError(httpOperationException.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(httpOperationException.Response.Content);
                throw new HttpOperationException("Error getting contact by Siteminder Guid");
            }

            // create a new contact.
            MicrosoftDynamicsCRMcontact contact = new MicrosoftDynamicsCRMcontact();
            MicrosoftDynamicsCRMadoxioWorker worker = new MicrosoftDynamicsCRMadoxioWorker()
            {
                AdoxioFirstname = item.firstname,
                AdoxioMiddlename = item.middlename,
                AdoxioLastname = item.lastname,
                AdoxioIsmanual = 0 // 0 for false - is a portal user.
            };


            contact.CopyValues(item);
            // set the type to Retail Worker.
            contact.Customertypecode = 845280000;

            if (userSettings.NewWorker != null && !_env.IsDevelopment())
            {
                // get additional information from the service card headers.
                contact.CopyContactUserSettings(userSettings.NewContact);
                worker.CopyValues(userSettings.NewWorker);
            }

            //Default the country to Canada
            if (string.IsNullOrEmpty(contact.Address1Country))
            {
                contact.Address1Country = "Canada";
            }
            if (string.IsNullOrEmpty(contact.Address2Country))
            {
                contact.Address2Country = "Canada";
            }


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
                _logger.LogError("Error updating contact");
                _logger.LogError("Request:");
                _logger.LogError(httpOperationException.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(httpOperationException.Response.Content);

                //fail
                throw httpOperationException;
            }


            // if we have not yet authenticated, then this is the new record for the user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.ContactId = contact.Contactid.ToString();

                // we can now authenticate.
                if (userSettings.AuthenticatedUser == null)
                {
                    Models.User user = new Models.User();
                    user.Active = true;
                    user.ContactId = Guid.Parse(userSettings.ContactId);
                    user.UserType = userSettings.UserType;
                    user.SmUserId = userSettings.UserId;
                    userSettings.AuthenticatedUser = user;
                }

                userSettings.IsNewUserRegistration = false;

                string userSettingsString = JsonConvert.SerializeObject(userSettings);
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
            string folderName = await FileController.GetFolderName("worker", worker.AdoxioWorkerid, _dynamicsClient);
            string name = worker.AdoxioWorkerid + " Files";



            _fileManagerClient.CreateFolderIfNotExist(_logger, WorkerDocumentUrlTitle, folderName);

            // Create the SharePointDocumentLocation entity
            MicrosoftDynamicsCRMsharepointdocumentlocation mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation()
            {
                Relativeurl = folderName,
                Description = "Worker Qualification",
                Name = name
            };


            try
            {
                mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError("Error creating SharepointDocumentLocation");
                _logger.LogError("Request:");
                _logger.LogError(httpOperationException.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(httpOperationException.Response.Content);
                mdcsdl = null;
            }
            if (mdcsdl != null)
            {
                // add a regardingobjectid.
                string workerReference = _dynamicsClient.GetEntityURI("adoxio_workers", worker.AdoxioWorkerid);
                var patchSharePointDocumentLocation = new MicrosoftDynamicsCRMsharepointdocumentlocation();
                patchSharePointDocumentLocation.RegardingobjectidWorkerApplicationODataBind = workerReference;
                // set the parent document library.
                string parentDocumentLibraryReference = GetDocumentLocationReferenceByRelativeURL(WorkerDocumentUrlTitle);
                patchSharePointDocumentLocation.ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference);

                try
                {
                    _dynamicsClient.Sharepointdocumentlocations.Update(mdcsdl.Sharepointdocumentlocationid, patchSharePointDocumentLocation);
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.LogError("Error adding reference SharepointDocumentLocation to application");
                    _logger.LogError("Request:");
                    _logger.LogError(httpOperationException.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(httpOperationException.Response.Content);
                    throw httpOperationException;
                }

                string sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", mdcsdl.Sharepointdocumentlocationid);
                // update the sharePointLocationData.
                Odataid oDataId = new Odataid()
                {
                    OdataidProperty = sharePointLocationData
                };
                try
                {
                    _dynamicsClient.Workers.AddReference(worker.AdoxioWorkerid, "adoxio_worker_SharePointDocumentLocations", oDataId);
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.LogError("Error adding reference to SharepointDocumentLocation");
                    _logger.LogError("Request:");
                    _logger.LogError(httpOperationException.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(httpOperationException.Response.Content);
                    throw httpOperationException;
                }
            }
        }


        /// <summary>
        /// Get a document location by reference
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        private string GetDocumentLocationReferenceByRelativeURL(string relativeUrl)
        {
            string result = null;
            string sanitized = relativeUrl.Replace("'", "''");
            // first see if one exists.
            var locations = _dynamicsClient.Sharepointdocumentlocations.Get(filter: "relativeurl eq '" + sanitized + "'");
            var location = locations.Value.FirstOrDefault();

            if (location == null)
            {
                //get parent location 
                var parentSite = _dynamicsClient.Sharepointsites.Get().Value.FirstOrDefault();
                var parentSiteRef = _dynamicsClient.GetEntityURI("sharepointsites", parentSite.Sharepointsiteid);
                MicrosoftDynamicsCRMsharepointdocumentlocation newRecord = new MicrosoftDynamicsCRMsharepointdocumentlocation()
                {
                    Relativeurl = relativeUrl,
                    Name = "Worker Qualification",
                    ParentSiteODataBind = parentSiteRef
                };
                // create a new document location.
                try
                {
                    location = _dynamicsClient.Sharepointdocumentlocations.Create(newRecord);
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.LogError("Error creating document location");
                    _logger.LogError("Request:");
                    _logger.LogError(httpOperationException.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(httpOperationException.Response.Content);
                }
            }

            if (location != null)
            {
                result = location.Sharepointdocumentlocationid;
            }

            return result;
        }


        [HttpGet("phs-link/{contactId}")]
        [AllowAnonymous]
        public JsonResult Subscribe(string contactId)
        {
            string confirmationEmailLink = GetPhsLink(contactId);
            // string bclogo = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/assets/bc-logo.svg";
            // /* send the user an email confirmation. */
            // string body = "<img src='" + bclogo + "'/><br><h2>Confirm your email address</h2><p>Thank you for signing up to receive updates about cannabis stores in B.C. We’ll send you updates as new rules and regulations are released about selling cannabis.</p>"
            //     + "<p>To confirm your request and begin receiving updates by email, click here:</p>"
            //     + "<a href='" + confirmationEmailLink + "'>" + confirmationEmailLink + "</a>";

            // // send the email.
            // SmtpClient client = new SmtpClient(_configuration["SMTP_HOST"]);

            // // Specify the message content.
            // MailMessage message = new MailMessage("no-reply@gov.bc.ca", email);
            // message.Subject = "BC LCLB Cannabis Licensing Newsletter Subscription Confirmation";
            // message.Body = body;
            // message.IsBodyHtml = true;
            // client.Send(message);

            return new JsonResult(confirmationEmailLink);
        }

        private string GetPhsLink(string contactId)
        {
            string result = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/personal-history-summary/";
            result += WebUtility.UrlEncode(EncryptionUtility.EncryptString(contactId, _encryptionKey));
            return result;
        }

        [HttpGet("phs/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetContactByToken(string code)
        {
            string id = EncryptionUtility.DecryptString(code, _encryptionKey);
            if (!string.IsNullOrEmpty(id))
            {
                Guid contactId = Guid.Parse(id);
                // query the Dynamics system to get the contact record.
                var contact = await _dynamicsClient.GetContactById(contactId);

                if (contact != null)
                {
                    var result = new PHSContact
                    {
                        token = code,
                        shortName = (contact.Firstname.First().ToString() + " " + contact.Lastname)
                    };
                    return new JsonResult(result);
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
        }
    }

    public class PHSContact
    {
        public string token { get; set; }
        public string shortName { get; set; }
    }
}
