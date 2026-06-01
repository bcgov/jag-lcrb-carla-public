# Application Operations (LCSD-8536)

Implements `adoxio_application` CRUD and child-loading methods on `DataverseClient`.

## Methods

| Method | Description |
|---|---|
| `GetApplicationByIdAsync` | Retrieve by GUID; returns `null` on not-found (no exception) |
| `GetApplicationByIdWithChildrenAsync` | Retrieve + parallel-load Licence, Establishment, LegalEntities |
| `GetApplicationsByAccountIdAsync` | Query by `adoxio_applicant` account GUID |
| `CreateApplicationAsync` | Create and return new GUID |
| `UpdateApplicationAsync` | Update existing record |
| `DeleteApplicationAsync` | Delete by GUID; no-op on invalid GUID |
| `CreateApplicationExtensionAsync` | Create `adoxio_applicationextension` record |
| `UpdateApplicationExtensionAsync` | Update `adoxio_applicationextension` record |
| `CreateAnnualVolumeAsync` | Create `adoxio_annualvolume` record |

## WithChildren loading strategy

`GetApplicationByIdWithChildrenAsync` fires three parallel tasks after the initial retrieve:

1. **Licence** — if `adoxio_AssignedLicence` EntityReference is set, retrieves the full `adoxio_licences` record and attaches it via relationship `adoxio_adoxio_licences_adoxio_application_AssignedLicence`.
2. **Establishment** — if `adoxio_LicenceEstablishment` EntityReference is set, retrieves the full `adoxio_establishment` record and attaches it via `adoxio_adoxio_establishment_adoxio_application_Establishment`.
3. **LegalEntities** — queries `adoxio_legalentity` where `adoxio_relatedapplication == applicationId` and attaches via `adoxio_adoxio_application_adoxio_legalentity_RelatedApplication`.

Callers access the pre-loaded entities through the generated relationship navigation properties on `adoxio_application`.
