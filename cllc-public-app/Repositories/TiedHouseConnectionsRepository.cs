using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Public.Repositories
{
    /// <summary>
    /// Repository for managing the dynamics calls and related business logic for the Tied House Connections entity.
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
        /// Get all liquor Tied House Connections for a user.
        /// </summary>
        /// <param name="accountId">The accountId of the user to filter results by</param>
        /// <returns>A list of tied house connections</returns>
        /// /// <exception cref="Exception">Thrown when there is an error fetching the records.</exception>
        public IEnumerable<TiedHouseConnection> GetAllLiquorTiedHouseConnectionsForUser(string accountId)
        {
            _logger.LogDebug($"GetAllLiquorTiedHouseConnectionsForUser. AccountId = {accountId}.");

            try
            {
                IEnumerable<MicrosoftDynamicsCRMadoxioTiedhouseconnection> tiedHouseConnections = null;

                var andFilterConditions = new List<string>
                {
                    $"_adoxio_accountid_value eq {accountId}",
                    $"statuscode eq {(int)TiedHouseStatusCode.Existing}",
                    $"adoxio_categorytype eq {(int)TiedHouseCategoryType.Liquor}",
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
                throw;
            }
        }

        /// <summary>
        /// Get the count of all "existing" liquor Tied House Connections for a user.
        /// </summary>
        /// <param name="accountId">The accountId of the user to filter results by</param>
        /// <returns>The count of "existing" tied house connections</returns>
        /// <exception cref="Exception">Thrown when there is an error fetching the records.</exception>
        public int GetExistingLiquorTiedHouseConnectionsCountForUser(string accountId)
        {
            _logger.LogDebug($"GetExistingLiquorTiedHouseConnectionsCountForUser. AccountId = {accountId}.");

            try
            {
                var andFilterConditions = new List<string>
                {
                    $"_adoxio_accountid_value eq {accountId}",
                    $"statuscode eq {(int)TiedHouseStatusCode.Existing}",
                    $"categorytype eq {(int)TiedHouseCategoryType.Liquor}",
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
                throw;
            }
        }

        /// <summary>
        /// Gets the singleton cannabis tied house connection for a user.
        /// A user should not have more than one cannabis tied house connection.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>The cannabis tied house connection record or `null` if no record exists</returns>
        /// <exception cref="Exception">Thrown when there is an error fetching the records.</exception>
        public TiedHouseConnection GetCannabisTiedHouseConnectionForUser(string accountId)
        {
            _logger.LogDebug($"GetCannabisTiedHouseConnectionForUser. AccountId = {accountId}.");

            try
            {
                var andFilterConditions = new List<string>
                {
                    $"_adoxio_accountid_value eq {accountId}",
                    $"statuscode eq {(int)TiedHouseStatusCode.Existing}",
                    $"adoxio_categorytype eq {(int)TiedHouseCategoryType.Cannabis}",
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
                throw;
            }
        }

        /// <summary>
        /// Creates the cannabis tied house connection.
        /// </summary>
        /// <param name="accountId">The ID of the account associated with the cannabis tied house connection.</param>
        /// <param name="incomingTiedHouseConnectionRecord">Optional cannabis tied house connection record used to
        /// initialize the new record. If not provided, an empty record will be created.</param>
        /// <returns>The created cannabis tied house connection record.</returns>
        /// <exception cref="Exception">Thrown when there is an error creating the record.</exception>
        public async Task<TiedHouseConnection> CreateCannabisTiedHouseConnection(
            string accountId,
            TiedHouseConnection incomingTiedHouseConnectionRecord = null
        )
        {
            _logger.LogDebug($"CreateCannabisTiedHouseConnection. AccountId = {accountId}.");

            try
            {
                TiedHouseConnection existingCannabisTiedHouseConnectionRecord = GetCannabisTiedHouseConnectionForUser(
                    accountId
                );

                if (existingCannabisTiedHouseConnectionRecord != null)
                {
                    // Only 1 cannabis tied house connection should exist per user account.
                    return existingCannabisTiedHouseConnectionRecord;
                }

                var newCannabisTiedHouseConnectionRecord = new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

                if (incomingTiedHouseConnectionRecord != null)
                {
                    newCannabisTiedHouseConnectionRecord.CopyValues(incomingTiedHouseConnectionRecord);
                }

                // Associate the new tied house connection with the account
                newCannabisTiedHouseConnectionRecord.AccountODataBind = _dynamicsClient.GetEntityURI(
                    "accounts",
                    accountId
                );

                // Ensure the tied house connection is of type (category) "cannabis"
                newCannabisTiedHouseConnectionRecord.AdoxioCategoryType = (int)TiedHouseCategoryType.Cannabis;

                var createdCannabisTiedHouseConnectionRecord = await _dynamicsClient.Tiedhouseconnections.CreateAsync(
                    newCannabisTiedHouseConnectionRecord
                );

                return createdCannabisTiedHouseConnectionRecord.ToViewModel();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error creating cannabis tied house connection");
                throw;
            }
        }

        /// <summary>
        /// Updates the cannabis tied house connection by fully replacing the existing record.
        /// This method does not perform a partial update (patch); instead, all existing values are overwritten
        /// with the values from the provided record.
        /// </summary>
        /// <param name="tiedHouseConnectionId">The ID of the cannabis tied house connection to update.</param>
        /// <param name="incomingTiedHouseConnectionRecord">The updated cannabis tied house connection record.</param>
        /// <returns>The updated cannabis tied house connection record.</returns>
        /// <exception cref="Exception">Thrown when there is an error updating the record.</exception>
        public async Task<TiedHouseConnection> UpdateCannabisTiedHouseConnection(
            string tiedHouseConnectionId,
            TiedHouseConnection incomingTiedHouseConnectionRecord
        )
        {
            _logger.LogDebug($"UpdateCannabisTiedHouseConnection. TiedHouseConnectionId = {tiedHouseConnectionId}.");

            try
            {
                Guid tiedHouseConnectionGuid = new Guid(tiedHouseConnectionId);

                MicrosoftDynamicsCRMadoxioTiedhouseconnection existingTiedHouseConnectionRecord =
                    await _dynamicsClient.GetTiedHouseConnectionById(tiedHouseConnectionGuid);

                if (existingTiedHouseConnectionRecord == null)
                {
                    throw new Exception($"Tied House Connection with ID {tiedHouseConnectionId} could not be found.");
                }

                var updatedTiedHouseConnectionRecord = new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

                updatedTiedHouseConnectionRecord.CopyValues(incomingTiedHouseConnectionRecord);

                await _dynamicsClient.Tiedhouseconnections.UpdateAsync(
                    tiedHouseConnectionId.ToString(),
                    updatedTiedHouseConnectionRecord
                );

                return updatedTiedHouseConnectionRecord.ToViewModel();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error updating cannabis tied house connection");
                throw;
            }
        }

        /// <summary>
        /// Deletes a tied house connection by ID.
        /// </summary>
        /// <param name="tiedHouseConnectionId">The ID of the tied house connection to delete.</param>
        /// <returns>void</returns>
        /// <exception cref="Exception">Thrown when there is an error deleting the record.</exception>
        public void DeleteTiedHouseConnectionById(string tiedHouseConnectionId)
        {
            _logger.LogDebug($"DeleteTiedHouseConnectionById. TiedHouseConnectionId = {tiedHouseConnectionId}.");

            try
            {
                _dynamicsClient.Tiedhouseconnections.Delete(tiedHouseConnectionId);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error deleting tied house connection");
                throw;
            }
        }
    }
}
