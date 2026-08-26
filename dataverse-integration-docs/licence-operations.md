# Licence Operations (LCSD-8537)

Implements `adoxio_licences` CRUD and child-loading methods on `DataverseClient`, plus ServiceArea, HourOfSale, OffSiteStorage, and ApplicationTermsConditionsLimitation CRUD.

## Licence Methods

| Method | Description |
|---|---|
| `GetLicenceByIdAsync` | Retrieve by GUID; returns `null` on not-found (no exception) |
| `GetLicenceByIdWithChildrenAsync` | Retrieve + parallel-load 4 child collections |
| `GetLicenceByNumberAsync` | Query by `adoxio_licencenumber` string |
| `GetLicencesByAccountIdAsync` | Query by `adoxio_licencee` account GUID |
| `UpdateLicenceAsync` | Update existing record |

## Child Entity Methods

### Service Area (`adoxio_servicearea`)

| Method | Description |
|---|---|
| `GetServiceAreasByLicenceIdAsync` | Query by `adoxio_licenceid` |
| `CreateServiceAreaAsync` | Create; returns new GUID |
| `UpdateServiceAreaAsync` | Update existing record |
| `DeleteServiceAreaAsync` | Delete by GUID; no-op on invalid GUID |

### Hour of Sale (`adoxio_hoursofservice`)

| Method | Description |
|---|---|
| `GetHoursOfSaleByLicenceIdAsync` | Query by `adoxio_licenceid` |
| `CreateHourOfSaleAsync` | Create; returns new GUID |
| `UpdateHourOfSaleAsync` | Update existing record |
| `DeleteHourOfSaleAsync` | Delete by GUID; no-op on invalid GUID |

### Off-Site Storage (`adoxio_offsitestorage`)

| Method | Description |
|---|---|
| `GetOffSiteStorageByLicenceIdAsync` | Query by `adoxio_licenceid` |
| `CreateOffSiteStorageAsync` | Create; returns new GUID |
| `DeleteOffSiteStorageAsync` | Delete by GUID; no-op on invalid GUID |

### Application Terms Conditions Limitation (`adoxio_applicationtermsconditionslimitation`)

| Method | Description |
|---|---|
| `GetTermsConditionsByLicenceIdAsync` | Query by `adoxio_licenceid` |
| `CreateTermsConditionsAsync` | Create; returns new GUID |
| `UpdateTermsConditionsAsync` | Update existing record |

## WithChildren loading strategy

`GetLicenceByIdWithChildrenAsync` fires four parallel queries after the initial `Retrieve`:

1. **ServiceArea** — `adoxio_servicearea` where `adoxio_licenceid == licenceId`; attached via `adoxio_licences_adoxio_servicearea`.
2. **HoursOfSale** — `adoxio_hoursofservice` where `adoxio_licenceid == licenceId`; attached via `adoxio_licences_adoxio_hoursofservice`.
3. **OffSiteStorage** — `adoxio_offsitestorage` where `adoxio_licenceid == licenceId`; attached via `adoxio_licences_adoxio_offsitestorage`.
4. **TermsConditions** — `adoxio_applicationtermsconditionslimitation` where `adoxio_licenceid == licenceId`; attached via `adoxio_licences_adoxio_applicationtermsconditionslimitation`.

Only non-empty collections are attached to `RelatedEntities`.
