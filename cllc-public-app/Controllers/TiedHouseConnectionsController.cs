using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
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
using static Gov.Lclb.Cllb.Interfaces.SharePointFileManager;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class TiedHouseConnectionsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly string _encryptionKey;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public TiedHouseConnectionsController(IConfiguration configuration, SharePointFileManager sharePointFileManager, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            Configuration = configuration;
            this._encryptionKey = Configuration["ENCRYPTION_KEY"];
            this._httpContextAccessor = httpContextAccessor;
            this._dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(TiedHouseConnectionsController));
        }

        /// <summary>
        /// Get all Dynamics Legal Entities
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("{accountId}")]
        public async Task<JsonResult> GetTiedHouseConnections(string accountId)
        {
            var result = new List<ViewModels.TiedHouseConnection>();
            IEnumerable<MicrosoftDynamicsCRMadoxioTiedhouseconnection> tiedHouseConnections = null;
            String accountfilter = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();

            // set account filter
            accountfilter = "_adoxio_account_value eq " + accountId;
            _logger.LogError("Account filter = " + accountfilter);

            tiedHouseConnections = _dynamicsClient.AdoxioTiedhouseconnections.Get(filter: accountfilter).Value;

            foreach (var tiedHouse in tiedHouseConnections)
            {
                result.Add(tiedHouse.ToViewModel());
            }

            return Json(result);
        }

                /// <summary>
        /// Create a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateTiedHouse([FromBody] ViewModels.TiedHouseConnection item)
        {

            // create a new legal entity.
            var tiedHouse = new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
            // check that the session is setup correctly.
            userSettings.Validate();
            // copy received values to Dynamics LegalEntity
            tiedHouse.CopyValues(item);
            try
            {
                tiedHouse = await _dynamicsClient.AdoxioTiedhouseconnections.CreateAsync(tiedHouse);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error creating legal entity");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                throw new Exception("Unable to create legal entity");
            }

            // setup navigation properties.
            MicrosoftDynamicsCRMadoxioTiedhouseconnection patchEntity = new MicrosoftDynamicsCRMadoxioTiedhouseconnection();
            Guid accountId = Guid.Parse(userSettings.AccountId);
            var userAccount = await _dynamicsClient.GetAccountById(accountId);
            //patchEntity.AdoxioAccountValueODataBind = _dynamicsClient.GetEntityURI("accounts", accountId.ToString());

            // patch the record.
            try
            {
                await _dynamicsClient.AdoxioTiedhouseconnections.UpdateAsync(tiedHouse.AdoxioTiedhouseconnectionid, patchEntity);
            }
            catch (OdataerrorException odee)
            {
                _logger.LogError("Error patching legal entity");
                _logger.LogError(odee.Request.RequestUri.ToString());
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
            }

            // TODO take the default for now from the parent account's legal entity record
            // TODO likely will have to re-visit for shareholders that are corporations/organizations
            //MicrosoftDynamicsCRMadoxioTiedhouseconnection tempLegalEntity = await _dynamicsClient.GetAdoxioLegalentityByAccountId(Guid.Parse(userSettings.AccountId));
            //if (tempLegalEntity != null)
            //{
            //    Guid tempLegalEntityId = Guid.Parse(tempLegalEntity.AdoxioLegalentityid);

            //    // see https://msdn.microsoft.com/en-us/library/mt607875.aspx
            //    patchEntity = new MicrosoftDynamicsCRMadoxioTiedhouseconnection();
            //    patchEntity.AdoxioLegalEntityOwnedODataBind = _dynamicsClient.GetEntityURI("adoxio_legalentities", tempLegalEntityId.ToString());

            //    // patch the record.
            //    try
            //    {
            //        await _dynamicsClient.AdoxioTiedhouseconnections.UpdateAsync(tiedHouse.AdoxioLegalentityid, patchEntity);
            //    }
            //    catch (OdataerrorException odee)
            //    {
            //        _logger.LogError("Error adding LegalEntityOwned reference to legal entity");
            //        _logger.LogError(odee.Request.RequestUri.ToString());
            //        _logger.LogError("Request:");
            //        _logger.LogError(odee.Request.Content);
            //        _logger.LogError("Response:");
            //        _logger.LogError(odee.Response.Content);
            //    }
            //}

            return Json(tiedHouse.ToViewModel());
        }

        /// <summary>
        /// Update a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTiedHouse([FromBody] ViewModels.AdoxioLegalEntity item, string id)
        {
            if (id != item.id)
            {
                return BadRequest();
            }

            //// get the legal entity.
            //Guid adoxio_legalentityid = new Guid(id);

            //MicrosoftDynamicsCRMadoxioTiedhouseconnection adoxioLegalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);
            //if (adoxioLegalEntity == null)
            //{
            //    return new NotFoundResult();
            //}

            //// we are doing a patch, so wipe out the record.
            //adoxioLegalEntity = new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

            //// copy values over from the data provided
            //adoxioLegalEntity.CopyValues(item);

            //await _dynamicsClient.AdoxioTiedhouseconnections.UpdateAsync(adoxio_legalentityid.ToString(), adoxioLegalEntity);
            //return Json(adoxioLegalEntity.ToViewModel());
            return Ok();
        }

        /// <summary>
        /// Delete a legal entity.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteTiedHouse(string id)
        {
            //// get the legal entity.
            //Guid adoxio_legalentityid = new Guid(id);
            //MicrosoftDynamicsCRMadoxioTiedhouseconnection legalEntity = await _dynamicsClient.GetLegalEntityById(adoxio_legalentityid);
            //if (legalEntity == null)
            //{
            //    return new NotFoundResult();
            //}

            //await _dynamicsClient.AdoxioTiedhouseconnections.DeleteAsync(adoxio_legalentityid.ToString());

            return NoContent(); // 204
        }
    }
}
