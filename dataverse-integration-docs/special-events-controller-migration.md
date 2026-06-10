# LCSD-8550: SpecialEventsController Migration

Migrates `cllc-public-app/Controllers/SpecialEventsController.cs` main-entity CRUD from `IDynamicsClient` (AutoRest) to `IDataverseClient` (Dataverse SDK).

## Scope

**Fully migrated (main entity `adoxio_specialevent`):**
- `CreateSpecialEvent` — creates via `_dataverse.CreateSpecialEventAsync`, sets invoice trigger via `UpdateSpecialEventAsync`
- `UpdateSpecialEvent` — patches via `_dataverse.UpdateSpecialEventAsync` with `EntityReference` for SepCity
- `GenerateInvoice` — sets `adoxio_InvoiceTrigger = true` via `UpdateSpecialEventAsync`
- `Submit` — sets `statuscode = adoxio_specialevent_statuscode.Submitted` via `UpdateSpecialEventAsync`
- `PoliceAssign` — sets `adoxio_PoliceRepresentativeId` with `EntityReference("contact", guid)`
- `PoliceApprove` — sets `adoxio_PoliceApproval = adoxio_specialevent_adoxio_policeapproval.Reviewed`
- `PoliceDeny` — sets `adoxio_PoliceApproval = Denied`, `adoxio_DenialReason`, `adoxio_DatePoliceApproved`
- `PoliceCancel` — sets `adoxio_PoliceApproval = Cancelled`, `adoxio_CancellationReason`, `adoxio_DatePoliceApproved`
- `PoliceSetMunicipality` — sets `adoxio_SpecialEventCityDistrictId` with `EntityReference("adoxio_sepcity", guid)`
- `linkClaimToContact` — sets `adoxio_ContactId` + optionally `adoxio_AccountId` with EntityReferences

**Kept on `_dynamicsClient` (sub-entities — no generated Dataverse types):**
- `adoxio_specialeventlocation`, `adoxio_specialeventschedule`, `adoxio_specialeventlicencedarea`, `adoxio_specialeventtandc`
- `GetSpecialEventData` private helper (reads with sub-entity expand, deep-joined result still on AutoRest)
- Police listing queries (`GetSepSummaries`, `GetSepEvents`, etc.)

## New IDataverseClient methods added (LCSD-8550)

Three lookup methods added to support portal SEP form dropdowns:

```csharp
// In IDataverseClient / DataverseClient
Task<IList<adoxio_sepcity>> GetSepCitiesAsync(CancellationToken ct = default);
Task<adoxio_sepcity?> GetSepCityByIdAsync(string id, CancellationToken ct = default);
Task<IList<adoxio_sepdrinktype>> GetSepDrinkTypesAsync(CancellationToken ct = default);
```

## Extension methods added (Models.Extensions/SpecialEvent.cs)

Two new overloads added alongside the existing AutoRest overloads:

```csharp
// Read DV entity → ViewModel
public static ViewModels.SpecialEvent ToViewModel(this DvSpecialEvent se)

// Write ViewModel values → DV patch entity
public static void CopyValues(this DvSpecialEvent to, ViewModels.SpecialEvent from)
```

## Key patterns

### extern alias + using aliases
```csharp
extern alias DV;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DvSpecialEvent = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent;
using adoxio_specialevent_statuscode = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent_statuscode;
using adoxio_specialevent_adoxio_policeapproval = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent_adoxio_policeapproval;
```

### EntityReference replaces ODataBind
```csharp
// AutoRest
patchEvent.SepCityODataBind = _dynamicsClient.GetEntityURI("adoxio_sepcities", cityId);

// Dataverse SDK
dvPatch.adoxio_SpecialEventCityDistrictId = new EntityReference("adoxio_sepcity", cityGuid);
```

### Police approval enum values
```csharp
adoxio_specialevent_adoxio_policeapproval.Reviewed   // 845280000 — Approved
adoxio_specialevent_adoxio_policeapproval.Denied     // 845280001
adoxio_specialevent_adoxio_policeapproval.Cancelled  // 845280002
```

### Status code enum values
```csharp
adoxio_specialevent_statuscode.Draft      // 845280001
adoxio_specialevent_statuscode.Submitted  // 845280002
adoxio_specialevent_statuscode.Issued     // 845280003
adoxio_specialevent_statuscode.Cancelled  // 845280004
```

### Patch entity ID — always set explicitly
```csharp
var dvPatch = new DvSpecialEvent();
if (Guid.TryParse(eventId, out var guid))
    dvPatch.Id = guid;
await _dataverse.UpdateSpecialEventAsync(dvPatch);
```

## Property naming gotchas (AutoRest → Dataverse SDK)

| AutoRest | Dataverse SDK |
|---|---|
| `AdoxioInvoicetrigger` | `adoxio_InvoiceTrigger` |
| `AdoxioPoliceapproval` (int) | `adoxio_PoliceApproval` (enum) |
| `AdoxioDenialreason` | `adoxio_DenialReason` |
| `AdoxioCancellationreason` | `adoxio_CancellationReason` |
| `AdoxioDatepoliceapproved` | `adoxio_DatePoliceApproved` |
| `SepCityODataBind` | `adoxio_SpecialEventCityDistrictId` (EntityReference) |
| `ContactODataBind` | `adoxio_ContactId` (EntityReference) |
| `AccountODataBind` | `adoxio_AccountId` (EntityReference) |
| `PoliceRepresentativeIdODataBind` | `adoxio_PoliceRepresentativeId` (EntityReference) |
