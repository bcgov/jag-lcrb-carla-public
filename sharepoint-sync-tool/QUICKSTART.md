# Quick Start Guide

## Before You Begin

**IMPORTANT**: This tool supports:
- **On-Premises SharePoint** with ADFS authentication
- **Cloud Dynamics CRM** (Dataverse) with AAD authentication

## Quick Setup

1. **Copy the example script**:
   - Linux/Mac: Copy `run-sync.example.sh` to `run-sync.sh`

2. **Edit your script** with your actual credentials and configuration

3. **Test with dry run first**:
   - Set `SYNC_DRY_RUN=true`
   - Run the script to see what would happen

4. **Run for real**:
   - Set `SYNC_DRY_RUN=false`
   - Run the script to actually create document locations

## What You Need

### SharePoint (On-Premises with ADFS)

- `SHAREPOINT_ODATA_URI` - Your SharePoint API endpoint
- `SHAREPOINT_NATIVE_BASE_URI` - Your SharePoint site URL
- `SHAREPOINT_USERNAME` - Service account username
- `SHAREPOINT_PASSWORD` - Service account password
- `SHAREPOINT_STS_TOKEN_URI` - STS token endpoint
- `SHAREPOINT_RELYING_PARTY_IDENTIFIER` - Relying party identifier

### Dynamics CRM (Cloud Dataverse AAD)

- `DYNAMICS_ODATA_URI` - Your Dynamics instance URL
- `DYNAMICS_AAD_TENANT_ID` - Azure AD tenant ID
- `DYNAMICS_SERVER_APP_ID_URI` - Dynamics server app ID URI
- `DYNAMICS_APP_REG_CLIENT_ID` - App registration client ID
- `DYNAMICS_APP_REG_CLIENT_KEY` - App registration client secret

### Sync Settings

- `SYNC_ENTITY_NAME` - Entity type: `account`, `application`, `contact`, `worker`, `event`, `licence`, `contravention`, or `enforcement action`
  - **Note**: `contravention` and `enforcement action` are nested under Account folders in SharePoint
- `SYNC_DOCUMENT_LIBRARY` - SharePoint library name (e.g., `adoxio_application`)
  - **Note**: Not used for `contravention` or `enforcement action` (automatically uses Account folders)
- `SYNC_MODIFIED_AFTER_DATE` - Optional: Filter by date (e.g., `2024-01-01`)
- `SYNC_BATCH_SIZE` - Optional: Folders per batch (default: 100)
- `SYNC_DRY_RUN` - Test mode: `true` or `false`

## Example Command

### Linux/Mac

```bash
chmod +x run-sync.sh
./run-sync.sh
```

## Safety Tips

✅ **Always test with dry run first**
✅ **Start with a small batch size (e.g., 10-20)**
✅ **Back up your Dynamics data before running**
✅ **Review the logs carefully**
✅ **Never commit credentials to source control**

## Need Help?

See the full [README.md](README.md) for detailed documentation.
