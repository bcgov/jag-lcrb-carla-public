# Geocoder Service Migration

Migration of `geocoder-service` from `IDynamicsClient` (AutoRest) to `IDataverseClient` (Dataverse SDK).

---

## What Was Implemented

### Files Modified

| File | Change |
|---|---|
| `geocoder-service/GeocodeUtils.cs` | Replaced `IDynamicsClient` field + manual construction with injected `IDataverseClient`; updated all field access to Dataverse SDK property names |
| `geocoder-service/Controllers/GeocoderController.cs` | Added `IDataverseClient` constructor parameter; passes it through to `GeocodeUtils` |
| `geocoder-service/Startup.cs` | `SetupHangfireJobs` resolves `IDataverseClient` from service scope and passes it to `GeocodeUtils` |
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` | Added 4 new methods (see below) |
| `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` | Implemented all 4 new methods |

### New IDataverseClient Methods

| Method | Description |
|---|---|
| `GetActiveLicencesByTypeIdsAsync(IList<string> licenceTypeIds)` | Fetches active (`statuscode = 1`) licences filtered by one or more licence type IDs using an OR condition |
| `GetEstablishmentsByNameAsync(string name)` | Fetches establishments by `adoxio_name` |
| `GetLicenceTypeByNameAsync(string name)` | Fetches a single licence type by `adoxio_name` |
| `GetLginByIdAsync(string id)` | Fetches a Local Government / Indigenous Nation record by ID |

---

## Design Decisions

### Constructor injection vs. internal setup
The original `GeocodeUtils` created an `IDynamicsClient` internally from `IConfiguration`. The new version receives `IDataverseClient` as a constructor parameter. Hangfire job lambdas in `Startup.cs` and `GeocoderController.cs` both capture the resolved singleton `IDataverseClient` from the DI container.

### Licence type name check removed
The original code expanded `adoxio_LicenceType` and re-checked the type name inside the loop. Since `GetActiveLicencesByTypeIdsAsync` already filters by the exact type GUIDs, the redundant name check was dropped.

### LGIN null guard added
`GetLginByIdAsync` returns `null` on a 404. The LGIN fallback geocode path now guards against a null return.

### Exception handling
`HttpOperationException` (AutoRest) catches were replaced with `Exception` catches. The Dataverse SDK throws `FaultException<OrganizationServiceFault>` for service errors; logging now records `ex.Message` instead of raw HTTP request/response content.

### Property name changes (AutoRest → Dataverse SDK)

| Old (`MicrosoftDynamicsCRMadoxioEstablishment`) | New (`adoxio_establishment`) |
|---|---|
| `AdoxioAddressstreet` | `adoxio_AddressStreet` |
| `AdoxioAddresscity` | `adoxio_AddressCity` |
| `AdoxioLatitude` (`decimal?`) | `adoxio_Latitude` (`double?`) |
| `AdoxioLongitude` (`decimal?`) | `adoxio_Longitude` (`double?`) |
| `_adoxioLginValue` (string ID) | `adoxio_LGIN` (EntityReference) |
| `AdoxioEstablishmentid` | `.Id` (inherited from Entity) |

| Old (`MicrosoftDynamicsCRMadoxioLicences`) | New (`adoxio_licences`) |
|---|---|
| `_adoxioEstablishmentValue` (string ID) | `adoxio_establishment` (EntityReference) |

---

## Gotcha: AutoRest ModelExtensions deletions

During model extension porting, 7 AutoRest model support files were deleted that are still referenced by `Extensions/EntityDefinitions.cs` and `Extensions/ApplicationExtension.cs`. These must be kept:

- `StatsResultModel.cs` — `StatsResultResponse`, `StatsResultModel`
- `MicrosoftDynamicsCRMpicklistAttributeMetadataCollection.cs`
- `MicrosoftDynamicsCRMpicklistAttributeMetadata.cs`
- `MicrosoftDynamicsCRMoptionSet.cs`
- `MicrosoftDynamicsCRMlocalizedLabel.cs`
- `MicrosoftDynamicsCRMlabel.cs`
- `MicrosoftDynamicsCRMoption.cs`

These are not ModelExtension helper methods — they are model classes for AutoRest-generated extension API calls and must remain in the `DynamicsAutorest` project.

---

## Manual Testing

### 1. Build

```powershell
dotnet build geocoder-service/geocoder-service.csproj
```

Expected: 0 errors.

### 2. Verify no remaining IDynamicsClient references

```powershell
Select-String -Path "geocoder-service/" -Pattern "IDynamicsClient|MicrosoftDynamicsCRM" -Recurse
```

Expected: no matches.

### 3. Run the service

```powershell
dotnet run --project geocoder-service/geocoder-service.csproj
```

### 4. Trigger geocoding via HTTP

Find an establishment in dev Dataverse with a null `adoxio_latitude`. Note its ID, then call:

```
GET /api/Geocoder/GeocodeEstablishment/{establishmentId}
```

Open the Hangfire dashboard at `/hangfire` and confirm the job succeeded. Then verify in Dataverse that `adoxio_latitude` and `adoxio_longitude` were written back.

### 5. Trigger bulk geocoding

```
GET /api/Geocoder/GeocodeEstablishments
```

Confirm via Hangfire that active Cannabis Retail Store and Section 119 establishments are processed and at least one establishment has coordinates updated.
