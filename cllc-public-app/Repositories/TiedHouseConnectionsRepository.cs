using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        public IEnumerable<TiedHouseConnection> GetLiquorTiedHouseConnectionsForUser(string accountId)
        {
            _logger.LogDebug($"GetLiquorTiedHouseConnectionsForUser. AccountId = {accountId}.");

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

        /// <summary>
        /// Get the count of all "existing" liquor Tied House Connections for a user.
        /// </summary>
        /// <param name="accountId">The accountId of the user to filter results by</param>
        /// <returns>The count of "existing" tied house connections</returns>
        public int GetExistingLiquorTiedHouseConnectionsCountForUser(string accountId)
        {
            _logger.LogDebug($"GetExistingLiquorTiedHouseConnectionsCountForUser. AccountId = {accountId}.");

            var andFilterConditions = new List<string>
            {
                $"_adoxio_accountid_value eq {accountId}",
                $"statuscode eq {(int)TiedHouseStatusCode.Existing}",
                $"adoxio_categorytype eq {(int)TiedHouseCategoryType.Liquor}",
                "statecode eq 0"
            };
            var filter = string.Join(" and ", andFilterConditions);

            var response = _dynamicsClient.Tiedhouseconnections.Get(filter: filter, top: 1, count: true);

            int result = int.TryParse(response?.Count, out var parsedCount) ? parsedCount : 0;

            return result;
        }

        /// <summary>
        /// Gets the singleton cannabis tied house connection for a user.
        /// A user should not have more than one cannabis tied house connection.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>The cannabis tied house connection record or `null` if no record exists</returns>
        public TiedHouseConnection GetCannabisTiedHouseConnectionForUser(string accountId)
        {
            _logger.LogDebug($"GetCannabisTiedHouseConnectionForUser. AccountId = {accountId}.");

            var andFilterConditions = new List<string>
            {
                $"_adoxio_accountid_value eq {accountId}",
                $"statuscode eq {(int)TiedHouseStatusCode.Existing}",
                $"(adoxio_categorytype eq {(int)TiedHouseCategoryType.Cannabis} or adoxio_categorytype ne {(int)TiedHouseCategoryType.Liquor})",
                "statecode eq 0"
            };
            var filter = string.Join(" and ", andFilterConditions);

            var expand = new List<string> { "adoxio_adoxio_tiedhouseconnection_adoxio_licence" };

            var matchingTiedHouseConnections = _dynamicsClient.Tiedhouseconnections.Get(filter: filter, expand: expand);

            if (matchingTiedHouseConnections.Value.Count == 0)
            {
                _logger.LogDebug($"No cannabis tied house connection found for account {accountId}.");

                return null;
            }

            var cannabisTiedHouseConnection = matchingTiedHouseConnections
                .Value.OrderByDescending(item => item.AdoxioCategoryType == (int)TiedHouseCategoryType.Cannabis)
                .ThenByDescending(item => item.Modifiedon)
                .First();

            if (cannabisTiedHouseConnection == null)
            {
                _logger.LogDebug($"No cannabis tied house connection found for account {accountId}.");
                return null;
            }

            return cannabisTiedHouseConnection.ToViewModel();
        }

        /// <summary>
        /// Gets all liquor tied house connections for an application.
        /// </summary>
        /// <remarks>
        /// This includes liquor tied house connections that are associated with the user account, as well as those that
        /// are associated with the application.
        /// </remarks>
        /// <param name="applicationId">The ID of the application to filter results by.</param>
        /// <param name="accountId">The ID of the user's account to filter results by.</param>
        /// <returns></returns>
        public IEnumerable<TiedHouseConnection> GetLiquorTiedHouseConnectionsForApplication(
            string applicationId,
            string accountId
        )
        {
            _logger.LogDebug($"GetLiquorTiedHouseConnectionsForApplication. ApplicationId = {applicationId}.");

            // Fetch existing liquor records for the user account
            var andAccountFilterConditions = new List<string>
            {
                $"_adoxio_accountid_value eq {accountId}",
                $"statuscode eq {(int)TiedHouseStatusCode.Existing}",
                $"adoxio_categorytype eq {(int)TiedHouseCategoryType.Liquor}",
                "statecode eq 0"
            };
            var accountFilterConditions = string.Join(" and ", andAccountFilterConditions);

            // Fetch all liquor records for the application
            var andApplicationFilterConditions = new List<string>
            {
                $"_adoxio_application_value eq {applicationId}",
                $"adoxio_categorytype eq {(int)TiedHouseCategoryType.Liquor}",
                "statecode eq 0"
            };
            var applicationFilterConditions = string.Join(" and ", andApplicationFilterConditions);

            var filter = $"({accountFilterConditions}) or ({applicationFilterConditions})";
            var expand = new List<string> { "adoxio_adoxio_tiedhouseconnection_adoxio_licence" };

            var matchingTiedHouseConnections = _dynamicsClient
                .Tiedhouseconnections.Get(filter: filter, expand: expand)
                .Value;

            var tiedHouseConnections = new List<TiedHouseConnection>();
            var supersededbyIds = new List<string>();

            foreach (var matchingTiedHouseConnection in matchingTiedHouseConnections)
            {
                if (matchingTiedHouseConnection.Statuscode == (int)TiedHouseStatusCode.Existing)
                {
                    matchingTiedHouseConnection._adoxio_supersededbyValue =
                        matchingTiedHouseConnection.AdoxioTiedhouseconnectionid;
                }

                if (matchingTiedHouseConnection.Statuscode == (int)TiedHouseStatusCode.New)
                {
                    supersededbyIds.Add(matchingTiedHouseConnection._adoxio_supersededbyValue);
                }

                tiedHouseConnections.Add(matchingTiedHouseConnection.ToViewModel());
            }

            // If a "new" record supersedes an "existing" record, we want to exclude the "existing" record
            var result = tiedHouseConnections.Where(item => !supersededbyIds.Contains(item.id)).ToList();

            return result;
        }

        /// <summary>
        /// Creates a new liquor tied house connection for an application.
        /// </summary>
        /// <remarks>
        /// Liquor tied house connections are not updated directly. Instead, a new record is created with the new
        /// data, and the old record is updated to set the `SupersededBy` field to the new record's ID. In this way,
        /// the history of changes to the tied house connection record is preserved.
        /// </remarks>
        /// <param name="incomingTiedHouseConnection"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public async Task<TiedHouseConnection> AddLiquorTiedHouseConnectionToApplication(
            TiedHouseConnection incomingTiedHouseConnection,
            string applicationId
        )
        {
            MicrosoftDynamicsCRMadoxioTiedhouseconnection adoxioTiedHouseConnection =
                new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

            adoxioTiedHouseConnection.CopyValues(incomingTiedHouseConnection);

            // Ensure the tied house connection is of type (category) "Liquor"
            adoxioTiedHouseConnection.AdoxioCategoryType = (int)TiedHouseCategoryType.Liquor;

            // If the incoming record already has an account ID defined, then we are updating an existing record.
            if (incomingTiedHouseConnection.ApplicationId == applicationId)
            {
                if (
                    incomingTiedHouseConnection.MarkedForRemoval == true
                    && incomingTiedHouseConnection.SupersededById == null
                )
                {
                    await DeleteTiedHouseConnectionById(adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid);
                }
                else
                {
                    await _dynamicsClient.Tiedhouseconnections.UpdateAsync(
                        adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid,
                        adoxioTiedHouseConnection
                    );

                    await RemoveAndAddAssociateLicenses(
                        incomingTiedHouseConnection.AssociatedLiquorLicense.Select(item => item.Id).ToList(),
                        adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid
                    );
                }

                return adoxioTiedHouseConnection.ToViewModel();
            }

            // If the incoming record does not have an account ID defined, then we are creating a new record.

            if (!string.IsNullOrEmpty(incomingTiedHouseConnection.id))
            {
                // If the incoming record already has an ID, then we are soft-deleting the previous version of the
                // record, and creating a new record, which will "supersede" the previous version.
                adoxioTiedHouseConnection.SupersededByOdataBind =
                    $"/adoxio_tiedhouseconnections({incomingTiedHouseConnection.id})";
            }

            // Associate the tied house connection with the application
            adoxioTiedHouseConnection.ApplicationOdataBind = $"/adoxio_applications({applicationId})";

            // Ensure this is a new tied house connection
            adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid = null;

            adoxioTiedHouseConnection.AdoxioSelfDeclared = 1;

            adoxioTiedHouseConnection.AdoxioDeclarationDate = DateTimeOffset.Now;

            var createdTiedHouseConnection = await _dynamicsClient.Tiedhouseconnections.CreateAsync(
                adoxioTiedHouseConnection
            );

            // Associate the new tied house connection with the associated liquor licenses
            await AssociateTiedHouseConnectionToLicenses(
                incomingTiedHouseConnection.AssociatedLiquorLicense.Select(item => item.Id).ToList(),
                createdTiedHouseConnection.AdoxioTiedhouseconnectionid
            );

            return createdTiedHouseConnection.ToViewModel();
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
        /// <param name="incomingTiedHouseConnection">The tied house connection data to create.</param>
        /// /// <param name="accountId">The ID of the user's account.</param>
        /// <returns>The created tied house connection.</returns>
        public async Task<TiedHouseConnection> AddLiquorTiedHouseConnectionToUser(
            TiedHouseConnection incomingTiedHouseConnection,
            string accountId
        )
        {
            MicrosoftDynamicsCRMadoxioTiedhouseconnection adoxioTiedHouseConnection =
                new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

            adoxioTiedHouseConnection.CopyValues(incomingTiedHouseConnection);

            // Ensure the tied house connection is of type (category) "Liquor"
            adoxioTiedHouseConnection.AdoxioCategoryType = (int)TiedHouseCategoryType.Liquor;

            // If the incoming record already has an account ID defined, then we are updating an existing record.
            if (incomingTiedHouseConnection.AccountId == accountId)
            {
                if (
                    incomingTiedHouseConnection.MarkedForRemoval == true
                    && incomingTiedHouseConnection.SupersededById == null
                )
                {
                    await DeleteTiedHouseConnectionById(adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid);
                }
                else
                {
                    await _dynamicsClient.Tiedhouseconnections.UpdateAsync(
                        adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid,
                        adoxioTiedHouseConnection
                    );

                    await RemoveAndAddAssociateLicenses(
                        incomingTiedHouseConnection.AssociatedLiquorLicense.Select(item => item.Id).ToList(),
                        adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid
                    );
                }

                return adoxioTiedHouseConnection.ToViewModel();
            }

            // If the incoming record does not have an account ID defined, then we are creating a new record.

            if (!string.IsNullOrEmpty(incomingTiedHouseConnection.id))
            {
                // If the incoming record already has an ID, then we are soft-deleting the previous version of the
                // record, and creating a new record, which will "supersede" the previous version.
                adoxioTiedHouseConnection.SupersededByOdataBind =
                    $"/adoxio_tiedhouseconnections({incomingTiedHouseConnection.id})";
            }

            // Ensure this is a new tied house connection
            adoxioTiedHouseConnection.AdoxioTiedhouseconnectionid = null;

            // Tied house connections created directly against the user account are automatically set to "Existing"
            adoxioTiedHouseConnection.Statuscode = (int)TiedHouseStatusCode.Existing;

            adoxioTiedHouseConnection.AdoxioSelfDeclared = 1;

            adoxioTiedHouseConnection.AdoxioDeclarationDate = DateTimeOffset.Now;

            var createdTiedHouseConnection = await _dynamicsClient.Tiedhouseconnections.CreateAsync(
                adoxioTiedHouseConnection
            );

            // Associate the new tied house connection with the account
            await AssociateTiedHouseConnectionToUserAccount(
                createdTiedHouseConnection.AdoxioTiedhouseconnectionid,
                accountId
            );

            // Associate the new tied house connection with the associated liquor licenses
            await AssociateTiedHouseConnectionToLicenses(
                incomingTiedHouseConnection.AssociatedLiquorLicense.Select(item => item.Id).ToList(),
                createdTiedHouseConnection.AdoxioTiedhouseconnectionid
            );

            return createdTiedHouseConnection.ToViewModel();
        }

        /// <summary>
        /// Creates or Updates the singleton cannabis tied house connection.
        ///
        /// If a cannabis tied house connection already exists for the user, it will update and return that existing
        /// record.
        ///
        /// If no cannabis tied house connection exists, it will create a new one and return it.
        /// </summary>
        /// <param name="accountId">The ID of the account associated with the cannabis tied house connection.</param>
        /// <param name="incomingTiedHouseConnectionRecord">Optional cannabis tied house connection record used to
        /// create or update the record.</param>
        /// <returns>The cannabis tied house connection record.</returns>
        public async Task<TiedHouseConnection> UpsertCannabisTiedHouseConnection(
            string accountId,
            TiedHouseConnection incomingTiedHouseConnectionRecord = null
        )
        {
            _logger.LogDebug($"UpsertCannabisTiedHouseConnection. AccountId = {accountId}.");

            TiedHouseConnection existingCannabisTiedHouseConnectionRecord = GetCannabisTiedHouseConnectionForUser(
                accountId
            );

            if (existingCannabisTiedHouseConnectionRecord != null)
            {
                if (incomingTiedHouseConnectionRecord != null)
                {
                    _logger.LogDebug(
                        $"Updating and returning existing cannabis tied house connection. TiedHouseConnectionId = {existingCannabisTiedHouseConnectionRecord.id}."
                    );

                    return await UpdateCannabisTiedHouseConnection(
                        existingCannabisTiedHouseConnectionRecord.id,
                        incomingTiedHouseConnectionRecord
                    );
                }

                _logger.LogDebug(
                    $"Returning existing cannabis tied house connection. TiedHouseConnectionId = {existingCannabisTiedHouseConnectionRecord.id}."
                );

                return existingCannabisTiedHouseConnectionRecord;
            }

            _logger.LogDebug($"Creating and returning new cannabis tied house connection. AccountId = {accountId}.");

            return await CreateCannabisTiedHouseConnection(accountId, incomingTiedHouseConnectionRecord);
        }

        /// <summary>
        /// Creates a new cannabis tied house connection.
        ///
        /// Important: This method will create a new record even if one already exists. Use the
        /// `UpsertCannabisTiedHouseConnection` method if you want to safely ensure that only one cannabis tied house
        /// connection exists for the user.
        /// </summary>
        /// <param name="accountId">The ID of the account associated with the cannabis tied house connection.</param>
        /// <param name="incomingTiedHouseConnectionRecord">Optional cannabis tied house connection record used to
        /// initialize the new record. If not provided, an empty record will be created.</param>
        /// <returns>The created cannabis tied house connection record.</returns>
        private async Task<TiedHouseConnection> CreateCannabisTiedHouseConnection(
            string accountId,
            TiedHouseConnection incomingTiedHouseConnectionRecord = null
        )
        {
            _logger.LogDebug($"CreateCannabisTiedHouseConnection. AccountId = {accountId}.");

            var newCannabisTiedHouseConnectionRecord = new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

            if (incomingTiedHouseConnectionRecord != null)
            {
                newCannabisTiedHouseConnectionRecord.CopyValues(incomingTiedHouseConnectionRecord);
            }

            // Ensure the tied house connection is of type (category) "Cannabis"
            newCannabisTiedHouseConnectionRecord.AdoxioCategoryType = (int)TiedHouseCategoryType.Cannabis;
            // The singleton cannabis tied house connection should always be in the "Existing" status
            newCannabisTiedHouseConnectionRecord.Statuscode = (int)TiedHouseStatusCode.Existing;

            var createdCannabisTiedHouseConnectionRecord = await _dynamicsClient.Tiedhouseconnections.CreateAsync(
                newCannabisTiedHouseConnectionRecord
            );

            // Associate the new tied house connection with the account
            await AssociateTiedHouseConnectionToUserAccount(
                createdCannabisTiedHouseConnectionRecord.AdoxioTiedhouseconnectionid.ToString(),
                accountId
            );

            return createdCannabisTiedHouseConnectionRecord.ToViewModel();
        }

        /// <summary>
        /// Updates a cannabis tied house connection by fully replacing the existing record.
        ///
        /// Important: This method does not perform a partial update (patch); instead, all existing values are
        /// overwritten with the values from the provided record.
        /// </summary>
        /// <param name="tiedHouseConnectionId">The ID of the cannabis tied house connection to update.</param>
        /// <param name="incomingTiedHouseConnectionRecord">The updated cannabis tied house connection record.</param>
        /// <returns>The updated cannabis tied house connection record.</returns>
        public async Task<TiedHouseConnection> UpdateCannabisTiedHouseConnection(
            string tiedHouseConnectionId,
            TiedHouseConnection incomingTiedHouseConnectionRecord
        )
        {
            _logger.LogDebug($"UpdateCannabisTiedHouseConnection. TiedHouseConnectionId = {tiedHouseConnectionId}.");

            Guid tiedHouseConnectionGuid = new Guid(tiedHouseConnectionId);

            MicrosoftDynamicsCRMadoxioTiedhouseconnection existingTiedHouseConnectionRecord =
                await _dynamicsClient.GetTiedHouseConnectionById(tiedHouseConnectionGuid);

            if (existingTiedHouseConnectionRecord == null)
            {
                throw new Exception($"Tied House Connection with ID {tiedHouseConnectionId} could not be found.");
            }

            var updatedTiedHouseConnectionRecord = new MicrosoftDynamicsCRMadoxioTiedhouseconnection();

            updatedTiedHouseConnectionRecord.CopyValues(incomingTiedHouseConnectionRecord);

            // Ensure the tied house connection is of type (category) "Cannabis"
            updatedTiedHouseConnectionRecord.AdoxioCategoryType = (int)TiedHouseCategoryType.Cannabis;
            // The singleton cannabis tied house connection should always be in the "Existing" status
            updatedTiedHouseConnectionRecord.Statuscode = (int)TiedHouseStatusCode.Existing;

            await _dynamicsClient.Tiedhouseconnections.UpdateAsync(
                tiedHouseConnectionId.ToString(),
                updatedTiedHouseConnectionRecord
            );

            return updatedTiedHouseConnectionRecord.ToViewModel();
        }

        /// <summary>
        /// Deletes a tied house connection by ID.
        /// </summary>
        /// <param name="tiedHouseConnectionId">The ID of the tied house connection to delete.</param>
        /// <returns>void</returns>
        public async Task DeleteTiedHouseConnectionById(string tiedHouseConnectionId)
        {
            _logger.LogDebug($"DeleteTiedHouseConnectionById. TiedHouseConnectionId = {tiedHouseConnectionId}.");

            await _dynamicsClient.Tiedhouseconnections.DeleteAsync(tiedHouseConnectionId);
        }

        /// <summary>
        /// Associates a tied house connection with a list of licenses.
        /// </summary>
        /// <param name="licenses"></param>
        /// <param name="tiedHouseId"></param>
        /// <returns></returns>
        public async Task AssociateTiedHouseConnectionToLicenses(List<string> licenses, string tiedHouseId)
        {
            foreach (string licenceId in licenses)
            {
                Odataid odataid = new Odataid
                {
                    OdataidProperty = _dynamicsClient.GetEntityURI("adoxio_licenceses", licenceId)
                };

                await _dynamicsClient.Tiedhouseconnections.AddReferenceWithHttpMessagesAsync(
                    tiedHouseId,
                    "adoxio_adoxio_tiedhouseconnection_adoxio_licence",
                    odataid
                );
            }
        }

        /// <summary>
        /// Associates a tied house connection record with a user account record.
        /// </summary>
        /// <param name="tiedHouseConnectionId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task AssociateTiedHouseConnectionToUserAccount(string tiedHouseConnectionId, string accountId)
        {
            Odataid odataid = new Odataid { OdataidProperty = _dynamicsClient.GetEntityURI("accounts", accountId) };

            await _dynamicsClient.Tiedhouseconnections.AddReferenceWithHttpMessagesAsync(
                tiedHouseConnectionId,
                "adoxio_AccountId",
                odataid
            );
        }

        /// <summary>
        /// Removes existing license associations and adds new ones for a tied house connection.
        /// </summary>
        /// <remarks>
        /// Fetches the existing tied house connection by ID, and compares the existing license associations
        /// with the provided list of licenses. If they differ, it removes the existing associations and
        /// adds the new ones.
        /// </remarks>
        /// <param name="licenses"></param>
        /// <param name="tiedHouseId"></param>
        /// <returns></returns>
        public async Task RemoveAndAddAssociateLicenses(List<string> licenses, string tiedHouseId)
        {
            var result = new List<TiedHouseConnection>();

            string filter = $"adoxio_tiedhouseconnectionid eq {tiedHouseId}";
            var expand = new List<string> { "adoxio_adoxio_tiedhouseconnection_adoxio_licence" };
            var select = new List<string> { "adoxio_adoxio_tiedhouseconnection_adoxio_licence" };

            var tiedHouseConnection = _dynamicsClient
                .Tiedhouseconnections.Get(filter: filter, select: select, expand: expand)
                .Value.FirstOrDefault();

            var licencesIds = tiedHouseConnection.Adoxio_Adoxio_TiedHouseConnection_Adoxio_Licence.Select(item =>
                item.AdoxioLicencesid
            );

            var hasLicencesBeenUpdated = !licencesIds
                .OrderBy(item => item)
                .SequenceEqual(licenses.OrderBy(item => item));

            if (!hasLicencesBeenUpdated)
            {
                return;
            }

            foreach (var id in licencesIds)
            {
                await _dynamicsClient.Tiedhouseconnections.DeleteReferenceWithHttpMessagesAsync(
                    tiedHouseId,
                    "adoxio_adoxio_tiedhouseconnection_adoxio_licence",
                    id
                );
            }

            await AssociateTiedHouseConnectionToLicenses(licenses, tiedHouseId);
        }
    }
}
