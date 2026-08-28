# ApplicationsController Migration

**File:** `cllc-public-app/Controllers/ApplicationsController.cs`

## Summary

Full migration of `ApplicationsController` from AutoRest `IDynamicsClient` to `IDataverseClient`. The controller previously had ~154 `_dynamicsClient` references. After migration all primary user-facing endpoints use DV SDK; a subset of PCL/LE-review methods retain AutoRest calls where no DV equivalent exists. Dead-code helpers `InitializeSharepoint(AutoRest)` and `GetApplicationFolderName` were removed; orphaned `using` directives (`System.ComponentModel`, `System.Diagnostics`, `System.Diagnostics.CodeAnalysis`, `System.Text.Encodings.Web`, `System.Text.Json`, `System.Web`, `Google.Protobuf.WellKnownTypes`, `Grpc.Core`, `Newtonsoft.Json`, `System.Net.Mime.MediaTypeNames`) were cleaned up.

## New Aliases Added

```csharp
using adoxio_licences_statuscode = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licences_statuscode;
using adoxio_applicationtype_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicationtype;
using adoxio_servicearea_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_servicearea;
using adoxio_hoursofservice_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_hoursofservice;
using adoxio_applicationextension_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicationextension;
using adoxio_tiedhouseconnection_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_tiedhouseconnection;
using adoxio_tiedhouseconnection_adoxio_connectiontype = DV::Gov.Lclb.Cllb.Interfaces.adoxio_tiedhouseconnection_adoxio_connectiontype;
using adoxio_generalyesno_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno;
using adoxio_servicehoursoptionsethours = DV::Gov.Lclb.Cllb.Interfaces.adoxio_servicehoursoptionsethours;
using adoxio_application_adoxio_manufacturerproductionamountunit_dv = DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_manufacturerproductionamountunit;
using EntityReference = DV::Microsoft.Xrm.Sdk.EntityReference;
```

## Methods Migrated

### Fully Migrated to DV

| Method | Notes |
|---|---|
| `GetApplicationSummariesByApplicantAsync` | `GetApplicationsByApplicantExpandedAsync`; app-type cached in dict; expired-licence check via `GetLicenceByIdAsync` |
| `GetApplicationsByApplicant` | Status filter with DV enum; `ToViewModelAsync` |
| `GetApprovedCannabisRetailStoreLicenceCountByApplicantAsync` | `GetLicencesByAccountIdAsync`; status = `adoxio_licences_statuscode.Active`; type name from `EntityReference.Name` |
| `GetSubmittedCannabisRetailStoreCountByApplicantAsync` | `GetApplicationTypeByNameAsync` + `GetApplicationsByApplicantAndTypeAsync` |
| `GetApprovedApplicationsCountByApplicantAsync` | `GetApplicationsByApplicantAndTypeAsync(requireStatecode0: true)` |
| `GetCountForCurrentUserSubmittedApplications` | Now `async Task<JsonResult>` |
| `GetCountOfSubmittedApplicationsForCurrentUser` | Now `async Task<JsonResult>` |
| `GetCurrentUserApplications` | Calls `GetApplicationSummariesByApplicantAsync` |
| `GetCurrentUserLgApprovalApplications` | `GetApplicationsByLginAsync` + `GetApplicationTypesByFilterAsync` |
| All 4 LG paged approval endpoints | `GetApplicationsByLginPagedAsync` + `GetApplicationTypesByFilterAsync` |
| `GetLicenseeData` (licences section) | `License.GetLicensesByLicenceeAsync` + `GetPaidLicensesOnTransferAsync` |
| `_GetPermanentChangesToLicenseeData` (licences) | Same DV helpers |
| `_GetPermanentChangesToLicenseeDataForLegalEntityReview` (licences) | Same DV helpers |
| `_GetLegalEntityReviewData` (licences) | Same DV helpers |
| `GetApplication` | `GetApplicationByIdWithChildrenAsync`; `ToViewModelAsync`; `GetSharePointDocLocsByObjectIdAsync` |
| `CurrentUserIsLgForApplicationAsync` (new DV overload) | Uses `GetAccountByIdAsync` + `adoxio_LGINLinkId` |
| `InitializeSharepointAsync` (new DV overload) | `CreateEntitySharePointDocumentLocationAsync` |
| `CreateApplication` | Full DV rewrite; `EntityReference` bindings; Marketing TiedHouseConnection post-create |
| `CreateCovidApplication` | Manual field assignment (no DV CopyValues for `CovidApplication`); `ToCovidViewModelAsync` |
| `UpdateApplication` | DV rewrite; inline null-clearing for LGIN/PolicJurisdiction; DV hours-of-service upsert |
| `SubmitLegalEntityApplication` | DV; `adoxio_application_statuscode.Incompleteinforeq` check |
| `ProcessApplication` | `ExecuteWorkflowAsync("0a78e6dc-…")` |
| `ProcessEndorsementApplication` | `ExecuteWorkflowAsync("e755b96c-…")` |
| `GetAutocomplete` | `GetApplicationsByJobNumberContainsAsync`; aliased link-entity values via `GetAttributeValue<AliasedValue>` |
| `UserHasInProgressLegalEntityReview` | `GetApplicationsByApplicantAndTypeAsync` + client-side status filter |
| `RemoveServiceAreasFromApplicationAsync` | `GetServiceAreasByApplicationIdAsync` + `DeleteServiceAreaAsync` |
| `AddServiceAreasToApplicationAsync` | `CreateServiceAreaAsync` with `adoxio_ApplicationId` EntityReference |
| `UpsertApplicationExtensionAsync(ApplicationExtension, string)` | DV create/update + `LinkApplicationExtensionToApplication` |
| `LinkApplicationExtensionToApplication` | Sets `adoxio_Application` EntityReference on extension via `UpdateApplicationExtensionAsync` |

### Fully Migrated in Ticket E (PCL/LE-review flow)

| Method | Notes |
|---|---|
| `GetCurrentLicenseeApplicationAsync` (was sync `GetCurrentLicenseeApplication`) | `GetApplicationTypeByNameAsync("Licensee Changes")` + `GetApplicationsByApplicantAndTypeAsync`; create via `CreateApplicationAsync` + `GetApplicationByIdWithChildrenAsync` |
| `GetOngoingLicenseeApplicationId` | Now `async Task<IActionResult>`; calls `GetCurrentLicenseeApplicationAsync` |
| `GetLicenseeData` (application block) | `GetCurrentLicenseeApplicationAsync` + `ToViewModelAsync`; app-type/contents block removed (handled internally by `ToViewModelAsync`) |
| `_GetExistingInProgressPermanentChangeApplication` | Returns `adoxio_application_dv`; uses `GetApplicationsByApplicantAndTypeAsync` with `requireStatecode0: true`; invoice-paid check on DV fields (`adoxio_Invoice`, `adoxio_PrimaryApplicationInvoicePaid`, `adoxio_SecondaryApplicationInvoice`, `adoxio_SecondaryApplicationInvoicePaid`) |
| `_createPermanentChangeApplication` | Returns `adoxio_application_dv`; `GetApplicationTypeByNameAsync` + `CreateApplicationAsync` + `GetApplicationByIdWithChildrenAsync` |
| `_GetPermanentChangesToLicenseeData` | DV throughout; `GetCannabisPaymentStatus(app_dv, _dataverse, _bcep)` / `GetLiquorPaymentStatus(app_dv, _dataverse, _bcep)`; `GetApplicationByIdWithChildrenAsync` + `ToViewModelAsync` |
| `_GetPermanentChangesToLicenseeDataForLegalEntityReview` | Same DV pattern as above |
| `_GetLegalEntityReviewData` | `GetApplicationByIdWithChildrenAsync` + `ToViewModelAsync` |
| `GetOrCreatePermanentChangeForLegalEntityReviewApplicationAsync` | Full DV rewrite; `GetApplicationByIdAsync`; LE Review type ID comparison; `GetApplicationExtensionByIdAsync` for PCL link check; `CreateApplicationAsync(CopyLEReviewApplicationToPCL(...))`; inline extension create/update with `adoxio_relatedleorpclapplication` |
| `CopyLEReviewApplicationToPCL` | Returns `adoxio_application_dv`; accepts `Guid pclApplicationTypeId`; copies all CS-prefixed bool fields |

### Removed Dead AutoRest Methods

| Method | Reason removed |
|---|---|
| `CurrentUserIsLgForApplication(MicrosoftDynamicsCRMadoxioApplication)` | No longer called; PCL/LE flow fully on DV |
| `UpsertApplicationExtensionAsync(MicrosoftDynamicsCRMadoxioApplicationextension, string)` | No longer called; PCL/LE extension upsert done inline in `GetOrCreatePermanentChangeForLegalEntityReviewApplicationAsync` |
| `IDynamicsClient _dynamicsClient` field + constructor param | No remaining usages in controller |

### Intentionally Kept on AutoRest

| Method | Reason |
|---|---|
| `GetLicenseeData` (legal entity tree, changelogs) | `GetLegalEntityTree`, `GetApplicationChangeLogs`, `GetNotTerminatedCRSApplicationCount` — no DV equivalents |

## Key Patterns

### EntityReference Bindings (Create)
```csharp
dvApp.adoxio_Applicant = new EntityReference("account", Guid.Parse(userSettings.AccountId));
dvApp.adoxio_ApplicationTypeId = new EntityReference("adoxio_applicationtype", applicationType.adoxio_applicationtypeId!.Value);
dvApp.adoxio_LicenceType = new EntityReference("adoxio_licencetype", licenceType.adoxio_licencetypeId!.Value);
```

### Clearing Lookups (Update patch)
```csharp
dvApp.adoxio_localgovindigenousnationid = null;  // clears the field via XRM SDK Update
dvApp.adoxio_PoliceJurisdictionId = null;
```

### Hours of Service Upsert
```csharp
var hoursEntity = await _dataverse.GetHoursOfServiceByApplicationIdAsync(id);
if (hoursEntity != null)
{
    patchHours.Id = hoursEntity.adoxio_hoursofserviceId!.Value;
    await _dataverse.UpdateHoursOfServiceAsync(patchHours);
}
else
{
    patchHours.adoxio_Application = new EntityReference("adoxio_application", applicationId);
    await _dataverse.CreateHoursOfServiceAsync(patchHours);
}
```

### Autocomplete Aliased Values
`GetApplicationsByJobNumberContainsAsync` uses a LeftOuter link-entity join with alias `"lic"`. Results are accessed via:
```csharp
var licNumber = app.GetAttributeValue<DV::Microsoft.Xrm.Sdk.AliasedValue>("lic.adoxio_licencenumber")?.Value as string;
```

### Marketing TiedHouseConnection
In DV you cannot inline `AdoxioApplicationAdoxioTiedhouseconnectionApplication` as a related-entity collection — create the connection AFTER the application exists:
```csharp
var conn = new adoxio_tiedhouseconnection_dv
{
    adoxio_ConnectionType = adoxio_tiedhouseconnection_adoxio_connectiontype.Marketer,
    adoxio_Application = new EntityReference("adoxio_application", createdId)
};
await _dataverse.CreateTiedHouseConnectionAsync(conn);
```
