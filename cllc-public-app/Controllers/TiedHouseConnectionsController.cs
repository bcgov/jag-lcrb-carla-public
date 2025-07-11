using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class TiedHouseConnectionsController : ControllerBase
    {
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public TiedHouseConnectionsController(ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, IHttpContextAccessor httpContextAccessor)
        {
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(TiedHouseConnectionsController));
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get TiedHouseConnection by accountId
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("{accountId}")]
        public JsonResult GetTiedHouseConnection(string accountId)
        {
            var result = new List<ViewModels.TiedHouseConnection>();
            IEnumerable<MicrosoftDynamicsCRMadoxioTiedhouseconnection> tiedHouseConnections = null;
            string accountfilter = "_adoxio_accountid_value eq " + accountId;
            _logger.LogDebug("Account filter = " + accountfilter);

            tiedHouseConnections = _dynamicsClient.Tiedhouseconnections.Get(filter: accountfilter).Value;

            foreach (var tiedHouse in tiedHouseConnections)
            {
                result.Add(tiedHouse.ToViewModel());
            }

            return new JsonResult(result.FirstOrDefault());
        }

        [HttpGet("application/{applicationId?}")]
        public JsonResult GetAllTiedHouseConnections(string applicationId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var result = new List<ViewModels.TiedHouseConnection>();
            var supersededbyIds = new List<string>();
            IEnumerable<MicrosoftDynamicsCRMadoxioTiedhouseconnection> tiedHouseConnections = null;
            string accountFilter = $"(_adoxio_accountid_value eq {userSettings.AccountId} and statuscode eq {(int)TiedHouseStatusCode.Existing})";
            if (!String.IsNullOrEmpty(applicationId))
            {
                accountFilter = accountFilter + $" or (_adoxio_application_value eq {applicationId} and adoxio_markedforremoval ne 1)";
            }
            _logger.LogDebug("Account filter = " + accountFilter);

            try
            {


                tiedHouseConnections = _dynamicsClient.Tiedhouseconnections.Get(filter: accountFilter).Value;

                foreach (var tiedHouse in tiedHouseConnections)
                {
                    if (tiedHouse.AdoxioMarkedForRemoval == 1)
                    {
                        var x = 1;
                    }
                    if (tiedHouse.Statuscode == (int)TiedHouseStatusCode.Existing)
                    {
                        tiedHouse._adoxio_supersededbyValue = tiedHouse.AdoxioTiedhouseconnectionid;
                    }
                    if (tiedHouse.Statuscode == (int)TiedHouseStatusCode.New)
                    {
                        supersededbyIds.Add(tiedHouse._adoxio_supersededbyValue);
                    }

                    result.Add(tiedHouse.ToViewModel());
                }
            }

            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating tied house connections");
                throw new Exception("Unable to add tied house connection");
            }

            result = result
                .Where(s => !supersededbyIds.Contains(s.id))
                .ToList();
            return new JsonResult(result);
        }

        /// <summary>
        /// Update a TiedHouseConnection
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTiedHouse([FromBody] ViewModels.TiedHouseConnection item, string id)
        {
            if (item == null || id != item.id)
            {
                return BadRequest();
            }

            // get the legal entity.
            Guid tiedHouseId = new Guid(id);

            MicrosoftDynamicsCRMadoxioTiedhouseconnection res = await _dynamicsClient.GetTiedHouseConnectionById(tiedHouseId);
            if (res == null)
            {
                return new NotFoundResult();
            }

            // we are doing a patch, so wipe out the record.
            var tiedHouse = new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

            // copy values over from the data provided
            tiedHouse.CopyValues(item);

            try
            {
                await _dynamicsClient.Tiedhouseconnections.UpdateAsync(tiedHouseId.ToString(), tiedHouse);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating tied house connections");
                throw new Exception("Unable to update tied house connections");
            }


            return new JsonResult(tiedHouse.ToViewModel());
        }
    }
}
