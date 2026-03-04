# SharePoint to Dynamics Document Location Sync Tool

## Overview

This tool synchronizes SharePoint folders with Dynamics CRM document location table. It's designed to fix situations where folders were created in SharePoint but the corresponding document location records were not created in Dynamics.

The tool:

1. Fetches all folders from a specified SharePoint document library (with optional filtering by creation date)
2. For each folder, extracts the GUID from the folder name
3. Checks if a document location record already exists in Dynamics
4. Creates missing document location records and links them to the appropriate entities

## Prerequisites

- .NET 6.0 SDK or later
- Access to SharePoint (On-Premises or Cloud)
- Access to Dynamics CRM
- Appropriate credentials for both systems

## Architecture

This tool leverages existing LCRB infrastructure components:

- **SharePoint Integration**: Uses `OnPremSharePointFileManager` from `cllc-interfaces\SharePoint\` (On-Premises only)
- **Dynamics Integration**: Uses `DynamicsClient` and `DynamicsSetupUtil` from `cllc-interfaces\Dynamics-Autorest\`
- **Authentication**: Uses Cloud Dataverse (AAD/OAuth2) for Dynamics and ADFS for On-Premises SharePoint

## Building the Tool

```bash
cd sharepoint-sync-tool
dotnet build
```

## Configuration

The tool is configured entirely through environment variables. All configuration must be provided before running the tool.

### Required Environment Variables

#### SharePoint Configuration

**On-Premises SharePoint** (ADFS authentication):

```bash
SHAREPOINT_ODATA_URI=https://your-sharepoint-server/sites/yoursite/_api/
SHAREPOINT_NATIVE_BASE_URI=https://your-sharepoint-server/sites/yoursite
SHAREPOINT_USERNAME=domain\username
SHAREPOINT_PASSWORD=your-password
SHAREPOINT_STS_TOKEN_URI=https://your-sts-server/adfs/services/trust/2005/UsernameMixed
SHAREPOINT_RELYING_PARTY_IDENTIFIER=urn:sharepoint:yourapplication
```

#### Dynamics CRM Configuration

**Cloud Dataverse (AAD) Authentication:**

```bash
DYNAMICS_ODATA_URI=https://your-dynamics-instance.crm.dynamics.com/
DYNAMICS_AAD_TENANT_ID=your-tenant-id
DYNAMICS_SERVER_APP_ID_URI=https://your-dynamics-instance.crm.dynamics.com
DYNAMICS_APP_REG_CLIENT_ID=your-app-registration-client-id
DYNAMICS_APP_REG_CLIENT_KEY=your-app-registration-client-secret
```

#### Sync Configuration

```bash
# Required: Entity type to sync (account, application, contact, worker, event, licence)
SYNC_ENTITY_NAME=application

# Required: SharePoint document library name (e.g., 'account', 'adoxio_application', 'adoxio_worker')
SYNC_DOCUMENT_LIBRARY=adoxio_application

# Optional: Only process folders created after this date (format: YYYY-MM-DD)
SYNC_MODIFIED_AFTER_DATE=2024-01-01

# Optional: Number of folders to process in each batch (default: 100)
SYNC_BATCH_SIZE=50

# Optional: Dry run mode - log what would happen without making changes (default: false)
SYNC_DRY_RUN=true
```

## Usage Examples

### Example 1: Sync Application Folders (Dry Run)

First, test with dry run mode to see what would happen:

```bash
# Linux/Mac
export SHAREPOINT_ODATA_URI="https://sharepoint.example.com/sites/lcrb/_api/"
export SHAREPOINT_NATIVE_BASE_URI="https://sharepoint.example.com/sites/lcrb"
export SHAREPOINT_USERNAME="domain\serviceaccount"
export SHAREPOINT_PASSWORD="password123"

export DYNAMICS_ODATA_URI="https://lcrb.crm3.dynamics.com/"
export DYNAMICS_AAD_TENANT_ID="12345678-1234-1234-1234-123456789abc"
export DYNAMICS_SERVER_APP_ID_URI="https://lcrb.crm3.dynamics.com"
export DYNAMICS_APP_REG_CLIENT_ID="87654321-4321-4321-4321-cba987654321"
export DYNAMICS_APP_REG_CLIENT_KEY="your-secret"

export SYNC_ENTITY_NAME="application"
export SYNC_DOCUMENT_LIBRARY="adoxio_application"
export SYNC_MODIFIED_AFTER_DATE="2024-01-01"
export SYNC_DRY_RUN="true"

dotnet run --project sharepoint-sync-tool.csproj
```

### Example 2: Actually Sync Worker Folders

Once you've verified the dry run output, remove the dry run flag:

```bash
$env:SYNC_ENTITY_NAME="worker"
$env:SYNC_DOCUMENT_LIBRARY="adoxio_worker"
$env:SYNC_DRY_RUN="false"  # or just remove this line

dotnet run --project sharepoint-sync-tool.csproj
```

### Example 3: Using Environment File

Create a file named `.env.local` (don't commit this to source control):

```bash
# SharePoint Configuration
SHAREPOINT_ODATA_URI=https://sharepoint.example.com/sites/lcrb/_api/
SHAREPOINT_NATIVE_BASE_URI=https://sharepoint.example.com/sites/lcrb
SHAREPOINT_USERNAME=domain\serviceaccount
SHAREPOINT_PASSWORD=password123

# Dynamics Configuration
DYNAMICS_ODATA_URI=https://lcrb.crm3.dynamics.com/
DYNAMICS_AAD_TENANTID=12345678-1234-1234-1234-123456789abc
DYNAMICS_CLIENT_ID=87654321-4321-4321-4321-cba987654321
DYNAMICS_CLIENT_SECRET=your-secret

# Sync Configuration
SYNC_ENTITY_NAME=application
SYNC_DOCUMENT_LIBRARY=adoxio_application
SYNC_MODIFIED_AFTER_DATE=2024-01-01
SYNC_BATCH_SIZE=100
SYNC_DRY_RUN=false
```

Then load the environment variables:

```bash
# Linux/Mac
export $(cat .env.local | xargs)
dotnet run --project sharepoint-sync-tool.csproj
```

## Supported Entity Types

The tool supports the following entity types:

| Entity Name | Document Library Name | Folder Name Format     |
| ----------- | --------------------- | ---------------------- |
| account     | account               | `{AccountName}_{GUID}` |
| application | adoxio_application    | `{JobNumber}_{GUID}`   |
| contact     | contact               | `contact_{GUID}`       |
| worker      | adoxio_worker         | `{WorkerName}_{GUID}`  |
| event       | adoxio_event          | `{EventName}_{GUID}`   |
| licence     | adoxio_licences       | `{LicenceName}_{GUID}` |

The GUID in the folder name is the entity ID without dashes and in uppercase.

Example: `Application-123_A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6`

## How It Works

1. **Folder Discovery**: The tool queries SharePoint to get all folders in the specified document library
2. **GUID Extraction**: For each folder, it extracts the GUID from the folder name using the pattern `_[A-F0-9]{32}$`
3. **Existence Check**: It queries Dynamics to see if a document location already exists with the same `relativeurl`
4. **Document Location Creation**: If not exists, it creates:
   - A new `sharepointdocumentlocation` record with the server relative URL
   - Sets the parent to the document library location
   - Links it to the entity using the extracted GUID
   - Adds the reference to the entity's document location collection

## Output

The tool provides detailed logging:

```
info: SharePointSyncTool.Program[0]
      Starting SharePoint to Dynamics Sync Tool
info: SharePointSyncTool.Program[0]
      Configuration validated successfully
info: SharePointSyncTool.Program[0]
      Entity Name: application
info: SharePointSyncTool.Program[0]
      Document Library: adoxio_application
info: SharePointSyncTool.SyncService[0]
      Found 150 total folders in SharePoint
info: SharePointSyncTool.SyncService[0]
      Processing batch 1/2 (100 folders)
info: SharePointSyncTool.SyncService[0]
      Created document location for: Application-001_1234567890ABCDEF...
info: SharePointSyncTool.SyncService[0]
      Sync Summary:
info: SharePointSyncTool.SyncService[0]
      Total Processed: 150
info: SharePointSyncTool.SyncService[0]
      Created: 45
info: SharePointSyncTool.SyncService[0]
      Already Exists (Skipped): 102
info: SharePointSyncTool.SyncService[0]
      Errors: 3
```

## Troubleshooting

### "Could not extract GUID from folder name"

This means the folder name doesn't match the expected pattern. Check that:

- Folder names end with underscore followed by 32 hex characters
- The GUID portion is uppercase and has no dashes

### "Parent document library not found"

The tool couldn't find the parent document library location in Dynamics. Ensure:

- The `SYNC_DOCUMENT_LIBRARY` value matches the `relativeurl` of an existing document library location
- The document library exists in SharePoint

### "Folder listing for Cloud SharePoint not yet implemented"

Currently, the tool only supports folder listing for On-Premises SharePoint. To use with Cloud SharePoint, you would need to implement the `GetFoldersInDocumentLibraryAfterDate` method in `CloudSharePointFileManager`.

## Security Considerations

- Never commit credentials to source control
- Use service accounts with minimum required permissions
- Consider using Azure Key Vault or similar for storing secrets in production
- Run in dry-run mode first to verify behavior
- Process in small batches and monitor for errors

## Limitations

- Date filtering is not currently implemented (FolderItem doesn't have TimeCreated property)
- Folder listing only works with On-Premises SharePoint
- The tool processes folders sequentially within batches
- No automatic retry logic for transient failures

## Future Enhancements

- Implement Cloud SharePoint folder listing using Graph API
- Add date filtering support by fetching folder metadata
- Add parallel processing within batches
- Implement retry logic with exponential backoff
- Add progress reporting and interruption support
- Support for additional entity types

## License

This tool is part of the LCRB project and subject to the same license terms.
