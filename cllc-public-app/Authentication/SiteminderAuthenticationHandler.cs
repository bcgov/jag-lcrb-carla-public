using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Contexts;

namespace Gov.Lclb.Cllb.Public.Authentication
{    
    #region SiteMinder Authentication Options
    /// <summary>
    /// Options required for setting up SiteMinder Authentication
    /// </summary>
    public class SiteMinderAuthOptions : AuthenticationSchemeOptions
    {
        private const string ConstDevAuthenticationTokenKey = "DEV-USER";
        private const string ConstDevDefaultUserId = "TMcTesterson";       
        private const string ConstSiteMinderUserGuidKey = "smgov_userguid";
        private const string ConstSiteMinderUniversalIdKey = "sm_universalid";
        private const string ConstSiteMinderUserNameKey = "sm_user";
        private const string ConstSiteMinderBusinessGuidKey = "smgov_businessguid";
        private const string ConstSiteMinderBusinessLegalNameKey = "smgov_businesslegalname";

        private const string ConstSiteMinderUserDisplayNameKey = "smgov_userdisplayname";

        private const string ConstMissingSiteMinderUserIdError = "Missing SiteMinder UserId";
        private const string ConstMissingSiteMinderGuidError = "Missing SiteMinder Guid";
        private const string ConstMissingDbUserIdError = "Could not find UserId in the database";
        private const string ConstInactivegDbUserIdError = "Database UserId is inactive";
        private const string ConstInvalidPermissions = "UserId does not have valid permissions";

        /// <summary>
        /// DEfault Constructor
        /// </summary>
        public SiteMinderAuthOptions()
        {
            SiteMinderBusinessGuidKey = ConstSiteMinderBusinessGuidKey;
            SiteMinderUserGuidKey = ConstSiteMinderUserGuidKey;
            SiteMinderUniversalIdKey = ConstSiteMinderUniversalIdKey;
            SiteMinderUserNameKey = ConstSiteMinderUserNameKey;
            SiteMinderUserDisplayNameKey = ConstSiteMinderUserDisplayNameKey;
            MissingSiteMinderUserIdError = ConstMissingSiteMinderUserIdError;
            MissingSiteMinderGuidError = ConstMissingSiteMinderGuidError;
            MissingDbUserIdError = ConstMissingDbUserIdError;
            InactivegDbUserIdError = ConstInactivegDbUserIdError;
            InvalidPermissions = ConstInvalidPermissions;
            DevAuthenticationTokenKey = ConstDevAuthenticationTokenKey;
            DevDefaultUserId = ConstDevDefaultUserId;
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
        /// User does not jave active / valid permissions
        /// </summary>
        public string InvalidPermissions { get; set; }

        /// <summary>
        /// Development Environment Authentication Key
        /// </summary>
        public string DevAuthenticationTokenKey { get; set; }

        /// <summary>
        /// Development Environment efault UserId
        /// </summary>
        public string DevDefaultUserId { get; set; }
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
        private readonly ILogger _logger;        

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
            _logger = loggerFactory.CreateLogger(typeof(SiteminderAuthenticationHandler));                    
        }

        /// <summary>
        /// Process Authentication Request
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // get siteminder headers
            _logger.LogDebug("Parsing the HTTP headers for SiteMinder authentication credential");
                  
            SiteMinderAuthOptions options = new SiteMinderAuthOptions();

            try
            {
                ClaimsPrincipal principal;

                HttpContext context = Request.HttpContext;
                AppDbContext dataAccess = (AppDbContext)context.RequestServices.GetService(typeof(AppDbContext));
                IHostingEnvironment hostingEnv = (IHostingEnvironment)context.RequestServices.GetService(typeof(IHostingEnvironment));
                
                UserSettings userSettings = new UserSettings();
                string userId = "";
                string siteMinderGuid = "";

                // **************************************************
                // If this is an Error or Authentiation API - Ignore
                // **************************************************
                string url = context.Request.GetDisplayUrl().ToLower();

                if (url.Contains("/authentication/dev") ||
                    url.Contains("/error") ||
                    url.Contains(".map") ||
                    url.Contains(".js"))
                {
                    return AuthenticateResult.NoResult();
                }

                // **************************************************
                // Check if we have a Dev Environment Cookie
                // **************************************************
                if (hostingEnv.IsDevelopment() || hostingEnv.IsStaging())
                {
                    string temp = context.Request.Cookies[options.DevAuthenticationTokenKey];

                    if (!string.IsNullOrEmpty(temp))
                        userId = temp;
                }

                // **************************************************
                // Check if the user session is already created
                // **************************************************
                try
                {
                    _logger.LogInformation("Checking user session");
                    userSettings = UserSettings.ReadUserSettings(context);
                }
                catch
                {
                    //do nothing
                }

                // is user authenticated - if so we're done
                if ((userSettings.UserAuthenticated && string.IsNullOrEmpty(userId)) ||
                    (userSettings.UserAuthenticated && !string.IsNullOrEmpty(userId) &&
                     !string.IsNullOrEmpty(userSettings.UserId) && userSettings.UserId == userId))
                {
                    _logger.LogInformation("User already authenticated with active session: " + userSettings.UserId);
                    principal = userSettings.AuthenticatedUser.ToClaimsPrincipal(options.Scheme);
                    return AuthenticateResult.Success(new AuthenticationTicket(principal, null, Options.Scheme));
                }

                // **************************************************
                // Authenticate based on SiteMinder Headers
                // **************************************************
                _logger.LogDebug("Parsing the HTTP headers for SiteMinder authentication credential");

                if (string.IsNullOrEmpty(userId))
                {
                    userId = context.Request.Headers[options.SiteMinderUserNameKey];
                    if (string.IsNullOrEmpty(userId))
                    {
                        userId = context.Request.Headers[options.SiteMinderUniversalIdKey];
                    }

                    siteMinderGuid = context.Request.Headers[options.SiteMinderUserGuidKey];

                    // **************************************************
                    // Validate credentials
                    // **************************************************
                    if (string.IsNullOrEmpty(userId))
                    {
                        _logger.LogError(options.MissingSiteMinderUserIdError);
                        return AuthenticateResult.Fail(options.MissingSiteMinderGuidError);
                    }

                    if (string.IsNullOrEmpty(siteMinderGuid))
                    {
                        _logger.LogError(options.MissingSiteMinderGuidError);
                        return AuthenticateResult.Fail(options.MissingSiteMinderGuidError);
                    }
                }

                // **************************************************
                // Validate credential against database              
                // **************************************************
                userSettings.AuthenticatedUser = hostingEnv.IsDevelopment() || hostingEnv.IsStaging()
                    ? dataAccess.LoadUser(userId)
                    : dataAccess.LoadUser(userId, siteMinderGuid);
                


                if (userSettings.AuthenticatedUser != null && !userSettings.AuthenticatedUser.Active)
                {
                    _logger.LogWarning(options.InactivegDbUserIdError + " (" + userId + ")");
                    return AuthenticateResult.Fail(options.InactivegDbUserIdError);
                }

                // **************************************************
                // Validate / check user permissions
                // **************************************************
                ClaimsPrincipal userPrincipal = userSettings.AuthenticatedUser.ToClaimsPrincipal(options.Scheme);

                if (userPrincipal != null && !(userPrincipal.HasClaim(User.PermissionClaim, Permission.Login) || userPrincipal.HasClaim(User.PermissionClaim, Permission.NewUserRegistration)))
                {
                    _logger.LogWarning("User does not have permission to login or register.");
                    return AuthenticateResult.Fail(options.InvalidPermissions);
                }

                // **************************************************
                // Create authenticated user
                // **************************************************
                _logger.LogInformation("Authentication successful: " + userId);
                _logger.LogInformation("Setting identity and creating session for: " + userId);

                // create session info
                userSettings.UserId = userId;
                userSettings.UserAuthenticated = true;
                userSettings.IsNewUserRegistration = userPrincipal.HasClaim(User.PermissionClaim, Permission.NewUserRegistration);

                if (userSettings.IsNewUserRegistration && (hostingEnv.IsDevelopment() || hostingEnv.IsStaging()))
                {
                    userSettings.BusinessLegalName = userId + " TestBusiness";
                    userSettings.UserDisplayName = userId + " TestUser";
                    // add generated guids
                    userSettings.SiteMinderBusinessGuid = new Guid().ToString();
                    userSettings.SiteMinderGuid = new Guid().ToString();
                }

                // **************************************************
                // Update user settings
                // **************************************************                
                UserSettings.SaveUserSettings(userSettings, context);

                // done!
                principal = userPrincipal;
                return AuthenticateResult.Success(new AuthenticationTicket(principal, null, Options.Scheme));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
                Console.WriteLine(exception);
                throw;
            }
        }        
    }    
}
