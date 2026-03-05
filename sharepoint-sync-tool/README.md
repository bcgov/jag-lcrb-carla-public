# SharePoint to Dynamics Document Location Sync Tool

## Overview

This tool synchronizes SharePoint folders with Dynamics CRM document location table. It's designed to fix situations where folders were created in SharePoint but the corresponding document location records were not created in Dynamics.

The tool:

1. Fetches all folders from a specified SharePoint document library (with optional filtering by last modified date)
2. For each folder, extracts the GUID from the folder name
3. Checks if a document location record already exists in Dynamics
4. Creates missing document location records and links them to the appropriate entities

## Building the Tool

```bash
cd sharepoint-sync-tool
dotnet build
```

## Configuration

The tool is configured entirely through environment variables. All configuration must be provided before running the tool.

### Required Environment Variables

#### SharePoint Configuration

**On-Premises SharePoint:**

```bash
SHAREPOINT_ODATA_URI=https://your-sharepoint-server.gov.bc.ca/
SHAREPOINT_NATIVE_BASE_URI=https://your-sharepoint-server.gov.bc.ca/
SHAREPOINT_USERNAME=username@idir
SHAREPOINT_PASSWORD=your-password
SHAREPOINT_STS_TOKEN_URI=https://sts-server.gov.bc.ca/adfs/services/trust/2005/UsernameMixed
SHAREPOINT_RELYING_PARTY_IDENTIFIER=urn:sharepoint:yourapplication
```

**Cloud SharePoint:**

```bash
SHAREPOINT_ODATA_URI=https://bcgov.sharepoint.com/sites/your-site-name
SHAREPOINT_AAD_TENANTID=tenant-id
SHAREPOINT_CLIENT_ID=client-id
SHAREPOINT_CLIENT_SECRET=client-secret
SHAREPOINT_WEBNAME=your-site-name
```

#### Dynamics CRM Configuration

**Cloud Dataverse:**

```bash
DYNAMICS_ODATA_URI=https://your-dynamics-instance.crm3.dynamics.com/api/data/v9.2/
DYNAMICS_NATIVE_ODATA_URI=https://your-dynamics-instance.crm3.dynamics.com/api/data/v9.2/
DYNAMICS_AAD_TENANT_ID=your-tenant-id
DYNAMICS_SERVER_APP_ID_URI=https://your-dynamics-instance.crm3.dynamics.com/
DYNAMICS_APP_REG_CLIENT_ID=your-app-registration-client-id
DYNAMICS_APP_REG_CLIENT_KEY=your-app-registration-client-secret
```

#### Sync Configuration

```bash
# Required: Entity type to sync (account, application, contact, worker, event, licence)
SYNC_ENTITY_NAME=application

# Optional: Only process folders created after this date (format: YYYY-MM-DD)
SYNC_MODIFIED_AFTER_DATE=2026-02-06

# Optional: Number of folders to process in each batch (default: 100)
SYNC_BATCH_SIZE=50

# Optional: Index range for processing records (0-based, set both to 0 to process all)
SYNC_START_INDEX=0
SYNC_END_INDEX=1

# Optional: Dry run mode - log what would happen without making changes (default: false)
SYNC_DRY_RUN=true

# Optional: Set the logging level of the script (default: Information)
LOG_LEVEL=Information
```

## Usage

First, test with dry run mode to see what would happen:

```bash
export SHAREPOINT_ODATA_URI="https://your-sharepoint-server.gov.bc.ca/"
export SHAREPOINT_NATIVE_BASE_URI="https://your-sharepoint-server.gov.bc.ca/"
export SHAREPOINT_USERNAME="username@idir"
export SHAREPOINT_PASSWORD="your-password"
export SHAREPOINT_STS_TOKEN_URI="https://sts-server.gov.bc.ca/adfs/services/trust/2005/UsernameMixed"
export SHAREPOINT_RELYING_PARTY_IDENTIFIER="urn:sharepoint:yourapplication"

export DYNAMICS_ODATA_URI="https://lcrb.crm3.dynamics.com/"
export DYNAMICS_AAD_TENANT_ID="12345678-1234-1234-1234-123456789abc"
export DYNAMICS_SERVER_APP_ID_URI="https://lcrb.crm3.dynamics.com"
export DYNAMICS_APP_REG_CLIENT_ID="87654321-4321-4321-4321-cba987654321"
export DYNAMICS_APP_REG_CLIENT_KEY="your-secret"

export SYNC_ENTITY_NAME="application"
export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
export SYNC_DRY_RUN="true"
export SYNC_BATCH_SIZE="100"
export SYNC_START_INDEX="0"
export SYNC_END_INDEX="0"

dotnet run --project sharepoint-sync-tool.csproj
```

Once you've verified the dry run output, remove the dry run flag:

```bash
export SYNC_DRY_RUN="false"

dotnet run --project sharepoint-sync-tool.csproj
```

## Example

See `run-sync.example.sh` for a pre-built
