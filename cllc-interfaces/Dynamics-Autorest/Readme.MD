# Autorest for Microsoft Dynamics #


### Environment ###

#### Node.js ####
1. Install Node.js (at least version 8)

#### Odata-Openapi

Download and install odata-openapi from https://github.com/oasis-tcs/odata-openapi.git

#### Odata.OpenAPi



Note that there are many dependencies.  View the file transform.cmd for a list of them all.

#### API-Spec-Converter ####
You can use this tool to convert from Openapi V3 to V2.  Autorest currently requires V2.

1. `npm install -g api-spec-converter`

#### Autorest ####
1. npm install -g autorest
2. autorest --reset
	1. This will remove any old model / language modules that may be present.
3. Find the location where your autorest.cmd / autorest shell script is located.  Edit the file to increase the amount of RAM available to autorest.  For example, on Windows, it will be:

```

@IF EXIST "%~dp0\node.exe" (
  "%~dp0\node.exe" "--max-old-space-size=4096" "%~dp0\node_modules\autorest\dist\app.js" %*
) ELSE (
  @SETLOCAL
  @SET PATHEXT=%PATHEXT:;.JS;=;%
  node --max-old-space-size=4096 "%~dp0\node_modules\autorest\dist\app.js" %*
)
```

On Windows 10 this file is located in `C:\users\<username>\AppData\Roaming\npm\autorest.cmd`

If you need to patch the Autorest modeler, for example if the modeler is not able to parse a Microsoft Dynamics API with empty base classes, it will be located in `C:\users\<username>\.autorest\@microsoft.azure_autorest.modeler@2.3.55\node_modules\@microsoft.azure\autorest.modeler\src\bin\netcoreapp2.0` (substitute 2.3.55 for the current version of the modeler)

### Process ###

1. You will need lots of RAM for this operation.  Close all open programs including Visual Studio, Chrome and Docker.  Greater than 9 GB free is required.

2. Transfer the Dynamics meta data to your PC


3. Use the odata-openapi transform command to transform the metadata file into openapi.  It will emit a few errors however the file will be valid.

3.1 ALTERNATE:
use Odata.OpenAPI to convert the file
(from the OoasUtil folder)
`dotnet run --file \tmp\meta-data\metadata.xml -j -o dynamics.json`

4. Convert the openapi v3 file into openapi v2 using api-spec-converter   
`api-spec-converter dynamics.json --from=openapi_3 --to=swagger_2 > dynamics-2.json`
5. Use this file as an input to autorest.

```
autorest --verbose --input-file=dynamics-2.1.json --output-folder=Dynamics-Autorest  --csharp --use-datetimeoffset --generate-empty-classes --override-client-name=DynamicsClient  --namespace=Gov.Lclb.Cllb.Interfaces --preview  --add-credentials --debug
```

