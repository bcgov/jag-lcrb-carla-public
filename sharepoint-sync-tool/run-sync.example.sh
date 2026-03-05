#!/bin/bash

# SharePoint to Dynamics Sync Tool - Example Script
# Copy this file to run-sync.sh and update with your values
# DO NOT commit run-sync.sh with real credentials to source control!

# SharePoint Configuration (On-Premise)
export SHAREPOINT_ODATA_URI=""
export SHAREPOINT_NATIVE_BASE_URI=""
export SHAREPOINT_USERNAME=""
export SHAREPOINT_PASSWORD=''
export SHAREPOINT_STS_TOKEN_URI=""
export SHAREPOINT_RELYING_PARTY_IDENTIFIER=""

# Dynamics Configuration (Cloud)
export DYNAMICS_ODATA_URI=""
export DYNAMICS_NATIVE_ODATA_URI=""
export DYNAMICS_AAD_TENANT_ID=""
export DYNAMICS_SERVER_APP_ID_URI=""
export DYNAMICS_APP_REG_CLIENT_ID=""
export DYNAMICS_APP_REG_CLIENT_KEY=''

# Logging Configuration
export LOG_LEVEL="Debug"  # Options: Trace, Debug, Information, Warning, Error, Critical, None (for silent)

# Sync Configuration - Account
# export SYNC_ENTITY_NAME="account"
# export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
# export SYNC_DRY_RUN="true
# export SYNC_BATCH_SIZE="100"
# export SYNC_START_INDEX="0"
# export SYNC_END_INDEX="0"

# Sync Configuration - Application
# export SYNC_ENTITY_NAME="application"
# export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
# export SYNC_DRY_RUN="true
# export SYNC_BATCH_SIZE="100"
# export SYNC_START_INDEX="0"
# export SYNC_END_INDEX="0"

# Sync Configuration - Licence
# export SYNC_ENTITY_NAME="licence"
# export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
# export SYNC_DRY_RUN="true
# export SYNC_BATCH_SIZE="100"
# export SYNC_START_INDEX="0"
# export SYNC_END_INDEX="0"

# Sync Configuration - Special Event
export SYNC_ENTITY_NAME="special event"
export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
export SYNC_DRY_RUN="true
export SYNC_BATCH_SIZE="100"
export SYNC_START_INDEX="0"
export SYNC_END_INDEX="0"

# Sync Configuration - Contact
# export SYNC_ENTITY_NAME="contact"
# export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
# export SYNC_DRY_RUN="true
# export SYNC_BATCH_SIZE="100"
# export SYNC_START_INDEX="0"
# export SYNC_END_INDEX="0"

# Sync Configuration - Worker
# export SYNC_ENTITY_NAME="worker"
# export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
# export SYNC_DRY_RUN="true
# export SYNC_BATCH_SIZE="100"
# export SYNC_START_INDEX="0"
# export SYNC_END_INDEX="0"

# Sync Configuration - Event
# export SYNC_ENTITY_NAME="event"
# export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
# export SYNC_DRY_RUN="true
# export SYNC_BATCH_SIZE="100"
# export SYNC_START_INDEX="0"
# export SYNC_END_INDEX="0"

# Sync Configuration - Complaint
# export SYNC_ENTITY_NAME="complaint"
# export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
# export SYNC_DRY_RUN="true
# export SYNC_BATCH_SIZE="100"
# export SYNC_START_INDEX="0"
# export SYNC_END_INDEX="0"

# Sync Configuration - Incident (nested under Account)
# export SYNC_ENTITY_NAME="incident"
# export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
# export SYNC_DRY_RUN="true
# export SYNC_BATCH_SIZE="100"
# export SYNC_START_INDEX="0"
# export SYNC_END_INDEX="0"

# Sync Configuration - Contravention (nested under Account)
# export SYNC_ENTITY_NAME="contravention"
# export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
# export SYNC_DRY_RUN="true
# export SYNC_BATCH_SIZE="100"
# export SYNC_START_INDEX="0"
# export SYNC_END_INDEX="0"

# Sync Configuration - Enforcement Action (nested under Account)
# export SYNC_ENTITY_NAME="enforcement action"
# export SYNC_MODIFIED_AFTER_DATE="2026-02-06"
# export SYNC_DRY_RUN="true
# export SYNC_BATCH_SIZE="100"
# export SYNC_START_INDEX="0"
# export SYNC_END_INDEX="0"

# Run the tool
dotnet run --project sharepoint-sync-tool.csproj
