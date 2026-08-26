# Dataverse Integration — Setup & Authentication

---

## Project location

| Path | Purpose |
|---|---|
| `cllc-interfaces/Dynamics-Dataverse/` | New SDK-based interface project |
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/` | `IDataverseClient` interface |
| `cllc-interfaces/Dynamics-Dataverse/Generated/` | `pac modelbuilder` output — entity classes and option set enums |
| `cllc-interfaces/Dynamics-Dataverse/Extensions/` | `DataverseClient` extension methods (future) |
| `cllc-interfaces-test/Dataverse/` | Unit tests for the Dataverse interface project |

---

## Project Skeleton

### Files created

| File | Purpose |
|---|---|
| `cllc-interfaces/Dynamics-Dataverse/Dynamics-Dataverse.csproj` | .NET 6 class library |
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` | Interface — methods added incrementally as services are migrated |
| `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` | Authentication wrapper (see below) |
| `cllc-interfaces/Dynamics-Dataverse/Generated/.gitkeep` | Tracks empty folder in git |
| `cllc-interfaces/Dynamics-Dataverse/Extensions/.gitkeep` | Tracks empty folder in git |

### Solution placement

Added to `cllc-public-app/cllc-public-app.sln` under the `cllc-interfaces` solution folder:

```powershell
dotnet sln cllc-public-app/cllc-public-app.sln add cllc-interfaces/Dynamics-Dataverse/Dynamics-Dataverse.csproj
```

> This is a multi-solution repo — each service has its own `.sln`, there is no single root solution. When a service's `.csproj` gains a `ProjectReference` to `Dynamics-Dataverse`, that service's `.sln` picks it up transitively — no need to add it to every `.sln` manually.

### NuGet packages

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.PowerPlatform.Dataverse.Client` | 1.1.9 | Core SDK — `ServiceClient` |
| `Microsoft.Extensions.Configuration.Abstractions` | 6.0.0 | `IConfiguration` injection |
| `Microsoft.Extensions.Diagnostics.HealthChecks` | 6.0.0 | `IHealthCheck` for `DataverseClient` |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 6.0.0 | DI abstractions |
| `Microsoft.Extensions.Http` | 6.0.0 | HTTP client factory support |

### Testing steps

```powershell
dotnet build cllc-public-app/cllc-public-app.sln
```

Expected: `Build succeeded. 0 Warning(s) 0 Error(s)`. No existing service projects are changed.

---

## DataverseClient Authentication Wrapper

### Required environment variables

These variables are already in use by the existing `DynamicsSetupUtil.cs` — no new config keys are introduced.

> Store all values in your environment or secrets manager. Never hardcode credentials in source code or config files.

| Variable | Description |
|---|---|
| `DYNAMICS_ODATA_URI` | Dataverse org URL — `/api/data/vX.X/` suffix is stripped automatically by `DataverseClient` |
| `DYNAMICS_AAD_TENANT_ID` | AAD tenant ID for the app registration |
| `DYNAMICS_APP_REG_CLIENT_ID` | App registration client ID |
| `DYNAMICS_APP_REG_CLIENT_KEY` | App registration client secret |

### What was implemented

File: `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs`

- Reads the 4 env vars above via `IConfiguration`
- Throws `InvalidOperationException` (not a null reference) if any are missing
- Strips the `/api/data/vX.X/` suffix from `DYNAMICS_ODATA_URI` — `ServiceClient` needs the base org URL only
- Implements `IHealthCheck` — returns `Healthy` when `ServiceClient.IsReady` is `true`
- Uses native async `ServiceClient` methods — not `Task.Run` wrappers (see async pattern guidance below)

### Transitive NuGet versions (verified — no NU1605 conflicts)

| Package | Resolved version |
|---|---|
| `System.IdentityModel.Tokens.Jwt` | 8.6.1 |
| `Microsoft.Identity.Client` | 4.48.1 |
| `Microsoft.IdentityModel.Tokens` | 8.6.1 |

Verified with:

```powershell
dotnet restore cllc-public-app/cllc-public-app.csproj
dotnet list cllc-public-app/cllc-public-app.csproj package --include-transitive | Select-String "IdentityModel|Identity.Client"
```

### Testing steps

1. **Build:**

```powershell
dotnet build cllc-interfaces/Dynamics-Dataverse/Dynamics-Dataverse.csproj
```

Expected: `Build succeeded. 0 Warning(s) 0 Error(s)`

2. **Unit tests:**

```powershell
dotnet test cllc-interfaces-test/Dataverse/Dataverse.Tests.csproj --logger "console;verbosity=normal"
```

Expected: 5 passed, 0 failed. Tests cover:
- Config is read and parsed without null reference errors
- `InvalidOperationException` thrown for each of the 4 missing config keys individually

3. **NuGet conflict check:**

```powershell
dotnet restore cllc-public-app/cllc-public-app.csproj
dotnet list cllc-public-app/cllc-public-app.csproj package --include-transitive | Select-String "IdentityModel|Identity.Client"
```

Expected: zero `NU1605` downgrade warnings.

4. **Manual smoke test** (requires dev Dataverse credentials): set the 4 env vars in your environment, run the consuming app, confirm no `InvalidOperationException` on startup and the health check endpoint returns `Healthy`.

---

## Dependency injection registration

`IDataverseClient` / `DataverseClient` are registered as singletons alongside the existing `IDynamicsClient` in all 9 consuming services. This is pure plumbing — no functional changes.

### Services updated

| Service | File |
|---|---|
| `cllc-public-app` | `Startup.cs` |
| `carla-spice-sync-service` | `Startup.cs` |
| `federal-reporting-service` | `Startup.cs` |
| `ldb-orders-service` | `Startup.cs` |
| `geocoder-service` | `Startup.cs` |
| `one-stop-service` | `Startup.cs` |
| `orgbook-service` | `Startup.cs` |
| `watchdog` | `Startup.cs` |
| `sharepoint-sync-tool` | `Program.cs` |

### Registration pattern

```csharp
services.AddSingleton<IDataverseClient, DataverseClient>();
```

### Namespace collision resolution

Both `DynamicsAutorest` and `Dynamics-Dataverse` export types in `Gov.Lclb.Cllb.Interfaces`. To isolate them, `Dynamics-Dataverse` is referenced with an MSBuild alias in each consuming `.csproj`:

```xml
<ProjectReference Include="..\cllc-interfaces\Dynamics-Dataverse\Dynamics-Dataverse.csproj">
  <Aliases>DV</Aliases>
</ProjectReference>
```

Each `Startup.cs` / `Program.cs` then declares the alias and maps the two types:

```csharp
extern alias DV;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DataverseClient = DV::Gov.Lclb.Cllb.Interfaces.DataverseClient;
```

This keeps DynamicsAutorest types available in the global namespace unchanged.

### Additional fixes applied

| Project | Fix |
|---|---|
| `cllc-public-app` | Bumped `Microsoft.Extensions.Caching.Memory` from `3.1.7` → `3.1.8` (NU1605) |
| `watchdog` | Upgraded `TargetFramework` from `netcoreapp5` → `net6.0`; bumped HealthChecks.UI and MVC packages to `6.x` |

### Verification

```powershell
@(
  "cllc-public-app/cllc-public-app.csproj",
  "carla-spice-sync-service/CarlaSpiceSync.csproj",
  "federal-reporting-service/federal-reporting-service.csproj",
  "ldb-orders-service/ldb-orders-service.csproj",
  "geocoder-service/geocoder-service.csproj",
  "one-stop-service/one-stop-service.csproj",
  "orgbook-service/orgbook-service.csproj",
  "watchdog/Watchdog.csproj",
  "sharepoint-sync-tool/sharepoint-sync-tool.csproj"
) | ForEach-Object {
  $result = dotnet build $_ --no-incremental 2>&1
  $status = if ($result -match "Build succeeded") { "OK" } else { "FAILED" }
  "$status  $_"
  if ($status -eq "FAILED") { $result | Select-String "error" }
}
```

Expected: all 9 lines show `OK`.

---

## Async pattern guidance

`ServiceClient` exposes native async methods. Always prefer these over `Task.Run` wrappers:

```csharp
// Avoid
var entity = await Task.Run(() => _serviceClient.Retrieve(name, id, cols), ct);

// Prefer
var entity = await _serviceClient.RetrieveAsync(name, id, cols, ct);
```

Available native async methods: `CreateAsync`, `UpdateAsync`, `RetrieveAsync`, `RetrieveMultipleAsync`, `DeleteAsync`, `ExecuteAsync`.
