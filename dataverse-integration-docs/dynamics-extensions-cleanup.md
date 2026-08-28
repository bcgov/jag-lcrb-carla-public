# Dead Code Cleanup: DynamicsExtensions + Model Extensions

**Files:** `cllc-public-app/Contexts/DynamicsExtensions.cs`, `cllc-public-app/Models.Extensions/Application.cs`, `cllc-public-app/Models.Extensions/License.cs`

## Summary

Removed ~700 lines of dead `IDynamicsClient` extension methods from `DynamicsExtensions`, plus dead methods from `ApplicationExtensions` and `LicenseExtensions`. Also migrated the remaining live AutoRest methods in `DynamicsExtensions` to DV async equivalents and updated their callers in `ApplicationsController` and `LicensesController`.

Phase 2 (Ticket A): Added four DV async replacement methods (`GetApplicationChangeLogsAsync`, `GetLegalEntityChildrenAsync`, `GetLegalEntityTreeAsync`, `GetNotTerminatedCRSApplicationCountAsync`), removed the AutoRest originals plus `GetLEConnectionsForAccount` (dead), `GetApplicationListByApplicant` (dead), and `GetApplicationsForLicenceByApplicant` (dead). Updated callers in `ApplicationsController.GetLicenseeData` and `LicensesController.GetCurrentUserLicences`.

Phase 2 (Ticket C): Removed `ToLicenseSummaryViewModel(IDynamicsClient)` from `LicenseExtensions` — its only caller was `GetPaidLicensesOnTransfer` which was removed in Ticket A.

## Methods Removed

### Contact / User / Auth helpers (Siteminder-orphaned)

| Method | Reason dead |
|---|---|
| `GetActiveAccountByLegalName` | Siteminder handler migrated to DV |
| `GetActiveAccountBySiteminderBusinessGuid` | Siteminder handler migrated to DV |
| `GetActiveContactByExternalId` | Siteminder handler migrated to DV |
| `GetActiveContactByExternalIdBridged` | Siteminder handler migrated to DV |
| `GetLoginTypePicklistValue` | Only called by removed `UpdateContactBridgeLogin` |
| `GetActiveContactsByDetails` | No callers |
| `GetActiveContactsByAccountId` | No callers |
| `GetContactByNameAndBirthdate` | Siteminder handler migrated to DV |
| `GetContactByContactVmBlankSmGuid` | Only called by removed `GetUserByContactVmBlankSmGuid` |
| `LoadUserLegacy` | Siteminder handler inlined as `LoadUserLegacyAsync` (DV) |
| `LoadUser` | No callers |
| `GetUserByGuid` | No callers |
| `GetActiveUserBySmGuid` | Only called by removed `LoadUser*` |
| `GetActiveUserBySmGuidBridged` | Only called by removed `LoadUser*` |
| `GetUserByContactVmBlankSmGuid` | Only called by removed `LoadUser*` |
| `GetUserAsViewModelContact` | No callers |
| `UpdateContactBridgeLogin` | No callers (bridge login flow removed) |

### Licence / Legal entity helpers (no callers)

| Method | Reason dead |
|---|---|
| `GetLicensesByLicencee` (public overload) | No callers |
| `GetLicensesByLicencee` (internal) | No callers |
| `GetTransferLicensesByLicencee` | No callers |
| `GetAllLicensesByLicencee` | No callers |
| `GetInvoiceByIdWithApplications` | No callers |
| `GetLegalEntityById` | No callers |
| `GetTiedHouseConnectionById` | No callers |
| `GetAliasById` | No callers |
| `GetThirdPartyOperaotsLicences` | No callers |
| `GetAdoxioLegalentityByAccountId` | No callers |
| `GetCachedLicenceTypeByName` | No callers |
| `GetCachedLicenceTypeIdByName` | No callers |
| `GetAdoxioSubLicencetypeByName` | No callers |

### Address helpers (no callers)

| Method | Reason dead |
|---|---|
| `GetPreviousAddressByContactId` | No callers |
| `GetPreviousAddressById` | No callers |

### Account / payment helpers (superseded by DV overloads)

| Method | Reason dead |
|---|---|
| `CurrentUserHasAccessToAccount(IDynamicsClient)` | Superseded by `CurrentUserHasAccessToAccountAsync(IDataverseClient)` |
| `IsChildAccount(IDynamicsClient)` (private) | Only called by removed `CurrentUserHasAccessToAccount(IDynamicsClient)` |
| `IsMostlyLiquor(this MicrosoftDynamicsCRMaccount, IDynamicsClient)` | Only called by removed `GetPaymentType(IDynamicsClient)` |
| `GetPaymentType(this MicrosoftDynamicsCRMadoxioApplication, IDynamicsClient)` | Superseded by `GetPaymentTypeAsync(IDataverseClient)` |

## Using Directives Removed

- `System.Runtime.CompilerServices` — never referenced in file
- `Serilog` — was only used in removed methods (`UpdateContactBridgeLogin`, `GetUserAsViewModelContact`)

## Intentionally Kept on AutoRest

These `IDynamicsClient` methods remain because they are still called by controllers or have no DV equivalent:

| Method | Called by |
|---|---|
| `GetInvoiceById` (both overloads) | `PaymentController` static overloads |
| `GetApplicationById` / `GetApplicationByIdWithChildren` | `PaymentController` static overloads |
| `GetInventoryReportsForMonthlyReport` | Federal reporting |
| `GetApplicationTypeByName` / `GetApplicationTypeById` | Active callers |
| `GetSystemformViewModel` (AutoRest sync) | Active callers |
| `IsMostlyLiquor(List<...>)` | Unit tests + `GetPaymentTypeAsync` |
| `CurrentUserIsContact` | `ContactController` |
| `GetPhsLink` / `GetCASLink` | Worker flows |

## DV Async Methods Added to DynamicsExtensions (Ticket A)

| New Method | Replaces |
|---|---|
| `GetApplicationChangeLogsAsync(IDataverseClient, string, ILogger)` | `GetApplicationChangeLogs(IDynamicsClient, ...)` |
| `GetLegalEntityChildrenAsync(IDataverseClient, string, IConfiguration, List<string>?)` | `GetLegalEntityChildren(IDynamicsClient, ...)` |
| `GetLegalEntityTreeAsync(IDataverseClient, string, IConfiguration)` | `GetLegalEntityTree(IDynamicsClient, ...)` |
| `GetNotTerminatedCRSApplicationCountAsync(IDataverseClient, string)` | `GetNotTerminatedCRSApplicationCount(IDynamicsClient, ...)` |

## Methods Removed in Ticket A

| Method | Reason |
|---|---|
| `GetApplicationChangeLogs(IDynamicsClient)` | Replaced by DV async version |
| `GetLegalEntityTree(IDynamicsClient)` | Replaced by DV async version |
| `GetLegalEntityChildren(IDynamicsClient)` | Replaced by DV async version |
| `GetNotTerminatedCRSApplicationCount(IDynamicsClient)` | Replaced by DV async version |
| `GetLEConnectionsForAccount(IDynamicsClient)` | Dead — `LEConnectionsController` already uses `GetLeConnectionContactsAsync` directly |
| `GetPaidLicensesOnTransfer(IDynamicsClient)` | Replaced by existing `LicenseExtensions.GetPaidLicenseSummariesOnTransferAsync` |
| `GetApplicationListByApplicant(IDynamicsClient)` | Only called by removed `GetNotTerminatedCRSApplicationCount` |
| `GetApplicationsForLicenceByApplicant(IDynamicsClient)` | Only called by removed `GetPaidLicensesOnTransfer` |

## Methods Removed in Ticket C (License.cs)

| Method | Reason |
|---|---|
| `ToLicenseSummaryViewModel(IDynamicsClient)` | Only caller was `GetPaidLicensesOnTransfer` — removed in Ticket A |

## Application.cs — Method Removed

| Method | Reason dead |
|---|---|
| `ToCovidViewModel(this MicrosoftDynamicsCRMadoxioApplication, IDynamicsClient, ...)` | `CreateCovidApplication` now calls `ToCovidViewModelAsync(IDataverseClient)` — AutoRest overload has zero callers |

**Using removed:** `System.Security.Cryptography` — imported but never referenced in the file.
