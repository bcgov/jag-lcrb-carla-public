# SharePoint to Dynamics Document Location Sync Tool

## Overview

This tool synchronizes SharePoint folders with Dynamics CRM document location table. It's designed to fix situations where folders were created in SharePoint but the corresponding document location records were not created in Dynamics.

The tool:

1. Fetches folders from a specified SharePoint document library
2. For each folder, extracts the GUID from the folder name
3. Checks if a document location record already exists in Dynamics
4. Writes all document location records to a csv file for review, and future reference.
5. Creates missing document location records and links them to the appropriate entities

## Building the Tool

```bash
cd sharepoint-sync-tool
dotnet build
```

## Configuration

The tool is configured entirely through environment variables. All configuration must be provided before running the tool.

### Required Environment Variables

#### SharePoint Configuration

**On-Premise:**

```bash
SHAREPOINT_ODATA_URI=https://your-sharepoint-server.gov.bc.ca/
SHAREPOINT_NATIVE_BASE_URI=https://your-sharepoint-server.gov.bc.ca/
SHAREPOINT_USERNAME=username@idir
SHAREPOINT_PASSWORD=your-password
SHAREPOINT_STS_TOKEN_URI=https://sts-server.gov.bc.ca/adfs/services/trust/2005/UsernameMixed
SHAREPOINT_RELYING_PARTY_IDENTIFIER=urn:sharepoint:yourapplication
```

#### Dynamics CRM Configuration

**Cloud:**

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
SYNC_MODIFIED_AFTER_DATE=2026-02-07

# Optional: Number of folders to process in each batch (default: 100)
SYNC_BATCH_SIZE=100

# Optional: Index range for processing records (0-based, set both to 0 to process all)
# SYNC_END_INDEX is exclusive (so a range of Start: 10, End: 15, will include records 10, 11, 12, 13, 14)
SYNC_START_INDEX=0
SYNC_END_INDEX=0

# Optional: Dry run mode - log what would happen without making changes (default: false)
# This effectively stops the script at step 4 (skipping step 5). See Overview section.
SYNC_DRY_RUN=true

# Optional: Set the logging level of the script (default: Information)
LOG_LEVEL=Information
```

## Running the Tool

See `run-sync.example.sh` for a pre-built template.

### 1. Copy this template to `run-sync.sh`, which is git-ignored, and can be safely modified to include passwords/keys.

### 2. Update `run-sync.sh` with the relevant environment variables, secrets, and configuration settings.

### 3. First, test with the dry run flag enabled.

#### 3a. Update `run-sync.sh`

```shell
export SYNC_DRY_RUN="true"
```

#### 3b. Execute `run-sync.sh` in terminal

```bash
$ sh run-sync.sh
```

#### 3c. Review the generated csv files and ensure the content looks correct.

CSV Files will be generated each time you run the script.
These include all of the records that the script either would have added (if dry run is true) or did add (if dry run is false).

```
sharepoint-sync-account-20260306-122510.csv
sharepoint-sync-account-errors-20260306-122510.csv
```

### 4. Once you've verified the dry run output, remove the dry run flag and run for real.

#### 4a. Update `run-sync.sh`

```shell
export SYNC_DRY_RUN="true"
```

#### 4b. Execute `run-sync.sh` in terminal

```bash
$ sh run-sync.sh
```

### 5. Run for each entity type.

There are 

## Nested Folders

There are some entity types in Dynamics, for which the matching SharePoint folder is not created under a top-level library, but instead is nested under the Account library.

These include:

- Contraventions
- Enforcement Actions
- Incidents

This script specifically takes this into account, and will check and generate all Document Location records for these types, including any parent records.
