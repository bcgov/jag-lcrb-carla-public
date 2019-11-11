# Autorest for Microsoft Dynamics #


### Environment ###

#### Node.js ####
1. Install Node.js (at least version 8)

#### Odata-Openapi

A custom version of Odata-OpenAPI is included with the source code.

#### Autorest ####
1. npm install -g autorest
2. autorest --reset
	1. This will remove any old model / language modules that may be present.
3. Find the location where your autorest.cmd / autorest shell script is located.  Edit the file to increase the amount of RAM available to autorest.  For example, on Windows, it will be:

```

@IF EXIST "%~dp0\node.exe" (
  "%~dp0\node.exe" "--max-old-space-size=5192" "%~dp0\node_modules\autorest\dist\app.js" %*
) ELSE (
  @SETLOCAL
  @SET PATHEXT=%PATHEXT:;.JS;=;%
  node --max-old-space-size=5192 "%~dp0\node_modules\autorest\dist\app.js" %*
)
```

On Windows 10 this file is located in `C:\users\<username>\AppData\Roaming\npm\autorest.cmd`

If you need to patch the Autorest modeler, for example if the modeler is not able to parse a Microsoft Dynamics API with empty base classes, it will be located in `C:\users\<username>\.autorest\@microsoft.azure_autorest.modeler@2.3.55\node_modules\@microsoft.azure\autorest.modeler\src\bin\netcoreapp2.0` (substitute 2.3.55 for the current version of the modeler)

### Process ###

1. You will need lots of RAM for this operation.  Close all open programs including Visual Studio, Chrome and Docker.  Greater than 9 GB free is required; 32 GB of RAM is recommended.

2. Run the go.bat file in this directory

It will fetch the latest meta data (assuming your PC has correct secrets for the DEV environment, the same secrets required to run the portal application) and then run Autorest with the correct parameters.

``` yaml
# list all the input OpenAPI files (may be YAML, JSON, or Literate- OpenAPI markdown)
input-file:
  - dynamics-swagger.json

csharp:
  output-folder: .
  add-credentials: true
  override-client-name: DynamicsClient
  use-datetimeoffset: true 
  sync-methods: all 
  generate-empty-classes: true
  namespace: Gov.Lclb.Cllb.Interfaces
  azure-arm: false
  static-initializer: true
```