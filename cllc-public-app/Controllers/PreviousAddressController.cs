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

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class PreviousAddressController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public PreviousAddressController(IConfiguration configuration, IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _dynamicsClient = dynamicsClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(PreviousAddressController));
        }


        // /// <summary>
        // /// Get a specific previous address
        // /// </summary>
        // /// <param name="id"></param>
        // /// <returns></returns>
        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetAddress(string id)
        // {
        //     ViewModels.PreviousAddress result = null;

        //     if (!string.IsNullOrEmpty(id))
        //     {
        //         Guid AddressId = Guid.Parse(id);
        //         // query the Dynamics system to get the Address record.
        //         MicrosoftDynamicsCRMadoxioPreviousaddress Address = await _dynamicsClient.GetAccountById(AddressId);

        //         if (Address != null)
        //         {
        //             result = Address.ToViewModel();
        //         }
        //         else
        //         {
        //             return new NotFoundResult();
        //         }
        //     }
        //     else
        //     {
        //         return BadRequest();
        //     }

        //     return Json(result);
        // }


        // /// <summary>
        // /// Update a legal entity
        // /// </summary>
        // /// <param name="item"></param>
        // /// <param name="id"></param>
        // /// <returns></returns>
        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateAddress([FromBody] ViewModels.Address item, string id)
        // {
        //     if (id != null && item.id != null && id != item.id)
        //     {
        //         return BadRequest();
        //     }

        //     // get the Address
        //     Guid AddressId = Guid.Parse(id);

        //     MicrosoftDynamicsCRMAddress Address = await _dynamicsClient.GetAddressById(AddressId);
        //     if (Address == null)
        //     {
        //         return new NotFoundResult();
        //     }
        //     MicrosoftDynamicsCRMAddress patchAddress = new MicrosoftDynamicsCRMAddress();
        //     patchAddress.CopyValues(item);
        //     try
        //     {
        //         await _dynamicsClient.Addresss.UpdateAsync(AddressId.ToString(), patchAddress);
        //     }
        //     catch (OdataerrorException odee)
        //     {
        //         _logger.LogError("Error updating Address");
        //         _logger.LogError("Request:");
        //         _logger.LogError(odee.Request.Content);
        //         _logger.LogError("Response:");
        //         _logger.LogError(odee.Response.Content);
        //     }

        //     Address = await _dynamicsClient.GetAddressById(AddressId);
        //     return Json(Address.ToViewModel());
        // }

        // /// <summary>
        // /// Create a Address
        // /// </summary>
        // /// <param name="viewModel"></param>
        // /// <returns></returns>
        // [HttpPost()]
        // public async Task<IActionResult> CreateAddress([FromBody] ViewModels.Address item)
        // {

        //     // get UserSettings from the session
        //     string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
        //     UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

        //     // first check to see that a Address exists.
        //     string AddressSiteminderGuid = userSettings.SiteMinderGuid;
        //     if (AddressSiteminderGuid == null || AddressSiteminderGuid.Length == 0)
        //     {
        //         _logger.LogError(LoggingEvents.Error, "No Address Siteminder Guid exernal id");
        //         throw new Exception("Error. No AddressSiteminderGuid exernal id");
        //     }

        //     // get the Address record.
        //     MicrosoftDynamicsCRMAddress userAddress = null;

        //     // see if the Address exists.
        //     try
        //     {
        //         userAddress = await _dynamicsClient.GetAddressBySiteminderGuid(AddressSiteminderGuid);
        //         if (userAddress != null)
        //         {
        //             throw new Exception("Address already Exists");
        //         }
        //     }
        //     catch (OdataerrorException odee)
        //     {
        //         _logger.LogError(LoggingEvents.Error, "Error getting Address by Siteminder Guid.");
        //         _logger.LogError("Request:");
        //         _logger.LogError(odee.Request.Content);
        //         _logger.LogError("Response:");
        //         _logger.LogError(odee.Response.Content);
        //         throw new OdataerrorException("Error getting Address by Siteminder Guid");
        //     }

        //     // create a new Address.
        //     MicrosoftDynamicsCRMAddress Address = new MicrosoftDynamicsCRMAddress();
        //     Address.CopyValues(item);
        //     string sanitizedAccountSiteminderId = GuidUtility.SanitizeGuidString(AddressSiteminderGuid);
        //     Address.Externaluseridentifier = userSettings.UserId;

        //     //clean externalId    
        //     var externalId = "";
        //     var tokens = sanitizedAccountSiteminderId.Split('|');
        //     if (tokens.Length > 0)
        //     {
        //         externalId = tokens[0];
        //     }

        //     if (!string.IsNullOrEmpty(externalId))
        //     {
        //         tokens = externalId.Split(':');
        //         externalId = tokens[tokens.Length - 1];
        //     }

        //     Address.AdoxioExternalid = externalId;
        //     try
        //     {
        //         Address = await _dynamicsClient.Addresss.CreateAsync(Address);
        //     }
        //     catch (OdataerrorException odee)
        //     {
        //         _logger.LogError("Error updating Address");
        //         _logger.LogError("Request:");
        //         _logger.LogError(odee.Request.Content);
        //         _logger.LogError("Response:");
        //         _logger.LogError(odee.Response.Content);
        //     }

        //     // if we have not yet authenticated, then this is the new record for the user.
        //     if (userSettings.IsNewUserRegistration)
        //     {
        //         userSettings.AddressId = Address.Addressid.ToString();

        //         // we can now authenticate.
        //         if (userSettings.AuthenticatedUser == null)
        //         {
        //             Models.User user = new Models.User();
        //             user.Active = true;
        //             user.AddressId = Guid.Parse(userSettings.AddressId);
        //             user.UserType = userSettings.UserType;
        //             user.SmUserId = userSettings.UserId;
        //             userSettings.AuthenticatedUser = user;
        //         }

        //         userSettings.IsNewUserRegistration = false;

        //         string userSettingsString = JsonConvert.SerializeObject(userSettings);
        //         _logger.LogDebug("userSettingsString --> " + userSettingsString);

        //         // add the user to the session.
        //         _httpContextAccessor.HttpContext.Session.SetString("UserSettings", userSettingsString);
        //         _logger.LogDebug("user added to session. ");
        //     }
        //     else
        //     {
        //         _logger.LogError(LoggingEvents.Error, "Invalid user registration.");
        //         throw new Exception("Invalid user registration.");
        //     }

        //     return Json(Address.ToViewModel());
        // }
    }
}
