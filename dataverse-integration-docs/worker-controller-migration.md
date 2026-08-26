# LCSD-8555: WorkerController Migration

Migrates `cllc-public-app/Controllers/WorkerController.cs` from `IDynamicsClient` (AutoRest) to `IDataverseClient` (Dataverse SDK). Zero `IDynamicsClient` references remain in the controller.

## New interface methods added (LCSD-8555)

| Method | Description |
|---|---|
| `GetWorkersByContactIdAsync(contactId)` | Returns all workers linked to a contact GUID |
| `DeleteWorkerAsync(id)` | Deletes a worker record by GUID string |

Both implemented in `DataverseClient.cs` alongside the existing Worker section.

## New extension method added

`ToViewModel(this adoxio_worker worker)` added to `cllc-public-app/Models.Extensions/Worker.cs`.

Key mapping notes:
- `adoxio_IsLDBWorker`, `adoxio_SelfDisclosure`, `adoxio_TriggerPHS`, `adoxio_PaymentReceived` are `adoxio_generalyesno?` enums (Yes = 1, No = 0) — compared with `.HasValue` guard before converting to `bool?`
- `adoxio_DateofBirth`, `adoxio_PaymentReceivedDate`, `adoxio_CurrentAddressDateFrom`, `ModifiedOn` are `DateTime?` — cast to `DateTimeOffset?` via implicit conversion
- `statuscode` / `statecode` are typed enum optionsets — cast to `ViewModels.StatusCode` via `(int)` intermediate
- `adoxio_ContactId` is an `EntityReference` — only the `.Id` is used to populate `contact.id`; full contact is fetched separately when needed

## Controller changes

| Endpoint | Old pattern | New pattern |
|---|---|---|
| `GET contact/{contactId}` | `Workers.Get(filter, expand)` | `GetWorkersByContactIdAsync` |
| `GET {id}` | `Workers.Get(filter, expand).FirstOrDefault()` (sync) | `GetWorkerByIdAsync` (async) |
| `PUT {id}` | `Workers.Get` + `Workers.UpdateAsync` + `GetWorkerById` | `GetWorkerByIdAsync` + `UpdateWorkerAsync` + `GetWorkerByIdAsync` |
| `POST` | `Workers.CreateAsync` + `Workers.UpdateAsync` (link contact) | `CreateWorkerAsync` (contact set via `EntityReference` before create) |
| `POST {id}/delete` | `Workers.Get` + `Workers.DeleteAsync` | `GetWorkerByIdAsync` + `DeleteWorkerAsync` |
| `GET {workerId}/pdf` | `Workers.GetByKey(expand)` + `GetFolderName` (IDynamicsClient extension) | `GetWorkerByIdAsync` + `GetContactByIdAsync` + `GetSharePointDocLocsByObjectIdAsync` |

### Contact relationship — create pattern

The old AutoRest client required a two-step create (create worker, then PATCH with `ContactIdAccountODataBind`). The Dataverse SDK supports setting the relationship before creation:

```csharp
worker.adoxio_ContactId = new EntityReference(DvContact.EntityLogicalName, Guid.Parse(item.contact.id));
var workerId = await _dataverse.CreateWorkerAsync(worker);
```

### PDF endpoint — contact address fields

`adoxioWorker.adoxio_ContactId` is an `EntityReference` (not an expanded contact). The PDF endpoint now fetches the contact separately:

```csharp
var contactId = adoxioWorker.adoxio_ContactId?.Id.ToString();
var contact = contactId != null ? await _dataverse.GetContactByIdAsync(contactId) : null;
// contact?.Address1_Line1, contact?.Address1_City, etc.
```

### PDF endpoint — folder name

`IDynamicsClient.GetFolderName` (an extension on `IDynamicsClient`) is replaced with a direct Dataverse query:

```csharp
var docLocs = await _dataverse.GetSharePointDocLocsByObjectIdAsync(entityId);
var folderName = docLocs.FirstOrDefault(d => !string.IsNullOrEmpty(d.RelativeUrl))?.RelativeUrl
    ?? $"{adoxioWorker.adoxio_name}_{entityId.Replace("-", "")}";
```

The fallback format `{adoxio_name}_{idWithoutHyphens}` matches the old `GetDocumentFolderName` output for workers.

## Files modified

| File | Change |
|---|---|
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` | Added `GetWorkersByContactIdAsync`, `DeleteWorkerAsync` |
| `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` | Implemented both new methods |
| `cllc-public-app/Models.Extensions/Worker.cs` | Added `ToViewModel(this adoxio_worker)` |
| `cllc-public-app/Controllers/WorkerController.cs` | Full migration to `IDataverseClient` |
