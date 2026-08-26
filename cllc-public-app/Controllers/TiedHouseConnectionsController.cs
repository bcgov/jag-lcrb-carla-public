using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Repositories;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class TiedHouseConnectionsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly TiedHouseConnectionsRepository _tiedHouseConnectionsRepository;

        public TiedHouseConnectionsController(
            ILoggerFactory loggerFactory,
            IHttpContextAccessor httpContextAccessor,
            TiedHouseConnectionsRepository tiedHouseConnectionsRepository
        )
        {
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
        public async Task<ActionResult<IEnumerable<TiedHouseConnection>>> GetLiquorTiedHouseConnectionsForUser(string accountId)
        {
            try
            {
                UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
                var accountIdForFilter = accountId ?? userSettings.AccountId;

                _logger.LogDebug($"GetLiquorTiedHouseConnectionsForUser. AccountId = {accountIdForFilter}.");

                var result = await _tiedHouseConnectionsRepository.GetLiquorTiedHouseConnectionsForUser(accountIdForFilter);
                return new JsonResult(result);
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
        public async Task<ActionResult<TiedHouseConnection>> GetCannabisTiedHouseConnectionForUser(string accountId)
        {
            try
            {
                UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
                var accountIdForFilter = accountId ?? userSettings.AccountId;

                _logger.LogDebug($"GetCannabisTiedHouseConnectionForUser. AccountId = {accountIdForFilter}.");

                var result = await _tiedHouseConnectionsRepository.GetCannabisTiedHouseConnectionForUser(accountIdForFilter);
                return new JsonResult(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error fetching cannabis tied house connection.");
                throw new Exception("Failed to fetch cannabis tied house connection.");
            }
        }

        /// <summary>
        /// Gets all liquor tied house connections for an application.
        /// </summary>
        [HttpGet("liquor/application/{applicationId}")]
        public async Task<JsonResult> GetLiquorTiedHouseConnectionsForApplication(string applicationId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            _logger.LogDebug(
                $"GetLiquorTiedHouseConnectionsForApplication. ApplicationId = {applicationId}. AccountId = {userSettings.AccountId}."
            );

            try
            {
                var result = await _tiedHouseConnectionsRepository.GetLiquorTiedHouseConnectionsForApplication(
                    applicationId,
                    userSettings.AccountId
                );

                return new JsonResult(result);
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
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error adding tied house connections");
                throw new Exception("Failed to add tied house connection");
            }
        }

        /// <summary>
        /// Creates new liquor tied house connections for a user.
        /// </summary>
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
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error adding tied house connections for user");
                throw new Exception("Failed to add tied house connection");
            }
        }

        /// <summary>
        /// Creates or Updates the singleton cannabis tied house connection.
        /// </summary>
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
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error upserting cannabis tied house connection");
                throw new Exception("Failed to upsert cannabis tied house connection");
            }
        }

        /// <summary>
        /// Updates an existing cannabis tied house connection for a user.
        /// </summary>
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
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error updating cannabis tied house connection");
                throw new Exception("Failed to update cannabis tied house connection");
            }
        }
    }
}
