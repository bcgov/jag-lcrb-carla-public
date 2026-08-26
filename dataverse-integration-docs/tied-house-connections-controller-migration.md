# LCSD-8553: Migrate TiedHouseConnectionsController to IDataverseClient

## Summary

Migrated `cllc-public-app/Controllers/TiedHouseConnectionsController.cs` and its backing
`TiedHouseConnectionsRepository` from `IDynamicsClient` (AutoRest) to `IDataverseClient`
(Dataverse SDK). Zero `IDynamicsClient` references remain in either file.

---

## Files Changed

| File | Change |
|---|---|
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` | 8 new TiedHouseConnection methods |
| `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` | Implementations for those 8 methods |
| `cllc-public-app/Models.Extensions/AdoxioTiedHouseConnectionDataverse.cs` | New file — `CopyValues` + `ToViewModel` on `adoxio_tiedhouseconnection` |
| `cllc-public-app/Repositories/TiedHouseConnectionsRepository.cs` | Full rewrite — IDynamicsClient → IDataverseClient |
| `cllc-public-app/Controllers/TiedHouseConnectionsController.cs` | Remove IDynamicsClient injection; sync GET actions → async |

---

## New IDataverseClient Methods

```csharp
Task<adoxio_tiedhouseconnection?> GetTiedHouseConnectionByIdAsync(string id, ...)
Task<IList<adoxio_tiedhouseconnection>> GetLiquorTiedHouseConnectionsByAccountAsync(string accountId, ...)
Task<adoxio_tiedhouseconnection?> GetCannabisTiedHouseConnectionByAccountAsync(string accountId, ...)
Task<IList<adoxio_tiedhouseconnection>> GetTiedHouseConnectionsByApplicationAsync(string applicationId, string accountId, ...)
Task<IList<adoxio_licences>> GetLicencesByTiedHouseConnectionAsync(string tiedHouseId, ...)
Task UpdateTiedHouseConnectionAsync(adoxio_tiedhouseconnection connection, ...)
Task AssociateTiedHouseConnectionToLicenceAsync(string tiedHouseId, string licenceId, ...)
Task DisassociateTiedHouseConnectionFromLicenceAsync(string tiedHouseId, string licenceId, ...)
```

---

## Key Implementation Notes

### N:N licence associations
The N:N relationship between `adoxio_tiedhouseconnection` and `adoxio_licences` uses the schema
name `adoxio_adoxio_tiedhouseconnection_adoxio_licence`. Associate/Disassociate use
`ServiceClient.Associate` / `ServiceClient.Disassociate` with that relationship name.

`GetLicencesByTiedHouseConnectionAsync` uses `RetrieveRequest` with a
`RelationshipQueryCollection` to fetch the related licences in one call.

### Cannabis category OR filter
`GetCannabisTiedHouseConnectionByAccountAsync` replicates the original OData filter
`(categorytype eq Cannabis or categorytype ne Liquor)` using a `FilterExpression` with
`LogicalOperator.Or`. Post-query ordering (Cannabis first, then most recently modified) is
applied in-memory to match the original sort behavior.

### Application+Account OR query
`GetTiedHouseConnectionsByApplicationAsync` uses a `QueryExpression` with two `FilterExpression`
children joined by `LogicalOperator.Or`, replacing the OData `($account OR $application)` filter.

### Account association
The old code used `AddReferenceWithHttpMessagesAsync` to associate a TiedHouseConnection with an
Account after creation. The new code sets `adoxio_AccountId = new EntityReference("account", guid)`
directly on the entity before calling `CreateTiedHouseConnectionAsync`, which is cleaner and atomic.

### GetLiquorTiedHouseConnectionsForApplication business logic
The original code modified `_adoxio_supersededbyValue` in-place on Existing records (setting it
to the record's own ID) before calling `ToViewModel`. The new code replicates this by overriding
`SupersededById` on the ViewModel after `ToViewModel()` is called, keeping the class immutable.

### sync → async
`GetLiquorTiedHouseConnectionsForUser`, `GetCannabisTiedHouseConnectionForUser`, and
`GetLiquorTiedHouseConnectionsForApplication` were synchronous in the old repository and
controller. All are now fully async, eliminating the blocking `.Get()` AutoRest calls.

### Extension methods
Old extensions (`CopyValues`, `ToViewModel`) were on `MicrosoftDynamicsCRMadoxioTiedhouseconnection`.
New ones are on `adoxio_tiedhouseconnection` in `AdoxioTiedHouseConnectionDataverse.cs`.
The old file (`AdoxioTiedHouseConnections.cs`) is preserved for any callers still on AutoRest
(e.g. `ApplicationsController`, `LegalEntitiesController`).

### Enum cast table
| ViewModel field type | Entity property type | Cast pattern |
|---|---|---|
| `int?` (0/1) | `adoxio_generalyesno?` (No=0, Yes=1) | `(adoxio_generalyesno?)value` |
| `MarketerYesNo?` (845280000/845280001) | `adoxio_tiedhouseconnection_adoxio_crsconnectiontomarketer?` | `(enum?)(int?)value` |
| `TiedHouseConnectionType?` | `adoxio_tiedhouseconnection_adoxio_connectiontype?` | `(enum?)(int?)value` |
| `int?` (CategoryType) | `adoxio_tiedhouseconnection_adoxio_categorytype?` | `(enum?)(int?)value` |
| `int?` (StatusCode) | `adoxio_tiedhouseconnection_statuscode?` | `(enum?)(int?)value` |
| `DateTimeOffset?` | `DateTime?` | `.DateTime` / `(DateTimeOffset?)value` |
