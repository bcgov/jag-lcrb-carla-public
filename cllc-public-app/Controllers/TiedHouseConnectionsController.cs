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
        /// Get all liquor Tied House Connections for a user.
        /// If `accountId` is provided, it will return connections for that account.
        /// If `accountId` is not provided, it will return connections for the current logged in user's account.
        /// </summary>
        /// <param name="accountId">An optional accountId to filter results by</param>
        /// <returns>A list of tied house connections</returns>
        [HttpGet("user/liquor/{accountId?}")]
        [ProducesResponseType(typeof(IEnumerable<TiedHouseConnection>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<TiedHouseConnection>> GetAllLiquorTiedHouseConnectionsForUser(string accountId)
        {
            try
            {
                UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

                // Use `accountId` if provided, otherwise use the current logged in user's account Id
                var accountIdForFilter = accountId != null ? accountId : userSettings.AccountId;

                _logger.LogDebug($"GetAllLiquorTiedHouseConnectionsForUser. AccountId = {accountIdForFilter}.");

                var result = _tiedHouseConnectionsRepository.GetAllLiquorTiedHouseConnectionsForUser(
                    accountIdForFilter
                );

                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Failed to fetch liquor tied house connections.");
                throw new HttpOperationException("Failed to fetch liquor tied house connections.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch liquor tied house connections.");
                throw new Exception("Failed to fetch liquor tied house connections.");
            }
        }

        /// <summary>
        /// Get the singleton cannabis Tied House Connection for a user.
        /// If `accountId` is provided, it will return connections for that account.
        /// If `accountId` is not provided, it will return connections for the current logged in user's account.
        /// </summary>
        /// <param name="accountId">An optional accountId to filter results by</param>
        /// <returns>A single cannabis tied house connection</returns>
        [HttpGet("user/cannabis/{accountId?}")]
        [ProducesResponseType(typeof(IEnumerable<TiedHouseConnection>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TiedHouseConnection> GetCannabisTiedHouseConnectionForUser(string accountId)
        {
            try
            {
                UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

                // Use `accountId` if provided, otherwise use the current logged in user's account Id
                var accountIdForFilter = accountId != null ? accountId : userSettings.AccountId;

                _logger.LogDebug($"GetCannabisTiedHouseConnectionForUser. AccountId = {accountIdForFilter}.");

                var result = _tiedHouseConnectionsRepository.GetCannabisTiedHouseConnectionForUser(accountIdForFilter);

                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Failed to fetch cannabis tied house connection.");
                throw new HttpOperationException("Failed to fetch cannabis tied house connection.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch cannabis tied house connection.");
                throw new Exception("Failed to fetch cannabis tied house connection.");
            }
        }

        /// <summary>
        /// Get the count of all "existing" liquor Tied House Connections for a user.
        /// If `accountId` is provided, it will return connections for that account.
        /// If `accountId` is not provided, it will return connections for the current logged in user's account.
        /// </summary>
        /// <param name="accountId">An optional accountId to filter results by</param>
        /// <returns>The count of "existing" tied house connections</returns>
        [HttpGet("user/liquor/existing/count/{accountId?}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<int> GetExistingLiquorTiedHouseConnectionsCountForUser(string accountId)
        {
            try
            {
                UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

                // Use `accountId` if provided, otherwise use the current logged in user's account Id
                var accountIdForFilter = accountId != null ? accountId : userSettings.AccountId;

                _logger.LogDebug(
                    $"GetExistingLiquorTiedHouseConnectionsCountForUser. AccountId = {accountIdForFilter}."
                );

                int result = _tiedHouseConnectionsRepository.GetExistingLiquorTiedHouseConnectionsCountForUser(
                    accountIdForFilter
                );

                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(
                    httpOperationException,
                    "Failed to fetch existing liquor tied house connections count."
                );
                throw new HttpOperationException("Failed to fetch existing liquor tied house connections count.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch existing liquor tied house connections count.");
                throw new Exception("Failed to fetch existing liquor tied house connections count.");
            }
        }

        [HttpGet("liquor/application/{applicationId?}")]
        public JsonResult GetAllLiquorTiedHouseConnections(string applicationId)
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
                    + $" or (_adoxio_application_value eq {applicationId} and adoxio_categorytype eq {(int)TiedHouseCategoryType.Liquor} and (_adoxio_supersededby_value ne null or (_adoxio_supersededby_value eq null and adoxio_markedforremoval ne 1)))";
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

        [HttpPost("liquor/application/{applicationId}")]
        public async Task<IActionResult> AddLiquorTiedHouseConnectionToApplication(
            [FromBody] ViewModels.TiedHouseConnection tiedHouseConnection,
            string applicationId
        )
        {
            MicrosoftDynamicsCRMadoxioTiedhouseconnection adoxioTiedHouseConnection =
                new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

            adoxioTiedHouseConnection.CopyValues(tiedHouseConnection);

            adoxioTiedHouseConnection.AdoxioCategoryType = (int)TiedHouseCategoryType.Liquor;
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

        [HttpPost("cannabis/{accountId}")]
        public async Task<ActionResult<TiedHouseConnection>> CreateCannabisTiedHouseConnectionForUser(
            string accountId,
            [FromBody] TiedHouseConnection tiedHouseConnection
        )
        {
            try
            {
                var createdCannabisTiedHouseConnection =
                    await _tiedHouseConnectionsRepository.CreateCannabisTiedHouseConnection(
                        accountId,
                        tiedHouseConnection
                    );

                return new JsonResult(createdCannabisTiedHouseConnection);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Failed to create cannabis tied house connection");
                throw new Exception("Failed to create cannabis tied house connection");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to create cannabis tied house connection");
                throw new Exception("Failed to create cannabis tied house connection");
            }
        }

        [HttpPost("cannabis/{tiedHouseConnectionId}")]
        public async Task<ActionResult<TiedHouseConnection>> UpdateCannabisTiedHouseConnectionForUser(
            string tiedHouseConnectionId,
            [FromBody] TiedHouseConnection tiedHouseConnection
        )
        {
            try
            {
                var createdCannabisTiedHouseConnection =
                    await _tiedHouseConnectionsRepository.UpdateCannabisTiedHouseConnection(
                        tiedHouseConnectionId,
                        tiedHouseConnection
                    );

                return new JsonResult(createdCannabisTiedHouseConnection);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Failed to update cannabis tied house connection");
                throw new Exception("Failed to update cannabis tied house connection");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to update cannabis tied house connection");
                throw new Exception("Failed to update cannabis tied house connection");
            }
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
