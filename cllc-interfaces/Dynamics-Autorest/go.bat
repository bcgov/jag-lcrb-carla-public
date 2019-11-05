@echo off

echo Updating meta data

dotnet run -p ..\OData.OpenAPI\odata2openapi\odata2openapi.csproj

echo Updating client

autorest --azure-arm --debug --verbose Readme.md