#DataTool
Simple application that connects to the environment specified domain and can be used to clear or populate a dev database

The application may be run locally with:
dotnet build
dotnet run clean
dotnet run import
dotnet run export 

NOTE: local execution depends on the configuration of the following secrets as environment variables:
export DYNAMICS_ODATA_URI
export DYNAMICS_NATIVE_ODATA_URI
export DYNAMICS_AAD_TENANT_ID
export DYNAMICS_SERVER_APP_ID_URI
export DYNAMICS_CLIENT_KEY
export DYNAMICS_CLIENT_ID
These secrets should NEVER be committed to source control.