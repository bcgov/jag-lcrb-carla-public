# LCSD-8543: Migrate ldb-orders-service to IDataverseClient

Migrates `ldb-orders-service/LdbOrdersUtils.cs` — the smallest service in the migration epic — from the AutoRest `IDynamicsClient` to the Dataverse SDK `IDataverseClient`. Also adds `CreateLdbOrderAsync` to `IDataverseClient` / `DataverseClient`.

## New interface method

| Method | Entity | Notes |
|---|---|---|
| `CreateLdbOrderAsync(adoxio_ldborder)` | `adoxio_ldborder` | Write-only; no read/delete needed |

## LdbOrdersUtils changes

### Constructor

`IDataverseClient` is now injected instead of `IDynamicsClient` being instantiated locally from config:

```csharp
// Before — local instantiation inside CheckForLdbSales
IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(Configuration);

// After — constructor injection
public LdbOrdersUtils(IConfiguration configuration, IDataverseClient dataverse)
```

### CheckForLdbSales

| Old (AutoRest) | New (Dataverse SDK) |
|---|---|
| `dynamicsClient.GetLicenceByNumber(row.Licence.ToString())` | `await _dataverse.GetLicenceByNumberAsync(row.Licence.ToString())` |
| `new MicrosoftDynamicsCRMadoxioLdborder { LicenceIdODataBind = dynamicsClient.GetEntityURI(...) }` | `new adoxio_ldborder { adoxio_LicenceId = new EntityReference(adoxio_licences.EntityLogicalName, licence.Id) }` |
| `dynamicsClient.Ldborders.Create(ldbOrder)` | `await _dataverse.CreateLdbOrderAsync(ldbOrder)` |

The `if (dynamicsClient != null)` null-guard is removed — `IDataverseClient` is always present via DI.

## Removed usings

| Using | Reason |
|---|---|
| `Gov.Lclb.Cllb.Interfaces.Models` | AutoRest model namespace — no longer used |
| `Microsoft.Extensions.Caching.Memory` | Unused |
| `System.ServiceModel` | Unused |
| `System.Text` | Unused |

## Added usings / alias

```csharp
extern alias DV;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using adoxio_ldborder = DV::Gov.Lclb.Cllb.Interfaces.adoxio_ldborder;
using adoxio_licences = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licences;
using Microsoft.Xrm.Sdk;   // EntityReference
```

The `DV` alias is needed because both `DynamicsAutorest` and `Dynamics-Dataverse` expose the same `Gov.Lclb.Cllb.Interfaces` namespace.

## Startup.cs changes

| Change | Detail |
|---|---|
| Added `services.AddTransient<LdbOrdersUtils>()` | So Hangfire can resolve it from DI |
| Hangfire registration | Changed from `new LdbOrdersUtils(Configuration)` (no DI) to `RecurringJob.AddOrUpdate<LdbOrdersUtils>(utils => utils.CheckForLdbSales(null), Cron.Daily())` |
