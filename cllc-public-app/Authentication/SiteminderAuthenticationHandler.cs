using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Gov.Lclb.Cllb.Interfaces.Models;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using Gov.Lclb.Cllb.Services.FileManager;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;
using Gov.Lclb.Cllb.Public.ViewModels;
using System.Web;
using Gov.Lclb.Cllb.Public.Utility;
using Microsoft.Rest;
using System.Collections.Specialized;
using Serilog;

namespace Gov.Lclb.Cllb.Public.Authentication
{
    #region SiteMinder Authentication Options
    /// <summary>
    /// Options required for setting up SiteMinder Authentication
    /// </summary>
    public class SiteMinderAuthOptions : AuthenticationSchemeOptions
    {
        // note that header keys are case insensitive, thus the reason why the keys are all lower case.

        private const string ConstDevAuthenticationTokenKey = "DEV-USER";
        private const string ConstDevBCSCAuthenticationTokenKey = "DEV-BCSC-USER";
        private const string ConstDevDefaultUserId = "TMcTesterson";
        private const string ConstSiteMinderUserGuidKey = "smgov_userguid"; //deprecated -- smgov_useridentifier
        private const string ConstSiteMinderUserIdentifierKey = "smgov_useridentifier";
        private const string ConstSiteMinderUniversalIdKey = "sm_universalid";
        private const string ConstSiteMinderUserNameKey = "sm_user";

        //BCeId Values
        private const string ConstSiteMinderBusinessGuidKey = "smgov_businessguid";
        private const string ConstSiteMinderBusinessLegalNameKey = "smgov_businesslegalname";

        //BC Services Card
        private const string ConstSiteMinderBirthDate = "smgov_birthdate";

        //BCeID or BC Services Card
        private const string ConstSiteMinderUserType = "smgov_usertype"; // get the type values from siteminder header this will be bceid or bcservices

        private const string ConstSiteMinderUserDisplayNameKey = "smgov_userdisplayname";

        private const string ConstMissingSiteMinderUserIdError = "Missing SiteMinder UserId";
        private const string ConstMissingSiteMinderGuidError = "Missing SiteMinder Guid";
        private const string ConstMissingSiteMinderUserTypeError = "Missing SiteMinder User Type";
        private const string ConstMissingDbUserIdError = "Could not find UserId in the database";
        private const string ConstUnderageError = "You must be 19 years of age or older to access this website.";

        private const string ConstInactivegDbUserIdError = "Database UserId is inactive";
        private const string ConstInvalidPermissions = "UserId does not have valid permissions";
        private const string ConstLoginDisabledError = "Login Disabled";

        /// <summary>
        /// DEfault Constructor
        /// </summary>
        public SiteMinderAuthOptions()
        {
            SiteMinderBusinessGuidKey = ConstSiteMinderBusinessGuidKey;
            SiteMinderUserGuidKey = ConstSiteMinderUserGuidKey;
            SiteMinderUserIdentifierKey = ConstSiteMinderUserIdentifierKey;
            SiteMinderUniversalIdKey = ConstSiteMinderUniversalIdKey;
            SiteMinderUserNameKey = ConstSiteMinderUserNameKey;
            SiteMinderUserDisplayNameKey = ConstSiteMinderUserDisplayNameKey;
            SiteMinderBusinessLegalNameKey = ConstSiteMinderBusinessLegalNameKey;
            SiteMinderUserTypeKey = ConstSiteMinderUserType;
            SiteMinderBirthDate = ConstSiteMinderBirthDate;
            MissingSiteMinderUserIdError = ConstMissingSiteMinderUserIdError;
            MissingSiteMinderUserTypeError = ConstMissingSiteMinderUserIdError;
            MissingSiteMinderGuidError = ConstMissingSiteMinderGuidError;
            MissingDbUserIdError = ConstMissingDbUserIdError;
            InactivegDbUserIdError = ConstInactivegDbUserIdError;
            InvalidPermissions = ConstInvalidPermissions;
            DevAuthenticationTokenKey = ConstDevAuthenticationTokenKey;
            DevBCSCAuthenticationTokenKey = ConstDevBCSCAuthenticationTokenKey;
            DevDefaultUserId = ConstDevDefaultUserId;
            UnderageError = ConstUnderageError;
            LoginDisabledError = ConstLoginDisabledError;
        }

        /// <summary>
        /// Default Scheme Name
        /// </summary>
        public static string AuthenticationSchemeName => "site-minder-auth";

        /// <summary>
        /// SiteMinder Authentication Scheme Name
        /// </summary>
        public string Scheme => AuthenticationSchemeName;

        public string SiteMinderBusinessGuidKey { get; set; }

        /// <summary>
        /// User GUID
        /// </summary>
        public string SiteMinderUserGuidKey { get; set; }

        /// <summary>
        /// User Identifier
        /// </summary>
        public string SiteMinderUserIdentifierKey { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        public string SiteMinderUniversalIdKey { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        public string SiteMinderUserNameKey { get; set; }

        /// <summary>
        /// User's Display Name
        /// </summary>
        public string SiteMinderUserDisplayNameKey { get; set; }

        ///<summary>
        ///User's Type (BCeID or BC services card)
        /// </summary>
        public string SiteMinderUserTypeKey { get; set; }

        /// <summary>
        /// Business Legal Name Key
        /// </summary>
        public string SiteMinderBusinessLegalNameKey { get; set; }

        /// <summary>
        /// BC Service Card - Birth Date field.
        /// </summary>
        public string SiteMinderBirthDate { get; set; }

        /// <summary>
        /// Missing SiteMinder User Type Error
        /// </summary>
        public string MissingSiteMinderUserTypeError { get; set; }

        /// <summary>
        /// Missing SiteMinder UserId Error
        /// </summary>
        public string MissingSiteMinderUserIdError { get; set; }

        /// <summary>
        /// Missing SiteMinder Guid Error
        /// </summary>
        public string MissingSiteMinderGuidError { get; set; }

        /// <summary>
        /// Missing Database UserId Error
        /// </summary>
        public string MissingDbUserIdError { get; set; }

        /// <summary>
        /// Inactive Database UserId Error
        /// </summary>
        public string InactivegDbUserIdError { get; set; }

        /// <summary>
        /// Inactive Database UserId Error
        /// </summary>
        public string UnderageError { get; set; }

        /// <summary>
        /// User does not jave active / valid permissions
        /// </summary>
        public string InvalidPermissions { get; set; }

        /// <summary>
        /// Development Environment Authentication Key
        /// </summary>
        public string DevAuthenticationTokenKey { get; set; }

        /// <summary>
        /// Development Environment Authentication Key
        /// </summary>
        public string DevBCSCAuthenticationTokenKey { get; set; }

        /// <summary>
        /// Development Environment efault UserId
        /// </summary>
        public string DevDefaultUserId { get; set; }

        public string LoginDisabledError { get; set; }
    }
    #endregion    

    /// <summary>
    /// Setup Siteminder Authentication Handler
    /// </summary>
    public static class SiteminderAuthenticationExtensions
    {
        /// <summary>
        /// Add Authentication Handler
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddSiteminderAuth(this AuthenticationBuilder builder, Action<SiteMinderAuthOptions> configureOptions)
        {
            return builder.AddScheme<SiteMinderAuthOptions, SiteminderAuthenticationHandler>(SiteMinderAuthOptions.AuthenticationSchemeName, configureOptions);
        }
    }

    /// <summary>
    /// Siteminder Authentication Handler
    /// </summary>
    public class SiteminderAuthenticationHandler : AuthenticationHandler<SiteMinderAuthOptions>
    {
        private readonly Serilog.ILogger _logger;
        private readonly Microsoft.Extensions.Logging.ILogger _ms_logger;
        private readonly SiteMinderAuthOptions _options;
        private IDynamicsClient _dynamicsClient;

        /// <summary>
        /// Siteminder Authentication Constructir
        /// </summary>
        /// <param name="configureOptions"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        public SiteminderAuthenticationHandler(IOptionsMonitor<SiteMinderAuthOptions> configureOptions, ILoggerFactory loggerFactory, UrlEncoder encoder, ISystemClock clock)
            : base(configureOptions, loggerFactory, encoder, clock)
        {
            _logger = Log.Logger;
            _ms_logger = loggerFactory.CreateLogger(typeof(SiteminderAuthenticationHandler));
            _options = new SiteMinderAuthOptions();
        }

        private async Task<AuthenticateResult> HandleBridgeAuthentication(UserSettings userSettings, HttpContext context)
        {
            // **************************************************
            // Authenticate based on SiteMinder Headers
            // **************************************************
            _logger.Debug("Parsing the HTTP headers for SiteMinder authentication credential");
            _logger.Debug("Getting user data from headers");

            FileManagerClient _fileManagerClient = (FileManagerClient)context.RequestServices.GetService(typeof(FileManagerClient));

            if (!string.IsNullOrEmpty(context.Request.Headers[_options.SiteMinderUserDisplayNameKey]))
            {
                userSettings.UserDisplayName = context.Request.Headers[_options.SiteMinderUserDisplayNameKey];
            }

            if (!string.IsNullOrEmpty(context.Request.Headers[_options.SiteMinderBusinessLegalNameKey]))
            {
                userSettings.BusinessLegalName = context.Request.Headers[_options.SiteMinderBusinessLegalNameKey];
            }

            var userId = context.Request.Headers[_options.SiteMinderUserNameKey];
            if (string.IsNullOrEmpty(userId))
            {
                userId = context.Request.Headers[_options.SiteMinderUniversalIdKey];
            }

            string siteMinderGuid = context.Request.Headers[_options.SiteMinderUserGuidKey];
            string siteMinderBusinessGuid = context.Request.Headers[_options.SiteMinderBusinessGuidKey];
            string siteMinderUserType = context.Request.Headers[_options.SiteMinderUserTypeKey];


            // **************************************************
            // Validate credentials
            // **************************************************
            if (string.IsNullOrEmpty(userId))
            {
                _logger.Debug(_options.MissingSiteMinderUserIdError);
                return AuthenticateResult.Fail(_options.MissingSiteMinderGuidError);
            }

            if (string.IsNullOrEmpty(siteMinderGuid))
            {
                _logger.Debug(_options.MissingSiteMinderGuidError);
                return AuthenticateResult.Fail(_options.MissingSiteMinderGuidError);
            }
            if (string.IsNullOrEmpty(siteMinderUserType))
            {
                _logger.Debug(_options.MissingSiteMinderUserTypeError);
                return AuthenticateResult.Fail(_options.MissingSiteMinderUserTypeError);
            }

            _logger.Debug("Loading user external id = " + siteMinderGuid);
            // 3/18/2020 - Note that LoadUserLegacy will now work if there is a match on the guid, as well as a match on name in a case where there is no guid.
            userSettings.AuthenticatedUser = await _dynamicsClient.LoadUserLegacy(siteMinderGuid, context.Request.Headers, _ms_logger);
            _logger.Information("After getting authenticated user = " + userSettings.GetJson());

            // check that the user is active
            if (userSettings.AuthenticatedUser != null
                && !userSettings.AuthenticatedUser.Active)
            {
                _logger.Debug(_options.InactivegDbUserIdError + " (" + userId + ")");
                return AuthenticateResult.Fail(_options.InactivegDbUserIdError);
            }

            // set the usertype to siteminder
            if (userSettings.AuthenticatedUser != null
                && !string.IsNullOrEmpty(siteMinderUserType))
            {
                userSettings.AuthenticatedUser.UserType = siteMinderUserType;
            }

            userSettings.UserType = siteMinderUserType;

            // Get the various claims for the current user.
            ClaimsPrincipal userPrincipal = userSettings.AuthenticatedUser.ToClaimsPrincipal(_options.Scheme, userSettings.UserType);

            // **************************************************
            // Create authenticated user
            // **************************************************
            _logger.Debug("Authentication successful: " + userId);
            _logger.Debug("Setting identity and creating session for: " + userId);

            // create session info for the current user
            userSettings.UserId = userId;
            userSettings.UserAuthenticated = true;
            userSettings.IsNewUserRegistration = userSettings.AuthenticatedUser == null;

            // set other session info
            userSettings.SiteMinderGuid = siteMinderGuid;
            userSettings.SiteMinderBusinessGuid = siteMinderBusinessGuid;
            _logger.Debug("Before getting contact and account ids = " + userSettings.GetJson());

            if (userSettings.AuthenticatedUser != null)
            {
                userSettings.ContactId = userSettings.AuthenticatedUser.ContactId.ToString();
                // ensure that the given account has a documents folder.

                if (siteMinderBusinessGuid != null) // BCeID user
                {
                    var contact = _dynamicsClient.GetActiveContactByExternalIdBridged(false, siteMinderGuid);
                    if (contact == null)
                    {
                        _logger.Information($"No bridged contact found for {siteMinderGuid}");
                        // try by other means.
                        var contactVM = new ViewModels.Contact();
                        contactVM.CopyHeaderValues(context.Request.Headers);
                        var temp = _dynamicsClient.GetContactByContactVmBlankSmGuid(contactVM);
                        if (temp != null) // ensure it is active.
                        {
                            contact = temp;
                            // update the contact.
                            _logger.Information(
                                $"Adding bridge record for login.  ContactID is {contact.Contactid}, GUID is {siteMinderGuid}");
                            _dynamicsClient.UpdateContactBridgeLogin(contact.Contactid, siteMinderGuid,
                                contact._accountidValue, siteMinderBusinessGuid);
                        }
                        else
                        {
                            _logger.Error("No existing contact found by search by header info.");
                        }
                    }
                    if (contact != null && contact.Contactid != null)
                    {
                        await CreateSharePointContactDocumentLocation(_fileManagerClient, contact);
                    }
                }
                
            }

            // populate the Account settings.
            if (siteMinderBusinessGuid != null) // BCeID user
            {
                // Note that this will search for active accounts
                var account = await _dynamicsClient.GetActiveAccountBySiteminderBusinessGuid(siteMinderBusinessGuid);
                if (account == null)
                {
                    // try by other means.
                    account = _dynamicsClient.GetActiveAccountByLegalName(userSettings.BusinessLegalName);
                }
                if (account != null && account.Accountid != null)
                {
                    userSettings.AccountId = account.Accountid;
                    if (userSettings.AuthenticatedUser == null)
                    {
                        userSettings.AuthenticatedUser = new Models.User();
                    }
                    userSettings.AuthenticatedUser.AccountId = Guid.Parse(account.Accountid);

                    // ensure that the given account has a documents folder.
                    await CreateSharePointAccountDocumentLocation(_fileManagerClient, account);
                }
                else  // force the new user process if contact exists but account does not.
                {
                    userSettings.AuthenticatedUser = null;
                    userSettings.IsNewUserRegistration = true;
                }
            }
                                               

            // add the worker settings if it is a new user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.NewWorker = new Worker();
                userSettings.NewWorker.CopyHeaderValues(context.Request.Headers);

                userSettings.NewContact = new ViewModels.Contact();
                userSettings.NewContact.CopyHeaderValues(context.Request.Headers);
            }
            else if (siteMinderUserType == "VerifiedIndividual")
            {
                await HandleVerifiedIndividualLogin(userSettings, context);
                if (HttpUtility.ParseQueryString(context.Request.QueryString.ToString()).Get("path") != "cannabis-associate-screening")
                {
                    await HandleWorkerLogin(userSettings, context);
                }
            }

            // **************************************************
            // Update user settings
            // **************************************************                
            UserSettings.SaveUserSettings(userSettings, context);

            return AuthenticateResult.Success(new AuthenticationTicket(userPrincipal, null, Options.Scheme));
        }


        private async Task<AuthenticateResult> HandleLegacyAuthentication(UserSettings userSettings, HttpContext context)
        {
            // **************************************************
            // Authenticate based on SiteMinder Headers
            // **************************************************
            _logger.Debug("Parsing the HTTP headers for SiteMinder authentication credential");
            _logger.Debug("Getting user data from headers");

            FileManagerClient _fileManagerClient = (FileManagerClient)context.RequestServices.GetService(typeof(FileManagerClient));

            if (!string.IsNullOrEmpty(context.Request.Headers[_options.SiteMinderUserDisplayNameKey]))
            {
                userSettings.UserDisplayName = context.Request.Headers[_options.SiteMinderUserDisplayNameKey];
            }

            if (!string.IsNullOrEmpty(context.Request.Headers[_options.SiteMinderBusinessLegalNameKey]))
            {
                userSettings.BusinessLegalName = context.Request.Headers[_options.SiteMinderBusinessLegalNameKey];
            }

            var userId = context.Request.Headers[_options.SiteMinderUserNameKey];
            if (string.IsNullOrEmpty(userId))
            {
                userId = context.Request.Headers[_options.SiteMinderUniversalIdKey];
            }

            string siteMinderGuid = context.Request.Headers[_options.SiteMinderUserGuidKey];
            string siteMinderBusinessGuid = context.Request.Headers[_options.SiteMinderBusinessGuidKey];
            string siteMinderUserType = context.Request.Headers[_options.SiteMinderUserTypeKey];


            // **************************************************
            // Validate credentials
            // **************************************************
            if (string.IsNullOrEmpty(userId))
            {
                _logger.Debug(_options.MissingSiteMinderUserIdError);
                return AuthenticateResult.Fail(_options.MissingSiteMinderGuidError);
            }

            if (string.IsNullOrEmpty(siteMinderGuid))
            {
                _logger.Debug(_options.MissingSiteMinderGuidError);
                return AuthenticateResult.Fail(_options.MissingSiteMinderGuidError);
            }
            if (string.IsNullOrEmpty(siteMinderUserType))
            {
                _logger.Debug(_options.MissingSiteMinderUserTypeError);
                return AuthenticateResult.Fail(_options.MissingSiteMinderUserTypeError);
            }

            _logger.Debug("Loading user external id = " + siteMinderGuid);
            // 3/18/2020 - Note that LoadUserLegacy will now work if there is a match on the guid, as well as a match on name in a case where there is no guid.
            userSettings.AuthenticatedUser = await _dynamicsClient.LoadUserLegacy(siteMinderGuid, context.Request.Headers, _ms_logger);
            _logger.Information("After getting authenticated user = " + userSettings.GetJson());


            // check that the user is active
            if (userSettings.AuthenticatedUser != null
                && !userSettings.AuthenticatedUser.Active)
            {
                _logger.Debug(_options.InactivegDbUserIdError + " (" + userId + ")");
                return AuthenticateResult.Fail(_options.InactivegDbUserIdError);
            }

            // set the usertype to siteminder
            if (userSettings.AuthenticatedUser != null
                && !string.IsNullOrEmpty(siteMinderUserType))
            {
                userSettings.AuthenticatedUser.UserType = siteMinderUserType;
            }

            userSettings.UserType = siteMinderUserType;

            // Get the various claims for the current user.
            ClaimsPrincipal userPrincipal = userSettings.AuthenticatedUser.ToClaimsPrincipal(_options.Scheme, userSettings.UserType);

            // **************************************************
            // Create authenticated user
            // **************************************************
            _logger.Debug("Authentication successful: " + userId);
            _logger.Debug("Setting identity and creating session for: " + userId);

            // create session info for the current user
            userSettings.UserId = userId;
            userSettings.UserAuthenticated = true;
            userSettings.IsNewUserRegistration = userSettings.AuthenticatedUser == null;

            // set other session info
            userSettings.SiteMinderGuid = siteMinderGuid;
            userSettings.SiteMinderBusinessGuid = siteMinderBusinessGuid;
            _logger.Debug("Before getting contact and account ids = " + userSettings.GetJson());

            if (userSettings.AuthenticatedUser != null)
            {
                userSettings.ContactId = userSettings.AuthenticatedUser.ContactId.ToString();
                // ensure that the given account has a documents folder.

                if (siteMinderBusinessGuid != null) // BCeID user
                {
                    var contact = _dynamicsClient.GetActiveContactByExternalId(userSettings.ContactId);
                    if (contact == null)
                    {
                        // try by other means.
                        var contactVM = new ViewModels.Contact();
                        contactVM.CopyHeaderValues(context.Request.Headers);
                        contact = _dynamicsClient.GetContactByContactVmBlankSmGuid(contactVM);
                    }
                    if (contact != null && contact.Contactid != null)
                    {
                        await CreateSharePointContactDocumentLocation(_fileManagerClient, contact);
                    }

                    // Note that this will search for active accounts
                    var account = await _dynamicsClient.GetActiveAccountBySiteminderBusinessGuid(siteMinderBusinessGuid);
                    if (account == null)
                    {
                        // try by other means.
                        account = _dynamicsClient.GetActiveAccountByLegalName(userSettings.BusinessLegalName);
                    }
                    if (account != null && account.Accountid != null)
                    {
                        userSettings.AccountId = account.Accountid;
                        userSettings.AuthenticatedUser.AccountId = Guid.Parse(account.Accountid);

                        // ensure that the given account has a documents folder.
                        await CreateSharePointAccountDocumentLocation(_fileManagerClient, account);
                    }
                    else  // force the new user process if contact exists but account does not.
                    {
                        userSettings.AuthenticatedUser = null;
                        userSettings.IsNewUserRegistration = true;
                    }

                    // handle cases where Contact was deleted.
                    if (contact == null)
                    {
                        userSettings.IsNewUserRegistration = true;
                    }

                }
            }

            // add the worker settings if it is a new user.
            if (userSettings.IsNewUserRegistration)
            {
                userSettings.NewWorker = new Worker();
                userSettings.NewWorker.CopyHeaderValues(context.Request.Headers);

                userSettings.NewContact = new ViewModels.Contact();
                userSettings.NewContact.CopyHeaderValues(context.Request.Headers);
            }
            else if (siteMinderUserType == "VerifiedIndividual")
            {
                await HandleVerifiedIndividualLogin(userSettings, context);
                if (HttpUtility.ParseQueryString(context.Request.QueryString.ToString()).Get("path") != "cannabis-associate-screening")
                {
                    await HandleWorkerLogin(userSettings, context);
                }
            }

            // **************************************************
            // Update user settings
            // **************************************************                
            UserSettings.SaveUserSettings(userSettings, context);

            return AuthenticateResult.Success(new AuthenticationTicket(userPrincipal, null, Options.Scheme));
        }

        /// <summary>
        /// Process Authentication Request
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // get siteminder headers
            _logger.Debug("Parsing the HTTP headers for SiteMinder authentication credential");


            string userId = null;
            string devCompanyId = null;
            string siteMinderGuid = "";
            string siteMinderBusinessGuid = "";
            string siteMinderUserType = "";

            try
            {
                ClaimsPrincipal principal;
                HttpContext context = Request.HttpContext;
                UserSettings userSettings = new UserSettings();

                IConfiguration _configuration = (IConfiguration)context.RequestServices.GetService(typeof(IConfiguration));
                _dynamicsClient = (IDynamicsClient)context.RequestServices.GetService(typeof(IDynamicsClient));
                
                IWebHostEnvironment hostingEnv = (IWebHostEnvironment)context.RequestServices.GetService(typeof(IWebHostEnvironment));

                // Fail if login disabled
                if (!string.IsNullOrEmpty(_configuration["FEATURE_DISABLE_LOGIN"]))
                {
                    return AuthenticateResult.Fail(_options.LoginDisabledError);
                }

                // Fail if coming from JS
                if (context.Request.GetDisplayUrl().ToLower().Contains(".js"))
                {
                    return AuthenticateResult.NoResult();
                }

                // **************************************************
                // Check if the user session is already created
                // **************************************************
                try
                {
                    _logger.Debug("Checking user session");
                    userSettings = UserSettings.ReadUserSettings(context);

                    // fix for cases where AuthenticatedUser contact is empty.
                    if (userSettings?.AuthenticatedUser?.ContactId != null &&
                        userSettings?.AuthenticatedUser?.ContactId == Guid.Empty && !string.IsNullOrEmpty(userSettings?.SiteMinderGuid))
                    {
                        
                        var contact = _dynamicsClient.GetActiveContactByExternalId(userSettings.SiteMinderGuid);
                        if (contact != null)
                        {
                            userSettings.AuthenticatedUser.ContactId = Guid.Parse(contact.Contactid);
                        }
                    }

                    _logger.Debug("UserSettings found: " + userSettings.GetJson());
                }
                catch
                {
                    //do nothing
                    _logger.Debug("No UserSettings found");
                }

                // is user authenticated - if so we're done
                if ((userSettings.UserAuthenticated && string.IsNullOrEmpty(userId)) ||
                    (userSettings.UserAuthenticated && !string.IsNullOrEmpty(userId) &&
                     !string.IsNullOrEmpty(userSettings.UserId) && userSettings.UserId == userId))
                {
                    _logger.Debug("User already authenticated with active session: " + userSettings.UserId);
                    principal = userSettings.AuthenticatedUser.ToClaimsPrincipal(_options.Scheme, userSettings.UserType);
                    return AuthenticateResult.Success(new AuthenticationTicket(principal, null, Options.Scheme));
                }

                // **************************************************
                // Check if we have a Dev Environment Cookie
                // **************************************************
                if (!hostingEnv.IsProduction() &&
                    (!string.IsNullOrEmpty(context.Request.Cookies[_options.DevAuthenticationTokenKey]) ||
                    !string.IsNullOrEmpty(context.Request.Cookies[_options.DevBCSCAuthenticationTokenKey]) ||
                    !string.IsNullOrEmpty(context.Request.Headers[_options.DevAuthenticationTokenKey]) ||
                    !string.IsNullOrEmpty(context.Request.Headers[_options.DevBCSCAuthenticationTokenKey]))
                )
                {
                    try
                    {
                        return await LoginDevUser(context, _dynamicsClient);
                    }
                    catch (Exception ex)
                    {
                        _logger.Information(ex.Message);
                        _logger.Information("Couldn't successfully login as dev user - continuing as regular user");
                    }
                }

                // determine if it is the new bridge entity based flow or the legacy.
                if (!string.IsNullOrEmpty(_configuration["FEATURE_BRIDGE_LOGIN"]))
                {
                    return await HandleBridgeAuthentication(userSettings, context);
                }
                else
                {
                    return await HandleLegacyAuthentication(userSettings, context);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
                throw;
            }
        }

        private async Task CreateSharePointAccountDocumentLocation(FileManagerClient _fileManagerClient, MicrosoftDynamicsCRMaccount account)
        {
            string folderName;
            string logFolderName = ""; 
            try
            {

                folderName = account.GetDocumentFolderName();
                logFolderName = WordSanitizer.Sanitize(folderName);

                var createFolderRequest = new CreateFolderRequest
                {
                    EntityName = "account",
                    FolderName = folderName
                };

                var createFolderResult = _fileManagerClient.CreateFolder(createFolderRequest);

                if (createFolderResult.ResultStatus == ResultStatus.Fail)
                {
                    _logger.Error($"Error creating folder for account {logFolderName}. Error is {createFolderResult.ErrorDetail}");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error creating folder for account {logFolderName}");
            }
        
        }

        private async Task CreateSharePointContactDocumentLocation(FileManagerClient _fileManagerClient, MicrosoftDynamicsCRMcontact contact)
        {
            string folderName;
            string logFolderName = "";
            try
            { 

                folderName = contact.GetDocumentFolderName();
                logFolderName = WordSanitizer.Sanitize(folderName);

                var createFolderRequest = new CreateFolderRequest
                {
                    EntityName = "contact",
                    FolderName = folderName
                };

                var createFolderResult = _fileManagerClient.CreateFolder(createFolderRequest);

                if (createFolderResult.ResultStatus == ResultStatus.Fail)
                {
                    _logger.Error($"Error creating folder for contact {logFolderName}. Error is {createFolderResult.ErrorDetail}");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error creating folder for contact {logFolderName}");
            }
        }

        private async Task CreateSharePointWorkerDocumentLocation(FileManagerClient _fileManagerClient, MicrosoftDynamicsCRMadoxioWorker worker)
        {
            string folderName = "";
            string logFolderName = "";
            try
            {

                folderName = worker.GetDocumentFolderName();
                logFolderName = WordSanitizer.Sanitize(folderName);

                var createFolderRequest = new CreateFolderRequest
                {
                    EntityName = "worker",
                    FolderName = folderName
                };

                var createFolderResult = _fileManagerClient.CreateFolder(createFolderRequest);

                if (createFolderResult.ResultStatus == ResultStatus.Fail)
                {
                    _logger.Error($"Error creating folder for contact {logFolderName}. Error is {createFolderResult.ErrorDetail}");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error creating folder for contact {logFolderName}");
            }
        }

        private async Task HandleVerifiedIndividualLogin(UserSettings userSettings, HttpContext context)
        {
            IConfiguration _configuration = (IConfiguration)context.RequestServices.GetService(typeof(IConfiguration));
            IDynamicsClient _dynamicsClient = (IDynamicsClient)context.RequestServices.GetService(typeof(IDynamicsClient));
            FileManagerClient _fileManagerClient = (FileManagerClient)context.RequestServices.GetService(typeof(FileManagerClient));

            BCeIDBusinessQuery _bceid = (BCeIDBusinessQuery)context.RequestServices.GetService(typeof (BCeIDBusinessQuery));

            ViewModels.Contact contact = new ViewModels.Contact();
            contact.CopyHeaderValues(context.Request.Headers);
            //LCSD-6488 - Change to Alway get First & Lastname from BCEID Web Query
            //These fields (FIRSTNAME & LASTNAME) are READONLY on our form and managed in BCEID.
            Gov.Lclb.Cllb.Interfaces.BCeIDBusiness bceidBusiness = await _bceid.ProcessBusinessQuery(userSettings.SiteMinderGuid);
            if(bceidBusiness != null) 
            {
                contact.firstname = bceidBusiness.individualFirstname;
                contact.lastname = bceidBusiness.individualSurname;
            }
            else
            {
                Gov.Lclb.Cllb.Interfaces.BCeIDBasic bceidBasic = await _bceid.ProcessBasicQuery(userSettings.SiteMinderGuid);
                if(bceidBasic != null)
                {
                    contact.firstname = bceidBasic.individualFirstname;
                    contact.lastname = bceidBasic.individualSurname;
                }
            }

            MicrosoftDynamicsCRMcontact savedContact = _dynamicsClient.Contacts.GetByKey(userSettings.ContactId);
            if (savedContact.Address1Line1 != null && savedContact.Address1Line1 != contact.address1_line1)
            {
                MicrosoftDynamicsCRMadoxioPreviousaddress prevAddress = new MicrosoftDynamicsCRMadoxioPreviousaddress
                {
                    AdoxioStreetaddress = savedContact.Address1Line1,
                    AdoxioProvstate = savedContact.Address1Stateorprovince,
                    AdoxioCity = savedContact.Address1City,
                    AdoxioCountry = savedContact.Address1Country,
                    AdoxioPostalcode = savedContact.Address1Postalcode,
                    ContactIdODataBind = _dynamicsClient.GetEntityURI("contacts", savedContact.Contactid)
                };
                _dynamicsClient.Previousaddresses.Create(prevAddress);
            }

            _dynamicsClient.Contacts.Update(userSettings.ContactId, contact.ToModel());
        }

        private async Task HandleWorkerLogin(UserSettings userSettings, HttpContext context)
        {
            IConfiguration _configuration = (IConfiguration)context.RequestServices.GetService(typeof(IConfiguration));
            IDynamicsClient _dynamicsClient = (IDynamicsClient)context.RequestServices.GetService(typeof(IDynamicsClient));
            FileManagerClient _fileManagerClient = (FileManagerClient)context.RequestServices.GetService(typeof(FileManagerClient));

            // Update worker with latest info from BC Service Card
            MicrosoftDynamicsCRMadoxioWorkerCollection workerCollection = _dynamicsClient.Workers.Get(filter: $"_adoxio_contactid_value eq {userSettings.ContactId}");
            if (workerCollection.Value.Count > 0)
            {
                MicrosoftDynamicsCRMadoxioWorker savedWorker = workerCollection.Value[0];

                Worker worker = new Worker();
                worker.CopyHeaderValues(context.Request.Headers);

                MicrosoftDynamicsCRMadoxioWorker patchWorker = new MicrosoftDynamicsCRMadoxioWorker
                {
                    AdoxioFirstname = worker.firstname,
                    AdoxioLastname = worker.lastname,
                    AdoxioMiddlename = worker.middlename
                };
                if (worker.gender != 0)
                {
                    patchWorker.AdoxioGendercode = (int)worker.gender;
                }

                _dynamicsClient.Workers.Update(savedWorker.AdoxioWorkerid, patchWorker);

                var updatedWorker = await _dynamicsClient.GetWorkerByIdWithChildren(savedWorker.AdoxioWorkerid);

                // only create the worker document location if the FEATURE_NO_WET_SIGNATURE setting is blank
                if (string.IsNullOrEmpty(_configuration["FEATURE_NO_WET_SIGNATURE"]))
                {
                    // ensure that the worker has a documents folder.                        
                    await CreateSharePointWorkerDocumentLocation(_fileManagerClient, updatedWorker);
                }
            }
        }

        private async Task<AuthenticateResult> LoginDevUser(HttpContext context, IDynamicsClient dynamicsClient)
        {
            string userId = null;
            string devCompanyId = null;
            bool isDeveloperLogin = false;
            bool isBCSCDeveloperLogin = false;
            UserSettings userSettings = new UserSettings();

            // check for a fake BCeID login
            
            string temp = context.Request.Cookies[_options.DevAuthenticationTokenKey];

            if (string.IsNullOrEmpty(temp)) // could be an automated test user.
            {
                temp = context.Request.Headers["DEV-USER"];
            }

            if (!string.IsNullOrEmpty(temp))
            {
                if (temp.Contains("::"))
                {
                    var temp2 = temp.Split("::");
                    userId = temp2[0];
                    if (temp2.Length >= 2)
                        devCompanyId = temp2[1];
                    else
                        devCompanyId = temp2[0];
                }
                else
                {
                    userId = temp;
                    devCompanyId = temp;
                }
                isDeveloperLogin = true;

                _logger.Debug("Got user from dev cookie = " + userId + ", company = " + devCompanyId);
            }
            else
            {
                // same set of tests for a BC Services Card dev login
                temp = context.Request.Cookies[_options.DevBCSCAuthenticationTokenKey];

                if (string.IsNullOrEmpty(temp)) // could be an automated test user.
                {
                    temp = context.Request.Headers["DEV-BCSC-USER"];
                }

                if (!string.IsNullOrEmpty(temp))
                {
                    userId = temp;
                    isBCSCDeveloperLogin = true;
                    _logger.Debug("Got user from dev cookie = " + userId);
                }
            }

            if (isDeveloperLogin)
            {
                _logger.Debug("Generating a Development user");
                userSettings.BusinessLegalName = devCompanyId + " TestBusiness";
                userSettings.UserDisplayName = userId + " TestUser";
                userSettings.SiteMinderGuid = GuidUtility.CreateIdForDynamics("contact", userSettings.UserDisplayName).ToString();
                userSettings.SiteMinderBusinessGuid = GuidUtility.CreateIdForDynamics("account", userSettings.BusinessLegalName).ToString();
                userSettings.UserType = "Business";
            }
            else if (isBCSCDeveloperLogin)
            {
                _logger.Debug("Generating a Development BC Services user");
                userSettings.BusinessLegalName = null;
                userSettings.UserDisplayName = userId + " Associate";
                userSettings.SiteMinderGuid = GuidUtility.CreateIdForDynamics("bcsc", userSettings.UserDisplayName).ToString();
                userSettings.SiteMinderBusinessGuid = null;
                userSettings.UserType = "VerifiedIndividual";
            }

            NameValueCollection queryStringParams = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());
            if (queryStringParams.Get("path") == "cannabis-associate-screening" && queryStringParams.Get("success") == "false")
            {
                context.Request.Headers.Add("smgov_givenname", "NORMAN");
                context.Request.Headers.Add("smgov_givennames", "Norman Percevel ");
                context.Request.Headers.Add("smgov_useremail", "norman@rockwell.com");
                context.Request.Headers.Add("smgov_birthdate", "1986-12-03");
                context.Request.Headers.Add("smgov_sex", "Male");
                context.Request.Headers.Add("smgov_streetaddress", "2000 STORMAN ROW");
                context.Request.Headers.Add("smgov_city", "PENTICTON");
                context.Request.Headers.Add("smgov_postalcode", "V8V8V8");
                context.Request.Headers.Add("smgov_province", "BC");
                context.Request.Headers.Add("smgov_country", "CA");
                context.Request.Headers.Add("smgov_surname", "ROCKWELL");
                userSettings.UserDisplayName = "NORMAN ROCKWELL";
            }
            else if (queryStringParams.Get("path") == "cannabis-associate-screening")
            {
                context.Request.Headers.Add("smgov_givenname", "JOE");
                context.Request.Headers.Add("smgov_givennames", "Joe Shmoe ");
                context.Request.Headers.Add("smgov_useremail", "joe@associate.com");
                context.Request.Headers.Add("smgov_birthdate", "1986-12-03");
                context.Request.Headers.Add("smgov_sex", "Male");
                context.Request.Headers.Add("smgov_streetaddress", "2000 COLONIAL ROW");
                context.Request.Headers.Add("smgov_city", "PENTICTON");
                context.Request.Headers.Add("smgov_postalcode", "V2A7P4");
                context.Request.Headers.Add("smgov_province", "BC");
                context.Request.Headers.Add("smgov_country", "CA");
                context.Request.Headers.Add("smgov_surname", "ONE");
                userSettings.UserDisplayName = "JOE ONE";
                userSettings.ContactId = "3c254b30-ebf2-ea11-b81d-00505683fbf4";
            }

            _logger.Debug("DEV MODE Setting identity and creating session for: " + userId);

            // create session info for the current user
            userSettings.AuthenticatedUser = await _dynamicsClient.LoadUserLegacy(userSettings.SiteMinderGuid, context.Request.Headers, _ms_logger);
            if (userSettings.AuthenticatedUser == null)
            {
                userSettings.UserAuthenticated = true;
                userSettings.IsNewUserRegistration = true;
            }
            else
            {
                userSettings.AuthenticatedUser.UserType = userSettings.UserType;
                userSettings.AccountId = userSettings.AuthenticatedUser.AccountId.ToString();
                userSettings.ContactId = userSettings.AuthenticatedUser.ContactId.ToString();
                userSettings.UserAuthenticated = true;
                userSettings.IsNewUserRegistration = false;
            }
            userSettings.UserId = userId;

            ClaimsPrincipal userPrincipal = userSettings.AuthenticatedUser.ToClaimsPrincipal(_options.Scheme, userSettings.UserType);
            UserSettings.SaveUserSettings(userSettings, context);
            return AuthenticateResult.Success(new AuthenticationTicket(userPrincipal, null, _options.Scheme));
        }
        private bool UserIsUnderage(HttpContext context)
        {
            string rawBirthDate = context.Request.Headers[_options.SiteMinderBirthDate];
            // get the birthdate.
            if (DateTimeOffset.TryParse(rawBirthDate, out DateTimeOffset birthDate))
            {
                DateTimeOffset nineteenYears = DateTimeOffset.Now.AddYears(-19);
                if (birthDate > nineteenYears)
                {
                    // younger than 19, cannot login.
                    return true;
                }
            }
            return false;
        }

        
        
    }
}
