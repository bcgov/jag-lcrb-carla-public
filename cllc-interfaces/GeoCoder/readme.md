# GeoCoder
> see https://aka.ms/autorest

This is the AutoRest configuration file for the APIs.

---
### Useful tools

https://www.json2yaml.com - Converts between json and yaml.

https://apidevtools.org/swagger-parser/online/ - openapi validator

https://roger13.github.io/SwagDefGen/ - convert raw json into OpenAPI 

#### Basic Information 
These are the global settings for APIs.

``` yaml
# list all the input OpenAPI files (may be YAML, JSON, or Literate- OpenAPI markdown)
input-file:
  - geocoder.json   

csharp:
  output-folder: .
  add-credentials: true
  override-client-name: GeocoderClient
  use-datetimeoffset: true 
  sync-methods: all 
  generate-empty-classes: true
  namespace: Gov.Lclb.Cllb.Interfaces.GeoCoder

# this allows you to programatically tweak the swagger file before it is modeled.
directive:
  - from: swagger-document # do it globally 
    where: $.paths.*.* 
  
    # set each operationId to 'GeoCoderAPI<Tag>'
    transform: $.operationId = `GeoCoderAPI_${$.tags[0]}`
  
```