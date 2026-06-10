# LicensesController Migration (LCSD-8558)

Migrates `cllc-public-app/Controllers/LicensesController.cs` from `IDynamicsClient` (AutoRest) to `IDataverseClient` (Dataverse SDK).

## New interface methods added

| Method | Description |
|---|---|
| `GetActiveApplicationsByAssignedLicenceIdAsync` | Active applications (`statecode=0`) where `adoxio_assignedlicence` equals the given licence GUID |
| `GetLicencesByThirdPartyOperatorAsync` | Licences by TPO account |
| `GetLicencesByProposedOwnerAsync` | Licences where account is proposed owner |
| `ClearLicenceProposedOwnerAsync` | Disassociate `adoxio_ProposedOwner` from a licence |
| `ClearLicenceThirdPartyOperatorAsync` | Disassociate `adoxio_ThirdPartyOperatorId` from a licence |
| `ClearAccountProposedOperatorAsync` | Disassociate `adoxio_account_adoxio_licences_ProposedOperator` from an account |
| `CreateLicenceSharePointDocLocAsync` | Create a SharePoint document location for a licence |
| `ExecuteWorkflowAsync` | Execute a Dynamics workflow by GUID against an entity record |

## Migration decisions

### Migrated to `_dataverse`

| Endpoint | What changed |
|---|---|
| `GET /{id}` | `GetLicenceByIdWithChildrenAsync` + `ToViewModelAsync`; SharePoint init via `CreateLicenceSharePointDocLocAsync` |
| `PUT /{licenceId}/representative` | Patch via `UpdateLicenceAsync`; reload + `ToLicenseSummaryViewModelAsync` |
| `PUT /{licenceId}/offsite-storage` | `GetOffSiteStorageByLicenceIdAsync`, `CreateOffSiteStorageAsync`, `UpdateOffSiteStorageAsync` |
| `POST /cancel-transfer` | `UpdateLicenceAsync` + `ClearLicenceProposedOwnerAsync` + `GetActiveApplicationsByAssignedLicenceIdAsync` |
| `POST /initiate-transfer` | `UpdateLicenceAsync` with `EntityReference` for `adoxio_ProposedOwner` |
| `POST /set-third-party-operator` | `UpdateLicenceAsync` with `EntityReference` for `adoxio_ThirdPartyOperatorId` |
| `POST /cancel-operator-application` | `UpdateLicenceAsync` + `ClearAccountProposedOperatorAsync` + `GetActiveApplicationsByAssignedLicenceIdAsync` |
| `POST /terminate-operator-relationship` | `ClearLicenceThirdPartyOperatorAsync` |
| `GET /{workflowGUID}/setexpiry/{licenceID}` | `ExecuteWorkflowAsync` |
| `GET /denyautorenew/{licenceID}` | `ExecuteWorkflowAsync` |
| `GET /current` | `GetLicencesByAccountIdAsync` + `GetApplicationsForLicenceByApplicantAsync` + `ToLicenseSummaryViewModelAsync` |
| `GET /third-party-operator` | `GetLicencesByThirdPartyOperatorAsync` + `ToLicenseSummaryViewModelAsync` |
| `GET /proposed-owner` | `GetLicencesByProposedOwnerAsync` + `ToLicenseSummaryViewModelAsync` |
| `GET /licencee/{licenceeId}` | `GetLicencesByAccountIdAsync` + `ToLicenseSummaryViewModelAsync` |
| `GET /{licenceId}/pdf/{filename}` | `GetLicenceByIdWithChildrenAsync` + `GetTermsConditionsByLicenceIdAsync` + `GetServiceAreasByLicenceIdAsync` + `GetHoursOfSaleByLicenceIdNoEndorsementAsync` + `ToViewModelAsync` |
| `PUT /{licenceId}/ldbordertotals` | Patch `adoxio_LDBOrderTotals` (decimal?) via `UpdateLicenceAsync` |
| `PUT /{licenceId}/establishment` | Patch establishment address fields via `UpdateLicenceAsync` |

### Kept on `_dynamicsClient`

| Endpoint / method | Reason |
|---|---|
| `GET /autocomplete` | No search/contains query in `IDataverseClient` |
| `POST /initiate-tied-house-excemption` + `CreateApplication` | Complex: `CopyValuesForChangeOfLocation`, ODataBind, LGIN + police jurisdiction lookups |
| `POST /{licenceId}/create-action-application` | Calls `CreateApplication` |
| `POST /{licenceId}/create-action-application-term/{termId}` | Calls `CreateApplication` + `GetTermChangeApplication` |
| `GetTermChangeApplication` (private) | Complex multi-status filter |
| `GET /outstanding-prior-balance-invoice` | `Invoice` model not in `IDataverseClient` |
| `isConclusivelyDeemed` (private) | Complex application status + type filter |
| `GetPaidLicensesOnTransfer` call in `GetCurrentUserLicences` | Complex paid-application filter not in `IDataverseClient` |
| `GetFolderName` in `GetLicencePDF` | TODO: pending migration |

## Property name notes (DV entity → field used)

- `adoxio_licences.adoxio_LDBOrderTotals` — `decimal?`
- `adoxio_licences.adoxio_EffectiveDate` / `adoxio_ExpiryDate` — `DateTime?`
- `adoxio_licences.adoxio_EstablishmentAddressStreet/City/PostalCode` — denormalized fields used in PDF (establishment EntityReference only carries Name)
- `adoxio_hoursofservice.adoxio_MondayOpen` etc. — `adoxio_servicehoursoptionsethours?` enum, cast to `(int?)` for `StoreHoursUtility.ConvertOpenHoursToString`
- `adoxio_licences_adoxio_transferrequested.No/Yes` and `adoxio_licences_adoxio_tporequested.No/Yes` — option set enums on the generated entity
- `adoxio_application_statuscode.Terminated` — used when cancelling transfer/TPO applications

## `GetActiveApplicationsByAssignedLicenceIdAsync` implementation

Queries `adoxio_application` where `adoxio_assignedlicence == licenceId AND statecode == 0`. Used in `CancelTransfer` and `CancelTPO` to find and terminate related in-flight applications.
