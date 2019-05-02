@echo off

echo Updating meta data

curl http://spice-carla-dev.pathfinder.bcgov/swagger/v1/swagger.json > spice-carla.swagger

echo Updating client

autorest --verbose --input-file=spice-carla.swagger --output-folder=.  --csharp --use-datetimeoffset --sync-methods=all --generate-empty-classes --override-client-name=SpiceClient  --namespace=Gov.Lclb.Cllb.Interfaces.Spice --preview  --add-credentials --debug
