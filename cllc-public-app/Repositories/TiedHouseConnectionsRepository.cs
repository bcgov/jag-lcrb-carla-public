using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Public.Repositories
{
    /// <summary>
    /// Repository for managing Tied House Connections.
    /// </summary>
    public class TiedHouseConnectionsRepository
    {
        private readonly IDynamicsClient _dynamicsClient;
        private readonly ILogger _logger;

        public TiedHouseConnectionsRepository(IDynamicsClient dynamicsClient, ILoggerFactory loggerFactory)
        {
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(TiedHouseConnectionsRepository));
        }

        /// <summary>
        /// Get all Tied House Connections for a user.
        /// </summary>
        /// <param name="accountId">The accountId of the user to filter results by</param>
        /// <returns>A list of tied house connections</returns>
        /// /// <exception cref="Exception">Thrown when there is an error fetching the records.</exception>
        public IEnumerable<TiedHouseConnection> GetAllTiedHouseConnectionsForUser(string accountId)
        {
            try
            {
                IEnumerable<MicrosoftDynamicsCRMadoxioTiedhouseconnection> tiedHouseConnections = null;

                _logger.LogDebug($"GetAllTiedHouseConnectionsForUser. AccountId = {accountId}.");

                var andFilterConditions = new List<string>
                {
                    $"_adoxio_accountid_value eq {accountId}",
                    $"statuscode eq {(int)TiedHouseStatusCode.Existing}",
                    // "type eq 'Liquor'", // TODO: tiedhouse - Update this condition when the field is added to dynamics, currently does not exist.
                    "statecode eq 0"
                };
                var filter = string.Join(" and ", andFilterConditions);

                var expand = new List<string> { "adoxio_adoxio_tiedhouseconnection_adoxio_licence" };

                tiedHouseConnections = _dynamicsClient.Tiedhouseconnections.Get(filter: filter, expand: expand).Value;

                var result = tiedHouseConnections.Select(item => item.ToViewModel()).ToList();

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch tied house connections.");
                throw new Exception("Failed to fetch tied house connections.");
            }
        }

        /// <summary>
        /// Get the count of all "existing" Tied House Connections for a user.
        /// </summary>
        /// <param name="accountId">The accountId of the user to filter results by</param>
        /// <returns>The count of "existing" tied house connections</returns>
        /// <exception cref="Exception">Thrown when there is an error fetching the records.</exception>
        public int GetExistingTiedHouseConnectionsCountForUser(string accountId)
        {
            try
            {
                var andFilterConditions = new List<string>
                {
                    $"_adoxio_accountid_value eq {accountId}",
                    $"statuscode eq {(int)TiedHouseStatusCode.Existing}",
                    // "type eq 'Liquor'", // TODO: tiedhouse - Update this condition when the field is added to dynamics, currently does not exist.
                    "statecode eq 0"
                };
                var filter = string.Join(" and ", andFilterConditions);

                var response = _dynamicsClient.Tiedhouseconnections.Get(filter: filter, top: 1, count: true);

                int result = int.TryParse(response?.Count, out var parsedCount) ? parsedCount : 0;

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch existing tied house connections count.");
                throw new Exception("Failed to fetch existing tied house connections count.");
            }
        }

        /// <summary>
        /// Gets the cannabis tied house connection for a user.
        /// A user should not have more than one cannabis tied house connection.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>The cannabis tied house connection record or `null` if no record exists</returns>
        /// <exception cref="Exception">Thrown when there is an error fetching the records.</exception>
        public TiedHouseConnection GetCannabisTiedHouseConnectionForUser(string accountId)
        {
            try
            {
                _logger.LogDebug($"GetCannabisTiedHouseConnectionForUser. AccountId = {accountId}.");

                var andFilterConditions = new List<string>
                {
                    $"_adoxio_accountid_value eq {accountId}",
                    $"statuscode eq {(int)TiedHouseStatusCode.Existing}",
                    // "type eq 'Cannabis'", // TODO: tiedhouse - Update this condition when the field is added to dynamics, currently does not exist.
                    "statecode eq 0"
                };
                var filter = string.Join(" and ", andFilterConditions);

                var expand = new List<string> { "adoxio_adoxio_tiedhouseconnection_adoxio_licence" };

                var tiedHouseConnections = _dynamicsClient.Tiedhouseconnections.Get(filter: filter, expand: expand);

                if (tiedHouseConnections.Value.Count > 1)
                {
                    _logger.LogError(
                        $"Found {tiedHouseConnections.Value.Count} cannabis tied house connections for account {accountId}. Expecting only one. This may indicate a data issue."
                    );

                    return null;
                }

                if (tiedHouseConnections.Value.Count == 0)
                {
                    _logger.LogDebug($"No cannabis tied house connection found for account {accountId}.");
                    return null;
                }

                return tiedHouseConnections.Value[0].ToViewModel();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to fetch existing cannabis tied house connection.");
                throw new Exception("Failed to fetch existing cannabis tied house connection.");
            }
        }
    }
}
