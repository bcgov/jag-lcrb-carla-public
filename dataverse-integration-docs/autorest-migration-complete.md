# LCSD-8565: Final regression and documentation — AutoRest migration complete

## Summary

All AutoRest dependencies have been removed from the repository. The `IDataverseClient` /
`DataverseClient` SDK client is now the sole Dynamics 365 integration layer.
This document records the final validation and documentation steps.

---

## Test suite status

Run before reporting this ticket complete:

```powershell
$testTargets = @(
  "cllc-public-app-test/cllc-public-app-test.sln",
  "geocoder-service-test/geocoder-service-test.sln",
  "cllc-interfaces-test/OrgBook/OrgBook.Tests.sln",
  "cllc-interfaces-test/PDF/PDF.Tests.sln"
)
foreach ($t in $testTargets) {
  dotnet test $t --logger "console;verbosity=normal"
  if (-not $?) { Write-Error "TESTS FAILED: $t"; break }
}
```

No test files required updates — zero `IDynamicsClient` mock references existed in any
test project before this ticket.

---

## Files created / modified

### New files
| File | Purpose |
|---|---|
| `docs/adding-new-entities.md` | Step-by-step guide for adding Dataverse entities going forward |
| `cllc-public-app-test/Helpers/MockDataverseClientBuilder.cs` | Fluent mock builder for future `IDataverseClient` unit tests |

### Updated files
| File | Change |
|---|---|
| `README.md` | Updated Technology Stack table (net6.0, Dataverse cloud, AAD auth); updated Developer Prerequisites; updated Dynamic Forms step 1 to reference `generate-entities.ps1`; added "Adding New Dynamics Entities" section |
| `cllc-public-app-test/cllc-public-app-test.csproj` | Added `Moq` package reference (required by `MockDataverseClientBuilder`) |

---

## MockDataverseClientBuilder usage

```csharp
// In a unit test
var client = new MockDataverseClientBuilder()
    .WithAccount(accountId, "Test Account")
    .WithApplication(appId, app => { app.adoxio_name = "Test App"; })
    .Build();

// Or get the mock to verify calls
var builder = new MockDataverseClientBuilder().WithAccount(accountId, "Test Account");
var client = builder.Build();
// ... act ...
builder.Mock.Verify(c => c.GetAccountByIdAsync(accountId.ToString(), It.IsAny<CancellationToken>()), Times.Once);
```

---

## Completion checklist

- [ ] All 4 test projects pass: `dotnet test` on each `.sln` above
- [ ] All service builds pass: re-run `dotnet build` across all 9 service solutions
- [ ] Smoke-tested against dev Dataverse (login, application load, licence load, create application)
- [ ] Git tag applied: `git tag -a autorest-removal-complete -m "AutoRest migration complete - LCSD-8527 through LCSD-8565"`
