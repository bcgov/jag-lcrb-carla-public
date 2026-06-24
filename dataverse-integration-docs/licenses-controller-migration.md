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
| `GetLicencesByNameOrNumberAsync` | `statecode=0` licences filtered by OR-contains on `adoxio_name` / `adoxio_licencenumber`; `TopCount` configurable |
| `GetApplicationsByTypeAndAssignedLicenceAsync` | Applications by `adoxio_applicationtypeid` + `adoxio_assignedlicence` + `statecode=0` + exclude statuses list |

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

### Phase 2 — Remaining AutoRest methods migrated (Ticket D)

| Method | What changed |
|---|---|
| `GET /autocomplete` | `GetLicencesByNameOrNumberAsync` (new); async; DV licence fields (`adoxio_name`, `adoxio_licencenumber`, `adoxio_EstablishmentAddressStreet/City/PostalCode`, EntityReference `.Name` for licencee/establishment) |
| `POST /initiate-tied-house-excemption` | `GetLicenceByIdAsync` × 2; `CreateApplicationAsync` |
| `POST /{licenceId}/create-action-application` | `CreateApplicationAsync` + `ToViewModelAsync` |
| `POST /{licenceId}/create-action-application-term/{termId}` | `GetTermChangeApplicationAsync` + `CreateApplicationAsync` + `AssociateTermsConditionsToApplicationAsync` + `ToViewModelAsync` |
| `CreateApplication` (private) → `CreateApplicationAsync` | `GetLicenceByIdAsync`, `GetApplicationTypeByNameAsync`, `GetAccountByIdAsync`, `GetEstablishmentByIdAsync`, `GetActiveApplicationsByAssignedLicenceIdAsync`; EntityReference bindings for type/licence/subcategory/establishment/applicant/LGIN/police |
| `GetTermChangeApplication` (private) → `GetTermChangeApplicationAsync` | `GetApplicationsByTypeAndAssignedLicenceAsync` (new); checks `adoxio_application` N:1 on TC via `GetTermsConditionsByIdAsync` |
| `GET /outstanding-prior-balance-invoice` | `GetApplicationsByApplicantTypeAndStatusesAsync` + `GetInvoiceByIdAsync` + `GetLicenceByIdAsync`; `DvInvoice.ToViewModel()` overload added to `Invoice.cs` |
| `isConclusivelyDeemed` (private) → `isConclusivelyDeemedAsync` | `GetApplicationsByApplicantAndTypeAsync` + client-side filter by `adoxio_AssignedLicence`; checks `adoxio_ChecklistConclusivelyDeem == Yes` enum |

### Kept on `_dynamicsClient`

Nothing. The `_dynamicsClient` field, constructor parameter, and `_dynamicsClient = dynamicsClient;` assignment have been removed. Zero IDynamicsClient references remain in LicensesController.

### Completed pinned TODOs

| Item | Resolution |
|---|---|
| `GetFolderName` in `GetLicencePDF` | Replaced with `await _dataverse.GetFolderNameAsync(entityName, entityId)` |
| `Endorsement.ToHtml(_dynamicsClient)` × 2 in `GetLicencePDF` | `ToHtml` renamed to `ToHtmlAsync(IDataverseClient)` using `GetHoursOfSaleByEndorsementIdAsync` + `GetServiceAreasByEndorsementIdAsync`; callers updated to `await ... ToHtmlAsync(_dataverse)` |

## Property name notes (DV entity → field used)

- `adoxio_licences.adoxio_LDBOrderTotals` — `decimal?`
- `adoxio_licences.adoxio_EffectiveDate` / `adoxio_ExpiryDate` — `DateTime?`
- `adoxio_licences.adoxio_EstablishmentAddressStreet/City/PostalCode` — denormalized fields used in PDF (establishment EntityReference only carries Name)
- `adoxio_hoursofservice.adoxio_MondayOpen` etc. — `adoxio_servicehoursoptionsethours?` enum, cast to `(int?)` for `StoreHoursUtility.ConvertOpenHoursToString`
- `adoxio_licences_adoxio_transferrequested.No/Yes` and `adoxio_licences_adoxio_tporequested.No/Yes` — option set enums on the generated entity
- `adoxio_application_statuscode.Terminated` — used when cancelling transfer/TPO applications

## `DvInvoice.ToViewModel()` overload

Added to `cllc-public-app/Models.Extensions/Invoice.cs` (requires `extern alias DV`). Maps:
- `InvoiceId` → `id` (string)
- `Name`, `InvoiceNumber` → `name`, `invoicenumber`
- `StateCode`, `StatusCode` → `statecode`, `statuscode` (cast to `int?`)
- `TotalTax.Value`, `TotalAmount.Value` → decimal fields
- `adoxio_TransactionID`, `adoxio_returnedtransactionid` → `transactionId`, `returnedTransactionId`
- `DueDate` → `duedate` (`DateTime.SpecifyKind(..., Local)`)

## `GetActiveApplicationsByAssignedLicenceIdAsync` implementation

Queries `adoxio_application` where `adoxio_assignedlicence == licenceId AND statecode == 0`. Used in `CancelTransfer` and `CancelTPO` to find and terminate related in-flight applications.
