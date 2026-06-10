# Build Validation — Common Migration Fixes

This document records the cross-cutting compile errors found during a full build validation pass after the Dynamics-Dataverse migration, and the canonical fix for each pattern. Applied across all projects in the solution.

---

## 1. Orphan `using Gov.Lclb.Cllb.Interfaces;` in Startup.cs

**Affected services:** `federal-reporting-service`, `ldb-orders-service`, `orgbook-service`, `watchdog`, `one-stop-service`

**Error:**
```
CS0234: The type or namespace name 'Interfaces' does not exist in the namespace 'Gov.Lclb.Cllb'
CS0246: The type or namespace name 'Gov' could not be found
```

**Cause:** During migration the project reference was switched to `Dynamics-Dataverse` with `<Aliases>DV</Aliases>`. The plain `using Gov.Lclb.Cllb.Interfaces;` that worked when `DynamicsAutorest` was unaliased is now unreachable — the DV assembly requires the `DV::` prefix.

**Fix:** Remove the plain `using Gov.Lclb.Cllb.Interfaces;` from `Startup.cs`. `IDataverseClient` and `DataverseClient` are already brought in via explicit aliases at the top of the file:
```csharp
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DataverseClient  = DV::Gov.Lclb.Cllb.Interfaces.DataverseClient;
```

---

## 2. Newtonsoft.Json version downgrade (NU1605)

**Affected projects:** `watchdog`, `sharepoint-sync-tool`

**Error:**
```
NU1605: Detected package downgrade: Newtonsoft.Json from 13.0.3 to 13.0.1.
  project -> Dynamics-Dataverse -> Newtonsoft.Json (>= 13.0.3)
  project -> Newtonsoft.Json (>= 13.0.1)
```

**Cause:** `Dynamics-Dataverse` upgraded its `Newtonsoft.Json` dependency to `13.0.3`; services that pinned `13.0.1` now fail the downgrade check.

**Fix:** Bump `Newtonsoft.Json` to `13.0.3` in the service's `.csproj`:
```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

---

## 3. Dataverse property casing — lowercase vs PascalCase

**Affected files:** `cllc-public-app` ModelExtensions and Controllers

**Error:**
```
CS1061: 'Entity' does not contain a definition for 'adoxio_propertyname'
```

**Cause:** The Dataverse Model Builder generates property names with mixed casing (e.g. `adoxio_Slug`, `adoxio_Category`, `adoxio_LicenceNumber`). AutoRest used all-lowercase. Any property copied from AutoRest code must be matched against the generated entity class.

**Fix:** Check `cllc-interfaces/Dynamics-Dataverse/Generated/Entities/<entity>.cs` for the exact C# property name. Common examples:
| AutoRest (wrong) | Dataverse SDK (correct) |
|---|---|
| `adoxio_licencenumber` | `adoxio_LicenceNumber` |
| `adoxio_slug` | `adoxio_Slug` |
| `adoxio_account` | `adoxio_Account` |
| `adoxio_licence` | `adoxio_Licence` |
| `Websiteurl` | `WebSiteURL` |

---

## 4. DateTimeOffset? → DateTime? conversion

**Affected files:** `Account.cs`, `SpecialEvent.cs`, `AdoxioTiedHouseConnections.cs`, `Worker.cs`, `PreviousAddress.cs`

**Error:**
```
CS0029: Cannot implicitly convert type 'System.DateTimeOffset?' to 'System.DateTime?'
CS0030: Cannot convert type 'System.DateTime' to 'System.DateTimeOffset?'
```

**Cause:** ViewModel date fields are `DateTimeOffset?`; Dataverse SDK entity date fields are `DateTime?`. AutoRest used `DateTimeOffset?` everywhere.

**Fix:** Use `.UtcDateTime` when writing to an entity field, and wrap in `DateTimeOffset?` when reading back:
```csharp
// ViewModel → Entity (write)
entity.adoxio_SomeDate = from.SomeDate?.UtcDateTime;

// Entity → ViewModel (read)
result.SomeDate = entity.adoxio_SomeDate.HasValue
    ? (DateTimeOffset?)entity.adoxio_SomeDate.Value
    : null;
```
Also ensure `using System;` is present — `DateTimeOffset` lives in the `System` namespace.

---

## 5. Ambiguous type references when both AutoRest and DV are referenced

**Affected files:** `one-stop-service/OneStopUtils.cs`, `LicenceEventsController.cs`, `EligibiltyController.cs`, `UserController.cs`

**Error:**
```
CS0104: 'TypeName' is an ambiguous reference between 'Gov.Lclb.Cllb.Interfaces.TypeName'
        and 'Gov.Lclb.Cllb.Interfaces.Models.TypeName'
```

**Cause:** Both `using DV::Gov.Lclb.Cllb.Interfaces;` and `using Gov.Lclb.Cllb.Interfaces.Models;` are in scope. When the same enum/class is defined in both (e.g. `OneStopHubStatusChange`, `OneStopMessageStatus`, `Account`, `User`), the compiler can't pick one.

**Fix:** Add an explicit using alias to pin the desired type:
```csharp
// Pin to DV version (when the type is used with Dataverse entities)
using OneStopHubStatusChange = DV::Gov.Lclb.Cllb.Interfaces.OneStopHubStatusChange;
using OneStopMessageStatus   = DV::Gov.Lclb.Cllb.Interfaces.OneStopMessageStatus;

// Qualify inline for ViewModel vs entity disambiguation
var patchAccount = new DV::Gov.Lclb.Cllb.Interfaces.Account { Id = accountId };
userSettings.AuthenticatedUser = new Models.User();
```

For services that still reference AutoRest (e.g. `one-stop-service`), enums used with Dataverse entity fields **must** be the DV version — the generated entities type their option-set fields to the DV enum, not the AutoRest one.

---

## 6. Duplicate extension methods causing CS0121 ambiguity

**Affected file:** `AdoxioTiedHouseConnections.cs`

**Error:**
```
CS0121: The call is ambiguous between the following methods or properties:
  'AdoxioTiedhouseconnectionsExtensions.CopyValues(...)' and
  'AdoxioTiedHouseConnectionDataverseExtensions.CopyValues(...)'
```

**Cause:** Dataverse overloads (`CopyValues` / `ToViewModel` for `adoxio_tiedhouseconnection`) were added to the existing AutoRest extension class, but the same overloads already existed in the dedicated `AdoxioTiedHouseConnectionDataverse.cs` file.

**Fix:** Keep overloads only in the dedicated Dataverse extension file. Never add DV overloads to the AutoRest extension class.

---

## 7. Extension method not in scope (CS1061 on IDataverseClient)

**Affected file:** `FormsController.cs`

**Error:**
```
CS1061: 'IDataverseClient' does not contain a definition for 'GetSystemformViewModelAsync'
```

**Cause:** `GetSystemformViewModelAsync` is a static extension method defined in `Gov.Lclb.Cllb.Interfaces.DynamicsExtensions` (in `Contexts/DynamicsExtensions.cs`). The controller was missing the using directive that brings that namespace into scope.

**Fix:** Add:
```csharp
using Gov.Lclb.Cllb.Interfaces;
```

---

## 8. Unused type alias referencing removed assembly

**Affected file:** `federal-reporting-service/FederalReportingController.cs`

**Error:**
```
CS0234: The type or namespace name 'Interfaces' does not exist in the namespace 'Gov.Lclb.Cllb'
```

**Cause:** `using FolderSegment = Gov.Lclb.Cllb.Interfaces.FolderSegment;` was a leftover alias. `FolderSegment` is never used in the file, and the AutoRest assembly is no longer referenced.

**Fix:** Delete the unused alias line entirely.
