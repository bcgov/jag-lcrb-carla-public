# Dataverse Integration — Entity Generation

---

## Overview

AutoRest required 64GB RAM and a deprecated Node.js toolchain to regenerate entity classes. This was replaced permanently with `pac modelbuilder` — the official Microsoft Power Platform CLI tool. It reads entity metadata directly from the live Dataverse instance and generates strongly-typed C# entity classes.

Going forward, any developer can add a new entity in minutes with a single command.

---

## Output location

```
cllc-interfaces/Dynamics-Dataverse/Generated/
  Entities/       <- one .cs file per entity
  OptionSets/     <- global option set enums
```

Generated files are **committed to the repo** so CI/CD does not need Dataverse access on every build.

---

## Prerequisites

### Install the Power Platform CLI

```powershell
winget install Microsoft.PowerPlatformCLI
```

Verify:

```powershell
pac --version
```

### Authenticate to the Dataverse instance

Use environment variables for all credentials — never hardcode values.

```powershell
pac auth create `
  --url $env:DYNAMICS_ODATA_URI `
  --applicationId $env:DYNAMICS_APP_REG_CLIENT_ID `
  --clientSecret $env:DYNAMICS_APP_REG_CLIENT_KEY `
  --tenant $env:DYNAMICS_AAD_TENANT_ID
```

Verify connection:

```powershell
pac org who
```

---

## Regenerating entities

Use the `generate-entities.ps1` script at the repo root. It runs `pac modelbuilder build` with all entities and writes output to `Generated/`.

```powershell
.\generate-entities.ps1
```

**When to re-run:**
- A new Dataverse entity needs to be added
- Existing entity fields changed in Dataverse

**To add a new entity:**
1. Add the entity logical name (lowercase) to the `$entities` array in `generate-entities.ps1`
2. Run `.\generate-entities.ps1`
3. Commit the new/updated files in `Generated/`

### Generation flags

The script runs with these flags to ensure option set enums are emitted:

```powershell
pac modelbuilder build `
  --outputDirectory "cllc-interfaces/Dynamics-Dataverse/Generated" `
  --namespace "Gov.Lclb.Cllb.Interfaces" `
  --entities ($entities -join ",") `
  --emitEntityETC false `
  --emitFieldClasses true `
  --generateActions false
```

> `--emitFieldClasses true` ensures `OptionSets/` and `EntityOptionSetEnum.cs` are generated. Entity enum casts (e.g. `(AdoxioApplicationStatusCodes)application.Statuscode?.Value`) will not compile without them.

---

## Entity list (47 total — 45 generated)

All entities referenced across the codebase were identified via audit and added upfront to avoid mid-migration re-runs.

| Category | Entities |
|---|---|
| Standard CRM | `account`, `contact`, `invoice`, `lead`, `list` |
| Application | `adoxio_application`, `adoxio_applicationextension`, `adoxio_applicationtype`, `adoxio_applicationtypecontent`, `adoxio_applicationtermsconditionslimitation`, `adoxio_termsconditionslimitationspreset` |
| Licence | `adoxio_licences`, `adoxio_licencetype`, `adoxio_licencesubcategory`, `adoxio_licenseechangelog`, `adoxio_endorsement` |
| Worker & screening | `adoxio_worker`, `adoxio_personalhistorysummary`, `adoxio_previousaddress`, `adoxio_alias`, `adoxio_login` |
| Establishment / corporate | `adoxio_establishment`, `adoxio_legalentity`, `adoxio_tiedhouseconnection`, `adoxio_tiedhouseassociation` |
| Special events | `adoxio_specialevent`, `adoxio_specialeventlocation`, `adoxio_specialeventlicencedarea`, `adoxio_specialeventschedule`, `adoxio_specialeventtandc`, `adoxio_event`, `adoxio_eventlocation`, `adoxio_eventschedule`, `adoxio_sepcity`, `adoxio_sepdrinktype`, `adoxio_sepdrinksalesforecast` |
| Licence operations | `adoxio_leconnection`, `adoxio_annualvolume`, `adoxio_servicearea`, `adoxio_hoursofservice`, `adoxio_offsitestorage` |
| Reporting & sync | `adoxio_cannabismonthlyreport`, `adoxio_cannabisinventoryreport`, `adoxio_ldborder`, `adoxio_federalreportexport` |
| Reference / policy | `adoxio_policydocument`, `adoxio_policejurisdiction`, `adoxio_localgovindigenousnation` |
| SharePoint / attachments | `sharepointdocumentlocation`, `annotation` |

### Missing entities (not found in dev Dataverse instance)

These entities were silently skipped by `pac modelbuilder` in previous runs.

| Entity | Status |
|---|---|
| `adoxio_hoursofsale` | Still unresolved — verify logical name in staging/prod |
| `adoxio_licensechangelog` | **Was a typo** — correct name is `adoxio_licenseechangelog` (double 'e'). Fixed in `generate-entities.ps1`. Re-run the script to generate. |
| `adoxio_specialeventlocation` | Added to script — re-run to generate |
| `adoxio_specialeventlicencedarea` | Added to script — re-run to generate |
| `adoxio_specialeventschedule` | Added to script — re-run to generate |
| `adoxio_specialeventtandc` | Added to script — re-run to generate |

If any are not found after re-running, verify the logical name in staging/prod and update `generate-entities.ps1`.

> **`annotation` is critical** — the codebase has 16+ annotation-related files covering file attachments and notes on records (`documentbody`, `notetext`, `objectid`). Missing it would break every supporting-document upload flow.

---

## Testing steps

### 1. Verify generated files compile

```powershell
dotnet build cllc-interfaces/Dynamics-Dataverse/Dynamics-Dataverse.csproj
```

Expected: `Build succeeded. 0 Warning(s) 0 Error(s)`

### 2. Verify entity files are present

```powershell
Get-ChildItem cllc-interfaces/Dynamics-Dataverse/Generated/Entities/ | Select-Object Name | Sort-Object Name
```

At minimum these must be present: `account.cs`, `adoxio_application.cs`, `adoxio_licences.cs`, `adoxio_worker.cs`, `contact.cs`, `adoxio_personalhistorysummary.cs`, `adoxio_previousaddress.cs`, `adoxio_leconnection.cs`, `sharepointdocumentlocation.cs`, `annotation.cs`

### 3. Verify option set enums were generated

```powershell
Test-Path cllc-interfaces/Dynamics-Dataverse/Generated/OptionSets/
Get-ChildItem cllc-interfaces/Dynamics-Dataverse/Generated/OptionSets/ | Measure-Object | Select-Object Count
```

Expected: folder exists with multiple `.cs` files.

### 4. Spot-check fields against AutoRest models

Cross-reference 3–4 field names between the generated entity and the corresponding `MicrosoftDynamicsCRM*` file in `cllc-interfaces/Dynamics-Autorest/Models/` to confirm field names align.
