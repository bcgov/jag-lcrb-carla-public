# SiteminderAuthenticationHandler Migration

## Ticket
No specific ticket — prerequisite for full public-app migration to `IDataverseClient`.

## File
`cllc-public-app/Authentication/SiteminderAuthenticationHandler.cs`

## What changed

### Removed
- `private IDynamicsClient _dynamicsClient;` field (was assigned via DI in `HandleAuthenticateAsync`)
- `using Gov.Lclb.Cllb.Interfaces.Models;` (AutoRest model types)
- `using Microsoft.Rest;`
- `using FolderSegment = Gov.Lclb.Cllb.Interfaces.FolderSegment;`
- All 23 `_dynamicsClient` references replaced with `_dataverse` calls

### Added (IDataverseClient + DataverseClient)
Three new methods added to `IDataverseClient` / `DataverseClient` to replace complex AutoRest extension methods from `DynamicsExtensions.cs`:

| New method | Replaces |
|---|---|
| `GetContactByLoginAsync(bool isServicesCard, string siteminderId)` | `GetActiveContactByExternalIdBridged(bool, string)` — queries `adoxio_login` by type + externalid, then resolves contact |
| `GetContactByDetailsAsync(string? firstname, string? middlename, string? lastname, string? email)` | `GetContactByContactVmBlankSmGuid(contactVM)` — queries Contact with all provided fields AND `statecode=0` |
| `GetContactByNameAndBirthdateAsync(string firstName, string lastName, string birthDate)` | BC Services Card fallback lookup — filters by lastname then matches first-initial + birthdate client-side |

### Inlined private methods
`LoadUserLegacyAsync(DvIDataverseClient, string smGuid, IHeaderDictionary)` replaces the large `DynamicsExtensions.LoadUserLegacy`. Key logic:

- If `smGuid` parses as a `Guid` → BCeID path: `GetContactByExternalIdAsync`, fallback to `GetContactByDetailsAsync` (only accepted when `adoxio_ExternalID` is empty)
- Otherwise → BC Services Card path: `ParseServiceCardId(smGuid)` extracts the canonical ID, tries `GetContactByExternalIdAsync`, then `GetContactByNameAndBirthdateAsync`
- Both paths patch the contact afterwards (`CopyValues` for BCeID, `CopyValuesNoEmailPhone` for BCSC) and update workers for BCSC logins

`ParseServiceCardId(string)` replaces the standalone `DynamicsExtensions.GetServiceCardID` — splits on `|` and `:` then sanitizes with `GuidUtility.SanitizeGuidString`.

### CreateSharePoint*DocumentLocation rewrites
The three private SharePoint location helpers were rewritten to accept DV types instead of AutoRest types:

- `CreateSharePointAccountDocumentLocation(FileManagerClient, DvAccount)` — folder name: `{account.Name}_{ID_upper_no_dashes}`
- `CreateSharePointContactDocumentLocation(FileManagerClient, DvContact)` — folder name: `contact_{ID_upper_no_dashes}`
- `CreateSharePointWorkerDocumentLocation(FileManagerClient, DvWorker)` — folder name: `{worker.adoxio_name}_{ID_upper_no_dashes}`

`CleanGuidForSharePoint(Guid)` is a new private helper computing the canonical SharePoint ID format.

### HandleVerifiedIndividualLogin
- `_dynamicsClient.Contacts.GetByKey(...)` → `_dataverse.GetContactByIdAsync(userSettings.ContactId)`
- `_dynamicsClient.Contacts.Update(...)` → `_dataverse.UpdateContactAsync(patchContact)` where patchContact has only the patched fields via `DvContact.CopyValues(contactVM)`
- Previous-address creation preserved using the existing `_dataverse.CreatePreviousAddressAsync`

### HandleWorkerLogin
- `_dynamicsClient.Workers.Get(filter:...)` → `_dataverse.GetWorkersByContactIdAsync(userSettings.ContactId)`
- Patch worker manually sets only `adoxio_FirstName/LastName/MiddleName/GenderCode` — does NOT use `CopyValuesNoEmailPhone` to avoid overwriting other fields
- `_dynamicsClient.Workers.Update(...)` → `_dataverse.UpdateWorkerAsync(patchWorker)`
- `_dynamicsClient.GetWorkerByIdWithChildren(...)` → `_dataverse.GetWorkerByIdWithChildrenAsync(id)`

### HandleBridgeAuthentication / HandleLegacyAuthentication
- Both resolve `_dataverse` and `_fileManagerClient` from `context.RequestServices` at call time (no stored field)
- `LoadUserLegacy` → `LoadUserLegacyAsync`
- `GetActiveAccountBySiteminderBusinessGuid` → `_dataverse.GetAccountByExternalIdAsync(GuidUtility.SanitizeGuidString(siteMinderBusinessGuid))`
- `GetActiveAccountByLegalName` → `_dataverse.GetAccountByNameAsync(...)` with inline statecode + empty-ExternalID check
- Account `adoxio_ExternalID` and `Id` accessed as DV SDK properties directly

### LoginDevUser
- Parameter changed from `IDynamicsClient` to `DvIDataverseClient`
- Uses `LoadUserLegacyAsync` internally

## Patterns used
- `DvIDataverseClient` resolved via `context.RequestServices.GetService(typeof(DvIDataverseClient))` (no constructor injection, to match the handler's existing service-locator style)
- `GetAttributeValue<OptionSetValue>("statecode")?.Value == 0` for active-record check on DV Account
- `GuidUtility.SanitizeGuidString` for normalizing raw SiteMinder GUIDs before querying
- `extern alias DV` with `DvContact`, `DvAccount`, `DvWorker`, `DvGender`, `DvIDataverseClient`, `DvPreviousAddress` aliases to keep the two namespaces distinct
