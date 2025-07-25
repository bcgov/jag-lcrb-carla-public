using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Repositories;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;

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
        private readonly TiedHouseConnectionsRepository _tiedHouseConnectionsRepository;

        public TiedHouseConnectionsController(
            ILoggerFactory loggerFactory,
            IDynamicsClient dynamicsClient,
            IHttpContextAccessor httpContextAccessor,
            TiedHouseConnectionsRepository tiedHouseConnectionsRepository
        )
        {
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(TiedHouseConnectionsController));
            _httpContextAccessor = httpContextAccessor;
            _tiedHouseConnectionsRepository = tiedHouseConnectionsRepository;
        }

        /// <summary>
        /// Get all Tied House Connections for a user.
        /// If `accountId` is provided, it will return connections for that account.
        /// If `accountId` is not provided, it will return connections for the current logged in user's account.
        /// </summary>
        /// <param name="accountId">An optional accountId to filter results by</param>
        /// <returns>A list of tied house connections</returns>
        [HttpGet("user/{accountId?}")]
        [ProducesResponseType(typeof(IEnumerable<TiedHouseConnection>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<TiedHouseConnection>> GetAllTiedHouseConnectionsForUser(string accountId)
        {
            try
            {
                IEnumerable<MicrosoftDynamicsCRMadoxioTiedhouseconnection> tiedHouseConnections = null;

                UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

                // Use `accountId` if provided, otherwise use the current logged in user's account Id
                var accountIdForFilter = accountId != null ? accountId : userSettings.AccountId;

                _logger.LogDebug($"GetAllTiedHouseConnectionsForUser. AccountId = {accountIdForFilter}.");

                var result = _tiedHouseConnectionsRepository.GetAllTiedHouseConnectionsForUser(accountIdForFilter);

                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Failed to fetch tied house connections.");
                throw new HttpOperationException("Failed to fetch tied house connections.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch tied house connections.");
                throw new Exception("Failed to fetch tied house connections.");
            }
        }

        /// <summary>
        /// Get the count of all "existing" Tied House Connections for a user.
        /// If `accountId` is provided, it will return connections for that account.
        /// If `accountId` is not provided, it will return connections for the current logged in user's account.
        /// </summary>
        /// <param name="accountId">An optional accountId to filter results by</param>
        /// <returns>The count of "existing" tied house connections</returns>
        [HttpGet("user/existing/count/{accountId?}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<int> GetExistingTiedHouseConnectionsCountForUser(string accountId)
        {
            try
            {
                UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

                // Use `accountId` if provided, otherwise use the current logged in user's account Id
                var accountIdForFilter = accountId != null ? accountId : userSettings.AccountId;

                _logger.LogDebug($"GetExistingTiedHouseConnectionsCountForUser. AccountId = {accountIdForFilter}.");

                int result = _tiedHouseConnectionsRepository.GetExistingTiedHouseConnectionsCountForUser(
                    accountIdForFilter
                );

                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Failed to fetch existing tied house connections count.");
                throw new HttpOperationException("Failed to fetch existing tied house connections count.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch existing tied house connections count.");
                throw new Exception("Failed to fetch existing tied house connections count.");
            }
        }

        [HttpGet("application/{applicationId?}")]
        public JsonResult GetAllTiedHouseConnections(string applicationId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var result = new List<ViewModels.TiedHouseConnection>();
            var supersededbyIds = new List<string>();
            IEnumerable<MicrosoftDynamicsCRMadoxioTiedhouseconnection> tiedHouseConnections = null;
            string accountFilter =
                $"(_adoxio_accountid_value eq {userSettings.AccountId} and statuscode eq {(int)TiedHouseStatusCode.Existing})";
            if (!String.IsNullOrEmpty(applicationId))
            {
                /* If updating saved application
                 * only marked for removed connections if they are updating existing connection*/
                accountFilter =
                    accountFilter
                    + $" or (_adoxio_application_value eq {applicationId} and (_adoxio_supersededby_value ne null or (_adoxio_supersededby_value eq null and adoxio_markedforremoval ne 1)))";
            }
            _logger.LogDebug("Account filter = " + accountFilter);

            try
            {
                var expand = new List<string> { "adoxio_adoxio_tiedhouseconnection_adoxio_licence" };
                tiedHouseConnections = _dynamicsClient
                    .Tiedhouseconnections.Get(filter: accountFilter, expand: expand)
                    .Value;

                foreach (var tiedHouse in tiedHouseConnections)
                {
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
            /* If connection is loaded that is updating existing record
             Do not show existing record*/
            result = result.Where(s => !supersededbyIds.Contains(s.id)).ToList();
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

            MicrosoftDynamicsCRMadoxioTiedhouseconnection res = await _dynamicsClient.GetTiedHouseConnectionById(
                tiedHouseId
            );
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

        [HttpPost("application/{applicationId}")]
        public async Task<IActionResult> AddTiedHouseConnectionToApplication(
            [FromBody] ViewModels.TiedHouseConnection tiedHouseConnection,
            string applicationId
        )
        {
            MicrosoftDynamicsCRMadoxioTiedhouseconnection adoxioTiedHouseConnection =
                new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

            adoxioTiedHouseConnection.CopyValues(tiedHouseConnection);
            try
            {
                if (tiedHouseConnection.ApplicationId == applicationId)
                {
                    if (tiedHouseConnection.MarkedForRemoval == true && tiedHouseConnection.SupersededById == null)
                    {
                        await _dynamicsClient.Tiedhouseconnections.DeleteAsync(
                            adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid
                        );
                    }
                    else
                    {
                        await _dynamicsClient.Tiedhouseconnections.UpdateAsync(
                            adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid,
                            adoxioTiedHouseConnection
                        );
                        await RemoveAndAddAssociateLicenses(
                            tiedHouseConnection.AssociatedLiquorLicense.Select(x => x.Id).ToList(),
                            adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid
                        );
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(tiedHouseConnection.id))
                    {
                        adoxioTiedHouseConnection.SupersededByOdataBind =
                            $"/adoxio_tiedhouseconnections({tiedHouseConnection.id})";
                    }
                    adoxioTiedHouseConnection.ApplicationOdataBind = $"/adoxio_applications({applicationId})";

                    adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid = null;

                    var response = await _dynamicsClient.Tiedhouseconnections.CreateAsync(adoxioTiedHouseConnection);

                    await AssociateLicenses(
                        tiedHouseConnection.AssociatedLiquorLicense.Select(x => x.Id).ToList(),
                        response.AdoxioTiedhouseconnectionid
                    );
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error adding tied house connections");
                throw new Exception("Unable to add tied house connection");
            }

            return new JsonResult(adoxioTiedHouseConnection);
        }

        private async Task RemoveAndAddAssociateLicenses(List<string> licenses, string tiedHouseId)
        {
            var result = new List<ViewModels.TiedHouseConnection>();
            IEnumerable<MicrosoftDynamicsCRMadoxioTiedhouseconnection> tiedHouseConnections = null;
            string filter = "adoxio_tiedhouseconnectionid eq " + tiedHouseId;
            var expand = new List<string> { "adoxio_adoxio_tiedhouseconnection_adoxio_licence" };
            var select = new List<string>() { "adoxio_adoxio_tiedhouseconnection_adoxio_licence" };
            var tiedHouseConnection = _dynamicsClient
                .Tiedhouseconnections.Get(filter: filter, select: select, expand: expand)
                .Value.FirstOrDefault();

            var licencesIds = tiedHouseConnection.Adoxio_Adoxio_TiedHouseConnection_Adoxio_Licence.Select(l =>
                l.AdoxioLicencesid
            );
            var hasLicencesBeenUpdated = !licencesIds.OrderBy(x => x).SequenceEqual(licenses.OrderBy(x => x));
            if (hasLicencesBeenUpdated)
            {
                foreach (var id in licencesIds)
                {
                    _dynamicsClient
                        .Tiedhouseconnections.DeleteReferenceWithHttpMessagesAsync(
                            tiedHouseId,
                            "adoxio_adoxio_tiedhouseconnection_adoxio_licence",
                            id
                        )
                        .GetAwaiter()
                        .GetResult();
                }
                ;

                await AssociateLicenses(licenses, tiedHouseId);
            }
        }

        private async Task AssociateLicenses(List<string> licenses, string tiedHouseId)
        {
            foreach (var licenceId in licenses)
            {
                var odataId = new Odataid
                {
                    OdataidProperty = _dynamicsClient.GetEntityURI("adoxio_licenceses", licenceId)
                };

                await _dynamicsClient.Tiedhouseconnections.AddReferenceWithHttpMessagesAsync(
                    tiedHouseId,
                    "adoxio_adoxio_tiedhouseconnection_adoxio_licence",
                    odataid: odataId
                );
            }
        }
    }
}
