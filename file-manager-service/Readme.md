# File Manager Service

The purpose of the file manager service is to act as an interface to SharePoint.

## Development

File manager is a .NET 6.0 application. As such you can use an IDE such as Visual Studio or VS Code to edit the files.

## Installation

Templates for OpenShift deployment are in the `openshift` directory

## Secrets

See `file-manager-service.csproj` -> `user secrets`.

```JSON
{
  "ASPNETCORE_ENVIRONMENT": "Development",

  "JWT_TOKEN_KEY": "<token_key>",
  "JWT_VALID_AUDIENCE": "http://localhost:5000",
  "JWT_VALID_ISSUER": "http://localhost:5000",

  "SHAREPOINT_ODATA_URI": "https://bcgov.sharepoint.com/sites/lcrb-cllceDEV",
  "SHAREPOINT_WEBNAME": "lcrb-cllceDEV",
  "SHAREPOINT_AAD_TENANTID": "<tenant_id>",
  "SHAREPOINT_CLIENT_ID": "<client_id>",
  "SHAREPOINT_CLIENT_SECRET": "<client_secret>"
}
```
