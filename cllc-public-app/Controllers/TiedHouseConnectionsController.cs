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
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;

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
        public ActionResult<IEnumerable<TiedHouseConnection>> GetLiquorTiedHouseConnectionsForUser(string accountId)
        {
            try
            {
                UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

                // Use `accountId` if provided, otherwise use the current logged in user's account Id
                var accountIdForFilter = accountId != null ? accountId : userSettings.AccountId;

                _logger.LogDebug($"GetLiquorTiedHouseConnectionsForUser. AccountId = {accountIdForFilter}.");

                var result = _tiedHouseConnectionsRepository.GetLiquorTiedHouseConnectionsForUser(accountIdForFilter);

                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error fetching liquor tied house connections.");
                _logger.LogDebug($"Request: {JsonConvert.SerializeObject(httpOperationException.Request)}");
                _logger.LogDebug($"Response: {JsonConvert.SerializeObject(httpOperationException.Response)}");
                throw new Exception("Failed to fetch liquor tied house connections.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error fetching liquor tied house connections.");
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
                _logger.LogError(httpOperationException, "Error fetching cannabis tied house connection.");
                _logger.LogDebug($"Request: {JsonConvert.SerializeObject(httpOperationException.Request)}");
                _logger.LogDebug($"Response: {JsonConvert.SerializeObject(httpOperationException.Response)}");
                throw new Exception("Failed to fetch cannabis tied house connection.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error fetching cannabis tied house connection.");
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
                    "Error fetching existing liquor tied house connections count."
                );
                throw new Exception("Failed to fetch existing liquor tied house connections count.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error fetching existing liquor tied house connections count.");
                throw new Exception("Failed to fetch existing liquor tied house connections count.");
            }
        }

        /// <summary>
        /// Gets all liquor tied house connections for an application.
        /// </summary>
        /// <remarks>
        /// This includes liquor tied house connections that are associated with the user account, as well as those that
        /// are associated with the application.
        /// </remarks>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [HttpGet("liquor/application/{applicationId}")]
        public JsonResult GetLiquorTiedHouseConnectionsForApplication(string applicationId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            _logger.LogDebug(
                $"GetLiquorTiedHouseConnectionsForApplication. ApplicationId = {applicationId}. AccountId = {userSettings.AccountId}."
            );

            try
            {
                var result = _tiedHouseConnectionsRepository.GetLiquorTiedHouseConnectionsForApplication(
                    applicationId,
                    userSettings.AccountId
                );

                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating tied house connections");
                _logger.LogDebug($"Request: {JsonConvert.SerializeObject(httpOperationException.Request)}");
                _logger.LogDebug($"Response: {JsonConvert.SerializeObject(httpOperationException.Response)}");
                throw new Exception("Unable to add tied house connection");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error updating tied house connections");
                throw new Exception("Failed to add tied house connection");
            }
        }

        /// <summary>
        /// Creates a new liquor tied house connection for an application.
        /// </summary>
        /// <remarks>
        /// Liquor tied house connections are not updated directly. Instead, a new record is created with the new
        /// data, and the old record is updated to set the `SupersededBy` field to the new record's ID. In this way,
        /// the history of changes to the tied house connection record is preserved.
        /// </remarks>
        /// <param name="tiedHouseConnection"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [HttpPost("liquor/application/{applicationId}")]
        public async Task<IActionResult> AddLiquorTiedHouseConnectionToApplication(
            [FromBody] TiedHouseConnection tiedHouseConnection,
            string applicationId
        )
        {
            _logger.LogDebug($"AddLiquorTiedHouseConnectionToApplication. ApplicationId = {applicationId}.");

            try
            {
                var result = await _tiedHouseConnectionsRepository.AddLiquorTiedHouseConnectionToApplication(
                    tiedHouseConnection,
                    applicationId
                );

                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error adding tied house connections");
                _logger.LogDebug($"Request: {JsonConvert.SerializeObject(httpOperationException.Request)}");
                _logger.LogDebug($"Response: {JsonConvert.SerializeObject(httpOperationException.Response)}");
                throw new Exception("Unable to add tied house connection");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error adding tied house connections");
                throw new Exception("Failed to add tied house connection");
            }
        }

        /// <summary>
        /// Creates new liquor tied house connections for a user.
        /// </summary>
        /// <remarks>
        /// Business Rules - This endpoint should only be called if:
        /// <br/>
        /// <list type="bullet">
        ///   <item><description>The user does not have any existing tied house connections of type <c>Liquor</c>.</description></item>
        ///   <item><description>The user does not have any approved applications, of any type.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="tiedHouseConnection">The tied house connection data to create.</param>
        /// <param name="accountId">The ID of the user's account.</param>
        /// <returns>The created tied house connection.</returns>
        [HttpPost("liquor/user/{accountId}")]
        public async Task<IActionResult> AddLiquorTiedHouseConnectionToUser(
            [FromBody] TiedHouseConnection tiedHouseConnection,
            string accountId
        )
        {
            _logger.LogDebug($"AddLiquorTiedHouseConnectionToUser. AccountId = {accountId}.");

            try
            {
                var result = await _tiedHouseConnectionsRepository.AddLiquorTiedHouseConnectionToUser(
                    tiedHouseConnection,
                    accountId
                );

                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error adding tied house connections for user");
                _logger.LogDebug($"Request: {JsonConvert.SerializeObject(httpOperationException.Request)}");
                _logger.LogDebug($"Response: {JsonConvert.SerializeObject(httpOperationException.Response)}");
                throw new Exception("Failed to add tied house connection for user");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error adding tied house connections for user");
                throw new Exception("Failed to add tied house connection");
            }
        }

        /// <summary>
        /// Creates or Updates the singleton cannabis tied house connection.
        /// </summary>
        /// <remarks>
        /// If a cannabis tied house connection already exists for the user, it will update and return that existing
        /// record.
        /// <br/>
        /// If no cannabis tied house connection exists, it will create a new one and return it.
        /// </remarks>
        /// <param name="accountId">The ID of the account associated with the cannabis tied house connection.</param>
        /// <param name="incomingTiedHouseConnectionRecord">Optional cannabis tied house connection record used to
        /// create or update the record.</param>
        /// <returns>The cannabis tied house connection record.</returns>
        /// <exception cref="Exception">Thrown when there is an error creating or updating the record.</exception>
        [HttpPost("cannabis/{accountId}")]
        public async Task<ActionResult<TiedHouseConnection>> UpsertCannabisTiedHouseConnectionForUser(
            string accountId,
            [FromBody] TiedHouseConnection tiedHouseConnection
        )
        {
            _logger.LogDebug($"AddCannabisTiedHouseConnectionToUser. AccountId = {accountId}.");

            try
            {
                var result = await _tiedHouseConnectionsRepository.UpsertCannabisTiedHouseConnection(
                    accountId,
                    tiedHouseConnection
                );

                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error upserting cannabis tied house connection");
                _logger.LogDebug($"Request: {JsonConvert.SerializeObject(httpOperationException.Request)}");
                _logger.LogDebug($"Response: {JsonConvert.SerializeObject(httpOperationException.Response)}");
                throw new Exception("Failed to upsert cannabis tied house connection");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error upserting cannabis tied house connection");
                throw new Exception("Failed to upsert cannabis tied house connection");
            }
        }

        /// <summary>
        /// Updates an existing cannabis tied house connection for a user.
        /// </summary>
        /// <param name="tiedHouseConnectionId"></param>
        /// <param name="tiedHouseConnection"></param>
        /// <returns>The updating tied house connection</returns>
        /// <exception cref="Exception">Thrown when there is an error updating the record.</exception>
        [HttpPut("cannabis/{tiedHouseConnectionId}")]
        public async Task<ActionResult<TiedHouseConnection>> UpdateCannabisTiedHouseConnectionForUser(
            string tiedHouseConnectionId,
            [FromBody] TiedHouseConnection tiedHouseConnection
        )
        {
            try
            {
                var result = await _tiedHouseConnectionsRepository.UpdateCannabisTiedHouseConnection(
                    tiedHouseConnectionId,
                    tiedHouseConnection
                );

                return new JsonResult(result);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error updating cannabis tied house connection");
                _logger.LogDebug($"Request: {JsonConvert.SerializeObject(httpOperationException.Request)}");
                _logger.LogDebug($"Response: {JsonConvert.SerializeObject(httpOperationException.Response)}");
                throw new Exception("Failed to update cannabis tied house connection");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error updating cannabis tied house connection");
                throw new Exception("Failed to update cannabis tied house connection");
            }
        }
    }
}
