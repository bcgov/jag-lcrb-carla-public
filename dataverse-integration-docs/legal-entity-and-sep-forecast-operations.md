# Legal Entity & SEP Drink Sales Forecast Operations (Phase 2 Prerequisites)

Prerequisite DV SDK infrastructure added to unblock `LegalEntitiesController` and `SpecialEventsController` migrations.

## Legal Entity (adoxio_legalentity)

### New IDataverseClient methods

| Method | Description |
|---|---|
| `UpdateLegalEntityAsync(entity)` | Updates an existing legal entity record |
| `GetLegalEntityByAccountIdAsync(accountId)` | Returns first legal entity linked to an account; delegates to `GetLegalEntitiesByAccountIdAsync` + FirstOrDefault |

These complement the pre-existing `GetLegalEntityByIdAsync`, `GetLegalEntitiesByAccountIdAsync`, `GetLegalEntitiesByParentEntityIdAsync`, `CreateLegalEntityAsync`, and `DeleteLegalEntityAsync`.

### New model extension

Added `CopyValues(this DvLegalEntity to, LegalEntity from)` to `cllc-public-app/Models.Extensions/Adoxio_LegalEntity.cs`.

Maps all view model fields to their DV SDK property equivalents:

- Enum conversions: `isindividual`/`sameasapplyingperson` → `adoxio_generalyesno` enum; `legalentitytype` → `adoxio_applicanttypecodes`; `partnerType` → `adoxio_partnertype`
- Bool fields (`isApplicant`, `isPartner`, `isShareholder`, etc.) map directly as `bool?`
- `interestpercentage` (`decimal?`) cast to `double?` for `adoxio_InterestPercentage`

### adoxio_licenseechangelog — no DV type

`adoxio_licenseechangelog` has no generated DV entity class (confirmed via Glob). `LegalEntitiesController` uses `DeleteByLogicalNameAsync` for changelog deletes and `GetLicenseeChangelogIdsByAccountIdAsync` (already in `IDataverseClient`) for reads. The controller remains a hybrid during migration.

## SEP Drink Sales Forecast (adoxio_sepdrinksalesforecast)

### New IDataverseClient methods

| Method | Description |
|---|---|
| `GetSepDrinkSalesForecastsByEventIdAsync(eventId)` | Query all forecasts by `adoxio_specialevent` FK |
| `CreateSepDrinkSalesForecastAsync(forecast)` | Creates record; returns new `Guid` |
| `UpdateSepDrinkSalesForecastAsync(forecast)` | Updates existing record |
| `DeleteSepDrinkSalesForecastAsync(id)` | Deletes record by GUID |

### FK field

Child-to-parent link: `adoxio_specialevent` (lookup field on `adoxio_sepdrinksalesforecast`).

## Special Event sub-entities — no DV types

The following SEP sub-entity types have no generated DV entity class and cannot use the SDK typed API:

| Logical name | Status |
|---|---|
| `adoxio_specialeventlocation` | No DV type — keep AutoRest |
| `adoxio_specialeventlicencedarea` | No DV type — keep AutoRest |
| `adoxio_specialeventschedule` | No DV type — keep AutoRest |
| `adoxio_specialeventtandc` | No DV type — keep AutoRest |

`SpecialEventsController` will remain a hybrid: top-level `adoxio_specialevent` and `adoxio_sepdrinksalesforecast` via `IDataverseClient`; sub-entity CRUD stays on `IDynamicsClient`.

## Files modified

- `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` — 6 new method signatures
- `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` — 6 new implementations
- `cllc-public-app/Models.Extensions/Adoxio_LegalEntity.cs` — `CopyValues(this DvLegalEntity, LegalEntity)` added
