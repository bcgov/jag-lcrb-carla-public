using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class LegalEntitiesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly string _encryptionKey;

        public LegalEntitiesController(IConfiguration configuration, SharePointFileManager sharePointFileManager, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            _configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _encryptionKey = _configuration["ENCRYPTION_KEY"];
            _logger = loggerFactory.CreateLogger(typeof(LegalEntitiesController));
        }

        /// <summary>
        /// Get all Dynamics Legal Entities
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet()]
        public JsonResult GetDynamicsLegalEntities()
        {
            List<ViewModels.LegalEntity> result = new List<LegalEntity>();
            IEnumerable<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = null;
            String accountfilter = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();

            // set account filter
            accountfilter = "_adoxio_account_value eq " + userSettings.AccountId;
            _logger.LogDebug("Account filter = " + accountfilter);

            try
            {
                legalEntities = _dynamicsClient.Legalentities.Get(filter: accountfilter).Value;
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, $"Error while getting legal entities. ");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected Exception while getting legal entities.");
            }


            foreach (var legalEntity in legalEntities)
            {
                result.Add(legalEntity.ToViewModel());
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Get all Dynamics Legal Entities for the current Business Profile Summary
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("business-profile-summary")]
        public JsonResult GetBusinessProfileSummary()
        {
            List<ViewModels.LegalEntity> result = new List<LegalEntity>();

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();
            List<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = GetAccountLegalEntities(userSettings.AccountId);

            foreach (var legalEntity in legalEntities)
            {
                result.Add(legalEntity.ToViewModel());
            }

            return new JsonResult(result);
        }

        [HttpGet("current-hierarchy")]
        public JsonResult GetCurrentHierarchy()
        {
            List<ViewModels.LegalEntity> result = new List<LegalEntity>();

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();

            List<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = GetAccountLegalEntities(userSettings.AccountId);

            foreach (var legalEntity in legalEntities)
            {
                result.Add(legalEntity.ToViewModel());
            }

            return new JsonResult(result);
        }

        private List<LegalEntity> GetAccountHierarchy(string accountId, List<string> shareHolders = null)
        {
            List<LegalEntity> result = null;
            var filter = "_adoxio_account_value eq " + accountId;
            if (shareHolders == null)
            {
                shareHolders = new List<string>();
            }

            var response = _dynamicsClient.Legalentities.Get(filter: filter);

            if (response != null && response.Value != null)
            {
                var legalEntities = response.Value.ToList();

                foreach (var legalEntity in legalEntities)
                {
                    var viewModel = legalEntity.ToViewModel();
                    viewModel.relatedentities = new List<LegalEntity>();
                    if (!String.IsNullOrEmpty(legalEntity._adoxioShareholderaccountidValue) && !shareHolders.Contains(legalEntity._adoxioShareholderaccountidValue))
                    {
                        shareHolders.Add(legalEntity._adoxioShareholderaccountidValue);
                        viewModel.relatedentities.AddRange(GetAccountHierarchy(legalEntity._adoxioShareholderaccountidValue, shareHolders));
                    }

                    result.Add(viewModel);
                }

            }
            return result;

        }

        private List<MicrosoftDynamicsCRMadoxioLegalentity> GetAccountLegalEntities(string accountId, List<string> shareHolders = null)
        {
            List<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = null;
            if (shareHolders == null)
            {
                shareHolders = new List<string>();
            }
            var filter = "_adoxio_account_value eq " + accountId;
            filter += " and adoxio_isindividual eq 0";

            try
            {
                legalEntities = _dynamicsClient.Legalentities.Get(filter: filter).Value.ToList();
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, $"Error while getting account legal entities. ");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected Exception while getting legal entities.");
            }


            if (legalEntities != null)
            {
                var children = new List<MicrosoftDynamicsCRMadoxioLegalentity>();

                foreach (var legalEntity in legalEntities)
                {
                    if (!String.IsNullOrEmpty(legalEntity._adoxioShareholderaccountidValue) && !shareHolders.Contains(legalEntity._adoxioShareholderaccountidValue))
                    {
                        shareHolders.Add(legalEntity._adoxioShareholderaccountidValue);
                        children.AddRange(GetAccountLegalEntities(legalEntity._adoxioShareholderaccountidValue, shareHolders));
                    }
                }
                legalEntities.AddRange(children);
            }
            return legalEntities.Distinct().ToList();

        }

        /// <summary>
        /// Get all Legal Entities where the position matches the parameter received
        /// By default, the account linked to the current user is used
        /// </summary>
        /// <param name="positionType"></param>
        /// <param name="parentLegalEntityId"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("position/{parentLegalEntityId}/{positionType}")]
        public IActionResult GetDynamicsLegalEntitiesByPosition(string parentLegalEntityId, string positionType)
        {
            List<ViewModels.LegalEntity> result = new List<LegalEntity>();
            IEnumerable<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = null;
            String filter = null;

            // Stops injections
            try
            {
                new Guid(parentLegalEntityId);
            }
            catch
            {
                return NotFound();
            }

            filter = "_adoxio_legalentityowned_value eq " + parentLegalEntityId;
            switch (positionType)
            {
                case "shareholders":
                case "partners":
                    filter += " and (adoxio_ispartner eq true or adoxio_isshareholder eq true)";
                    break;
                case "key-personnel":
                    filter += " and adoxio_iskeypersonnel eq true";
                    break;
                case "directors-officers-management":
                    filter += " and (adoxio_isdirector eq true or adoxio_isseniormanagement eq true or adoxio_isofficer eq true)";
                    break;
                case "director-officer-shareholder":
                    filter += " and adoxio_isindividual eq 1";
                    break;
                default:
                    filter += " and adoxio_isindividual eq 2"; //return nothing
                    break;
            }

            try
            {
                _logger.LogDebug("Account filter = " + filter);
                legalEntities = _dynamicsClient.Legalentities.Get(filter: filter).Value;

            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, $"Error while getting account legal entities. ");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected Exception while getting legal entities.");
            }


            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            if (legalEntities != null)
            {
                foreach (var legalEntity in legalEntities)
                {
                    // Users can't access other users legal entities.
                    if (!DynamicsExtensions.CurrentUserHasAccessToAccount(new Guid(legalEntity._adoxioAccountValue), _httpContextAccessor, _dynamicsClient))
                    {
                        return NotFound();
                    }
                    result.Add(legalEntity.ToViewModel());
                }
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Get the special applicant legal entity for the current user
        /// </summary>
        /// <returns></returns>
        [HttpGet("applicant")]
        public async Task<IActionResult> GetApplicantDynamicsLegalEntity()
        {
            ViewModels.LegalEntity result = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();

            // query the Dynamics system to get the legal entity record.
            MicrosoftDynamicsCRMadoxioLegalentity legalEntity = null;
            _logger.LogDebug("Find legal entity for applicant = " + userSettings.AccountId.ToString());

            legalEntity = _dynamicsClient.GetAdoxioLegalentityByAccountId(Guid.Parse(userSettings.AccountId));

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

            return new JsonResult(result);
        }

        /// <summary>
        /// Get a specific legal entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDynamicsLegalEntity(string id)
        {
            ViewModels.LegalEntity result = null;
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
                //prevent getting legal entity data if the user is not associated with the account
                if (adoxioLegalEntity == null || !DynamicsExtensions.CurrentUserHasAccessToAccount(new Guid(adoxioLegalEntity._adoxioAccountValue), _httpContextAccessor, _dynamicsClient))
                {
                    return new NotFoundResult();
                }
                result = adoxioLegalEntity.ToViewModel();
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Create a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateDynamicsLegalEntity([FromBody] ViewModels.LegalEntity item)
        {

            // create a new legal entity.
            MicrosoftDynamicsCRMadoxioLegalentity adoxioLegalEntity = new MicrosoftDynamicsCRMadoxioLegalentity();

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();
            // copy received values to Dynamics LegalEntity
            adoxioLegalEntity.CopyValues(item);
            try
            {
                adoxioLegalEntity = await _dynamicsClient.Legalentities.CreateAsync(adoxioLegalEntity);
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, $"Error while creating legal entity ");
                throw new Exception("Unable to create legal entity");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected Exception while getting legal entities.");
                throw new Exception("Unable to create legal entity");
            }

            // setup navigation properties.
            MicrosoftDynamicsCRMadoxioLegalentity patchEntity = new MicrosoftDynamicsCRMadoxioLegalentity();
            Guid accountId = Guid.Parse(userSettings.AccountId);
            var userAccount = await _dynamicsClient.GetAccountById(accountId);
            patchEntity.AdoxioAccountValueODataBind = _dynamicsClient.GetEntityURI("accounts", accountId.ToString());

            // patch the record.
            try
            {
                await _dynamicsClient.Legalentities.UpdateAsync(adoxioLegalEntity.AdoxioLegalentityid, patchEntity);
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, $"Error while patching legal entity: {odee.Request.Content} Response: {odee.Response.Content}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected Exception while patching legal entity");
            }

            // TODO take the default for now from the parent account's legal entity record
            // TODO likely will have to re-visit for shareholders that are corporations/organizations
            MicrosoftDynamicsCRMadoxioLegalentity tempLegalEntity = _dynamicsClient.GetAdoxioLegalentityByAccountId(Guid.Parse(userSettings.AccountId));
            if (tempLegalEntity != null)
            {
                Guid tempLegalEntityId = Guid.Parse(tempLegalEntity.AdoxioLegalentityid);

                // see https://msdn.microsoft.com/en-us/library/mt607875.aspx
                patchEntity = new MicrosoftDynamicsCRMadoxioLegalentity();
                patchEntity.AdoxioLegalEntityOwnedODataBind = _dynamicsClient.GetEntityURI("adoxio_legalentities", tempLegalEntityId.ToString());

                // patch the record.
                try
                {
                    await _dynamicsClient.Legalentities.UpdateAsync(adoxioLegalEntity.AdoxioLegalentityid, patchEntity);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, $"Error adding LegalEntityOwned reference to legal entity: {odee.Request.Content} Response: {odee.Response.Content}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Unexpected Exception while adding LegalEntityOwned reference to legal entity");
                }
            }

            return new JsonResult(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Create a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("child-legal-entity")]
        public IActionResult CreateDynamicsShareholderLegalEntity([FromBody] ViewModels.LegalEntity item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            MicrosoftDynamicsCRMadoxioLegalentity adoxioLegalEntity = new MicrosoftDynamicsCRMadoxioLegalentity();
            adoxioLegalEntity.CopyValues(item);

            if (item.isindividual != true)
            {
                var account = new MicrosoftDynamicsCRMaccount();
                account.Name = item.name;
                if (item.isShareholder == true)
                {
                    account.AdoxioAccounttype = (int)AdoxioAccountTypeCodes.Shareholder;
                }
                else if (item.isPartner == true)
                {
                    account.AdoxioAccounttype = (int)AdoxioAccountTypeCodes.Partner;
                }
                if (item.legalentitytype != null)
                {
                    account.AdoxioBusinesstype = (int)Enum.ToObject(typeof(Gov.Lclb.Cllb.Public.ViewModels.AdoxioApplicantTypeCodes), item.legalentitytype);
                }
                try
                {
                    account = _dynamicsClient.Accounts.Create(account);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, $"Error creating account: {odee.Request.Content} Response: {odee.Response.Content}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Unexpected Exception while creating tied house connection");
                }

                //create tied house under account
                var tiedHouse = new MicrosoftDynamicsCRMadoxioTiedhouseconnection()
                {
                    AccountODataBind = _dynamicsClient.GetEntityURI("accounts", account.Accountid)
                };

                adoxioLegalEntity.AdoxioShareholderAccountODataBind = _dynamicsClient.GetEntityURI("accounts", account.Accountid);
                try
                {
                    _dynamicsClient.Tiedhouseconnections.Create(tiedHouse);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, $"Error creating tied house connection");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Unexpected Exception while creating tied house connection");
                }
            }
            adoxioLegalEntity.AdoxioAccountValueODataBind = _dynamicsClient.GetEntityURI("accounts", item.account.id);
            adoxioLegalEntity.AdoxioLegalEntityOwnedODataBind = _dynamicsClient.GetEntityURI("adoxio_legalentities", item.parentLegalEntityId);

            try
            {
                adoxioLegalEntity = _dynamicsClient.Legalentities.Create(adoxioLegalEntity);
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, $"Error creating legal entity: {odee.Request.Content} Response: {odee.Response.Content}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected Exception while creating legal entity");
            }

            return new JsonResult(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Update a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDynamicsLegalEntity([FromBody] ViewModels.LegalEntity item, string id)
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
            try
            {
                _dynamicsClient.Legalentities.Update(adoxio_legalentityid.ToString(), adoxioLegalEntity);
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, $"Error updating legal entity: {odee.Request.Content} Response: {odee.Response.Content}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected Exception while updating legal entity");
            }


            adoxioLegalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);

            return new JsonResult(adoxioLegalEntity.ToViewModel());
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

            try
            {
                await _dynamicsClient.Legalentities.DeleteAsync(adoxio_legalentityid.ToString());
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, $"Error deleting legal entity: {odee.Request.Content} Response: {odee.Response.Content}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected Exception while deleting legal entity");
            }


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
            string result = _configuration["BASE_URI"] + _configuration["BASE_PATH"];

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
            return new JsonResult(result);
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
            // check that the session is setup correctly.
            userSettings.Validate();

            MicrosoftDynamicsCRMadoxioLegalentity userLegalentity = _dynamicsClient.GetAdoxioLegalentityByAccountId(Guid.Parse(userSettings.AccountId));

            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);

            // TODO verify that this is the current user's legal entity
            var adoxioLegalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);

            // now get each of the supplied ids and send an email to them.

            foreach (string recipientId in recipientIds)
            {
                Guid recipientIdGuid = new Guid(recipientId);
                // TODO verify that each recipient is part of the current user's user set
                // TODO switch over to new AuthAPI framework
                var recipientEntity = await _dynamicsClient.GetLegalEntityById(recipientIdGuid);
                string email = recipientEntity.AdoxioEmail;
                string firstname = recipientEntity.AdoxioFirstname;
                string lastname = recipientEntity.AdoxioLastname;

                string confirmationEmailLink = GetConsentLink(email, recipientId, id);
                string bclogo = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/assets/bc-logo.svg";
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
                SmtpClient client = new SmtpClient(_configuration["SMTP_HOST"]);

                // Specify the message content.
                MailMessage message = new MailMessage("no-reply@gov.bc.ca", email);
                message.Subject = "BC LCLB Cannabis Licensing Security Consent";
                message.Body = body;
                message.IsBodyHtml = true;
                client.Send(message);


                // save the consent link and the fact that the email has been sent
                MicrosoftDynamicsCRMadoxioLegalentity patchEntity = new MicrosoftDynamicsCRMadoxioLegalentity();
                patchEntity.AdoxioDateemailsent = DateTime.Now;

                // patch the record.
                try
                {
                    await _dynamicsClient.Legalentities.UpdateAsync(adoxioLegalEntity.AdoxioLegalentityid, patchEntity);
                }
                catch (HttpOperationException odee)
                {
                    _logger.LogError(odee, $"Error updating date email sent. ");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Unexpected Exception while updating date email sent.");
                }
            }

            return NoContent(); // 204
        }
    }
}
