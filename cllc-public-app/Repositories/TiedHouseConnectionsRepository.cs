extern alias DV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Public.Repositories
{
    /// <summary>
    /// Repository for managing Dataverse calls and related business logic for the Tied House Connections entity.
    /// </summary>
    public class TiedHouseConnectionsRepository
    {
        private readonly IDataverseClient _dataverse;
        private readonly ILogger _logger;

        public TiedHouseConnectionsRepository(IDataverseClient dataverse, ILoggerFactory loggerFactory)
        {
            _dataverse = dataverse;
            _logger = loggerFactory.CreateLogger(typeof(TiedHouseConnectionsRepository));
        }

        /// <summary>
        /// Fetch a single Tied House Connection by its ID, including related licences.
        /// </summary>
        public async Task<TiedHouseConnection> GetTiedHouseConnectionById(string tiedHouseConnectionId)
        {
            _logger.LogDebug($"GetTiedHouseConnectionById. TiedHouseConnectionId = {tiedHouseConnectionId}.");

            var entity = await _dataverse.GetTiedHouseConnectionByIdAsync(tiedHouseConnectionId);
            if (entity == null)
            {
                _logger.LogDebug($"No Tied House Connection found for ID {tiedHouseConnectionId}.");
                return null;
            }

            var licences = await _dataverse.GetLicencesByTiedHouseConnectionAsync(tiedHouseConnectionId);
            return entity.ToViewModel(licences);
        }

        /// <summary>
        /// Get all liquor Tied House Connections for a user.
        /// </summary>
        public async Task<IEnumerable<TiedHouseConnection>> GetLiquorTiedHouseConnectionsForUser(string accountId)
        {
            _logger.LogDebug($"GetLiquorTiedHouseConnectionsForUser. AccountId = {accountId}.");

            var connections = await _dataverse.GetLiquorTiedHouseConnectionsByAccountAsync(accountId);
            return connections.Select(c => c.ToViewModel()).ToList();
        }

        /// <summary>
        /// Gets the singleton cannabis tied house connection for a user.
        /// A user should not have more than one cannabis tied house connection.
        /// </summary>
        public async Task<TiedHouseConnection> GetCannabisTiedHouseConnectionForUser(string accountId)
        {
            _logger.LogDebug($"GetCannabisTiedHouseConnectionForUser. AccountId = {accountId}.");

            var entity = await _dataverse.GetCannabisTiedHouseConnectionByAccountAsync(accountId);
            if (entity == null)
            {
                _logger.LogDebug($"No cannabis tied house connection found for account {accountId}.");
                return null;
            }

            return entity.ToViewModel();
        }

        /// <summary>
        /// Gets all liquor tied house connections for an application.
        /// Includes connections associated with the user account and those associated with the application.
        /// </summary>
        public async Task<IEnumerable<TiedHouseConnection>> GetLiquorTiedHouseConnectionsForApplication(
            string applicationId,
            string accountId
        )
        {
            _logger.LogDebug($"GetLiquorTiedHouseConnectionsForApplication. ApplicationId = {applicationId}.");

            var matchingConnections = await _dataverse.GetTiedHouseConnectionsByApplicationAsync(applicationId, accountId);

            var tiedHouseConnections = new List<TiedHouseConnection>();
            var supersededbyIds = new List<string>();

            foreach (var connection in matchingConnections)
            {
                var connectionId = connection.adoxio_tiedhouseconnectionId?.ToString();
                var supersededById = connection.adoxio_SupersededBy?.Id.ToString();
                var statusCode = (int?)connection.statuscode;

                if (statusCode == (int)TiedHouseStatusCode.Existing)
                {
                    // Mirror original logic: set SupersededById to own ID for existing records
                    supersededById = connectionId;
                }

                if (statusCode == (int)TiedHouseStatusCode.New && supersededById != null)
                {
                    supersededbyIds.Add(supersededById);
                }

                var vm = connection.ToViewModel();
                vm.SupersededById = supersededById;
                tiedHouseConnections.Add(vm);
            }

            return tiedHouseConnections.Where(item => !supersededbyIds.Contains(item.id)).ToList();
        }

        /// <summary>
        /// Creates a new liquor tied house connection for an application.
        /// If the incoming record already belongs to this application, updates or deletes it instead.
        /// </summary>
        public async Task<TiedHouseConnection> AddLiquorTiedHouseConnectionToApplication(
            TiedHouseConnection incomingTiedHouseConnection,
            string applicationId
        )
        {
            var connection = new adoxio_tiedhouseconnection();
            connection.CopyValues(incomingTiedHouseConnection);
            connection.adoxio_CategoryType = adoxio_tiedhouseconnection_adoxio_categorytype.Liquor;

            if (incomingTiedHouseConnection.ApplicationId == applicationId)
            {
                if (incomingTiedHouseConnection.MarkedForRemoval == true && incomingTiedHouseConnection.SupersededById == null)
                {
                    await _dataverse.DeleteTiedHouseConnectionAsync(connection.adoxio_tiedhouseconnectionId?.ToString());
                }
                else
                {
                    await _dataverse.UpdateTiedHouseConnectionAsync(connection);
                    await RemoveAndAddAssociateLicenses(
                        incomingTiedHouseConnection.AssociatedLiquorLicense?.Select(item => item.Id).ToList() ?? new List<string>(),
                        connection.adoxio_tiedhouseconnectionId?.ToString()
                    );
                }

                connection.adoxio_Application = new Microsoft.Xrm.Sdk.EntityReference("adoxio_application", new Guid(applicationId));
                return connection.ToViewModel();
            }

            // Creating a new record
            if (!string.IsNullOrEmpty(incomingTiedHouseConnection.id) && Guid.TryParse(incomingTiedHouseConnection.id, out var predecessorGuid))
            {
                connection.adoxio_SupersededBy = new Microsoft.Xrm.Sdk.EntityReference("adoxio_tiedhouseconnection", predecessorGuid);
            }

            connection.adoxio_Application = new Microsoft.Xrm.Sdk.EntityReference("adoxio_application", new Guid(applicationId));
            connection.adoxio_tiedhouseconnectionId = null;
            connection.Id = Guid.Empty;
            connection.adoxio_SelfDeclared = adoxio_generalyesno.Yes;
            connection.adoxio_DeclarationDate = DateTime.UtcNow;

            var newId = await _dataverse.CreateTiedHouseConnectionAsync(connection);

            await AssociateTiedHouseConnectionToLicenses(
                incomingTiedHouseConnection.AssociatedLiquorLicense?.Select(item => item.Id).ToList() ?? new List<string>(),
                newId.ToString()
            );

            return await GetTiedHouseConnectionById(newId.ToString());
        }

        /// <summary>
        /// Creates new liquor tied house connections for a user.
        /// </summary>
        public async Task<TiedHouseConnection> AddLiquorTiedHouseConnectionToUser(
            TiedHouseConnection incomingTiedHouseConnection,
            string accountId
        )
        {
            var connection = new adoxio_tiedhouseconnection();
            connection.CopyValues(incomingTiedHouseConnection);
            connection.adoxio_CategoryType = adoxio_tiedhouseconnection_adoxio_categorytype.Liquor;

            if (incomingTiedHouseConnection.AccountId == accountId)
            {
                if (incomingTiedHouseConnection.MarkedForRemoval == true && incomingTiedHouseConnection.SupersededById == null)
                {
                    await _dataverse.DeleteTiedHouseConnectionAsync(connection.adoxio_tiedhouseconnectionId?.ToString());
                }
                else
                {
                    await _dataverse.UpdateTiedHouseConnectionAsync(connection);
                    await RemoveAndAddAssociateLicenses(
                        incomingTiedHouseConnection.AssociatedLiquorLicense?.Select(item => item.Id).ToList() ?? new List<string>(),
                        connection.adoxio_tiedhouseconnectionId?.ToString()
                    );
                }

                return connection.ToViewModel();
            }

            // Creating a new record
            if (!string.IsNullOrEmpty(incomingTiedHouseConnection.id) && Guid.TryParse(incomingTiedHouseConnection.id, out var predecessorGuid))
            {
                connection.adoxio_SupersededBy = new Microsoft.Xrm.Sdk.EntityReference("adoxio_tiedhouseconnection", predecessorGuid);
            }

            connection.adoxio_tiedhouseconnectionId = null;
            connection.Id = Guid.Empty;
            connection.statuscode = adoxio_tiedhouseconnection_statuscode.Existing;
            connection.adoxio_SelfDeclared = adoxio_generalyesno.Yes;
            connection.adoxio_DeclarationDate = DateTime.UtcNow;
            connection.adoxio_AccountId = new Microsoft.Xrm.Sdk.EntityReference("account", new Guid(accountId));

            var newId = await _dataverse.CreateTiedHouseConnectionAsync(connection);

            await AssociateTiedHouseConnectionToLicenses(
                incomingTiedHouseConnection.AssociatedLiquorLicense?.Select(item => item.Id).ToList() ?? new List<string>(),
                newId.ToString()
            );

            return await GetTiedHouseConnectionById(newId.ToString());
        }

        /// <summary>
        /// Creates or updates the singleton cannabis tied house connection.
        /// </summary>
        public async Task<TiedHouseConnection> UpsertCannabisTiedHouseConnection(
            string accountId,
            TiedHouseConnection incomingTiedHouseConnectionRecord = null
        )
        {
            _logger.LogDebug($"UpsertCannabisTiedHouseConnection. AccountId = {accountId}.");

            var existing = await GetCannabisTiedHouseConnectionForUser(accountId);

            if (existing != null)
            {
                if (incomingTiedHouseConnectionRecord != null)
                {
                    _logger.LogDebug($"Updating and returning existing cannabis tied house connection. TiedHouseConnectionId = {existing.id}.");
                    return await UpdateCannabisTiedHouseConnection(existing.id, incomingTiedHouseConnectionRecord);
                }

                _logger.LogDebug($"Returning existing cannabis tied house connection. TiedHouseConnectionId = {existing.id}.");
                return existing;
            }

            _logger.LogDebug($"Creating and returning new cannabis tied house connection. AccountId = {accountId}.");
            return await CreateCannabisTiedHouseConnection(accountId, incomingTiedHouseConnectionRecord);
        }

        /// <summary>
        /// Updates a cannabis tied house connection by fully replacing the existing record.
        /// </summary>
        public async Task<TiedHouseConnection> UpdateCannabisTiedHouseConnection(
            string tiedHouseConnectionId,
            TiedHouseConnection incomingTiedHouseConnectionRecord
        )
        {
            _logger.LogDebug($"UpdateCannabisTiedHouseConnection. TiedHouseConnectionId = {tiedHouseConnectionId}.");

            var existing = await _dataverse.GetTiedHouseConnectionByIdAsync(tiedHouseConnectionId);
            if (existing == null)
                throw new Exception($"Tied House Connection with ID {tiedHouseConnectionId} could not be found.");

            var connection = new adoxio_tiedhouseconnection();
            connection.CopyValues(incomingTiedHouseConnectionRecord);
            connection.adoxio_tiedhouseconnectionId = new Guid(tiedHouseConnectionId);
            connection.Id = new Guid(tiedHouseConnectionId);
            connection.adoxio_CategoryType = adoxio_tiedhouseconnection_adoxio_categorytype.Cannabis;
            connection.statuscode = adoxio_tiedhouseconnection_statuscode.Existing;

            await _dataverse.UpdateTiedHouseConnectionAsync(connection);
            return await GetTiedHouseConnectionById(tiedHouseConnectionId);
        }

        /// <summary>
        /// Deletes a tied house connection by ID.
        /// </summary>
        public async Task DeleteTiedHouseConnectionById(string tiedHouseConnectionId)
        {
            _logger.LogDebug($"DeleteTiedHouseConnectionById. TiedHouseConnectionId = {tiedHouseConnectionId}.");
            await _dataverse.DeleteTiedHouseConnectionAsync(tiedHouseConnectionId);
        }

        /// <summary>
        /// Associates a tied house connection with a list of licences.
        /// </summary>
        public async Task AssociateTiedHouseConnectionToLicenses(List<string> licences, string tiedHouseId)
        {
            foreach (var licenceId in licences)
            {
                await _dataverse.AssociateTiedHouseConnectionToLicenceAsync(tiedHouseId, licenceId);
            }
        }

        /// <summary>
        /// Removes existing licence associations and adds new ones if the list has changed.
        /// </summary>
        public async Task RemoveAndAddAssociateLicenses(List<string> licences, string tiedHouseId)
        {
            var existingLicences = await _dataverse.GetLicencesByTiedHouseConnectionAsync(tiedHouseId);
            var existingIds = existingLicences.Select(l => l.Id.ToString()).ToList();

            var hasChanged = !existingIds.OrderBy(x => x).SequenceEqual(licences.OrderBy(x => x));
            if (!hasChanged) return;

            foreach (var licenceId in existingIds)
                await _dataverse.DisassociateTiedHouseConnectionFromLicenceAsync(tiedHouseId, licenceId);

            await AssociateTiedHouseConnectionToLicenses(licences, tiedHouseId);
        }

        private async Task<TiedHouseConnection> CreateCannabisTiedHouseConnection(
            string accountId,
            TiedHouseConnection incomingTiedHouseConnectionRecord = null
        )
        {
            _logger.LogDebug($"CreateCannabisTiedHouseConnection. AccountId = {accountId}.");

            var connection = new adoxio_tiedhouseconnection();
            if (incomingTiedHouseConnectionRecord != null)
                connection.CopyValues(incomingTiedHouseConnectionRecord);

            connection.adoxio_CategoryType = adoxio_tiedhouseconnection_adoxio_categorytype.Cannabis;
            connection.statuscode = adoxio_tiedhouseconnection_statuscode.Existing;
            // Do not set Id/primary key before Create — an explicit Guid.Empty is treated
            // by Dataverse as an invalid supplied key ("Expected non-empty Guid") rather than
            // "generate one". Leave it untouched, matching every other Create*Async call site.
            connection.adoxio_AccountId = new Microsoft.Xrm.Sdk.EntityReference("account", new Guid(accountId));

            var newId = await _dataverse.CreateTiedHouseConnectionAsync(connection);
            return await GetTiedHouseConnectionById(newId.ToString());
        }
    }
}
