# LCSD-8548: Migrate ContactController to IDataverseClient

Covers the migration of `cllc-public-app/Controllers/ContactController.cs` from `IDynamicsClient` (AutoRest REST) to `IDataverseClient` (Dataverse SDK).

## New IDataverseClient methods added

All added to `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` and implemented in `DataverseClient.cs`:

| Method | Purpose |
|---|---|
| `GetContactByExternalIdAsync(string externalId)` | Query contact by `adoxio_externalid` with `statecode = 0` filter |
| `CreateAliasAsync(adoxio_alias alias)` | Create an `adoxio_alias` record |
| `UpdateAliasAsync(adoxio_alias alias)` | Update an `adoxio_alias` record |
| `CreateWorkerSharePointDocLocAsync(string workerId, string folderName)` | Get-or-create parent SharePoint library for `adoxio_worker`, then create a `SharePointDocumentLocation` linked to the worker |

## Model extension methods added

### Models.Extensions/Contact.cs
- `ToViewModel(this DataverseContact contact)` — maps all Xrm.Sdk Contact properties, including `BirthDate` (`DateTime?` → `DateTimeOffset?`) and typed enum casts
- `CopyValues(this DataverseContact to, Contact from)` — delegates to `CopyValuesNoEmailPhone` + email/phone
- `CopyValuesNoEmailPhone(this DataverseContact to, Contact from)` — full field mapping with `adoxio_*` enum casts and `Birthdate?.DateTime` conversion
- `CopyHeaderValues(this DataverseContact to, IHttpContextAccessor)` — reads SMGOV_* headers
- `CopyContactUserSettings(this DataverseContact contact, Contact newContact)` — copies user settings fields

### Models.Extensions/Alias.cs
- `ToViewModel(this adoxio_alias alias)` — maps id/firstname/middlename/lastname
- `CopyValues(this adoxio_alias to, ViewModels.Alias from, string contactId)` — copies fields and optionally sets `adoxio_ContactId` via `EntityReference`

### Models.Extensions/Worker.cs
- `CopyValues(this adoxio_worker to, ViewModels.Worker from)` — delegates to `CopyValuesNoEmailPhone` + phone/email
- `CopyValuesNoEmailPhone(this adoxio_worker to, ViewModels.Worker from)` — maps all worker fields using typed Xrm.Sdk enums (`adoxio_generalyesno`, `adoxio_worker_statuscode`, `adoxio_worker_statecode`)

## DynamicsExtensions changes

Added async `IDataverseClient` overloads to `cllc-public-app/Contexts/DynamicsExtensions.cs`:

- `CurrentUserHasAccessToAccountAsync(Guid, IHttpContextAccessor, IDataverseClient)` — replaces the sync `IDynamicsClient` overload
- `IsChildAccountAsync(string, string, IDataverseClient)` — recursive child-account check using `GetLegalEntitiesByAccountIdAsync` + `adoxio_ShareholderAccountID?.Id`

## Key migration patterns

**`extern alias DV` in all consumer files.** Because `Dynamics-Dataverse.csproj` is referenced with `<Aliases>DV</Aliases>` in `cllc-public-app.csproj`, every file that uses Dataverse SDK types must declare `extern alias DV;` at the top and alias each type explicitly:
```csharp
extern alias DV;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DvContact = DV::Gov.Lclb.Cllb.Interfaces.Contact;
// etc.
```
Files updated: `ContactController.cs`, `Models.Extensions/Contact.cs`, `Models.Extensions/Alias.cs`, `Models.Extensions/Worker.cs`, `Contexts/DynamicsExtensions.cs`.

**Deep-insert → sequential create.** AutoRest used `worker.AdoxioContactId = contact` (OData deep-insert). The Dataverse SDK requires creating contact first, then linking: `worker.adoxio_ContactId = new EntityReference(DvContact.EntityLogicalName, contactId)`.

**`CreateAlias` helper.** The old two-step (create alias, then PATCH contact navigation) is replaced by setting `adoxio_ContactId` at creation time via `EntityReference` in `adoxio_alias.CopyValues(item, contactId)`.

**`BirthDate` type difference.** Xrm.Sdk `Contact.BirthDate` is `DateTime?`; the ViewModel `Birthdate` is `DateTimeOffset?`. Conversion: `contact.BirthDate.HasValue ? new DateTimeOffset(contact.BirthDate.Value) : (DateTimeOffset?)null` (ToViewModel) and `from.Birthdate?.DateTime` (CopyValues).

**`adoxio_IsWorker` type difference.** Xrm.Sdk uses `bool?`; AutoRest/ViewModel uses `int?`. Conversion: `contact.adoxio_IsWorker.HasValue ? (contact.adoxio_IsWorker.Value ? (int?)1 : 0) : null`.
