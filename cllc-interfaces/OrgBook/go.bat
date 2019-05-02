@echo off

echo Updating meta data

rem curl -o orgbook-swagger.json --header "Accept: application/json,*/*" https://orgbook.gov.bc.ca/api/v2/?format=openapi

echo Ajusting meta data

rem dotnet run -p ..\OData.OpenAPI\OrgBookCleaner\OrgBookCleaner.csproj

echo Updating client

autorest --verbose --input-file=orgbook-swagger-simplified.json --output-folder=.  --csharp --use-datetimeoffset --generate-empty-classes --override-client-name=OrgBookClient  --namespace=Gov.Lclb.Cllb.Interfaces --preview --add-credentials --debug