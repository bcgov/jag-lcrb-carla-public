@echo off

echo Updating meta data

dotnet run -p ..\OData.OpenAPI\odata2openapi\odata2openapi.csproj

echo Updating client

autorest --verbose --input-file=dynamics-swagger.json --output-folder=.  --csharp --use-datetimeoffset --generate-empty-classes --override-client-name=DynamicsClient  --namespace=Gov.Lclb.Cllb.Interfaces --preview  --add-credentials --debug