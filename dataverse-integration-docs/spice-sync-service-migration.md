# LCSD-8556: Migrate carla-spice-sync-service / SpiceUtils.cs

## Summary

Replaced all `IDynamicsClient` (AutoRest) usages in `carla-spice-sync-service/SpiceUtils.cs`, `Validation.cs`, `Controllers/WorkerScreeningsController.cs`, and `Controllers/ApplicationScreeningsController.cs` with `IDataverseClient` (Dataverse SDK).

Nine new SPICE-specific query methods were added to `IDataverseClient` and implemented in `DataverseClient.cs`. One additional method (`GetApplicationTypeByIdAsync`) was added during implementation when a missing lookup was identified.

---

## Files Changed

| File | Change |
|---|---|
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` | Added 10 SPICE-specific methods (9 bulk/query + 1 by-ID lookup) |
| `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` | Implemented all 10 new methods |
| `carla-spice-sync-service/SpiceUtils.cs` | Full rewrite — `IDynamicsClient` → `IDataverseClient`, all methods async |
| `carla-spice-sync-service/Validation.cs` | `ValidateAssociateConsent` → `ValidateAssociateConsentAsync`, uses `IDataverseClient` |
| `carla-spice-sync-service/Controllers/WorkerScreeningsController.cs` | `SendWorkerScreeningRequest` → `async Task<ActionResult>` |
| `carla-spice-sync-service/Controllers/ApplicationScreeningsController.cs` | Added `await` to two `SendApplicationScreeningRequest` call sites |

---

## New IDataverseClient Methods

```csharp
// SPICE sync support
Task<Contact?> GetContactBySpdJobIdAsync(int spdJobId, CancellationToken ct = default);
Task<adoxio_personalhistorysummary?> GetPersonalHistorySummaryByWorkerJobNumberAsync(string jobNumber, CancellationToken ct = default);
Task<IList<adoxio_alias>> GetAliasesByContactIdAsync(string contactId, CancellationToken ct = default);
Task<IList<adoxio_previousaddress>> GetPreviousAddressesByContactIdAsync(string contactId, CancellationToken ct = default);
Task<adoxio_application?> GetApplicationByJobNumberAsync(string jobNumber, CancellationToken ct = default);
Task<IList<adoxio_leconnection>> GetActiveLeConnectionsByParentAccountIdAsync(string accountId, CancellationToken ct = default);
Task<IList<adoxio_worker>> GetWorkersToSendAsync(CancellationToken ct = default);
Task<IList<adoxio_applicationtype>> GetApplicationTypesWithLeSectionAsync(CancellationToken ct = default);
Task<IList<adoxio_application>> GetApplicationsToSendAsync(CancellationToken ct = default);

// Application Type section
Task<adoxio_applicationtype?> GetApplicationTypeByIdAsync(string id, CancellationToken ct = default);
```

---

## Key Migration Decisions

### SpiceUtils constructor — no DI injection
Hangfire lambdas (`RecurringJob.AddOrUpdate`) in `Startup.cs` construct `SpiceUtils` directly with `new SpiceUtils(_configuration, loggerFactory)`. Constructor injection of `IDataverseClient` via DI is not feasible here. Solution: constructor self-creates `new DataverseClient(configuration)` as a default, with an optional override parameter for testing:

```csharp
public SpiceUtils(IConfiguration configuration, ILoggerFactory loggerFactory, IDataverseClient dataverse = null)
{
    _dataverse = dataverse ?? new DataverseClient(configuration);
}
```

### Related entity loading
Old AutoRest used OData `expand:` to inline-load navigation properties in one call. The Dataverse SDK returns only `EntityReference` values. All related entities are fetched with separate `await _dataverse.GetXxxByIdAsync(ref.Id.ToString())` calls in `CreateApplicationScreeningRequestV2`, `CreateAssociate`, and `GenerateWorkerScreeningRequest`.

### V1 overload removal
The `CreateAssociate(adoxio_legalentity)` overload and its `GetLegalEntityPositions(adoxio_legalentity)` helper were dead code in the V2 flow and used outdated AutoRest property shapes. Both were removed. Only the `adoxio_leconnection` overloads remain.

### Property name changes — Account
`adoxio_BcIncorporationNumber` (old AutoRest) → `adoxio_BCIncorporationNumber` (Dataverse SDK, capital BC).

### Enum changes — adoxio_legalentity.adoxio_IsIndividual
Old AutoRest: `int?` (checked with `== 1`). New Dataverse SDK: `adoxio_generalyesno?` (check with `== adoxio_generalyesno.Yes`). Only relevant for `adoxio_leconnection.adoxio_IsIndividual` which remains `bool?`.

### Validation.cs
`ValidateAssociateConsent(IDynamicsClient, ...)` → `ValidateAssociateConsentAsync(IDataverseClient, ...)`. Contact is fetched via `await dataverse.GetContactByIdAsync(id)`. Status check uses `contact.StatusCode == contact_statuscode.Active` and `contact.adoxio_ConsentValidated != adoxio_contact_adoxio_consentvalidated.Yes`.

### GetApplicationsToSendAsync filter
Applications to send are those with `adoxio_checklistsenttospd = 1` AND security clearance status in (`RequestNotSent=845280000`, `RequestSending=845280007`). The method returns all matching applications; `SendFoundApplicationsV2` then filters by the application type IDs that have `adoxio_haslesection = true`.

---

## Build Verification

```
dotnet build carla-spice-sync-service/CarlaSpiceSync.csproj
```

Expected: zero errors, zero `IDynamicsClient` references in `carla-spice-sync-service/`.
