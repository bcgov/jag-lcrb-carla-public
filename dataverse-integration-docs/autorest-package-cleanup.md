# LCSD-8562: Remove deprecated NuGet packages

## Summary

Removed stale `Microsoft.IdentityModel.Clients.ActiveDirectory` (ADAL) package references and
dead `Microsoft.Rest` extension code from projects where these packages are no longer used.
`Microsoft.Rest.ClientRuntime` is intentionally kept in AutoRest interface clients for other APIs
(GeoCoder, SPICE, SharePoint) that are outside the Dynamics migration scope.

---

## Files Changed

| File | Change |
|---|---|
| `geocoder-service/geocoder-service.csproj` | Removed ADAL package reference — not used in service code or `cllc-interfaces/GeoCoder` |
| `sharepoint-sync-tool/sharepoint-sync-tool.csproj` | Removed ADAL package reference — not used directly; flows transitively from `SharePoint.csproj` which does use it |
| `cllc-interfaces/GeocoderClient/Geocoder.csproj` | Removed ADAL package reference — GeocoderClient uses API-key credentials (`ServiceClientCredentials`), not ADAL |
| `cllc-interfaces/PDF/PDF.csproj` | Removed ADAL and `Microsoft.Rest.ClientRuntime` — PDF client uses plain `HttpClient`, not AutoRest |
| `cllc-public-app/Startup.cs` | Removed stale `using Microsoft.IdentityModel.Clients.ActiveDirectory` — never used in body |
| `cllc-public-app/Extensions/LoggerExtensions.cs` | Deleted — `HttpOperationException`-specific log helper, never called after Dynamics migration; all `HttpOperationException` catch blocks in cllc-public-app were collapsed to `catch (Exception)` in LCSD-8561 |

---

## Packages Deliberately Kept

| Project | Package | Reason |
|---|---|---|
| `cllc-interfaces/SharePoint/SharePoint.csproj` | ADAL | `OnPremSharePointFileManager.cs` uses `AuthenticationContext.AcquireTokenAsync` for on-prem SharePoint auth |
| `cllc-interfaces/GeoCoder/Geocoder.csproj` | `Microsoft.Rest.ClientRuntime` | AutoRest-generated geocoder client — actively used by `geocoder-service` |
| `cllc-interfaces/GeocoderClient/Geocoder.csproj` | `Microsoft.Rest.ClientRuntime` | AutoRest-generated client used by `cllc-public-app` for address autocomplete |
| `cllc-interfaces/SPICE/SpiceClient.csproj` | `Microsoft.Rest.ClientRuntime` | AutoRest-generated SPICE API client — actively used by `carla-spice-sync-service` |
| `cllc-interfaces/Dynamics-Autorest/DynamicsAutorest.csproj` | Both | Being deleted entirely in LCSD-8564 |
| `carla-spice-sync-service` | `Microsoft.Rest` (via SPICE client) | SPICE client still uses `HttpOperationException` — out of scope for Dynamics cleanup |
</content>
