# LCSD-8546: Migrate orgbook-service

## Overview

Migrated `orgbook-service` from `IDynamicsClient` (AutoRest) to `IDataverseClient` (Dataverse SDK). The service syncs BC Registries / OrgBook data with Dynamics via gRPC and Hangfire background jobs.

## Files Changed

| File | Change |
|---|---|
| `orgbook-service/Controllers/OrgBookController.cs` | Replace `IDynamicsClient` with `IDataverseClient`; add two-constructor pattern for Hangfire compatibility |
| `orgbook-service/VonAgentClient.cs` | Change `MicrosoftDynamicsCRMadoxioLicences` parameter to `adoxio_licences`; update property access |
| `orgbook-service/OrgbookUtils.cs` | Remove unused AutoRest using directives |
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` | Add 3 OrgBook sync query methods |
| `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` | Implement the 3 new methods |

## New IDataverseClient Methods

### GetActiveLicencesMissingOrgBookCredentialAsync
Returns active licences (`statuscode = 1`) where `adoxio_orgbookcredentialresult` is null. Used by `SyncLicencesToOrgbook`.

### GetActiveLicencesWithOrgBookCredentialPendingSyncAsync
Returns active licences that passed OrgBook credential (`adoxio_orgbookcredentialresult = Pass`) but have no credential ID yet (`adoxio_orgbookcredentialid = null`). Used by `SyncOrgbookToLicences`.

### GetAccountsMissingOrgBookLinkAsync
Returns accounts with a BC incorporation number but no OrgBook organization link and no business registration number. Uses a minimal `ColumnSet` (`adoxio_bcincorporationnumber`, `accountid`) since only these fields are needed for the sync. Used by `SyncOrgbookToAccounts`.

## Property Mapping

### adoxio_licences (Dataverse SDK)
| Old AutoRest | New Dataverse SDK |
|---|---|
| `AdoxioLicencenumber` | `adoxio_LicenceNumber` |
| `AdoxioLicencesid` | `Id` (inherited from Entity) |
| `AdoxioLicenceType.AdoxioName` | `adoxio_LicenceType?.Name` (EntityReference.Name) |
| `AdoxioLicencee.AdoxioOrgbookorganizationlink` | Separate `GetAccountByIdAsync` call |
| `AdoxioLicencee.AdoxioBcincorporationnumber` | Separate `GetAccountByIdAsync` call |
| `AdoxioEffectivedate` | `adoxio_EffectiveDate` (DateTime? → DateTimeOffset? conversion) |
| `AdoxioExpirydate` | `adoxio_ExpiryDate` (DateTime? → DateTimeOffset? conversion) |
| `AdoxioEstablishment?.AdoxioName` | `adoxio_establishment?.Name` (EntityReference.Name) |
| `AdoxioEstablishmentaddressstreet` | `adoxio_EstablishmentAddressStreet` |
| `AdoxioEstablishmentaddresscity` | `adoxio_EstablishmentAddressCity` |
| `AdoxioEstablishmentaddresspostalcode` | `adoxio_EstablishmentAddressPostalCode` |
| `AdoxioOrgbookcredentialresult` | `adoxio_OrgBookCredentialResult` (enum `adoxio_licences_adoxio_orgbookcredentialresult`) |
| `AdoxioOrgbookcredentialid` | `adoxio_OrgBookCredentialID` |
| `AdoxioOrgbookcredentiallink` | `adoxio_OrgBookCredentialLink` |

### Account (Dataverse SDK)
| Old AutoRest | New Dataverse SDK |
|---|---|
| `AdoxioBcincorporationnumber` | `adoxio_BCIncorporationNumber` |
| `AdoxioOrgbookorganizationlink` | `adoxio_OrgBookOrganizationLink` |
| `AdoxioIsorgbooklinkfound = 845280000` | `adoxio_IsOrgbookLinkFound = adoxio_account_adoxio_isorgbooklinkfound.Yes` |
| `AdoxioIsorgbooklinkfound = 845280001` | `adoxio_IsOrgbookLinkFound = adoxio_account_adoxio_isorgbooklinkfound.No` |

## Constructor Pattern

`OrgBookController` uses two constructors:
- Primary: `(IConfiguration, ILoggerFactory, IDataverseClient)` — used by gRPC DI via `MapGrpcService<OrgBookController>()`.
- Forwarding: `(IConfiguration, ILoggerFactory)` — creates a `DataverseClient` internally. Preserved for Hangfire job registration in `Startup.cs` which uses `new OrgBookController(Configuration, _loggerFactory)`.

## Related Entity Access Pattern

`adoxio_Licencee` and `adoxio_LicenceType` are `EntityReference` objects (not full entities). To access fields beyond the primary key and name:

- **LicenceType name**: `item.adoxio_LicenceType?.Name` — EntityReference.Name holds the primary name field, which is `adoxio_name` for licence types.
- **Licencee account fields** (e.g., `adoxio_BCIncorporationNumber`): requires a separate `GetAccountByIdAsync` call. `SyncLicencesToOrgbook` caches account lookups in a local `Dictionary<Guid, Account?>` to avoid redundant fetches.

## DateTime Conversion

`adoxio_licences.adoxio_EffectiveDate` and `adoxio_ExpiryDate` are `DateTime?`. The `Attributes`/`CRSAttributes` models expect `DateTimeOffset?`. Converted with UTC timezone:
```csharp
licence.adoxio_EffectiveDate.HasValue
    ? new DateTimeOffset(licence.adoxio_EffectiveDate.Value, TimeSpan.Zero)
    : (DateTimeOffset?)null
```
