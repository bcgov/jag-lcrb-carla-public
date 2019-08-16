@echo off

rem This batch file requires the following:
rem 1. Curl installed
rem 2. BETA version of autorest installed.
rem 2.1 npm install autorest@BETA
rem 2.2 autorest --reset

echo Updating meta data

curl https://raw.githubusercontent.com/bcgov/api-specs/master/geocoder/geocoder-combined.json > geocoder.json

echo Updating client

autorest --verbose --input-file=geocoder.json --output-folder=.  --csharp --use-datetimeoffset --sync-methods=all --generate-empty-classes --override-client-name=GeocoderClient  --namespace=Gov.Lclb.Cllb.Interfaces.GeoCoder --add-credentials --debug
