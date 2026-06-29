# LCSD-8561: Verify zero IDynamicsClient references outside AutoRest project

## Summary

Safety-gate ticket before AutoRest deletion. All remaining AutoRest / `Microsoft.Rest` references
outside `Dynamics-Autorest` were found and eliminated. Zero `IDynamicsClient`,
`MicrosoftDynamicsCRM*`, or `using Gov.Lclb.Cllb.Interfaces.Models` references remain in any
project outside `Dynamics-Autorest`.

---

## Files Changed

| File | Change |
|---|---|
| `cllc-public-app/Contexts/DynamicsExtensions.cs` | `IsMostlyLiquor` signature changed from `List<MicrosoftDynamicsCRMadoxioLicences>` to `IList<ApplicationTypeCategory?>`; removed `using Gov.Lclb.Cllb.Interfaces.Models` |
| `cllc-public-app-test/PaymentIsLiquorTests.cs` | All 7 tests rewritten to pass `List<ApplicationTypeCategory?>` directly; AutoRest model construction removed |
| `cllc-public-app/Controllers/ApplicationsController.cs` | Removed stale `using Gov.Lclb.Cllb.Interfaces.Models` |
| `cllc-public-app/Controllers/LicensesController.cs` | Removed stale `using Gov.Lclb.Cllb.Interfaces.Models` |
| `cllc-public-app/Controllers/PaymentController.cs` | Removed stale `using Gov.Lclb.Cllb.Interfaces.Models` |
| `cllc-public-app/Controllers/TiedHouseConnectionsController.cs` | Removed `using Microsoft.Rest`; collapsed 7 dual `catch (HttpOperationException) + catch (Exception)` blocks into single `catch (Exception)` per method |
| `cllc-public-app/Startup.cs` | Removed `services.AddHttpClient<IDynamicsClient, DynamicsClient>()`; removed dead `#if (USE_MSSQL)` seeder block (referenced non-existent `Seeders.SeedFactory`) |
| `cllc-public-app-test/ApiIntegrationTestBaseWithLogin.cs` | Removed stale `using Gov.Lclb.Cllb.Interfaces.Models` |
| `cllc-public-app-test/AccountTests.cs` | Removed stale `using Gov.Lclb.Cllb.Interfaces.Models` |
| `cllc-public-app-test/AliasTests.cs` | Removed stale `using Gov.Lclb.Cllb.Interfaces.Models` |
| `cllc-public-app-test/EstablishmentTests.cs` | Removed stale `using Gov.Lclb.Cllb.Interfaces.Models` |
| `cllc-public-app-test/LegalEntityTests.cs` | Removed stale `using Gov.Lclb.Cllb.Interfaces.Models` |
| `cllc-public-app-test/LicenceEventsTests.cs` | Removed stale `using Gov.Lclb.Cllb.Interfaces.Models` |
| `cllc-public-app-test/WorkerTests.cs` | Removed stale `using Gov.Lclb.Cllb.Interfaces.Models` |
| `cllc-public-app-test/PreviousAddressTests.cs` | Removed stale `using Gov.Lclb.Cllb.Interfaces.Models` |
| `orgbook-service-test/VonAgentClientTests.cs` | Removed fully commented-out `/* ... */` test block containing AutoRest types; removed stale usings |
| `cllc-public-app/Models.Extensions/AdoxioTiedHouseConnectionDataverse.cs` | Removed AutoRest type name from doc comment |

---

## Key Decisions

### IsMostlyLiquor signature change
The old signature accepted `List<MicrosoftDynamicsCRMadoxioLicences>` and navigated
`l.AdoxioLicenceType?.AdoxioCategory` to compare against `ApplicationTypeCategory.Liquor`.
In the Dataverse SDK, `adoxio_licences.adoxio_LicenceType` is an `EntityReference` with no
embedded category — callers must resolve the category separately before calling this helper.
The parameter was changed to `IList<ApplicationTypeCategory?>` so callers pass the
already-resolved values. The comparison integer values are identical in both enum representations.

### TiedHouseConnectionsController catch collapse
Each of the 7 action methods had this pattern:
```csharp
catch (HttpOperationException httpOperationException)
{
    _logger.LogError(httpOperationException, "Error ...");
    throw new Exception("...");
}
catch (Exception exception)
{
    _logger.LogError(exception, "Error ...");
    throw new Exception("...");
}
```
Both handlers did exactly the same thing (log + rethrow). Collapsed to a single
`catch (Exception)` — identical runtime behaviour, no `Microsoft.Rest` dependency.

### Dead #if (USE_MSSQL) block in Startup.cs
The block was already permanently disabled (`#undef USE_MSSQL` at the top of the file) and
referenced `Seeders.SeedFactory`, a class that does not exist anywhere in the codebase.
Removing it was safe and correct.
</content>
</invoke>