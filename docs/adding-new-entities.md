# Adding a New Dataverse Entity

## Prerequisites

- Power Platform CLI: `winget install Microsoft.PowerPlatformCLI`
- Authenticated session:
  ```powershell
  pac auth create --url https://<org>.crm.dynamics.com `
    --applicationId <DYNAMICS_APP_REG_CLIENT_ID> `
    --clientSecret <DYNAMICS_APP_REG_CLIENT_KEY> `
    --tenant <DYNAMICS_AAD_TENANT_ID>
  ```

## Steps

### 1. Add entity to the generation script

Edit `generate-entities.ps1` at the repo root and add the entity logical name to the `--entities` list:

```powershell
"adoxio_mynewentity"
```

### 2. Run generation

```powershell
.\generate-entities.ps1
```

The entity class appears in `cllc-interfaces/Dynamics-Dataverse/Generated/`. Do not edit generated files by hand.

### 3. Add interface methods

In `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs`:

```csharp
Task<adoxio_mynewentity?> GetMyNewEntityByIdAsync(string id, CancellationToken ct = default);
Task<IList<adoxio_mynewentity>> GetMyNewEntitiesByParentIdAsync(string parentId, CancellationToken ct = default);
```

### 4. Implement in DataverseClient

In `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs`:

```csharp
public async Task<adoxio_mynewentity?> GetMyNewEntityByIdAsync(string id, CancellationToken ct = default)
{
    if (!Guid.TryParse(id, out var guid)) return null;
    try
    {
        var entity = await Task.Run(() =>
            _serviceClient.Retrieve(adoxio_mynewentity.EntityLogicalName, guid, new ColumnSet(true)), ct);
        return entity?.ToEntity<adoxio_mynewentity>();
    }
    catch (Exception ex) when (ex.Message.Contains("Does Not Exist")) { return null; }
}
```

For list queries, use `QueryExpression` and `RetrieveMultiple`:

```csharp
public async Task<IList<adoxio_mynewentity>> GetMyNewEntitiesByParentIdAsync(string parentId, CancellationToken ct = default)
{
    var query = new QueryExpression(adoxio_mynewentity.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
    query.Criteria.AddCondition("adoxio_parentid", ConditionOperator.Equal, parentId);
    var result = await Task.Run(() => _serviceClient.RetrieveMultiple(query), ct);
    return result.Entities.Select(e => e.ToEntity<adoxio_mynewentity>()).ToList();
}
```

### 5. Commit

```powershell
git add cllc-interfaces/Dynamics-Dataverse/Generated/
git add cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs
git add cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs
git commit -m "Add MyNewEntity Dataverse entity"
```

## Notes

- Entity logical names are always lowercase (e.g. `adoxio_application`, `account`, `contact`).
- Generated class names follow PascalCase of the logical name (e.g. `adoxio_application` → `adoxio_application` class; `account` → `Account`).
- When consuming `Dynamics-Dataverse` types in `cllc-public-app`, prefix the `ProjectReference` with `<Aliases>DV</Aliases>` and add `extern alias DV;` at the top of any file that uses both the DV types and other packages that share type names.
- See `dataverse-integration-docs/entity-generation.md` for the full entity list and re-generation guide.
