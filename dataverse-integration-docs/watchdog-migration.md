# Watchdog Service Migration

Migration of `watchdog` from `IDynamicsClient` (AutoRest) to `IDataverseClient` (Dataverse SDK).

---

## What Was Implemented

### Files Modified

| File | Change |
|---|---|
| `watchdog/Startup.cs` | Added `.AddCheck<DataverseClient>("dataverse")` to the health checks registration |
| `watchdog/Pages/ApplicationTypesCheck.cshtml.cs` | Replaced `IDynamicsClient` / `DynamicsSetupUtil` with `DataverseClient` directly instantiated per environment; updated `CreateConfig` to use AAD key names; switched dictionary type to `adoxio_applicationtype` |
| `watchdog/Pages/ApplicationTypesCheck.cshtml` | Updated all property references from AutoRest (`AdoxioXxx`) to Dataverse SDK (`adoxio_Xxx`) naming |
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` | Added `GetApplicationTypesAsync` and `GetSystemFormXmlByIdAsync` |
| `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` | Implemented both new methods |

### New IDataverseClient Methods

| Method | Description |
|---|---|
| `GetApplicationTypesAsync()` | Fetches all `adoxio_applicationtype` records with full column set |
| `GetSystemFormXmlByIdAsync(string id)` | Retrieves the `formxml` attribute from a `systemform` record by ID; returns `null` on 404 |

---

## Design Decisions

### Health check wiring
`DataverseClient` already implements `IHealthCheck` from LCSD-8529. Registering it with `.AddCheck<DataverseClient>("dataverse")` is sufficient — Dataverse connection readiness is checked via `_serviceClient.IsReady` and exposed at `/hc/ready`.

### ApplicationTypesCheck — multi-environment approach
The ApplicationTypesCheck page compares application type configurations across DEV, TST, and PRD Dynamics environments. The original code instantiated an `IDynamicsClient` per environment via `DynamicsSetupUtil`. The new code instantiates a `DataverseClient` directly (not via DI) using a custom `IConfigurationRoot` built from per-environment env vars. This preserves the multi-environment diagnostic behaviour.

### Configuration key changes
The old code used ADFS-era keys (`ADFS_OAUTH2_URI`, `DYNAMICS_APP_GROUP_CLIENT_ID`, `DYNAMICS_APP_GROUP_SECRET`, etc.). The new `DataverseClient` constructor requires AAD client secret keys:

| Old key (with `{PREFIX}_` prefix) | New key (with `{PREFIX}_` prefix) |
|---|---|
| `DYNAMICS_ODATA_URI` | `DYNAMICS_ODATA_URI` (unchanged) |
| `ADFS_OAUTH2_URI` | _(removed)_ |
| `DYNAMICS_APP_GROUP_CLIENT_ID` | `DYNAMICS_APP_REG_CLIENT_ID` |
| `DYNAMICS_APP_GROUP_SECRET` | `DYNAMICS_APP_REG_CLIENT_KEY` |
| `DYNAMICS_USERNAME` / `DYNAMICS_PASSWORD` | _(removed)_ |
| _(new)_ | `DYNAMICS_AAD_TENANT_ID` |

OpenShift deployment configs for watchdog must be updated to provide `DEV_DYNAMICS_AAD_TENANT_ID`, `DEV_DYNAMICS_APP_REG_CLIENT_ID`, `DEV_DYNAMICS_APP_REG_CLIENT_KEY` (and TST/PRD equivalents) for the ApplicationTypesCheck page to function.

### Property name changes (AutoRest → Dataverse SDK)
Optionset properties that were `int?` in AutoRest are now `adoxio_yesnoandreadonly?` (enum, `Yes = 1, No = 0, ReadOnly = 2`) in the Dataverse SDK. The Razor template previously used `.Equals(1).ToString()` (outputting `"True"`/`"False"`); it now uses `.ToString()` directly (outputting `"Yes"`/`"No"`/`"ReadOnly"`). The cross-environment comparison is still correct since all three columns use the same representation.

### `extern alias DV` in ApplicationTypesCheck.cshtml.cs
Both the AutoRest and Dataverse SDK assemblies share the `Gov.Lclb.Cllb.Interfaces` root namespace. `ApplicationTypesCheck.cshtml.cs` now adds `extern alias DV;` and using aliases (`DataverseClient`, `adoxio_applicationtype`) to access the Dataverse types without ambiguity. The AutoRest assembly reference is no longer used in this file but remains in the project for other watchdog pages.

---

## Manual Testing

### 1. Build

```powershell
dotnet build watchdog/Watchdog.csproj
```

Expected: 0 errors.

### 2. Verify no remaining IDynamicsClient references

```powershell
Select-String -Path "watchdog/" -Pattern "IDynamicsClient|MicrosoftDynamicsCRM" -Recurse
```

Expected: no matches.

### 3. Run and check health endpoint

```powershell
dotnet run --project watchdog/Watchdog.csproj
```

Hit `/hc/ready` and confirm the `dataverse` health check entry appears and reports `Healthy`.

### 4. ApplicationTypesCheck

Ensure the per-environment env vars (`DEV_DYNAMICS_ODATA_URI`, `DEV_DYNAMICS_AAD_TENANT_ID`, `DEV_DYNAMICS_APP_REG_CLIENT_ID`, `DEV_DYNAMICS_APP_REG_CLIENT_KEY`, and TST/PRD equivalents) are set, then navigate to `/ApplicationTypesCheck` and confirm the table renders application type comparison data.
