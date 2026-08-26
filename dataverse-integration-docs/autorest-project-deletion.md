# LCSD-8564: Remove Dynamics-Autorest project from solution

## Summary

The `cllc-interfaces/Dynamics-Autorest` folder has been deleted. All `ProjectReference` and
`.sln` entries pointing to `DynamicsAutorest.csproj` have been removed. Zero `IDynamicsClient`
or `MicrosoftDynamicsCRM*` references existed outside the project before deletion.

---

## Files Changed

### .csproj files — ProjectReference removed
| File | Action |
|---|---|
| `cllc-public-app/cllc-public-app.csproj` | Removed `<ProjectReference Include="..\cllc-interfaces\Dynamics-Autorest\DynamicsAutorest.csproj" />` |
| `cllc-interfaces/OData.OpenAPI/odata2openapi/odata2openapi.csproj` | Removed `<ProjectReference Include="..\..\Dynamics-Autorest\DynamicsAutorest.csproj" />` and its `<ItemGroup>` wrapper |

Note: The other 7 service `.csproj` files (carla-spice-sync-service, federal-reporting-service,
ldb-orders-service, geocoder-service, one-stop-service, orgbook-service, watchdog) did NOT have a
direct `ProjectReference` to `DynamicsAutorest.csproj` — they only referenced it via `.sln` for
VS browsing, not as a compile dependency.

### .sln files — Project entry and build config removed (10 files)
| Solution | DynamicsAutorest GUID removed |
|---|---|
| `cllc-public-app/cllc-public-app.sln` | `{309D775B-CF5B-4194-9E63-3410A95D20F4}` |
| `cllc-public-app-test/cllc-public-app-test.sln` | `{309D775B-CF5B-4194-9E63-3410A95D20F4}` |
| `carla-spice-sync-service/CarlaSpiceSync.sln` | `{309D775B-CF5B-4194-9E63-3410A95D20F4}` |
| `one-stop-service/one-stop-service.sln` | `{309D775B-CF5B-4194-9E63-3410A95D20F4}` |
| `watchdog/Watchdog.sln` | `{B77C0A77-8243-4D20-9FD7-B4F55F05936D}` |
| `orgbook-service/orgbook-service.sln` | `{F08E1B16-C244-4380-ACA0-C1DAB7E3F18A}` |
| `ldb-orders-service/ldb-orders-service.sln` | `{86DC98C2-5007-4444-A9AF-EB085FB5600F}` |
| `geocoder-service/geocoder-service.sln` | `{4A132CDF-0A93-4AFA-B587-EBFF6B54C038}` |
| `federal-reporting-service/federal-reporting-service.sln` | `{108B2CB1-0A07-4C7C-9F83-9068E5D59C81}` |
| `cllc-interfaces/OData.OpenAPI/odata2openapi.sln` | `{9F3D4FF0-970C-436A-BDBA-33650FE26808}` |

### Folder deleted
`cllc-interfaces/Dynamics-Autorest/` — ~5,900 AutoRest-generated `.cs` files eliminated.

---

### Infrastructure files — stale Dynamics-Autorest references removed
| File | Action |
|---|---|
| `.gitignore` | Removed `/cllc-interfaces/Dynamics-Autorest/package-lock.json` and `/cllc-interfaces/Dynamics-Autorest/code-model-v1` entries |
| `.github/workflows/cd-orgbook-service.yml` | Removed `/cllc-interfaces/Dynamics-Autorest/**` path trigger |
| `cllc-interfaces/.gitattributes` | Cleared file (only contained `Dynamics-Autorest/**` LF line-ending rule) |
| `Dockerfile` | Removed `COPY ["cllc-interfaces/Dynamics-Autorest/DynamicsAutorest.csproj", ...]` layer |

---

## Pre-deletion verification

- Zero `IDynamicsClient` matches outside `Dynamics-Autorest` in any `.cs` file
- Zero `MicrosoftDynamicsCRM*` matches outside `Dynamics-Autorest` in any `.cs` file
- Only 2 `.csproj` files referenced `DynamicsAutorest.csproj` directly (`cllc-public-app.csproj`, `odata2openapi.csproj`)
</content>
