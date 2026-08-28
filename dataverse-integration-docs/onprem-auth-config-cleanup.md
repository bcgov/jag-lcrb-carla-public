# LCSD-8563: Remove on-premises auth config and dead code

## Summary

Removed on-prem ADFS and Basic auth environment variable declarations from all OpenShift
deployment JSON files. Updated `README.md` to document only the current AAD cloud-auth variables.
No `appsettings.json` files contained these variables.

---

## Deployment Files Changed

| File | Removed Variables |
|---|---|
| `cllc-public-app/openshift/.../cllc-public-api-deploy.json` | `ADFS_OAUTH2_URI`, `DYNAMICS_APP_GROUP_CLIENT_ID`, `DYNAMICS_APP_GROUP_RESOURCE`, `DYNAMICS_APP_GROUP_SECRET`, `DYNAMICS_PASSWORD`, `DYNAMICS_USERNAME`, `SSG_USERNAME`, `SSG_PASSWORD` |
| `carla-spice-sync-service/openshift/.../carla-spice-sync-service-deploy.json` | `ADFS_OAUTH2_URI`, `DYNAMICS_APP_GROUP_CLIENT_ID`, `DYNAMICS_APP_GROUP_RESOURCE`, `DYNAMICS_APP_GROUP_SECRET`, `DYNAMICS_PASSWORD`, `DYNAMICS_USERNAME` |
| `geocoder-service/openshift/.../geocoder-service-deploy.json` | `SSG_USERNAME`, `ADFS_OAUTH2_URI`, `DYNAMICS_APP_GROUP_CLIENT_ID`, `DYNAMICS_APP_GROUP_RESOURCE`, `DYNAMICS_APP_GROUP_SECRET`, `DYNAMICS_PASSWORD`, `DYNAMICS_USERNAME` |
| `one-stop-service/openshift/.../one-stop-service-deploy.json` | `ADFS_OAUTH2_URI`, `DYNAMICS_APP_GROUP_CLIENT_ID`, `DYNAMICS_APP_GROUP_RESOURCE`, `DYNAMICS_APP_GROUP_SECRET`, `DYNAMICS_PASSWORD`, `DYNAMICS_USERNAME` |
| `federal-reporting-service/openshift/.../federal-reporting-service.deploy.json` | `DYNAMICS_PASSWORD`, `DYNAMICS_USERNAME`, `SSG_USERNAME`, `SSG_PASSWORD` |
| `file-manager-service/openshift/.../file-manager-service-deploy.json` | `SSG_USERNAME`, `SSG_PASSWORD` |
| `orgbook-service/openshift/.../orgbook-service.deploy.json` | `SSG_USERNAME`, `SSG_PASSWORD` |
| `README.md` | Replaced on-prem variable list with current AAD cloud-auth variables |

---

## Variables Removed

| Variable | Auth Path | Reason |
|---|---|---|
| `ADFS_OAUTH2_URI` | On-prem ADFS | Cloud Dataverse uses AAD, not ADFS |
| `DYNAMICS_APP_GROUP_CLIENT_ID` | On-prem ADFS | Replaced by `DYNAMICS_APP_REG_CLIENT_ID` |
| `DYNAMICS_APP_GROUP_RESOURCE` | On-prem ADFS | Not used in AAD auth |
| `DYNAMICS_APP_GROUP_SECRET` | On-prem ADFS | Replaced by `DYNAMICS_APP_REG_CLIENT_KEY` |
| `DYNAMICS_USERNAME` | On-prem ADFS | No credential-based auth in Dataverse SDK |
| `DYNAMICS_PASSWORD` | On-prem ADFS | No credential-based auth in Dataverse SDK |
| `BYPASS_STS_CERT_VALIDATION` | On-prem ADFS | No STS endpoint in cloud path (not found in any deploy JSON — only in SharePoint library code for on-prem SharePoint) |
| `SSG_USERNAME` | Basic/API gateway auth | Not consumed in service code; flows only into `OnPremSharePointFileManager` which is cloud-replaced |
| `SSG_PASSWORD` | Basic/API gateway auth | Same as SSG_USERNAME |

---

## Variables Kept

| Variable | Reason |
|---|---|
| `DYNAMICS_ODATA_URI` | Required by DataverseClient for organization service URL |
| `DYNAMICS_NATIVE_ODATA_URI` | Required for native Dynamics OData calls |
| `DYNAMICS_AAD_TENANT_ID` | Required for AAD app auth |
| `DYNAMICS_APP_REG_CLIENT_ID` | Required for AAD app auth |
| `DYNAMICS_APP_REG_CLIENT_KEY` | Required for AAD app auth |
| `SHAREPOINT_SSG_USERNAME`, `SHAREPOINT_SSG_PASSWORD` | Still used in `OnPremSharePointFileManager` for on-prem SharePoint Basic auth (SharePoint migration is out of scope for this epic) |
</content>
