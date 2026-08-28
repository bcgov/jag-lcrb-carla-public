# LegalEntitiesController Migration

**File:** `cllc-public-app/Controllers/LegalEntitiesController.cs`
**Ticket:** LCSD-8539 (DV migration phase)

---

## Summary

Full replacement of `IDynamicsClient` with `IDataverseClient`. No hybrid — the old AutoRest client is completely removed.

---

## What changed

### Constructor
- Removed `IDynamicsClient dynamicsClient`
- Added `IDataverseClient dataverse` (alias: `using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;`)

### Action methods

| Old | New |
|---|---|
| `_dynamicsClient.Legalentities.Get(filter:...)` | `_dataverse.GetLegalEntitiesByAccountIdAsync(accountId)` |
| `_dynamicsClient.GetLegalEntityTree(...)` | Private `GetLegalEntityTreeAsync(accountId)` |
| `_dynamicsClient.GetLicensesByLicencee(...)` | `_dataverse.GetLicencesByAccountIdAsync(accountId)` |
| `_dynamicsClient.GetApplicationsForLicenceByApplicant(...)` | `_dataverse.GetApplicationsByAccountIdAsync(accountId)` |
| `_dynamicsClient.GetApplicationChangeLogs(...)` | `_dataverse.GetLicenseeChangelogsByApplicationIdAsync(...)` |
| `_dynamicsClient.Licenseechangelogs.Get(filter:...)` | `_dataverse.GetLicenseeChangelogsByAccountIdAsync(...)` |
| `_dynamicsClient.GetAdoxioLegalentityByAccountId(...)` | `_dataverse.GetLegalEntityByAccountIdAsync(...)` |
| `_dynamicsClient.GetLegalEntityById(...)` | `_dataverse.GetLegalEntityByIdAsync(...)` |
| `_dynamicsClient.GetAccountByIdAsync(...)` | `_dataverse.GetAccountByIdAsync(...)` |
| `_dynamicsClient.Legalentities.Create(...)` | `_dataverse.CreateLegalEntityAsync(...)` |
| `_dynamicsClient.Legalentities.Update(...)` | `_dataverse.UpdateLegalEntityAsync(...)` |
| `_dynamicsClient.Legalentities.DeleteAsync(...)` | `_dataverse.DeleteLegalEntityAsync(...)` |
| `_dynamicsClient.Accounts.Create(...)` | `_dataverse.CreateAccountAsync(...)` |
| `_dynamicsClient.Tiedhouseconnections.Create(...)` | `_dataverse.CreateTiedHouseConnectionAsync(...)` |
| `_dynamicsClient.Licenseechangelogs.Create(...)` | `_dataverse.CreateLicenseeChangelogAsync(...)` |
| `_dynamicsClient.Licenseechangelogs.Update(...)` | `_dataverse.UpdateLicenseeChangelogAsync(...)` |
| `_dynamicsClient.Licenseechangelogs.Delete(...)` | `_dataverse.DeleteLicenseeChangelogAsync(...)` |
| `DynamicsExtensions.CurrentUserHasAccessToAccount(...)` | `await DynamicsExtensions.CurrentUserHasAccessToAccountAsync(...)` |

### Private helpers rewritten as async

- `GetLegalEntityTree(string)` → `GetLegalEntityTreeAsync(string)` — fetches all entities for account, picks root (null `adoxio_LegalEntityOwned`), recurses via children
- `GetLegalEntityChildren(string, List<string>)` → `GetLegalEntityChildrenAsync(...)` — uses `GetLegalEntitiesByParentEntityIdAsync`
- `GetAccountLegalEntities(string)` → `GetAccountLegalEntitiesAsync(string)` — returns `List<DvLegalEntity>`, filters `adoxio_IsIndividual != Yes`
- `SaveChangeObjects(...)` → `SaveChangeObjectsAsync(...)` — uses `DvChangelog.CopyValues` + `EntityReference` for all FK binds
- `SaveAccountChangeObjects(...)` → `SaveAccountChangeObjectsAsync(...)` — same pattern

### OData bind → EntityReference

All OData bind strings (`_dynamicsClient.GetEntityURI(...)`) replaced with:
```csharp
new EntityReference("logicalname", Guid.Parse(id))
```

### Screening data

Contact lookup changed from AutoRest `_dynamicsClient.GetContactById(...)` to `_dataverse.GetContactByIdAsync(...)`.
DV property names:
- `adoxio_PHSComplete` (enum `adoxio_contact_adoxio_phscomplete`, `Yes = 845280000`)
- `adoxio_PHSDateSubmitted` (PascalCase, `DateTime?`)
- `adoxio_cascomplete` (lowercase, enum `adoxio_contact_adoxio_cascomplete`)
- `adoxio_casdatesubmitted` (lowercase, `DateTime?`)

---

## DV entity quirks discovered

| Entity | Property | Issue |
|---|---|---|
| `adoxio_tiedhouseconnection` | Account FK | Property is `adoxio_AccountId`, not `adoxio_Account` |
| `adoxio_application` | Application type FK | Property is `adoxio_ApplicationTypeId`, not `adoxio_ApplicationType` |

---

## Prerequisites completed in same migration

- `CopyValues(this DvLegalEntity, LegalEntity)` — `Adoxio_LegalEntity.cs`
- `CopyValues(this DvChangelog, LicenseeChangeLog)` + `ToViewModel(this DvChangelog)` — `LicenseeChangeLog.cs`
- `UpdateLegalEntityAsync`, `GetLegalEntityByAccountIdAsync` — `IDataverseClient` + `DataverseClient`
- Full changelog CRUD (6 methods) — `IDataverseClient` + `DataverseClient`
- SEP sub-entity types generated: `adoxio_specialeventlocation`, `adoxio_specialeventlicencedarea`, `adoxio_specialeventschedule`, `adoxio_specialeventtandc`

---

## Test steps

1. Build passes: `dotnet build cllc-public-app/cllc-public-app.csproj`
2. `GET /api/legalentities` returns legal entities for the current user's account
3. `GET /api/legalentities/current-hierarchy` returns nested tree
4. `GET /api/legalentities/applicant` returns root legal entity with account populated
5. `POST /api/legalentities` creates a legal entity linked to user's account
6. `POST /api/legalentities/child-legal-entity` creates a shareholder/partner entity with account + tied house
7. `PUT /api/legalentities/{id}` updates an existing entity
8. `POST /api/legalentities/{id}/delete` removes the entity
9. `POST /api/legalentities/save-change-tree/{applicationId}` saves a change log tree
10. `POST /api/legalentities/cancel-change-logs` deletes the specified change logs
