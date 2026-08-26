# LCSD-8549: Migrate sharepoint-sync-tool to IDataverseClient

## Summary

`sharepoint-sync-tool/SyncService.cs` syncs SharePoint document locations with Dataverse. It reads SharePoint folder names, extracts entity GUIDs, and creates `SharePointDocumentLocation` records in Dataverse linking each folder to its entity. This ticket replaced the AutoRest `IDynamicsClient` with `IDataverseClient` (Dataverse SDK).

---

## Changes Made

### `IDataverseClient.cs` — new method

```csharp
Task<IList<SharePointDocumentLocation>> GetSharePointDocLocsByRelativeUrlAsync(
    string relativeUrl, CancellationToken ct = default);
```

Queries `sharepointdocumentlocation` by `relativeurl` only (no name filter). Used by `SyncService` for:
- Looking up whether a location already exists (then inspecting `RegardingObjectId`)
- Looking up parent document library records (root-level SharePoint library references)
- Finding entity-type-folder locations filtered in-memory by `ParentSiteOrLocation.Id`

### `DataverseClient.cs` — implementation

```csharp
public async Task<IList<SharePointDocumentLocation>> GetSharePointDocLocsByRelativeUrlAsync(
    string relativeUrl, CancellationToken ct = default)
{
    var query = new QueryExpression(SharePointDocumentLocation.EntityLogicalName)
        { ColumnSet = new ColumnSet(true) };
    query.Criteria.AddCondition("relativeurl", ConditionOperator.Equal, relativeUrl);
    var result = await Task.Run(() => _serviceClient.RetrieveMultiple(query), ct);
    return result.Entities.Select(e => e.ToEntity<SharePointDocumentLocation>()).ToList();
}
```

### `SyncService.cs` — full migration

| Old (AutoRest) | New (Dataverse SDK) |
|---|---|
| `IDynamicsClient _dynamicsClient` | `IDataverseClient _dataverse` |
| `MicrosoftDynamicsCRMsharepointdocumentlocation` | `SharePointDocumentLocation` |
| `.Sharepointdocumentlocations.Get(filter:)` | `_dataverse.GetSharePointDocLocsByRelativeUrlAsync(relativeUrl)` |
| `.Sharepointdocumentlocations.Create(loc)` | `_dataverse.CreateSharePointDocLocAsync(loc)` |
| `_dynamicsClient.GetEntityURI("sharepointdocumentlocations", id)` → OData bind string | `new EntityReference(SharePointDocumentLocation.EntityLogicalName, guid)` |
| `_dynamicsClient.GetEntityURI("accounts", guid)` | `new EntityReference("account", guid)` |
| `SetRegardingObject(…)` — set OData bind properties by entity type | `GetRegardingObjectReference(entityName, entityGuid)` — returns `EntityReference?` |
| `HttpOperationException` catch + `ex.Response?.Content` | `Exception` catch + `ex.Message` |
| `location._regardingobjectidValue` | `location.RegardingObjectId?.Id.ToString()` |
| `location.Sharepointdocumentlocationid` | `location.Id.ToString()` |
| `.Relativeurl` | `.RelativeUrl` |
| `ParentsiteorlocationSharepointdocumentlocationODataBind = …` | `ParentSiteOrLocation = new EntityReference(…)` |

Key design notes:
- `GetRegardingObjectReference` replaces the old `SetRegardingObject` side-effectful method. It maps the friendly entity name (`"application"`, `"worker"`, etc.) to the Dataverse logical name and returns an `EntityReference?`.
- Entity type folder lookups (Level 2 in nested hierarchy) filter in-memory by `ParentSiteOrLocation.Id` after querying by `relativeurl`.
- `GetDocumentLocationReferenceByRelativeURL` (sync) became `GetDocumentLocationReferenceByRelativeUrlAsync` (async).
- The `AddReferenceToEntity` no-op method was removed entirely.

### `Program.cs` — DI wiring

```csharp
// Before
var dynamicsClient = DynamicsSetupUtil.SetupDynamics(configuration);
var syncService = new SyncService(sharePointManager, dynamicsClient, loggerFactory);

// After — DataverseClient already registered in the service collection above
var dataverseClient = serviceProvider.GetRequiredService<IDataverseClient>();
var syncService = new SyncService(sharePointManager, dataverseClient, loggerFactory);
```

Removed unused `using` directives: `Gov.Lclb.Cllb.Interfaces.Models`, `Microsoft.Rest`, `System.Text.RegularExpressions`.

---

## `extern alias DV` Pattern in SyncService.cs

Both `Dynamics-Autorest` and `Dynamics-Dataverse` define types in the `Gov.Lclb.Cllb.Interfaces` namespace. `Dynamics-Dataverse` is referenced with `<Aliases>DV</Aliases>` in the csproj. Dataverse types are accessed via aliases at the top of the file:

```csharp
extern alias DV;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using SharePointDocumentLocation = DV::Gov.Lclb.Cllb.Interfaces.SharePointDocumentLocation;
using EntityReference = DV::Microsoft.Xrm.Sdk.EntityReference;
```

The unaliased `using Gov.Lclb.Cllb.Interfaces` still brings in `ISharePointFileManager`, `SharePointConstants`, and `FolderItem` from the SharePoint project.

---

## ISharePointFileManager — Not Modified

`SharePointFileManager.cs`, `CloudSharePointFileManager.cs`, and `OnPremSharePointFileManager.cs` in `cllc-interfaces/SharePoint/` are SharePoint-side and were not touched.
