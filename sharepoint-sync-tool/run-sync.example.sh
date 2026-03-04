#!/bin/bash

# SharePoint to Dynamics Sync Tool - Example Script
# Copy this file to run-sync.sh and update with your values
# DO NOT commit run-sync.sh with real credentials to source control!

# SharePoint Configuration (On-Premises only)
export SHAREPOINT_ODATA_URI="https://lcrb-cllce-sp.dev.jag.gov.bc.ca/"
export SHAREPOINT_NATIVE_BASE_URI="https://lcrb-cllce-sp.dev.jag.gov.bc.ca/"
export SHAREPOINT_USERNAME="CLLCDEV@IDIR"
export SHAREPOINT_PASSWORD=""
export SHAREPOINT_STS_TOKEN_URI="https://ststest.gov.bc.ca/adfs/services/trust/2005/UsernameMixed"
export SHAREPOINT_RELYING_PARTY_IDENTIFIER="urn:spcrm:lcrb:cllce"

# Dynamics Configuration (Cloud Dataverse AAD only)
export DYNAMICS_ODATA_URI="https://wsgw.dev.jag.gov.bc.ca/clb/crmclouddev-v9/api/data/v9.2/"
export DYNAMICS_NATIVE_ODATA_URI="https://wsgw.dev.jag.gov.bc.ca/clb/crmclouddev-v9/api/data/v9.2/"
export DYNAMICS_AAD_TENANT_ID="6fdb5200-3d0d-4a8a-b036-d3685e359adc"
export DYNAMICS_SERVER_APP_ID_URI="https://lcrb-carla-dev-jag.crm3.dynamics.com/"
export DYNAMICS_APP_REG_CLIENT_ID="c3bbda2a-3453-4b72-b9de-42b99df03816"
export DYNAMICS_APP_REG_CLIENT_KEY=""

# Sync Configuration
export SYNC_ENTITY_NAME="contravention"            # account, application, contact, worker, event, licence, contravention, enforcement action
export SYNC_DOCUMENT_LIBRARY="account"             # document library name in SharePoint (not used for contravention/enforcement action)
export SYNC_MODIFIED_AFTER_DATE="2026-02-06"       # Optional: filter by creation date
export SYNC_BATCH_SIZE="100"                       # Optional: number of folders per batch
export SYNC_DRY_RUN="true"                         # Set to false to actually make changes

# Run the tool
dotnet run --project sharepoint-sync-tool.csproj
