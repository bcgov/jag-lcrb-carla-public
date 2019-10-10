using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
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
        private readonly ILogger _logger;

        public TiedHouseConnectionsController(ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(TiedHouseConnectionsController));
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
            catch (HttpOperationException odee)
            {
                _logger.LogError(odee, "Error updating tied house connections");
                throw new Exception("Unable to update tied house connections");
            }


            return new JsonResult(tiedHouse.ToViewModel());
        }
    }
}
