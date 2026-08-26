# LCSD-8554: AccountsController Migration

Migrates `cllc-public-app/Controllers/AccountsController.cs` from `IDynamicsClient` (AutoRest) to `IDataverseClient` (Dataverse SDK). Zero `IDynamicsClient` references remain in the controller after this ticket.

## Files Changed

| File | Change |
|---|---|
| `cllc-public-app/Controllers/AccountsController.cs` | Full rewrite — all endpoints ported to `IDataverseClient` |
| `cllc-public-app/Models.Extensions/Account.cs` | Added `extern alias DV`, `ToViewModel(DvAccount)`, `CopyValues(DvAccount, ViewModel, bool)` SDK overloads |
| `cllc-public-app/Models.Extensions/Adoxio_LegalEntity.cs` | Added `extern alias DV`, `ToViewModel(DvLegalEntity)` SDK overload |
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` | Added 4 new methods |
| `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` | Implemented 4 new methods |

## New IDataverseClient Methods

```csharp
// Legal entity children
Task<IList<adoxio_legalentity>> GetLegalEntitiesByParentEntityIdAsync(string parentLegalEntityId, CancellationToken ct = default);

// Licence cleanup
Task DeleteLicenceAsync(string id, CancellationToken ct = default);

// Licensee changelog IDs (queries 3 account fields)
Task<IList<string>> GetLicenseeChangelogIdsByAccountIdAsync(string accountId, CancellationToken ct = default);

// SharePoint doc locations by object ID
Task<IList<SharePointDocumentLocation>> GetSharePointDocLocsByObjectIdAsync(string objectId, CancellationToken ct = default);
```

## Key Migration Patterns

### extern alias DV in ModelExtensions

Both `Account.cs` and `Adoxio_LegalEntity.cs` model extensions reference the Dataverse SDK entity types, which live in the `DV` alias:

```csharp
extern alias DV;
using DvAccount = DV::Gov.Lclb.Cllb.Interfaces.Account;
using DvLegalEntity = DV::Gov.Lclb.Cllb.Interfaces.adoxio_legalentity;
```

### SDK Entity Property Names (PascalCase)

SDK-generated entities use PascalCase for custom fields, unlike AutoRest which uses lowercase. Gotchas:

| AutoRest property | SDK property |
|---|---|
| `adoxio_externalid` | `adoxio_ExternalID` |
| `adoxio_bcincorporationnumber` | `adoxio_BCIncorporationNumber` |
| `adoxio_dateofincorporationinbc` | `adoxio_DateofIncorporationinBC` |
| `adoxio_pstnumber` | `adoxio_PSTNumber` |
| `adoxio_lginkid` | `adoxio_LGINLinkId` (EntityReference) |
| `adoxio_accounturls` | `adoxio_AccountURLs` |

### Sequential Creates Replacing Deep-Insert

AutoRest OData allowed a single call to create an account with an embedded legal entity. The SDK requires sequential creates:

```csharp
// 1. Create account, get GUID back
var accountId = await _dataverse.CreateAccountAsync(account);

// 2. Create legal entity with EntityReference to account
var legalEntity = new adoxio_legalentity
{
    adoxio_account = new EntityReference("account", Guid.Parse(accountId)),
    // ...
};
await _dataverse.CreateLegalEntityAsync(legalEntity);
```

### Navigation Properties → Separate Queries

AutoRest OData used `$expand` to load related entities in one call. The SDK requires explicit separate calls:

```csharp
// Old: account loaded with primarycontactid expanded
// New: fetch account, then fetch contact separately
var account = await _dataverse.GetAccountByIdAsync(accountId);
if (account.PrimaryContactId != null)
{
    var contact = await _dataverse.GetContactByIdAsync(account.PrimaryContactId.Id.ToString());
}
```

### GetActiveAccountBySiteminderBusinessGuid Replacement

The old AutoRest helper `GetActiveAccountBySiteminderBusinessGuid` is replaced by `GetAccountByExternalIdAsync`. The external ID must be normalized to uppercase without dashes (BCeID GUID format):

```csharp
var externalId = siteminderGuid.ToUpper().Replace("-", "");
var account = await _dataverse.GetAccountByExternalIdAsync(externalId);
```

### Licensee Changelog Has No Generated Entity

`adoxio_licenseechangelog` was not included in the Dataverse generation run. Delete operations use the logical name string:

```csharp
var ids = await _dataverse.GetLicenseeChangelogIdsByAccountIdAsync(accountId);
foreach (var id in ids)
    await _dataverse.DeleteByLogicalNameAsync("adoxio_licenseechangelog", id);
```

`GetLicenseeChangelogIdsByAccountIdAsync` queries 3 separate lookup fields (`adoxio_parentbusinessaccount`, `adoxio_businessaccount`, `adoxio_shareholderbusinessaccount`) and deduplicates results.

### adoxio_licences statecode (lowercase)

`adoxio_licences` uses lowercase `statecode` (type `adoxio_licences_statecode`) unlike `Contact` which uses PascalCase `StatusCode`:

```csharp
// Licences: lowercase
.Where(l => l.statecode == adoxio_licences_statecode.Active)

// Contact: PascalCase
contact.StatusCode = new OptionSetValue(1);
```

### DateTime? to DateTimeOffset? Conversion

`adoxio_licences.adoxio_ExpiryDate` is `DateTime?` but `AccountSummaryLicence.expiryDate` is `DateTimeOffset?`:

```csharp
expiryDate = item.adoxio_ExpiryDate.HasValue
    ? (DateTimeOffset?)item.adoxio_ExpiryDate.Value
    : null
```

### LicenceType Lookup Cache Pattern

`AccountSummary` fetches each unique licence type once and caches it locally to avoid N+1 calls:

```csharp
var licenceTypeCache = new Dictionary<string, ViewModels.AdoxioLicenceType>();
foreach (var typeId in licences.Where(l => l.adoxio_LicenceType != null)
                               .Select(l => l.adoxio_LicenceType.Id.ToString())
                               .Distinct())
{
    var lt = await _dataverse.GetLicenceTypeByIdAsync(typeId);
    if (lt != null) licenceTypeCache[typeId] = lt.ToViewModel();
}
```

### DynamicsExtensions.CurrentUserHasAccessToAccountAsync

The async overload taking `IDataverseClient` is available at `DynamicsExtensions.cs:1896`:

```csharp
bool hasAccess = await DynamicsExtensions.CurrentUserHasAccessToAccountAsync(
    accountId, HttpContext, _dataverse);
```

### Contact.FullName Is Read-Only

`FullName` on the SDK Contact entity is a computed field with no setter. Only set writable name fields:

```csharp
userContact.NickName = userSettings.UserDisplayName;
// Do NOT set: userContact.FullName = ...
```
