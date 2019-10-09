# Autorest for Microsoft Dynamics #


### Models Only ###

This file can be used to only generate models.

Run it with `autorest ModelsOnly.MD`

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
  
directive:
  # removes all the operations from the code generation (leaves behind empty serviceclient though)
  - from: "code-model-v1" 
    where: $.operations[*]
    transform: return undefined
  
  # removes all the rest of the files that we don't want. (ie, if they say "using Models") 
  - from: source-file-csharp
    where: $
    transform: if ($.includes("using Models;")) return ""; 
```