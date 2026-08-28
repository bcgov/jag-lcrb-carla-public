# LCSD-8552: EstablishmentsController Migration

Replaces all `IDynamicsClient` (AutoRest) usage in `cllc-public-app/Controllers/EstablishmentsController.cs`
with `IDataverseClient` (Dataverse SDK).

## Files Changed

| File | Change |
|---|---|
| `cllc-public-app/Controllers/EstablishmentsController.cs` | Full rewrite — `IDataverseClient` constructor, all methods async |
| `cllc-public-app/Models.Extensions/Adoxio_Establishment.cs` | Added DV `ToViewModel()` and `CopyValues()` for `adoxio_establishment` |
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` | Added 4 new methods |
| `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` | Implemented 4 new methods |

## New IDataverseClient Methods

```csharp
// Establishment
Task<Guid> CreateEstablishmentAsync(adoxio_establishment establishment, CancellationToken ct = default);
Task DeleteEstablishmentAsync(string id, CancellationToken ct = default);

// Application Type
Task<adoxio_applicationtype?> GetApplicationTypeByNameAsync(string name, CancellationToken ct = default);

// Proposed LRS
Task<IList<adoxio_application>> GetProposedLrsApplicationsAsync(string applicationTypeId, IList<int> excludeStatuses, CancellationToken ct = default);
```

## DV Extension Methods Added (Models.Extensions/Adoxio_Establishment.cs)

```csharp
// CopyValues — copies Email, Phone, IsOpen only (same scope as AutoRest version)
public static void CopyValues(this adoxio_establishment to, ViewModels.Establishment from)

// ToViewModel — maps all entity-specific + standard system fields
public static ViewModels.Establishment ToViewModel(this adoxio_establishment e)
```

## Key Migration Patterns

### Licence type ID lookup
```csharp
// Before
_dynamicsClient.GetAdoxioLicencetypeByName(name)?.AdoxioLicencetypeid

// After
(await _dataverse.GetLicenceTypeByNameAsync(name))?.Id.ToString()
```

### Map data — licences + establishment N+1
The map/LRS endpoints previously used OData `$expand=adoxio_establishment` inline with the licence query.
The DV SDK `GetActiveLicencesByTypeIdsAsync` returns licences with the `adoxio_establishment` EntityReference,
but not the establishment's fields (name, phone, isopen, lat/lon are not denormalized on the licence).
Establishments are now fetched individually per licence. This is acceptable because all map data
is cached for 1–2 days.

```csharp
licences = await _dataverse.GetActiveLicencesByTypeIdsAsync(typeIds);
foreach (var licence in licences)
{
    if (licence.adoxio_establishment == null) continue;
    var establishment = await _dataverse.GetEstablishmentByIdAsync(licence.adoxio_establishment.Id.ToString());
    // ...
}
```

### LDB stores
```csharp
// Before
var account = _dynamicsClient.GetAccountByNameWithEstablishments(LDB_ACCOUNT_NAME);
account.AdoxioAccountAdoxioEstablishmentLicencee  // expanded inline

// After
var account = await _dataverse.GetAccountByNameAsync(LDB_ACCOUNT_NAME);
var establishments = await _dataverse.GetEstablishmentsByAccountIdAsync(account.Id.ToString());
```

### LDB store status check
AutoRest used `establishment.Statuscode.Value == 845280000`. In DV, the active/inactive state lives
on `statecode` (typed as `adoxio_establishment_statecode?`), not `statuscode`. The `statuscode` enum
has no `Active` member — its values are `AIP`, `Licensed`, and `Inactive`.
```csharp
// statecode — adoxio_establishment_statecode: Active = 0, Inactive = 1
establishment.statecode == adoxio_establishment_statecode.Active
```

## Property Mapping (AutoRest → DV SDK)

| AutoRest (`MicrosoftDynamicsCRMadoxioEstablishment`) | DV SDK (`adoxio_establishment`) |
|---|---|
| `AdoxioEstablishmentid` | `Id.ToString()` |
| `AdoxioName` | `adoxio_name` |
| `AdoxioPhone` | `adoxio_Phone` |
| `AdoxioEmail` | `adoxio_Email` |
| `AdoxioIsopen` | `adoxio_IsOpen` (bool?) |
| `AdoxioAddresscity` | `adoxio_AddressCity` |
| `AdoxioAddresspostalcode` | `adoxio_AddressPostalCode` |
| `AdoxioAddressstreet` | `adoxio_AddressStreet` |
| `AdoxioLatitude` | `adoxio_Latitude` (double?) |
| `AdoxioLongitude` | `adoxio_Longitude` (double?) |
| `Statuscode` (int?) | `statuscode` (adoxio_establishment_statuscode?) · active state: `statecode` (adoxio_establishment_statecode?) |
| `_adoxioLicenceeValue` (string) | `adoxio_Licencee?.Id` |
| `AdoxioFridayclose` (int?) | `adoxio_FridayClose` (adoxio_servicehoursoptionsethours?) → cast to `(int?)` |
