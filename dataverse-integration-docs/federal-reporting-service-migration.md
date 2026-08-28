# LCSD-8547: Migrate federal-reporting-service to IDataverseClient

## Overview

`FederalReportingController` exports cannabis monthly report data as CSV for federal tracking. It was migrated from `IDynamicsClient` (AutoRest) to `IDataverseClient` (Dataverse SDK).

## New IDataverseClient methods

### Federal Report Export
```csharp
Task<IList<adoxio_federalreportexport>> GetPendingFederalReportExportsAsync(ct);
Task UpdateFederalReportExportAsync(adoxio_federalreportexport export, ct);
```
Queries `adoxio_exportcompleted eq null` for pending exports.

### Cannabis Monthly Report
```csharp
Task<IList<adoxio_cannabismonthlyreport>> GetSubmittedCannabisMonthlyReportsAsync(ct);
Task UpdateCannabisMonthlyReportAsync(adoxio_cannabismonthlyreport report, ct);
```
Queries by `statuscode = Submitted (845280001)`.

### Cannabis Inventory Report
```csharp
Task<IList<adoxio_cannabisinventoryreport>> GetInventoryReportsByMonthlyReportIdAsync(string monthlyReportId, ct);
```
Queries by `adoxio_monthlyreportid` lookup.

### Cannabis Product Admin (generic entity — not generated)
```csharp
Task<string?> GetCannabisProductAdminNameByIdAsync(string id, ct);
```
`adoxio_cannabisproductadmin` has no generated SDK class. Uses `_serviceClient.Retrieve("adoxio_cannabisproductadmin", guid, new ColumnSet("adoxio_name"))` and returns just the name string.

### Extended SharePoint doc location
```csharp
Task<IList<SharePointDocumentLocation>> GetSharePointDocLocsByRelativeUrlAndNameAsync(string relativeUrl, string name, ct);
Task AssociateFederalReportExportWithDocLocAsync(string exportId, string docLocId, ct);
```
`AssociateFederalReportExportWithDocLocAsync` replaces the old `Federalreportexports.AddReference(...)` using `_serviceClient.Associate(...)` with the `adoxio_federalreportexport_SharePointDocumentLocations` relationship.

## Key translation notes

| Old (AutoRest) | New (Dataverse SDK) |
|---|---|
| `_dynamicsClient.Federalreportexports.Get(filter: ...)` | `GetPendingFederalReportExportsAsync()` |
| `_dynamicsClient.Cannabismonthlyreports.Get(filter: ...)` | `GetSubmittedCannabisMonthlyReportsAsync()` |
| `_dynamicsClient.Cannabisinventoryreports.Get(filter: ...)` | `GetInventoryReportsByMonthlyReportIdAsync(reportId)` |
| `_dynamicsClient.Cannabisproductadmins.GetByKey(id)` | `GetCannabisProductAdminNameByIdAsync(id)` (returns `string?`) |
| `_dynamicsClient.GetEntityURI(...)` + OData bind props | `new EntityReference(entityLogicalName, guid)` |
| `_dynamicsClient.Federalreportexports.AddReference(...)` | `AssociateFederalReportExportWithDocLocAsync(...)` |
| `export.GetDocumentFolderName()` (AutoRest extension) | Inlined: `$"{export.adoxio_name}_{export.Id.ToString().ToUpper().Replace("-", "")}"`  |
| `patchRecord.AdoxioFederalReportExportIdODateBind = uri` | `patchRecord.adoxio_FederalReportExportId = new EntityReference(...)` |

## Money field handling

`adoxio_ValueofClosingInventory` and `adoxio_TotalValue` are `Microsoft.Xrm.Sdk.Money` in the generated entity. Access the decimal via `.Value`:
```csharp
// Old
(double)inventoryReport.AdoxioValueofclosinginventory

// New
(double)inventoryReport.adoxio_ValueofClosingInventory.Value
```

## PopulateProduct signature change

`FederalReportingMonthlyExport.PopulateProduct` signature updated:
```csharp
// Old
public void PopulateProduct(MicrosoftDynamicsCRMadoxioCannabisinventoryreport inventoryReport,
                            MicrosoftDynamicsCRMadoxioCannabisproductadmin product)

// New
public void PopulateProduct(adoxio_cannabisinventoryreport inventoryReport, string productName)
```

## Hangfire wiring (Startup.cs)

`IDataverseClient` resolved from DI scope and passed to controller constructor:
```csharp
var dataverseClient = serviceScope.ServiceProvider.GetRequiredService<IDataverseClient>();
RecurringJob.AddOrUpdate(() =>
    new FederalReportingController(Configuration, loggerFactory, _fileManagerClient, dataverseClient)
        .ExportFederalReports(null), "*/10 * * * *");
```
