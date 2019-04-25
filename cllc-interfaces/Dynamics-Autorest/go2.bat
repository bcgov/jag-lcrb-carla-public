@echo off

echo Updating client

autorest --verbose --input-file=dynamics-swagger.json --output-folder=.  --csharp --use-datetimeoffset --generate-empty-classes --override-client-name=DynamicsClient  --namespace=Gov.Lclb.Cllb.Interfaces --preview  --add-credentials --debug