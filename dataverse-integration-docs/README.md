# Dataverse Integration

Reference for how this codebase talks to Dataverse — replaces the old AutoRest-generated
`DynamicsClient` with `Microsoft.PowerPlatform.Dataverse.Client.ServiceClient`.

---

## Project layout

| Path | Purpose |
|---|---|
| `cllc-interfaces/Dynamics-Dataverse/` | SDK-based interface project |
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` | The interface every service codes against |
| `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` | Auth wrapper + all entity CRUD methods |
| `cllc-interfaces/Dynamics-Dataverse/Generated/` | `pac modelbuilder` output — entity classes and option-set enums |
| `cllc-interfaces/Dynamics-Dataverse/Extensions/` | `DataverseClient` extension methods |
| `cllc-interfaces-test/Dataverse/` | Unit tests for the interface project |

This is a multi-solution repo — each service has its own `.sln`. When a service's `.csproj`
references `Dynamics-Dataverse`, its `.sln` picks it up transitively.

---

## Authentication

Dataverse auth is **app-registration client credentials** (Azure AD), not a username/password.
`DataverseClient`'s constructor builds a `ServiceClient` connection string from 4 required
environment variables:

| Variable | Description |
|---|---|
| `DYNAMICS_NATIVE_ODATA_URI` (preferred) or `DYNAMICS_ODATA_URI` (fallback) | Dataverse org URL. Only `scheme://host` is used — any `/api/data/vX.X/` suffix or path is stripped automatically (`ExtractOrgUrl`) |
| `DYNAMICS_AAD_TENANT_ID` | Azure AD tenant ID |
| `DYNAMICS_APP_REG_CLIENT_ID` | App registration client ID |
| `DYNAMICS_APP_REG_CLIENT_KEY` | App registration client **secret value** (not the secret ID — pasting the ID instead of the value produces `AADSTS7000215: Invalid client secret`) |

`DYNAMICS_USERNAME` / `DYNAMICS_PASSWORD` are legacy on-prem AD credentials and are **not**
used by this auth path — don't rely on them.

Missing config throws `InvalidOperationException` (not a null reference) at startup.
`DataverseClient` also implements `IHealthCheck`, returning `Healthy` when
`ServiceClient.IsReady`.

### Verifying credentials directly (bypassing the app)

```bash
ORG_URL=<scheme://host derived as above>
TOKEN=$(curl -s -X POST "https://login.microsoftonline.com/${TENANT_ID}/oauth2/v2.0/token" \
  -d "grant_type=client_credentials" -d "client_id=${CLIENT_ID}" \
  -d "client_secret=${CLIENT_SECRET}" -d "scope=${ORG_URL}/.default" \
  | jq -r '.access_token')

curl -s "${ORG_URL}/api/data/v9.2/accounts(<id>)?\$select=name" \
  -H "Authorization: Bearer $TOKEN" -H "Accept: application/json" \
  -H "OData-MaxVersion: 4.0" -H "OData-Version: 4.0"
```

---

## DI registration

Registered as a singleton in every consuming service's `Startup.cs` / `Program.cs`:

```csharp
services.AddSingleton<IDataverseClient, DataverseClient>();
```

Services wired: `cllc-public-app`, `carla-spice-sync-service`, `federal-reporting-service`,
`ldb-orders-service`, `geocoder-service`, `one-stop-service`, `orgbook-service`, `watchdog`,
`sharepoint-sync-tool`.

### Namespace alias

`Dynamics-Autorest` (legacy, being phased out) and `Dynamics-Dataverse` both export types in
`Gov.Lclb.Cllb.Interfaces`. Consuming projects reference `Dynamics-Dataverse` with an MSBuild
alias to avoid collisions:

```xml
<ProjectReference Include="..\cllc-interfaces\Dynamics-Dataverse\Dynamics-Dataverse.csproj">
  <Aliases>DV</Aliases>
</ProjectReference>
```

```csharp
extern alias DV;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DataverseClient = DV::Gov.Lclb.Cllb.Interfaces.DataverseClient;
```

---

## Code patterns

**Async** — always use the SDK's native async methods, never `Task.Run` wrappers:

```csharp
// Avoid
var entity = await Task.Run(() => _serviceClient.Retrieve(name, id, cols), ct);

// Prefer
var entity = await _serviceClient.RetrieveAsync(name, id, cols, ct);
```

**Reading records** — SDK entity → ViewModel via `ToViewModel()` extension methods.

**Writing records** — ViewModel → SDK entity via `CopyValues()` extension methods. Only
populate the fields that changed; use `copyIfNull` where the caller must be able to explicitly
clear a field.

**Creating a new entity — do not set `Id`.** Leave the primary key untouched before `Create`;
letting Dataverse generate it. Explicitly assigning `Guid.Empty` is treated by Dataverse as a
supplied-but-invalid key (`Expected non-empty Guid`), not as "please generate one."

**Child/related data** — loaded with parallel `WithChildren`-style calls (multiple async
fetches issued together, then awaited), not sequential N+1 queries.

**Adding a new entity**: run `pac modelbuilder` to regenerate the SDK entity class into
`Generated/`, add the corresponding method(s) to `IDataverseClient`, implement them in
`DataverseClient.cs`.

---

## Configuration model (OpenShift / GHA secrets)

Dataverse credentials are **shared** across every GHA service via the `cllc-public` secret
(pre-existing in every namespace) — this is the single source of truth; do not duplicate
`DYNAMICS_*` values into a service-specific secret. See `lcrb-carla-pipelines/docs/SECRETS.md`
for the full secret model and the credential-rotation runbook.

---

## Reference documents

| Topic | File |
|---|---|
| Full setup, packages, DI, async patterns | `setup-and-authentication.md` |
| Entity generation (`pac modelbuilder`) | `entity-generation.md` |
| Model-extension conventions (`ToViewModel`/`CopyValues`) | `model-extensions-port.md` |
| Cross-cutting build fixes / gotchas | `build-validation-common-fixes.md` |
| Cross-branch regression analysis | `regression-analysis-report.md` |

Per-controller/per-service migration history (ticket-by-ticket detail) lives in the remaining
files in this folder and in `../tasks/`.
