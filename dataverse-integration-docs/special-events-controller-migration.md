# LCSD-8539/8550: SpecialEventsController Migration (Complete)

Fully migrates `cllc-public-app/Controllers/SpecialEventsController.cs` from `IDynamicsClient` (AutoRest) to `IDataverseClient` (Dataverse SDK). All AutoRest references removed.

## Scope

All methods migrated — no `_dynamicsClient` remains in the file.

### Main-entity CRUD
- `CreateSpecialEvent` — `CreateSpecialEventAsync`, sub-entities via typed DV create methods
- `UpdateSpecialEvent` — loads existing sub-entities via DV lists, delete/recreate pattern for areas/schedules
- `GenerateInvoice` — `GetSpecialEventByIdAsync`, sets `adoxio_InvoiceTrigger = true`
- `Submit` — `GetSpecialEventByIdAsync`, sets `statuscode = adoxio_specialevent_statuscode.Submitted`

### PDF generation
- `GetSummaryPdf` / `GetPermitPDF` — parallel DV loads (se, contact, city, locations, areas, schedules, forecasts, invoice, T&Cs)
- `StoreCopyOfPdf` — `GetFolderNameAsync`

### Terms & Conditions
- `UpdateSpecialEventTermsAndConditions` — `GetSpecialEventTandCsByEventIdAsync`, `CreateSpecialEventTandCAsync`, `UpdateSpecialEventTandCAsync`, `DeleteSpecialEventTandCAsync`

### Forecasts / Drink types
- `SaveTotalServingsAsync` — `GetSepDrinkSalesForecastsByEventIdAsync`, `GetSepDrinkTypesAsync`
- `CreateOrUpdateForecastAsync` — `CreateSepDrinkSalesForecastAsync` / `UpdateSepDrinkSalesForecastAsync`
- `GetDrinkTypes` — `GetSepDrinkTypesAsync` + `dt.ToViewModel()`

### Police listing
- `GetPoliceCurrent` — `GetSpecialEventsByJurisdictionAsync` x3 in parallel
- `GetPolicePendingReview/Approved/Denied` — `GetSpecialEventsByJurisdictionPagedAsync` returning `(items, totalCount)`
- `GetPoliceMy` — `GetSpecialEventsByRepresentativeAsync` x3 in parallel
- `GetPoliceHome` — `GetSpecialEventsByRepresentativeAsync` with status/approval filters

### Police actions
- `PoliceAssign` — `GetAccountByIdAsync`, `GetSpecialEventByIdAsync`, `GetContactByIdAsync`; jurisdiction check via `Id` comparison
- `PoliceApprove/Deny/Cancel/SetMunicipality` — `GetAccountByIdAsync`, `GetSpecialEventByIdAsync`; DV patch

### Misc
- `GetAutocomplete` — `GetSepCitiesFilteredAsync(nameContains, defaultsOnly)` + `c.ToViewModel()`
- `getClaimInfo` — `GetSpecialEventByLicenceNumberAsync` + status check
- `linkClaimToContact` — `GetSpecialEventByLicenceNumberAsync` + `UpdateSpecialEventAsync`

## Deleted Helpers

| Helper | Reason |
|---|---|
| `GetSpecialEventData` | Monolithic AutoRest expand loader — replaced by parallel DV loads per caller |
| `GetSepSummaries` | AutoRest filter wrapper — replaced by `GetSpecialEventsByJurisdictionAsync` / `GetSpecialEventsByRepresentativeAsync` |
| `GetPagedSepSummaries` | AutoRest paged OData query — replaced by `GetSpecialEventsByJurisdictionPagedAsync` |

## Sub-entity Operations Pattern

Old AutoRest approach loaded all sub-entities via OData `$expand`. DV approach loads each collection separately:

```csharp
var locationsTask = _dataverse.GetSpecialEventLocationsByEventIdAsync(eventId);
var areasTask     = _dataverse.GetSpecialEventLicencedAreasByEventIdAsync(eventId);
var schedulesTask = _dataverse.GetSpecialEventSchedulesByEventIdAsync(eventId);
await Task.WhenAll(locationsTask, areasTask, schedulesTask);
```

For updates, areas and schedules use a **delete + recreate** pattern (no `UpdateSpecialEventLicencedAreaAsync` / `UpdateSpecialEventScheduleAsync` exist). Locations have `UpdateSpecialEventLocationAsync`.

## Jurisdiction Check Pattern

```csharp
// AutoRest (string GUID comparison)
userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue

// Dataverse SDK (EntityReference.Id comparison)
specialEvent.adoxio_PoliceJurisdictionId?.Id != userAccount.adoxio_PoliceJurisdictionId.Id
```

## Paged Results Pattern

```csharp
var (items, totalCount) = await _dataverse.GetSpecialEventsByJurisdictionPagedAsync(
    jurisdictionId, policeApprovals, excludeStatuses, pageIndex, pageSize, orderByField, sortDir);

return new JsonResult(new PagingResult<SpecialEventSummary>
{
    Value = items.Select(se => se.ToSummaryViewModel()).ToList(),
    Count = totalCount
});
```

## Sort Column Fix

`policeDecisionBy` in `transformColumnNametoSchemaName` now maps to `adoxio_policerepresentativeid` (was incorrectly `_adoxio_policerepresentativeid_value`).

## Key Aliases (top of file)

```csharp
extern alias DV;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DvSpecialEvent   = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent;
using adoxio_specialevent_statuscode         = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent_statuscode;
using adoxio_specialevent_adoxio_policeapproval = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialevent_adoxio_policeapproval;
using DvLocation   = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialeventlocation;
using DvArea       = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialeventlicencedarea;
using DvSchedule   = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialeventschedule;
using DvTandC      = DV::Gov.Lclb.Cllb.Interfaces.adoxio_specialeventtandc;
using DvForecast   = DV::Gov.Lclb.Cllb.Interfaces.adoxio_sepdrinksalesforecast;
using DvDrinkType  = DV::Gov.Lclb.Cllb.Interfaces.adoxio_sepdrinktype;
using DvCity       = DV::Gov.Lclb.Cllb.Interfaces.adoxio_sepcity;
using DataverseContact = DV::Gov.Lclb.Cllb.Interfaces.Contact;
using DvAccount    = DV::Gov.Lclb.Cllb.Interfaces.Account;
using DvInvoice    = DV::Gov.Lclb.Cllb.Interfaces.Invoice;
```

## Property Naming (AutoRest → Dataverse SDK)

| AutoRest | Dataverse SDK |
|---|---|
| `AdoxioSpecialeventid` (string) | `Id` (Guid) |
| `_adoxioPolicejurisdictionidValue` (string GUID) | `adoxio_PoliceJurisdictionId.Id` (Guid) |
| `AdoxioInvoicetrigger` | `adoxio_InvoiceTrigger` |
| `AdoxioPoliceapproval` (int) | `adoxio_PoliceApproval` (enum) |
| `AdoxioDenialreason` | `adoxio_DenialReason` |
| `AdoxioCancellationreason` | `adoxio_CancellationReason` |
| `AdoxioDatepoliceapproved` | `adoxio_DatePoliceApproved` |
| `SepCityODataBind` | `adoxio_SpecialEventCityDistrictId` (EntityReference) |
| `ContactODataBind` | `adoxio_ContactId` (EntityReference) |
| `AccountODataBind` | `adoxio_AccountId` (EntityReference) |
| `PoliceRepresentativeIdODataBind` | `adoxio_PoliceRepresentativeId` (EntityReference) |
| `AdoxioSpecialeventpermitnumber` | `adoxio_SpecialEventPermitNumber` |
| `_adoxioContactidValue` | `adoxio_ContactId?.Id.ToString()` |
